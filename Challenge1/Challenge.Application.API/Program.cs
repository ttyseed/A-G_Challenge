using Amazon;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using challenge1.Common.EmailService;
using challenge1.Common.EmailService.Classes;
using challenge1.Common.EmailService.Classes.Interfaces;
using challenge1.Common.EmailService.Services;
using challenge1.Common.Extension;
using challenge1.Common.Util.Common;
using challenge1.Common.Util.Middleware;
using challenge1.Application.API.BackgroundServices;
using challenge1.Application.API.DataSeeder;
using challenge1.Application.Bll.Auth;
using challenge1.Database.Repositories.Context;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.RateLimiting;
using static challenge1.Common.EmailService.EmailServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

#region Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = false;
    options.SingleLine = true;
    options.TimestampFormat = null; // no timestamp
});

// Suppress default HttpClient logs
builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.Extensions.Http", LogLevel.Warning);
#endregion Configure Logging

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "challenge1 Weather API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(_ =>
    {
        var req = new OpenApiSecurityRequirement();
        req.Add(new OpenApiSecuritySchemeReference("Bearer"), new List<string>());
        return req;
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.SuppressXFrameOptionsHeader = true;
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.IsAuthenticated == true ? httpContext.User.Identity.Name! : httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100,
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(1)
                        }));
});

await Secrets.InitializeAsync();

#region Connection String Setup
var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    if (isDevelopment)
    {
        // Self-hosted SQLite — no external DB required in development
        var dbPath = Path.Combine(AppContext.BaseDirectory, "dev.db");
        options.UseSqlite($"Data Source={dbPath}");
    }
    else
    {
        var connectionString = Secrets.GetSecret("ConnectionStrings");
        options.UseNpgsql(connectionString);
    }
});
#endregion Connection String Setup

builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.ListenAnyIP(3000);
    options.ListenAnyIP(3080, listenOptions =>
    {
        listenOptions.UseHttps(httpsOptions =>
        {
            httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls13;
            httpsOptions.ServerCertificate = X509CertificateLoader.LoadPkcs12FromFile(Path.Combine(AppContext.BaseDirectory, "cert.pfx"), Environment.GetEnvironmentVariable("CERT_PASSWORD"));
        });
    });
});

#region JWT Authentication
var jwtSecret = isDevelopment
    ? builder.Configuration["Jwt:Secret"]!
    : Secrets.GetSecret("jwt_secret") ?? builder.Configuration["Jwt:Secret"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();
#endregion JWT Authentication

builder.Services.AddCustomServices();

#region Weather Alert Background Service
builder.Services.AddHostedService<WeatherAlertBackgroundService>();
#endregion Weather Alert Background Service


// Orders the middleware correctly, builds the app, and runs it. Orders are important in ASP.NET Core.
var app = builder.Build();

#region Auto-migrate and seed database on startup (Development only)
if (isDevelopment)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.EnsureCreated();
    await WeatherDataSeeder.SeedAsync(db);
    await AuthDataSeeder.SeedAsync(db);
}
#endregion

// Development tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// MUST be first
app.UseForwardedHeaders();

// Enforce HTTPS
app.UseHttpsRedirection();

// Static files (wwwroot)
app.UseStaticFiles();

// Routing (REQUIRED)
app.UseRouting();

// Rate limiting (needs routing)
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Security headers
app.UseXfo(options => options.SameOrigin());
app.UseReferrerPolicy(opts => opts.NoReferrer());
app.UseCsp(options => options
    .DefaultSources(s => s.Self())
    .ScriptSources(s => s.Self().UnsafeInline().UnsafeEval())
    .ObjectSources(s => s.None())
    .StyleSources(s => s.Self().UnsafeInline())
    .BaseUris(s => s.None())
);

app.UseMiddleware<AuditContextMiddleware>();

app.MapControllers();

app.Run();
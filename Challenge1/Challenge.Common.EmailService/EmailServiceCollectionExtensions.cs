using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using challenge1.Common.EmailService.Classes;
using challenge1.Common.EmailService.Classes.Interfaces;
using challenge1.Common.EmailService.Data;
using challenge1.Common.EmailService.Repositories;
using challenge1.Common.EmailService.Repositories.Interfaces;
using challenge1.Common.EmailService.Services;
using challenge1.Common.EmailService.Services.Interfaces;
using static challenge1.Common.EmailService.Classes.Constant;

namespace challenge1.Common.EmailService
{
    public static class EmailServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailService(
            this IServiceCollection services,
            string provider,
            string connectionString)
        {
            if (provider.Equals(DbProviders.PostgreSQL, StringComparison.OrdinalIgnoreCase))
            {
                services.AddDbContext<EmailDbContext>(options =>
                    options.UseNpgsql(connectionString));
            }
            else if (provider.Equals(DbProviders.SqlServer, StringComparison.OrdinalIgnoreCase))
            {
                services.AddDbContext<EmailDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }
            else
            {
                throw new ArgumentException($"Unsupported provider: {provider}");
            }

            // Repositories
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IEmailAttachmentRepository, EmailAttachmentRepository>();
            services.AddScoped<IEmailLogRepository, EmailLogRepository>();

            // Services
            services.AddScoped<IEmailAttachmentService, EmailAttachmentService>();
            services.AddScoped<IEmailService, Services.EmailService>();
            services.AddScoped<IEmailLogService, EmailLogService>();

            // Classes
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IS3Helper, S3Helper>();
            services.AddScoped<ILogging, Logging>();
            services.AddScoped<IUtilities, Utilities>();

            // AutoMapper
            services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());

            return services;
        }
    }
}

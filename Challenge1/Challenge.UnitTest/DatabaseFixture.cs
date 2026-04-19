using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using challenge1.Application.Bll.Common;
using challenge1.Application.Bll.Common.Interfaces;
using challenge1.Application.Bll.Weather;
using challenge1.Application.Bll.Weather.Interfaces;
using challenge1.Common.Logging;
using challenge1.Common.Mapper;
using challenge1.Database.Repositories.Context;
using challenge1.Database.Repositories.Repositories;
using challenge1.Database.Repositories.Repositories.Interfaces;
using challenge1.Database.Repositories.Repositories.Weather;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.UnitTest
{
    /// <summary>
    /// Shared fixture that wires up a fresh SQLite in-memory database per test run.
    /// Keeps a single open SqliteConnection alive so the schema persists across DI scopes.
    /// No external database or network connection required — safe for CI.
    /// </summary>
    public class DatabaseFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }
        public DatabaseContext DbContext { get; }

        // Keep one connection open for the lifetime of the fixture so the
        // SQLite in-memory database isn't destroyed between DI scope boundaries.
        private readonly SqliteConnection _connection;

        public DatabaseFixture()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var services = new ServiceCollection();

            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlite(_connection));

            services.AddAutoMapper(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
                cfg.DisableConstructorMapping();
                cfg.ShouldMapProperty = p => p.GetMethod?.IsPublic == true;
            }, typeof(MapperConfig));

            services.AddLogging();
            services.AddScoped<ILogging, challenge1.Common.Logging.Logging>();

            // Repositories
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<IBackendUserRepository, BackendUserRepository>();
            services.AddScoped<ILoginUserRepository, LoginUserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IWeatherLocationRepository, WeatherLocationRepository>();
            services.AddScoped<IWeatherRecordRepository, WeatherRecordRepository>();
            services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
            services.AddScoped<IWeatherAlertRepository, WeatherAlertRepository>();

            // BLLs (WeatherBll is instantiated directly in tests to inject mock HTTP clients)
            services.AddScoped<IAuditBll, AuditBll>();
            services.AddScoped<IWeatherAlertBll, WeatherAlertBll>();

            ServiceProvider = services.BuildServiceProvider();

            // Use a dedicated scope for setup so the DbContext and connection
            // are not shared with test code (avoids change-tracker pollution).
            using var setupScope = ServiceProvider.CreateScope();
            var db = setupScope.ServiceProvider.GetRequiredService<DatabaseContext>();
            db.Database.EnsureCreated();

            // Expose a DbContext via its own scope for direct assertions in tests.
            var testScope = ServiceProvider.CreateScope();
            DbContext = testScope.ServiceProvider.GetRequiredService<DatabaseContext>();
        }

        public void Dispose()
        {
            ServiceProvider.Dispose();
            _connection.Dispose();
        }
    }
}

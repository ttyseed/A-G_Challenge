using Microsoft.EntityFrameworkCore;
using challenge1.Database.Models;
using challenge1.Database.Models.Weather;

namespace challenge1.Database.Repositories.Context
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<BackendUser> BackendUsers { get; set; } = null!;
		public DbSet<LoginUser> LoginUsers { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
		public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<Audit> Audits { get; set; } = null!;

        // Weather
        public DbSet<WeatherLocation> WeatherLocations { get; set; } = null!;
        public DbSet<WeatherRecord> WeatherRecords { get; set; } = null!;
        public DbSet<WeatherForecast> WeatherForecasts { get; set; } = null!;
        public DbSet<WeatherAlert> WeatherAlerts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Constraints & relationships
            modelBuilder.Entity<BackendUser>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("pk_backend_user");

                entity.HasMany(e => e.UserRoles)
                      .WithOne(a => a.BackendUser)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_role");

                entity.HasKey(e => e.UserRoleId);

                entity.HasOne(ur => ur.BackendUser)
                      .WithMany(bu => bu.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .HasConstraintName("fk_user_role_backend_user");

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleNumber)
                      .HasPrincipalKey(r => r.RoleNumber)
                      .HasConstraintName("fk_user_role_role");
            });


            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");
                entity.HasKey(e => e.RoleId).HasName("pk_role");

                entity.HasAlternateKey(e => e.RoleNumber);
            });

            modelBuilder.Entity<Audit>(entity =>
            {
                entity.ToTable("audit");
                entity.HasKey(e => e.AuditId).HasName("pk_audit");
            });

            modelBuilder.Entity<LoginUser>(entity =>
            {
                entity.ToTable("login_user");
                entity.HasKey(e => e.LoginUserId).HasName("pk_login_user");
            });

            modelBuilder.Entity<WeatherLocation>(entity =>
            {
                entity.ToTable("weather_location");
                entity.HasKey(e => e.LocationId).HasName("pk_weather_location");

                entity.HasMany(e => e.WeatherRecords)
                      .WithOne(r => r.WeatherLocation)
                      .HasForeignKey(r => r.LocationId)
                      .HasConstraintName("fk_weather_record_location")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.WeatherForecasts)
                      .WithOne(f => f.WeatherLocation)
                      .HasForeignKey(f => f.LocationId)
                      .HasConstraintName("fk_weather_forecast_location")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.WeatherAlerts)
                      .WithOne(a => a.WeatherLocation)
                      .HasForeignKey(a => a.LocationId)
                      .HasConstraintName("fk_weather_alert_location")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WeatherRecord>(entity =>
            {
                entity.ToTable("weather_record");
                entity.HasKey(e => e.RecordId).HasName("pk_weather_record");
            });

            modelBuilder.Entity<WeatherForecast>(entity =>
            {
                entity.ToTable("weather_forecast");
                entity.HasKey(e => e.ForecastId).HasName("pk_weather_forecast");
            });

            modelBuilder.Entity<WeatherAlert>(entity =>
            {
                entity.ToTable("weather_alert");
                entity.HasKey(e => e.AlertId).HasName("pk_weather_alert");
            });

            // SQLite does not understand PostgreSQL-specific TypeName values such as
            // "timestamp without time zone" or "decimal(5,2)". Override them so EF Core
            // uses SQLite-native storage types instead.
            if (Database.IsSqlite())
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                        {
                            property.SetColumnType("TEXT");
                        }
                        else if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                        {
                            property.SetColumnType("REAL");
                        }
                    }
                }
            }
        }
    }

}

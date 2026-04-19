using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using challenge1.Application.Bll.Auth;
using challenge1.Application.Bll.Auth.Interfaces;
using challenge1.Application.Bll.Common;
using challenge1.Application.Bll.Common.Interfaces;
using challenge1.Application.Bll.Weather;
using challenge1.Application.Bll.Weather.Interfaces;
using challenge1.Common.Logging;
using challenge1.Common.Mapper;
using challenge1.Common.Util.AWS;
using challenge1.Common.Util.AWS.Interfaces;
using challenge1.Common.Util.Common.Helpers;
using challenge1.Common.Util.Common.Helpers.Interfaces;
using challenge1.Common.Util.WeatherClients;
using challenge1.Common.Util.WeatherClients.Interfaces;
using challenge1.Database.Repositories.Repositories;
using challenge1.Database.Repositories.Repositories.Interfaces;
using challenge1.Database.Repositories.Repositories.Weather;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.Common.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {

            services.AddHttpClient("APIClient");

            // Register services here
            services.AddAutoMapper(cfg =>
            {
                // Configure mapper to ignore null values and use faster compilation
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;

                // Disable constructor mapping for better performance
                cfg.DisableConstructorMapping();

                // Use faster member access
                cfg.ShouldMapProperty = p => p.GetMethod?.IsPublic == true;
            }, typeof(MapperConfig));

            services.AddScoped<ILogging, Logging.Logging>();
            services.AddScoped<IS3Helper, S3Helper>();
            services.AddScoped<IWebServiceHelper, WebServiceHelper>();

            #region Repositories
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<IBackendUserRepository, BackendUserRepository>();
            services.AddScoped<ILoginUserRepository, LoginUserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();

            // Weather repositories
            services.AddScoped<IWeatherLocationRepository, WeatherLocationRepository>();
            services.AddScoped<IWeatherRecordRepository, WeatherRecordRepository>();
            services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
            services.AddScoped<IWeatherAlertRepository, WeatherAlertRepository>();
            #endregion Repositories

            #region BLLs
            services.AddScoped<IAuthBll, AuthBll>();
            services.AddScoped<IAuditBll, AuditBll>();
            services.AddScoped<IBackendUserBll, BackendUserBll>();
            services.AddScoped<ILoginUserBll, LoginUserBll>();
            services.AddScoped<IRoleBll, RoleBll>();
            services.AddScoped<IUserRoleBll, UserRoleBll>();

            // Weather BLLs
            services.AddScoped<IWeatherBll, WeatherBll>();
            services.AddScoped<IWeatherAlertBll, WeatherAlertBll>();
            #endregion BLLs

            #region Weather HTTP Clients
            services.AddHttpClient<IDataGovSgClient, DataGovSgClient>();
            services.AddHttpClient<IOpenWeatherMapClient, OpenWeatherMapClient>();
            #endregion Weather HTTP Clients
        }
    }
}
using Microsoft.EntityFrameworkCore;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Logging;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Context;
using challenge1.Database.Repositories.Repositories.Base;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.Database.Repositories.Repositories.Weather
{
    public class WeatherAlertRepository : EntityFrameworkCoreRepository<WeatherAlert>, IWeatherAlertRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogging _logging;

        public WeatherAlertRepository(DatabaseContext context, ILogging logging) : base(context, logging)
        {
            _context = context;
            _logging = logging;
        }

        public async Task<List<WeatherAlert>?> GetAlertsAsync(GetWeatherAlertsFilter request)
        {
            try
            {
                return await _context.WeatherAlerts.AsNoTracking()
                    .Where(a =>
                        (!request.LocationId.HasValue || a.LocationId == request.LocationId)
                        && (!request.IsActive.HasValue || a.IsActive == request.IsActive)
                        && (!request.IsDeleted.HasValue || a.IsDeleted == request.IsDeleted)
                        && (string.IsNullOrEmpty(request.SubscriberEmail) || a.SubscriberEmail == request.SubscriberEmail)
                    )
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
            }

            return null;
        }

        public async Task<List<WeatherAlert>?> GetActiveAlertsForCheckAsync()
        {
            try
            {
                return await _context.WeatherAlerts.AsNoTracking()
                    .Where(a => a.IsActive && !a.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
            }

            return null;
        }
    }
}

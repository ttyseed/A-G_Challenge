using Microsoft.EntityFrameworkCore;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Logging;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Context;
using challenge1.Database.Repositories.Repositories.Base;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.Database.Repositories.Repositories.Weather
{
    public class WeatherForecastRepository : EntityFrameworkCoreRepository<WeatherForecast>, IWeatherForecastRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogging _logging;

        public WeatherForecastRepository(DatabaseContext context, ILogging logging) : base(context, logging)
        {
            _context = context;
            _logging = logging;
        }

        public async Task<List<WeatherForecast>?> GetForecastsAsync(GetWeatherForecastsFilter request)
        {
            try
            {
                var cutoff = request.DateFrom ?? DateTime.UtcNow.Date;
                var upperBound = request.DateTo
                    ?? (request.Days.HasValue ? cutoff.AddDays(request.Days.Value) : cutoff.AddDays(4));

                return await _context.WeatherForecasts.AsNoTracking()
                    .Where(f =>
                        (!request.LocationId.HasValue || f.LocationId == request.LocationId)
                        && f.ForecastDate >= cutoff
                        && f.ForecastDate <= upperBound
                    )
                    .OrderBy(f => f.ForecastDate)
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

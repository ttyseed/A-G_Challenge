using Microsoft.EntityFrameworkCore;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Logging;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Context;
using challenge1.Database.Repositories.Repositories.Base;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.Database.Repositories.Repositories.Weather
{
    public class WeatherLocationRepository : EntityFrameworkCoreRepository<WeatherLocation>, IWeatherLocationRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogging _logging;

        public WeatherLocationRepository(DatabaseContext context, ILogging logging) : base(context, logging)
        {
            _context = context;
            _logging = logging;
        }

        public async Task<List<WeatherLocation>?> GetWeatherLocationsAsync(GetWeatherLocationsFilter request)
        {
            try
            {
                return await _context.WeatherLocations.AsNoTracking()
                    .Where(o =>
                        (!request.IsDeleted.HasValue || request.IsDeleted == o.IsDeleted)
                        && (!request.IsActive.HasValue || request.IsActive == o.IsActive)
                        && (string.IsNullOrEmpty(request.Region) || o.Region == request.Region)
                        && (string.IsNullOrEmpty(request.LocationName) || EF.Functions.Like(o.LocationName, request.LocationName))
                    ).ToListAsync();
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
            }

            return null;
        }

        public async Task<WeatherLocation?> GetByNameAsync(string locationName)
        {
            try
            {
                return await FirstOrDefaultAsync(x => x.LocationName == locationName && !x.IsDeleted);
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
            }

            return null;
        }
    }
}

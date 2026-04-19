using Microsoft.EntityFrameworkCore;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Logging;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Context;
using challenge1.Database.Repositories.Repositories.Base;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.Database.Repositories.Repositories.Weather
{
    public class WeatherRecordRepository : EntityFrameworkCoreRepository<WeatherRecord>, IWeatherRecordRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogging _logging;

        public WeatherRecordRepository(DatabaseContext context, ILogging logging) : base(context, logging)
        {
            _context = context;
            _logging = logging;
        }

        public async Task<WeatherRecord?> GetLatestByLocationAsync(Guid locationId)
        {
            try
            {
                return await _context.WeatherRecords.AsNoTracking()
                    .Where(r => r.LocationId == locationId)
                    .OrderByDescending(r => r.RecordedAt)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
            }

            return null;
        }

        public async Task<List<WeatherRecord>?> GetHistoricalAsync(GetWeatherRecordsFilter request)
        {
            try
            {
                return await _context.WeatherRecords.AsNoTracking()
                    .Where(r =>
                        (!request.LocationId.HasValue || r.LocationId == request.LocationId)
                        && (!request.DateFrom.HasValue || r.RecordedAt >= request.DateFrom)
                        && (!request.DateTo.HasValue || r.RecordedAt <= request.DateTo)
                        && (string.IsNullOrEmpty(request.Source) || r.Source == request.Source)
                    )
                    .OrderByDescending(r => r.RecordedAt)
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

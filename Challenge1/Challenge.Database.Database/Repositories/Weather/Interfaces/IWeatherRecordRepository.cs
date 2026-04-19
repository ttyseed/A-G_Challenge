using challenge1.Application.Filter.Weather;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Repositories.Base;

namespace challenge1.Database.Repositories.Repositories.Weather.Interfaces
{
    public interface IWeatherRecordRepository : IRepository<WeatherRecord>
    {
        Task<WeatherRecord?> GetLatestByLocationAsync(Guid locationId);
        Task<List<WeatherRecord>?> GetHistoricalAsync(GetWeatherRecordsFilter request);
    }
}

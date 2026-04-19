using challenge1.Application.Filter.Weather;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Repositories.Base;

namespace challenge1.Database.Repositories.Repositories.Weather.Interfaces
{
    public interface IWeatherLocationRepository : IRepository<WeatherLocation>
    {
        Task<List<WeatherLocation>?> GetWeatherLocationsAsync(GetWeatherLocationsFilter request);
        Task<WeatherLocation?> GetByNameAsync(string locationName);
    }
}

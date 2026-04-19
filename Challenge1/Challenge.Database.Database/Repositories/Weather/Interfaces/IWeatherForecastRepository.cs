using challenge1.Application.Filter.Weather;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Repositories.Base;

namespace challenge1.Database.Repositories.Repositories.Weather.Interfaces
{
    public interface IWeatherForecastRepository : IRepository<WeatherForecast>
    {
        Task<List<WeatherForecast>?> GetForecastsAsync(GetWeatherForecastsFilter request);
    }
}

using challenge1.Common.Util.WeatherClients.Models;

namespace challenge1.Common.Util.WeatherClients.Interfaces
{
    public interface IOpenWeatherMapClient
    {
        Task<OpenWeatherCurrentResponse?> GetCurrentWeatherAsync(string city);
        Task<List<OpenWeatherForecastItem>?> GetFiveDayForecastAsync(string city);
    }
}

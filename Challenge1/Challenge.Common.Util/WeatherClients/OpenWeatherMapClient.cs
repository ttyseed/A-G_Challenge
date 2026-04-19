using challenge1.Common.Logging;
using challenge1.Common.Util.WeatherClients.Interfaces;
using challenge1.Common.Util.WeatherClients.Models;
using System.Text.Json;

namespace challenge1.Common.Util.WeatherClients
{
    public class OpenWeatherMapClient : IOpenWeatherMapClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogging _logging;

        private const string BaseUrl = "https://api.openweathermap.org/data/2.5";

        public OpenWeatherMapClient(HttpClient httpClient, ILogging logging)
        {
            _httpClient = httpClient;
            _logging = logging;
        }

        public async Task<OpenWeatherCurrentResponse?> GetCurrentWeatherAsync(string city)
        {
            try
            {
                var apiKey = Common.Secrets.GetSecret("openweathermap_api_key");
                var url = $"{BaseUrl}/weather?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric";
                var response = await _httpClient.GetStringAsync(url);
                return JsonSerializer.Deserialize<OpenWeatherCurrentResponse>(response);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<List<OpenWeatherForecastItem>?> GetFiveDayForecastAsync(string city)
        {
            try
            {
                var apiKey = Common.Secrets.GetSecret("openweathermap_api_key");
                var url = $"{BaseUrl}/forecast?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric";
                var response = await _httpClient.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<OpenWeatherForecastResponse>(response);
                return result?.List;
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString());
                return null;
            }
        }
    }
}

using challenge1.Application.DTO.Weather;
using challenge1.Application.Filter.Weather;

namespace challenge1.Application.Bll.Weather.Interfaces
{
    public interface IWeatherBll
    {
        Task<GetCurrentWeatherResponseDTO?> GetCurrentWeatherAsync(Guid locationId);
        Task<GetCurrentWeatherResponseDTO?> GetCurrentWeatherByNameAsync(string locationName);
        Task<List<GetWeatherForecastResponseDTO>?> GetForecastAsync(Guid locationId, int days = 4);
        Task<List<GetWeatherForecastResponseDTO>?> GetForecastByNameAsync(string locationName, int days = 4);
        Task<List<GetHistoricalWeatherResponseDTO>?> GetHistoricalAsync(GetWeatherRecordsFilter request);
        Task<string> ExportToCsvAsync(GetWeatherRecordsFilter request);
        Task<List<GetWeatherLocationsResponseDTO>?> GetLocationsAsync(GetWeatherLocationsFilter request);
        Task<Guid?> CreateLocationAsync(CreateWeatherLocationRequestDTO request);
        Task<WeatherLocationSummaryDTO?> GetLocationSummaryAsync(string locationName);
    }
}

using challenge1.Application.Filter.Weather;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Repositories.Base;

namespace challenge1.Database.Repositories.Repositories.Weather.Interfaces
{
    public interface IWeatherAlertRepository : IRepository<WeatherAlert>
    {
        Task<List<WeatherAlert>?> GetAlertsAsync(GetWeatherAlertsFilter request);
        Task<List<WeatherAlert>?> GetActiveAlertsForCheckAsync();
    }
}

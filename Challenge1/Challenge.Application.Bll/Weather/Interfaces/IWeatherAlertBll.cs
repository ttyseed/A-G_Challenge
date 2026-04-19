using challenge1.Application.DTO.Weather;
using challenge1.Application.Filter.Weather;

namespace challenge1.Application.Bll.Weather.Interfaces
{
    public interface IWeatherAlertBll
    {
        Task<List<GetWeatherAlertsResponseDTO>?> GetAlertsAsync(GetWeatherAlertsFilter request);
        Task<Guid?> SubscribeAsync(SubscribeWeatherAlertRequestDTO request);
        Task<bool> UnsubscribeAsync(UnsubscribeWeatherAlertRequestDTO request);
    }
}

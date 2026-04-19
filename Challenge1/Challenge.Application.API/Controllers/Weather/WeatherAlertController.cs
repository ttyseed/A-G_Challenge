using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using challenge1.Application.Bll.Weather.Interfaces;
using challenge1.Application.DTO.Weather;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Logging;

namespace challenge1.Application.API.Controllers.Weather
{
    [ApiController]
    [Authorize]
    [Route("Weather/[controller]")]
    public class WeatherAlertController : ControllerBase
    {
        private readonly ILogging _logging;
        private readonly IWeatherAlertBll _weatherAlertBll;

        public WeatherAlertController(ILogging logging, IWeatherAlertBll weatherAlertBll)
        {
            _logging = logging;
            _weatherAlertBll = weatherAlertBll;
        }

        [HttpPost("GetAlerts")]
        public async Task<IActionResult> GetAlertsAsync([FromBody] GetWeatherAlertsFilter request)
        {
            try
            {
                var result = await _weatherAlertBll.GetAlertsAsync(request);
                if (result != null)
                    return Ok(result);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("Subscribe")]
        public async Task<IActionResult> SubscribeAsync([FromBody] SubscribeWeatherAlertRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.SubscriberEmail) || string.IsNullOrWhiteSpace(request.AlertType))
                    return BadRequest("SubscriberEmail and AlertType are required.");

                var validAlertTypes = new[] { "temperature_high", "temperature_low", "rainfall", "aqi" };
                if (!validAlertTypes.Contains(request.AlertType))
                    return BadRequest($"AlertType must be one of: {string.Join(", ", validAlertTypes)}");

                var alertId = await _weatherAlertBll.SubscribeAsync(request);
                if (alertId != null)
                    return Ok(alertId);
                return BadRequest("Failed to create alert subscription. Verify the locationId is valid.");
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("Unsubscribe")]
        public async Task<IActionResult> UnsubscribeAsync([FromBody] UnsubscribeWeatherAlertRequestDTO request)
        {
            try
            {
                var success = await _weatherAlertBll.UnsubscribeAsync(request);
                if (success)
                    return Ok();
                return BadRequest("Failed to unsubscribe. Alert not found.");
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}

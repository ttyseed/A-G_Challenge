using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using challenge1.Application.Bll.Weather.Interfaces;
using challenge1.Application.DTO.Weather;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Logging;
using System.Text;

namespace challenge1.Application.API.Controllers.Weather
{
    [ApiController]
    [Authorize]
    [Route("Weather/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogging _logging;
        private readonly IWeatherBll _weatherBll;

        public WeatherController(ILogging logging, IWeatherBll weatherBll)
        {
            _logging = logging;
            _weatherBll = weatherBll;
        }

        /// <summary>List all seeded/created locations. Optionally filter by region or name.</summary>
        [HttpGet("Locations")]
        public async Task<IActionResult> GetLocationsAsync([FromQuery] GetWeatherLocationsFilter request)
        {
            try
            {
                var result = await _weatherBll.GetLocationsAsync(request);
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

        [HttpPost("Locations/Create")]
        public async Task<IActionResult> CreateLocationAsync([FromBody] CreateWeatherLocationRequestDTO request)
        {
            try
            {
                var locationId = await _weatherBll.CreateLocationAsync(request);
                if (locationId != null)
                    return Ok(locationId);
                return BadRequest("Failed to create weather location.");
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get current weather by locationId (Guid).
        /// Use GET /Weather/Weather/CurrentByName?locationName=Tampines for a friendlier entry point.
        /// </summary>
        [HttpGet("Current")]
        public async Task<IActionResult> GetCurrentWeatherAsync([FromQuery] Guid locationId)
        {
            try
            {
                var result = await _weatherBll.GetCurrentWeatherAsync(locationId);
                if (result != null)
                    return Ok(result);
                return NotFound("Location not found.");
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get current weather by location name (e.g. "Tampines", "Ang Mo Kio", "central").
        /// Auto-creates the location if it does not exist yet.
        /// </summary>
        [HttpGet("CurrentByName")]
        public async Task<IActionResult> GetCurrentWeatherByNameAsync([FromQuery] string locationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(locationName))
                    return BadRequest("locationName is required.");

                var result = await _weatherBll.GetCurrentWeatherByNameAsync(locationName);
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

        /// <summary>
        /// Get a combined summary (current weather + 4-day forecast) by location name.
        /// This is the recommended starting endpoint when using a fresh database.
        /// </summary>
        [HttpGet("Summary")]
        public async Task<IActionResult> GetLocationSummaryAsync([FromQuery] string locationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(locationName))
                    return BadRequest("locationName is required.");

                var result = await _weatherBll.GetLocationSummaryAsync(locationName);
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

        /// <summary>Get forecast by locationId.</summary>
        [HttpGet("Forecast")]
        public async Task<IActionResult> GetForecastAsync([FromQuery] Guid locationId, [FromQuery] int days = 4)
        {
            try
            {
                if (days < 1 || days > 7)
                    return BadRequest("Days must be between 1 and 7.");

                var result = await _weatherBll.GetForecastAsync(locationId, days);
                if (result != null)
                    return Ok(result);
                return NotFound("Location not found.");
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>Get forecast by location name. Auto-creates location if not found.</summary>
        [HttpGet("ForecastByName")]
        public async Task<IActionResult> GetForecastByNameAsync([FromQuery] string locationName, [FromQuery] int days = 4)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(locationName))
                    return BadRequest("locationName is required.");

                if (days < 1 || days > 7)
                    return BadRequest("Days must be between 1 and 7.");

                var result = await _weatherBll.GetForecastByNameAsync(locationName, days);
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

        [HttpPost("Historical")]
        public async Task<IActionResult> GetHistoricalAsync([FromBody] GetWeatherRecordsFilter request)
        {
            try
            {
                var result = await _weatherBll.GetHistoricalAsync(request);
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

        [HttpPost("Export")]
        public async Task<IActionResult> ExportToCsvAsync([FromBody] GetWeatherRecordsFilter request)
        {
            try
            {
                var csvContent = await _weatherBll.ExportToCsvAsync(request);
                if (string.IsNullOrEmpty(csvContent))
                    return NotFound("No data found for the given filter.");

                var bytes = Encoding.UTF8.GetBytes(csvContent);
                var fileName = $"weather_export_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}

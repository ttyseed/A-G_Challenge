using AutoMapper;
using challenge1.Application.Bll.Weather.Interfaces;
using challenge1.Application.DTO.Weather;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Logging;
using challenge1.Common.Util.Common;
using challenge1.Common.Util.WeatherClients.Interfaces;
using challenge1.Common.Util.WeatherClients.Models;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;
using System.Text;

namespace challenge1.Application.Bll.Weather
{
    public class WeatherBll : IWeatherBll
    {
        private readonly IMapper _mapper;
        private readonly ILogging _logging;
        private readonly IWeatherLocationRepository _locationRepo;
        private readonly IWeatherRecordRepository _recordRepo;
        private readonly IWeatherForecastRepository _forecastRepo;
        private readonly IDataGovSgClient _dataGovSgClient;
        private readonly IOpenWeatherMapClient _openWeatherMapClient;

        public WeatherBll(
            IMapper mapper,
            ILogging logging,
            IWeatherLocationRepository locationRepo,
            IWeatherRecordRepository recordRepo,
            IWeatherForecastRepository forecastRepo,
            IDataGovSgClient dataGovSgClient,
            IOpenWeatherMapClient openWeatherMapClient)
        {
            _mapper = mapper;
            _logging = logging;
            _locationRepo = locationRepo;
            _recordRepo = recordRepo;
            _forecastRepo = forecastRepo;
            _dataGovSgClient = dataGovSgClient;
            _openWeatherMapClient = openWeatherMapClient;
        }

        public async Task<GetCurrentWeatherResponseDTO?> GetCurrentWeatherAsync(Guid locationId)
        {
            try
            {
                var location = await _locationRepo.GetByIdAsync(locationId);
                if (location == null) return null;

                // Try to fetch fresh data from data.gov.sg and persist
                await FetchAndPersistCurrentAsync(location);

                // Return latest persisted record
                var record = await _recordRepo.GetLatestByLocationAsync(locationId);
                if (record == null) return null;

                var dto = _mapper.Map<GetCurrentWeatherResponseDTO>(record);
                dto.LocationName = location.LocationName;
                return dto;
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        public async Task<GetCurrentWeatherResponseDTO?> GetCurrentWeatherByNameAsync(string locationName)
        {
            try
            {
                var location = await ResolveOrCreateLocationAsync(locationName);
                if (location == null) return null;
                return await GetCurrentWeatherAsync(location.LocationId);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        public async Task<List<GetWeatherForecastResponseDTO>?> GetForecastByNameAsync(string locationName, int days = 4)
        {
            try
            {
                var location = await ResolveOrCreateLocationAsync(locationName);
                if (location == null) return null;
                return await GetForecastAsync(location.LocationId, days);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        public async Task<WeatherLocationSummaryDTO?> GetLocationSummaryAsync(string locationName)
        {
            try
            {
                var location = await ResolveOrCreateLocationAsync(locationName);
                if (location == null) return null;

                var current = await GetCurrentWeatherAsync(location.LocationId);
                var forecasts = await GetForecastAsync(location.LocationId, 4);

                return new WeatherLocationSummaryDTO
                {
                    LocationId   = location.LocationId,
                    LocationName = location.LocationName,
                    Region       = location.Region,
                    CurrentWeather = current,
                    Forecasts = forecasts
                };
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        // Finds a location by name (case-insensitive); auto-creates it if not found.
        private async Task<WeatherLocation?> ResolveOrCreateLocationAsync(string locationName)
        {
            var location = await _locationRepo.GetByNameAsync(locationName);
            if (location != null) return location;

            // Auto-create so the API works without manual seeding
            var newLocation = new WeatherLocation
            {
                LocationId    = Guid.NewGuid(),
                LocationName  = locationName,
                IsActive      = true,
                CreatedById   = Constant.DEFAULT_PUBLIC_USER_ID,
                CreatedByName = Constant.DEFAULT_PUBLIC_USER_NAME,
                CreatedDate   = DateTime.Now
            };
            return await _locationRepo.CreateAsync(newLocation);
        }

        public async Task<List<GetWeatherForecastResponseDTO>?> GetForecastAsync(Guid locationId, int days = 4)
        {
            try
            {
                var location = await _locationRepo.GetByIdAsync(locationId);
                if (location == null) return null;

                // Fetch and persist fresh forecasts
                await FetchAndPersistForecastAsync(location);

                var filter = new GetWeatherForecastsFilter { LocationId = locationId, Days = days };
                var forecasts = await _forecastRepo.GetForecastsAsync(filter);
                if (forecasts == null) return null;

                var dtos = _mapper.Map<List<GetWeatherForecastResponseDTO>>(forecasts);
                dtos.ForEach(d => d.LocationName = location.LocationName);
                return dtos;
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        public async Task<List<GetHistoricalWeatherResponseDTO>?> GetHistoricalAsync(GetWeatherRecordsFilter request)
        {
            try
            {
                var records = await _recordRepo.GetHistoricalAsync(request);
                if (records == null) return null;

                var locationIds = records.Select(r => r.LocationId).Distinct().ToList();
                var locations = new Dictionary<Guid, string>();
                foreach (var id in locationIds)
                {
                    var loc = await _locationRepo.GetByIdAsync(id);
                    if (loc != null) locations[id] = loc.LocationName;
                }

                var dtos = _mapper.Map<List<GetHistoricalWeatherResponseDTO>>(records);
                dtos.ForEach(d => d.LocationName = locations.TryGetValue(d.LocationId, out var name) ? name : "");
                return dtos;
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        public async Task<string> ExportToCsvAsync(GetWeatherRecordsFilter request)
        {
            try
            {
                var records = await _recordRepo.GetHistoricalAsync(request);
                if (records == null || records.Count == 0) return string.Empty;

                var locationIds = records.Select(r => r.LocationId).Distinct().ToList();
                var locations = new Dictionary<Guid, string>();
                foreach (var id in locationIds)
                {
                    var loc = await _locationRepo.GetByIdAsync(id);
                    if (loc != null) locations[id] = loc.LocationName;
                }

                var sb = new StringBuilder();
                sb.AppendLine("LocationName,RecordedAt,Temperature(°C),FeelsLike(°C),Humidity(%),WindSpeed(km/h),WindDirection,Rainfall(mm),AirQualityIndex,WeatherDescription,Source");

                foreach (var r in records)
                {
                    var locName = locations.TryGetValue(r.LocationId, out var n) ? n : "";
                    sb.AppendLine(string.Join(",",
                        CsvEscape(locName),
                        CsvEscape(r.RecordedAt.ToString("yyyy-MM-dd HH:mm:ss")),
                        r.Temperature?.ToString("F2") ?? "",
                        r.FeelsLike?.ToString("F2") ?? "",
                        r.Humidity?.ToString("F2") ?? "",
                        r.WindSpeed?.ToString("F2") ?? "",
                        CsvEscape(r.WindDirection ?? ""),
                        r.Rainfall?.ToString("F2") ?? "",
                        r.AirQualityIndex?.ToString() ?? "",
                        CsvEscape(r.WeatherDescription ?? ""),
                        CsvEscape(r.Source)
                    ));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return string.Empty;
            }
        }

        public async Task<List<GetWeatherLocationsResponseDTO>?> GetLocationsAsync(GetWeatherLocationsFilter request)
        {
            try
            {
                var locations = await _locationRepo.GetWeatherLocationsAsync(request);
                return _mapper.Map<List<GetWeatherLocationsResponseDTO>>(locations);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        public async Task<Guid?> CreateLocationAsync(CreateWeatherLocationRequestDTO request)
        {
            try
            {
                var location = _mapper.Map<WeatherLocation>(request);
                location.LocationId = Guid.NewGuid();
                location.CreatedDate = DateTime.Now;
                location.IsActive = true;

                location = await _locationRepo.CreateAsync(location);
                return location?.LocationId;
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        private async Task FetchAndPersistCurrentAsync(WeatherLocation location)
        {
            try
            {
                // Try data.gov.sg 2-hour forecast first (Singapore-specific)
                var twoHrReadings = await _dataGovSgClient.GetTwoHourForecastAsync();
                var psiReadings = await _dataGovSgClient.GetPsiReadingsAsync();
                var forecast24hr = await _dataGovSgClient.GetTwentyFourHourForecastAsync();

                string? forecastDesc = twoHrReadings?
                    .FirstOrDefault(r => r.Area.Equals(location.LocationName, StringComparison.OrdinalIgnoreCase))?.Forecast;

                // Fall back to OpenWeatherMap for numeric temperature/humidity
                var owmData = await _openWeatherMapClient.GetCurrentWeatherAsync("Singapore");

                var psiValue = GetPsiForRegion(psiReadings, location.Region);

                var record = new WeatherRecord
                {
                    RecordId = Guid.NewGuid(),
                    LocationId = location.LocationId,
                    RecordedAt = DateTime.UtcNow,
                    Temperature = owmData?.Main?.Temp,
                    FeelsLike = owmData?.Main?.FeelsLike,
                    Humidity = owmData?.Main?.Humidity,
                    WindSpeed = owmData?.Wind != null ? Math.Round(owmData.Wind.Speed * 3.6m, 2) : null, // m/s to km/h
                    WindDirection = owmData?.Wind != null ? DegreesToCompass(owmData.Wind.Deg) : forecast24hr?.General?.Wind?.Direction,
                    Rainfall = owmData?.Rain?.OneHour ?? owmData?.Rain?.ThreeHour,
                    AirQualityIndex = psiValue.HasValue ? (int)psiValue.Value : null,
                    WeatherDescription = forecastDesc ?? owmData?.Weather?.FirstOrDefault()?.Description,
                    Source = owmData != null ? "data.gov.sg+openweathermap" : "data.gov.sg",
                    CreatedById = Constant.DEFAULT_PUBLIC_USER_ID,
                    CreatedByName = Constant.DEFAULT_PUBLIC_USER_NAME,
                    CreatedDate = DateTime.Now
                };

                await _recordRepo.CreateAsync(record);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
            }
        }

        private async Task FetchAndPersistForecastAsync(WeatherLocation location)
        {
            try
            {
                var fourDayOutlook = await _dataGovSgClient.GetFourDayOutlookAsync();
                if (fourDayOutlook == null || fourDayOutlook.Count == 0) return;

                foreach (var outlook in fourDayOutlook)
                {
                    if (!DateTime.TryParse(outlook.Timestamp, out var forecastDate)) continue;

                    var forecast = new WeatherForecast
                    {
                        ForecastId = Guid.NewGuid(),
                        LocationId = location.LocationId,
                        ForecastDate = forecastDate,
                        ForecastDescription = outlook.Forecast,
                        TemperatureLow = outlook.Temperature?.Low,
                        TemperatureHigh = outlook.Temperature?.High,
                        HumidityLow = outlook.RelativeHumidity?.Low,
                        HumidityHigh = outlook.RelativeHumidity?.High,
                        WindSpeed = outlook.Wind?.Speed != null
                            ? $"{outlook.Wind.Speed.Low}-{outlook.Wind.Speed.High} km/h"
                            : null,
                        WindDirection = outlook.Wind?.Direction,
                        Source = "data.gov.sg",
                        CreatedById = Constant.DEFAULT_PUBLIC_USER_ID,
                        CreatedByName = Constant.DEFAULT_PUBLIC_USER_NAME,
                        CreatedDate = DateTime.Now
                    };

                    await _forecastRepo.CreateAsync(forecast);
                }
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
            }
        }

        private static decimal? GetPsiForRegion(DataGovSgPsiReading? psi, string? region)
        {
            if (psi?.PsiTwentyFourHourly == null) return null;
            return region?.ToLower() switch
            {
                "north" => psi.PsiTwentyFourHourly.North,
                "south" => psi.PsiTwentyFourHourly.South,
                "east" => psi.PsiTwentyFourHourly.East,
                "west" => psi.PsiTwentyFourHourly.West,
                "central" => psi.PsiTwentyFourHourly.Central,
                _ => psi.PsiTwentyFourHourly.National
            };
        }

        private static string DegreesToCompass(int degrees)
        {
            string[] directions = ["N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW"];
            return directions[(int)Math.Round(degrees / 22.5) % 16];
        }

        private static string CsvEscape(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}

using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using challenge1.Application.Bll.Weather;
using challenge1.Application.DTO.Weather;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Logging;
using challenge1.Common.Util.Common;
using challenge1.Common.Util.WeatherClients.Interfaces;
using challenge1.Common.Util.WeatherClients.Models;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.UnitTest.Weather
{
    [TestFixture]
    public class WeatherBllTests
    {
        private DatabaseFixture _fixture = null!;
        private WeatherBll _bll = null!;
        private Mock<IDataGovSgClient> _dataGovMock = null!;
        private Mock<IOpenWeatherMapClient> _owmMock = null!;

        [SetUp]
        public void SetUp()
        {
            _fixture = new DatabaseFixture();

            _dataGovMock = new Mock<IDataGovSgClient>();
            _owmMock     = new Mock<IOpenWeatherMapClient>();

            // Default: external APIs return null (graceful degradation path)
            _dataGovMock.Setup(c => c.GetTwoHourForecastAsync()).ReturnsAsync((List<DataGovSgReading>?)null);
            _dataGovMock.Setup(c => c.GetPsiReadingsAsync()).ReturnsAsync((DataGovSgPsiReading?)null);
            _dataGovMock.Setup(c => c.GetTwentyFourHourForecastAsync()).ReturnsAsync((DataGovSgTwentyFourHourForecast?)null);
            _dataGovMock.Setup(c => c.GetFourDayOutlookAsync()).ReturnsAsync((List<DataGovSgFourDayOutlook>?)null);
            _owmMock.Setup(c => c.GetCurrentWeatherAsync(It.IsAny<string>())).ReturnsAsync((OpenWeatherCurrentResponse?)null);
            _owmMock.Setup(c => c.GetFiveDayForecastAsync(It.IsAny<string>())).ReturnsAsync((List<OpenWeatherForecastItem>?)null);

            _bll = new WeatherBll(
                _fixture.ServiceProvider.GetRequiredService<IMapper>(),
                _fixture.ServiceProvider.GetRequiredService<ILogging>(),
                _fixture.ServiceProvider.GetRequiredService<IWeatherLocationRepository>(),
                _fixture.ServiceProvider.GetRequiredService<IWeatherRecordRepository>(),
                _fixture.ServiceProvider.GetRequiredService<IWeatherForecastRepository>(),
                _dataGovMock.Object,
                _owmMock.Object);
        }

        [TearDown]
        public void TearDown() => _fixture.Dispose();

        // ── Locations ────────────────────────────────────────────────────────────

        [Test]
        public async Task CreateLocation_ReturnsNewGuid()
        {
            var request = new CreateWeatherLocationRequestDTO
            {
                LocationName  = "Jurong East",
                Region        = "west",
                CreatedById   = "test",
                CreatedByName = "Test User"
            };

            var id = await _bll.CreateLocationAsync(request);

            Assert.That(id, Is.Not.Null);
            Assert.That(id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task GetLocations_ReturnsCreatedLocations()
        {
            await _bll.CreateLocationAsync(new CreateWeatherLocationRequestDTO
                { LocationName = "Punggol", Region = "north", CreatedById = "s", CreatedByName = "s" });
            await _bll.CreateLocationAsync(new CreateWeatherLocationRequestDTO
                { LocationName = "Sengkang", Region = "north", CreatedById = "s", CreatedByName = "s" });

            var result = await _bll.GetLocationsAsync(new GetWeatherLocationsFilter { Region = "north" });

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.GreaterThanOrEqualTo(2));
        }

        // ── Current weather ───────────────────────────────────────────────────────

        [Test]
        public async Task GetCurrentWeatherByName_AutoCreatesLocation_AndReturnsRecord()
        {
            // Arrange: OWM returns real data
            _owmMock.Setup(c => c.GetCurrentWeatherAsync(It.IsAny<string>()))
                .ReturnsAsync(new OpenWeatherCurrentResponse
                {
                    Name = "Singapore",
                    Dt   = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Main = new OpenWeatherMain { Temp = 30.5m, FeelsLike = 33m, Humidity = 80m },
                    Wind = new OpenWeatherWind { Speed = 3.5m, Deg = 180 },
                    Weather = [new OpenWeatherDescription { Description = "light rain" }]
                });

            var result = await _bll.GetCurrentWeatherByNameAsync("Tampines");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.LocationName, Is.EqualTo("Tampines"));
            Assert.That(result.Temperature, Is.EqualTo(30.5m));
            Assert.That(result.Humidity, Is.EqualTo(80m));
        }

        [Test]
        public async Task GetCurrentWeatherByName_SameLocationName_ReturnsExistingLocation()
        {
            _owmMock.Setup(c => c.GetCurrentWeatherAsync(It.IsAny<string>()))
                .ReturnsAsync(new OpenWeatherCurrentResponse
                {
                    Main    = new OpenWeatherMain { Temp = 28m, FeelsLike = 30m, Humidity = 75m },
                    Wind    = new OpenWeatherWind { Speed = 2m, Deg = 90 },
                    Weather = [new OpenWeatherDescription { Description = "cloudy" }]
                });

            // Call twice with the same name
            await _bll.GetCurrentWeatherByNameAsync("Bedok");
            await _bll.GetCurrentWeatherByNameAsync("Bedok");

            // Only one location should exist
            var locations = await _bll.GetLocationsAsync(
                new GetWeatherLocationsFilter { IsDeleted = false });
            var bedokCount = locations?.Count(l => l.LocationName == "Bedok") ?? 0;

            Assert.That(bedokCount, Is.EqualTo(1));
        }

        [Test]
        public async Task GetCurrentWeather_WithDataGovSgForecast_MergesDescription()
        {
            _dataGovMock.Setup(c => c.GetTwoHourForecastAsync())
                .ReturnsAsync([new DataGovSgReading { Area = "Hougang", Forecast = "Thundery Showers" }]);

            _owmMock.Setup(c => c.GetCurrentWeatherAsync(It.IsAny<string>()))
                .ReturnsAsync(new OpenWeatherCurrentResponse
                {
                    Main    = new OpenWeatherMain { Temp = 27m, FeelsLike = 29m, Humidity = 90m },
                    Wind    = new OpenWeatherWind { Speed = 1m, Deg = 45 },
                    Weather = [new OpenWeatherDescription { Description = "heavy rain" }]
                });

            var result = await _bll.GetCurrentWeatherByNameAsync("Hougang");

            Assert.That(result, Is.Not.Null);
            // data.gov.sg forecast takes priority over OWM description
            Assert.That(result!.WeatherDescription, Is.EqualTo("Thundery Showers"));
        }

        // ── Forecast ──────────────────────────────────────────────────────────────

        [Test]
        public async Task GetForecastByName_WithFourDayData_ReturnsPersisted()
        {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);
            _dataGovMock.Setup(c => c.GetFourDayOutlookAsync())
                .ReturnsAsync(
                [
                    new DataGovSgFourDayOutlook
                    {
                        Timestamp   = tomorrow.ToString("o"),
                        Forecast    = "Partly Cloudy",
                        Temperature = new DataGovSgRange { Low = 25m, High = 33m },
                        RelativeHumidity = new DataGovSgRange { Low = 60m, High = 95m },
                        Wind = new DataGovSgWind { Direction = "S", Speed = new DataGovSgRange { Low = 10m, High = 20m } }
                    }
                ]);

            var result = await _bll.GetForecastByNameAsync("Tampines", 4);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.GreaterThan(0));
            Assert.That(result.First().ForecastDescription, Is.EqualTo("Partly Cloudy"));
        }

        // ── Export CSV ───────────────────────────────────────────────────────────

        [Test]
        public async Task ExportToCsv_ReturnsValidCsv_WithHeader()
        {
            // Seed a location + record directly via repos
            var locationRepo = _fixture.ServiceProvider.GetRequiredService<IWeatherLocationRepository>();
            var recordRepo   = _fixture.ServiceProvider.GetRequiredService<IWeatherRecordRepository>();

            var location = await locationRepo.CreateAsync(new WeatherLocation
            {
                LocationId    = Guid.NewGuid(),
                LocationName  = "Changi",
                Region        = "east",
                IsActive      = true,
                CreatedById   = Constant.DEFAULT_PUBLIC_USER_ID,
                CreatedByName = Constant.DEFAULT_PUBLIC_USER_NAME,
                CreatedDate   = DateTime.Now
            });

            await recordRepo.CreateAsync(new WeatherRecord
            {
                RecordId           = Guid.NewGuid(),
                LocationId         = location.LocationId,
                RecordedAt         = DateTime.UtcNow,
                Temperature        = 31m,
                Humidity           = 78m,
                WeatherDescription = "Sunny",
                Source             = "test",
                CreatedById        = Constant.DEFAULT_PUBLIC_USER_ID,
                CreatedByName      = Constant.DEFAULT_PUBLIC_USER_NAME,
                CreatedDate        = DateTime.Now
            });

            var csv = await _bll.ExportToCsvAsync(new GetWeatherRecordsFilter { LocationId = location.LocationId });

            Assert.That(csv, Is.Not.Empty);
            Assert.That(csv, Does.Contain("LocationName,RecordedAt"));
            Assert.That(csv, Does.Contain("Changi"));
        }

        // ── Summary endpoint ─────────────────────────────────────────────────────

        [Test]
        public async Task GetLocationSummary_ReturnsCurrentAndForecast()
        {
            _owmMock.Setup(c => c.GetCurrentWeatherAsync(It.IsAny<string>()))
                .ReturnsAsync(new OpenWeatherCurrentResponse
                {
                    Main    = new OpenWeatherMain { Temp = 29m, FeelsLike = 31m, Humidity = 85m },
                    Wind    = new OpenWeatherWind { Speed = 2m, Deg = 270 },
                    Weather = [new OpenWeatherDescription { Description = "overcast" }]
                });

            var summary = await _bll.GetLocationSummaryAsync("Novena");

            Assert.That(summary, Is.Not.Null);
            Assert.That(summary!.LocationName, Is.EqualTo("Novena"));
            Assert.That(summary.CurrentWeather, Is.Not.Null);
            Assert.That(summary.CurrentWeather!.Temperature, Is.EqualTo(29m));
        }
    }
}

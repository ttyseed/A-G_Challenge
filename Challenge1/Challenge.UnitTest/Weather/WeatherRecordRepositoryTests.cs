using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Util.Common;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.UnitTest.Weather
{
    [TestFixture]
    public class WeatherRecordRepositoryTests
    {
        private DatabaseFixture _fixture = null!;
        private IWeatherLocationRepository _locationRepo = null!;
        private IWeatherRecordRepository _recordRepo = null!;

        [SetUp]
        public async Task SetUp()
        {
            _fixture = new DatabaseFixture();
            _locationRepo = _fixture.ServiceProvider.GetRequiredService<IWeatherLocationRepository>();
            _recordRepo   = _fixture.ServiceProvider.GetRequiredService<IWeatherRecordRepository>();
        }

        [TearDown]
        public void TearDown() => _fixture.Dispose();

        [Test]
        public async Task CreateRecord_AndGetLatest_ReturnsNewest()
        {
            var location = await _locationRepo.CreateAsync(BuildLocation("Tampines"));

            var older = BuildRecord(location.LocationId, DateTime.UtcNow.AddHours(-2), 28.0m);
            var newer = BuildRecord(location.LocationId, DateTime.UtcNow.AddHours(-1), 31.5m);

            await _recordRepo.CreateAsync(older);
            await _recordRepo.CreateAsync(newer);

            var latest = await _recordRepo.GetLatestByLocationAsync(location.LocationId);

            Assert.That(latest, Is.Not.Null);
            Assert.That(latest!.Temperature, Is.EqualTo(31.5m));
        }

        [Test]
        public async Task GetHistorical_FiltersByDateRange()
        {
            var location = await _locationRepo.CreateAsync(BuildLocation("Bedok"));
            var baseTime = DateTime.UtcNow.Date;

            await _recordRepo.CreateAsync(BuildRecord(location.LocationId, baseTime.AddDays(-3), 25m));
            await _recordRepo.CreateAsync(BuildRecord(location.LocationId, baseTime.AddDays(-1), 27m));
            await _recordRepo.CreateAsync(BuildRecord(location.LocationId, baseTime,             29m));

            var filter = new GetWeatherRecordsFilter
            {
                LocationId = location.LocationId,
                DateFrom   = baseTime.AddDays(-2),
                DateTo     = baseTime.AddDays(1)
            };

            var results = await _recordRepo.GetHistoricalAsync(filter);

            Assert.That(results, Is.Not.Null);
            Assert.That(results!.Count, Is.EqualTo(2));
            Assert.That(results.All(r => r.RecordedAt >= baseTime.AddDays(-2)), Is.True);
        }

        [Test]
        public async Task GetLatestByLocation_ReturnsNull_WhenNoRecords()
        {
            var location = await _locationRepo.CreateAsync(BuildLocation("Empty Location"));

            var result = await _recordRepo.GetLatestByLocationAsync(location.LocationId);

            Assert.That(result, Is.Null);
        }

        private static WeatherLocation BuildLocation(string name) => new()
        {
            LocationId    = Guid.NewGuid(),
            LocationName  = name,
            Region        = "east",
            IsActive      = true,
            CreatedById   = Constant.DEFAULT_PUBLIC_USER_ID,
            CreatedByName = Constant.DEFAULT_PUBLIC_USER_NAME,
            CreatedDate   = DateTime.Now
        };

        private static WeatherRecord BuildRecord(Guid locationId, DateTime recordedAt, decimal temp) => new()
        {
            RecordId           = Guid.NewGuid(),
            LocationId         = locationId,
            RecordedAt         = recordedAt,
            Temperature        = temp,
            Humidity           = 75m,
            WeatherDescription = "Partly cloudy",
            Source             = "test",
            CreatedById        = Constant.DEFAULT_PUBLIC_USER_ID,
            CreatedByName      = Constant.DEFAULT_PUBLIC_USER_NAME,
            CreatedDate        = DateTime.Now
        };
    }
}

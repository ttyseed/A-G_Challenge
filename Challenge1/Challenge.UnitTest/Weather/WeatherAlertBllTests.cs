using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using challenge1.Application.Bll.Weather;
using challenge1.Application.DTO.Weather;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Logging;
using challenge1.Common.Util.Common;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.UnitTest.Weather
{
    [TestFixture]
    public class WeatherAlertBllTests
    {
        private DatabaseFixture _fixture = null!;
        private WeatherAlertBll _bll = null!;
        private IWeatherLocationRepository _locationRepo = null!;
        private Guid _locationId;

        [SetUp]
        public async Task SetUp()
        {
            _fixture      = new DatabaseFixture();
            _locationRepo = _fixture.ServiceProvider.GetRequiredService<IWeatherLocationRepository>();

            _bll = new WeatherAlertBll(
                _fixture.ServiceProvider.GetRequiredService<IMapper>(),
                _fixture.ServiceProvider.GetRequiredService<ILogging>(),
                _fixture.ServiceProvider.GetRequiredService<IWeatherAlertRepository>(),
                _locationRepo);

            var location = await _locationRepo.CreateAsync(new WeatherLocation
            {
                LocationId    = Guid.NewGuid(),
                LocationName  = "Test Area",
                Region        = "central",
                IsActive      = true,
                CreatedById   = Constant.DEFAULT_PUBLIC_USER_ID,
                CreatedByName = Constant.DEFAULT_PUBLIC_USER_NAME,
                CreatedDate   = DateTime.Now
            });
            _locationId = location.LocationId;
        }

        [TearDown]
        public void TearDown() => _fixture.Dispose();

        [Test]
        public async Task Subscribe_ValidRequest_ReturnsAlertId()
        {
            var request = BuildSubscribeRequest("temperature_high", 35m);

            var alertId = await _bll.SubscribeAsync(request);

            Assert.That(alertId, Is.Not.Null);
            Assert.That(alertId, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task Subscribe_InvalidLocation_ReturnsNull()
        {
            var request = BuildSubscribeRequest("temperature_high", 35m);
            request.LocationId = Guid.NewGuid(); // non-existent

            var alertId = await _bll.SubscribeAsync(request);

            Assert.That(alertId, Is.Null);
        }

        [Test]
        public async Task GetAlerts_ReturnsSubscribedAlerts()
        {
            await _bll.SubscribeAsync(BuildSubscribeRequest("rainfall", 50m));
            await _bll.SubscribeAsync(BuildSubscribeRequest("aqi",      100m));

            var results = await _bll.GetAlertsAsync(new GetWeatherAlertsFilter
            {
                LocationId = _locationId,
                IsActive   = true,
                IsDeleted  = false
            });

            Assert.That(results, Is.Not.Null);
            Assert.That(results!.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Unsubscribe_DeactivatesAlert()
        {
            var alertId = await _bll.SubscribeAsync(BuildSubscribeRequest("temperature_low", 18m));
            Assert.That(alertId, Is.Not.Null);

            var success = await _bll.UnsubscribeAsync(new UnsubscribeWeatherAlertRequestDTO
            {
                AlertId       = alertId!.Value,
                UpdatedById   = "test",
                UpdatedByName = "Test User"
            });

            Assert.That(success, Is.True);

            // Verify it no longer appears in active list
            var active = await _bll.GetAlertsAsync(new GetWeatherAlertsFilter
            {
                LocationId = _locationId,
                IsActive   = true,
                IsDeleted  = false
            });

            Assert.That(active?.Any(a => a.AlertId == alertId.Value), Is.False);
        }

        [Test]
        public async Task Unsubscribe_NonExistentAlert_ReturnsFalse()
        {
            var success = await _bll.UnsubscribeAsync(new UnsubscribeWeatherAlertRequestDTO
            {
                AlertId       = Guid.NewGuid(),
                UpdatedById   = "test",
                UpdatedByName = "Test User"
            });

            Assert.That(success, Is.False);
        }

        [Test]
        [TestCase("temperature_high")]
        [TestCase("temperature_low")]
        [TestCase("rainfall")]
        [TestCase("aqi")]
        public async Task Subscribe_AllAlertTypes_Succeed(string alertType)
        {
            var alertId = await _bll.SubscribeAsync(BuildSubscribeRequest(alertType, 50m));

            Assert.That(alertId, Is.Not.Null);
        }

        private SubscribeWeatherAlertRequestDTO BuildSubscribeRequest(string alertType, decimal threshold) => new()
        {
            LocationId      = _locationId,
            SubscriberEmail = "test@example.com",
            SubscriberName  = "Test Subscriber",
            AlertType       = alertType,
            ThresholdValue  = threshold,
            CreatedById     = Constant.DEFAULT_PUBLIC_USER_ID,
            CreatedByName   = Constant.DEFAULT_PUBLIC_USER_NAME
        };
    }
}

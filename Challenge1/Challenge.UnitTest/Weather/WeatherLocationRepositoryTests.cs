using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using challenge1.Application.Filter.Weather;
using challenge1.Common.Util.Common;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.UnitTest.Weather
{
    [TestFixture]
    public class WeatherLocationRepositoryTests
    {
        private DatabaseFixture _fixture = null!;
        private IWeatherLocationRepository _repo = null!;

        [SetUp]
        public void SetUp()
        {
            _fixture = new DatabaseFixture();
            _repo = _fixture.ServiceProvider.GetRequiredService<IWeatherLocationRepository>();
        }

        [TearDown]
        public void TearDown() => _fixture.Dispose();

        [Test]
        public async Task CreateAndGetById_ReturnsLocation()
        {
            var location = BuildLocation("Tampines", "east");

            var created = await _repo.CreateAsync(location);

            Assert.That(created, Is.Not.Null);
            Assert.That(created.LocationId, Is.Not.EqualTo(Guid.Empty));

            var fetched = await _repo.GetByIdAsync(created.LocationId);
            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched!.LocationName, Is.EqualTo("Tampines"));
        }

        [Test]
        public async Task GetByName_ReturnsExistingLocation()
        {
            await _repo.CreateAsync(BuildLocation("Ang Mo Kio", "north"));

            var result = await _repo.GetByNameAsync("Ang Mo Kio");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Region, Is.EqualTo("north"));
        }

        [Test]
        public async Task GetByName_ReturnsNull_WhenNotFound()
        {
            var result = await _repo.GetByNameAsync("NonExistentArea");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetWeatherLocations_FilterByRegion_ReturnsMatchingOnly()
        {
            await _repo.CreateAsync(BuildLocation("Yishun",   "north"));
            await _repo.CreateAsync(BuildLocation("Bedok",    "east"));
            await _repo.CreateAsync(BuildLocation("Clementi", "west"));

            var result = await _repo.GetWeatherLocationsAsync(new GetWeatherLocationsFilter { Region = "north" });

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.All(l => l.Region == "north"), Is.True);
            Assert.That(result.Any(l => l.LocationName == "Yishun"), Is.True);
        }

        [Test]
        public async Task GetWeatherLocations_FilterByIsActive_ReturnsActiveOnly()
        {
            var active   = BuildLocation("Woodlands", "north");
            var inactive = BuildLocation("Seletar",   "north");
            inactive.IsActive = false;

            await _repo.CreateAsync(active);
            await _repo.CreateAsync(inactive);

            var result = await _repo.GetWeatherLocationsAsync(new GetWeatherLocationsFilter { IsActive = true, IsDeleted = false });

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Any(l => l.LocationName == "Woodlands"), Is.True);
            Assert.That(result.Any(l => l.LocationName == "Seletar"), Is.False);
        }

        [Test]
        public async Task Update_ChangesLocationName()
        {
            var location = await _repo.CreateAsync(BuildLocation("OldName", "central"));

            location.LocationName = "NewName";
            location.UpdatedDate = DateTime.Now;
            await _repo.UpdateAsync(location);

            var updated = await _repo.GetByIdAsync(location.LocationId);
            Assert.That(updated!.LocationName, Is.EqualTo("NewName"));
        }

        private static WeatherLocation BuildLocation(string name, string region) => new()
        {
            LocationId    = Guid.NewGuid(),
            LocationName  = name,
            Region        = region,
            IsActive      = true,
            IsDeleted     = false,
            CreatedById   = Constant.DEFAULT_PUBLIC_USER_ID,
            CreatedByName = Constant.DEFAULT_PUBLIC_USER_NAME,
            CreatedDate   = DateTime.Now
        };
    }
}

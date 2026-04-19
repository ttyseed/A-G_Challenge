using Microsoft.EntityFrameworkCore;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Context;

namespace challenge1.Application.API.DataSeeder
{
    public static class WeatherDataSeeder
    {
        // Singapore locations: 5 planning regions + 22 common areas used by data.gov.sg 2-hour forecast
        private static readonly List<(string Name, string Region, decimal Lat, decimal Lon)> Locations =
        [
            // Regions
            ("Central",          "central", 1.3521m,  103.8198m),
            ("North",            "north",   1.4200m,  103.8100m),
            ("South",            "south",   1.2800m,  103.8300m),
            ("East",             "east",    1.3500m,  103.9400m),
            ("West",             "west",    1.3400m,  103.7000m),

            // Areas matching data.gov.sg area names exactly
            ("Ang Mo Kio",       "north",   1.3691m,  103.8454m),
            ("Bedok",            "east",    1.3236m,  103.9273m),
            ("Bishan",           "central", 1.3526m,  103.8352m),
            ("Boon Lay",         "west",    1.3396m,  103.7060m),
            ("Bukit Batok",      "west",    1.3490m,  103.7494m),
            ("Bukit Merah",      "central", 1.2819m,  103.8239m),
            ("Bukit Panjang",    "west",    1.3774m,  103.7719m),
            ("Bukit Timah",      "central", 1.3294m,  103.8021m),
            ("Changi",           "east",    1.3644m,  103.9915m),
            ("Choa Chu Kang",    "west",    1.3840m,  103.7470m),
            ("Clementi",         "west",    1.3162m,  103.7649m),
            ("Geylang",          "east",    1.3201m,  103.8918m),
            ("Hougang",          "north",   1.3612m,  103.8863m),
            ("Jurong East",      "west",    1.3329m,  103.7436m),
            ("Jurong West",      "west",    1.3404m,  103.7090m),
            ("Kallang",          "central", 1.3100m,  103.8720m),
            ("Lim Chu Kang",     "north",   1.4331m,  103.7198m),
            ("Mandai",           "north",   1.4041m,  103.8070m),
            ("Marine Parade",    "east",    1.3022m,  103.9073m),
            ("Novena",           "central", 1.3204m,  103.8438m),
            ("Pasir Ris",        "east",    1.3721m,  103.9493m),
            ("Paya Lebar",       "east",    1.3180m,  103.8920m),
            ("Pioneer",          "west",    1.3151m,  103.6975m),
            ("Pulau Ubin",       "east",    1.4049m,  103.9600m),
            ("Punggol",          "north",   1.3984m,  103.9072m),
            ("Queenstown",       "central", 1.2942m,  103.8060m),
            ("Seletar",          "north",   1.4046m,  103.8697m),
            ("Sembawang",        "north",   1.4491m,  103.8185m),
            ("Sengkang",         "north",   1.3868m,  103.8914m),
            ("Serangoon",        "north",   1.3554m,  103.8679m),
            ("Singapore",        "central", 1.3521m,  103.8198m),
            ("Tampines",         "east",    1.3544m,  103.9439m),
            ("Tanglin",          "central", 1.3081m,  103.8144m),
            ("Tengah",           "west",    1.3740m,  103.7410m),
            ("Toa Payoh",        "central", 1.3343m,  103.8563m),
            ("Tuas",             "west",    1.2970m,  103.6369m),
            ("Western Islands",  "west",    1.2500m,  103.7500m),
            ("Western Water Catchment", "west", 1.4050m, 103.6890m),
            ("Woodlands",        "north",   1.4382m,  103.7890m),
            ("Yishun",           "north",   1.4304m,  103.8354m),
        ];

        public static async Task SeedAsync(DatabaseContext db)
        {
            var hasAny = await db.WeatherLocations.AnyAsync();
            if (hasAny) return;

            var now = DateTime.Now;
            var locations = Locations.Select(l => new WeatherLocation
            {
                LocationId   = Guid.NewGuid(),
                LocationName = l.Name,
                Region       = l.Region,
                Latitude     = l.Lat,
                Longitude    = l.Lon,
                IsActive     = true,
                IsDeleted    = false,
                CreatedById  = "SYSTEM",
                CreatedByName = "System",
                CreatedDate  = now
            }).ToList();

            db.WeatherLocations.AddRange(locations);
            await db.SaveChangesAsync();
        }
    }
}

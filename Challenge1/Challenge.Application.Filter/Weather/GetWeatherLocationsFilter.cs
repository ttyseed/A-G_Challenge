namespace challenge1.Application.Filter.Weather
{
    public class GetWeatherLocationsFilter
    {
        public string? LocationName { get; set; }
        public string? Region { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

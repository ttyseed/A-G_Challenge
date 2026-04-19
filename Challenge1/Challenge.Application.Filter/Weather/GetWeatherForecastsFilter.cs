namespace challenge1.Application.Filter.Weather
{
    public class GetWeatherForecastsFilter
    {
        public Guid? LocationId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? Days { get; set; }
    }
}

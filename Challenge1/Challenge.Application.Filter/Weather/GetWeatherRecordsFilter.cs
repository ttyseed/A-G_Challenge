namespace challenge1.Application.Filter.Weather
{
    public class GetWeatherRecordsFilter
    {
        public Guid? LocationId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Source { get; set; }
    }
}

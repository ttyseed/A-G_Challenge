namespace challenge1.Application.Filter.Weather
{
    public class GetWeatherAlertsFilter
    {
        public Guid? LocationId { get; set; }
        public string? SubscriberEmail { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

namespace challenge1.Application.DTO.Weather
{
    public class GetWeatherAlertsResponseDTO
    {
        public Guid AlertId { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public string SubscriberEmail { get; set; } = null!;
        public string SubscriberName { get; set; } = null!;
        public string AlertType { get; set; } = null!;
        public decimal ThresholdValue { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastTriggeredAt { get; set; }
    }

    public class SubscribeWeatherAlertRequestDTO
    {
        public Guid LocationId { get; set; }
        public string SubscriberEmail { get; set; } = null!;
        public string SubscriberName { get; set; } = null!;
        /// <summary>temperature_high | temperature_low | rainfall | aqi</summary>
        public string AlertType { get; set; } = null!;
        public decimal ThresholdValue { get; set; }
        public string CreatedById { get; set; } = null!;
        public string CreatedByName { get; set; } = null!;
    }

    public class UnsubscribeWeatherAlertRequestDTO
    {
        public Guid AlertId { get; set; }
        public string UpdatedById { get; set; } = null!;
        public string UpdatedByName { get; set; } = null!;
    }
}

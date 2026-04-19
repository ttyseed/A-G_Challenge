namespace challenge1.Application.DTO.Weather
{
    public class GetWeatherForecastResponseDTO
    {
        public Guid ForecastId { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public DateTime ForecastDate { get; set; }
        public string? ForecastPeriod { get; set; }
        public string? ForecastDescription { get; set; }
        public decimal? TemperatureLow { get; set; }
        public decimal? TemperatureHigh { get; set; }
        public decimal? HumidityLow { get; set; }
        public decimal? HumidityHigh { get; set; }
        public string? WindSpeed { get; set; }
        public string? WindDirection { get; set; }
        public string Source { get; set; } = null!;
    }
}

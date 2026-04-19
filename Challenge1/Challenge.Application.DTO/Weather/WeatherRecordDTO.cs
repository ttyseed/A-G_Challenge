namespace challenge1.Application.DTO.Weather
{
    public class GetCurrentWeatherResponseDTO
    {
        public Guid RecordId { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public DateTime RecordedAt { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? FeelsLike { get; set; }
        public decimal? Humidity { get; set; }
        public decimal? WindSpeed { get; set; }
        public string? WindDirection { get; set; }
        public decimal? Rainfall { get; set; }
        public int? AirQualityIndex { get; set; }
        public string? WeatherDescription { get; set; }
        public string Source { get; set; } = null!;
    }

    public class GetHistoricalWeatherResponseDTO
    {
        public Guid RecordId { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public DateTime RecordedAt { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public decimal? WindSpeed { get; set; }
        public string? WindDirection { get; set; }
        public decimal? Rainfall { get; set; }
        public int? AirQualityIndex { get; set; }
        public string? WeatherDescription { get; set; }
        public string Source { get; set; } = null!;
    }

    public class WeatherExportRowDTO
    {
        public string LocationName { get; set; } = null!;
        public string RecordedAt { get; set; } = null!;
        public string? Temperature { get; set; }
        public string? Humidity { get; set; }
        public string? WindSpeed { get; set; }
        public string? WindDirection { get; set; }
        public string? Rainfall { get; set; }
        public string? AirQualityIndex { get; set; }
        public string? WeatherDescription { get; set; }
        public string Source { get; set; } = null!;
    }
}

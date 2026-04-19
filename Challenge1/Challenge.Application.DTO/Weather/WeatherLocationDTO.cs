namespace challenge1.Application.DTO.Weather
{
    public class WeatherLocationSummaryDTO
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public string? Region { get; set; }
        public GetCurrentWeatherResponseDTO? CurrentWeather { get; set; }
        public List<GetWeatherForecastResponseDTO>? Forecasts { get; set; }
    }

    public class GetWeatherLocationsResponseDTO
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public string? Region { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateWeatherLocationRequestDTO
    {
        public string LocationName { get; set; } = null!;
        public string? Region { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string CreatedById { get; set; } = null!;
        public string CreatedByName { get; set; } = null!;
    }

    public class UpdateWeatherLocationRequestDTO
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public string? Region { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedById { get; set; } = null!;
        public string UpdatedByName { get; set; } = null!;
    }
}

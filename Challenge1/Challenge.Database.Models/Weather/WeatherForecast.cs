using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge1.Database.Models.Weather
{
    [Table("weather_forecast")]
    public class WeatherForecast
    {
        [Key]
        [Column("forecast_id")]
        public Guid ForecastId { get; set; }

        [Column("location_id")]
        public Guid LocationId { get; set; }

        [Column("forecast_date", TypeName = "timestamp without time zone")]
        public DateTime ForecastDate { get; set; }

        [Column("forecast_period")]
        [MaxLength(20)]
        public string? ForecastPeriod { get; set; }

        [Column("forecast_description")]
        [MaxLength(200)]
        public string? ForecastDescription { get; set; }

        [Column("temperature_low", TypeName = "decimal(5,2)")]
        public decimal? TemperatureLow { get; set; }

        [Column("temperature_high", TypeName = "decimal(5,2)")]
        public decimal? TemperatureHigh { get; set; }

        [Column("humidity_low", TypeName = "decimal(5,2)")]
        public decimal? HumidityLow { get; set; }

        [Column("humidity_high", TypeName = "decimal(5,2)")]
        public decimal? HumidityHigh { get; set; }

        [Column("wind_speed")]
        [MaxLength(30)]
        public string? WindSpeed { get; set; }

        [Column("wind_direction")]
        [MaxLength(20)]
        public string? WindDirection { get; set; }

        [Column("source")]
        [MaxLength(50)]
        public string Source { get; set; } = null!;

        [Column("created_by_id")]
        [MaxLength(40)]
        public string CreatedById { get; set; } = null!;

        [Column("created_by_name")]
        [MaxLength(66)]
        public string CreatedByName { get; set; } = null!;

        [Column("created_date", TypeName = "timestamp without time zone")]
        public DateTime CreatedDate { get; set; }

        // Navigation
        [ForeignKey(nameof(LocationId))]
        public WeatherLocation? WeatherLocation { get; set; }
    }
}

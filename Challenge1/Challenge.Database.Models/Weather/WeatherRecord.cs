using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge1.Database.Models.Weather
{
    [Table("weather_record")]
    public class WeatherRecord
    {
        [Key]
        [Column("record_id")]
        public Guid RecordId { get; set; }

        [Column("location_id")]
        public Guid LocationId { get; set; }

        [Column("recorded_at", TypeName = "timestamp without time zone")]
        public DateTime RecordedAt { get; set; }

        [Column("temperature", TypeName = "decimal(5,2)")]
        public decimal? Temperature { get; set; }

        [Column("feels_like", TypeName = "decimal(5,2)")]
        public decimal? FeelsLike { get; set; }

        [Column("humidity", TypeName = "decimal(5,2)")]
        public decimal? Humidity { get; set; }

        [Column("wind_speed", TypeName = "decimal(6,2)")]
        public decimal? WindSpeed { get; set; }

        [Column("wind_direction")]
        [MaxLength(10)]
        public string? WindDirection { get; set; }

        [Column("rainfall", TypeName = "decimal(6,2)")]
        public decimal? Rainfall { get; set; }

        [Column("air_quality_index")]
        public int? AirQualityIndex { get; set; }

        [Column("weather_description")]
        [MaxLength(200)]
        public string? WeatherDescription { get; set; }

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

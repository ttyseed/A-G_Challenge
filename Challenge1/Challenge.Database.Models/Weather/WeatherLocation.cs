using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge1.Database.Models.Weather
{
    [Table("weather_location")]
    public class WeatherLocation
    {
        [Key]
        [Column("location_id")]
        public Guid LocationId { get; set; }

        [Column("location_name")]
        [MaxLength(100)]
        public string LocationName { get; set; } = null!;

        [Column("region")]
        [MaxLength(20)]
        public string? Region { get; set; }

        [Column("latitude", TypeName = "decimal(9,6)")]
        public decimal? Latitude { get; set; }

        [Column("longitude", TypeName = "decimal(9,6)")]
        public decimal? Longitude { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("created_by_id")]
        [MaxLength(40)]
        public string CreatedById { get; set; } = null!;

        [Column("created_by_name")]
        [MaxLength(66)]
        public string CreatedByName { get; set; } = null!;

        [Column("created_date", TypeName = "timestamp without time zone")]
        public DateTime CreatedDate { get; set; }

        [Column("updated_by_id")]
        [MaxLength(40)]
        public string? UpdatedById { get; set; }

        [Column("updated_by_name")]
        [MaxLength(66)]
        public string? UpdatedByName { get; set; }

        [Column("updated_date", TypeName = "timestamp without time zone")]
        public DateTime? UpdatedDate { get; set; }

        // Child tables
        public ICollection<WeatherRecord> WeatherRecords { get; set; } = new List<WeatherRecord>();
        public ICollection<WeatherForecast> WeatherForecasts { get; set; } = new List<WeatherForecast>();
        public ICollection<WeatherAlert> WeatherAlerts { get; set; } = new List<WeatherAlert>();
    }
}

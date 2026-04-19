using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge1.Database.Models.Weather
{
    [Table("weather_alert")]
    public class WeatherAlert
    {
        [Key]
        [Column("alert_id")]
        public Guid AlertId { get; set; }

        [Column("location_id")]
        public Guid LocationId { get; set; }

        [Column("subscriber_email")]
        [MaxLength(320)]
        public string SubscriberEmail { get; set; } = null!;

        [Column("subscriber_name")]
        [MaxLength(100)]
        public string SubscriberName { get; set; } = null!;

        /// <summary>temperature_high | temperature_low | rainfall | aqi</summary>
        [Column("alert_type")]
        [MaxLength(30)]
        public string AlertType { get; set; } = null!;

        [Column("threshold_value", TypeName = "decimal(8,2)")]
        public decimal ThresholdValue { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("last_triggered_at", TypeName = "timestamp without time zone")]
        public DateTime? LastTriggeredAt { get; set; }

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

        // Navigation
        [ForeignKey(nameof(LocationId))]
        public WeatherLocation? WeatherLocation { get; set; }
    }
}

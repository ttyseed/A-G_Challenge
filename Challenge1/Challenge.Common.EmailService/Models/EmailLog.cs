using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge1.Common.EmailService.Models
{
    [Table("email_log")]
    public class EmailLog
    {
        [Key]
        [Column("email_log_id")]
        public Guid EmailLogId { get; set; }

        [Required]
        [Column("email_id")]
        public Guid EmailId { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("log_type")]
        public string LogType { get; set; } = string.Empty;

        [Required]
        [MaxLength(4000)]
        [Column("log_message")]
        public string LogMessage { get; set; } = string.Empty;

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        [MaxLength(1000)]
        [Column("remark")]
        public string? Remark { get; set; }

        [Required, MaxLength(40)]
        [Column("created_by_id")]
        public string CreatedById { get; set; } = string.Empty;

        [Required, MaxLength(66)]
        [Column("created_by_name")]
        public string CreatedByName { get; set; } = string.Empty;

        [Required]
        [Column("created_date", TypeName = "timestamp without time zone")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [MaxLength(40)]
        [Column("updated_by_id")]
        public string? UpdatedById { get; set; }

        [MaxLength(66)]
        [Column("updated_by_name")]
        public string? UpdatedByName { get; set; }

        [Column("updated_date", TypeName = "timestamp without time zone")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation
        public Email? Email { get; set; }
    }
}

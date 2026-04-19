using challenge1.Common.EmailService.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge1.Common.EmailService.Models
{
    [Table("email")]
    public class Email
    {
        [Key]
        [Column("email_id")]
        public Guid EmailId { get; set; }

        [Column("module_type")]
        public short? ModuleType { get; set; }

        [MaxLength(500)]
        [Column("subject")]
        public string Subject { get; set; } = string.Empty;

        [Column("body", TypeName = "text")]
        public string? Body { get; set; }

        [Required]
        [Column("recipients")]
        public string Recipients { get; set; } = string.Empty;

        [Column("cc")]
        public string? Cc { get; set; }

        [Column("bcc")]
        public string? Bcc { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("status")]
        public string Status { get; set; } = Constant.EmailStatus.Pending;

        [Required]
        [MaxLength(8)]
        [Column("send_type")]
        public string SendType { get; set; } = string.Empty;

        [Column("target_send_date", TypeName = "timestamp without time zone")]
        public DateTime? TargetSendDate { get; set; }

        [Column("sent_at", TypeName = "timestamp without time zone")]
        public DateTime? SentAt { get; set; }

        [Column("retry_count")]
        public short RetryCount { get; set; } = 0;

        [Column("priority")]
        public short Priority { get; set; } = 0;

        [Column("batch_group_id")]
        public Guid? BatchGroupId { get; set; }

        [Column("cancel_reason")]
        [MaxLength(1000)]
        public string? CancelReason { get; set; }

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

        // Navigation (Auto includes child tables, so no need to explicitly include them in queries. E.g. using .Include(x => x.Attachments). This is the same as left join but more convenient and faster unless it has many child records)
        public ICollection<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
        public ICollection<EmailLog> Logs { get; set; } = new List<EmailLog>();
    }
}

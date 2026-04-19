using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge1.Common.EmailService.Models
{
    [Table("email_attachment")]
    public class EmailAttachment
    {
        [Key]
        [Column("email_attachment_id")]
        public Guid EmailAttachmentId { get; set; }

        [Required]
        [Column("email_id")]
        public Guid EmailId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("attachment_type")]
        public string AttachmentType { get; set; } = string.Empty; 

        [MaxLength(500)]
        [Column("attachment_location")]
        public string? AttachmentLocation { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;

        [Column("content")]
        public byte[]? Content { get; set; }

        [Required]
        [Column("file_size")]
        public long FileSize { get; set; }

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

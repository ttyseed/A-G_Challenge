using challenge1.Common.EmailService.Classes;
using System.ComponentModel.DataAnnotations;

namespace challenge1.Common.EmailService.DTOs
{
    public class AttachmentDTO
    {
        public Guid EmailAttachmentId { get; set; }
        public AttachmentTypeEnum AttachmentType { get; set; }
        public string? AttachmentLocation { get; set; }
        public byte[]? Content { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string CreatedById { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? UpdatedById { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class AddAttachmentRequestDTO
    {
        [Required]
        public AttachmentTypeEnum AttachmentType { get; set; }
        public string? AttachmentLocation { get; set; }
        public byte[]? Content { get; set; }
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        internal Guid EmailId { get; set; }
        internal string CreatedById { get; set; } = string.Empty;
        internal string CreatedByName { get; set; } = string.Empty;
    }

    public class AddAttachmentResponseDTO
    {
        public Guid EmailAttachmentId { get; set; }
        internal Guid EmailId { get; set; }
        internal string ErrorMessage { get; set; } = string.Empty;
    }
}

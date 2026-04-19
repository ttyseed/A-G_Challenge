using challenge1.Common.EmailService.Classes;
using System.ComponentModel.DataAnnotations;

namespace challenge1.Common.EmailService.DTOs
{
    public class GetPendingBatchEmailsRequestDTO
    {
        public short ModuleType { get; set; }
    }

    public class GetPendingBatchEmailsResponseDTO
    {
        public Guid EmailId { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string Recipients { get; set; } = string.Empty;
        public string? Cc { get; set; }
        public string? Bcc { get; set; }
        public DateTime? TargetSendDate { get; set; }
        public short RetryCount { get; set; } = 0;
        public short Priority { get; set; } = 0;
        public Guid? BatchGroupId { get; set; }
        public string CreatedById { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? UpdatedById { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<AttachmentDTO>? Attachments { get; set; }
    }

    /// <summary>
    /// Request DTO for Manual Email Send. Contains email details and optional attachments.
    /// </summary>
    public class SendManualEmailRequestDTO
    {
        /// <summary>
        /// Email subject, max 500 characters
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Subject { get; set; } = string.Empty;
        /// <summary>
        /// Email body. Optional.
        /// </summary>
        public string? Body { get; set; }
        /// <summary>
        /// Email recipients, semicolon separated if multiple
        /// </summary>
        [Required]
        [EmailList]
        public string Recipients { get; set; } = string.Empty;
        /// <summary>
        /// Email CC, semicolon separated if multiple. Optional.
        /// </summary>
        [EmailList]
        public string? Cc { get; set; }
        /// <summary>
        /// Email BCC, semicolon separated if multiple. Optional.
        /// </summary>
        [EmailList]
        public string? Bcc { get; set; }
        /// <summary>
        /// Creator User Id
        /// </summary>
        [Required]
        [MaxLength(40)]
        public string CreatedById { get; set; } = string.Empty;
        /// <summary>
        /// Creator Name
        /// </summary>
        [Required]
        [MaxLength(66)]
        public string CreatedByName { get; set; } = string.Empty;
        /// <summary>
        /// Attachments if any for the email. Optional.
        /// </summary>
        public List<AddAttachmentRequestDTO>? Attachments { get; set; }
        /// <summary>
        /// Maximum number of recipients per batch email, system will split the email into multiple emails if recipients exceed this number. Optional.
        /// </summary>
        public int? MaxRecipientsPerBatch { get; set; }
        /// <summary>
        /// Module type of the email. Optional. E.g., 1 = LE, 2 = PV, 3 = TRS. This helps in categorizing the email.
        /// </summary>
        public short? ModuleType { get; set; }
    }

    /// <summary>
    /// Response DTO for Manual Email Send. If successful, returns the Email Id of the sent email. If failed, returns Error Message.
    /// </summary>
    public class SendManualEmailResponseDTO
    {
        /// <summary>
        /// Email Id of the sent email
        /// </summary>
        public Guid? EmailId { get; set; }
        /// <summary>
        /// Error message if sending failed
        /// </summary>
        public string? ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for Batch Job Email Send. Contains email details and optional attachments.
    /// </summary>
    public class SendBatchEmailRequestDTO
    {
        /// <summary>
        /// Email subject, max 500 characters
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Subject { get; set; } = string.Empty;
        /// <summary>
        /// Email body. Optional.
        /// </summary>
        public string? Body { get; set; }
        /// <summary>
        /// Email recipients, semicolon separated if multiple
        /// </summary>
        [Required]
        [EmailList]
        public string Recipients { get; set; } = string.Empty;
        /// <summary>
        /// Email CC, semicolon separated if multiple. Optional.
        /// </summary>
        [EmailList]
        public string? Cc { get; set; }
        /// <summary>
        /// Email BCC, semicolon separated if multiple. Optional.
        /// </summary>
        [EmailList]
        public string? Bcc { get; set; }
        /// <summary>
        /// Target send date and time for the email
        /// </summary>
        [Required]
        public DateTime TargetSendDate { get; set; }
        /// <summary>
        /// Priority of the email. Optional. Default is 0. The higher the number, the higher the priority.
        /// </summary>
        public short? Priority { get; set; } = 0;
        /// <summary>
        /// Creator User Id
        /// </summary>
        [Required]
        [MaxLength(40)]
        public string CreatedById { get; set; } = string.Empty;
        /// <summary>
        /// Creator Name
        /// </summary>
        [Required]
        [MaxLength(66)]
        public string CreatedByName { get; set; } = string.Empty;
        /// <summary>
        /// Attachments if any for the email. Optional.
        /// </summary>
        public List<AddAttachmentRequestDTO>? Attachments { get; set; }
        /// <summary>
        /// Maximum number of recipients per batch email, system will split the email into multiple emails if recipients exceed this number. Optional.
        /// </summary>
        public int? MaxRecipientsPerBatch { get; set; }
        /// <summary>
        /// Module type of the email. Optional. E.g., 1 = LE, 2 = PV, 3 = TRS. This helps in categorizing the email.
        /// </summary>
        public short? ModuleType { get; set; }
    }

    /// <summary>
    /// Response DTO for Batch Email Send. If successful, returns the Batch Group Id of the sent emails. If failed, returns Error Message.
    /// </summary>
    public class SendBatchEmailResponseDTO
    {
        /// <summary>
        /// Batch Group Id of the sent emails
        /// </summary>
        public Guid? BatchGroupId { get; set; }
        /// <summary>
        /// Error message if sending failed
        /// </summary>
        public string? ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for Canceling Pending Batch Emails by Batch Group Id.
    /// </summary>
    public class CancelPendingBatchEmailsRequestDTO
    {
        public Guid BatchGroupId { get; set; }
        public string CancelReason { get; set; } = string.Empty;
        public string UpdatedById { get; set; } = string.Empty;
        public string UpdatedByName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response DTO for Canceling Pending Batch Emails. Indicates success or failure with an optional error message.
    /// </summary>
    public class CancelPendingBatchEmailsResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; } = string.Empty;
    }
}

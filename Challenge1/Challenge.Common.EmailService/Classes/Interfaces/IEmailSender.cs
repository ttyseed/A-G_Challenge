using challenge1.Common.EmailService.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace challenge1.Common.EmailService.Classes.Interfaces
{
    internal interface IEmailSender
    {
        Task SendEmailAsync(string subject, string? body, string recipients, string? cc, string? bcc, List<AttachmentDTO>? attachments, int? maxRecipientsPerBatch, CancellationToken cancellationToken = default);
    }
}

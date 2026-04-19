using challenge1.Common.EmailService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace challenge1.Common.EmailService.Repositories.Interfaces
{
    internal interface IEmailAttachmentRepository
    {
        Task<EmailAttachment?> AddEmailAttachmentAsync(EmailAttachment emailAttachment);
    }
}

using challenge1.Common.EmailService.Classes.Interfaces;
using challenge1.Common.EmailService.Data;
using challenge1.Common.EmailService.Models;
using challenge1.Common.EmailService.Repositories.Interfaces;

namespace challenge1.Common.EmailService.Repositories
{
    internal class EmailAttachmentRepository : IEmailAttachmentRepository
    {
        private readonly EmailDbContext _context;
        private readonly ILogging _logging;

        public EmailAttachmentRepository(EmailDbContext context, ILogging logging)
        {
            _context = context;
            _logging = logging;
        }

        public async Task<EmailAttachment?> AddEmailAttachmentAsync(EmailAttachment emailAttachment)
        {
            try
            {
                await _context.AddAsync(emailAttachment);
                await _context.SaveChangesAsync();
                return emailAttachment;
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
                return null;
            }
        }
    }
}

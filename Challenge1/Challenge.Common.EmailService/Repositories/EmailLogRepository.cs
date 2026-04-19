using challenge1.Common.EmailService.Classes.Interfaces;
using challenge1.Common.EmailService.Data;
using challenge1.Common.EmailService.Models;
using challenge1.Common.EmailService.Repositories.Interfaces;

namespace challenge1.Common.EmailService.Repositories
{
    internal class EmailLogRepository : IEmailLogRepository
    {
        private readonly EmailDbContext _context;
        private readonly ILogging _logging;

        public EmailLogRepository(EmailDbContext context, ILogging logging)
        {
            _context = context;
            _logging = logging;
        }

        public async Task<EmailLog?> AddEmailLogAsync(EmailLog emailLog, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.AddAsync(emailLog, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return emailLog;
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
                return null;
            }
        }
    }
}

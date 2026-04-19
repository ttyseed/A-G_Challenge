using Microsoft.EntityFrameworkCore;
using challenge1.Common.EmailService.Classes;
using challenge1.Common.EmailService.Classes.Interfaces;
using challenge1.Common.EmailService.Data;
using challenge1.Common.EmailService.Models;
using challenge1.Common.EmailService.Repositories.Interfaces;
using static challenge1.Common.EmailService.Classes.Constant;

namespace challenge1.Common.EmailService.Repositories
{
    internal class EmailRepository : IEmailRepository
    {
        private readonly EmailDbContext _context;
        private readonly ILogging _logging;

        public EmailRepository(EmailDbContext context, ILogging logging)
        {
            _context = context;
            _logging = logging;
        }

        public async Task<Email?> AddEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.AddAsync(email, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return email;
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
                return null;
            }
        }

        public async Task<bool> UpdateEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Emails.Update(email);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
                return false;
            }
        }

        public async Task<bool> UpdateEmailsAsync(IEnumerable<Email> emails, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Emails.UpdateRange(emails);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
                return false;
            }
        }

        // Return emails sorted (Priority desc, CreatedDate asc) then grouped so that
        // items with the same BatchGroupId appear together (preserving sort within groups).
        public async Task<IEnumerable<Email>?> GetBatchJobPendingEmailsAsync(Email? request = null, CancellationToken cancellationToken = default)
        {
            // Build base query for pending batch-job emails that are not deleted
            var query = _context.Emails.AsNoTracking()
                .Include(e => e.Attachments)
                .Where(e => e.Status == EmailStatus.Pending
                            && e.SendType == SendType.BatchJob
                            && !e.IsDeleted);

            // Apply module filter only if request and ModuleType provided
            if (request != null && request.ModuleType != null)
            {
                query = query.Where(e => e.ModuleType == request.ModuleType);
            }

            // Apply batch group filter only if request and BatchGroupId provided
            if (request != null && request.BatchGroupId != null)
            {
                // If BatchGroupId is provided, return only emails in that batch group. No sorting/grouping needed.
                return await query.Where(e => e.BatchGroupId == request.BatchGroupId).ToListAsync(cancellationToken);
            }

            // Apply sorting: priority desc, created date asc
            var sortedList = await query
                .OrderByDescending(e => e.Priority)
                .ThenBy(e => e.CreatedDate)
                .ToListAsync(cancellationToken);

            if (sortedList == null || sortedList.Count == 0)
                return null;

            // Group same BatchGroupId together while preserving initial order
            return sortedList
                .GroupBy(e => e.BatchGroupId)
                .SelectMany(g => g)
                .ToList();
        }

        /// <summary>
        /// Get failed batch emails that are eligible for retry. An email is eligible for retry if its RetryCount is less than MaxRetryCount.
        /// </summary>
        /// <returns>List of failed batch emails eligible for retry.</returns>
        public async Task<IEnumerable<Email>?> GetFailedBatchEmailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Emails.AsNoTracking()
                .Include(e => e.Attachments)
                .Where(e => e.Status == EmailStatus.Failed && e.SendType == SendType.BatchJob && !e.IsDeleted && e.RetryCount < EmailServiceSettings.MaxRetryCount)
                .ToListAsync(cancellationToken);
        }
    }
}

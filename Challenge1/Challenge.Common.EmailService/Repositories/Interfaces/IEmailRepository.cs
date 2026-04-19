using challenge1.Common.EmailService.Models;

namespace challenge1.Common.EmailService.Repositories.Interfaces
{
    public interface IEmailRepository
    {
        Task<Email?> AddEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<bool> UpdateEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<bool> UpdateEmailsAsync(IEnumerable<Email> emails, CancellationToken cancellationToken = default);
        Task<IEnumerable<Email>?> GetBatchJobPendingEmailsAsync(Email? request = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Email>?> GetFailedBatchEmailsAsync(CancellationToken cancellationToken = default);
    }
}

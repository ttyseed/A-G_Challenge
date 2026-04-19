using challenge1.Common.EmailService.DTOs;

namespace challenge1.Common.EmailService.Services.Interfaces
{
    public interface IEmailService
    {
        Task<List<GetPendingBatchEmailsResponseDTO>?> GetPendingBatchEmailsAsync(GetPendingBatchEmailsRequestDTO? request);
        Task<SendManualEmailResponseDTO> SendManualEmailAsync(SendManualEmailRequestDTO request);
        Task<SendBatchEmailResponseDTO> SendBatchEmailsAsync(List<SendBatchEmailRequestDTO> requests);
        Task<CancelPendingBatchEmailsResponseDTO> CancelPendingBatchEmailsAsync(CancelPendingBatchEmailsRequestDTO request, CancellationToken cancellationToken = default);
        internal Task<bool> TriggerBatchEmailsSendAsync(CancellationToken cancellationToken = default);
        internal Task<bool> RetryFailedBatchEmailsAsync(CancellationToken cancellationToken = default);
    }
}

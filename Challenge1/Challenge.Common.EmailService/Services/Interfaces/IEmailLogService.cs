using challenge1.Common.EmailService.DTOs;

namespace challenge1.Common.EmailService.Services.Interfaces
{
    internal interface IEmailLogService
    {
        Task<AddEmailLogResponseDTO?> AddEmailLogAsync(AddEmailLogRequestDTO request, CancellationToken cancellationToken = default);
    }
}

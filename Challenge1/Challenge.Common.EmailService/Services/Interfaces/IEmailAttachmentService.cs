using challenge1.Common.EmailService.DTOs;

namespace challenge1.Common.EmailService.Services.Interfaces
{
    internal interface IEmailAttachmentService
    {
        Task<AddAttachmentResponseDTO?> AddEmailAttachmentAsync(AddAttachmentRequestDTO request);
    }
}

using AutoMapper;
using challenge1.Common.EmailService.Classes.Interfaces;
using challenge1.Common.EmailService.DTOs;
using challenge1.Common.EmailService.Models;
using challenge1.Common.EmailService.Repositories.Interfaces;
using challenge1.Common.EmailService.Services.Interfaces;

namespace challenge1.Common.EmailService.Services
{
    internal class EmailLogService : IEmailLogService
    {
        private readonly IMapper _mapper;
        private readonly ILogging _logging;
        private readonly IEmailLogRepository _emailLogRepository;

        public EmailLogService(IMapper mapper, ILogging logging, IEmailLogRepository emailLogRepository)
        {
            _mapper = mapper;
            _logging = logging;
            _emailLogRepository = emailLogRepository;
        }

        public async Task<AddEmailLogResponseDTO?> AddEmailLogAsync(AddEmailLogRequestDTO request, CancellationToken cancellationToken = default)
        {
            try
            {
                var emailLog = _mapper.Map<EmailLog>(request);
                emailLog.CreatedDate = DateTime.Now;

                var response = await _emailLogRepository.AddEmailLogAsync(emailLog, cancellationToken);
                if (response == null)
                {
                    return null;
                }

                return new AddEmailLogResponseDTO { EmailLogId = response.EmailLogId };
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }
    }
}

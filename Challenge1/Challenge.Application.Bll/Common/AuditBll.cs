using AutoMapper;
using challenge1.Application.Bll.Common.Interfaces;
using challenge1.Application.DTO.Common;
using challenge1.Common.Logging;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Repositories.Interfaces;

namespace challenge1.Application.Bll.Common
{
    public class AuditBll : IAuditBll
    {
        private readonly IMapper _mapper;
        private readonly ILogging _logging;
        private readonly IAuditRepository _auditRepository;

        public AuditBll(IMapper mapper, ILogging logging, IAuditRepository auditRepository)
        {
            _mapper = mapper;
            _logging = logging;
            _auditRepository = auditRepository;
        }

        public async Task<Guid?> AddAuditAsync(AddAuditRequestDTO request)
        {
            try
            {
                var audit = _mapper.Map<Audit>(request);

                audit.AuditId = Guid.NewGuid();
                audit.IsVisible = true;

                var response = await _auditRepository.CreateAsync(audit);
                if (response == null)
                {
                    return null;
                }

                return audit.AuditId;
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }
    }
}

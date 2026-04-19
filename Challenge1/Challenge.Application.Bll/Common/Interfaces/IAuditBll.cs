using challenge1.Application.DTO.Common;

namespace challenge1.Application.Bll.Common.Interfaces
{
    public interface IAuditBll
    {
        Task<Guid?> AddAuditAsync(AddAuditRequestDTO request);
    }
}

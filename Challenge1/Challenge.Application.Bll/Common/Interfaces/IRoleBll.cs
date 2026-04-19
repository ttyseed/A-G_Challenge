using challenge1.Application.DTO.Common;
using challenge1.Application.Filter.Common;

namespace challenge1.Application.Bll.Common.Interfaces
{
    public interface IRoleBll
    {
        Task<List<GetRolesResponseDTO>?> GetRolesAsync(GetRolesFilter request);
        Task<GetRoleByIdResponseDTO?> GetRoleByIdAsync(Guid roleId);
        Task<Guid?> CreateRoleAsync(CreateRoleRequestDTO request);
        Task<bool> UpdateRoleAsync(UpdateRoleRequestDTO request);
        Task<bool> SoftDeleteRoleAsync(SoftDeleteRoleRequestDTO request);
    }
}

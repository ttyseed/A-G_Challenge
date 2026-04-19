using challenge1.Application.DTO.Common;

namespace challenge1.Application.Bll.Common.Interfaces
{
    public interface IUserRoleBll
    {
        Task<List<short>?> GetUserRoleNumbersByUserIdAsync(Guid userId);
        Task<bool> SoftDeleteUserRoleAsync(SoftDeleteUserRoleRequestDTO model);
        Task<bool> UpdateUserRoleAsync(UpdateUserRoleRequestDTO request);
    }
}

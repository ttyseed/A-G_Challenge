
using challenge1.Application.DTO.Common;
using challenge1.Application.Filter.Common;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Repositories.Base;

namespace challenge1.Database.Repositories.Repositories.Interfaces
{
    public interface IBackendUserRepository : IRepository<BackendUser>
    {
        Task<BackendUser?> GetBackendUserByLoginIdAsync(string loginId);

        Task<List<BackendUser>?> GetBackendUsersAsync(GetBackendUsersFilter request);
        Task<List<GetBackendUsersRolesResponseDTO>?> GetBackendUsersRolesAsync(Guid userId);

    }
}

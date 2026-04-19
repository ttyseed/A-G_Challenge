using challenge1.Application.Filter.Common;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Repositories.Base;

namespace challenge1.Database.Repositories.Repositories.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<List<Role>?> GetRolesAsync(GetRolesFilter request);
        Task<Role?> GetRoleByIdAsync(Guid roleId);
        Task<bool> SoftDeleteRoleAsync(Role role);
    }
}

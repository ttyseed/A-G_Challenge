using challenge1.Database.Models;
using challenge1.Database.Repositories.Repositories.Base;

namespace challenge1.Database.Repositories.Repositories.Interfaces
{
	public interface IUserRoleRepository : IRepository<UserRole>
    {
        Task<List<short>?> GetUserRoleNumbersByUserIdAsync(Guid userId);

    }
}

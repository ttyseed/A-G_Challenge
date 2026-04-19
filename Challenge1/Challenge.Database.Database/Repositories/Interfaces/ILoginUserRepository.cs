using challenge1.Application.Filter.Common;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Repositories.Base;

namespace challenge1.Database.Repositories.Repositories.Interfaces
{
    public interface ILoginUserRepository : IRepository<LoginUser>
    {
        Task<LoginUser?> GetLoginUserByLoginIdAsync(string loginId);

        Task<List<LoginUser>?> GetLoginUsersAsync(GetLoginUsersFilter request);

    }
}

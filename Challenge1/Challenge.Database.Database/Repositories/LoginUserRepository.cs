using Microsoft.EntityFrameworkCore;
using challenge1.Application.Filter.Common;
using challenge1.Common.Logging;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Context;
using challenge1.Database.Repositories.Repositories.Base;
using challenge1.Database.Repositories.Repositories.Interfaces;

namespace challenge1.Database.Repositories.Repositories
{
    public class LoginUserRepository : EntityFrameworkCoreRepository<LoginUser>, ILoginUserRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogging _logging;

        public LoginUserRepository(DatabaseContext context, ILogging logging) : base(context, logging)
        {
            _context = context;
            _logging = logging;
        }

        public async Task<LoginUser?> GetLoginUserByLoginIdAsync(string loginId)
        {
            return await FirstOrDefaultAsync(x => x.LoginId == loginId);
        }

        public async Task<List<LoginUser>?> GetLoginUsersAsync(GetLoginUsersFilter request)
        {
            try
            {
                return await _context.LoginUsers
                .Where(
                    o => (request.LoginUserId == null || !request.LoginUserId.Any() || request.LoginUserId.Contains(o.LoginUserId))
                    && (string.IsNullOrEmpty(request.LoginId) || request.LoginId == o.LoginId)
                    && (string.IsNullOrEmpty(request.LoginName) || !string.IsNullOrEmpty(o.LoginName) && EF.Functions.Like(o.LoginName, request.LoginName))
                    && (request.LoginType == null || request.LoginType == o.LoginType)
                    && (request.ReferenceId == null || request.ReferenceId == o.ReferenceId)
                    && (request.ReferenceType == null || request.ReferenceType == o.ReferenceType)
                    && (request.LastLoginCode == null || request.ReferenceType == o.ReferenceType)
                    && (string.IsNullOrEmpty(request.LastLoginCode) || request.LastLoginCode == o.LastLoginCode)
                    && (request.LastLoginDateStart == null || request.LastLoginDateEnd == null || o.LastLoginDate >= request.LastLoginDateStart && o.LastLoginDate <= request.LastLoginDateEnd)
                    && (request.LastLogoutDateStart == null || request.LastLogoutDateEnd == null || o.LastLogoutDate >= request.LastLogoutDateStart && o.LastLogoutDate <= request.LastLogoutDateEnd)
                    && (request.Status == null || request.Status == o.Status)
                    && (request.IsDeleted == null || request.IsDeleted == o.IsDeleted)
                ).ToListAsync();
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
            }

            return null;
        }

    }
}

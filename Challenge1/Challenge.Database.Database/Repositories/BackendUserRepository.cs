using Microsoft.EntityFrameworkCore;
using challenge1.Application.DTO.Common;
using challenge1.Application.Filter.Common;
using challenge1.Common.Logging;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Context;
using challenge1.Database.Repositories.Repositories.Base;
using challenge1.Database.Repositories.Repositories.Interfaces;

namespace challenge1.Database.Repositories.Repositories
{
    public class BackendUserRepository : EntityFrameworkCoreRepository<BackendUser>, IBackendUserRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogging _logging;

        public BackendUserRepository(DatabaseContext context, ILogging logging) : base(context, logging)
        {
            _context = context;
            _logging = logging;
        }

        public async Task<BackendUser?> GetBackendUserByLoginIdAsync(string loginId)
        {
            try
            {
				return await FirstOrDefaultAsync(x => x.LoginId == loginId);
			}
			catch (Exception ex)
			{
                _logging.LogRepoError(ex.ToString());
			}

            return null;
        }

        public async Task<List<BackendUser>?> GetBackendUsersAsync(GetBackendUsersFilter request)
        {
            try
            {
                return await _context.BackendUsers.AsNoTracking()
				.Where(
					o => (request.Status == null || request.Status == o.Status)
					&& (request.UserId == null || !request.UserId.Any() || request.UserId.Contains(o.UserId))
					&& (string.IsNullOrEmpty(request.LoginId) || request.LoginId == o.LoginId)
					&& (string.IsNullOrEmpty(request.FullName) || !string.IsNullOrEmpty(o.FullName) && EF.Functions.Like(o.FullName, request.FullName))
					&& (string.IsNullOrEmpty(request.EmailAddress) || !string.IsNullOrEmpty(o.EmailAddress) && EF.Functions.Like(o.EmailAddress, request.EmailAddress))
                    && (!request.IsDeleted.HasValue || request.IsDeleted == o.IsDeleted)
				).ToListAsync();
			}
			catch (Exception ex)
			{
                _logging.LogRepoError(ex.ToString());
			}

            return null;
        }

        public async Task<List<GetBackendUsersRolesResponseDTO>?> GetBackendUsersRolesAsync(Guid userId)
        {
            try
            {
                return await _context.BackendUsers.Where(bu => !bu.IsDeleted && bu.UserId == userId)
                                                   .SelectMany(bu => bu.UserRoles.Where(ur => !ur.IsDeleted)
                                                    .Select(ur => new GetBackendUsersRolesResponseDTO
                                                    {
                                                        UserId = bu.UserId,
                                                        FullName = bu.FullName,
                                                        TelephoneNo = bu.TelephoneNo,
                                                        EmailAddress = bu.EmailAddress,
                                                        Status = bu.Status.ToString(),
                                                        Role = ur.Role.RoleName
                                                    })).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
            }

            return null;
        }
    }
}

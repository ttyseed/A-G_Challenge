using Microsoft.EntityFrameworkCore;
using challenge1.Application.Filter.Common;
using challenge1.Common.Logging;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Context;
using challenge1.Database.Repositories.Repositories.Base;
using challenge1.Database.Repositories.Repositories.Interfaces;

namespace challenge1.Database.Repositories.Repositories
{
    public class RoleRepository : EntityFrameworkCoreRepository<Role>, IRoleRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogging _logging;

        public RoleRepository(DatabaseContext context, ILogging logging) : base(context, logging)
        {
            _context = context;
            _logging = logging;
        }

        public async Task<List<Role>?> GetRolesAsync(GetRolesFilter request)
		{
			try
			{
                return await _context.Roles.AsNoTracking()
                .Where
                    (
                        x => !x.IsDeleted &&
                        (request.RoleNumber == null || x.RoleNumber == request.RoleNumber) &&
                        (request.RoleName == null || x.RoleName.Contains(request.RoleName.Trim())) &&
                        (request.UserType == null || x.UserType == request.UserType) &&
                        (request.ModuleType == null || x.ModuleType == request.ModuleType)
                    )
                .OrderBy(x => x.RoleName)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
            }

            return null;
		}

		public async Task<Role?> GetRoleByIdAsync(Guid roleId)
		{
			return await FirstOrDefaultAsync(m => m.RoleId == roleId && !m.IsDeleted);
		}

        public async Task<bool> SoftDeleteRoleAsync(Role role)
        {
            try
            {
                role.IsDeleted = true;
                role.UpdatedDate = DateTime.Now;
                await UpdateAsync(role);
                return true;
            }
            catch (Exception ex)
            {
                _logging.LogRepoError(ex.ToString());
                return false;
            }
        }
	}
}

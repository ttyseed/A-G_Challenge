using Microsoft.EntityFrameworkCore;
using challenge1.Common.Logging;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Context;
using challenge1.Database.Repositories.Repositories.Base;
using challenge1.Database.Repositories.Repositories.Interfaces;
using System.Data;

namespace challenge1.Database.Repositories.Repositories
{
	public class UserRoleRepository : EntityFrameworkCoreRepository<UserRole>, IUserRoleRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogging _logging;

		public UserRoleRepository(DatabaseContext context, ILogging logging) : base(context, logging)
		{
			_context = context;
			_logging = logging;
        }

        public async Task<List<short>?> GetUserRoleNumbersByUserIdAsync(Guid userId)
		{
			return await _context.UserRoles.AsNoTracking()
				.Where(x => !x.IsDeleted && x.UserId == userId)
				.Select(x => x.RoleNumber)
				.ToListAsync();
		}
    }
}

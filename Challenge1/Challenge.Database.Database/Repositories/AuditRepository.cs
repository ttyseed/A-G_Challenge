using Microsoft.EntityFrameworkCore;
using challenge1.Application.Filter.Common;
using challenge1.Common.Logging;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Context;
using challenge1.Database.Repositories.Repositories.Base;
using challenge1.Database.Repositories.Repositories.Interfaces;

namespace challenge1.Database.Repositories.Repositories
{
    public class AuditRepository : EntityFrameworkCoreRepository<Audit>, IAuditRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogging _logging;

		public AuditRepository(DatabaseContext context, ILogging logging) : base(context, logging)
		{
			_context = context;
			_logging = logging;
        }

        public async Task<List<Audit>?> GetAuditsAsync(GetAuditsFilter request)
		{
			try
			{
				return await _context.Audits.AsNoTracking()
					.Where
						(
							x =>
							(request.ActionCode == null || x.ActionCode.Equals(request.ActionCode.Trim(), StringComparison.CurrentCultureIgnoreCase)) &&
							(request.ActionName == null || x.ActionName.Contains(request.ActionName.Trim(), StringComparison.CurrentCultureIgnoreCase))
						)
					.OrderBy(x => x.CreatedDate)
				.ToListAsync();
			}
			catch (Exception ex)
			{
                _logging.LogRepoError(ex.ToString());
			}

			return null;
		}
	}
}

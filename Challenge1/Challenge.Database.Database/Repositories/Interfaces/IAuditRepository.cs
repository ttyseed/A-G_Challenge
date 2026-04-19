using challenge1.Application.Filter.Common;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Repositories.Base;

namespace challenge1.Database.Repositories.Repositories.Interfaces
{
    public interface IAuditRepository : IRepository<Audit>
    {
        Task<List<Audit>?> GetAuditsAsync(GetAuditsFilter request);
    }
}

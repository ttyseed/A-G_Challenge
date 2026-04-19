using System.Linq.Expressions;

namespace challenge1.Database.Repositories.Repositories.Base
{
    public interface IRepository<TEntity> where TEntity : class
    {

        Task<TEntity> CreateAsync(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity, object key);

        Task<TEntity?> GetByIdAsync(int id);

        Task<TEntity?> GetByIdAsync(Guid id);

        Task<List<TEntity>?> GetListAsync();

        Task<bool> AnyAsync();

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where);

        Task<long> CountAsync();

        Task<long> CountAsync(Expression<Func<TEntity, bool>> where);

        Task<TEntity?> FindAsync(object key);

        Task<TEntity?> FirstOrDefaultAsync(params Expression<Func<TEntity, object>>[] properties);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] properties);

        Task<TEntity?> LastOrDefaultAsync(params Expression<Func<TEntity, object>>[] properties);

        Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] properties);

        Task<IEnumerable<TEntity?>> ListAsync(params Expression<Func<TEntity, object>>[] properties);

        Task<IEnumerable<TEntity?>> ListAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] properties);


    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using challenge1.Common.Logging;
using challenge1.Database.Models;
using System.Linq.Expressions;
using System.Text.Json;
using challenge1.Common.Util.Common;
using static challenge1.Common.Util.Common.Constant.Audit;

namespace challenge1.Database.Repositories.Repositories.Base
{
    public class EntityFrameworkCoreRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected EntityFrameworkCoreRepository(DbContext context, ILogging logging)
        {
            _context = context;
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _logging = logging;
        }

        private DbSet<TEntity> Set => _context.Set<TEntity>();
        private readonly DbContext _context;
        private readonly ILogging _logging;

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            try
            {
                await Set.AddAsync(entity);
                await SaveChangesAsync();
                return entity;
            }
            catch (Exception ex) when (ex is DbUpdateException or OperationCanceledException)
            {
                _logging.LogDatabaseError(ex.ToString());
                throw;
            }
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(entity);

                _context.Entry(entity).State = EntityState.Modified;
                await SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logging.LogDatabaseError(ex.ToString());
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logging.LogDatabaseError(ex.ToString());
                throw;
            }
            catch (OperationCanceledException ex)
            {
                _logging.LogDatabaseError(ex.ToString());
                throw;
            }
        }

        public async Task<bool> UpdateAsync(TEntity entity, object key)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(key);
                ArgumentNullException.ThrowIfNull(entity);

                var entityContext = Set.Find(key);

                if (entityContext != null)
                {
                    _context.Entry(entityContext).CurrentValues.SetValues(entity);
                    await SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logging.LogDatabaseError(ex.ToString());
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logging.LogDatabaseError(ex.ToString());
                throw;
            }
            catch (OperationCanceledException ex)
            {
                _logging.LogDatabaseError(ex.ToString());
                throw;
            }
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await Set.FindAsync(id);
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await Set.FindAsync(id);
        }

        public async Task<List<TEntity>?> GetListAsync()
        {
            return await Set.ToListAsync();
        }

        public async Task<bool> AnyAsync()
        {
            return await Set.AnyAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where)
        {
            return await Set.AnyAsync(where);
        }

        public async Task<long> CountAsync()
        {
            return await Set.LongCountAsync();
        }

        public async Task<long> CountAsync(Expression<Func<TEntity, bool>> where)
        {
            return await Set.LongCountAsync(where);
        }

        public async Task<TEntity?> FindAsync(object key)
        {
            ArgumentNullException.ThrowIfNull(key);

            return await Set.FindAsync(key);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(params Expression<Func<TEntity, object>>[] properties)
        {
            IQueryable<TEntity> query = Set;

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] properties)
        {
            IQueryable<TEntity> query = Set;

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.FirstOrDefaultAsync(where);
        }

        public async Task<TEntity?> LastOrDefaultAsync(params Expression<Func<TEntity, object>>[] properties)
        {
            IQueryable<TEntity> query = Set;

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.LastOrDefaultAsync();
        }

        public async Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] properties)
        {
            IQueryable<TEntity> query = Set;

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.LastOrDefaultAsync(where);
        }

        public async Task<IEnumerable<TEntity?>> ListAsync(params Expression<Func<TEntity, object>>[] properties)
        {
            IQueryable<TEntity> query = Set;

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity?>> ListAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] properties)
        {
            IQueryable<TEntity> query = Set;

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.Where(where).ToListAsync();
        }

        #region SaveChanges Async with Auto Audit functionality
        /// <summary>
        /// Customize SaveChangesAsync to include audit logging functionality. 
        /// DONT USE DBContext.SaveChangesAsync() directly, USE Repo.SaveChangesAsync() or directly SaveChangesAsync() if you are in the repository already.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Call your custom audit logic before saving
            await AddAuditsAsync(cancellationToken);

            return await _context.SaveChangesAsync(cancellationToken);
        }

        protected virtual async Task AddAuditsAsync(CancellationToken cancellationToken)
        {
            var auditEntries = new List<Audit>();

            foreach (var entry in _context.ChangeTracker.Entries())
            {
                if (entry.Entity is Audit)
                    continue;

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    var audit = CreateAuditEntry(entry);
                    if (audit != null)
                        auditEntries.Add(audit);
                }
            }

            if (auditEntries.Count > 0)
            {
                _context.Set<Audit>().AddRange(auditEntries);
            }

            await Task.CompletedTask;
        }

        private Audit CreateAuditEntry(EntityEntry entry)
        {
            string tableName = entry.Metadata.GetTableName() ?? "";
            string? sourceMethod = AuditContextProvider.SourceMethod; //Source Method is the OnPostFunction or OnGetFunction in the Razor Page or Controller Action Method (Example: OnPostDeleteAll)
            string actionType = ""; //This will be set based on AuditActionTypeMapper if found, otherwise will be set to URL + Source Method
            var pageDomainURL = ""; //This will get the first two segments of the URL (Example: /Declaration/Submit)

            #region Get Page Domain URL from AuditContextProvider.Url 
            string input = AuditContextProvider.Url ?? "";

            var parts = input.Split('/', StringSplitOptions.RemoveEmptyEntries);

            pageDomainURL = parts.Length >= 2 ? $"/{parts[0]}/{parts[1]}" : AuditContextProvider.Url; //Here will get the first two segments of the URL (Example: Declaration/Submit)
            #endregion

            #region Get Source Method from Stack Trace if not provided in AuditContextProvider
            if (string.IsNullOrEmpty(sourceMethod))
            {
                //1. Captures the current call stack (all methods leading to this point).
                var stackTrace = new System.Diagnostics.StackTrace();

                //2. Searches through the stack frames to find the first method that belongs to either a "Pages" or "Controllers" namespace.
                var frame = stackTrace.GetFrames()
                    ?.FirstOrDefault(f => f.GetMethod()?.DeclaringType?.Namespace?.Contains("Pages") == true
                                       || f.GetMethod()?.DeclaringType?.Namespace?.Contains("Controllers") == true);

                //3. Extracts the name of that method to use as the source method for auditing.
                sourceMethod = frame?.GetMethod()?.Name ?? "";
            }
            #endregion

            #region Determine Action Type based on AuditActionTypeMapper
            if (!string.IsNullOrEmpty(pageDomainURL))
            {
                var URL = pageDomainURL + "/";
                //Get Audit Type from AuditActionTypeMapper based on URL + sourceMethod
                if (AuditActionTypeMapper.ActionTypeKey.TryGetValue(URL + sourceMethod, out var actionTypeKey))
                {
                    actionType = actionTypeKey;
                }
                else
                {
                    actionType = pageDomainURL + "/" + sourceMethod; // Combine both to get full URL with Source Method (Example: /Declaration/Submit/OnPostDeleteAll)
                }
            }
            #endregion

            var audit = new Audit
            {
                ActionCode = actionType,
                ActionName = actionType,
                CreatedDate = DateTime.Now,
                CreatedByName = AuditContextProvider.CurrentUserName ?? "System",
                CreatedById = AuditContextProvider.CurrentUserId ?? "SYSTEM",
                UserAgent = AuditContextProvider.UserAgent,
                Url = AuditContextProvider.Url,
                Referer = AuditContextProvider.Referer,
                IsVisible = true,
                IP = AuditContextProvider.IPAddress,
            };

            // Primary key (single column)
            var pk = entry.Metadata.FindPrimaryKey();
            if (pk != null)
            {
                var keyValues = pk.Properties.Select(p => entry.Property(p.Name).CurrentValue).ToArray();
                if (keyValues.Length == 1 && Guid.TryParse(keyValues[0]?.ToString(), out var id))
                    audit.ItemId = id.ToString();
            }

            // Serialize entity values
            if (entry.State == EntityState.Added)
            {
                audit.ItemContent = SerializeValues(entry.CurrentValues);
            }
            else if (entry.State == EntityState.Modified)
            {
                audit.ItemContent = SerializeValues(entry.CurrentValues);
                var dbValues = entry.GetDatabaseValues();
                if (dbValues != null)
                    audit.ItemContentHistory = SerializeValues(dbValues);
            }
            else if (entry.State == EntityState.Deleted)
            {
                audit.ItemContentHistory = SerializeValues(entry.OriginalValues);
            }

            return audit;
        }

        private string SerializeValues(PropertyValues values)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var prop in values.Properties)
            {
                if (!prop.IsShadowProperty() && !prop.IsForeignKey())
                    dict[prop.Name] = values[prop.Name];
            }

            return JsonSerializer.Serialize(dict);
        }
        #endregion
    }
}

using TaskFlow.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace TaskFlow.Data.Repository.Common
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal TaskFlowContext context;
        internal DbSet<TEntity> dbSet;
        public BaseRepository(TaskFlowContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> GetWithRawSql(string query,
            params object[] parameters)
        {
            return null;
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }
        public virtual IList<TEntity> GetAll()
        {
            return dbSet.ToList();
        }
        public virtual async Task<IList<TEntity>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
            context.SaveChanges();
        }
        public virtual async Task<bool> InsertAsync(TEntity entity)
        {
            try
            {
                await dbSet.AddAsync(entity);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }
        public virtual EntityEntry<TEntity> InsertVal(TEntity entity)
        {
            var val = dbSet.Add(entity);
            context.SaveChanges();
            return val;
        }
        public virtual async Task<EntityEntry<TEntity>> InsertValAsync(TEntity entity)
        {
            try
            {
                var val = await dbSet.AddAsync(entity);
                context.SaveChanges();
                return val;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
            context.SaveChanges();
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            context.Entry(entityToDelete).CurrentValues["IsDeleted"] = true;
            var Dependencies = context.Entry(entityToDelete).Navigations.Where(x => !((IReadOnlyNavigation)x.Metadata).IsOnDependent);
            DeleteDependency(Dependencies);

            Update(entityToDelete);
        }
        public virtual void DeleteDependency(IEnumerable<NavigationEntry> NaventityToDelete)
        {
            foreach (var navigationEntry in NaventityToDelete)
            {
                if (navigationEntry is CollectionEntry collectionEntry && collectionEntry.CurrentValue != null)
                {
                    foreach (var dependentEntry in collectionEntry.CurrentValue)
                    {
                        try
                        {
                            var Dependencies = context.Entry(navigationEntry).Navigations.Where(x => !((IReadOnlyNavigation)x.Metadata).IsOnDependent);
                            DeleteDependency(Dependencies);
                        }
                        catch (Exception)
                        { }
                        context.Entry(dependentEntry).CurrentValues["IsDeleted"] = true;
                    }
                }
                else
                {
                    var dependentEntry = navigationEntry.CurrentValue;
                    if (dependentEntry != null)
                    {
                        try
                        {
                            var Dependencies = context.Entry(navigationEntry).Navigations.Where(x => !((IReadOnlyNavigation)x.Metadata).IsOnDependent);
                            DeleteDependency(Dependencies);
                        }
                        catch (Exception)
                        { }
                        context.Entry(dependentEntry).CurrentValues["IsDeleted"] = true;
                    }
                }
            }
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            try
            {
                dbSet.Update(entityToUpdate);
                context.SaveChanges();
            }
            catch (Exception)
            {

            }
        }
        public virtual async Task<bool> UpdateAsync(TEntity entityToUpdate)
        {
            try
            {
                
                dbSet.Update(entityToUpdate);
               var ok = await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public virtual async Task<bool> UpdateListAsync(List<TEntity> entityToUpdate)
        {
            try
            {
                dbSet.UpdateRange(entityToUpdate);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public virtual async Task<bool> InsertListAsync(List<TEntity> entityToUpdate)
        {
            try
            {
                dbSet.AddRange(entityToUpdate);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string HashPassword(string password)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, 16, 100000, HashAlgorithmName.SHA256))
            {
                byte[] salt = deriveBytes.Salt;
                byte[] key = deriveBytes.GetBytes(32);
                return Convert.ToBase64String(salt.Concat(key).ToArray());
            }
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var hashBytes = Convert.FromBase64String(storedHash);
            var salt = hashBytes.Take(16).ToArray();
            var storedKey = hashBytes.Skip(16).ToArray();

            using (var deriveBytes = new Rfc2898DeriveBytes(enteredPassword, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] newKey = deriveBytes.GetBytes(32);
                return CryptographicOperations.FixedTimeEquals(newKey, storedKey);
            }
        }
        public virtual async Task<int> SoftDeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    if (context.Entry(entity).State == EntityState.Detached)
                        dbSet.Attach(entity);

                    context.Entry(entity).CurrentValues["IsDeleted"] = true;
                }

                return await context.SaveChangesAsync();
            }
            catch
            {
                return 0;
            }
        }
        public virtual async Task<int> SoftDeleteListAsync(List<TEntity> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    if (context.Entry(entity).State == EntityState.Detached)
                        dbSet.Attach(entity);

                    context.Entry(entity).CurrentValues["IsDeleted"] = true;
                }

                return await context.SaveChangesAsync();
            }
            catch
            {
                return 0;
            }
        }
        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            try
            {
                if (context.Entry(entity).State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }

                dbSet.Remove(entity);

                return await context.SaveChangesAsync();
            }
            catch
            {
                return 0;
            }
        }
    }
}

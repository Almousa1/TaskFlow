using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Data.Repository.Common
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Delete(TEntity entityToDelete);
        void Delete(object id);
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
        TEntity GetByID(object id);
        IList<TEntity> GetAll();
        IEnumerable<TEntity> GetWithRawSql(string query,
            params object[] parameters);
        void Insert(TEntity entity);
        Task<bool> InsertAsync(TEntity entity);
        EntityEntry<TEntity> InsertVal(TEntity entity);
        Task<EntityEntry<TEntity>> InsertValAsync(TEntity entity);
        void Update(TEntity entityToUpdate);
        Task<bool> UpdateAsync(TEntity entity);
        public string HashPassword(string password);
        public bool VerifyPassword(string enteredPassword, string storedHash);
    }

}

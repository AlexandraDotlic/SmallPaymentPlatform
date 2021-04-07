using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task Insert(TEntity entity);
        Task InsertMany(ICollection<TEntity> entities);
        Task<bool> Update(TEntity entity);
        Task<bool> Delete(TEntity entity);
        Task<TEntity> GetById(params object[] ids);
        Task<IReadOnlyCollection<TEntity>> GetAllList();
        Task<IReadOnlyCollection<TEntity>> GetAllWithIncludesAsList(params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> GetFirstOrDefaultWithIncludes(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includePropertyExpressions);
        Task<IReadOnlyCollection<TEntity>> SearchByWithIncludes(Expression<Func<TEntity, bool>> searchBy, params Expression<Func<TEntity, object>>[] includes);
        Task<IReadOnlyCollection<TEntity>> GetFilteredList(Expression<Func<TEntity, bool>> filter);
    }
}

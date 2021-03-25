using Common.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreDataAccess.Repositories
{
    public class EfCoreRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal EfCoreDbContext _context;
        internal DbSet<TEntity> DbSet;

        public EfCoreRepository(EfCoreDbContext context)
        {
            _context = context;
            DbSet = context.Set<TEntity>();
        }

        public virtual async Task Insert(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual async Task InsertMany(ICollection<TEntity> entities)
        {
            if (entities.Count > 0)
            {
                await DbSet.AddRangeAsync(entities);
            }
        }

        public virtual async Task<bool> Update(TEntity entity)
        {
            try
            {
                DbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;

                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
        }

        public virtual async Task<bool> Delete(TEntity entity)
        {
            DbSet.Remove(entity);
            return await Task.FromResult(true);
        }

        public Task<TEntity> GetById(params object[] id)
        {
            return DbSet.FindAsync(id).AsTask();
        }

        public virtual async Task<IReadOnlyCollection<TEntity>> GetAllList()
        {
            return await DbSet.ToListAsync();
        }
        public virtual async Task<IReadOnlyCollection<TEntity>> GetFilteredList(Expression<Func<TEntity, bool>> filter)
        {
            return await DbSet.Where(filter).ToListAsync();
        }
        public virtual async Task<IReadOnlyCollection<TEntity>> GetAllWithIncludesAsList(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> result = DbSet.Where(i => true);

            foreach (Expression<Func<TEntity, object>> includeExpression in includes)
                result = result.Include(includeExpression);

            return await result.ToListAsync();
        }

        public async Task<TEntity> GetFirstOrDefaultWithIncludes(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            return await AddMultipleIncludesToQuery(DbSet, includePropertyExpressions)
                .FirstOrDefaultAsync(filter);
        }

        protected IQueryable<TEntity> AddMultipleIncludesToQuery(IQueryable<TEntity> initialQuery, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            IQueryable<TEntity> queryWithIncludes = includePropertyExpressions.Aggregate(initialQuery, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));
            return queryWithIncludes;
        }


        public virtual async Task<IReadOnlyCollection<TEntity>> SearchByWithIncludes(Expression<Func<TEntity, bool>> searchBy, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> result = DbSet.Where(searchBy);

            foreach (Expression<Func<TEntity, object>> includeExpression in includes)
                result = result.Include(includeExpression);

            return await result.ToListAsync();
        }
        public virtual void Dispose()
        {
            _context.Dispose();
        }
    }
}

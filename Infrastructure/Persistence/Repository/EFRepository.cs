using Domain.Interfaces;
using HRShared.Common;
//using AccountManagementDomain.Shared;
using Infrastructure.Helpers;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repository
{
    public class EfRepository<T, M> : IAsyncRepository<T, M> where T : class
    {
        private readonly ApplicationDbContext _dbContext;

        public EfRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> GetByIdAsync(M id)
        {
            return await _dbContext.Set<T>().FindAsync(id) ?? default!;
        }

        public virtual async Task<T> GetByAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync() ?? default!;
        }

        public virtual async Task<T> GetByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> set = _dbContext.Set<T>();
            foreach (var includeExpression in includeExpressions)
            {
                set = set.Include(includeExpression);
            }
            T result = await set.FirstOrDefaultAsync(predicate) ?? default!;
            return result;
        }

        public virtual async Task<T> GetFirstAsync()
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync() ?? default!;
        }

        public virtual async Task<T> GetFirstAsync(params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> set = _dbContext.Set<T>();
            foreach (var includeExpression in includeExpressions)
            {
                set = set.Include(includeExpression);
            }
            T result = await set.FirstOrDefaultAsync() ?? default!;
            return result;
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> ListAllAsync(params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> set = _dbContext.Set<T>();
            foreach (var includeExpression in includeExpressions)
            {
                set = set.Include(includeExpression);
            }
            return await set.AsNoTracking().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<PagedResult<T>> ListWithPagingAsync(int currentPage, int pageSize, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> set = _dbContext.Set<T>();
            if (includeExpressions != null && includeExpressions.Count() > 0)
            {
                foreach (var includeExpression in includeExpressions)
                {
                    set = set.Include(includeExpression);
                }

            }

            var results = await set.AsNoTracking().Where(predicate).GetPagedAsync(currentPage, pageSize);
            return results;
        }

        public async Task<PagedResult<T>> ListWithPagingAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? orderBy = null, bool isAscending = false, int pageIndex = 1, int pageSize = 20, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            query = query.Where(predicate);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (orderBy != null && orderBy.Count() > 0)
            {
                IOrderedQueryable<T>? queryWithOrder = null;

                if (isAscending)
                {
                    queryWithOrder = query.OrderBy(orderBy[0]);
                }
                else
                {
                    queryWithOrder = query.OrderByDescending(orderBy[0]);
                }

                if (orderBy.Count() > 1)
                {
                    for (int i = 1; i < orderBy.Count(); i++)
                    {
                        if (isAscending)
                        {
                            queryWithOrder = queryWithOrder.ThenBy(orderBy[i]);
                        }
                        else
                        {
                            queryWithOrder = queryWithOrder.ThenByDescending(orderBy[i]);
                        }
                    }
                }

                return await queryWithOrder.GetPagedAsync(pageIndex, pageSize);
            }

            return await query.GetPagedAsync(pageIndex, pageSize);
        }


        public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[]? includeExpressions)
        {
            IQueryable<T> set = _dbContext.Set<T>();
            if (includeExpressions != null && includeExpressions.Count() > 0)
            {
                foreach (var includeExpression in includeExpressions)
                {
                    set = set.Include(includeExpression);
                }
            }

            IReadOnlyList<T> results = await set.AsNoTracking().Where(predicate).ToListAsync();
            return results;
        }

        public async Task<List<T>> ListAllFromStoredProcedureAsync(string sql, object[] parameters)
        {
            return await _dbContext.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();
        }


        public async Task<decimal> FindSum(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> select)
        {
            var sum = await _dbContext.Set<T>().AsNoTracking()
             .AnyAsync(predicate) ? await _dbContext.Set<T>().AsNoTracking()
             .Where(predicate).Select(select).SumAsync() : 0;
            return sum;
        }

        /// <summary>
        /// Finds the sum.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="select">The select.</param>
        /// <returns>System.Decimal.</returns>
        public async Task<decimal> FindSum(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> select)
        {
            var sum = await _dbContext.Set<T>().AsNoTracking()
              .Where(predicate).Select(select).DefaultIfEmpty(0).SumAsync();
            return sum;
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);

        }


        public async Task AddListAsync(List<T> entity)
        {
            await _dbContext.Set<T>().AddRangeAsync(entity);
        }


        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void DeleteList(List<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void UpdateListAsync(List<T> entity)
        {
            _dbContext.Set<T>().UpdateRange(entity);
        }


        public Task UpdateAsync(T entity)
        {
            _dbContext.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
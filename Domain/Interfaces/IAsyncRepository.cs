using Domain.Shared;
using HRShared.Common;
using System.Linq.Expressions;


namespace Domain.Interfaces
{
    public interface IAsyncRepository<T, M> where T : class
    {
        Task<T> GetByIdAsync(M id);
        Task<T> GetByAsync(ISpecification<T> spec);

        Task<T> GetByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeExpressions);

        Task<T> GetFirstAsync();
        Task<T> GetFirstAsync(params Expression<Func<T, object>>[] includeExpressions);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<IReadOnlyList<T>> ListAllAsync(params Expression<Func<T, object>>[] includeExpressions);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[]? includeExpressions);
        Task<PagedResult<T>> ListWithPagingAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? orderBy = null, bool isAscending = false, int pageIndex = 1, int pageSize = 20, params Expression<Func<T, object>>[] includes);
        Task<PagedResult<T>> ListWithPagingAsync(int currentPage, int pageSize, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeExpressions);
        Task<List<T>> ListAllFromStoredProcedureAsync(string sql, object[] parameters);

        Task<decimal> FindSum(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> select);

        Task<decimal> FindSum(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> select);

        //Task<T> AddAsync(T entity);
        void Add(T entity);

        Task AddAsync(T entity);
        Task AddListAsync(List<T> entity);

        void UpdateListAsync(List<T> entity);
        void Update(T entity);
        //Task DeleteAsync(T entity);
        void Delete(T entity);
        void DeleteList(List<T> entities);
        Task SaveChangesAsync();
        Task<int> CountAsync(ISpecification<T> spec);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity);
    }
}
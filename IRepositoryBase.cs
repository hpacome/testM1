
namespace Interfaces
{
    public interface IRepositoryBase<T, TKey>
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        T GetById(TKey id);
        Task<T> GetByIdAsync(TKey id);

        T GetByFilter(Expression<Func<T, bool>> filter);

        IQueryable<T> GetListByFilter(Expression<Func<T, bool>> filter);
        Task<IQueryable<T>> GetListByFilterAsync(Expression<Func<T, bool>> filter);
        int GetCount();
        Task<int> GetCountAsync();

        int GetCount(Expression<Func<T, bool>> filter);
        Task<int> GetCountAsync(Expression<Func<T, bool>> filter);

        void Create(T entity);
        Task<T> CreateAsync(T entity);

        void Create(List<T> entities);
        Task CreateAsync(List<T> entities);

        int Update(T entity);
        Task<int> UpdateAsync(T entity);

        int Delete(TKey id);
        Task<int> DeleteAsync(TKey id);

        int Delete(T entity);
        Task<int> DeleteAsync(T entity);

        Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities);
        Task<int> DeleteRangeAsync(System.Linq.Expressions.Expression<Func<T, bool>> filter);
        Task<int> DeleteRangeAsync(IEnumerable<T> entities);
    }
}

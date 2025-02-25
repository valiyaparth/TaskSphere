using System.Linq.Expressions;

namespace TaskSphere.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] includeProperties);

        Task<T> GetAsync(Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] includeProperties);

        Task<T> AddAsync(T entity);

        void Delete(T entity);
    }
}
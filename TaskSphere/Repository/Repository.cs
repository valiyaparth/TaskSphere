using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskSphere.Data;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class Repository<T> : IRepository<T> where T : class //T - model ( either task or user or team or project)
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, 
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;

            if(filter != null)
            {
                query = query.Where(filter);
            }

            if(includeProperties != null && includeProperties.Length > 0)
            {
                foreach(var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.ToListAsync();
        }

        public Task<T> GetAsync(Expression<Func<T, bool>> filter = null, 
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null && includeProperties.Length > 0)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefaultAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }
    }
}

using System.Linq.Expressions;

namespace UserService.Application {
    public interface IRepository<TEntity> where TEntity : class {
        Task<TEntity?> GetAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IQueryable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
    }
}

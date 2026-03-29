using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserService.Application;

namespace UserService.Infrastructure {
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(AppDbContext context) {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<IQueryable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate) => _dbSet.Where(predicate);

        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

        public void Remove(TEntity entity) => _dbSet.Remove(entity);
    }
}

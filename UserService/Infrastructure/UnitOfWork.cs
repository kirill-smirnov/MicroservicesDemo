using UserService.Application;
using UserService.Domain.Entities;

namespace UserService.Infrastructure {
    public class UnitOfWork : IUnitOfWork {
        private readonly AppDbContext _context;

        public IRepository<User> Users { get; }

        public UnitOfWork(AppDbContext context) {
            _context = context;
            Users = new Repository<User>(_context);
        }

        public async Task<int> CommitAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}

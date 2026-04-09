using UserService.Domain.Entities;

namespace UserService.Application.Interfaces {
    public interface IUnitOfWork : IDisposable {
        IRepository<User> Users { get; }
        Task<int> CommitAsync();
    }
}

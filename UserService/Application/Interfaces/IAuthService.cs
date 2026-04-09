using UserService.Domain.Entities;

namespace UserService.Application.Interfaces {
    public interface IAuthService {
        string GenerateToken(User user);
        Task<User> GetByEmailAsync(string email);
    }
}
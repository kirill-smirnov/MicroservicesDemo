using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Services {
    public class AuthService : IAuthService {
        private readonly string _key;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(string key, IUnitOfWork unitOfWork) {
            _key = key;
            _unitOfWork = unitOfWork;
        }

        public string GenerateToken(User user) {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetByEmailAsync(string email) {
            var collection = await _unitOfWork.Users.Find(u => u.Email == email);

            return await collection.SingleOrDefaultAsync();
        }
    }
}

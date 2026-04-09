using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTO;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Events;
using UserService.Infrastructure;

namespace UserService.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase {

        private readonly RabbitMQPublisher _publisher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public UsersController(RabbitMQPublisher publisher, IUnitOfWork unitOfWork, IAuthService authService){
            _publisher = publisher;
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Create(UserCreatedDto dto) {

            var user = new User {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CommitAsync();    

            var @event = new UserCreatedEvent {
                UserId = user.Id,
                Email = user.Email
            };

            await _publisher.PublishUserCreatedAsync(@event);

            return Ok(dto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto) {
            var user = await _authService.GetByEmailAsync(dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized();

            var token = _authService.GenerateToken(user);

            return Ok(new { token });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync() {
            var users = await _unitOfWork.Users.GetAllAsync();

            return Ok(users);
        }
    }
}

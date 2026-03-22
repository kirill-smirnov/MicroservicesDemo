using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Entities;
using UserService.Domain.Events;
using UserService.Infrastructure;

namespace UserService.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase {
        private static readonly List<User> _users = new();

        private readonly RabbitMQPublisher _publisher;

        public UsersController(RabbitMQPublisher publisher) {
            _publisher = publisher;
        }

        [HttpPost]
        public async Task<IActionResult> Create(User request) {
            request.Id = Guid.NewGuid();
            _users.Add(request);

            var @event = new UserCreatedEvent {
                UserId = request.Id,
                Email = request.Email
            };

            await _publisher.PublishUserCreatedAsync(@event);

            return Ok(request);
        }

        [HttpGet]
        public IActionResult Get() {
            return Ok(_users);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using UserService.Application;
using UserService.Domain.Entities;
using UserService.Domain.Events;
using UserService.Infrastructure;

namespace UserService.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase {

        private readonly RabbitMQPublisher _publisher;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(RabbitMQPublisher publisher, IUnitOfWork unitOfWork){
            _publisher = publisher;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Create(User request) {

            await _unitOfWork.Users.AddAsync(request);
            await _unitOfWork.CommitAsync();    

            var @event = new UserCreatedEvent {
                UserId = request.Id,
                Email = request.Email
            };

            await _publisher.PublishUserCreatedAsync(@event);

            return Ok(request);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() {
            var users = await _unitOfWork.Users.GetAllAsync();

            return Ok(users);
        }
    }
}

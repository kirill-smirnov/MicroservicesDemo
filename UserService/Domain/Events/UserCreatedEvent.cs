namespace UserService.Domain.Events {
    public class UserCreatedEvent {
        public Guid UserId { get; set; }
        public string Email { get; set; }
    }
}

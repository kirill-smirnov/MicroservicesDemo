using UserService.Domain.Interfaces;

namespace UserService.Domain.Entities {
    public class User : IEntityWithId<Guid>, ICreatedAt, IUpdatedAt {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTimeOffset CreatedAt { get;  set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}

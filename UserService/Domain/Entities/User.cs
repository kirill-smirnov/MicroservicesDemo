namespace UserService.Domain.Entities {
    public class User : IEntityWithId<Guid> {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}

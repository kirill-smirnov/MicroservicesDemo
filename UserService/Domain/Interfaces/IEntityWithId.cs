namespace UserService.Domain.Interfaces {
    public interface IEntityWithId<TId> {
        TId Id { get; }
    }
}

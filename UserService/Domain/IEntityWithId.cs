namespace UserService.Domain {
    public interface IEntityWithId<TId> {
        TId Id { get; }
    }
}

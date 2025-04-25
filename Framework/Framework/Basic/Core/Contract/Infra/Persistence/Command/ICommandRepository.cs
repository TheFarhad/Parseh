namespace Framework;

public interface ICommandRepository<TAggregateRoot, TId> : IRepository
    where TAggregateRoot : AggregateRoot<TId>
    where TId : Identity
{ }

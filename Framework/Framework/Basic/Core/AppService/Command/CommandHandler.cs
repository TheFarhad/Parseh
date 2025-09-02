namespace Framework;

public interface ICommandHandler<in TCommand>
    : IRequestHandler<TCommand>
    where TCommand : ICommand
{ }

public interface ICommandHandler<in TCommand, TData>
    : IRequestHandler<TCommand, TData>
    where TCommand : ICommandRequest<TData>
{ }

public abstract class CommandRequestHandler<TCommand>
    : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    protected readonly IUnitOfWork UnitOfWork;

    // TODO: write default methods

    public CommandRequestHandler(IUnitOfWork unitOfWork)
        => UnitOfWork = unitOfWork;

    public abstract Task HandleAsync(TCommand command, CancellationToken token = default);
}

public abstract class CommandRequestHandler<TCommand, TData>
    : ICommandHandler<TCommand, TData>
    where TCommand : ICommandRequest<TData>
{
    protected readonly IUnitOfWork UnitOfWork;

    // TODO: write default methods

    public CommandRequestHandler(IUnitOfWork unitOfWork)
        => UnitOfWork = unitOfWork;

    public abstract Task<Response<TData>> HandleAsync(TCommand command, CancellationToken token = default);
}

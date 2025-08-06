namespace Framework;

public interface ICommandRequestHandler<in TCommandRequest>
    : IRequestHandler<TCommandRequest>
    where TCommandRequest : ICommandRequest
{ }

public interface ICommandRequestHandler<in TCommandRequest, TData>
    : IRequestHandler<TCommandRequest, TData>
    where TCommandRequest : ICommandRequest<TData>
{ }

public abstract class CommandRequestHandler<TCommandRequest>
    : ICommandRequestHandler<TCommandRequest>
    where TCommandRequest : ICommandRequest
{
    protected readonly IUnitOfWork UnitOfWork;

    // TODO: write default methods

    public CommandRequestHandler(IUnitOfWork unitOfWork)
        => UnitOfWork = unitOfWork;

    public abstract Task HandleAsync(TCommandRequest command, CancellationToken token = default);

    Task IRequestHandler<TCommandRequest>.HandleAsync(TCommandRequest request, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}

public abstract class CommandRequestHandler<TCommandRequest, TData>
    : ICommandRequestHandler<TCommandRequest, TData>
    where TCommandRequest : ICommandRequest<TData>
{
    protected readonly IUnitOfWork UnitOfWork;

    // TODO: write default methods

    public CommandRequestHandler(IUnitOfWork unitOfWork)
        => UnitOfWork = unitOfWork;

    public abstract Task<Response<TData>> HandleAsync(TCommandRequest command, CancellationToken token = default);
}

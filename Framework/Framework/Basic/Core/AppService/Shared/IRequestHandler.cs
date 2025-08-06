namespace Framework;

public interface IRequestHandler<in TRequest>
{
    Task HandleAsync(TRequest request, CancellationToken token = default);
}

public interface IRequestHandler<in TRequest, TData>
{
    Task<Response<TData>> HandleAsync(TRequest command, CancellationToken token = default);
}
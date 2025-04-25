namespace Framework;

public interface IRequest { }
public interface IRequest<out TData> : IRequest { }

public abstract class RequestPipe : Pipe<RequestPipe>
{
    public abstract Task SendAsync<TRequest>(TRequest request, CancellationToken? token = null)
        where TRequest : IRequest;

    public abstract Task<Response<TData>> SendAsync<TRequest, TData>(TRequest request, CancellationToken? token = null)
        where TRequest : IRequest<TData>;

    protected async Task InvokeChainAsync<TRequest>(TRequest request, CancellationToken? token = null)
        where TRequest : IRequest
    {
        if (HasChain) await Chain.SendAsync<TRequest>(request, token);
    }

    protected async Task<Response<TData>>? InvokeChainAsync<TRequest, TData>(TRequest request, CancellationToken? token = null)
        where TRequest : IRequest<TData>
    {
        Response<TData> result = default!;
        if (HasChain) result = await Chain.SendAsync<TRequest, TData>(request, token);
        return result;
    }
}

public sealed class RequestValidator : RequestPipe
{
    // TODO: by IFluentValidator
    public override Task SendAsync<TRequest>(TRequest request, CancellationToken? token = null)
    {
        throw new NotImplementedException();
    }

    public override Task<Response<TData>> SendAsync<TRequest, TData>(TRequest request, CancellationToken? token = null)
    {
        throw new NotImplementedException();
    }
}

public sealed class RequestHandler : RequestPipe
{
    // TODO: by IServiceProvider
    public override Task SendAsync<TRequest>(TRequest request, CancellationToken? token = null)
    {
        throw new NotImplementedException();
    }

    public override Task<Response<TData>> SendAsync<TRequest, TData>(TRequest request, CancellationToken? token = null)
    {
        throw new NotImplementedException();
    }
}


namespace Framework;

public interface IQueryRequestHandler<in TQueryRequest, TData>
    : IRequestHandler<TQueryRequest, TData>
    where TQueryRequest : IQueryRequest<TData>
{ }
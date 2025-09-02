namespace Framework;

public interface IQueryHandler<in TQuery, TData>
    : IRequestHandler<TQuery, TData>
    where TQuery : IQuery<TData>
{ }
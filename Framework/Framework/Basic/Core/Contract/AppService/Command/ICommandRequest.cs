namespace Framework;

public interface ICommandRequest : IRequest { }
public interface ICommandRequest<out TData> : IRequest<TData> { }
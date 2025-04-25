namespace Framework;

public interface IRequestHandler<in TRequest> { }
public interface IRequestHandler<in TRequest, TData> { }
namespace Framework;

public interface ICommandBase { }
public interface ICommand : IRequest, ICommandBase { }
public interface ICommandRequest<out TData> : IRequest<TData>, ICommandBase { }
namespace Framework;

public abstract class ServiceException(string message)
    : Exception($"{message}.")
{ }
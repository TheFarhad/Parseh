namespace Framework;

public abstract class PersistenceException(string message)
    : ServiceException(TemplatedMessage.Format(message))
{
    private const string TemplatedMessage = "At Infra.Persistence: \n\t[{0}]";
}

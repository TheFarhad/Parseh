namespace Framework;

public abstract class ExternalApiException(string message)
    : ServiceException(TemplatedMessage.Format(message))
{
    private const string TemplatedMessage = "At Infra.ExternalApiException: \n\t[{0}]";
}

namespace Framework;

public abstract class EndpointException(string message)
    : ServiceException(TemplateMessage.Format(message))
{
    private const string TemplateMessage = "At Endpoint: \n\t[{0}]";
}

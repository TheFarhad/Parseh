namespace Framework;

internal class AppServiceException(string message) : ServiceException(MessageTemplate.Format(message))
{
    private const string MessageTemplate = "At AppService: \n\t[{0}]";
}

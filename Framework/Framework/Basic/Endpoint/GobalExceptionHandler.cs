namespace Framework;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

public sealed class GobalExceptionHandler(ILogger<GobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken token)
    {
#if DEBUG
        _logger.LogError(exception, "Unhandled exception occurred: \n\t[{Message}]", exception.Message);
#endif
        var error = GetError(exception);
        await httpContext.Response.WriteAsJsonAsync(error, token);
        return true;
    }

    private Error GetError(Exception exception)
       => exception is ServiceException ? Error.BadRequest(exception.Message) :
                                          Error.Server(exception.Message, "Internal Server Error");
}

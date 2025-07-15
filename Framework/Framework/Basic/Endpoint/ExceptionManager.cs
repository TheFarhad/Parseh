namespace Framework;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

public sealed class ExceptionManager(ILogger<ExceptionManager> logger) : IExceptionHandler
{
    private readonly ILogger<ExceptionManager> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken token)
    {
#if DEBUG
        _logger.LogError(exception, "Unhandled exception occurred: \n\t[{Message}]", exception.Message);
#endif
        var error = GetError(exception);
        httpContext.Response.StatusCode = error.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(error, token);
        return true;
    }

    Error GetError(Exception exception)
      => exception is ServiceException
        ?
        Error.BadRequest(exception.Message) // TODO: آیا اینکه دقیقا خطا را به کلاینت اراسل کنیم کار اشتباهی نیست؟؟
                                            //Error.BadRequest("Not Found", String.Empty)
        :
        Error.Server(exception.Message, "Internal Server Error");
    //Error.Server("Internal Server Error", String.Empty    );
}

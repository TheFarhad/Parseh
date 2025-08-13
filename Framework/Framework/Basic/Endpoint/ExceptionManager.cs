namespace Framework;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

public sealed class ExceptionManager(ILogger<ExceptionManager> logger) : IExceptionHandler
{
    readonly ILogger<ExceptionManager> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken token)
    {
        // TODO: جزئیات خطا به صورت کامل لاگ شود
        //      ولی فقط یک پیغام کلی برای کلاینت ارسال شود که جزئیات در آن نباشد

        _logger.LogError(exception, "Unhandled exception occurred: \n\t[{Message}]", exception.Message);

        var error = GetError(exception);
        httpContext.Response.StatusCode = error.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(error, token);
        return true;
    }

    Error GetError(Exception exception)
     =>
#if DEBUG
        Error.Server(exception.Message, String.Empty);
#else
        Error.Server("Internal Server Error", String.Empty);
#endif

}

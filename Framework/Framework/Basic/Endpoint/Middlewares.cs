namespace Framework;

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class ApiError
{
    public string Id { get; set; }
    public short Status { get; set; }
    public string Code { get; set; }
    public string Links { get; set; }
    public string Title { get; set; }
    public string Detail { get; set; }
}

public class ApiExceptionOptions
{
    public Action<HttpContext, Exception, ApiError> AddResponseDetails { get; set; }
    public Func<Exception, LogLevel> DetermineLogLevel { get; set; }
}

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ISerializeService _serializer;
    private readonly ILogger<ApiExceptionMiddleware> _logger;
    private readonly ApiExceptionOptions _options;

    public ApiExceptionMiddleware(RequestDelegate next, ApiExceptionOptions options, ILogger<ApiExceptionMiddleware> logger, ISerializeService serializer)
    {
        _next = next;
        _options = options;
        _logger = logger;
        _serializer = serializer;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var error = new ApiError
        {
            Id = Guid.NewGuid().ToString(),
            Status = (short)HttpStatusCode.InternalServerError,
            Title = "SOME_KIND_OF_ERROR_OCCURRED_IN_THE_API"
        };

        _options.AddResponseDetails?.Invoke(context, exception, error);

        var innerExMessage = GetInnermostExceptionMessage(exception);

        var level = _options.DetermineLogLevel?.Invoke(exception) ?? LogLevel.Error;

        _logger.Log(level, exception, "BADNESS!!! " + innerExMessage + " -- {ErrorId}.", error.Id);

        var result = _serializer.Serialize(error);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsync(result);
    }

    private string GetInnermostExceptionMessage(Exception exception) =>
             exception.InnerException is not null
            ?
            GetInnermostExceptionMessage(exception.InnerException)
            :
            exception.Message;
}




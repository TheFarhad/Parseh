namespace Framework;

using FluentValidation;
using Microsoft.Extensions.Logging;

public interface IRequest { }
public interface IRequest<out TData> : IRequest { }

public abstract class RequestController : Pipe<RequestController>
{
    public abstract Task SendAsync<TRequest>(TRequest request, CancellationToken token = default)
        where TRequest : IRequest;

    public abstract Task<Response<TData>> SendAsync<TRequest, TData>(TRequest request, CancellationToken token = default)
        where TRequest : IRequest<TData>;

    protected async Task InvokeChainAsync<TRequest>(TRequest request, CancellationToken token = default)
        where TRequest : IRequest
    {
        if (HasChain) await Chain.SendAsync<TRequest>(request, token);
    }

    protected async Task<Response<TData>>? InvokeChainAsync<TRequest, TData>(TRequest request, CancellationToken token = default)
        where TRequest : IRequest<TData>
    {
        Response<TData> result = default!;
        if (HasChain) result = await Chain.SendAsync<TRequest, TData>(request, token);
        return result;
    }
}

public sealed class RequestValidator(ILogger<RequestValidator> logger, IServiceProvider service)
    : RequestController
{
    private readonly IServiceProvider _serviceProvider = service;
    private readonly ILogger<RequestValidator> _logger = logger;

    public override async Task SendAsync<TRequest>(TRequest request, CancellationToken token = default)
    {
        var type = RequestType(request);

        //_logger.LogDebug("Validating request of type {RequestType} With value {request}  start at :{StartDateTime}", type, request, DateTime.Now);

        var errors = Validate(request);
        if (errors is { })
        {
            //StringBuilder messages = new();
            //errors.ForEach(_ =>
            //{
            //    messages.AppendLine(_.Message);
            //});

            //_logger.LogInformation("Validating Request of type {RequestType} With value {Request} failed. Validation errors are: {ValidationErrors}", request, request, messages);
            return;
        }
        //_logger.LogDebug("Validating request of type {RequestType} With value {request}  finished at :{EndDateTime}", type, request, DateTime.Now);
        await InvokeChainAsync(request);
    }

    public override async Task<Response<TData>> SendAsync<TRequest, TData>(TRequest command, CancellationToken token = default!)
    {
        //LogStart(command, type);

        Response<TData> result = default!;
        var type = RequestType(command);
        var errors = Validate(command);

        if (errors is { })
        {
            //StringBuilder messages = new();
            //errors.ForEach(_ =>
            //{
            //    messages.AppendLine(_.Message);
            //});
            //LogError(command, type, messages);
            result = errors;
        }
        else
        {
            //LogSuccess(command, type);
            result = await InvokeChainAsync<TRequest, TData>(command, token);
        }
        return result;
    }

    Type RequestType<TRequest>(TRequest request)
        => request!.Type();

    void LogStart<TRequest>(TRequest request, Type requestType)
        => _logger
             .LogDebug("Validating request of type {RequestType} With value {Request}  start at :{StartDateTime}", requestType, request, DateTime.Now);

    void LogError<TRequest>(TRequest request, Type requestType, params string[] errors)
        => _logger.LogInformation("Validating request of type {RequestType} With value {Request}  failed. Validation errors are: {ValidationErrors}", requestType, request, errors);

    void LogSuccess<TRequest>(TRequest request, Type requestType)
        => _logger
                .LogDebug("Validating request of type {RequestType} With value {Request} finished at :{EndDateTime}", requestType, request, DateTime.Now);

    List<Error>? Validate<TRequest>(TRequest request)
    {
        List<Error> result = default!;

        var validator = _serviceProvider.GetService<IValidator<TRequest>>();
        if (validator is { })
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                result = [];

                validationResult
                    .Errors
                    .ForEach(_ =>
                    {
                        result.Add(Error.BadRequest(_.ErrorMessage));
                    });
            }
        }
        else
        {
            //_logger.LogInformation("There is not any validator for {RequestType}", RequestType(request));
        }
        return result;
    }
}

public sealed class RequestExecutor : RequestController
{
    private readonly IServiceProvider _provider;
    //private readonly ILogger<Executor> _logger;
    //private readonly Stopwatch _timer;

    public RequestExecutor(IServiceProvider provider/*, ILogger<Executor> logger*/)
    {
        _provider = provider;
        //_logger = logger;
        //_timer = new Stopwatch();
    }

    public override async Task SendAsync<TRequest>(TRequest request, CancellationToken token = default!)
    {
        var type = request.Type();
        //_timer.Start();
        try
        {
            //LogStart(command, type);

            // TODO: لزومی ندارد که چند تا هندلر برای یک ریکوئست وجود داشته باشد     
            //       برای ایونت ها مساله ای نیست

            List<Task> tasks = [];
            _provider
               .GetServices<IRequestHandler<TRequest>>()
               .ToList()
               .ForEach(_ => tasks.Add(_.HandleAsync(request, token)));

            await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            //LogError(e, type);
            throw;
        }
        finally
        {
            //_timer.Stop();
            //LogFinal(type);
        }
    }

    public override async Task<Response<TData>> SendAsync<TRequest, TData>(TRequest request, CancellationToken token = default!)
    {
        var type = request.Type();
        //_timer.Start();
        try
        {
            //LogStart(request, type);
            return await _provider
                            .GetRequiredService<IRequestHandler<TRequest, TData>>()!
                            .HandleAsync(request, token);
        }
        catch (Exception e)
        {
            //LogError(e, type);
            throw;
        }
        finally
        {
            //LogFinal(type);
            //_timer.Stop();
        }
    }

    //void LogStart<TRequest>(TRequest request, Type requestType)
    //    => _logger.LogDebug("Routing request of type {RequestType} With value {Request}  Start at {StartDateTime}", requestType, request, DateTime.Now);

    //void LogError(Exception exception, Type requestType)
    //    => _logger.LogError(exception, "There is not suitable handler for {RequestType} Routing failed at {StartDateTime}.", requestType, DateTime.Now);

    //void LogFinal(Type requestType)
    //    => _logger.LogInformation("Processing the {RequestType} request tooks {Millisecconds} Millisecconds", requestType, _timer.ElapsedMilliseconds);
}



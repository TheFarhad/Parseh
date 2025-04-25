namespace Framework;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[Controller]")]
public abstract class ApiControl : ControllerBase
{
    protected RequestPipe RequestPipe => HttpContext.RequestPipeService();

    protected async Task<IResult> GetAsync<TRequest, TOutput>(TRequest source, CancellationToken token = default!)
       where TRequest : IRequest<TOutput>
    {
        var response = await RequestPipe.SendAsync<TRequest, TOutput>(source, token);
        return Json(response);
    }

    protected async Task<IResult> PostAsync<TRequest, TOutput>(TRequest command, CancellationToken token = default!)
        where TRequest : IRequest<TOutput>
    {
        var response = await RequestPipe.SendAsync<TRequest, TOutput>(command, token);
        return Json(response);
    }

    protected async Task<IResult> PutAsync<TRequest, TOutput>(TRequest command, CancellationToken token = default!)
        where TRequest : IRequest<TOutput>
    {
        var response = await RequestPipe.SendAsync<TRequest, TOutput>(command, token);
        return Json(response);
    }

    protected async Task<IResult> DeleteAsync<TRequest, TOutput>(TRequest command, CancellationToken token = default!)
        where TRequest : IRequest<TOutput>
    {
        var response = await RequestPipe.SendAsync<TRequest, TOutput>(command, token);
        return Json(response);
    }

    private IResult Json<TOutput>(Response<TOutput> source)
        => Results.Json<Response<TOutput>>(source);
}

namespace Framework;

//using Microsoft.AspNetCore.Mvc;
public static class ResponseExtensions
{
    // TODO: کامل شود
    public static IResult JsonOutput<TData>(this Response<TData> response)
        => response.ErrorType switch
        {
            ErrorType.None => Results.Ok(response),
            ErrorType.Conflict => Results.Conflict(response),
            ErrorType.NotFound => Results.NotFound(response),
            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.Forbidden => Results.Forbid(),
            ErrorType.InternalServer => Results.InternalServerError(response),
            ErrorType.BadRequest => Results.BadRequest(response),
            ErrorType.Unknown => Results.InternalServerError(response),
            _ => Results.Ok(response),
        };
}

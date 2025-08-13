namespace Framework;

public readonly record struct Response<TData>
{
    public readonly TData? Data = default!;
    public readonly List<Error> Errors = [];
    public bool HasError => Errors.Count > 0;
    public bool Success => Errors.Count <= 0 && Data is not null;
    public readonly ErrorType ErrorType = ErrorType.None;

    public Response() { }
    public Response(TData data)
    {
        Data = data;
        Errors = [];
    }
    public Response(Error error)
    {
        Data = default!;
        Errors = [error];
        ErrorType = error.Type;
    }
    public Response(List<Error> errors)
    {
        Data = default!;
        Errors = [.. errors];
        ErrorType = errors.First().Type;
    }

    public static Response<TData> NoContent() => new();

    public static implicit operator Response<TData>(TData data)
        => new(data);

    public static implicit operator Response<TData>(Error error)
        => new(error);

    public static implicit operator Response<TData>(List<Error> errors)
        => new(errors);
}

public readonly record struct Error
{
    public readonly int StatusCode;
    public readonly string Message;
    public readonly string Description = String.Empty;
    public readonly ErrorType Type;

    private Error(ErrorType type, int statuscode, string message, string description)
    {
        StatusCode = statuscode;
        Message = message;
        Description = description;
        Type = type;
    }

    public static Error Server(string message, string description = "")
       => new(ErrorType.InternalServer, 500, message, description);

    public static Error BadRequest(string message, string description = "")
        => new(ErrorType.BadRequest, 400, message, description);

    public static Error Conflict(string message, string description = "")
        => new(ErrorType.Conflict, 409, message, description);

    public static Error NotFound(string message, string description = "")
        => new(ErrorType.NotFound, 404, message, description);

    public static Error Unauthorized(string message, string description = "")
        => new(ErrorType.Unauthorized, 401, message, description);

    public static Error Forbidden(string message, string description = "")
        => new(ErrorType.Forbidden, 403, message, description);

    public static Error Unknown(string message, string description = "")
        => new(ErrorType.Unknown, 520, message, description);
}

public enum ErrorType : byte
{
    None = 0,
    Conflict = 1,
    NotFound = 2,
    Unauthorized = 3,
    Forbidden = 4,
    InternalServer = 5,
    BadRequest = 6,
    Unknown = 7
}

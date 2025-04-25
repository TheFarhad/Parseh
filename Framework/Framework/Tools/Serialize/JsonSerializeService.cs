namespace Framework;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

public sealed class JsonSerializeService : ISerializeService
{
    private readonly JsonSerializerOptions _options;
    private readonly ILogger<NewtonSoftSerializeService> _logger;

    public JsonSerializeService(ILogger<NewtonSoftSerializeService> logger)
    {
        _options = new()
        {
            PropertyNameCaseInsensitive = true
        };
        _options.Converters.Add(new JsonStringEnumConverter());
        _logger = logger;
        _logger.LogInformation("Json Serializer start working");
    }

    public string Serialize(object source)
    {
        //LogSerizlie(source);
        return source is { } ? JsonSerializer.Serialize(source, _options) : String.Empty;
    }

    public string Serialize<TSource>(TSource source)
    {
        //LogSerizlie(source);
        return source is { } ? JsonSerializer.Serialize(source, _options) : String.Empty;
    }

    public object? Deserialize(string source, Type type)
    {
        //LogDeserizlie(source, type);
        return source is { } ? JsonSerializer.Deserialize(source, type, _options) : default;
    }

    public TOutput? Deserialize<TOutput>(string source)
    {
        //LogDeserizlie(source, source.Type());
        return source is { } ? JsonSerializer.Deserialize<TOutput>(source, _options) : default;
    }

    public void Dispose()
        => _logger.LogInformation("Json Serializer Stop working");

    private void LogSerizlie(object source)
        => _logger.LogTrace("Json Serializer Serialize with name {source}", source);

    private void LogDeserizlie(string source, Type type)
        => _logger.LogTrace("Json Serializer Deserialize with name {source} and type {type}", source, type);
}
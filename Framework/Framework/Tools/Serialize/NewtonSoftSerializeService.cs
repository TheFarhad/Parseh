namespace Framework;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

public sealed class NewtonSoftSerializeService : ISerializeService
{
    private readonly JsonSerializerSettings _settings;
    private readonly ILogger<NewtonSoftSerializeService> _logger;

    public NewtonSoftSerializeService(ILogger<NewtonSoftSerializeService> logger)
    {
        _settings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
        };
        _settings.Converters.Add(new StringEnumConverter());
        _logger = logger;
        _logger.LogInformation("Newton Soft Serializer start working");
    }

    public string Serialize(object source)
    {
        //LogSerizlie(source);
        return source is { } ? JsonConvert.SerializeObject(source, _settings) : String.Empty;
    }

    public string Serialize<TSource>(TSource source)
    {
        //LogSerizlie(source);
        return source is { } ? JsonConvert.SerializeObject(source, _settings) : String.Empty;
    }

    public object? Deserialize(string source, Type type)
    {
        //LogDeserizlie(source, type);
        return source is { } ? JsonConvert.DeserializeObject(source, _settings) : default;
    }

    public TOutput? Deserialize<TOutput>(string source)
    {
        //LogDeserizlie(source, source.GetType());
        return source is { } ? JsonConvert.DeserializeObject<TOutput>(source, _settings) : default;
    }

    public void Dispose()
        => _logger.LogInformation("Newton Soft Serializer Stop working");

    private void LogSerizlie(object source)
        => _logger.LogTrace("Newton Soft Serializer Serialize with name {source}", source);

    private void LogDeserizlie(string source, Type type)
        => _logger.LogTrace("Newton Soft Serializer Deserialize with name {source} and type {type}", source, type);
}
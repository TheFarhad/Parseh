namespace Framework;

using NetJSON;
using Microsoft.Extensions.Logging;

public sealed class NetJsonSerializeService : ISerializeService
{
    private readonly NetJSONSettings _settings;
    //private readonly ILogger<NetJsonSerializeService> _logger;

    public NetJsonSerializeService(/*ILogger<NetJsonSerializeService> logger*/)
    {
        _settings = new()
        {
            UseEnumString = true,
            UseStringOptimization = true,
            Format = NetJSONFormat.Prettify
        };
        //_logger = logger;
    }

    public string Serialize(object source)
    {
        //LogSerizlie(source);
        return source is { } ? NetJSON.Serialize(source.GetType(), source, _settings) : String.Empty;
    }

    public object? Deserialize(string source, Type type)
    {
        //LogDeserizlie(source, type);
        return source is { } ? NetJSON.Deserialize(source.GetType(), source, _settings) : default;
    }

    public string Serialize<TSource>(TSource source)
    {
        //LogSerizlie(source);
        return source is { } ? NetJSON.Serialize<TSource>(source, _settings)
                             : String.Empty;
    }

    public TOutput? Deserialize<TOutput>(string source)
    {
        TOutput? x = default!;
        try
        {
            x = source is { } ? NetJSON.Deserialize<TOutput>(source, _settings)
                           : default;
        }
        catch (Exception e)
        {
        }
        return x;
    }

    public TOutput? Deserializep<TOutput>(string source)
    {
        //LogSerizlie(source);
        return source is { } ? NetJSON.Deserialize<TOutput>(source, _settings)
                             : default;
    }

    public void Dispose()
    {
        //_logger.LogInformation("Json Serializer Stop working");
    }

    //private void LogSerizlie(object source)
    //    => _logger
    //         .LogTrace("NetJson Serializer Serialize with name {source}", source);

    //private void LogDeserizlie(string source, Type type)
    //    => _logger
    //        .LogTrace("NetJson Serializer Deserialize with name {source} and type {type}", source, type);
}

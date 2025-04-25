namespace Framework;

// TODO: complete service_registration

//using Microsoft.Extensions.Configuration;
//using StackExchange.Redis;
//using Tool.Serialize;

//public sealed class RedisServiceRegistry
//{
//    private readonly IConnectionMultiplexer _redis;
//    private readonly IDatabase _db;
//    private readonly IConfiguration _configuration;
//    private readonly ISerializeService _serializer;

//    public RedisServiceRegistry(IConnectionMultiplexer redis, IConfiguration configuration, ISerializeService serializer)
//    {
//        _redis = redis;
//        _db = _redis.GetDatabase();
//        _configuration = configuration;
//        _serializer = serializer;
//    }

//    public async Task Register(string url)
//    {
//        var host = Host(url);
//        var hostname = host.Item1;
//        var port = host.Item2;
//        var key = RedisKey();

//        var baseSrv = BaseServiceData();
//        var instanceInfo = new ServiceInstance(Guid.NewGuid().ToString(), hostname, port);
//        var serviceData = new Service(baseSrv.ClusterId, baseSrv.RouteId, baseSrv.RouteMatch, baseSrv.PathPattern, [instanceInfo]);

//        Services? result = default;
//        if (KeyExists(key))
//        {
//            result = await GetServices<Services>(key);

//            var currentService = result?
//                .ServicesData
//                .SingleOrDefault(_ => _.ClusterId.Equals(baseSrv.ClusterId, StringComparison.OrdinalIgnoreCase));

//            if (currentService is { })
//            {
//                var alreadyExist = currentService
//                    .Instances
//                    .Exists(_ => _.Host == hostname && _.Port == port);

//                if (!alreadyExist)
//                    currentService.Instances.Add(instanceInfo);
//            }
//            else
//            {
//                result?
//                    .ServicesData
//                    .Add(serviceData);
//            }
//        }
//        else
//        {
//            result = new Services
//            {
//                ServicesData = [serviceData]
//            };
//        }
//        var jsonData = _serializer.Serialize(result);
//        await _db.StringSetAsync(key, jsonData);
//    }

//    public async Task Deregister(string url)
//    {
//        var key = RedisKey();
//        if (key is { })
//        {
//            var registeredSerivices = await GetServices<Services>(key);
//            if (registeredSerivices is { })
//            {
//                var host = Host(url);
//                var hostname = host.Item1;
//                var port = host.Item2;
//                var currentAppService = BaseServiceData();

//                var currrentRegsiteredService = registeredSerivices
//                    .ServicesData
//                    .SingleOrDefault(_ => _.ClusterId == currentAppService.ClusterId);

//                var currrentRegsiteredServiceInstances = currrentRegsiteredService?.Instances;

//                var currentRegisteredInstance = currrentRegsiteredServiceInstances
//                    .SingleOrDefault(i => i.Host == hostname && i.Port == port);

//                if (currentRegisteredInstance is { })
//                {
//                    currrentRegsiteredServiceInstances?.Remove(currentRegisteredInstance);
//                    if (currrentRegsiteredServiceInstances?.Count == 0)
//                    {
//                        registeredSerivices
//                            .ServicesData
//                            .Remove(currrentRegsiteredService!);
//                    }
//                }

//                if (registeredSerivices.ServicesData.Count > 0)
//                {
//                    var data = _serializer.Serialize(registeredSerivices);
//                    await _db.StringSetAsync(key, data);
//                }
//                else
//                {
//                    await _db.KeyDeleteAsync(key);
//                }
//            }
//        }
//        await Dispose();
//    }

//    public string RedisKey()
//        => _configuration.GetValue<string>("SR_KEY_REDIS")!;

//    public bool KeyExists(string key)
//        => _db.KeyExists(key);

//    public async Task<TOutput?> GetServices<TOutput>(string key)
//    {
//        var data = await ServicesInJson(key);
//        return data.HasValue ? Deserialize<TOutput>(data!) : default!;
//    }

//    private async Task<RedisValue> ServicesInJson(string key)
//        => await _db.StringGetAsync(key);

//    private TOutput? Deserialize<TOutput>(string source)
//        => _serializer.Deserialize<TOutput>(source);

//    private BaseService BaseServiceData()
//    => new BaseService
//    {
//        ClusterId = _configuration.GetValue<string>("Service:ClusterId")!,
//        RouteId = _configuration.GetValue<string>("Service:RouteId")!,
//        RouteMatch = _configuration.GetValue<string>("Service:RouteMatch")!,
//        PathPattern = _configuration.GetValue<string>("Service:PathPattern")!
//    };

//    private ValueTuple<string, ushort> Host(string url)
//    {
//        var host = url.Split('/')[2].Split(':');
//        var hostname = host[0];
//        var port = ushort.Parse(host[1]);

//        return (hostname, port);
//    }

//    private async Task Dispose()
//        => await _redis.DisposeAsync();
//}

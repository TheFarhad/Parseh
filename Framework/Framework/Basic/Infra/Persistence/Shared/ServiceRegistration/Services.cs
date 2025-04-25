namespace Framework;

public sealed record Services
{
    public List<Service> ServicesData { get; init; } = [];
}

public sealed record Service
{
    public string ClusterId { get; init; } = null!;
    public string RouteId { get; init; } = null!;
    public string RouteMatch { get; init; } = null!;
    public string PathPattern { get; init; } = null!;
    public List<ServiceInstance> Instances { get; init; } = [];

    public Service(string clusterId, string routeName, string upstreamRoutePattern, string pathPattern, List<ServiceInstance> instances)
    {
        ClusterId = clusterId;
        RouteId = routeName;
        RouteMatch = upstreamRoutePattern;
        PathPattern = pathPattern;
        Instances = instances;
    }
}

public readonly record struct ServiceInstance
{
    public string InstanceId { get; init; }
    public string Host { get; init; } = "localhost";
    public ushort Port { get; init; }

    public ServiceInstance(string instanceId, string host, ushort port)
    {
        InstanceId = instanceId;
        Host = host;
        Port = port;
    }
}

public readonly record struct BaseService
{
    public string ClusterId { get; init; } = null!;
    public string RouteId { get; init; } = null!;
    public string RouteMatch { get; init; } = null!;
    public string PathPattern { get; init; } = null!;

    public BaseService() { }
}

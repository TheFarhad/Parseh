namespace Framework;

internal interface IHost
{
    string BaseUrl { get; }
}

internal interface IHostProcess : IHost
{
    void Start();
    void ShutDown();
}
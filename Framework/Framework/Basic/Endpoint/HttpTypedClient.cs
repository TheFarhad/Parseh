namespace Framework;

public abstract class HttpTypedClient
{
    protected readonly HttpClient Client;

    protected HttpTypedClient(HttpClient client, string baseAddress)
    {
        Client = client;
        Client.BaseAddress = new Uri(baseAddress);
    }
}
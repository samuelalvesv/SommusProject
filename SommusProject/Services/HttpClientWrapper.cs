namespace SommusProject.Services;

public class HttpClientWrapper : IHttpClientWrapper
{
    private readonly HttpClient _httpClient;

    public HttpClientWrapper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken)
    {
        return _httpClient.GetAsync(url, cancellationToken);
    }
}
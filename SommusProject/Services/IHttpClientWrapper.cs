namespace SommusProject.Services;

public interface IHttpClientWrapper
{
    Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken);
}
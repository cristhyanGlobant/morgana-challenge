namespace UmbracoBridge.Application.Interfaces;

public interface IUmbracoCmsApiClient
{
    Task<T> GetAsync<T>(string relativeUrl);
    Task<TResponse> PostAsync<TRequest, TResponse>(string relativeUrl, TRequest request);
    Task<HttpResponseMessage> PostRawAsync<TRequest>(string relativeUrl, TRequest request);
    Task DeleteAsync(string relativeUrl);

}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UmbracoBridge.Application.Interfaces;
using UmbracoBridge.Application.Options;
using UmbracoBridge.Infraestructure.Exceptions;

namespace UmbracoBridge.Infrastructure.Http;

public class UmbracoCmsApiClient : IUmbracoCmsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly UmbracoCmsRoutes _routes;
    private readonly JsonSerializerOptions _jsonOptions;
    private string? _accessToken;

    public UmbracoCmsApiClient(
        HttpClient httpClient,
        IConfiguration configuration,
        IOptions<UmbracoCmsRoutes> routesOptions,
        JsonSerializerOptions jsonOptions)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _routes = routesOptions.Value;
        _jsonOptions = jsonOptions;
    }

    private async Task EnsureAuthorizationHeaderAsync()
    {
        if (_accessToken == null)
        {
            _accessToken = await GetAccessTokenAsync();
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var clientId = _configuration["UmbracoCMS:ClientId"];
        var clientSecret = _configuration["UmbracoCMS:ClientSecret"];

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            throw new UmbracoCmsTokenRequestException("Client ID or Client Secret is not configured.");
        }

        var requestBody = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientId!),
            new KeyValuePair<string, string>("client_secret", clientSecret!)
        });

        var response = await _httpClient.PostAsync(_routes.Token, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            throw new UmbracoCmsTokenRequestException($"Failed to acquire access token. Status: {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("access_token", out var tokenElement))
        {
            throw new UmbracoCmsTokenRequestException("Access token is missing from the token response.");
        }

        return tokenElement.GetString()!;
    }

    private async Task<HttpResponseMessage> SendAsyncSafe(Func<Task<HttpResponseMessage>> send)
    {
        await EnsureAuthorizationHeaderAsync();
        var response = await send();

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UmbracoCmsUnauthorizedException("Unauthorized request to Umbraco API.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new UmbracoCmsHttpException($"Umbraco API returned error. Status: {response.StatusCode}, Body: {errorBody}");
        }

        return response;
    }

    public async Task<T> GetAsync<T>(string relativeUrl)
    {
        var response = await SendAsyncSafe(() => _httpClient.GetAsync(relativeUrl));
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, _jsonOptions)!;
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string relativeUrl, TRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await SendAsyncSafe(() => _httpClient.PostAsync(relativeUrl, content));
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(responseBody, _jsonOptions)!;
    }

    public async Task<HttpResponseMessage> PostRawAsync<TRequest>(string relativeUrl, TRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        return await SendAsyncSafe(() => _httpClient.PostAsync(relativeUrl, content));
    }

    public async Task DeleteAsync(string relativeUrl)
    {
        await SendAsyncSafe(() => _httpClient.DeleteAsync(relativeUrl));
    }
}

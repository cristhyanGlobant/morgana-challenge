using Microsoft.Extensions.Options;
using UmbracoBridge.Application.DTOs;
using UmbracoBridge.Application.Interfaces;
using UmbracoBridge.Application.Options;

namespace UmbracoBridge.Infraestructure.Services;

public class UmbracoService : IUmbracoService
{
    private readonly IUmbracoCmsApiClient _apiClient;
    private readonly UmbracoCmsRoutes _routes;

    public UmbracoService(IUmbracoCmsApiClient apiClient, IOptions<UmbracoCmsRoutes> routesOptions)
    {
        _apiClient = apiClient;
        _routes = routesOptions.Value;
    }

    public async Task<HealthCheckResponse> GetHealthCheckAsync()
    {
        return await _apiClient.GetAsync<HealthCheckResponse>(_routes.HealthCheck);
    }

    public async Task<Guid> CreateDocumentTypeAsync(DocumentTypeRequestDto request)
    {
        var response = await _apiClient.PostRawAsync(_routes.DocumentType, request);

        if (response.Headers.TryGetValues("Umb-Generated-Resource", out var values) &&
            Guid.TryParse(values.FirstOrDefault(), out var id))
        {
            return id;
        }

        throw new InvalidOperationException("Header 'Umb-Generated-Resource' not found or invalid.");
    }

    public async Task DeleteDocumentTypeAsync(Guid id)
    {
        await _apiClient.DeleteAsync($"{_routes.DocumentType}/{id}");
    }
}

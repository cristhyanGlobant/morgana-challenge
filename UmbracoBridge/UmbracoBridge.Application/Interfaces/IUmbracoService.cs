using UmbracoBridge.Application.DTOs;

namespace UmbracoBridge.Application.Interfaces;

public interface IUmbracoService
{
    Task<HealthCheckResponse> GetHealthCheckAsync();
    Task<Guid> CreateDocumentTypeAsync(DocumentTypeRequestDto request);
    Task DeleteDocumentTypeAsync(Guid id);
}

using Microsoft.Extensions.Options;
using Moq;
using UmbracoBridge.Application.DTOs;
using UmbracoBridge.Application.Interfaces;
using UmbracoBridge.Application.Options;
using UmbracoBridge.Infraestructure.Services;
using Xunit;

namespace UmbracoBridge.Tests.Services;

public class UmbracoServiceTests
{
    private readonly Mock<IUmbracoCmsApiClient> _apiClientMock;
    private readonly IOptions<UmbracoCmsRoutes> _options;
    private readonly UmbracoService _service;

    public UmbracoServiceTests()
    {
        _apiClientMock = new Mock<IUmbracoCmsApiClient>();
        _options = Options.Create(new UmbracoCmsRoutes
        {
            HealthCheck = "/health",
            DocumentType = "/document-types"
        });

        _service = new UmbracoService(_apiClientMock.Object, _options);
    }

    [Fact]
    public async Task GetHealthCheckAsync_ReturnsValidHealthCheckItems()
    {
        // Arrange
        var expected = new HealthCheckResponse
        {
            Total = 6,
            Items = new List<HealthCheckItem>
        {
            new HealthCheckItem { Name = "Configuration" },
            new HealthCheckItem { Name = "Data Integrity" },
            new HealthCheckItem { Name = "Live Environment" },
            new HealthCheckItem { Name = "Permissions" },
            new HealthCheckItem { Name = "Security" },
            new HealthCheckItem { Name = "Services" }
        }
        };

        _apiClientMock
            .Setup(x => x.GetAsync<HealthCheckResponse>("/health"))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetHealthCheckAsync();

        // Assert
        Assert.Equal(6, result.Total);
        Assert.Equal(6, result.Items.Count);

        var expectedNames = new[] { "Configuration", "Data Integrity", "Live Environment", "Permissions", "Security", "Services" };
        foreach (var name in expectedNames)
        {
            Assert.Contains(result.Items, item => item.Name == name);
        }
    }

    [Fact]
    public async Task CreateDocumentTypeAsync_ParsesGuidFromHeader()
    {
        // Arrange
        var request = new DocumentTypeRequestDto();
        var expectedGuid = Guid.NewGuid();

        var response = new HttpResponseMessage();
        response.Headers.TryAddWithoutValidation("Umb-Generated-Resource", expectedGuid.ToString());

        _apiClientMock
            .Setup(x => x.PostRawAsync("/document-types", request))
            .ReturnsAsync(response);

        // Act
        var result = await _service.CreateDocumentTypeAsync(request);

        // Assert
        Assert.Equal(expectedGuid, result);
    }

    [Fact]
    public async Task CreateDocumentTypeAsync_Throws_When_HeaderInvalid()
    {
        // Arrange
        var request = new DocumentTypeRequestDto();
        var response = new HttpResponseMessage(); // no headers

        _apiClientMock
            .Setup(x => x.PostRawAsync("/document-types", request))
            .ReturnsAsync(response);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateDocumentTypeAsync(request));
    }

    [Fact]
    public async Task DeleteDocumentTypeAsync_CallsDeleteWithCorrectUrl()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedUrl = $"/document-types/{id}";

        _apiClientMock
            .Setup(x => x.DeleteAsync(expectedUrl))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _service.DeleteDocumentTypeAsync(id);

        // Assert
        _apiClientMock.Verify();
    }
}

using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UmbracoBridge.Application.DTOs;
using UmbracoBridge.Application.Interfaces;
using UmbracoBridge.WebApi.Controllers;
using Xunit;

namespace UmbracoBridge.Tests.Controllers;

public class DocumentTypeControllerTests
{
    private readonly Mock<IUmbracoService> _umbracoServiceMock;
    private readonly Mock<IValidator<DocumentTypeRequestDto>> _validatorMock;
    private readonly DocumentTypeController _controller;

    public DocumentTypeControllerTests()
    {
        _umbracoServiceMock = new Mock<IUmbracoService>();
        _validatorMock = new Mock<IValidator<DocumentTypeRequestDto>>();
        _controller = new DocumentTypeController(_umbracoServiceMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task CreateDocumentType_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new DocumentTypeRequestDto { Name = "Article" };
        var expectedId = Guid.NewGuid();

        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _umbracoServiceMock
            .Setup(s => s.CreateDocumentTypeAsync(request))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _controller.CreateDocumentType(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedId, okResult.Value);
    }

    [Fact]
    public async Task CreateDocumentType_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new DocumentTypeRequestDto { Name = "" };
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required")
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _controller.CreateDocumentType(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorMessages = Assert.IsAssignableFrom<IEnumerable<string>>(badRequest.Value);
        Assert.Contains("Name is required", errorMessages);
    }

    [Fact]
    public async Task DeleteDocumentType_ValidId_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();

        _umbracoServiceMock
            .Setup(s => s.DeleteDocumentTypeAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteDocumentType(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}

using Microsoft.AspNetCore.Mvc;
using Moq;
using UmbracoBridge.Application.DTOs;
using UmbracoBridge.Application.Interfaces;
using UmbracoBridge.WebApi.Controllers;
using Xunit;

namespace UmbracoBridge.Tests.Controllers;

public class HealthcheckControllerTests
{
    [Fact]
    public async Task GetHealthStatus_ReturnsOkWithHealthCheckResponse()
    {
        // Arrange
        var mockService = new Mock<IUmbracoService>();

        var mockResponse = new HealthCheckResponse
        {
            Total = 6,
            Items = new List<HealthCheckItem>
            {
                new() { Name = "Configuration" },
                new() { Name = "Data Integrity" },
                new() { Name = "Live Environment" },
                new() { Name = "Permissions" },
                new() { Name = "Security" },
                new() { Name = "Services" }
            }
        };

        mockService.Setup(s => s.GetHealthCheckAsync())
                   .ReturnsAsync(mockResponse);

        var controller = new HealthcheckController(mockService.Object);

        // Act
        var result = await controller.GetHealthStatus();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResponse = Assert.IsType<HealthCheckResponse>(okResult.Value);

        Assert.Equal(6, returnedResponse.Total);
        Assert.Collection(returnedResponse.Items,
            item => Assert.Equal("Configuration", item.Name),
            item => Assert.Equal("Data Integrity", item.Name),
            item => Assert.Equal("Live Environment", item.Name),
            item => Assert.Equal("Permissions", item.Name),
            item => Assert.Equal("Security", item.Name),
            item => Assert.Equal("Services", item.Name)
        );
    }
}

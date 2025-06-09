var builder = DistributedApplication.CreateBuilder(args);

var umbracoCms = builder.AddProject("umbraco-cms", "../UmbracoCMS/UmbracoCMS.csproj")
    .WithEndpoint(name: "https-umbraco", port: 5110, scheme: "https")
    .WithEnvironment("Umbraco__DeliveryApi__Enabled", "true")
    .WithEnvironment("Umbraco__DeliveryApi__ApiKey", "my-secret-api-key");

// UmbracoBridge(API propia)
var umbracoBridge = builder.AddProject("umbraco-bridge", "../UmbracoBridge/UmbracoBridge.Api/UmbracoBridge.Api.csproj")
    .WithEndpoint(name: "https-bridge", port: 5120, scheme: "https")
    .WithEnvironment("DeliveryApi__ApiKey", "my-secret-api-key")
    .WithReference(umbracoCms); // cross-reference to consume UmbracoCMS API

builder.Build().Run();

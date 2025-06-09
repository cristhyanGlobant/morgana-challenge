using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UmbracoBridge.Application.DTOs;
using UmbracoBridge.Application.Interfaces;
using UmbracoBridge.Application.Options;
using UmbracoBridge.Application.Validators;
using UmbracoBridge.Infraestructure.Services;
using UmbracoBridge.Infrastructure.Http;

namespace UmbracoBridge.Infrastructure.DependencyInjection;

public static class UmbracoBridgeServiceRegistration
{
    public static IServiceCollection AddUmbracoBridgeServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddUmbracoHttpClient(configuration)
            .AddUmbracoRoutes(configuration)
            .AddUmbracoServices()
            .AddUmbracoValidation()
            .AddUmbracoJsonOptions();

        return services;
    }

    private static IServiceCollection AddUmbracoHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var umbracoCmsUrl = configuration["services:umbraco-cms:https-umbraco:0"]
            ?? configuration["UmbracoCMS:BaseUrl"]
            ?? throw new InvalidOperationException("No se pudo obtener la URL de UmbracoCMS desde Aspire.");

        services.AddHttpClient<IUmbracoCmsApiClient, UmbracoCmsApiClient>(client =>
        {
            client.BaseAddress = new Uri(umbracoCmsUrl);
        });

        return services;
    }

    private static IServiceCollection AddUmbracoRoutes(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<UmbracoCmsRoutes>(configuration.GetSection("UmbracoCMS:Routes"));
        return services;
    }

    private static IServiceCollection AddUmbracoServices(this IServiceCollection services)
    {
        services.AddScoped<IUmbracoService, UmbracoService>();
        return services;
    }

    private static IServiceCollection AddUmbracoValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddScoped<IValidator<DocumentTypeRequestDto>, DocumentTypeRequestValidator>();
        return services;
    }

    private static IServiceCollection AddUmbracoJsonOptions(this IServiceCollection services)
    {
        services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });

        return services;
    }
}

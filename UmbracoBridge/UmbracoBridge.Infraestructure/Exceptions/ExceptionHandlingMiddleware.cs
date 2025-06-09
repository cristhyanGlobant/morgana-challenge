using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace UmbracoBridge.Infraestructure.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Execute next middleware
        }
        catch (UmbracoCmsTokenRequestException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, "Unauthorized to access Umbraco Management API.", ex.Message);
        }
        catch (HttpRequestException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.ServiceUnavailable, "Unable to reach Umbraco Management API.", ex.Message);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.", ex.Message);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string error, string details)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(new { error, details });
        return context.Response.WriteAsync(result);
    }
}

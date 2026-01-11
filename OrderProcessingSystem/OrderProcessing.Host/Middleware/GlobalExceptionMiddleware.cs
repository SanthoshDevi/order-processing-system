using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace OrderProcessing.Host.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogWarning(ex, "Bad request");

            await WriteProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                "Invalid request payload",
                "The request body contains invalid or out-of-range values.");
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "JSON parsing error");

            await WriteProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                "Malformed JSON",
                "The JSON request body is invalid.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await WriteProblemDetails(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteProblemDetails(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new
        {
            title,
            status = statusCode,
            detail
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}

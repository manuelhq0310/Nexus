using System.Net;
using System.Text.Json;
using Nexus.Application.Common.Exceptions;

namespace Nexus.Api.Middleware;

/// <summary>
/// Middleware global para el manejo de excepciones. Captura cualquier excepción
/// no controlada en el pipeline y la traduce a una respuesta JSON consistente,
/// evitando exponer detalles internos (stack traces) al cliente.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            AppException appException => (appException.StatusCode, appException.Message),
            _ => (HttpStatusCode.InternalServerError, "Ha ocurrido un error inesperado al procesar la solicitud.")
        };

        if (exception is AppException)
        {
            _logger.LogWarning(exception, "Excepción controlada: {Message}", exception.Message);
        }
        else
        {
            _logger.LogError(exception, "Excepción no controlada.");
        }

        var response = new ProblemResponse
        {
            Status = (int)statusCode,
            Title = title,
            TraceId = context.TraceIdentifier,
            // Solo se exponen detalles técnicos en entornos de desarrollo.
            Detail = _environment.IsDevelopment() ? exception.ToString() : null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    private class ProblemResponse
    {
        public int Status { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
        public string? Detail { get; set; }
    }
}

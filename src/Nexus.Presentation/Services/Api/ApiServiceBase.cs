using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

/// <summary>
/// Base con helpers comunes para los servicios que consumen la API de Nexus:
/// centraliza la lectura/parseo de errores (ProblemDetails) del middleware global de excepciones.
/// </summary>
public abstract class ApiServiceBase
{
    protected readonly HttpClient Http;

    protected ApiServiceBase(HttpClient http)
    {
        Http = http;
    }

    protected async Task<T> ReadOrThrowAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw await BuildExceptionAsync(response);
        }

        var result = await response.Content.ReadFromJsonAsync<T>();
        return result is null
            ? throw new ApiException("El servidor devolvió una respuesta vacía.", (int)response.StatusCode)
            : result;
    }

    protected async Task<T?> ReadOrThrowNullableAsync<T>(HttpResponseMessage response) where T : class
    {
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw await BuildExceptionAsync(response);
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw await BuildExceptionAsync(response);
        }
    }

    private static async Task<ApiException> BuildExceptionAsync(HttpResponseMessage response)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
            if (problem is not null && !string.IsNullOrWhiteSpace(problem.Title))
            {
                return new ApiException(problem.Title, (int)response.StatusCode);
            }
        }
        catch
        {
            // El cuerpo no era JSON / no tenía el formato esperado; se usa el mensaje genérico.
        }

        return new ApiException($"Ocurrió un error ({(int)response.StatusCode}).", (int)response.StatusCode);
    }
}

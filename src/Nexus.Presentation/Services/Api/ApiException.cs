namespace Nexus.Presentation.Services.Api;

/// <summary>Excepción lanzada por los servicios de API cuando el backend responde con error.</summary>
public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

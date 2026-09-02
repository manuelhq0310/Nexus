using System.Net;

namespace Nexus.Application.Common.Exceptions;

/// <summary>
/// Excepción base de la capa de aplicación. Permite asociar un código
/// HTTP semántico que el middleware global traducirá en la respuesta.
/// </summary>
public abstract class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }

    protected AppException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

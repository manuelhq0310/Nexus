using System.Net;

namespace Nexus.Application.Common.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, HttpStatusCode.BadRequest) { }
}

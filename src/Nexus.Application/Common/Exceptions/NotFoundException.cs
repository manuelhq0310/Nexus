using System.Net;

namespace Nexus.Application.Common.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, HttpStatusCode.NotFound) { }
}

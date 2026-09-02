using System.Net;

namespace Nexus.Application.Common.Exceptions;

public class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message) : base(message, HttpStatusCode.Unauthorized) { }
}

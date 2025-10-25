namespace Flowertrack.Api.Exceptions;

/// <summary>
/// Exception thrown when authentication is required but not provided
/// </summary>
public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message = "Authentication is required") : base(message)
    {
    }
}

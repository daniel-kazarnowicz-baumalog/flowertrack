namespace Flowertrack.Api.Exceptions;

/// <summary>
/// Exception thrown when user lacks required permissions
/// </summary>
public class ForbiddenException : BaseException
{
    public ForbiddenException(string message = "You do not have permission to perform this action") : base(message)
    {
    }
}

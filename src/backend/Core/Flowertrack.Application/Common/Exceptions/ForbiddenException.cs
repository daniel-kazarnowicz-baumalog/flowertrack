namespace Flowertrack.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a user attempts an unauthorized action
/// </summary>
public sealed class ForbiddenException : Exception
{
    public ForbiddenException()
        : base("Access forbidden.")
    {
    }

    public ForbiddenException(string message)
        : base(message)
    {
    }

    public ForbiddenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

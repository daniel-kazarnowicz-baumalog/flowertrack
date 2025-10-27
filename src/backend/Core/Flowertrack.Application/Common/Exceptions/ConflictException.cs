namespace Flowertrack.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a resource conflict occurs (e.g., duplicate email)
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException()
        : base("A conflict occurred.")
    {
    }

    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

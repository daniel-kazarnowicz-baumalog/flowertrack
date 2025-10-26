namespace Flowertrack.Domain.Exceptions;

/// <summary>
/// Base exception class for all custom application exceptions
/// </summary>
public abstract class BaseException : Exception
{
    protected BaseException(string message) : base(message)
    {
    }

    protected BaseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

namespace Flowertrack.Application.Common.Interfaces;

/// <summary>
/// Service for generating secure tokens
/// </summary>
public interface ITokenGenerator
{
    /// <summary>
    /// Generates a secure random token for activation/reset purposes
    /// </summary>
    /// <param name="length">Token length (default 32 bytes)</param>
    /// <returns>URL-safe base64 encoded token</returns>
    string GenerateSecureToken(int length = 32);

    /// <summary>
    /// Generates a temporary password
    /// </summary>
    /// <param name="length">Password length (default 12 characters)</param>
    /// <returns>Secure temporary password</returns>
    string GenerateTemporaryPassword(int length = 12);
}

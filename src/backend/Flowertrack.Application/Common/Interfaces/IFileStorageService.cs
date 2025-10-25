namespace Flowertrack.Application.Common.Interfaces;

/// <summary>
/// Interface for file storage operations using Supabase Storage
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file to the storage
    /// </summary>
    /// <param name="bucketName">Name of the storage bucket</param>
    /// <param name="path">File path within the bucket</param>
    /// <param name="fileStream">File content stream</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The public URL of the uploaded file</returns>
    Task<string> UploadAsync(
        string bucketName, 
        string path, 
        Stream fileStream, 
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from the storage
    /// </summary>
    /// <param name="bucketName">Name of the storage bucket</param>
    /// <param name="path">File path within the bucket</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File content as byte array</returns>
    Task<byte[]> DownloadAsync(
        string bucketName, 
        string path,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the storage
    /// </summary>
    /// <param name="bucketName">Name of the storage bucket</param>
    /// <param name="path">File path within the bucket</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(
        string bucketName, 
        string path,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the public URL for a file
    /// </summary>
    /// <param name="bucketName">Name of the storage bucket</param>
    /// <param name="path">File path within the bucket</param>
    /// <returns>Public URL of the file</returns>
    string GetPublicUrl(string bucketName, string path);
}

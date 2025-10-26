using Flowertrack.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Infrastructure.Supabase;

/// <summary>
/// Implementation of file storage service using Supabase Storage
/// </summary>
public class SupabaseStorageService : IFileStorageService
{
    private readonly ISupabaseClient _supabaseClient;
    private readonly ILogger<SupabaseStorageService> _logger;

    public SupabaseStorageService(
        ISupabaseClient supabaseClient,
        ILogger<SupabaseStorageService> logger)
    {
        _supabaseClient = supabaseClient ?? throw new ArgumentNullException(nameof(supabaseClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Uploads a file to Supabase Storage
    /// Path strategy: {organizationId}/{ticketId}/{filename}
    /// </summary>
    public async Task<string> UploadAsync(
        string bucketName, 
        string path, 
        Stream fileStream, 
        string contentType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Uploading file to bucket: {Bucket}, path: {Path}", bucketName, path);

            // Read stream to byte array
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();

            // Upload to Supabase Storage
            var response = await _supabaseClient.Storage
                .From(bucketName)
                .Upload(fileBytes, path, new global::Supabase.Storage.FileOptions
                {
                    ContentType = contentType,
                    Upsert = false // Don't overwrite existing files
                });

            if (string.IsNullOrEmpty(response))
            {
                throw new InvalidOperationException($"Failed to upload file to {bucketName}/{path}");
            }

            var publicUrl = GetPublicUrl(bucketName, path);
            
            _logger.LogInformation("File uploaded successfully to: {Url}", publicUrl);
            
            return publicUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file to bucket: {Bucket}, path: {Path}", bucketName, path);
            throw;
        }
    }

    /// <summary>
    /// Downloads a file from Supabase Storage
    /// </summary>
    public async Task<byte[]> DownloadAsync(
        string bucketName, 
        string path,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Downloading file from bucket: {Bucket}, path: {Path}", bucketName, path);

            var fileBytes = await _supabaseClient.Storage
                .From(bucketName)
                .Download(path, null);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                throw new FileNotFoundException($"File not found: {bucketName}/{path}");
            }

            _logger.LogInformation("File downloaded successfully from: {Bucket}/{Path}", bucketName, path);
            
            return fileBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file from bucket: {Bucket}, path: {Path}", bucketName, path);
            throw;
        }
    }

    /// <summary>
    /// Deletes a file from Supabase Storage
    /// </summary>
    public async Task DeleteAsync(
        string bucketName, 
        string path,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting file from bucket: {Bucket}, path: {Path}", bucketName, path);

            await _supabaseClient.Storage
                .From(bucketName)
                .Remove(new List<string> { path });

            _logger.LogInformation("File deleted successfully from: {Bucket}/{Path}", bucketName, path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file from bucket: {Bucket}, path: {Path}", bucketName, path);
            throw;
        }
    }

    /// <summary>
    /// Gets the public URL for a file
    /// </summary>
    public string GetPublicUrl(string bucketName, string path)
    {
        try
        {
            var publicUrl = _supabaseClient.Storage
                .From(bucketName)
                .GetPublicUrl(path);

            return publicUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get public URL for bucket: {Bucket}, path: {Path}", bucketName, path);
            throw;
        }
    }
}

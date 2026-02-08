namespace Cooklyn.Server.Services;

using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using System.Net;

/// <summary>
/// Interface for file storage operations (S3-compatible).
/// Supports Northflank, AWS S3, MinIO, and other S3-compatible providers.
/// </summary>
public interface IFileStorage
{
    Task<string?> UploadFileAsync(string bucketName, string key, Stream fileStream, CancellationToken cancellationToken = default);
    Task<string?> UploadFileAsync(string bucketName, string key, byte[] fileBytes, CancellationToken cancellationToken = default);
    Task<string?> UploadFileAsync(string bucketName, string key, string filePath, CancellationToken cancellationToken = default);
    Task<string?> UploadFileAsync(string bucketName, string key, string filePath, string contentType, CancellationToken cancellationToken = default);
    Task<string?> UploadFileAsync(string bucketName, string key, IFormFile formFile, CancellationToken cancellationToken = default);
    Task<Stream?> GetFileAsync(string bucketName, string key, CancellationToken cancellationToken = default);
    string GetPreSignedUrl(string bucketName, string key, int durationInMinutes = 5);
    Task<bool> DeleteFileAsync(string bucketName, string key, CancellationToken cancellationToken = default);
    Task<bool> DeleteFilesAsync(string bucketName, IEnumerable<string> keys, CancellationToken cancellationToken = default);
    Task<bool> FileExistsAsync(string bucketName, string key, CancellationToken cancellationToken = default);
}

/// <summary>
/// S3-compatible file storage implementation.
/// Works with AWS S3, Northflank Object Storage, MinIO, and other S3-compatible services.
/// </summary>
public sealed class FileStorage(IAmazonS3 s3Client, IConfiguration configuration) : IFileStorage
{
    private readonly bool _useLocalCompatibility =
        !string.IsNullOrEmpty(configuration["AWS:ServiceURL"]);

    private readonly Protocol _presignedUrlProtocol =
        configuration["AWS:ServiceURL"]?.StartsWith("http://", StringComparison.OrdinalIgnoreCase) == true
            ? Protocol.HTTP
            : Protocol.HTTPS;

    public async Task<string?> UploadFileAsync(string bucketName, string key, Stream fileStream, CancellationToken cancellationToken = default)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileStream,
            UseChunkEncoding = !_useLocalCompatibility
        };

        var response = await s3Client.PutObjectAsync(putObjectRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK ? key : null;
    }

    public async Task<string?> UploadFileAsync(string bucketName, string key, IFormFile formFile, CancellationToken cancellationToken = default)
    {
        if (formFile is null)
            throw new ArgumentNullException(nameof(formFile), "No file was provided for upload");

        await using var stream = formFile.OpenReadStream();
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
            ContentType = formFile.ContentType,
            UseChunkEncoding = !_useLocalCompatibility
        };

        var response = await s3Client.PutObjectAsync(putObjectRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK ? key : null;
    }

    public async Task<string?> UploadFileAsync(string bucketName, string key, byte[] fileBytes, CancellationToken cancellationToken = default)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = new MemoryStream(fileBytes),
            UseChunkEncoding = !_useLocalCompatibility
        };

        var response = await s3Client.PutObjectAsync(putObjectRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK ? key : null;
    }

    public async Task<string?> UploadFileAsync(string bucketName, string key, string filePath, CancellationToken cancellationToken = default)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            FilePath = filePath,
            UseChunkEncoding = !_useLocalCompatibility
        };

        var response = await s3Client.PutObjectAsync(putObjectRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK ? key : null;
    }

    public async Task<string?> UploadFileAsync(string bucketName, string key, string filePath, string contentType, CancellationToken cancellationToken = default)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            FilePath = filePath,
            ContentType = contentType,
            UseChunkEncoding = !_useLocalCompatibility
        };

        var response = await s3Client.PutObjectAsync(putObjectRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK ? key : null;
    }

    public async Task<Stream?> GetFileAsync(string bucketName, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            var response = await s3Client.GetObjectAsync(getObjectRequest, cancellationToken);
            return response.HttpStatusCode == HttpStatusCode.OK ? response.ResponseStream : null;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public string GetPreSignedUrl(string bucketName, string key, int durationInMinutes = 5)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(durationInMinutes),
            Protocol = _presignedUrlProtocol
        };

        return s3Client.GetPreSignedURL(request);
    }

    public async Task<bool> DeleteFileAsync(string bucketName, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            var response = await s3Client.DeleteObjectAsync(deleteObjectRequest, cancellationToken);
            return response.HttpStatusCode == HttpStatusCode.NoContent;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return true; // Already deleted
        }
    }

    public async Task<bool> DeleteFilesAsync(string bucketName, IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        var keysList = keys.ToList();
        if (keysList.Count == 0)
            return true;

        var deleteObjectsRequest = new DeleteObjectsRequest
        {
            BucketName = bucketName,
            Objects = keysList.Select(key => new KeyVersion { Key = key }).ToList()
        };

        var response = await s3Client.DeleteObjectsAsync(deleteObjectsRequest, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> FileExistsAsync(string bucketName, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var metadataRequest = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = key
            };

            await s3Client.GetObjectMetadataAsync(metadataRequest, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}

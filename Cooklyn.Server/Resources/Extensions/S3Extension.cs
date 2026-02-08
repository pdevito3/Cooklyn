namespace Cooklyn.Server.Resources.Extensions;

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Util;
using Services;

public static class S3Extension
{
    public static async Task EnsureBucketsExist(this WebApplication app)
    {
        var config = app.Services.GetRequiredService<IConfiguration>();
        var s3Client = app.Services.GetRequiredService<IAmazonS3>();
        var bucketName = config["AWS:RecipeImagesBucket"];
        if (string.IsNullOrEmpty(bucketName)) return;

        try
        {
            if (!await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName))
                await s3Client.PutBucketAsync(bucketName);
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "Failed to ensure S3 bucket '{BucketName}' exists", bucketName);
        }
    }

    /// <summary>
    /// Registers AWS S3 services for file storage.
    /// Supports AWS S3, Northflank Object Storage, MinIO, and other S3-compatible providers.
    ///
    /// Configuration options (via environment variables or configuration):
    /// - AWS__ServiceURL: Custom endpoint URL for S3-compatible services (e.g., Northflank, MinIO)
    /// - AWS__AccessKey: Access key ID
    /// - AWS__SecretKey: Secret access key
    /// - AWS__Region: AWS region (default: us-east-1)
    /// - AWS__ForcePathStyle: Force path-style addressing for S3-compatible services (default: true for custom endpoints)
    /// </summary>
    public static IServiceCollection AddS3FileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceUrl = configuration["AWS:ServiceURL"];
        var accessKey = configuration["AWS:AccessKey"];
        var secretKey = configuration["AWS:SecretKey"];
        var region = configuration["AWS:Region"] ?? "us-east-1";
        var forcePathStyle = configuration.GetValue("AWS:ForcePathStyle", !string.IsNullOrEmpty(serviceUrl));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(region),
                ForcePathStyle = forcePathStyle,
                // Only compute checksums when required by the API operation.
                // AWSSDK v4 adds CRC32 trailing headers by default which
                // S3-compatible services (MinIO) count toward metadata limits.
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            };

            // For Northflank, MinIO, or other S3-compatible services
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                config.ServiceURL = serviceUrl;
            }

            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                return new AmazonS3Client(credentials, config);
            }

            // Use default credential chain (IAM roles, environment variables, etc.)
            return new AmazonS3Client(config);
        });

        services.AddScoped<IFileStorage, FileStorage>();

        return services;
    }
}

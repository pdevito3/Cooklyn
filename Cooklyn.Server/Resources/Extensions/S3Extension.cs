namespace Cooklyn.Server.Resources.Extensions;

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Services;

public static class S3Extension
{
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
                ForcePathStyle = forcePathStyle
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

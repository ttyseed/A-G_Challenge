using Amazon.S3.Model;

namespace challenge1.Common.EmailService.Classes.Interfaces
{
    internal interface IS3Helper
    {
        Task<Stream?> DownloadFromBucketAsync(string bucketName, string fileName);
        Task<GetObjectMetadataResponse?> GetS3FileMetadataAsync(string bucketName, string fileName);
    }
}

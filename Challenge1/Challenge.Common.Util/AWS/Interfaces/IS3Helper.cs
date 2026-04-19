
using Amazon.S3.Model;
using Amazon.SecurityToken.Model;
using Microsoft.AspNetCore.Http;

namespace challenge1.Common.Util.AWS.Interfaces
{
    public interface IS3Helper
    {
        Task<bool> UploadToBucketAsync(string fileLocation, IFormFile file, string bucketName);
        Task<Stream?> DownloadFromBucketAsync(string bucketName, string fileName);
        Task<List<string>> GetAllFilesV2FromBucketAsync(string bucketName, string prefix, string fileName);
        Task<GetObjectMetadataResponse?> GetS3FileMetadataAsync(string bucketName, string fileName);
        Task<Credentials> AssumeS3RoleAsync(AssumeRoleRequest assumeRoleRequest);
    }
}

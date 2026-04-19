using Amazon.S3;
using Amazon.S3.Model;
using challenge1.Common.EmailService.Classes.Interfaces;

namespace challenge1.Common.EmailService.Classes
{
    internal class S3Helper : IS3Helper
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ILogging _logging;

        public S3Helper(IAmazonS3 s3Client, ILogging logging)
        {
            _s3Client = s3Client;
            _logging = logging;
        }

        public async Task<Stream?> DownloadFromBucketAsync(string bucketName, string fileName)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName
                };

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                {
                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return response.ResponseStream;
                    }
                }
            }
            catch (AmazonS3Exception s3e)
            {
                _logging.LogError($"AmazonS3Exception. Message:'{s3e.Message}' when reading object");
            }
            catch (Exception ex)
            {
                _logging.LogError($"Exception. Message:'{ex.Message}' when reading object");
            }

            return null;
        }

        /// <summary>
        /// Get metadata of a file in S3 bucket. E.g. To get file size without downloading the file.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <returns>
        /// GetObjectMetadataResponse if successful, null otherwise.
        /// </returns>
        public async Task<GetObjectMetadataResponse?> GetS3FileMetadataAsync(string bucketName, string fileName)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = fileName
                };
                var response = await _s3Client.GetObjectMetadataAsync(request);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response;
                }
            }
            catch (AmazonS3Exception s3e)
            {
                _logging.LogError($"AmazonS3Exception. Message:'{s3e.Message}' when reading object metadata");
            }
            catch (Exception ex)
            {
                _logging.LogError($"Exception. Message:'{ex.Message}' when reading object metadata");
            }

            return null;
        }
    }
}

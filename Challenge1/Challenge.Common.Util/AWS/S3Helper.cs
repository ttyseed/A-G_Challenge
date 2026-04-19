using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Microsoft.AspNetCore.Http;
using challenge1.Common.Logging;
using challenge1.Common.Util.AWS.Interfaces;

namespace challenge1.Common.Util.AWS
{
    public class S3Helper : IS3Helper
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ILogging _logging;

        public S3Helper(IAmazonS3 s3Client, ILogging logging)
        {
            _s3Client = s3Client;
            _logging = logging;
        }

        public async Task<bool> UploadToBucketAsync(string fileLocation, IFormFile file, string bucketName)
        {
            try
            {
                await using MemoryStream stream = new MemoryStream();
                await file.CopyToAsync(stream);

                TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = fileLocation,
                    BucketName = bucketName
                };

                TransferUtility FileTU = new TransferUtility(_s3Client);
                await FileTU.UploadAsync(uploadRequest);

                return true;
            }
            catch (AmazonS3Exception s3e)
            {
                _logging.LogError($"AmazonS3Exception. Message:'{s3e.Message}' when reading object");
            }
            catch (Exception ex)
            {
                _logging.LogError($"Exception. Message:'{ex.Message}' when reading object");
            }

            return false;
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
        /// Get all files from S3 bucket with prefix and file name filter
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="prefix"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<List<string>> GetAllFilesV2FromBucketAsync(string bucketName, string prefix, string fileName)
        {
            var lstFileName = new List<string>();
            var result = new List<S3Object>();

            try
            {
                string? token = null;
                do
                {
                    ListObjectsV2Request request = new ListObjectsV2Request()
                    {
                        BucketName = bucketName,
                        Prefix = prefix,
                        Delimiter = "/"
                    };
                    ListObjectsV2Response response = await _s3Client.ListObjectsV2Async(request).ConfigureAwait(false);
                    result.AddRange(response.S3Objects);
                    token = response.NextContinuationToken;
                } while (token != null);

                foreach (S3Object s3Object in result)
                {
                    if (s3Object.Key.ToString().Contains(fileName))
                    {
                        lstFileName.Add(s3Object.Key.Split('/').Last().ToString());
                    }
                }

                lstFileName.Sort();
            }
            catch (AmazonS3Exception s3e)
            {
                _logging.LogError($"AmazonS3Exception. Message:'{s3e.Message}' when reading object");
            }
            catch (Exception ex)
            {
                _logging.LogError($"Exception. Message:'{ex.Message}' when reading object");
            }

            return lstFileName;
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

        /// <summary>
        /// Assume Role to access S3 bucket in another AWS account
        /// </summary>
        /// <param name="assumeRoleRequest"></param>
        /// <returns></returns>
        public async Task<Credentials> AssumeS3RoleAsync(AssumeRoleRequest assumeRoleRequest)
        {
            var client = new AmazonSecurityTokenServiceClient(RegionEndpoint.APSoutheast1);
            var response = await client.AssumeRoleAsync(assumeRoleRequest);
            return response.Credentials;

        }
        
    }
}

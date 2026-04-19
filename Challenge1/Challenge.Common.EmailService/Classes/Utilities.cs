using challenge1.Common.EmailService.Classes.Interfaces;

namespace challenge1.Common.EmailService.Classes
{
    internal class Utilities : IUtilities
    {
        /// <summary>
        /// Parses an S3 path like "s3://bucket-name/folder/file.txt"
        /// and returns bucket name and object key.
        /// </summary>
        /// <param name="s3Path">Full S3 path</param>
        /// <returns>Tuple of (bucketName, key)</returns>
        public (string bucketName, string key) ParseS3Path(string s3Path)
        {
            if (string.IsNullOrWhiteSpace(s3Path))
                throw new ArgumentException("S3 path cannot be null or empty.", nameof(s3Path));

            // Remove the s3:// scheme if present
            var path = s3Path.StartsWith("s3://", StringComparison.OrdinalIgnoreCase)
                ? s3Path.Substring(5)
                : s3Path;

            var firstSlashIndex = path.IndexOf('/');

            string bucketName = firstSlashIndex >= 0
                ? path.Substring(0, firstSlashIndex)
                : path; // No key, just bucket

            string key = firstSlashIndex >= 0
                ? path.Substring(firstSlashIndex + 1)
                : string.Empty;

            return (bucketName, key);
        }
    }
}

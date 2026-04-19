namespace challenge1.Common.EmailService.Classes.Interfaces
{
    internal interface IUtilities
    {
        (string bucketName, string key) ParseS3Path(string s3Path);
    }
}

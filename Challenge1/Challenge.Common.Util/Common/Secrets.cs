using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;

namespace challenge1.Common.Util.Common
{
    public static class Secrets
    {

        private static readonly string DEV_SECRETS =
                                    @"{
                                      ""ConnectionStrings"": ""Server=localhost\\MSSQLSERVER01;Database=SimsysTemplate;Trusted_Connection=True;TrustServerCertificate=true;"",
                                      ""salt"": """",
                                      ""api_url"": """",
                                      ""x-api-key"": """",
                                      ""x-apigw-api-id"": """",
                                      ""EsubPassword"": """",
                                      ""openweathermap_api_key"": """",
                                      ""jwt_secret"": ""simsys-dev-jwt-secret-key-must-be-at-least-32-characters-long!""
                                    }";

        private static JsonDocument JsonSecrets { get; set; }

        private static bool _loaded;

        public static async Task InitializeAsync()
        {
            if (_loaded) return;

            await LoadSecretsAsync();
            _loaded = true;
        }

        private static async Task LoadSecretsAsync()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                // Reads secrets from here if you press the play button up top (Visual Studio).
                // Hard code all the secrets you need in the global variable.
                JsonSecrets = JsonDocument.Parse(DEV_SECRETS);
            }
            else
            {
                // Reads secrets straight from the AWS Secret Manager.
                string secretName = Setting.SecretName;
                string region = "ap-southeast-1";

                //MemoryStream memoryStream = new MemoryStream();

                IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

                GetSecretValueRequest request = new GetSecretValueRequest
                {
                    SecretId = secretName,
                    VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
                };

                GetSecretValueResponse? response = null;

                try
                {
                    response = await client.GetSecretValueAsync(request);
                }

                catch (DecryptionFailureException)
                {
                    // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
                    // Deal with the exception here, and/or rethrow at your discretion.
                    JsonSecrets = JsonDocument.Parse(@"{""secrets_error"": ""DecryptionFailureException""}");
                }
                catch (InternalServiceErrorException)
                {
                    // An error occurred on the server side.
                    // Deal with the exception here, and/or rethrow at your discretion.
                    JsonSecrets = JsonDocument.Parse(@"{""secrets_error"": ""InternalServiceErrorException""}");
                }
                catch (InvalidParameterException)
                {
                    // You provided an invalid value for a parameter.
                    // Deal with the exception here, and/or rethrow at your discretion
                    JsonSecrets = JsonDocument.Parse(@"{""secrets_error"": ""InvalidParameterException""}");
                }
                catch (InvalidRequestException)
                {
                    // You provided a parameter value that is not valid for the current state of the resource.
                    // Deal with the exception here, and/or rethrow at your discretion.
                    JsonSecrets = JsonDocument.Parse(@"{""secrets_error"": ""InvalidRequestException""}");
                }
                catch (ResourceNotFoundException)
                {
                    // We can't find the resource that you asked for.
                    // Deal with the exception here, and/or rethrow at your discretion.
                    JsonSecrets = JsonDocument.Parse(@"{""secrets_error"": ""ResourceNotFoundException""}");
                }
                catch (AggregateException)
                {
                    // More than one of the above exceptions were triggered.
                    // Deal with the exception here, and/or rethrow at your discretion.
                    JsonSecrets = JsonDocument.Parse(@"{""secrets_error"": ""AggregateException""}");
                }

                // Decrypts secret using the associated KMS CMK.
                // Depending on whether the secret is a string or binary, one of these fields will be populated.
                if (response != null && response.SecretString != null)
                {
                    JsonSecrets = JsonDocument.Parse(response.SecretString);
                }
                else
                {
                    /*
                    memoryStream = response.SecretBinary;
                    StreamReader reader = new StreamReader(memoryStream);
                    string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
                    */
                }
            }
        }

        public static string? GetSecret(string key)
        {
            JsonElement root = JsonSecrets.RootElement;

            if (root.TryGetProperty("secrets_error", out JsonElement errorResult))
            {
                return errorResult.GetString();
            }
            else if (root.TryGetProperty(key, out JsonElement keyResult))
            {
                return keyResult.GetString();
            }

            return null;
        }
    }
}

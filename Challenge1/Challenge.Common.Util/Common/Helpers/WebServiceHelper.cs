using challenge1.Common.Logging;
using challenge1.Common.Util.Common.Helpers.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace challenge1.Common.Util.Common.Helpers
{
    public class WebServiceHelper : IWebServiceHelper
    {
        private readonly ILogging _logging;
        private readonly HttpClient _httpClient;

        public WebServiceHelper(ILogging logging, HttpClient httpClient)
        {
            _logging = logging;
            _httpClient = httpClient;
        }

        //Default content type is application/json
        public async Task<string?> PostWebServiceAsync(string apiUrl, MediaTypeHeaderValue? contentType = null, string? encryptedCookie = null, object? content = null, AuthenticationHeaderValue? authenticationHeader = null, string? x_api_key = null, string? x_apigw_api_id = null)
        {
            string byteArrayContent = string.Empty;

            try
            {
                HttpContent c;
                if (content != null)
                {
                    if (contentType == null || contentType.MediaType == "application/json")
                    {
                        byteArrayContent = JsonSerializer.Serialize(content);
                    }
                    else if (contentType.MediaType == "application/x-www-form-urlencoded")
                    {
                        byteArrayContent = content.ToString() ?? "";
                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported content type: {contentType.MediaType}");
                    }
                }

                c = new ByteArrayContent(Encoding.UTF8.GetBytes(byteArrayContent));
                c.Headers.ContentType = contentType ??= new MediaTypeHeaderValue("application/json");

                if (!string.IsNullOrEmpty(x_api_key))
                {
                    c.Headers.Add("x-api-key", x_api_key);
                }
                if (!string.IsNullOrEmpty(x_apigw_api_id))
                {
                    c.Headers.Add("x-apigw-api-id", x_apigw_api_id);
                }
                if (!string.IsNullOrEmpty(encryptedCookie))
                {
                    c.Headers.Add("UserSession", encryptedCookie);
                }

                using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl) { Content = c };

                if (authenticationHeader != null)
                {
                    request.Headers.Authorization = authenticationHeader;
                }

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logging.LogInfo("Response Content: " + await response.Content.ReadAsStringAsync());
                }
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<string?> GetWebServiceAsync(string apiUrl, AuthenticationHeaderValue? authenticationHeader = null, string? x_api_key = null, string? x_apigw_api_id = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

                if (authenticationHeader != null)
                {
                    request.Headers.Authorization = authenticationHeader;
                }

                if (!string.IsNullOrEmpty(x_api_key))
                {
                    request.Headers.Add("x-api-key", x_api_key);
                }
                if (!string.IsNullOrEmpty(x_apigw_api_id))
                {
                    request.Headers.Add("x-apigw-api-id", x_apigw_api_id);
                }

                var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString());
                return null;
            }
        }


    }
}

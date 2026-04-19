using System.Net.Http.Headers;

namespace challenge1.Common.Util.Common.Helpers.Interfaces
{
    public interface IWebServiceHelper
    {
        Task<string?> PostWebServiceAsync(string apiUrl, MediaTypeHeaderValue? contentType = null, string? encryptedCookie = null, object? content = null, AuthenticationHeaderValue? authenticationHeader = null, string? x_api_key = null, string? x_apigw_api_id = null);
        Task<string?> GetWebServiceAsync(string apiUrl, AuthenticationHeaderValue? authenticationHeader = null, string? x_api_key = null, string? x_apigw_api_id = null);
    }
}

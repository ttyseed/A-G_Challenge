using challenge1.Common.Logging;
using challenge1.Common.Util.WeatherClients.Interfaces;
using challenge1.Common.Util.WeatherClients.Models;
using System.Text.Json;

namespace challenge1.Common.Util.WeatherClients
{
    public class DataGovSgClient : IDataGovSgClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogging _logging;

        private const string BaseUrl = "https://api-open.data.gov.sg/v2/real-time/api";

        public DataGovSgClient(HttpClient httpClient, ILogging logging)
        {
            _httpClient = httpClient;
            _logging = logging;
        }

        public async Task<List<DataGovSgReading>?> GetTwoHourForecastAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{BaseUrl}/two-hr-forecast");
                var result = JsonSerializer.Deserialize<DataGovSgTwoHourResponse>(response);
                return result?.Data?.Items?.FirstOrDefault()?.Forecasts;
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<DataGovSgTwentyFourHourForecast?> GetTwentyFourHourForecastAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{BaseUrl}/twenty-four-hr-forecast");
                var result = JsonSerializer.Deserialize<DataGovSg24HrResponse>(response);
                return result?.Data?.Records?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<List<DataGovSgFourDayOutlook>?> GetFourDayOutlookAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{BaseUrl}/four-day-outlook");
                var result = JsonSerializer.Deserialize<DataGovSgFourDayResponse>(response);
                return result?.Data?.Records;
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString());
                return null;
            }
        }

        public async Task<DataGovSgPsiReading?> GetPsiReadingsAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{BaseUrl}/psi");
                var result = JsonSerializer.Deserialize<DataGovSgPsiResponse>(response);
                return result?.Data?.Items?.FirstOrDefault()?.Readings;
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString());
                return null;
            }
        }
    }
}

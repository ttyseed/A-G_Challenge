using challenge1.Common.Util.WeatherClients.Models;

namespace challenge1.Common.Util.WeatherClients.Interfaces
{
    public interface IDataGovSgClient
    {
        Task<List<DataGovSgReading>?> GetTwoHourForecastAsync();
        Task<DataGovSgTwentyFourHourForecast?> GetTwentyFourHourForecastAsync();
        Task<List<DataGovSgFourDayOutlook>?> GetFourDayOutlookAsync();
        Task<DataGovSgPsiReading?> GetPsiReadingsAsync();
    }
}

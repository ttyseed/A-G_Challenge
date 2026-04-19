using System.Text.Json.Serialization;

namespace challenge1.Common.Util.WeatherClients.Models
{
    public class DataGovSgReading
    {
        [JsonPropertyName("area")]
        public string Area { get; set; } = null!;

        [JsonPropertyName("forecast")]
        public string Forecast { get; set; } = null!;
    }

    public class DataGovSgTwentyFourHourForecast
    {
        [JsonPropertyName("general")]
        public DataGovSgGeneralForecast? General { get; set; }

        [JsonPropertyName("periods")]
        public List<DataGovSgForecastPeriod>? Periods { get; set; }
    }

    public class DataGovSgGeneralForecast
    {
        [JsonPropertyName("forecast")]
        public string? Forecast { get; set; }

        [JsonPropertyName("temperature")]
        public DataGovSgRange? Temperature { get; set; }

        [JsonPropertyName("relativeHumidity")]
        public DataGovSgRange? RelativeHumidity { get; set; }

        [JsonPropertyName("wind")]
        public DataGovSgWind? Wind { get; set; }
    }

    public class DataGovSgForecastPeriod
    {
        [JsonPropertyName("timePeriod")]
        public DataGovSgTimePeriod? TimePeriod { get; set; }

        [JsonPropertyName("regions")]
        public DataGovSgRegions? Regions { get; set; }
    }

    public class DataGovSgTimePeriod
    {
        [JsonPropertyName("start")]
        public string? Start { get; set; }

        [JsonPropertyName("end")]
        public string? End { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    public class DataGovSgRegions
    {
        [JsonPropertyName("north")]
        public string? North { get; set; }

        [JsonPropertyName("south")]
        public string? South { get; set; }

        [JsonPropertyName("east")]
        public string? East { get; set; }

        [JsonPropertyName("west")]
        public string? West { get; set; }

        [JsonPropertyName("central")]
        public string? Central { get; set; }
    }

    public class DataGovSgRange
    {
        [JsonPropertyName("low")]
        public decimal Low { get; set; }

        [JsonPropertyName("high")]
        public decimal High { get; set; }

        [JsonPropertyName("unit")]
        public string? Unit { get; set; }
    }

    public class DataGovSgWind
    {
        [JsonPropertyName("speed")]
        public DataGovSgRange? Speed { get; set; }

        [JsonPropertyName("direction")]
        public string? Direction { get; set; }
    }

    public class DataGovSgFourDayOutlook
    {
        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("forecast")]
        public string? Forecast { get; set; }

        [JsonPropertyName("temperature")]
        public DataGovSgRange? Temperature { get; set; }

        [JsonPropertyName("relativeHumidity")]
        public DataGovSgRange? RelativeHumidity { get; set; }

        [JsonPropertyName("wind")]
        public DataGovSgWind? Wind { get; set; }
    }

    public class DataGovSgPsiReading
    {
        [JsonPropertyName("psi_twenty_four_hourly")]
        public DataGovSgPsiRegional? PsiTwentyFourHourly { get; set; }

        [JsonPropertyName("pm25_one_hourly")]
        public DataGovSgPsiRegional? Pm25OneHourly { get; set; }
    }

    public class DataGovSgPsiRegional
    {
        [JsonPropertyName("national")]
        public decimal? National { get; set; }

        [JsonPropertyName("north")]
        public decimal? North { get; set; }

        [JsonPropertyName("south")]
        public decimal? South { get; set; }

        [JsonPropertyName("east")]
        public decimal? East { get; set; }

        [JsonPropertyName("west")]
        public decimal? West { get; set; }

        [JsonPropertyName("central")]
        public decimal? Central { get; set; }
    }

    // Raw API response wrappers
    public class DataGovSgTwoHourResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("data")]
        public DataGovSgTwoHourData? Data { get; set; }
    }

    public class DataGovSgTwoHourData
    {
        [JsonPropertyName("items")]
        public List<DataGovSgTwoHourItem>? Items { get; set; }
    }

    public class DataGovSgTwoHourItem
    {
        [JsonPropertyName("forecasts")]
        public List<DataGovSgReading>? Forecasts { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }
    }

    public class DataGovSg24HrResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("data")]
        public DataGovSg24HrData? Data { get; set; }
    }

    public class DataGovSg24HrData
    {
        [JsonPropertyName("records")]
        public List<DataGovSgTwentyFourHourForecast>? Records { get; set; }
    }

    public class DataGovSgFourDayResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("data")]
        public DataGovSgFourDayData? Data { get; set; }
    }

    public class DataGovSgFourDayData
    {
        [JsonPropertyName("records")]
        public List<DataGovSgFourDayOutlook>? Records { get; set; }
    }

    public class DataGovSgPsiResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("data")]
        public DataGovSgPsiData? Data { get; set; }
    }

    public class DataGovSgPsiData
    {
        [JsonPropertyName("items")]
        public List<DataGovSgPsiItem>? Items { get; set; }
    }

    public class DataGovSgPsiItem
    {
        [JsonPropertyName("readings")]
        public DataGovSgPsiReading? Readings { get; set; }
    }
}

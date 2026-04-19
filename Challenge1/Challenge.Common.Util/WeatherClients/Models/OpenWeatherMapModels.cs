using System.Text.Json.Serialization;

namespace challenge1.Common.Util.WeatherClients.Models
{
    public class OpenWeatherCurrentResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("dt")]
        public long Dt { get; set; }

        [JsonPropertyName("main")]
        public OpenWeatherMain? Main { get; set; }

        [JsonPropertyName("wind")]
        public OpenWeatherWind? Wind { get; set; }

        [JsonPropertyName("rain")]
        public OpenWeatherRain? Rain { get; set; }

        [JsonPropertyName("weather")]
        public List<OpenWeatherDescription>? Weather { get; set; }

        [JsonPropertyName("coord")]
        public OpenWeatherCoord? Coord { get; set; }
    }

    public class OpenWeatherMain
    {
        [JsonPropertyName("temp")]
        public decimal Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public decimal FeelsLike { get; set; }

        [JsonPropertyName("humidity")]
        public decimal Humidity { get; set; }
    }

    public class OpenWeatherWind
    {
        [JsonPropertyName("speed")]
        public decimal Speed { get; set; }

        [JsonPropertyName("deg")]
        public int Deg { get; set; }
    }

    public class OpenWeatherRain
    {
        [JsonPropertyName("1h")]
        public decimal? OneHour { get; set; }

        [JsonPropertyName("3h")]
        public decimal? ThreeHour { get; set; }
    }

    public class OpenWeatherDescription
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class OpenWeatherCoord
    {
        [JsonPropertyName("lat")]
        public decimal Lat { get; set; }

        [JsonPropertyName("lon")]
        public decimal Lon { get; set; }
    }

    public class OpenWeatherForecastResponse
    {
        [JsonPropertyName("list")]
        public List<OpenWeatherForecastItem>? List { get; set; }

        [JsonPropertyName("city")]
        public OpenWeatherCity? City { get; set; }
    }

    public class OpenWeatherForecastItem
    {
        [JsonPropertyName("dt")]
        public long Dt { get; set; }

        [JsonPropertyName("dt_txt")]
        public string? DtTxt { get; set; }

        [JsonPropertyName("main")]
        public OpenWeatherMain? Main { get; set; }

        [JsonPropertyName("wind")]
        public OpenWeatherWind? Wind { get; set; }

        [JsonPropertyName("rain")]
        public OpenWeatherRain? Rain { get; set; }

        [JsonPropertyName("weather")]
        public List<OpenWeatherDescription>? Weather { get; set; }
    }

    public class OpenWeatherCity
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}

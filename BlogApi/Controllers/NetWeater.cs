using BlogApi;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NetWeatherController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // IHttpClientFactory befecskendezése a hálózati hívásokhoz
        public NetWeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet(Name = "GetNetWeather")]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get(double latitude = 47.4979, double longitude = 19.0402)
        {
            // Példa koordináták: Budapest (47.4979, 19.0402)
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&daily=temperature_2m_max,weather_code&timezone=auto";

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Nem sikerült lekérni az időjárás adatokat.");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<OpenMeteoResponse>(jsonString);

            if (weatherData?.Daily?.Time == null)
            {
                return NotFound("Nem találhatók adatok.");
            }

            var result = new List<WeatherForecast>();

            for (int i = 0; i < weatherData.Daily.Time.Length; i++)
            {
                var tempC = (int)Math.Round(weatherData.Daily.TemperatureMax[i]);
                var code = weatherData.Daily.WeatherCode[i];

                result.Add(new WeatherForecast
                {
                    Date = DateOnly.Parse(weatherData.Daily.Time[i]),
                    TemperatureC = tempC,
                    Summary = MapWeatherCodeToSummary(code)
                });
            }

            return Ok(result);
        }

        // Open-Meteo WMO időjárási kódok átalakítása szöveges leírássá
        private static string MapWeatherCodeToSummary(int code) => code switch
        {
            0 => "Warm",             // Tiszta égbolt
            1 or 2 or 3 => "Mild",   // Gyengén / közepesen felhős
            45 or 48 => "Chilly",    // Köd
            51 or 53 or 55 => "Cool",// Szitálás
            61 or 63 or 65 => "Bracing", // Eső
            71 or 73 or 75 => "Freezing",// Havazás
            95 or 96 or 99 => "Sweltering", // Zivatar
            _ => "Balmy"
        };
    }

    // Az Open-Meteo API válaszának feldolgozásához szükséges modell osztályok
    public class OpenMeteoResponse
    {
        [JsonPropertyName("daily")]
        public DailyData? Daily { get; set; }
    }

    public class DailyData
    {
        [JsonPropertyName("time")]
        public string[] Time { get; set; } = [];

        [JsonPropertyName("temperature_2m_max")]
        public double[] TemperatureMax { get; set; } = [];

        [JsonPropertyName("weather_code")]
        public int[] WeatherCode { get; set; } = [];
    }
}
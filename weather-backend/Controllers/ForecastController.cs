using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using weather_backend.Models;

namespace weather_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ForecastController : ControllerBase
    {
        public IConfiguration configuration;
        public IHttpClientFactory httpClientFactory;

        public ForecastController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory
        )
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Get weather forecast for a global position
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <returns>Weather forecast</returns>
        [HttpGet(Name = "GetForecast")]
        public async Task<IActionResult> Get(float? lat, float? lon)
        {
            if (lat == null || lon == null)
            {
                return BadRequest("Please include a valid latitude and longitude");
            }

            using HttpClient client = httpClientFactory.CreateClient("OpenWeatherMap");
            try
            {
                // Retrieve API Key
                string APIKey = configuration.GetValue<string>("APIKey");

                // Call API with parameters
                HttpResponseMessage response = await client
                    .GetAsync($"/data/2.5/weather?units=metric&lat={lat}&lon={lon}&appid={APIKey}");
                response.EnsureSuccessStatusCode();

                // Deserialize forecast data
                string json = await response.Content.ReadAsStringAsync();
                JsonSerializerOptions options = new()
                {
                    PropertyNameCaseInsensitive = true
                };
                ForecastModel? forecast = JsonSerializer
                    .Deserialize<ForecastModel>(json, options);

                if (forecast == null)
                {
                    return NotFound("Could not find a forecast for the specified location");
                }

                DateTime? sunrise = null;
                if (forecast.Sys?.Sunrise != null)
                {
                    sunrise = DateTimeOffset
                        .FromUnixTimeSeconds((long)forecast.Sys.Sunrise).DateTime.ToLocalTime();
                }

                DateTime? sunset = null;
                if (forecast.Sys?.Sunset != null)
                {
                    sunset = DateTimeOffset
                        .FromUnixTimeSeconds((long)forecast.Sys.Sunset).DateTime.ToLocalTime();
                }

                // Return a flattened result for easier use on frontend
                return Ok(new
                {
                    forecast.Name,
                    forecast.Main?.Temp,
                    MaxTemp = forecast.Main?.Temp_Max,
                    MinTemp = forecast.Main?.Temp_Min,
                    forecast.Main?.Pressure,
                    forecast.Main?.Humidity,
                    Sunrise = sunrise,
                    Sunset = sunset
                });
            } catch
            {
                return BadRequest($"There was a problem getting the forecast");
            }
        }
    }
}

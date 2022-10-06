using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using weather_backend.Models;

namespace weather_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : Controller
    {
        public IConfiguration configuration;
        public IHttpClientFactory httpClientFactory;

        public WeatherController (
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory
        )
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Helper function to deserialize JSON string to object,
        /// Resolves need to configure options every time.
        /// </summary>
        /// <typeparam name="T">Type to deserialzie to</typeparam>
        /// <param name="json">JSON string</param>
        /// <returns>Object to serialize JSON to</returns>
        /// <exception cref="ArgumentNullException">JSON string was not provided</exception>
        /// <exception cref="Exception">Deserialization failed</exception>
        private static T Deserialize<T>(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true
            };

            var obj = JsonSerializer.Deserialize<T>(json, options);
            if (obj == null)
            {
                throw new Exception("Could not deserialize");
            }
            return obj;
        }

        /// <summary>
        /// Get latitude and longitude from loation name
        /// </summary>
        /// <param name="query">Location name to get position of</param>
        /// <param name="key">OpenWeatherMap API key</param>
        /// <returns>A latitude and longitude</returns>
        /// <exception cref="ArgumentNullException">Required parameter not provided</exception>
        /// <exception cref="Exception">Error getting location</exception>
        private async Task<LocationModel> GetLocation(string query, string key)
        {
            using HttpClient client = httpClientFactory.CreateClient("OpenWeatherMap");

            HttpResponseMessage response = await client
                .GetAsync($"/geo/1.0/direct?q={query}&limit=1&appid={key}");
            response.EnsureSuccessStatusCode();

            // Deserialize response to enum of locations
            string json = await response.Content.ReadAsStringAsync();
            IEnumerable<LocationModel> locations = Deserialize<IEnumerable<LocationModel>>(json);

            // Get the first result (OWM API will only return array so need to do this)
            if (!locations.Any())
            {
                throw new Exception("Could not find a location with the provided name");
            }
            return locations.First();
        }

        /// <summary>
        /// Get the current weather forecast for a location position
        /// </summary>
        /// <param name="query">Location name to get forecast for</param>
        /// <returns>Weather forecast</returns>
        [HttpGet(Name = "GetForecast")]
        public async Task<IActionResult> Get(string? query)
        {
            // Fail if no query provided
            if (query == null) return BadRequest("No location name provided");

            // Retrieve API key
            string key = configuration.GetValue<string>("APIKey");

            try
            {
                // Get latitude and longitude from location name
                LocationModel? location = await GetLocation(query, key);

                // Fail if an invalid location is returned
                if (location == null) return BadRequest("Invalid location");

                // Set up HTTP client
                using HttpClient client = httpClientFactory.CreateClient("OpenWeatherMap");

                // Call API
                string uri = "/data/2.5/weather?units=metric" +
                    $"&lat={location.Lat}&lon={location.Lon}&appid={key}";
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                // Deserialize response
                string json = await response.Content.ReadAsStringAsync();
                ForecastModel forecast = Deserialize<ForecastModel>(json);

                // Convert sunrise and sunset from unix epoch time to local DateTime
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

                // Return flattened result
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
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

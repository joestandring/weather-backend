using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using weather_backend.Models;

namespace weather_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;

        public LocationController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory
        )
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Get latitude, longitude, and name of a location
        /// </summary>
        /// <param name="searchTerm">Search term to query</param>
        /// <returns>Location information</returns>
        [HttpGet(Name = "GetLocation")]
        public async Task<IActionResult> Get(string? searchTerm)
        {
            // Dont accept empty search terms or those with the string "test"
            // Using "test" does not return expected values
            if (searchTerm == null || searchTerm.ToLower() == "test")
            {
                return BadRequest("Please input a location name");
            }

            using HttpClient? client = httpClientFactory.CreateClient("OpenWeatherMap");
            try
            {
                // Retrieve API key
                string APIKey = configuration.GetValue<string>("APIKey");

                // Call API with parameters
                HttpResponseMessage response = await client
                    .GetAsync($"/geo/1.0/direct?q={searchTerm}&limit=1&appid={APIKey}");
                response.EnsureSuccessStatusCode();

                // Deserialize response to enum of locations
                string json = await response.Content.ReadAsStringAsync();
                JsonSerializerOptions options = new()
                {
                    PropertyNameCaseInsensitive = true
                };
                IEnumerable<LocationModel>? locations = JsonSerializer
                    .Deserialize<IEnumerable<LocationModel>>(json, options);

                // Get the first result (OWM API will only return array so need to do this)
                LocationModel? location = null;
                if (locations != null && locations.Count() > 0)
                {
                    location = locations.FirstOrDefault();
                } else
                {
                    return NotFound("Sorry, we were not able to find a location with that name");
                }

                // Return 
                return Ok(location);
            }
            catch
            {
                return BadRequest($"There was an problem getting your location");
            }
        }
    }
}
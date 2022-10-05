using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using weather_backend.ViewModels;

namespace weather_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationController : ControllerBase
    {
        public IConfiguration configuration;

        public LocationController(IConfiguration iConfiguration)
        {
            configuration = iConfiguration;
        }

        /// <summary>
        /// Get latitude, longitude, and name of a location
        /// </summary>
        /// <param name="searchTerm">Search term to query</param>
        /// <returns>Information for a location</returns>
        [HttpGet(Name = "GetLocation")]
        public async Task<IActionResult> Get(string searchTerm)
        {
            using var client = new HttpClient();
            try
            {
                // Set up API address
                client.BaseAddress = new Uri("https://api.openweathermap.org");
                string APIKey = configuration.GetValue<string>("APIKey");

                // Call API with parameters
                HttpResponseMessage response = await client
                    .GetAsync($"/get/1.0/direct?q={searchTerm}&limit=1&appid={APIKey}");
                response.EnsureSuccessStatusCode();

                // Convert response to enum of locations
                string json = await response.Content.ReadAsStringAsync();
                IEnumerable<LocationViewModel>? locations = JsonSerializer
                    .Deserialize<IEnumerable<LocationViewModel>>(json);

                // Get the first result (OWM API will only return array so need to do this)
                LocationViewModel? location = null;
                if (locations != null)
                {
                    location = locations.FirstOrDefault();
                }

                // Return 
                return Ok(new LocationViewModel
                {
                    Name = location?.Name,
                    Country = location?.Country,
                    Lat = location?.Lat,
                    Long = location?.Long
                });
            }
            catch
            {
                return BadRequest($"There was an error getting your location");
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using weather_backend.Controllers;

namespace UnitTests
{
    /// <summary>
    /// Tests for actions in WeatherController
    /// </summary>
    public class WeatherTests
    {
        /// <summary>
        /// WeatherController Get should return a BadRequest result if query is null
        /// </summary>
        [Fact]
        public async void NoArgumentProvided()
        {
            Mock<IHttpClientFactory> mockFactory = new();
            Mock<IConfiguration> mockConfiguration = new();

            WeatherController weatherController = new(mockConfiguration.Object, mockFactory.Object);

            IActionResult result = await weatherController.Get("");
            BadRequestObjectResult? actual = result as BadRequestObjectResult;

            Assert.NotNull(actual);
        }

        /// <summary>
        /// WeatherController Get should return a BadRequest result if no location matches
        /// </summary>
        /*
        [Fact]
        public async void LocationNotFound() {
            WeatherController weatherController = new(configuration, httpClientFactory);

            IActionResult result = await weatherController.Get("ThisPlaceDoesNotExist");
            BadRequestObjectResult? actual = result as BadRequestObjectResult;

            Assert.NotNull(actual);
        }

        /// <summary>
        /// WeatherController Get should return successfully if a forecast has been found
        /// </summary>
        [Fact]
        public async void Success()
        {
            WeatherController weatherController = new(configuration, httpClientFactory);

            IActionResult result = await weatherController.Get("Peterborough");
            OkObjectResult? actual = result as OkObjectResult;

            Assert.NotNull(actual);
            Assert.Equal(200, actual?.StatusCode);
        }
        */
    }
}
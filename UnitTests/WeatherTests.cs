using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
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
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                .AddEnvironmentVariables()
                .Build();

            WeatherController weatherController = new(config, mockFactory.Object);

            IActionResult result = await weatherController.Get("");
            BadRequestObjectResult? actual = result as BadRequestObjectResult;

            Assert.NotNull(actual);
        }

        /// <summary>
        /// WeatherController Get should return a BadRequest result if no location matches
        /// </summary>
        [Fact]
        public async void LocationNotFound() {
            Mock<IHttpClientFactory> mockFactory = new();
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                .AddEnvironmentVariables()
                .Build();

            WeatherController weatherController = new(config, mockFactory.Object);

            IActionResult result = await weatherController.Get("ThisPlaceDoesNotExist");
            BadRequestObjectResult? actual = result as BadRequestObjectResult;

            Assert.NotNull(actual);
        }

        /// <summary>
        /// WeatherController Get should return successfully if a forecast has been found
        /// </summary>
        /*
        [Fact]
        public async void Success()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                .AddEnvironmentVariables()
                .Build();

            Mock<HttpMessageHandler> mockHandler = new();
            HttpResponseMessage response = new();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response)
                .Verifiable();
            HttpClient httpClient = new(mockHandler.Object)
            {
                BaseAddress = new Uri("https://api.openweathermap.org")
            };
            Mock<IHttpClientFactory> mockFactory = new();
            mockFactory.Setup(_ => _.CreateClient("OpenWeatherMap")).Returns(httpClient);

            WeatherController weatherController = new(config, mockFactory.Object);

            IActionResult result = await weatherController.Get("Peterborough");
            OkObjectResult? actual = result as OkObjectResult;

            Assert.IsAssignableFrom<OkObjectResult>(actual);
        }
        */
    }
}
using Xunit;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ChatGPT.Weather.Tests
{
    public class WeatherControllerTests
    {
        [Fact]
        public async Task GetWeatherForecast_WithValidCoordinates_ReturnsWeatherData()
        {
            await using var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            // Arrange
            double latitude = 47.6;
            double longitude = -122.3;

            // Act
            var result = await client.GetFromJsonAsync<GetWeatherResponse>($"/weather?latitude={latitude}&longitude={longitude}");
            // Assert
            Assert.NotNull(result);
           // Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            //Assert.NotNull(result.Content);
            //Assert.IsType<GetWeatherResponse>(result.Content);
            //var response = (GetWeatherResponse)result.Content;
            Assert.InRange(result.temperature, -100.0, 100.0); // Check that temperature is within a reasonable range
            Assert.InRange(result.windspeed, 0.0, 100.0); // Check that wind speed is within a reasonable range
            Assert.InRange(result.winddirection, 0.0, 360.0); // Check that wind direction is within a reasonable range
        }
        //[Fact]
        //public async Task GetWeatherForecast_WithInvalidCoordinates_ReturnsBadRequest()
        //{
        //    // Arrange
        //    var controller = new ChatGPT.Weather.src.GetWeatherResponse();
        //    var latitude = 200.0; // invalid latitude
        //    var longitude = -122.3;
        //    // Act
        //    var result = await controller.GetWeatherForecast(latitude, longitude);
        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        //    Assert.Null(result.Value);
        //}
    }
}
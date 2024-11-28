using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Diagnostics.CodeAnalysis;
using Web_Api.Controllers;

public class BikeControllerTests
{
    private readonly Mock<ITransportationService> _mockTransportationService;
    private readonly BikeController _controller;

    public BikeControllerTests()
    {
        _mockTransportationService = new Mock<ITransportationService>();
        _controller = new BikeController(_mockTransportationService.Object);
    }

    [Fact]
    public void RentBike_Success_ReturnsOkResult()
    {
        // Arrange
        int userId = 1;
        string bikeId = "BIKE001";
        DateTime startTime = DateTime.Now;

        _mockTransportationService
            .Setup(s => s.RentBike(It.IsAny<int>(), It.IsAny<string>(), out startTime))
            .Returns(true);

        // Act
        var result = _controller.RentBike(userId, bikeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var responseDict = Assert.IsType<Dictionary<string, object>>(
            ConvertAnonymousObjectToDictionary(okResult.Value));
        Assert.Equal("Vélo loué avec succès.", responseDict["Message"]);
        Assert.NotNull(responseDict["RentalStartTime"]);
    }

    [Fact]
    public void RentBike_Failure_ReturnsBadRequest()
    {
        // Arrange
        int userId = 1;
        string bikeId = "BIKE001";
        _mockTransportationService.Setup(s => s.RentBike(
            userId,
            bikeId,
            out It.Ref<DateTime>.IsAny))
            .Returns(false);

        // Act
        var result = _controller.RentBike(userId, bikeId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var responseDict = Assert.IsType<Dictionary<string, object>>(
            ConvertAnonymousObjectToDictionary(badRequestResult.Value));
        Assert.Equal("Échec de la location du vélo.", responseDict["Message"]);
    }
    [Fact]
    public void RentBike_InvalidOperationException_ReturnsBadRequest()
    {
        // Arrange
        int userId = 1;
        string bikeId = "BIKE001";
        var expectedMessage = "Message d'erreur spécifique";
        _mockTransportationService.Setup(s => s.RentBike(
            userId,
            bikeId,
            out It.Ref<DateTime>.IsAny))
            .Throws(new InvalidOperationException(expectedMessage));

        // Act
        var result = _controller.RentBike(userId, bikeId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var responseDict = Assert.IsType<Dictionary<string, object>>(
            ConvertAnonymousObjectToDictionary(badRequestResult.Value));
        Assert.Equal(expectedMessage, responseDict["Message"]);
    }

    [Fact]
    public void RentBike_Exception_Returns500()
    {
        // Arrange
        int userId = 1;
        string bikeId = "BIKE001";
        var expectedError = "Test error";
        _mockTransportationService.Setup(s => s.RentBike(
            userId,
            bikeId,
            out It.Ref<DateTime>.IsAny))
            .Throws(new Exception(expectedError));

        // Act
        var result = _controller.RentBike(userId, bikeId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var responseDict = Assert.IsType<Dictionary<string, object>>(
            ConvertAnonymousObjectToDictionary(statusCodeResult.Value));
        Assert.Equal("Une erreur interne s'est produite lors de la location.", responseDict["Message"]);
        Assert.Equal(expectedError, responseDict["Error"]);
    }

    [Fact]
    public void EndBikeRental_Success_ReturnsOkResult()
    {
        // Arrange
        int userId = 1;
        string bikeId = "BIKE001";
        _mockTransportationService.Setup(s => s.EndBikeRental(
            userId,
            bikeId,
            It.IsAny<DateTime>()))
            .Returns(true);

        // Act
        var result = _controller.EndBikeRental(userId, bikeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        var responseDict = Assert.IsType<Dictionary<string, object>>(
            ConvertAnonymousObjectToDictionary(okResult.Value));
        Assert.Equal("Location de vélo terminée avec succès.", responseDict["Message"]);
        Assert.NotNull(responseDict["RentalEndTime"]);
    }

    [Fact]
    public void EndBikeRental_Failure_ReturnsBadRequest()
    {
        // Arrange
        int userId = 1;
        string bikeId = "BIKE001";
        _mockTransportationService.Setup(s => s.EndBikeRental(
            userId,
            bikeId,
            It.IsAny<DateTime>()))
            .Returns(false);

        // Act
        var result = _controller.EndBikeRental(userId, bikeId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var responseDict = Assert.IsType<Dictionary<string, object>>(
            ConvertAnonymousObjectToDictionary(badRequestResult.Value));
        Assert.Equal("Échec de la fin de la location du vélo.", responseDict["Message"]);
    }

    [Fact]
    public void EndBikeRental_Exception_Returns500()
    {
        // Arrange
        int userId = 1;
        string bikeId = "BIKE001";
        var expectedError = "Test error";
        _mockTransportationService.Setup(s => s.EndBikeRental(
            userId,
            bikeId,
            It.IsAny<DateTime>()))
            .Throws(new Exception(expectedError));

        // Act
        var result = _controller.EndBikeRental(userId, bikeId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var responseDict = Assert.IsType<Dictionary<string, object>>(
            ConvertAnonymousObjectToDictionary(statusCodeResult.Value));
        Assert.Equal("Une erreur interne s'est produite lors de la fin de location.", responseDict["Message"]);
        Assert.Equal(expectedError, responseDict["Error"]);
    }

    [Fact]
    public void EndBikeRental_InvalidOperationException_ReturnsBadRequest()
    {
        // Arrange
        int userId = 1;
        string bikeId = "BIKE001";
        var expectedMessage = "Message d'erreur spécifique";
        _mockTransportationService.Setup(s => s.EndBikeRental(
            userId,
            bikeId,
            It.IsAny<DateTime>()))
            .Throws(new InvalidOperationException(expectedMessage));

        // Act
        var result = _controller.EndBikeRental(userId, bikeId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var responseDict = Assert.IsType<Dictionary<string, object>>(
            ConvertAnonymousObjectToDictionary(badRequestResult.Value));
        Assert.Equal(expectedMessage, responseDict["Message"]);
    }

    [ExcludeFromCodeCoverage]
    private static Dictionary<string, object> ConvertAnonymousObjectToDictionary(object obj)
    {
        return obj.GetType()
            .GetProperties()
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj));
    }
}
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Web_Api.Controllers;
using BLL.Services;
using System;

namespace Web_Api.Tests.Controllers
{
    public class SharedVehicleControllerTests
    {
        private readonly Mock<ITransportationService> _mockTransportationService;
        private readonly SharedVehicleController _controller;

        public SharedVehicleControllerTests()
        {
            _mockTransportationService = new Mock<ITransportationService>();
            _controller = new SharedVehicleController(_mockTransportationService.Object);
        }

        private static Dictionary<string, object> ConvertAnonymousObjectToDictionary(object obj)
        {
            return obj.GetType()
                .GetProperties()
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj));
        }

        [Fact]
        public void CreateSharedVehicleTrip_Success_ReturnsOkResult()
        {
            // Arrange
            int driverId = 1;
            string vehicleId = "SHARE001";
            _mockTransportationService
                .Setup(s => s.CreateSharedVehicleTrip(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()))
                .Returns(true);

            // Act
            var result = _controller.CreateSharedVehicleTrip(driverId, vehicleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(okResult.Value));
            Assert.Equal("Trajet en véhicule partagé créé avec succès.", responseDict["Message"]);
            Assert.NotNull(responseDict["RentalStartTime"]);
        }

        [Fact]
        public void CreateSharedVehicleTrip_Failure_ReturnsBadRequest()
        {
            // Arrange
            int driverId = 1;
            string vehicleId = "SHARE001";
            _mockTransportationService
                .Setup(s => s.CreateSharedVehicleTrip(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()))
                .Returns(false);

            // Act
            var result = _controller.CreateSharedVehicleTrip(driverId, vehicleId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(badRequestResult.Value));
            Assert.Equal("Échec de la création du trajet en véhicule partagé.", responseDict["Message"]);
        }

        [Fact]
        public void CreateSharedVehicleTrip_InvalidOperationException_ReturnsBadRequest()
        {
            // Arrange
            int driverId = 1;
            string vehicleId = "SHARE001";
            var expectedMessage = "Message d'erreur spécifique";
            _mockTransportationService
                .Setup(s => s.CreateSharedVehicleTrip(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()))
                .Throws(new InvalidOperationException(expectedMessage));

            // Act
            var result = _controller.CreateSharedVehicleTrip(driverId, vehicleId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(badRequestResult.Value));
            Assert.Equal(expectedMessage, responseDict["Message"]);
        }

        [Fact]
        public void CreateSharedVehicleTrip_Exception_Returns500()
        {
            // Arrange
            int driverId = 1;
            string vehicleId = "SHARE001";
            var expectedError = "Test error";
            _mockTransportationService
                .Setup(s => s.CreateSharedVehicleTrip(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()))
                .Throws(new Exception(expectedError));

            // Act
            var result = _controller.CreateSharedVehicleTrip(driverId, vehicleId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(statusCodeResult.Value));
            Assert.Equal("Une erreur interne s'est produite lors de la création du trajet.", responseDict["Message"]);
            Assert.Equal(expectedError, responseDict["Error"]);
        }

        [Fact]
        public void RentSharedVehicle_Success_ReturnsOkResult()
        {
            // Arrange
            int userId = 1;
            string vehicleId = "SHARE001";
            int driverId = 2;
            DateTime startTime = DateTime.Now;

            _mockTransportationService
                .Setup(s => s.RentSharedVehicle(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    out startTime,
                    It.IsAny<int>()))
                .Returns(true);

            // Act
            var result = _controller.RentSharedVehicle(userId, vehicleId, driverId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(okResult.Value));
            Assert.Equal("Véhicule partagé loué avec succès.", responseDict["Message"]);
            Assert.NotNull(responseDict["RentalStartTime"]);
        }

        [Fact]
        public void RentSharedVehicle_Failure_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            string vehicleId = "SHARE001";
            int driverId = 2;

            _mockTransportationService
                .Setup(s => s.RentSharedVehicle(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    out It.Ref<DateTime>.IsAny,
                    It.IsAny<int>()))
                .Returns(false);

            // Act
            var result = _controller.RentSharedVehicle(userId, vehicleId, driverId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(badRequestResult.Value));
            Assert.Equal("Échec de la location du véhicule partagé.", responseDict["Message"]);
        }

        [Fact]
        public void RentSharedVehicle_InvalidOperationException_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            string vehicleId = "SHARE001";
            int driverId = 2;
            var expectedMessage = "Message d'erreur spécifique";

            _mockTransportationService
                .Setup(s => s.RentSharedVehicle(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    out It.Ref<DateTime>.IsAny,
                    It.IsAny<int>()))
                .Throws(new InvalidOperationException(expectedMessage));

            // Act
            var result = _controller.RentSharedVehicle(userId, vehicleId, driverId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(badRequestResult.Value));
            Assert.Equal(expectedMessage, responseDict["Message"]);
        }

        [Fact]
        public void RentSharedVehicle_Exception_Returns500()
        {
            // Arrange
            int userId = 1;
            string vehicleId = "SHARE001";
            int driverId = 2;
            var expectedError = "Test error";

            _mockTransportationService
                .Setup(s => s.RentSharedVehicle(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    out It.Ref<DateTime>.IsAny,
                    It.IsAny<int>()))
                .Throws(new Exception(expectedError));

            // Act
            var result = _controller.RentSharedVehicle(userId, vehicleId, driverId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(statusCodeResult.Value));
            Assert.Equal("Une erreur interne s'est produite lors de la location.", responseDict["Message"]);
            Assert.Equal(expectedError, responseDict["Error"]);
        }

        [Fact]
        public void EndSharedVehicleRental_Success_ReturnsOkResult()
        {
            // Arrange
            string vehicleId = "SHARE001";
            int driverId = 1;

            _mockTransportationService
                .Setup(s => s.EndSharedVehicleRental(
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(true);

            // Act
            var result = _controller.EndSharedVehicleRental(vehicleId, driverId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(okResult.Value));
            Assert.Equal("Trajet en véhicule partagé terminé avec succès.", responseDict["Message"]);
        }

        [Fact]
        public void EndSharedVehicleRental_Failure_ReturnsBadRequest()
        {
            // Arrange
            string vehicleId = "SHARE001";
            int driverId = 1;

            _mockTransportationService
                .Setup(s => s.EndSharedVehicleRental(
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(false);

            // Act
            var result = _controller.EndSharedVehicleRental(vehicleId, driverId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(badRequestResult.Value));
            Assert.Equal("Échec de la fin du trajet en véhicule partagé.", responseDict["Message"]);
        }

        [Fact]
        public void EndSharedVehicleRental_InvalidOperationException_ReturnsBadRequest()
        {
            // Arrange
            string vehicleId = "SHARE001";
            int driverId = 1;
            var expectedMessage = "Message d'erreur spécifique";

            _mockTransportationService
                .Setup(s => s.EndSharedVehicleRental(
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Throws(new InvalidOperationException(expectedMessage));

            // Act
            var result = _controller.EndSharedVehicleRental(vehicleId, driverId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(badRequestResult.Value));
            Assert.Equal(expectedMessage, responseDict["Message"]);
        }

        [Fact]
        public void EndSharedVehicleRental_Exception_Returns500()
        {
            // Arrange
            string vehicleId = "SHARE001";
            int driverId = 1;
            var expectedError = "Test error";

            _mockTransportationService
                .Setup(s => s.EndSharedVehicleRental(
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Throws(new Exception(expectedError));

            // Act
            var result = _controller.EndSharedVehicleRental(vehicleId, driverId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(statusCodeResult.Value));
            Assert.Equal("Une erreur interne s'est produite lors de la fin du trajet.", responseDict["Message"]);
            Assert.Equal(expectedError, responseDict["Error"]);
        }
    }
}
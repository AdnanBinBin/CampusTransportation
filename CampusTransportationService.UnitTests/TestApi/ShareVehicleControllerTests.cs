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
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void CreateSharedVehicleTrip_Exception_Returns500()
        {
            // Arrange
            int driverId = 1;
            string vehicleId = "SHARE001";
            _mockTransportationService
                .Setup(s => s.CreateSharedVehicleTrip(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>()))
                .Throws(new Exception("Test error"));

            // Act
            var result = _controller.CreateSharedVehicleTrip(driverId, vehicleId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
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
                    out It.Ref<DateTime>.IsAny,
                    It.IsAny<int>()))
                .Returns(true);

            // Act
            var result = _controller.RentSharedVehicle(userId, vehicleId, driverId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
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
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void RentSharedVehicle_Exception_Returns500()
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
                .Throws(new Exception("Test error"));

            // Act
            var result = _controller.RentSharedVehicle(userId, vehicleId, driverId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
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
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void EndSharedVehicleRental_Exception_Returns500()
        {
            // Arrange
            string vehicleId = "SHARE001";
            int driverId = 1;

            _mockTransportationService
                .Setup(s => s.EndSharedVehicleRental(
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Throws(new Exception("Test error"));

            // Act
            var result = _controller.EndSharedVehicleRental(vehicleId, driverId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
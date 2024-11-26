using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Web_Api.Controllers;
using BLL.Services;
using System;
using DAL.DB;
using Microsoft.EntityFrameworkCore;

namespace Web_Api.Tests.Controllers
{
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
            var okResult = Assert.IsType<OkObjectResult>(result);  // On vérifie que c'est un OkObjectResult
            Assert.Equal(200, okResult.StatusCode);  // On vérifie le status code
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
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void RentBike_Exception_Returns500()
        {
            // Arrange
            int userId = 1;
            string bikeId = "BIKE001";
            _mockTransportationService.Setup(s => s.RentBike(
                userId,
                bikeId,
                out It.Ref<DateTime>.IsAny))
                .Throws(new Exception("Test error"));

            // Act
            var result = _controller.RentBike(userId, bikeId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
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
            var okResult = Assert.IsType<OkObjectResult>(result);  // Vérifie que c'est un OkObjectResult
            Assert.Equal(200, okResult.StatusCode);  // Vérifie le status code
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
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void EndBikeRental_Exception_Returns500()
        {
            // Arrange
            int userId = 1;
            string bikeId = "BIKE001";
            _mockTransportationService.Setup(s => s.EndBikeRental(
                userId,
                bikeId,
                It.IsAny<DateTime>()))
                .Throws(new Exception("Test error"));

            // Act
            var result = _controller.EndBikeRental(userId, bikeId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

      

      
    }
}
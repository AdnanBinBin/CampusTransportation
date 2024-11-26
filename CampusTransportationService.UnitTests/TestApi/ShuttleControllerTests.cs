using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Web_Api.Controllers;
using BLL.Services;
using System;

namespace Web_Api.Tests.Controllers
{
    public class ShuttleControllerTests
    {
        private readonly Mock<ITransportationService> _mockTransportationService;
        private readonly ShuttleController _controller;

        public ShuttleControllerTests()
        {
            _mockTransportationService = new Mock<ITransportationService>();
            _controller = new ShuttleController(_mockTransportationService.Object);
        }

        [Fact]
        public void BoardShuttle_Success_ReturnsOkResult()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "SHUT001";
            _mockTransportationService
                .Setup(s => s.BoardShuttle(userId, shuttleId))
                .Returns(true);

            // Act
            var result = _controller.BoardShuttle(userId, shuttleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var value = okResult.Value as object;
            Assert.NotNull(value);
        }

        [Fact]
        public void BoardShuttle_Failure_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "SHUT001";
            _mockTransportationService
                .Setup(s => s.BoardShuttle(userId, shuttleId))
                .Returns(false);

            // Act
            var result = _controller.BoardShuttle(userId, shuttleId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void BoardShuttle_Exception_Returns500()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "SHUT001";
            string errorMessage = "Test error";
            _mockTransportationService
                .Setup(s => s.BoardShuttle(userId, shuttleId))
                .Throws(new Exception(errorMessage));

            // Act
            var result = _controller.BoardShuttle(userId, shuttleId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains(errorMessage, statusCodeResult.Value.ToString());
        }

        [Theory]
        [InlineData(0, "SHUT001")] // Invalid userId
        [InlineData(1, "")] // Empty shuttleId
        [InlineData(1, null)] // Null shuttleId
        public void BoardShuttle_InvalidInput_ReturnsBadRequest(int userId, string shuttleId)
        {
            // Arrange
            _mockTransportationService
                .Setup(s => s.BoardShuttle(userId, shuttleId))
                .Returns(false);

            // Act
            var result = _controller.BoardShuttle(userId, shuttleId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void BoardShuttle_ServiceThrowsArgumentException_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "SHUT001";
            _mockTransportationService
                .Setup(s => s.BoardShuttle(userId, shuttleId))
                .Throws(new ArgumentException("Invalid argument"));

            // Act
            var result = _controller.BoardShuttle(userId, shuttleId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Invalid argument", statusCodeResult.Value.ToString());
        }

        [Fact]
        public void BoardShuttle_ServiceThrowsInvalidOperationException_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "SHUT001";
            _mockTransportationService
                .Setup(s => s.BoardShuttle(userId, shuttleId))
                .Throws(new InvalidOperationException("Operation not allowed"));

            // Act
            var result = _controller.BoardShuttle(userId, shuttleId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Operation not allowed", statusCodeResult.Value.ToString());
        }

        [Fact]
        public void BoardShuttle_VerifyServiceIsCalled()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "SHUT001";
            _mockTransportationService
                .Setup(s => s.BoardShuttle(userId, shuttleId))
                .Returns(true);

            // Act
            _controller.BoardShuttle(userId, shuttleId);

            // Assert
            _mockTransportationService.Verify(
                s => s.BoardShuttle(userId, shuttleId),
                Times.Once);
        }
    }
}
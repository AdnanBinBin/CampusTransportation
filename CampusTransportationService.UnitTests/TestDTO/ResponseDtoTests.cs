using Xunit;
using System;
using ConsoleApp1.DTOs.Responses;

namespace ConsoleApp1.Tests.DTOs.Responses
{
    public class ResponseDtoTests
    {
        [Fact]
        public void RentBikeResponseDto_PropertiesSetAndGet_CorrectValues()
        {
            // Arrange
            var expectedMessage = "Bike rented successfully";
            var expectedStartTime = DateTime.Now;
            var response = new RentBikeResponseDto
            {
                Message = expectedMessage,
                RentalStartTime = expectedStartTime
            };

            // Act & Assert
            Assert.Equal(expectedMessage, response.Message);
            Assert.Equal(expectedStartTime, response.RentalStartTime);
        }

        [Fact]
        public void EndBikeRentalResponseDto_PropertiesSetAndGet_CorrectValues()
        {
            // Arrange
            var expectedMessage = "Rental ended successfully";
            var expectedEndTime = DateTime.Now;
            var response = new EndBikeRentalResponseDto
            {
                Message = expectedMessage,
                RentalEndTime = expectedEndTime
            };

            // Act & Assert
            Assert.Equal(expectedMessage, response.Message);
            Assert.Equal(expectedEndTime, response.RentalEndTime);
        }

        [Fact]
        public void BoardShuttleResponseDto_PropertiesSetAndGet_CorrectValues()
        {
            // Arrange
            var expectedMessage = "Successfully boarded shuttle";
            var expectedBoardingTime = DateTime.Now;
            var response = new BoardShuttleResponseDto
            {
                Message = expectedMessage,
                BoardingTime = expectedBoardingTime
            };

            // Act & Assert
            Assert.Equal(expectedMessage, response.Message);
            Assert.Equal(expectedBoardingTime, response.BoardingTime);
        }

        [Fact]
        public void SharedVehicleResponseDto_PropertiesSetAndGet_CorrectValues()
        {
            // Arrange
            var expectedMessage = "Shared vehicle operation successful";
            var expectedStartTime = DateTime.Now;
            var expectedEndTime = DateTime.Now.AddHours(2);
            var expectedDriverId = 1;
            var response = new SharedVehicleResponseDto
            {
                Message = expectedMessage,
                RentalStartTime = expectedStartTime,
                RentalEndTime = expectedEndTime,
                DriverId = expectedDriverId
            };

            // Act & Assert
            Assert.Equal(expectedMessage, response.Message);
            Assert.Equal(expectedStartTime, response.RentalStartTime);
            Assert.Equal(expectedEndTime, response.RentalEndTime);
            Assert.Equal(expectedDriverId, response.DriverId);
        }

        [Fact]
        public void SharedVehicleResponseDto_NullableProperties_CanBeNull()
        {
            // Arrange
            var response = new SharedVehicleResponseDto
            {
                Message = "Test message",
                RentalStartTime = null,
                RentalEndTime = null,
                DriverId = null
            };

            // Act & Assert
            Assert.NotNull(response.Message);
            Assert.Null(response.RentalStartTime);
            Assert.Null(response.RentalEndTime);
            Assert.Null(response.DriverId);
        }

        [Fact]
        public void RentBikeResponseDto_DefaultDateTime_NotEqualToMinValue()
        {
            // Arrange
            var response = new RentBikeResponseDto();

            // Act & Assert
            Assert.Equal(default(DateTime), response.RentalStartTime);
        }

        [Fact]
        public void EndBikeRentalResponseDto_DefaultDateTime_NotEqualToMinValue()
        {
            // Arrange
            var response = new EndBikeRentalResponseDto();

            // Act & Assert
            Assert.Equal(default(DateTime), response.RentalEndTime);
        }

        [Fact]
        public void BoardShuttleResponseDto_DefaultDateTime_NotEqualToMinValue()
        {
            // Arrange
            var response = new BoardShuttleResponseDto();

            // Act & Assert
            Assert.Equal(default(DateTime), response.BoardingTime);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void AllResponseDtos_MessageProperty_CanBeNullOrEmpty(string message)
        {
            // Arrange & Act
            var rentBikeResponse = new RentBikeResponseDto { Message = message };
            var endBikeResponse = new EndBikeRentalResponseDto { Message = message };
            var boardShuttleResponse = new BoardShuttleResponseDto { Message = message };
            var sharedVehicleResponse = new SharedVehicleResponseDto { Message = message };

            // Assert
            Assert.Equal(message, rentBikeResponse.Message);
            Assert.Equal(message, endBikeResponse.Message);
            Assert.Equal(message, boardShuttleResponse.Message);
            Assert.Equal(message, sharedVehicleResponse.Message);
        }
    }
}
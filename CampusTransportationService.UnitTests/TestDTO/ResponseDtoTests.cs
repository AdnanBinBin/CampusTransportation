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
        public void ApiErrorResponse_PropertiesSetAndGet_CorrectValues()
        {
            // Arrange
            var expectedMessage = "An error occurred";
            var expectedError = "Invalid operation";
            var response = new ApiErrorResponse
            {
                Message = expectedMessage,
                Error = expectedError
            };

            // Act & Assert
            Assert.Equal(expectedMessage, response.Message);
            Assert.Equal(expectedError, response.Error);
        }

        [Fact]
        public void UserTransportationResponseDto_PropertiesSetAndGet_CorrectValues()
        {
            // Arrange
            var expectedMessage = "Transportation transactions retrieved";
            var expectedTransactions = new List<TransportationTransactionDto>
            {
                new TransportationTransactionDto
                {
                    Id = 1,
                    UserId = 1,
                    ShuttleId = "SHUT001",
                    BikeId = "BIKE001",
                    SharedVehiculeId = "SHARE001",
                    Date = DateTime.Now,
                    RentalStartTime = DateTime.Now,
                    RentalEndTime = DateTime.Now.AddHours(2)
                }
            };
            var response = new UserTransportationResponseDto
            {
                Message = expectedMessage,
                Transactions = expectedTransactions
            };

            // Act & Assert
            Assert.Equal(expectedMessage, response.Message);
            Assert.Single(response.Transactions);
            Assert.Equal(expectedTransactions[0].Id, response.Transactions[0].Id);
            Assert.Equal(expectedTransactions[0].UserId, response.Transactions[0].UserId);
        }

        [Fact]
        public void UserPaymentResponseDto_PropertiesSetAndGet_CorrectValues()
        {
            // Arrange
            var expectedMessage = "Payment transactions retrieved";
            var expectedTransactions = new List<PaymentTransactionDto>
            {
                new PaymentTransactionDto
                {
                    Id = 1,
                    UserId = 1,
                    Amount = 50.0m,
                    Date = DateTime.Now,
                    PaymentMethod = "CreditCard",
                    IsRefund = false
                }
            };
            var response = new UserPaymentResponseDto
            {
                Message = expectedMessage,
                Transactions = expectedTransactions
            };

            // Act & Assert
            Assert.Equal(expectedMessage, response.Message);
            Assert.Single(response.Transactions);
            Assert.Equal(expectedTransactions[0].Id, response.Transactions[0].Id);
            Assert.Equal(expectedTransactions[0].Amount, response.Transactions[0].Amount);
        }

        [Fact]
        public void TransportationTransactionDto_PropertiesSetAndGet_CorrectValues()
        {
            // Arrange
            var expectedId = 1;
            var expectedUserId = 2;
            var expectedShuttleId = "SHUT001";
            var expectedBikeId = "BIKE001";
            var expectedSharedVehiculeId = "SHARE001";
            var expectedDate = DateTime.Now;
            var expectedStartTime = DateTime.Now;
            var expectedEndTime = DateTime.Now.AddHours(2);

            var dto = new TransportationTransactionDto
            {
                Id = expectedId,
                UserId = expectedUserId,
                ShuttleId = expectedShuttleId,
                BikeId = expectedBikeId,
                SharedVehiculeId = expectedSharedVehiculeId,
                Date = expectedDate,
                RentalStartTime = expectedStartTime,
                RentalEndTime = expectedEndTime
            };

            // Act & Assert
            Assert.Equal(expectedId, dto.Id);
            Assert.Equal(expectedUserId, dto.UserId);
            Assert.Equal(expectedShuttleId, dto.ShuttleId);
            Assert.Equal(expectedBikeId, dto.BikeId);
            Assert.Equal(expectedSharedVehiculeId, dto.SharedVehiculeId);
            Assert.Equal(expectedDate, dto.Date);
            Assert.Equal(expectedStartTime, dto.RentalStartTime);
            Assert.Equal(expectedEndTime, dto.RentalEndTime);
        }

        [Fact]
        public void PaymentTransactionDto_PropertiesSetAndGet_CorrectValues()
        {
            // Arrange
            var expectedId = 1;
            var expectedUserId = 2;
            var expectedAmount = 100.50m;
            var expectedDate = DateTime.Now;
            var expectedPaymentMethod = "CreditCard";
            var expectedIsRefund = true;

            var dto = new PaymentTransactionDto
            {
                Id = expectedId,
                UserId = expectedUserId,
                Amount = expectedAmount,
                Date = expectedDate,
                PaymentMethod = expectedPaymentMethod,
                IsRefund = expectedIsRefund
            };

            // Act & Assert
            Assert.Equal(expectedId, dto.Id);
            Assert.Equal(expectedUserId, dto.UserId);
            Assert.Equal(expectedAmount, dto.Amount);
            Assert.Equal(expectedDate, dto.Date);
            Assert.Equal(expectedPaymentMethod, dto.PaymentMethod);
            Assert.Equal(expectedIsRefund, dto.IsRefund);
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
            var apiErrorResponse = new ApiErrorResponse { Message = message };
            var userTransportationResponse = new UserTransportationResponseDto { Message = message };
            var userPaymentResponse = new UserPaymentResponseDto { Message = message };

            // Assert
            Assert.Equal(message, rentBikeResponse.Message);
            Assert.Equal(message, endBikeResponse.Message);
            Assert.Equal(message, boardShuttleResponse.Message);
            Assert.Equal(message, sharedVehicleResponse.Message);
            Assert.Equal(message, apiErrorResponse.Message);
            Assert.Equal(message, userTransportationResponse.Message);
            Assert.Equal(message, userPaymentResponse.Message);
        }

        [Fact]
        public void TransactionsLists_WhenEmpty_ReturnsEmptyLists()
        {
            // Arrange
            var transportationResponse = new UserTransportationResponseDto
            {
                Message = "Test",
                Transactions = new List<TransportationTransactionDto>()
            };
            var paymentResponse = new UserPaymentResponseDto
            {
                Message = "Test",
                Transactions = new List<PaymentTransactionDto>()
            };

            // Act & Assert
            Assert.Empty(transportationResponse.Transactions);
            Assert.Empty(paymentResponse.Transactions);
        }
    }
}
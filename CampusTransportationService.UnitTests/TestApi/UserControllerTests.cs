using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Web_Api.Controllers;
using BLL.Services;
using System;
using System.Collections.Generic;
using DAL.DB.Model;

namespace Web_Api.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<ITransportationService> _mockTransportationService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockTransportationService = new Mock<ITransportationService>();
            _controller = new UserController(_mockTransportationService.Object);
        }

        private static Dictionary<string, object> ConvertAnonymousObjectToDictionary(object obj)
        {
            return obj.GetType()
                .GetProperties()
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj));
        }

        [Fact]
        public void GetUserTransportationTransactions_Success_ReturnsOkResult()
        {
            // Arrange
            int userId = 1;
            var transactions = new List<TransportationTransaction>
            {
                new TransportationTransaction { Id = 1, UserId = userId }
            };

            _mockTransportationService
                .Setup(s => s.GetUserTransportationTransactions(userId))
                .Returns(transactions);

            // Act
            var result = _controller.GetUserTransportationTransactions(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(okResult.Value));
            Assert.Equal("Transactions récupérées avec succès", responseDict["Message"]);
            Assert.NotNull(responseDict["Transactions"]);
        }

        [Fact]
        public void GetUserTransportationTransactions_NoTransactions_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            var transactions = new List<TransportationTransaction>();

            _mockTransportationService
                .Setup(s => s.GetUserTransportationTransactions(userId))
                .Returns(transactions);

            // Act
            var result = _controller.GetUserTransportationTransactions(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(notFoundResult.Value));
            Assert.Equal("Aucune transaction trouvée pour cet utilisateur.", responseDict["Message"]);
        }

        [Fact]
        public void GetUserTransportationTransactions_InvalidOperationException_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            var expectedMessage = "Message d'erreur spécifique";
            _mockTransportationService
                .Setup(s => s.GetUserTransportationTransactions(userId))
                .Throws(new InvalidOperationException(expectedMessage));

            // Act
            var result = _controller.GetUserTransportationTransactions(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(badRequestResult.Value));
            Assert.Equal(expectedMessage, responseDict["Message"]);
        }

        [Fact]
        public void GetUserTransportationTransactions_Exception_Returns500()
        {
            // Arrange
            int userId = 1;
            var expectedError = "Test error";
            _mockTransportationService
                .Setup(s => s.GetUserTransportationTransactions(userId))
                .Throws(new Exception(expectedError));

            // Act
            var result = _controller.GetUserTransportationTransactions(userId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(statusCodeResult.Value));
            Assert.Equal("Une erreur interne s'est produite lors de la récupération des transactions.", responseDict["Message"]);
            Assert.Equal(expectedError, responseDict["Error"]);
        }

        [Fact]
        public void GetUserPaymentTransactions_Success_ReturnsOkResult()
        {
            // Arrange
            int userId = 1;
            var transactions = new List<PaymentTransaction>
            {
                new PaymentTransaction { Id = 1, UserId = userId }
            };

            _mockTransportationService
                .Setup(s => s.GetUserPaymentTransactions(userId))
                .Returns(transactions);

            // Act
            var result = _controller.GetUserPaymentTransactions(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(okResult.Value));
            Assert.Equal("Transactions de paiement récupérées avec succès", responseDict["Message"]);
            Assert.NotNull(responseDict["Transactions"]);
        }

        [Fact]
        public void GetUserPaymentTransactions_NoTransactions_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            var transactions = new List<PaymentTransaction>();

            _mockTransportationService
                .Setup(s => s.GetUserPaymentTransactions(userId))
                .Returns(transactions);

            // Act
            var result = _controller.GetUserPaymentTransactions(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(notFoundResult.Value));
            Assert.Equal("Aucune transaction de paiement trouvée pour cet utilisateur.", responseDict["Message"]);
        }

        [Fact]
        public void GetUserPaymentTransactions_InvalidOperationException_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            var expectedMessage = "Message d'erreur spécifique";
            _mockTransportationService
                .Setup(s => s.GetUserPaymentTransactions(userId))
                .Throws(new InvalidOperationException(expectedMessage));

            // Act
            var result = _controller.GetUserPaymentTransactions(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(badRequestResult.Value));
            Assert.Equal(expectedMessage, responseDict["Message"]);
        }

        [Fact]
        public void GetUserPaymentTransactions_Exception_Returns500()
        {
            // Arrange
            int userId = 1;
            var expectedError = "Test error";
            _mockTransportationService
                .Setup(s => s.GetUserPaymentTransactions(userId))
                .Throws(new Exception(expectedError));

            // Act
            var result = _controller.GetUserPaymentTransactions(userId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var responseDict = Assert.IsType<Dictionary<string, object>>(
                ConvertAnonymousObjectToDictionary(statusCodeResult.Value));
            Assert.Equal("Une erreur interne s'est produite lors de la récupération des transactions de paiement.", responseDict["Message"]);
            Assert.Equal(expectedError, responseDict["Error"]);
        }
    }
}
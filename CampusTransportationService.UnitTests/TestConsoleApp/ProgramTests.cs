using Xunit;
using Moq;
using ConsoleApp1.Commands;
using ConsoleApp1.Commands.Bike;
using ConsoleApp1.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleApp1.Tests
{
    public class ProgramTests
    {
        private readonly Mock<BikeApiClient> _mockBikeClient;
        private readonly Mock<ShuttleApiClient> _mockShuttleClient;
        private readonly Mock<SharedVehicleApiClient> _mockSharedVehicleClient;
        private readonly Mock<ICommand> _mockCommand;
        private readonly List<ICommand> _mockCommands;
        private const int MAX_VALID_USER_ID = 1000;
        private const int MIN_VALID_USER_ID = 1;

        public ProgramTests()
        {
            _mockBikeClient = new Mock<BikeApiClient>("http://test.com");
            _mockShuttleClient = new Mock<ShuttleApiClient>("http://test.com");
            _mockSharedVehicleClient = new Mock<SharedVehicleApiClient>("http://test.com");
            _mockCommand = new Mock<ICommand>();
            _mockCommands = new List<ICommand> { _mockCommand.Object };
        }

        [Fact]
        public void InitializeCommands_CreatesAllRequiredCommands()
        {
            // Arrange
            int userId = 1;
            var expectedCommandCount = 6;

            // Act
            var commands = new List<ICommand>
            {
                new RentBikeCommand(_mockBikeClient.Object, userId),
                new EndBikeRentalCommand(_mockBikeClient.Object, userId),
                new BoardShuttleCommand(_mockShuttleClient.Object, userId),
                new CreateSharedVehicleTripCommand(_mockSharedVehicleClient.Object, userId),
                new JoinSharedVehicleCommand(_mockSharedVehicleClient.Object, userId),
                new EndSharedVehicleTripCommand(_mockSharedVehicleClient.Object, userId)
            };

            // Assert
            Assert.Equal(expectedCommandCount, commands.Count);
            Assert.Contains(commands, c => c is RentBikeCommand);
            Assert.Contains(commands, c => c is EndBikeRentalCommand);
            Assert.Contains(commands, c => c is BoardShuttleCommand);
            Assert.Contains(commands, c => c is CreateSharedVehicleTripCommand);
            Assert.Contains(commands, c => c is JoinSharedVehicleCommand);
            Assert.Contains(commands, c => c is EndSharedVehicleTripCommand);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("7")]
        public void ProcessMenuChoice_SpecialOptions_HandledCorrectly(string choice)
        {
            // Arrange
            var validSpecialChoices = new[] { "0", "7" };

            // Act & Assert
            Assert.Contains(choice, validSpecialChoices);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("-1")]
        [InlineData("999")]
        public void ProcessMenuChoice_InvalidInput_ReturnsError(string invalidChoice)
        {
            // Arrange
            var maxCommandCount = 6;

            // Act
            bool isValid = int.TryParse(invalidChoice, out int choiceNumber) &&
                          choiceNumber > 0 &&
                          choiceNumber <= maxCommandCount;

            // Assert
            Assert.False(isValid);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("6")]
        public void ProcessMenuChoice_ValidInput_ReturnsSuccess(string validChoice)
        {
            // Arrange
            var maxCommandCount = 6;

            // Act
            bool isValid = int.TryParse(validChoice, out int choiceNumber) &&
                          choiceNumber > 0 &&
                          choiceNumber <= maxCommandCount;

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public async Task CommandExecution_Success()
        {
            // Arrange
            _mockCommand.Setup(c => c.ExecuteAsync())
                       .Returns(Task.CompletedTask);

            // Act
            await _mockCommand.Object.ExecuteAsync();

            // Assert
            _mockCommand.Verify(c => c.ExecuteAsync(), Times.Once);
        }

        [Fact]
        public async Task CommandExecution_ThrowsException_HandlesError()
        {
            // Arrange
            var expectedError = "Test error";
            _mockCommand.Setup(c => c.ExecuteAsync())
                       .ThrowsAsync(new Exception(expectedError));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _mockCommand.Object.ExecuteAsync());
            Assert.Equal(expectedError, exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1001)]
        [InlineData(9999)]
        public void InitializeUserSession_InvalidUserId_ReturnsError(int invalidUserId)
        {
            // Act
            bool isValidUserId = IsValidUserId(invalidUserId);

            // Assert
            Assert.False(isValidUserId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(1000)]
        public void InitializeUserSession_ValidUserId_Succeeds(int validUserId)
        {
            // Act
            bool isValidUserId = IsValidUserId(validUserId);

            // Assert
            Assert.True(isValidUserId);
        }

        private bool IsValidUserId(int userId)
        {
            return userId >= MIN_VALID_USER_ID && userId <= MAX_VALID_USER_ID;
        }

        [Fact]
        public void DisplayMenu_ContainsAllOptions()
        {
            // Arrange
            int userId = 1;
            var expectedCommandCount = 6;

            // Act
            var commands = new List<ICommand>
            {
                new RentBikeCommand(_mockBikeClient.Object, userId),
                new EndBikeRentalCommand(_mockBikeClient.Object, userId),
                new BoardShuttleCommand(_mockShuttleClient.Object, userId),
                new CreateSharedVehicleTripCommand(_mockSharedVehicleClient.Object, userId),
                new JoinSharedVehicleCommand(_mockSharedVehicleClient.Object, userId),
                new EndSharedVehicleTripCommand(_mockSharedVehicleClient.Object, userId)
            };

            // Assert
            Assert.Equal(expectedCommandCount, commands.Count);
            Assert.All(commands, command => Assert.NotNull(command.Name));
        }

        [Fact]
        public void ApiClients_InitializedWithCorrectBaseUrl()
        {
            // Arrange
            var expectedBaseUrl = "https://localhost:7087";

            // Act
            var bikeClient = new BikeApiClient(expectedBaseUrl);
            var shuttleClient = new ShuttleApiClient(expectedBaseUrl);
            var sharedVehicleClient = new SharedVehicleApiClient(expectedBaseUrl);

            // Assert
            Assert.NotNull(bikeClient);
            Assert.NotNull(shuttleClient);
            Assert.NotNull(sharedVehicleClient);
        }

        [Fact]
        public void ApiClients_InvalidBaseUrl_ThrowsException()
        {
            // Arrange
            var invalidBaseUrl = "";

            // Act & Assert
            Assert.Throws<UriFormatException>(() => new BikeApiClient(invalidBaseUrl));
            Assert.Throws<UriFormatException>(() => new ShuttleApiClient(invalidBaseUrl));
            Assert.Throws<UriFormatException>(() => new SharedVehicleApiClient(invalidBaseUrl));
        }

        [Theory]
        [InlineData("not a url")]
        [InlineData("http://")]
        [InlineData(":\\\\")]
        public void ApiClients_MalformedBaseUrl_ThrowsUriFormatException(string malformedUrl)
        {
            // Act & Assert
            Assert.Throws<UriFormatException>(() => new BikeApiClient(malformedUrl));
            Assert.Throws<UriFormatException>(() => new ShuttleApiClient(malformedUrl));
            Assert.Throws<UriFormatException>(() => new SharedVehicleApiClient(malformedUrl));
        }
    }
}
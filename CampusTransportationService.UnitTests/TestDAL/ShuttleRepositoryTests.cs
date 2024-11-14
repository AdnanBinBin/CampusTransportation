
using Microsoft.EntityFrameworkCore;
using Moq;
using DAL.DB;
using DAL.DB.Model;
using DAL.Repositories;
using static CardRepositoryTests;

namespace CampusTransportationService.UnitTests.TestDAL
{
    public class ShuttleRepositoryTests
    {
        private readonly Mock<DbSet<Shuttle>> _mockSet;
        private readonly Context _context;
        private readonly ShuttleRepository _repository;
        private readonly List<Shuttle> _data;

        public ShuttleRepositoryTests()
        {
            // Préparer les données de test
            _data = new List<Shuttle>
            {
                new Shuttle
                {
                    Id = "SHUTTLE1",
                    Name = "Shuttle 1",
                    IsAvailable = true,
                    Price = 30
                },
                new Shuttle
                {
                    Id = "SHUTTLE2",
                    Name = "Shuttle 2",
                    IsAvailable = false,
                    Price = 35
                }
            };

            var queryableData = _data.AsQueryable();

            _mockSet = new Mock<DbSet<Shuttle>>();

            // Setup pour IQueryable
            _mockSet.As<IQueryable<Shuttle>>()
                   .Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider<Shuttle>(queryableData.Provider));

            _mockSet.As<IQueryable<Shuttle>>()
                   .Setup(m => m.Expression)
                   .Returns(queryableData.Expression);

            _mockSet.As<IQueryable<Shuttle>>()
                   .Setup(m => m.ElementType)
                   .Returns(queryableData.ElementType);

            _mockSet.As<IQueryable<Shuttle>>()
                   .Setup(m => m.GetEnumerator())
                   .Returns(() => queryableData.GetEnumerator());

            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new Context(options);

            var property = _context.GetType().GetProperty("Shuttles");
            property.SetValue(_context, _mockSet.Object);

            _repository = new ShuttleRepository(_context);
        }

        #region Helper Classes
        // Mêmes classes helper que précédemment (TestAsyncQueryProvider, etc.)
        #endregion

        [Fact]
        public void GetById_ExistingShuttleId_ReturnsShuttle()
        {
            // Arrange
            var expectedShuttle = _data[0];
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                   .Returns(expectedShuttle);

            // Act
            var result = _repository.GetById("SHUTTLE1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedShuttle.Id, result.Id);
            Assert.Equal(expectedShuttle.Name, result.Name);
        }

        [Fact]
        public void GetByName_ExistingShuttleName_ReturnsShuttle()

        {
            // Act
            var result = _repository.GetByName("Shuttle 1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SHUTTLE1", result.Id);
            Assert.Equal("Shuttle 1", result.Name);
        }

        [Fact]
        public void Insert_ValidShuttle_AddedToDatabase()
        {
            // Arrange
            var shuttle = new Shuttle
            {
                Id = "SHUTTLE3",
                Name = "Shuttle 3",
                IsAvailable = true,
                Price = 40
            };

            // Act
            _repository.Insert(shuttle);

            // Assert
            _mockSet.Verify(m => m.Add(It.Is<Shuttle>(s =>
                s.Id == shuttle.Id &&
                s.Name == shuttle.Name &&
                s.Price == shuttle.Price)),
                Times.Once());
        }

        [Fact]
        public void Update_ExistingShuttle_UpdatesDatabase()
        {
            // Arrange
            var shuttle = _data[0];
            shuttle.IsAvailable = false;
            shuttle.Price = 45;

            _mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                   .Returns(shuttle);

            // Act
            _repository.Update(shuttle);

            // Assert
            _mockSet.Verify(m => m.Find(It.Is<object[]>(o =>
                o[0].ToString() == shuttle.Id)), Times.Once());
        }

        [Fact]
        public void Delete_ExistingShuttle_RemovedFromDatabase()
        {
            // Arrange
            var shuttle = _data[0];

            // Act
            _repository.Delete(shuttle);

            // Assert
            _mockSet.Verify(m => m.Remove(It.Is<Shuttle>(s =>
                s.Id == shuttle.Id)), Times.Once());
        }

        [Fact]
        public void GetAll_DataExists_ReturnsAllShuttles()
        {
            // Act
            var result = _repository.GetAll().ToList();

            // Assert
            Assert.Equal(_data.Count, result.Count);
            Assert.Contains(result, s => s.Id == "SHUTTLE1");
            Assert.Contains(result, s => s.Id == "SHUTTLE2");
        }
    }
}
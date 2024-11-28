using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using DAL.DB;
using DAL.DB.Model;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace CampusTransportationService.UnitTests.TestDAL
{
    public class SharedVehiculeRepositoryTests
    {
        private readonly Mock<DbSet<SharedVehicule>> _mockSet;
        private readonly Context _context;
        private readonly SharedVehiculeRepository _repository;
        private readonly List<SharedVehicule> _data;

        public SharedVehiculeRepositoryTests()
        {
            // Préparer les données de test
            _data = new List<SharedVehicule>
            {
                new SharedVehicule
                {
                    Id = "VEHICLE1",
                    Name = "Vehicle 1",
                    IsAvailable = true,
                    Capacity = 4,
                    Price = 50,
                    DriverId = 1,
                    Users = new List<User>()
                },
                new SharedVehicule
                {
                    Id = "VEHICLE2",
                    Name = "Vehicle 2",
                    IsAvailable = false,
                    Capacity = 6,
                    Price = 75,
                    DriverId = 2,
                    Users = new List<User>()
                }
            };

            var queryableData = _data.AsQueryable();

            _mockSet = new Mock<DbSet<SharedVehicule>>();

            // Setup pour IQueryable
            _mockSet.As<IQueryable<SharedVehicule>>()
                   .Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider<SharedVehicule>(queryableData.Provider));

            _mockSet.As<IQueryable<SharedVehicule>>()
                   .Setup(m => m.Expression)
                   .Returns(queryableData.Expression);

            _mockSet.As<IQueryable<SharedVehicule>>()
                   .Setup(m => m.ElementType)
                   .Returns(queryableData.ElementType);

            _mockSet.As<IQueryable<SharedVehicule>>()
                   .Setup(m => m.GetEnumerator())
                   .Returns(() => queryableData.GetEnumerator());

            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new Context(options);

            var property = _context.GetType().GetProperty("SharedVehicules");
            property.SetValue(_context, _mockSet.Object);

            _repository = new SharedVehiculeRepository(_context);
        }


        [ExcludeFromCodeCoverage]
        #region Helper Classes
        internal class TestAsyncQueryProvider<TEntity> : IQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }
        }

        [ExcludeFromCodeCoverage]
        internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            { }

            public TestAsyncEnumerable(Expression expression)
                : base(expression)
            { }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }
        }

        [ExcludeFromCodeCoverage]

        internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_inner.MoveNext());
            }

            public T Current => _inner.Current;

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return new ValueTask();
            }
        }
        #endregion

        [Fact]
        public void GetById_ExistingVehicleId_ReturnsVehicle()
        {
            // Arrange
            var expectedVehicle = _data[0];
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                   .Returns(expectedVehicle);

            // Act
            var result = _repository.GetById("VEHICLE1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedVehicle.Id, result.Id);
            Assert.Equal(expectedVehicle.Name, result.Name);
        }

        [Fact]
        public void GetByName_ExistingVehicleName_ReturnsVehicle()
        {
            // Act
            var result = _repository.GetByName("Vehicle 1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("VEHICLE1", result.Id);
            Assert.Equal("Vehicle 1", result.Name);
        }

        [Fact]
        public void Insert_ValidVehicle_AddedToDatabase()
        {
            // Arrange
            var vehicle = new SharedVehicule
            {
                Id = "VEHICLE3",
                Name = "Vehicle 3",
                IsAvailable = true,
                Capacity = 5,
                Price = 60,
                Users = new List<User>()
            };

            // Act
            _repository.Insert(vehicle);

            // Assert
            _mockSet.Verify(m => m.Add(It.Is<SharedVehicule>(v =>
                v.Id == vehicle.Id &&
                v.Name == vehicle.Name &&
                v.Capacity == vehicle.Capacity)),
                Times.Once());
        }

        [Fact]
        public void Update_ExistingVehicle_UpdatesDatabase()
        {
            // Arrange
            var vehicle = _data[0];
            vehicle.IsAvailable = false;
            vehicle.Price = 65;

            _mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                   .Returns(vehicle);

            // Act
            _repository.Update(vehicle);

            // Assert
            _mockSet.Verify(m => m.Find(It.Is<object[]>(o =>
                o[0].ToString() == vehicle.Id)), Times.Once());
        }

        [Fact]
        public void Delete_ExistingVehicle_RemovedFromDatabase()
        {
            // Arrange
            var vehicle = _data[0];

            // Act
            _repository.Delete(vehicle);

            // Assert
            _mockSet.Verify(m => m.Remove(It.Is<SharedVehicule>(v =>
                v.Id == vehicle.Id)), Times.Once());
        }

        [Fact]
        public void GetAll_DataExists_ReturnsAllVehicles()
        {
            // Act
            var result = _repository.GetAll().ToList();

            // Assert
            Assert.Equal(_data.Count, result.Count);
            Assert.Contains(result, v => v.Id == "VEHICLE1");
            Assert.Contains(result, v => v.Id == "VEHICLE2");
        }
    }
}
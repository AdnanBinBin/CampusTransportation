using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using DAL.DB;
using DAL.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Repositories;
using System.Linq.Expressions;

public class BikeRepositoryTests
{
    private readonly Mock<DbSet<Bike>> _mockSet;
    private readonly Context _context;
    private readonly BikeRepository _repository;

    public BikeRepositoryTests()
    {
        var data = new List<Bike>
        {
            new Bike { Id = "BIKE1", Name = "TestBike", IsAvailable = true, Price = 10 },
            new Bike { Id = "BIKE2", Name = "OtherBike", IsAvailable = true, Price = 15 }
        };

        var queryableData = data.AsQueryable();

        _mockSet = new Mock<DbSet<Bike>>();

        // Setup pour IQueryable
        _mockSet.As<IQueryable<Bike>>()
               .Setup(m => m.Provider)
               .Returns(new TestAsyncQueryProvider<Bike>(queryableData.Provider));

        _mockSet.As<IQueryable<Bike>>()
               .Setup(m => m.Expression)
               .Returns(queryableData.Expression);

        _mockSet.As<IQueryable<Bike>>()
               .Setup(m => m.ElementType)
               .Returns(queryableData.ElementType);

        _mockSet.As<IQueryable<Bike>>()
               .Setup(m => m.GetEnumerator())
               .Returns(() => queryableData.GetEnumerator());

        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new Context(options);

        var property = _context.GetType().GetProperty("Bikes");
        property.SetValue(_context, _mockSet.Object);

        _repository = new BikeRepository(_context);
    }

    // Classe helper pour supporter les requêtes asynchrones
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

    [Fact]
    public void GetByName_ExistingBikeName_ReturnsBike()
    {
        // Act
        var result = _repository.GetByName("TestBike");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("BIKE1", result.Id);
        Assert.Equal("TestBike", result.Name);
    }

    [Fact]
    public void GetByName_NonexistentBikeName_ReturnsNull()
    {
        // Act
        var result = _repository.GetByName("NonExistentBike");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Insert_ValidBike_AddedToDatabase()
    {
        // Arrange
        var bike = new Bike
        {
            Id = "BIKE3",
            Name = "NewBike",
            IsAvailable = true,
            Price = 10
        };

        // Act
        _repository.Insert(bike);

        // Assert
        _mockSet.Verify(m => m.Add(It.Is<Bike>(b =>
            b.Id == bike.Id &&
            b.Name == bike.Name &&
            b.Price == bike.Price)),
            Times.Once());
    }

    [Fact]
    public void GetById_ExistingBikeId_ReturnsBike()
    {
        // Arrange
        var expectedBike = new Bike
        {
            Id = "BIKE1",
            Name = "TestBike",
            IsAvailable = true,
            Price = 10
        };
        _mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(expectedBike);

        // Act
        var result = _repository.GetById("BIKE1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedBike.Id, result.Id);
        Assert.Equal(expectedBike.Name, result.Name);
    }

    [Fact]
    public void Update_ExistingBike_UpdatesDatabase()
    {
        // Arrange
        var bike = new Bike
        {
            Id = "BIKE1",
            Name = "UpdatedBike",
            IsAvailable = false,
            Price = 15
        };

        var existingBike = new Bike { Id = "BIKE1" };
        _mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(existingBike);

        // Act
        _repository.Update(bike);

        // Assert
        _mockSet.Verify(m => m.Find(It.Is<object[]>(o => o[0].ToString() == bike.Id)), Times.Once());
    }

    [Fact]
    public void GetAll_DataExists_ReturnsAllBikes()
    {
        // Act
        var result = _repository.GetAll().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, b => b.Id == "BIKE1");
        Assert.Contains(result, b => b.Id == "BIKE2");
    }

    [Fact]
    public void Delete_ExistingBike_RemovesFromDatabase()
    {
        // Arrange
        var bike = new Bike
        {
            Id = "BIKE1",
            Name = "TestBike",
            IsAvailable = true,
            Price = 10
        };

        // Act
        _repository.Delete(bike);

        // Assert
        _mockSet.Verify(m => m.Remove(It.Is<Bike>(b =>
            b.Id == bike.Id &&
            b.Name == bike.Name &&
            b.IsAvailable == bike.IsAvailable &&
            b.Price == bike.Price)), Times.Once());
    }
}
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

public class CardRepositoryTests
{
    private readonly Mock<DbSet<Card>> _mockSet;
    private readonly Context _context;
    private readonly CardRepository _repository;

    public CardRepositoryTests()
    {
        var data = new List<Card>
        {
            new Card { Id = 1, PaymentMethod = PaymentMethod.CreditCard },
            new Card { Id = 2, PaymentMethod = PaymentMethod.DebitCard }
        };

        var queryableData = data.AsQueryable();

        _mockSet = new Mock<DbSet<Card>>();

        // Setup pour IQueryable
        _mockSet.As<IQueryable<Card>>()
               .Setup(m => m.Provider)
               .Returns(new TestAsyncQueryProvider<Card>(queryableData.Provider));

        _mockSet.As<IQueryable<Card>>()
               .Setup(m => m.Expression)
               .Returns(queryableData.Expression);

        _mockSet.As<IQueryable<Card>>()
               .Setup(m => m.ElementType)
               .Returns(queryableData.ElementType);

        _mockSet.As<IQueryable<Card>>()
               .Setup(m => m.GetEnumerator())
               .Returns(() => queryableData.GetEnumerator());

        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new Context(options);

        var property = _context.GetType().GetProperty("Cards");
        property.SetValue(_context, _mockSet.Object);

        _repository = new CardRepository(_context);
    }

    [ExcludeFromCodeCoverage]
    // Classes helper pour les tests asynchrones
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

    [Fact]
    public void GetById_ExistingCardId_ReturnsCard()
    {
        // Arrange
        var expectedCard = new Card { Id = 1, PaymentMethod = PaymentMethod.CreditCard };
        _mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                .Returns(expectedCard);

        // Act
        var result = _repository.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCard.Id, result.Id);
        Assert.Equal(expectedCard.PaymentMethod, result.PaymentMethod);
    }

    [Fact]
    public void GetAll_DataExists_ReturnsAllCards()
    {
        // Act
        var result = _repository.GetAll().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Id == 1);
        Assert.Contains(result, c => c.Id == 2);
    }

    [Fact]
    public void Insert_ValidCard_AddedToDatabase()
    {
        // Arrange
        var card = new Card
        {
            Id = 3,
            PaymentMethod = PaymentMethod.CreditCard
        };

        // Act
        _repository.Insert(card);

        // Assert
        _mockSet.Verify(m => m.Add(It.Is<Card>(c =>
            c.Id == card.Id &&
            c.PaymentMethod == card.PaymentMethod)),
            Times.Once());
    }

    [Fact]
    public void Update_ExistingCard_UpdatesDatabase()
    {
        // Arrange
        var card = new Card
        {
            Id = 1,
            PaymentMethod = PaymentMethod.CreditCard
        };

        // Act
        _repository.Update(card);

        // Assert
        _mockSet.Verify(m => m.Update(It.Is<Card>(c =>
            c.Id == card.Id &&
            c.PaymentMethod == card.PaymentMethod)),
            Times.Once());
    }

    [Fact]
    public void Delete_ExistingCard_RemovedFromDatabase()
    {
        // Arrange
        var card = new Card
        {
            Id = 1,
            PaymentMethod = PaymentMethod.CreditCard
        };

        // Act
        _repository.Delete(card);

        // Assert
        _mockSet.Verify(m => m.Remove(It.Is<Card>(c =>
            c.Id == card.Id &&
            c.PaymentMethod == card.PaymentMethod)),
            Times.Once());
    }
}
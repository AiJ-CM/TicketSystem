using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using System.Linq.Expressions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Infrastructure.Services;

namespace TicketSystem.Application.UnitTests.Infrastructure.Services;

public class DbUserPermissionProviderTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task GetPermissionCodesAsync_Should_Return_Distinct_Permissions_For_Active_User()
    {
        var readPermission = _fixture.Build<Permission>()
            .Without(permission => permission.Roles)
            .With(permission => permission.Id, 1)
            .With(permission => permission.PermissionCode, "ticket:read")
            .With(permission => permission.IsDeleted, false)
            .Create();
        var createPermission = _fixture.Build<Permission>()
            .Without(permission => permission.Roles)
            .With(permission => permission.Id, 2)
            .With(permission => permission.PermissionCode, "ticket:create")
            .With(permission => permission.IsDeleted, false)
            .Create();
        var roleOne = _fixture.Build<Role>()
            .Without(role => role.Users)
            .With(role => role.Id, 10)
            .With(role => role.Permissions, new List<Permission> { readPermission, createPermission })
            .Create();
        var roleTwo = _fixture.Build<Role>()
            .Without(role => role.Users)
            .With(role => role.Id, 11)
            .With(role => role.Permissions, new List<Permission> { readPermission })
            .Create();
        var user = _fixture.Build<User>()
            .Without(user => user.CreatedTickets)
            .Without(user => user.AssignedTickets)
            .Without(user => user.Audits)
            .Without(user => user.Comments)
            .With(user => user.Id, 42)
            .With(user => user.IsActive, true)
            .With(user => user.Roles, new List<Role> { roleOne, roleTwo })
            .Create();
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.Users).ReturnsDbSet([user]);
        var sut = new DbUserPermissionProvider(db.Object);

        var result = await sut.GetPermissionCodesAsync(42, CancellationToken.None);

        result.Should().BeEquivalentTo(["ticket:read", "ticket:create"]);
        result.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task GetPermissionCodesAsync_Should_Return_Empty_When_User_Has_No_Permissions()
    {
        var role = _fixture.Build<Role>()
            .Without(role => role.Users)
            .With(role => role.Id, 12)
            .With(role => role.Permissions, new List<Permission>())
            .Create();
        var user = _fixture.Build<User>()
            .Without(user => user.CreatedTickets)
            .Without(user => user.AssignedTickets)
            .Without(user => user.Audits)
            .Without(user => user.Comments)
            .With(user => user.Id, 43)
            .With(user => user.IsActive, true)
            .With(user => user.Roles, new List<Role> { role })
            .Create();
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.Users).ReturnsDbSet([user]);
        var sut = new DbUserPermissionProvider(db.Object);

        var result = await sut.GetPermissionCodesAsync(43, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPermissionCodesAsync_Should_Return_Empty_When_User_Is_Unknown_Or_Inactive()
    {
        var unknownDb = new Mock<ITicketingDbContext>();
        unknownDb.Setup(x => x.Users).ReturnsDbSet(Array.Empty<User>());

        var inactiveDb = new Mock<ITicketingDbContext>();
        inactiveDb.Setup(x => x.Users).ReturnsDbSet([
            _fixture.Build<User>()
                .Without(user => user.CreatedTickets)
                .Without(user => user.AssignedTickets)
                .Without(user => user.Audits)
                .Without(user => user.Comments)
                .With(user => user.Id, 44)
                .With(user => user.IsActive, false)
                .With(user => user.Roles, new List<Role>
                {
                    _fixture.Build<Role>()
                        .Without(role => role.Users)
                        .With(role => role.Id, 13)
                        .With(role => role.Permissions, new List<Permission>
                        {
                            _fixture.Build<Permission>()
                                .Without(permission => permission.Roles)
                                .With(permission => permission.Id, 3)
                                .With(permission => permission.PermissionCode, "ticket:delete")
                                .With(permission => permission.IsDeleted, false)
                                .Create()
                        })
                        .Create()
                })
                .Create()
        ]);

        var unknownResult = await new DbUserPermissionProvider(unknownDb.Object)
            .GetPermissionCodesAsync(999, CancellationToken.None);
        var inactiveResult = await new DbUserPermissionProvider(inactiveDb.Object)
            .GetPermissionCodesAsync(44, CancellationToken.None);

        unknownResult.Should().BeEmpty();
        inactiveResult.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPermissionCodesAsync_Should_Propagate_Query_Exception_And_Cancellation()
    {
        var users = new Mock<DbSet<User>>();
        var queryProvider = new ThrowingAsyncQueryProvider();
        users.As<IQueryable<User>>().Setup(x => x.Provider).Returns(queryProvider);
        users.As<IQueryable<User>>().Setup(x => x.Expression).Returns(Expression.Constant(users.Object));
        users.As<IQueryable<User>>().Setup(x => x.ElementType).Returns(typeof(User));
        users.As<IQueryable<User>>().Setup(x => x.GetEnumerator()).Returns(new List<User>().GetEnumerator());
        var db = new Mock<ITicketingDbContext>();
        db.Setup(x => x.Users).Returns(users.Object);
        var sut = new DbUserPermissionProvider(db.Object);

        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        var canceledAct = async () => await sut.GetPermissionCodesAsync(45, cancellation.Token);

        await canceledAct.Should().ThrowAsync<OperationCanceledException>();

        var exceptionAct = async () => await sut.GetPermissionCodesAsync(45, CancellationToken.None);

        await exceptionAct.Should().ThrowAsync<InvalidOperationException>().WithMessage("database unavailable");
    }

    private sealed class ThrowingAsyncQueryProvider : IAsyncQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
            => new ThrowingQueryable(expression, this);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new ThrowingQueryable<TElement>(expression, this);

        public object? Execute(Expression expression)
            => throw new InvalidOperationException("database unavailable");

        public TResult Execute<TResult>(Expression expression)
            => throw new InvalidOperationException("database unavailable");

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            throw new InvalidOperationException("database unavailable");
        }
    }

    private sealed class ThrowingQueryable(Expression expression, IQueryProvider provider) : IQueryable
    {
        public Type ElementType => typeof(User);
        public Expression Expression { get; } = expression;
        public IQueryProvider Provider { get; } = provider;

        public IEnumerator GetEnumerator() => Array.Empty<User>().Cast<User>().GetEnumerator();
    }

    private sealed class ThrowingQueryable<T>(Expression expression, IQueryProvider provider) : IQueryable<T>, IAsyncEnumerable<T>
    {
        public Type ElementType => typeof(T);
        public Expression Expression { get; } = expression;
        public IQueryProvider Provider { get; } = provider;

        public IEnumerator<T> GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new ThrowingAsyncEnumerator<T>(cancellationToken);
    }

    private sealed class ThrowingAsyncEnumerator<T>(CancellationToken cancellationToken) : IAsyncEnumerator<T>
    {
        public T Current => default!;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public ValueTask<bool> MoveNextAsync()
        {
            cancellationToken.ThrowIfCancellationRequested();
            throw new InvalidOperationException("database unavailable");
        }
    }
}
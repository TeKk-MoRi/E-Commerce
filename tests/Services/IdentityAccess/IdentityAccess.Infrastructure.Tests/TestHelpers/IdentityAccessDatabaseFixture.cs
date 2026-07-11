using IdentityAccess.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace IdentityAccess.Infrastructure.Tests.TestHelpers;

public sealed class IdentityAccessDatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("identityaccess_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public IdentityAccessDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityAccessDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        return new IdentityAccessDbContext(options);
    }

    public async Task ResetDatabaseAsync()
    {
        await using var dbContext = CreateDbContext();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }
}

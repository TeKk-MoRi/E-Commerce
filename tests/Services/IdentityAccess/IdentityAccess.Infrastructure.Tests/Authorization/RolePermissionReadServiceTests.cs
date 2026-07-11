using FluentAssertions;
using IdentityAccess.Domain.Authorization;
using IdentityAccess.Infrastructure.Authorization;
using IdentityAccess.Infrastructure.Tests.TestHelpers;

namespace IdentityAccess.Infrastructure.Tests.Authorization;

[Collection(IdentityAccessDatabaseCollection.Name)]
public class RolePermissionReadServiceTests(IdentityAccessDatabaseFixture fixture)
{
    [Fact]
    public async Task GetByRoleNameAsync_Should_Return_Permissions_For_Role_Ordered_By_Permission_Code()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();

        await using (var dbContext = fixture.CreateDbContext())
        {
            var secondPermission = Permission.Create("support.tickets.view", "Can view support tickets.");
            var firstPermission = Permission.Create("billing.invoices.view", "Can view invoices.");

            await dbContext.Permissions.AddRangeAsync(secondPermission, firstPermission);
            await dbContext.RolePermissions.AddRangeAsync(
                RolePermission.Create("support", secondPermission.Id),
                RolePermission.Create("support", firstPermission.Id));
            await dbContext.SaveChangesAsync();
        }

        await using var queryDbContext = fixture.CreateDbContext();
        var service = new RolePermissionReadService(queryDbContext);

        // Act
        var result = await service.GetByRoleNameAsync(" SUPPORT ");

        // Assert
        result.RoleName.Should().Be("support");
        result.Permissions
            .Select(permission => permission.Code)
            .Should()
            .Equal("billing.invoices.view", "support.tickets.view");
    }
}

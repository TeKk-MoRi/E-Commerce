using Catalog.Contracts.Authorization;
using ECommerce.BuildingBlocks.Application.Enums;
using FluentAssertions;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Infrastructure.Authorization;
using IdentityAccess.Infrastructure.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace IdentityAccess.Infrastructure.Tests.Authorization;

[Collection(IdentityAccessDatabaseCollection.Name)]
public class RolePermissionWriteServiceTests(IdentityAccessDatabaseFixture fixture)
{
    [Fact]
    public async Task ReplaceAsync_Should_Replace_Existing_Role_Permissions()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();

        await using var dbContext = fixture.CreateDbContext();
        var service = new RolePermissionWriteService(dbContext);

        // Act
        var result = await service.ReplaceAsync(
            ApplicationRoles.Customer,
            [ApplicationPermissions.IdentityUsersManage]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Permissions
            .Select(permission => permission.Code)
            .Should()
            .Equal(ApplicationPermissions.IdentityUsersManage);

        var storedPermissionCodes = await dbContext.RolePermissions
            .Where(rolePermission => rolePermission.RoleName == ApplicationRoles.Customer)
            .OrderBy(rolePermission => rolePermission.Permission.Code)
            .Select(rolePermission => rolePermission.Permission.Code)
            .ToListAsync();

        storedPermissionCodes.Should().Equal(ApplicationPermissions.IdentityUsersManage);
    }

    [Fact]
    public async Task ReplaceAsync_Should_Return_Validation_Failure_When_Permission_Codes_Are_Invalid()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();

        await using var dbContext = fixture.CreateDbContext();
        var service = new RolePermissionWriteService(dbContext);

        // Act
        var result = await service.ReplaceAsync(
            ApplicationRoles.Customer,
            ["identity.permissions.fly"]);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Message.Should().Contain("identity.permissions.fly");
    }

    [Fact]
    public async Task ReplaceAsync_Should_Persist_Valid_Role_Permission_Mappings()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();

        await using (var dbContext = fixture.CreateDbContext())
        {
            var service = new RolePermissionWriteService(dbContext);

            // Act
            var result = await service.ReplaceAsync(
                " CUSTOMER ",
                [
                    CatalogPermissions.ProductsView,
                    ApplicationPermissions.IdentityUsersManage
                ]);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        await using var assertionDbContext = fixture.CreateDbContext();

        var storedPermissionCodes = await assertionDbContext.RolePermissions
            .Where(rolePermission => rolePermission.RoleName == ApplicationRoles.Customer)
            .OrderBy(rolePermission => rolePermission.Permission.Code)
            .Select(rolePermission => rolePermission.Permission.Code)
            .ToListAsync();

        storedPermissionCodes.Should().Equal(
            CatalogPermissions.ProductsManage,
            ApplicationPermissions.IdentityUsersManage);
    }
}

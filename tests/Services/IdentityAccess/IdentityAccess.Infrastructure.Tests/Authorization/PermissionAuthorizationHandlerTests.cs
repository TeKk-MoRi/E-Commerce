using FluentAssertions;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Domain.Authorization;
using IdentityAccess.Infrastructure.Authorization;
using IdentityAccess.Infrastructure.Tests.TestHelpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Catalog.Contracts.Authorization;

namespace IdentityAccess.Infrastructure.Tests.Authorization;

[Collection(IdentityAccessDatabaseCollection.Name)]
public class PermissionAuthorizationHandlerTests(IdentityAccessDatabaseFixture fixture)
{
    [Fact]
    public async Task HandleAsync_Should_Succeed_When_AnyUserRoleHasRequiredPermission()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();

        await using var dbContext = fixture.CreateDbContext();
        var handler = new PermissionAuthorizationHandler(dbContext);
        var requirement = new PermissionRequirement(CatalogPermissions.ProductsView);
        var context = new AuthorizationHandlerContext(
            [requirement],
            CreateUser([ApplicationRoles.Customer]),
            resource: null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_Should_NotSucceed_When_UserRolesDoNotHaveRequiredPermission()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();

        await using var dbContext = fixture.CreateDbContext();
        var handler = new PermissionAuthorizationHandler(dbContext);
        var requirement = new PermissionRequirement(ApplicationPermissions.IdentityUsersManage);
        var context = new AuthorizationHandlerContext(
            [requirement],
            CreateUser([ApplicationRoles.Customer]),
            resource: null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_Should_NotSucceed_When_UserHasNoRoles()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();

        await using var dbContext = fixture.CreateDbContext();
        var handler = new PermissionAuthorizationHandler(dbContext);
        var requirement = new PermissionRequirement(CatalogPermissions.ProductsView);
        var context = new AuthorizationHandlerContext(
            [requirement],
            CreateUser([]),
            resource: null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    private static ClaimsPrincipal CreateUser(IReadOnlyCollection<string> roles)
    {
        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role));

        return new ClaimsPrincipal(
            new ClaimsIdentity(
                claims,
                authenticationType: "Test"));
    }
}

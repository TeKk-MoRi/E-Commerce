using FluentAssertions;
using IdentityAccess.Domain.Authorization;

namespace IdentityAccess.Domain.Tests.Authorization.Permission;

public class PermissionCreateTests
{
    [Fact]
    public void Create_Should_Throw_ArgumentException_When_Code_Is_Empty()
    {
        // Arrange
        var act = () => IdentityAccess.Domain.Authorization.Permission.Create("", "Can view products.");

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Should_Throw_ArgumentException_When_Description_Is_Empty()
    {
        // Arrange
        var act = () => IdentityAccess.Domain.Authorization.Permission.Create("catalog.products.view", "");

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Should_Trim_Code_And_Description()
    {
        // Act
        var permission = IdentityAccess.Domain.Authorization.Permission.Create(
            " catalog.products.view ",
            " Can view products. ");

        // Assert
        permission.Code.Should().Be("catalog.products.view");
        permission.Description.Should().Be("Can view products.");
    }
}

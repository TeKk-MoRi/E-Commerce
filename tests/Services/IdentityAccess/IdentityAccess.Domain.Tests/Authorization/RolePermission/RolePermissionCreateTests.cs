using FluentAssertions;

namespace IdentityAccess.Domain.Tests.Authorization.RolePermission;

public class RolePermissionCreateTests
{
    [Fact]
    public void Create_Should_Throw_ArgumentException_When_Role_Name_Is_Empty()
    {
        // Arrange
        var act = () => IdentityAccess.Domain.Authorization.RolePermission.Create("", Guid.NewGuid());

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Should_Throw_ArgumentException_When_Permission_Id_Is_Empty()
    {
        // Arrange
        var act = () => IdentityAccess.Domain.Authorization.RolePermission.Create("admin", Guid.Empty);

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Should_Trim_Role_Name()
    {
        // Act
        var rolePermission = IdentityAccess.Domain.Authorization.RolePermission.Create(
            " admin ",
            Guid.NewGuid());

        // Assert
        rolePermission.RoleName.Should().Be("admin");
    }
}

using FluentAssertions;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Application.Authorization.DTOs;
using IdentityAccess.Application.Common.Interfaces;
using IdentityAccess.Application.Usecases.Roles.GetRolePermissions;
using Moq;

namespace IdentityAccess.Application.Tests.Usecases.Roles.GetRolePermissions;

public class GetRolePermissionsQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_GetByRoleNameAsync()
    {
        // Arrange
        var readServiceMock = new Mock<IRolePermissionReadService>();
        var response = new RolePermissionsResponse(
            ApplicationRoles.Admin,
            [new PermissionResponse(ApplicationPermissions.IdentityRolesManage, "Can manage roles.")]);

        readServiceMock
            .Setup(x => x.GetByRoleNameAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var handler = new GetRolePermissionsQueryHandler(readServiceMock.Object);

        // Act
        var result = await handler.Handle(
            new GetRolePermissionsQuery("admin"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(response);

        readServiceMock.Verify(
            x => x.GetByRoleNameAsync(
                "admin",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

using Catalog.Contracts.Authorization;
using ECommerce.BuildingBlocks.Application;
using ECommerce.BuildingBlocks.Application.Enums;
using FluentAssertions;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Application.Authorization.DTOs;
using IdentityAccess.Application.Common.Interfaces;
using IdentityAccess.Application.Usecases.Roles.UpdateRolePermissions;
using Moq;

namespace IdentityAccess.Application.Tests.Usecases.Roles.UpdateRolePermissions;

public class UpdateRolePermissionsCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Validation_Failure_When_Role_Name_Is_Empty()
    {
        // Arrange
        var writeServiceMock = new Mock<IRolePermissionWriteService>();
        var handler = new UpdateRolePermissionsCommandHandler(writeServiceMock.Object);

        // Act
        var result = await handler.Handle(
            new UpdateRolePermissionsCommand("", [ApplicationPermissions.IdentityRolesManage]),
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().Be("Role.NameRequired");
    }

    [Fact]
    public async Task Handle_Should_Return_Validation_Failure_When_Permission_List_Is_Empty()
    {
        // Arrange
        var writeServiceMock = new Mock<IRolePermissionWriteService>();
        var handler = new UpdateRolePermissionsCommandHandler(writeServiceMock.Object);

        // Act
        var result = await handler.Handle(
            new UpdateRolePermissionsCommand(ApplicationRoles.Customer, []),
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().Be("Role.PermissionRequired");
    }

    [Fact]
    public async Task Handle_Should_Return_Validation_Failure_When_Admin_Roles_Manage_Permission_Is_Removed()
    {
        // Arrange
        var writeServiceMock = new Mock<IRolePermissionWriteService>();
        var handler = new UpdateRolePermissionsCommandHandler(writeServiceMock.Object);

        // Act
        var result = await handler.Handle(
            new UpdateRolePermissionsCommand(
                " ADMIN ",
                [CatalogPermissions.ProductsView]),
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().Be("Role.AdminPermissionRequired");
    }

    [Fact]
    public async Task Handle_Should_Call_ReplaceAsync_When_Input_Is_Valid()
    {
        // Arrange
        var writeServiceMock = new Mock<IRolePermissionWriteService>();
        var response = new RolePermissionsResponse(
            ApplicationRoles.Admin,
            [new PermissionResponse(ApplicationPermissions.IdentityRolesManage, "Can manage roles.")]);

        writeServiceMock
            .Setup(x => x.ReplaceAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RolePermissionsResponse>.Success(response));

        var handler = new UpdateRolePermissionsCommandHandler(writeServiceMock.Object);

        // Act
        var result = await handler.Handle(
            new UpdateRolePermissionsCommand(
                " ADMIN ",
                [ApplicationPermissions.IdentityRolesManage, " IDENTITY.ROLES.MANAGE "]),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        writeServiceMock.Verify(
            x => x.ReplaceAsync(
                ApplicationRoles.Admin,
                It.Is<IReadOnlyCollection<string>>(codes =>
                    codes.Count == 1 &&
                    codes.Contains(ApplicationPermissions.IdentityRolesManage)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

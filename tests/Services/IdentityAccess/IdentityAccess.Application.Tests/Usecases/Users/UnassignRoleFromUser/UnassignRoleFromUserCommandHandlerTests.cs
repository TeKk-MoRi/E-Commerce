using ECommerce.BuildingBlocks.Application;
using ECommerce.BuildingBlocks.Application.Enums;
using FluentAssertions;
using IdentityAccess.Application.Authorization;
using IdentityAccess.Application.Common.Interfaces;
using IdentityAccess.Application.Usecases.Users.UnassignRoleFromUser;
using Moq;

namespace IdentityAccess.Application.Tests.Usecases.Users.UnassignRoleFromUser;

public class UnassignRoleFromUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Validation_Failure_When_User_Id_Is_Empty()
    {
        // Arrange
        var keycloakServiceMock = new Mock<IKeycloakService>();
        var handler = new UnassignRoleFromUserCommandHandler(keycloakServiceMock.Object);

        // Act
        var result = await handler.Handle(
            new UnassignRoleFromUserCommand("", ApplicationRoles.Customer),
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().Be("User.IdRequired");
    }

    [Fact]
    public async Task Handle_Should_Return_Validation_Failure_When_Role_Name_Is_Empty()
    {
        // Arrange
        var keycloakServiceMock = new Mock<IKeycloakService>();
        var handler = new UnassignRoleFromUserCommandHandler(keycloakServiceMock.Object);

        // Act
        var result = await handler.Handle(
            new UnassignRoleFromUserCommand("user-1", ""),
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().Be("Role.NameRequired");
    }

    [Fact]
    public async Task Handle_Should_Return_Validation_Failure_When_Role_Is_Unsupported()
    {
        // Arrange
        var keycloakServiceMock = new Mock<IKeycloakService>();
        var handler = new UnassignRoleFromUserCommandHandler(keycloakServiceMock.Object);

        // Act
        var result = await handler.Handle(
            new UnassignRoleFromUserCommand("user-1", "manager"),
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().Be("Role.Invalid");
    }

    [Fact]
    public async Task Handle_Should_Return_Validation_Failure_When_Removing_Admin_Role()
    {
        // Arrange
        var keycloakServiceMock = new Mock<IKeycloakService>();
        var handler = new UnassignRoleFromUserCommandHandler(keycloakServiceMock.Object);

        // Act
        var result = await handler.Handle(
            new UnassignRoleFromUserCommand("user-1", " ADMIN "),
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().Be("Role.AdminRemovalBlocked");
    }

    [Fact]
    public async Task Handle_Should_Call_UnassignRealmRoleFromUserAsync_When_Input_Is_Valid()
    {
        // Arrange
        var keycloakServiceMock = new Mock<IKeycloakService>();

        keycloakServiceMock
            .Setup(x => x.UnassignRealmRoleFromUserAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var handler = new UnassignRoleFromUserCommandHandler(keycloakServiceMock.Object);

        // Act
        var result = await handler.Handle(
            new UnassignRoleFromUserCommand(" user-1 ", " CUSTOMER "),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        keycloakServiceMock.Verify(
            x => x.UnassignRealmRoleFromUserAsync(
                "user-1",
                ApplicationRoles.Customer,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

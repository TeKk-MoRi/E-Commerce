using ECommerce.BuildingBlocks.Application;
using ECommerce.BuildingBlocks.Application.Enums;
using FluentAssertions;
using IdentityAccess.Application.Common.Interfaces;
using IdentityAccess.Application.Usecases.Auth.ForgotPassword;
using Moq;

namespace IdentityAccess.Application.Tests.Usecases.Auth.ForgotPassword;

public class ForgotPasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Validation_Failure_When_Email_Is_Empty()
    {
        // Arrange
        var keycloakServiceMock = new Mock<IKeycloakService>();
        var handler = new ForgotPasswordCommandHandler(keycloakServiceMock.Object);

        // Act
        var result = await handler.Handle(new ForgotPasswordCommand(""), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().Be("Auth.EmailRequired");
    }

    [Fact]
    public async Task Handle_Should_Call_SendPasswordResetEmailAsync_When_Email_Is_Valid()
    {
        // Arrange
        var keycloakServiceMock = new Mock<IKeycloakService>();

        keycloakServiceMock
            .Setup(x => x.SendPasswordResetEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var handler = new ForgotPasswordCommandHandler(keycloakServiceMock.Object);

        // Act
        var result = await handler.Handle(
            new ForgotPasswordCommand(" user@example.com "),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        keycloakServiceMock.Verify(
            x => x.SendPasswordResetEmailAsync(
                "user@example.com",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

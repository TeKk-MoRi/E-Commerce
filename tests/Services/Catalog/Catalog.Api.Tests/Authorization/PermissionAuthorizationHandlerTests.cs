using System.Security.Claims;
using Catalog.Api.Authorization;
using Catalog.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging.Abstractions;

namespace Catalog.Api.Tests.Authorization;

public sealed class PermissionAuthorizationHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenUserHasPermission_ShouldSucceed()
    {
        var permissionClient =
            new StubPermissionClient(hasPermission: true);

        var handler = CreateHandler(permissionClient);
        var requirement =
            new PermissionRequirement("catalog.products.view");

        var context = CreateContext(
            requirement,
            authenticated: true);

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
        Assert.Equal(1, permissionClient.CallCount);
    }

    [Fact]
    public async Task HandleAsync_WhenUserDoesNotHavePermission_ShouldFail()
    {
        var permissionClient =
            new StubPermissionClient(hasPermission: false);

        var handler = CreateHandler(permissionClient);
        var requirement =
            new PermissionRequirement("catalog.products.manage");

        var context = CreateContext(
            requirement,
            authenticated: true);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
        Assert.Equal(1, permissionClient.CallCount);
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsUnauthenticated_ShouldNotCallIdentityAccess()
    {
        var permissionClient =
            new StubPermissionClient(hasPermission: true);

        var handler = CreateHandler(permissionClient);
        var requirement =
            new PermissionRequirement("catalog.products.view");

        var context = CreateContext(
            requirement,
            authenticated: false);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
        Assert.Equal(0, permissionClient.CallCount);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityAccessIsUnavailable_ShouldFailClosed()
    {
        var permissionClient =
            new StubPermissionClient(
                exception: new HttpRequestException(
                    "IdentityAccess unavailable."));

        var handler = CreateHandler(permissionClient);
        var requirement =
            new PermissionRequirement("catalog.products.view");

        var context = CreateContext(
            requirement,
            authenticated: true);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityAccessTimesOut_ShouldFailClosed()
    {
        var permissionClient =
            new StubPermissionClient(
                exception: new TaskCanceledException(
                    "IdentityAccess timed out."));

        var handler = CreateHandler(permissionClient);
        var requirement =
            new PermissionRequirement("catalog.products.view");

        var context = CreateContext(
            requirement,
            authenticated: true);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    private static PermissionAuthorizationHandler CreateHandler(
        IIdentityAccessPermissionClient permissionClient)
    {
        return new PermissionAuthorizationHandler(
            permissionClient,
            NullLogger<PermissionAuthorizationHandler>.Instance);
    }

    private static AuthorizationHandlerContext CreateContext(
        PermissionRequirement requirement,
        bool authenticated)
    {
        var identity = authenticated
            ? new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, "test-user")],
                authenticationType: "Test")
            : new ClaimsIdentity();

        return new AuthorizationHandlerContext(
            [requirement],
            new ClaimsPrincipal(identity),
            resource: null);
    }

    private sealed class StubPermissionClient(
        bool hasPermission = false,
        Exception? exception = null)
        : IIdentityAccessPermissionClient
    {
        public int CallCount { get; private set; }

        public Task<bool> HasPermissionAsync(
            string permission,
            CancellationToken cancellationToken)
        {
            CallCount++;

            if (exception is not null)
            {
                return Task.FromException<bool>(exception);
            }

            return Task.FromResult(hasPermission);
        }
    }
}
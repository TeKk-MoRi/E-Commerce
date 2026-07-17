using System.Net;
using System.Text;
using Catalog.Infrastructure.Authorization;

namespace Catalog.Infrastructure.Tests.Authorization;

public sealed class IdentityAccessPermissionClientTests
{
    [Fact]
    public async Task HasPermissionAsync_WhenPermissionIsAllowed_ShouldReturnTrue()
    {
        var handler = new StubHttpMessageHandler(
            HttpStatusCode.OK,
            """{"allowed":true}""");

        var client = CreateClient(handler);

        var result = await client.HasPermissionAsync(
            "catalog.products.view",
            CancellationToken.None);

        Assert.True(result);
        Assert.Equal(HttpMethod.Post, handler.RequestMethod);
        Assert.Equal(
            "/api/v1/authorization/check",
            handler.RequestUri?.AbsolutePath);
    }

    [Fact]
    public async Task HasPermissionAsync_WhenPermissionIsDenied_ShouldReturnFalse()
    {
        var handler = new StubHttpMessageHandler(
            HttpStatusCode.OK,
            """{"allowed":false}""");

        var client = CreateClient(handler);

        var result = await client.HasPermissionAsync(
            "catalog.products.manage",
            CancellationToken.None);

        Assert.False(result);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    public async Task HasPermissionAsync_WhenRequestIsRejected_ShouldReturnFalse(
        HttpStatusCode statusCode)
    {
        var handler = new StubHttpMessageHandler(statusCode);
        var client = CreateClient(handler);

        var result = await client.HasPermissionAsync(
            "catalog.products.view",
            CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task HasPermissionAsync_WhenIdentityAccessFails_ShouldThrowHttpRequestException()
    {
        var handler = new StubHttpMessageHandler(
            HttpStatusCode.InternalServerError);

        var client = CreateClient(handler);

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            client.HasPermissionAsync(
                "catalog.products.view",
                CancellationToken.None));
    }

    [Fact]
    public async Task HasPermissionAsync_WhenPermissionIsEmpty_ShouldThrowArgumentException()
    {
        var handler = new StubHttpMessageHandler(
            HttpStatusCode.OK,
            """{"allowed":true}""");

        var client = CreateClient(handler);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.HasPermissionAsync(
                string.Empty,
                CancellationToken.None));
    }

    private static IdentityAccessPermissionClient CreateClient(
        HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost:7271/")
        };

        return new IdentityAccessPermissionClient(httpClient);
    }

    private sealed class StubHttpMessageHandler(
        HttpStatusCode statusCode,
        string? content = null)
        : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }

        public HttpMethod? RequestMethod { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            RequestMethod = request.Method;

            var response = new HttpResponseMessage(statusCode);

            if (content is not null)
            {
                response.Content = new StringContent(
                    content,
                    Encoding.UTF8,
                    "application/json");
            }

            return Task.FromResult(response);
        }
    }
}
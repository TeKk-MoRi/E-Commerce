namespace Catalog.Api.Authorization;

public sealed class BearerTokenForwardingHandler(
    IHttpContextAccessor httpContextAccessor)
    : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var authorizationHeader =
            httpContextAccessor.HttpContext?
                .Request
                .Headers
                .Authorization
                .ToString();

        if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
            !request.Headers.Contains("Authorization"))
        {
            request.Headers.TryAddWithoutValidation(
                "Authorization",
                authorizationHeader);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
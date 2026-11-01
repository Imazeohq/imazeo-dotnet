using Imazeo.Http;
using Imazeo.Models;

namespace Imazeo.Resources;

public sealed class ContextResource
{
    private readonly HttpTransport _http;

    internal ContextResource(HttpTransport http) => _http = http;

    public async Task<ApiKeyContext> GetAsync(CancellationToken cancellationToken = default)
    {
        var url = _http.Path("api-key", "context");
        var res = await _http.RequestJsonAsync<ApiKeyContext>(
                HttpMethod.Get,
                url,
                timeoutMs: ImazeoVersion.LightTimeoutMs,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return res.Data;
    }
}

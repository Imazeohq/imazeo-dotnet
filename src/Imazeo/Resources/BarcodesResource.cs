using System.Text;
using System.Text.Json;
using Imazeo.Http;
using Imazeo.Models;

namespace Imazeo.Resources;

public sealed class BarcodesResource
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpTransport _http;
    private readonly Func<CancellationToken, Task<string>> _projectId;

    internal BarcodesResource(HttpTransport http, Func<CancellationToken, Task<string>> projectId)
    {
        _http = http;
        _projectId = projectId;
    }

    public async Task<BinaryResult> GenerateAsync(
        BarcodeGenerateInput input,
        CancellationToken cancellationToken = default)
    {
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var url = _http.Path("projects", projectId, "tools", "barcode");
        var json = JsonSerializer.Serialize(input, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _http.RequestBinaryAsync(
                HttpMethod.Post,
                url,
                content,
                headers: new Dictionary<string, string> { ["Accept"] = "*/*" },
                timeoutMs: ImazeoVersion.LightTimeoutMs,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}

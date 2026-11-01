using System.Text;
using System.Text.Json;
using Imazeo.Http;
using Imazeo.Models;

namespace Imazeo.Resources;

public sealed class TransformsResource
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpTransport _http;
    private readonly Func<CancellationToken, Task<string>> _projectId;

    internal TransformsResource(HttpTransport http, Func<CancellationToken, Task<string>> projectId)
    {
        _http = http;
        _projectId = projectId;
    }

    public async Task<BinaryResult> CreateAsync(
        TransformRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.ImageId) && string.IsNullOrEmpty(request.Url))
            throw new ArgumentException("Exactly one of imageId or url is required");
        if (!string.IsNullOrEmpty(request.ImageId) && !string.IsNullOrEmpty(request.Url))
            throw new ArgumentException("Provide only one of imageId or url");

        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var url = _http.Path("projects", projectId, "transforms");
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _http.RequestBinaryAsync(
                HttpMethod.Post,
                url,
                content,
                headers: new Dictionary<string, string> { ["Accept"] = "*/*" },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<PagedResult<TransformationRecord>> ListAsync(
        PageParams? parameters = null,
        CancellationToken cancellationToken = default)
    {
        parameters ??= new PageParams();
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var parts = new List<string>();
        if (parameters.Page is not null)
            parts.Add($"page={Uri.EscapeDataString(parameters.Page.Value.ToString())}");
        if (parameters.Limit is not null)
            parts.Add($"limit={Uri.EscapeDataString(parameters.Limit.Value.ToString())}");
        var qs = parts.Count == 0 ? "" : "?" + string.Join("&", parts);
        var url = _http.Path("projects", projectId, "transforms") + qs;
        var res = await _http.RequestJsonAsync<List<TransformationRecord>>(
                HttpMethod.Get,
                url,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return new PagedResult<TransformationRecord>
        {
            Items = res.Data,
            Page = res.Meta.Page ?? parameters.Page ?? 1,
            Limit = res.Meta.Limit ?? parameters.Limit ?? 20,
            Total = res.Meta.Total ?? res.Data.Count,
            Pages = res.Meta.Pages ?? 1,
            RequestId = res.Meta.RequestId,
        };
    }

    public async Task<TransformationRecord> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var url = _http.Path("projects", projectId, "transforms", id);
        var res = await _http.RequestJsonAsync<TransformationRecord>(
                HttpMethod.Get,
                url,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return res.Data;
    }

    public async Task<BinaryResult> DownloadAsync(
        string id,
        string? ifNoneMatch = null,
        CancellationToken cancellationToken = default)
    {
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var url = _http.Path("projects", projectId, "transforms", id, "content");
        Dictionary<string, string>? headers = null;
        if (!string.IsNullOrEmpty(ifNoneMatch))
            headers = new Dictionary<string, string> { ["If-None-Match"] = ifNoneMatch };
        return await _http.RequestBinaryAsync(
                HttpMethod.Get,
                url,
                headers: headers,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<MessageResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var url = _http.Path("projects", projectId, "transforms", id);
        var res = await _http.RequestJsonAsync<MessageResult>(
                HttpMethod.Delete,
                url,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return res.Data;
    }
}

using Imazeo.Http;
using Imazeo.Models;

namespace Imazeo.Resources;

public sealed class ImagesResource
{
    private readonly HttpTransport _http;
    private readonly Func<CancellationToken, Task<string>> _projectId;

    internal ImagesResource(HttpTransport http, Func<CancellationToken, Task<string>> projectId)
    {
        _http = http;
        _projectId = projectId;
    }

    public Task<ImageRecord> UploadAsync(
        byte[] data,
        string? filename = null,
        string? contentType = null,
        CancellationToken cancellationToken = default)
        => UploadAsync(new UploadSource { Data = data, Filename = filename, ContentType = contentType }, cancellationToken);

    public async Task<ImageRecord> UploadAsync(
        UploadSource source,
        CancellationToken cancellationToken = default)
    {
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var prepared = MultipartHelper.PrepareUpload(source);
        using var form = new MultipartFormDataContent();
        MultipartHelper.AppendFile(form, "file", prepared);
        var url = _http.Path("projects", projectId, "images");
        var res = await _http.RequestMultipartJsonAsync<ImageRecord>(
                HttpMethod.Post,
                url,
                form,
                uploadLength: prepared.ByteLength,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return res.Data;
    }

    public async Task<Page<ImageRecord>> ListAsync(
        PageParams? parameters = null,
        CancellationToken cancellationToken = default)
    {
        parameters ??= new PageParams();
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var qs = BuildQuery(parameters);
        var url = _http.Path("projects", projectId, "images") + qs;
        var res = await _http.RequestJsonAsync<List<ImageRecord>>(
                HttpMethod.Get,
                url,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return new Page<ImageRecord>
        {
            Items = res.Data,
            Page = res.Meta.Page ?? parameters.Page ?? 1,
            Limit = res.Meta.Limit ?? parameters.Limit ?? 20,
            Total = res.Meta.Total ?? res.Data.Count,
            Pages = res.Meta.Pages ?? 1,
            RequestId = res.Meta.RequestId,
        };
    }

    public async Task<ImageRecord> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var url = _http.Path("projects", projectId, "images", id);
        var res = await _http.RequestJsonAsync<ImageRecord>(
                HttpMethod.Get,
                url,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return res.Data;
    }

    public async Task<MessageResult> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var url = _http.Path("projects", projectId, "images", id);
        var res = await _http.RequestJsonAsync<MessageResult>(
                HttpMethod.Delete,
                url,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return res.Data;
    }

    public async Task<BinaryResult> DownloadAsync(string id, CancellationToken cancellationToken = default)
    {
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var url = _http.Path("projects", projectId, "images", id, "content");
        return await _http.RequestBinaryAsync(HttpMethod.Get, url, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    private static string BuildQuery(PageParams parameters)
    {
        var parts = new List<string>();
        if (parameters.Page is not null)
            parts.Add($"page={Uri.EscapeDataString(parameters.Page.Value.ToString())}");
        if (parameters.Limit is not null)
            parts.Add($"limit={Uri.EscapeDataString(parameters.Limit.Value.ToString())}");
        if (!string.IsNullOrEmpty(parameters.Q))
            parts.Add($"q={Uri.EscapeDataString(parameters.Q)}");
        return parts.Count == 0 ? "" : "?" + string.Join("&", parts);
    }
}

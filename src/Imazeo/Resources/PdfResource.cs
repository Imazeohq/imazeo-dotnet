using System.Text.Json;
using Imazeo.Http;
using Imazeo.Models;

namespace Imazeo.Resources;

public sealed class PdfResource
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpTransport _http;
    private readonly Func<CancellationToken, Task<string>> _projectId;

    internal PdfResource(HttpTransport http, Func<CancellationToken, Task<string>> projectId)
    {
        _http = http;
        _projectId = projectId;
    }

    public Task<PdfInfo> InfoAsync(
        byte[] data,
        string? filename = null,
        CancellationToken cancellationToken = default)
        => InfoAsync(new UploadSource { Data = data, Filename = filename }, cancellationToken);

    public async Task<PdfInfo> InfoAsync(
        UploadSource source,
        CancellationToken cancellationToken = default)
    {
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var prepared = MultipartHelper.PrepareUpload(source, "document.pdf", "application/pdf");
        using var form = new MultipartFormDataContent();
        MultipartHelper.AppendFile(form, "file", prepared);
        var url = _http.Path("projects", projectId, "tools", "pdf", "info");
        var res = await _http.RequestMultipartJsonAsync<PdfInfo>(
                HttpMethod.Post,
                url,
                form,
                timeoutMs: ImazeoVersion.PdfTimeoutMs,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return res.Data;
    }

    public Task<object> RenderAsync(
        byte[] data,
        PdfRenderOptions? options = null,
        CancellationToken cancellationToken = default)
        => RenderAsync(new UploadSource { Data = data, Filename = options?.Filename }, options, cancellationToken);

    /// <summary>
    /// Renders PDF pages. Single-page responses return <see cref="BinaryResult"/>;
    /// multi-page JSON responses return <see cref="PdfMultiPageRenderResult"/>.
    /// </summary>
    public async Task<object> RenderAsync(
        UploadSource source,
        PdfRenderOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new PdfRenderOptions();
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var prepared = MultipartHelper.PrepareUpload(
            source,
            options.Filename ?? "document.pdf",
            "application/pdf");
        using var form = new MultipartFormDataContent();
        MultipartHelper.AppendFile(form, "file", prepared);
        var url = _http.Path("projects", projectId, "tools", "pdf", "render") + BuildRenderQuery(options);
        var result = await _http.RequestMultipartBinaryAsync(
                HttpMethod.Post,
                url,
                form,
                timeoutMs: ImazeoVersion.PdfTimeoutMs,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (result is BinaryResult binary)
            return binary;

        if (result is JsonSuccess<object> json)
        {
            var pages = ExtractPages(json.Data);
            return new PdfMultiPageRenderResult
            {
                Pages = pages,
                RequestId = json.Meta.RequestId,
            };
        }

        throw new InvalidOperationException("Unexpected PDF render response");
    }

    public async Task<BinaryResult> MergeAsync(
        IEnumerable<UploadSource> sources,
        CancellationToken cancellationToken = default)
    {
        var list = sources.ToList();
        if (list.Count == 0)
            throw new ArgumentException("At least one PDF is required");

        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        using var form = new MultipartFormDataContent();
        for (var i = 0; i < list.Count; i++)
        {
            var prepared = MultipartHelper.PrepareUpload(
                list[i],
                $"file-{i + 1}.pdf",
                "application/pdf");
            MultipartHelper.AppendFile(form, "file", prepared);
        }

        var url = _http.Path("projects", projectId, "tools", "pdf", "merge");
        var result = await _http.RequestMultipartBinaryAsync(
                HttpMethod.Post,
                url,
                form,
                timeoutMs: ImazeoVersion.PdfTimeoutMs,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (result is BinaryResult binary)
            return binary;
        throw new InvalidOperationException("Expected PDF binary response from merge");
    }

    public Task<BinaryResult> ExtractAsync(
        byte[] data,
        PdfExtractOptions options,
        CancellationToken cancellationToken = default)
        => ExtractAsync(new UploadSource { Data = data, Filename = options.Filename }, options, cancellationToken);

    public async Task<BinaryResult> ExtractAsync(
        UploadSource source,
        PdfExtractOptions options,
        CancellationToken cancellationToken = default)
    {
        var projectId = await _projectId(cancellationToken).ConfigureAwait(false);
        var prepared = MultipartHelper.PrepareUpload(
            source,
            options.Filename ?? "document.pdf",
            "application/pdf");
        using var form = new MultipartFormDataContent();
        MultipartHelper.AppendFile(form, "file", prepared);
        var url = _http.Path("projects", projectId, "tools", "pdf", "extract") + BuildExtractQuery(options);
        var result = await _http.RequestMultipartBinaryAsync(
                HttpMethod.Post,
                url,
                form,
                timeoutMs: ImazeoVersion.PdfTimeoutMs,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (result is BinaryResult binary)
            return binary;
        throw new InvalidOperationException("Expected PDF binary response from extract");
    }

    private static string BuildRenderQuery(PdfRenderOptions options)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(options.Pages))
            parts.Add($"pages={Uri.EscapeDataString(options.Pages)}");
        if (options.RangeStart is not null)
            parts.Add($"rangeStart={options.RangeStart.Value}");
        if (options.RangeEnd is not null)
            parts.Add($"rangeEnd={options.RangeEnd.Value}");
        if (options.All is not null)
            parts.Add($"all={options.All.Value.ToString().ToLowerInvariant()}");
        if (!string.IsNullOrEmpty(options.Format))
            parts.Add($"format={Uri.EscapeDataString(options.Format)}");
        if (options.Scale is not null)
            parts.Add($"scale={options.Scale.Value}");
        if (options.Quality is not null)
            parts.Add($"quality={options.Quality.Value}");
        return parts.Count == 0 ? "" : "?" + string.Join("&", parts);
    }

    private static string BuildExtractQuery(PdfExtractOptions options)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(options.Pages))
            parts.Add($"pages={Uri.EscapeDataString(options.Pages)}");
        if (options.RangeStart is not null)
            parts.Add($"rangeStart={options.RangeStart.Value}");
        if (options.RangeEnd is not null)
            parts.Add($"rangeEnd={options.RangeEnd.Value}");
        return parts.Count == 0 ? "" : "?" + string.Join("&", parts);
    }

    private static IReadOnlyList<PdfRenderPage> ExtractPages(object? data)
    {
        if (data is null) return Array.Empty<PdfRenderPage>();

        if (data is JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Object && el.TryGetProperty("pages", out var pagesEl))
                return pagesEl.Deserialize<List<PdfRenderPage>>(JsonOptions) ?? new List<PdfRenderPage>();
            return Array.Empty<PdfRenderPage>();
        }

        // object deserialized as JsonElement typically; fallback via re-serialize
        var json = JsonSerializer.SerializeToElement(data, JsonOptions);
        if (json.ValueKind == JsonValueKind.Object && json.TryGetProperty("pages", out var p))
            return p.Deserialize<List<PdfRenderPage>>(JsonOptions) ?? new List<PdfRenderPage>();
        return Array.Empty<PdfRenderPage>();
    }
}

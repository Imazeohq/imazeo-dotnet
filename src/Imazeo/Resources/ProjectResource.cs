using System.Text.Json;
using Imazeo.Http;
using Imazeo.Models;
namespace Imazeo.Resources;
public abstract class ProjectResource
{
    private protected readonly HttpTransport Http;
    private readonly Func<CancellationToken, Task<string>> _projectId;
    private protected ProjectResource(HttpTransport http, Func<CancellationToken, Task<string>> projectId) { Http = http; _projectId = projectId; }
    private protected async Task<string> Url(string[] parts, object? query, CancellationToken ct)
    {
        var id = await _projectId(ct).ConfigureAwait(false);
        var url = Http.Path(new[] { "projects", id }.Concat(parts).ToArray());
        if (query is null) return url;
        var element = JsonSerializer.SerializeToElement(query);
        var fields = element.EnumerateObject().Where(p => p.Value.ValueKind != JsonValueKind.Null).Select(p => Uri.EscapeDataString(p.Name) + "=" + Uri.EscapeDataString(p.Value.ValueKind == JsonValueKind.String ? p.Value.GetString()! : p.Value.GetRawText()));
        var qs = string.Join("&", fields);
        return url + (qs.Length > 0 ? "?" + qs : "");
    }
    private protected async Task<T> Json<T>(HttpMethod method, string[] parts, object? body = null, object? query = null, ProcessingOptions? processing = null, CancellationToken ct = default)
    {
        return (await Http.RequestJsonAsync<T>(method, await Url(parts, query, ct).ConfigureAwait(false), body, headers: processing?.Headers(), cancellationToken: ct).ConfigureAwait(false)).Data;
    }
    private protected async Task<PagedResult<T>> Page<T>(string[] parts, object? query, CancellationToken ct)
    {
        var result = await Http.RequestJsonAsync<List<T>>(HttpMethod.Get, await Url(parts, query, ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false);
        return new PagedResult<T> { Items = result.Data, Page = result.Meta.Page ?? 1, Limit = result.Meta.Limit ?? 20, Total = result.Meta.Total ?? result.Data.Count, Pages = result.Meta.Pages ?? 1, RequestId = result.Meta.RequestId };
    }
    private protected async Task<BinaryResult> Binary(string[] parts, object? query, CancellationToken ct) => await Http.RequestBinaryAsync(HttpMethod.Get, await Url(parts, query, ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false);
}

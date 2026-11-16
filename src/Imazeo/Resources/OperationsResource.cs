using System.Text.Json;
using Imazeo.Http;
using Imazeo.Models;
namespace Imazeo.Resources;
public sealed class OperationsResource : ProjectResource
{
    internal OperationsResource(HttpTransport http, Func<CancellationToken, Task<string>> projectId) : base(http, projectId) {}
    public Task<ProcessingStatus> GetAsync(string id, CancellationToken cancellationToken = default) => Json<ProcessingStatus>(HttpMethod.Get, new[] { "operations",id }, null, null, null, cancellationToken);
    public Task<ProcessingStatus> LookupAsync(ProcessingLookupInput input, CancellationToken cancellationToken = default) => Json<ProcessingStatus>(HttpMethod.Post, new[] { "operations","lookup" }, input, null, null, cancellationToken);
    public async Task<object> ResultAsync(string id, CancellationToken cancellationToken = default) {
            var result = await Binary(new[] {"operations", id, "result"}, null, cancellationToken).ConfigureAwait(false);
            if (result.ContentType.Contains("application/json")) { using var json = JsonDocument.Parse(result.Data); return json.RootElement.GetProperty("data").Clone(); } return result;
        }
}

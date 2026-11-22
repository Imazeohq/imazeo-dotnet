using System.Text.Json;
using Imazeo.Http;
using Imazeo.Models;
namespace Imazeo.Resources;
public sealed class OutputsResource : ProjectResource
{
    internal OutputsResource(HttpTransport http, Func<CancellationToken, Task<string>> projectId) : base(http, projectId) {}
    public Task<OutputsSummary> SummaryAsync(CancellationToken cancellationToken = default) => Json<OutputsSummary>(HttpMethod.Get, new[] { "outputs","summary" }, null, null, null, cancellationToken);
    public Task<PagedResult<OutputRow>> ListAsync(Dictionary<string, object?>? query = null, CancellationToken cancellationToken = default) => Page<OutputRow>(new[] { "outputs" }, query, cancellationToken);
}

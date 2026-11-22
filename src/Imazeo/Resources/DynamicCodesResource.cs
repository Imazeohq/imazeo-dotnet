using System.Text.Json;
using Imazeo.Http;
using Imazeo.Models;
namespace Imazeo.Resources;
public sealed class DynamicCodesResource : ProjectResource
{
    internal DynamicCodesResource(HttpTransport http, Func<CancellationToken, Task<string>> projectId) : base(http, projectId) {}
    public Task<DynamicCapabilities> CapabilitiesAsync(CancellationToken cancellationToken = default) => Json<DynamicCapabilities>(HttpMethod.Get, new[] { "dynamic-codes","capabilities" }, null, null, null, cancellationToken);
    public Task<PagedResult<DynamicCode>> ListAsync(Dictionary<string, object?>? query = null, CancellationToken cancellationToken = default) => Page<DynamicCode>(new[] { "dynamic-codes" }, query, cancellationToken);
    public Task<DynamicCode> GetAsync(string id, CancellationToken cancellationToken = default) => Json<DynamicCode>(HttpMethod.Get, new[] { "dynamic-codes",id }, null, null, null, cancellationToken);
    public Task<DynamicCode> CreateAsync(DynamicCreateInput input, string idempotencyKey, CancellationToken cancellationToken = default) => Json<DynamicCode>(HttpMethod.Post, new[] { "dynamic-codes" }, input, null, new ProcessingOptions { IdempotencyKey = idempotencyKey }, cancellationToken);
    public Task<DynamicCode> UpdateAsync(string id, DynamicEditInput input, string idempotencyKey, CancellationToken cancellationToken = default) => Json<DynamicCode>(HttpMethod.Patch, new[] { "dynamic-codes",id }, input, null, new ProcessingOptions { IdempotencyKey = idempotencyKey }, cancellationToken);
    public Task<ScanAnalytics> AnalyticsAsync(string id, Dictionary<string, object?>? query = null, CancellationToken cancellationToken = default) => Json<ScanAnalytics>(HttpMethod.Get, new[] { "dynamic-codes",id,"analytics" }, null, query, null, cancellationToken);
    public Task<BinaryResult> ExportAnalyticsAsync(string id, Dictionary<string, object?>? query = null, CancellationToken cancellationToken = default) => Binary(new[] { "dynamic-codes",id,"analytics","export" }, query, cancellationToken);
}

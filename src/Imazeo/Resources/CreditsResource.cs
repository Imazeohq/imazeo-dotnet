using System.Text.Json;
using Imazeo.Http;
using Imazeo.Models;
namespace Imazeo.Resources;
public sealed class CreditsResource : ProjectResource
{
    internal CreditsResource(HttpTransport http, Func<CancellationToken, Task<string>> projectId) : base(http, projectId) {}
    public Task<CreditQuote> QuoteAsync(CreditQuoteInput input, CancellationToken cancellationToken = default) => Json<CreditQuote>(HttpMethod.Post, new[] { "credits","quote" }, input, null, null, cancellationToken);
}

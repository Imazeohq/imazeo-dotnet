using System.Net;
using System.Text;
using System.Text.Json;
using Imazeo.Models;
using Xunit;

namespace Imazeo.Tests;

public class FeaturesTests
{
    private sealed class Handler : HttpMessageHandler
    {
        public Func<HttpRequestMessage, Task>? Check { get; init; }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Assert.True(request.Headers.Contains("X-API-Key"));
            Assert.False(request.Headers.Contains("Authorization"));
            if (Check is not null) await Check(request);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"success\":true,\"data\":{\"id\":\"job\",\"status\":\"queued\"},\"meta\":{\"requestId\":\"request\"}}", Encoding.UTF8, "application/json") };
        }
    }

    [Fact]
    public async Task Conversion_preserves_headers_upload_and_false_options()
    {
        using var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "test-key", ProjectId = "project",
            HttpMessageHandler = new Handler { Check = async request => {
                Assert.EndsWith("/conversions/xlsx?showGridLines=false", request.RequestUri!.ToString());
                Assert.Equal("operation-key", request.Headers.GetValues("Idempotency-Key").Single());
                Assert.Equal("quote-id", request.Headers.GetValues("X-Quote-ID").Single());
                Assert.Equal("3", request.Headers.GetValues("X-Upload-Length").Single());
                Assert.Contains("abc", await request.Content!.ReadAsStringAsync());
            } }
        });
        var job = await client.Conversions.XLSXAsync(new UploadSource { Data = Encoding.UTF8.GetBytes("abc") }, new ProcessingOptions { IdempotencyKey = "operation-key", QuoteId = "quote-id" }, new XlsxOptions { ShowGridLines = false });
        Assert.Equal("queued", job.Status);
    }

    [Fact]
    public async Task Dynamic_edit_preserves_revision_and_false_setting()
    {
        using var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "test-key", ProjectId = "project",
            HttpMessageHandler = new Handler { Check = async request => {
                Assert.Equal(HttpMethod.Patch, request.Method);
                Assert.Equal("edit-key", request.Headers.GetValues("Idempotency-Key").Single());
                using var json = JsonDocument.Parse(await request.Content!.ReadAsStringAsync());
                Assert.Equal(0, json.RootElement.GetProperty("expectedRevision").GetInt32());
                Assert.False(json.RootElement.GetProperty("analyticsEnabled").GetBoolean());
            } }
        });
        await client.DynamicCodes.UpdateAsync("code", new DynamicEditInput { ExpectedRevision = 0, AnalyticsEnabled = false }, "edit-key");
    }
}

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Imazeo.Errors;
using Imazeo.Models;
using Xunit;

namespace Imazeo.Tests;

public class ClientTests
{
    [Fact]
    public void NormalizeBaseUrl_strips_slash_and_v1()
    {
        Assert.Equal("https://api.example.com", ImazeoClient.NormalizeBaseUrl("https://api.example.com/"));
        Assert.Equal("https://api.example.com", ImazeoClient.NormalizeBaseUrl("https://api.example.com/v1"));
        Assert.Throws<ArgumentException>(() => ImazeoClient.NormalizeBaseUrl("ftp://x"));
    }

    [Fact]
    public void Rejects_unsupported_apiVersion()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new ImazeoClient(new ImazeoOptions
            {
                ApiKey = "key",
                ApiVersion = "v9",
            }));
        Assert.Contains("Unsupported apiVersion", ex.Message);
    }

    [Fact]
    public async Task Sends_X_API_Key_and_User_Agent()
    {
        HttpRequestMessage? seen = null;
        using var handler = new CallbackHandler(async (req, _) =>
        {
            seen = req;
            Assert.Equal("https://api.example.com/v1/api-key/context", req.RequestUri!.ToString());
            Assert.Equal("secret-key", req.Headers.GetValues("X-API-Key").Single());
            Assert.True(req.Headers.TryGetValues("User-Agent", out var ua));
            Assert.Equal($"Imazeo-DotNet/{ImazeoVersion.SdkVersion}", ua.First());
            Assert.False(req.Headers.Contains("Authorization"));
            return JsonResponse(new
            {
                projectId = "proj_1",
                keyId = "key_1",
            });
        });

        using var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "secret-key",
            BaseUrl = "https://api.example.com",
            HttpMessageHandler = handler,
        });

        var ctx = await client.Context.GetAsync();
        Assert.Equal("proj_1", ctx.ProjectId);
        Assert.NotNull(seen);
    }

    [Fact]
    public async Task Resolves_projectId_from_context_when_omitted()
    {
        using var handler = new CallbackHandler(async (req, _) =>
        {
            var url = req.RequestUri!.ToString();
            if (url.EndsWith("/api-key/context", StringComparison.Ordinal))
            {
                return JsonResponse(new
                {
                    projectId = "proj_auto",
                    keyId = "k",
                });
            }

            Assert.Contains("/v1/projects/proj_auto/images?", url, StringComparison.Ordinal);
            return JsonResponse(Array.Empty<object>(), meta: new { requestId = "req_test", page = 1, limit = 10, total = 0, pages = 0 });
        });

        using var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "k",
            BaseUrl = "https://api.example.com",
            HttpMessageHandler = handler,
        });

        var page = await client.Images.ListAsync(new PageParams { Page = 1, Limit = 10 });
        Assert.Empty(page.Items);
        Assert.Equal("req_test", page.RequestId);
    }

    [Fact]
    public async Task Uploads_with_X_Upload_Length_and_multipart_file()
    {
        var bytes = new byte[] { 1, 2, 3, 4 };
        using var handler = new CallbackHandler(async (req, _) =>
        {
            Assert.Equal("4", req.Headers.GetValues("X-Upload-Length").Single());
            Assert.IsType<MultipartFormDataContent>(req.Content);
            var form = (MultipartFormDataContent)req.Content!;
            Assert.Contains(form, c => c.Headers.ContentDisposition?.Name?.Trim('"') == "file"
                || c.Headers.ContentDisposition?.Name == "file");
            return JsonResponse(new
            {
                _id = "img_1",
                projectId = "p1",
                sourceType = "upload",
                originalName = "a.png",
                bytes = 4,
                sha256 = "abc",
                metadata = new
                {
                    format = "png",
                    mimeType = "image/png",
                    width = 1,
                    height = 1,
                    frames = 1,
                    pixelCount = 1,
                    hasAlpha = false,
                    orientation = (int?)null,
                },
                status = "ready",
                createdAt = "2026-01-01T00:00:00.000Z",
                updatedAt = "2026-01-01T00:00:00.000Z",
            }, status: HttpStatusCode.Created);
        });

        using var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "k",
            ProjectId = "p1",
            BaseUrl = "https://api.example.com",
            HttpMessageHandler = handler,
        });

        var image = await client.Images.UploadAsync(bytes, filename: "a.png", contentType: "image/png");
        Assert.Equal("img_1", image.Id);
    }

    [Fact]
    public async Task Maps_error_envelopes_to_typed_errors()
    {
        using (var handler = new CallbackHandler((_, _) =>
                   Task.FromResult(JsonResponse(new { code = "API_KEY_INVALID", message = "bad key" }, HttpStatusCode.Unauthorized, success: false))))
        using (var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "k",
            ProjectId = "p",
            BaseUrl = "https://api.example.com",
            HttpMessageHandler = handler,
        }))
        {
            await Assert.ThrowsAsync<AuthenticationError>(() => client.Images.GetAsync("x"));
        }

        using (var handler = new CallbackHandler((_, _) =>
                   Task.FromResult(JsonResponse(new { code = "RATE_LIMIT_EXCEEDED", message = "slow down" }, (HttpStatusCode)429, success: false))))
        using (var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "k",
            ProjectId = "p",
            BaseUrl = "https://api.example.com",
            HttpMessageHandler = handler,
        }))
        {
            await Assert.ThrowsAsync<RateLimitError>(() => client.Images.GetAsync("x"));
        }

        using (var handler = new CallbackHandler((_, _) =>
                   Task.FromResult(JsonResponse(new { code = "TOOL_INVALID", message = "bad" }, HttpStatusCode.BadRequest, success: false))))
        using (var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "k",
            ProjectId = "p",
            BaseUrl = "https://api.example.com",
            HttpMessageHandler = handler,
        }))
        {
            await Assert.ThrowsAsync<ValidationError>(() =>
                client.Qr.GenerateAsync(new QrGenerateInput { Content = "hi" }));
        }
    }

    [Fact]
    public async Task Returns_binary_results_for_QR_generation()
    {
        var png = new byte[] { 137, 80, 78, 71 };
        using var handler = new CallbackHandler((_, _) =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(png),
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            response.Headers.TryAddWithoutValidation("x-request-id", "req_bin");
            return Task.FromResult(response);
        });

        using var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "k",
            ProjectId = "p",
            BaseUrl = "https://api.example.com",
            HttpMessageHandler = handler,
        });

        var result = await client.Qr.GenerateAsync(new QrGenerateInput { Content = "https://imazeo.com" });
        Assert.Equal("image/png", result.ContentType);
        Assert.Equal(png, result.Data);
        Assert.Equal("req_bin", result.RequestId);
    }

    [Fact]
    public async Task Passes_image_list_q_filter()
    {
        using var handler = new CallbackHandler(async (req, _) =>
        {
            Assert.Contains("q=photo", req.RequestUri!.Query, StringComparison.Ordinal);
            return JsonResponse(Array.Empty<object>());
        });

        using var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "k",
            ProjectId = "p",
            BaseUrl = "https://api.example.com",
            HttpMessageHandler = handler,
        });

        await client.Images.ListAsync(new PageParams { Q = "photo", Page = 1 });
    }

    [Fact]
    public async Task Returns_notModified_on_transform_download_304()
    {
        using var handler = new CallbackHandler((_, _) =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotModified);
            response.Headers.TryAddWithoutValidation("etag", "\"abc\"");
            response.Headers.TryAddWithoutValidation("x-request-id", "req_304");
            return Task.FromResult(response);
        });

        using var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "k",
            ProjectId = "p",
            BaseUrl = "https://api.example.com",
            HttpMessageHandler = handler,
        });

        var result = await client.Transforms.DownloadAsync("tr_1", ifNoneMatch: "\"abc\"");
        Assert.True(result.NotModified);
        Assert.Empty(result.Data);
        Assert.Equal("req_304", result.RequestId);
        Assert.Equal("\"abc\"", result.ETag);
    }

    [Fact]
    public async Task Posts_transform_JSON_and_returns_binary()
    {
        using var handler = new CallbackHandler(async (req, _) =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            var body = await req.Content!.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.Equal("img", doc.RootElement.GetProperty("imageId").GetString());
            Assert.Equal(100, doc.RootElement.GetProperty("transform").GetProperty("width").GetInt32());
            Assert.Equal("webp", doc.RootElement.GetProperty("transform").GetProperty("format").GetString());

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(new byte[] { 1, 2 }),
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/webp");
            response.Headers.TryAddWithoutValidation("x-transformation-id", "tr_1");
            response.Headers.TryAddWithoutValidation("x-cache", "MISS");
            response.Headers.TryAddWithoutValidation("x-request-id", "req_t");
            return response;
        });

        using var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = "k",
            ProjectId = "p",
            BaseUrl = "https://api.example.com",
            HttpMessageHandler = handler,
        });

        var output = await client.Transforms.CreateAsync(new TransformRequest
        {
            ImageId = "img",
            Transform = new TransformSpec { Width = 100, Format = "webp" },
        });
        Assert.Equal("tr_1", output.TransformationId);
        Assert.Equal("MISS", output.Cache);
    }

    private static HttpResponseMessage JsonResponse(
        object data,
        HttpStatusCode status = HttpStatusCode.OK,
        object? meta = null,
        bool success = true)
    {
        object body = success
            ? new { success = true, data, meta = meta ?? new { requestId = "req_test" } }
            : new { success = false, error = data, meta = meta ?? new { requestId = "req_test" } };

        var response = new HttpResponseMessage(status)
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"),
        };
        response.Headers.TryAddWithoutValidation("x-request-id", "req_test");
        return response;
    }

    private sealed class CallbackHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _callback;

        public CallbackHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> callback)
            => _callback = callback;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => _callback(request, cancellationToken);
    }
}

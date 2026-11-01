using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Imazeo.Errors;
using Imazeo.Models;

namespace Imazeo.Http;

internal sealed class JsonSuccess<T>
{
    public required T Data { get; init; }
    public required ResponseMeta Meta { get; init; }
}

internal sealed class ResponseMeta
{
    public required string RequestId { get; init; }
    public int? Page { get; init; }
    public int? Limit { get; init; }
    public int? Total { get; init; }
    public int? Pages { get; init; }
}

internal sealed class TransportConfig
{
    public required string ApiKey { get; init; }
    public required string BaseUrl { get; init; }
    public required string ApiVersion { get; init; }
    public required int TimeoutMs { get; init; }
    public required string UserAgent { get; init; }
}

internal static class TransportConfigFactory
{
    public static string NormalizeBaseUrl(string raw)
    {
        if (!Uri.TryCreate(raw, UriKind.Absolute, out var url))
            throw new ArgumentException($"Invalid baseUrl: {raw}", nameof(raw));

        if (url.Scheme is not ("http" or "https"))
            throw new ArgumentException("baseUrl must use http or https", nameof(raw));

        if (!string.IsNullOrEmpty(url.UserInfo))
            throw new ArgumentException("baseUrl must not include credentials", nameof(raw));

        var path = url.AbsolutePath.TrimEnd('/');
        if (path == "/v1")
            path = "";

        var authority = url.GetLeftPart(UriPartial.Authority).TrimEnd('/');
        if (string.IsNullOrEmpty(path) || path == "/")
            return authority;

        return $"{authority}{path}";
    }

    public static TransportConfig Create(ImazeoOptions options)
    {
        var apiKey = options.ApiKey?.Trim() ?? "";
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentException("apiKey is required", nameof(options));
        if (apiKey.Length > 128)
            throw new ArgumentException("apiKey is too long", nameof(options));

        var apiVersion = options.ApiVersion ?? ImazeoVersion.DefaultApiVersion;
        if (!ImazeoVersion.SupportedApiVersions.Contains(apiVersion, StringComparer.Ordinal))
            throw new ArgumentException(
                $"Unsupported apiVersion \"{apiVersion}\". Supported: {string.Join(", ", ImazeoVersion.SupportedApiVersions)}",
                nameof(options));

        var baseUrl = NormalizeBaseUrl(options.BaseUrl ?? ImazeoVersion.DefaultBaseUrl);
        var timeoutMs = options.TimeoutMs ?? ImazeoVersion.DefaultTimeoutMs;
        if (timeoutMs < 1)
            throw new ArgumentException("timeoutMs must be a positive number", nameof(options));

        var ua = string.IsNullOrWhiteSpace(options.AppName)
            ? $"Imazeo-DotNet/{ImazeoVersion.SdkVersion}"
            : $"Imazeo-DotNet/{ImazeoVersion.SdkVersion} {options.AppName.Trim()}";

        if (ua.Length > 256)
            ua = ua[..256];

        return new TransportConfig
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl,
            ApiVersion = apiVersion,
            TimeoutMs = timeoutMs,
            UserAgent = ua,
        };
    }
}

internal sealed class HttpTransport : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
    };

    private readonly TransportConfig _config;
    private readonly HttpClient _http;
    private readonly bool _ownsClient;

    public HttpTransport(TransportConfig config, HttpClient httpClient, bool ownsClient)
    {
        _config = config;
        _http = httpClient;
        _ownsClient = ownsClient;
        if (_http.DefaultRequestHeaders.UserAgent.Count == 0)
        {
            // Prefer per-request User-Agent; avoid mutating shared clients when possible.
        }
    }

    public TransportConfig Config => _config;

    public string Path(params string[] segments)
    {
        var joined = string.Join("/", segments.Select(s => Uri.EscapeDataString(s).Replace("%2F", "/", StringComparison.Ordinal)));
        return $"{_config.BaseUrl}/{_config.ApiVersion}/{joined}";
    }

    public async Task<JsonSuccess<T>> RequestJsonAsync<T>(
        HttpMethod method,
        string url,
        object? body = null,
        IDictionary<string, string>? headers = null,
        int? timeoutMs = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(method, url);
        ApplyCommonHeaders(request, accept: "application/json", headers);
        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        using var response = await SendAsync(request, timeoutMs, cancellationToken).ConfigureAwait(false);
        return await ParseJsonEnvelopeAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<BinaryResult> RequestBinaryAsync(
        HttpMethod method,
        string url,
        HttpContent? content = null,
        IDictionary<string, string>? headers = null,
        int? timeoutMs = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(method, url);
        ApplyCommonHeaders(request, accept: "*/*", headers);
        if (content is not null)
            request.Content = content;

        using var response = await SendAsync(request, timeoutMs, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            var headerMap = HeaderRecord(response);
            string? ifNoneMatch = null;
            headers?.TryGetValue("If-None-Match", out ifNoneMatch);
            return new BinaryResult
            {
                Data = Array.Empty<byte>(),
                ContentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream",
                RequestId = GetHeader(response, "x-request-id") ?? "",
                Headers = headerMap,
                ETag = GetHeader(response, "etag") ?? ifNoneMatch,
                Width = NumHeader(response, "x-image-width"),
                Height = NumHeader(response, "x-image-height"),
                Format = GetHeader(response, "x-image-format"),
                TransformationId = GetHeader(response, "x-transformation-id"),
                Cache = GetHeader(response, "x-cache"),
                PdfPage = NumHeader(response, "x-pdf-page"),
                NotModified = true,
            };
        }

        if (!response.IsSuccessStatusCode)
            throw await ErrorFromResponseAsync(response, cancellationToken).ConfigureAwait(false);

        var buf = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
        return new BinaryResult
        {
            Data = buf,
            ContentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream",
            RequestId = GetHeader(response, "x-request-id") ?? "",
            Headers = HeaderRecord(response),
            ETag = GetHeader(response, "etag"),
            Width = NumHeader(response, "x-image-width"),
            Height = NumHeader(response, "x-image-height"),
            Format = GetHeader(response, "x-image-format"),
            TransformationId = GetHeader(response, "x-transformation-id"),
            Cache = GetHeader(response, "x-cache"),
            PdfPage = NumHeader(response, "x-pdf-page"),
        };
    }

    public async Task<JsonSuccess<T>> RequestMultipartJsonAsync<T>(
        HttpMethod method,
        string url,
        MultipartFormDataContent form,
        long? uploadLength = null,
        int? timeoutMs = null,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(method, url);
        ApplyCommonHeaders(request, accept: "application/json", headers);
        if (uploadLength is not null)
            request.Headers.TryAddWithoutValidation("X-Upload-Length", uploadLength.Value.ToString());
        request.Content = form;

        using var response = await SendAsync(request, timeoutMs, cancellationToken).ConfigureAwait(false);
        return await ParseJsonEnvelopeAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<object> RequestMultipartBinaryAsync(
        HttpMethod method,
        string url,
        MultipartFormDataContent form,
        long? uploadLength = null,
        int? timeoutMs = null,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(method, url);
        ApplyCommonHeaders(request, accept: "*/*", headers);
        if (uploadLength is not null)
            request.Headers.TryAddWithoutValidation("X-Upload-Length", uploadLength.Value.ToString());
        request.Content = form;

        using var response = await SendAsync(request, timeoutMs, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            throw await ErrorFromResponseAsync(response, cancellationToken).ConfigureAwait(false);

        var ct = response.Content.Headers.ContentType?.MediaType ?? "";
        if (ct.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            return await ParseJsonEnvelopeAsync<object>(response, cancellationToken).ConfigureAwait(false);

        var buf = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
        return new BinaryResult
        {
            Data = buf,
            ContentType = string.IsNullOrEmpty(ct) ? "application/octet-stream" : (response.Content.Headers.ContentType?.ToString() ?? ct),
            RequestId = GetHeader(response, "x-request-id") ?? "",
            Headers = HeaderRecord(response),
            ETag = GetHeader(response, "etag"),
            Width = NumHeader(response, "x-image-width"),
            Height = NumHeader(response, "x-image-height"),
            Format = GetHeader(response, "x-image-format"),
            PdfPage = NumHeader(response, "x-pdf-page"),
        };
    }

    private void ApplyCommonHeaders(
        HttpRequestMessage request,
        string accept,
        IDictionary<string, string>? headers)
    {
        request.Headers.TryAddWithoutValidation("Accept", accept);
        request.Headers.TryAddWithoutValidation("X-API-Key", _config.ApiKey);
        request.Headers.TryAddWithoutValidation("User-Agent", _config.UserAgent);
        if (headers is null) return;
        foreach (var (key, value) in headers)
            request.Headers.TryAddWithoutValidation(key, value);
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        int? timeoutMs,
        CancellationToken cancellationToken)
    {
        var ms = timeoutMs ?? _config.TimeoutMs;
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromMilliseconds(ms));
        try
        {
            return await _http.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    timeoutCts.Token)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new NetworkError($"Request timed out after {ms}ms", innerException: ex);
        }
        catch (HttpRequestException ex)
        {
            throw new NetworkError(ex.Message, innerException: ex);
        }
        catch (Exception ex) when (ex is not ImazeoError and not OperationCanceledException)
        {
            throw new NetworkError(ex.Message, innerException: ex);
        }
    }

    private async Task<JsonSuccess<T>> ParseJsonEnvelopeAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var requestId = GetHeader(response, "x-request-id");
        string text;
        try
        {
            text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw ErrorMapper.MapHttpError(
                    (int)response.StatusCode,
                    response.ReasonPhrase ?? "Request failed",
                    requestId: requestId);
            }

            throw new NetworkError("Response was not valid JSON", requestId: requestId, innerException: ex);
        }

        JsonElement payload;
        try
        {
            payload = string.IsNullOrWhiteSpace(text)
                ? default
                : JsonSerializer.Deserialize<JsonElement>(text);
        }
        catch (Exception ex)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw ErrorMapper.MapHttpError(
                    (int)response.StatusCode,
                    response.ReasonPhrase ?? "Request failed",
                    requestId: requestId);
            }

            throw new NetworkError("Response was not valid JSON", requestId: requestId, innerException: ex);
        }

        if (!response.IsSuccessStatusCode)
            throw ErrorFromPayload((int)response.StatusCode, payload, requestId);

        if (payload.ValueKind != JsonValueKind.Object
            || !payload.TryGetProperty("success", out var success)
            || success.ValueKind != JsonValueKind.True
            || !payload.TryGetProperty("data", out var dataEl))
        {
            throw ErrorMapper.MapHttpError(
                (int)response.StatusCode,
                "Unexpected success envelope",
                requestId: requestId);
        }

        T? data;
        try
        {
            data = dataEl.Deserialize<T>(JsonOptions);
        }
        catch (Exception ex)
        {
            throw new NetworkError("Failed to deserialize response data", requestId: requestId, innerException: ex);
        }

        if (data is null && typeof(T).IsValueType)
            throw new NetworkError("Failed to deserialize response data", requestId: requestId);

        string metaRequestId = requestId ?? "";
        int? page = null, limit = null, total = null, pages = null;
        if (payload.TryGetProperty("meta", out var meta) && meta.ValueKind == JsonValueKind.Object)
        {
            if (meta.TryGetProperty("requestId", out var rid) && rid.ValueKind == JsonValueKind.String)
                metaRequestId = rid.GetString() ?? metaRequestId;
            page = GetInt(meta, "page");
            limit = GetInt(meta, "limit");
            total = GetInt(meta, "total");
            pages = GetInt(meta, "pages");
        }

        return new JsonSuccess<T>
        {
            Data = data!,
            Meta = new ResponseMeta
            {
                RequestId = metaRequestId,
                Page = page,
                Limit = limit,
                Total = total,
                Pages = pages,
            },
        };
    }

    private static async Task<ImazeoError> ErrorFromResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var requestId = GetHeader(response, "x-request-id");
        try
        {
            var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var payload = JsonSerializer.Deserialize<JsonElement>(text);
            return ErrorFromPayload((int)response.StatusCode, payload, requestId);
        }
        catch
        {
            return ErrorMapper.MapHttpError(
                (int)response.StatusCode,
                response.ReasonPhrase ?? "Request failed",
                requestId: requestId);
        }
    }

    private static ImazeoError ErrorFromPayload(int status, JsonElement payload, string? requestId)
    {
        string? code = null;
        string message = "Request failed";
        object? details = null;
        string? metaRequestId = requestId;

        if (payload.ValueKind == JsonValueKind.Object
            && payload.TryGetProperty("error", out var error)
            && error.ValueKind == JsonValueKind.Object)
        {
            if (error.TryGetProperty("code", out var c) && c.ValueKind == JsonValueKind.String)
                code = c.GetString();
            if (error.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String)
                message = m.GetString() ?? message;
            if (error.TryGetProperty("details", out var d))
                details = d;
        }

        if (payload.ValueKind == JsonValueKind.Object
            && payload.TryGetProperty("meta", out var meta)
            && meta.ValueKind == JsonValueKind.Object
            && meta.TryGetProperty("requestId", out var rid)
            && rid.ValueKind == JsonValueKind.String)
        {
            metaRequestId = rid.GetString() ?? metaRequestId;
        }

        return ErrorMapper.MapHttpError(status, message, code, metaRequestId, details);
    }

    private static Dictionary<string, string> HeaderRecord(HttpResponseMessage response)
    {
        var outMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in response.Headers)
            outMap[header.Key.ToLowerInvariant()] = string.Join(", ", header.Value);
        foreach (var header in response.Content.Headers)
            outMap[header.Key.ToLowerInvariant()] = string.Join(", ", header.Value);
        return outMap;
    }

    private static string? GetHeader(HttpResponseMessage response, string name)
    {
        if (response.Headers.TryGetValues(name, out var values))
            return values.FirstOrDefault();
        if (response.Content.Headers.TryGetValues(name, out var contentValues))
            return contentValues.FirstOrDefault();
        return null;
    }

    private static int? NumHeader(HttpResponseMessage response, string name)
    {
        var raw = GetHeader(response, name);
        if (string.IsNullOrEmpty(raw)) return null;
        return int.TryParse(raw, out var n) ? n : null;
    }

    private static int? GetInt(JsonElement obj, string name)
    {
        if (!obj.TryGetProperty(name, out var el)) return null;
        if (el.ValueKind == JsonValueKind.Number && el.TryGetInt32(out var n)) return n;
        return null;
    }

    public void Dispose()
    {
        if (_ownsClient)
            _http.Dispose();
    }
}

internal static class MultipartHelper
{
    public static PreparedUpload PrepareUpload(
        UploadSource source,
        string? defaultFilename = null,
        string? defaultContentType = null)
    {
        return new PreparedUpload(
            source.Data,
            source.Filename ?? defaultFilename ?? "file.bin",
            source.ContentType ?? defaultContentType ?? "application/octet-stream",
            source.Data.LongLength);
    }

    public static PreparedUpload PrepareUpload(
        byte[] data,
        string? filename = null,
        string? contentType = null)
    {
        return new PreparedUpload(
            data,
            filename ?? "file.bin",
            contentType ?? "application/octet-stream",
            data.LongLength);
    }

    public static void AppendFile(MultipartFormDataContent form, string field, PreparedUpload prepared)
    {
        var content = new ByteArrayContent(prepared.Bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue(prepared.ContentType);
        form.Add(content, field, prepared.Filename);
    }
}

internal sealed record PreparedUpload(
    byte[] Bytes,
    string Filename,
    string ContentType,
    long ByteLength);

namespace Imazeo;

/// <summary>
/// Configuration for <see cref="ImazeoClient"/>.
/// Secret API keys are for trusted servers only — never embed in browsers.
/// </summary>
public sealed class ImazeoOptions
{
    /// <summary>Secret project API key. Sent as <c>X-API-Key</c>.</summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Project id used in <c>/v1/projects/:projectId/...</c> paths.
    /// If omitted, resolved once via <c>GET /v1/api-key/context</c>.
    /// </summary>
    public string? ProjectId { get; init; }

    /// <summary>API origin only (no <c>/v1</c> suffix). Default: https://api.imazeo.com</summary>
    public string? BaseUrl { get; init; }

    /// <summary>Allowlisted API version. Default: v1</summary>
    public string? ApiVersion { get; init; }

    /// <summary>Default request timeout in milliseconds. Default: 60000</summary>
    public int? TimeoutMs { get; init; }

    /// <summary>Optional app name appended to User-Agent.</summary>
    public string? AppName { get; init; }

    /// <summary>
    /// Optional shared <see cref="HttpClient"/>. When provided, the SDK does not dispose it.
    /// Prefer a single long-lived client or <c>IHttpClientFactory</c>.
    /// </summary>
    public HttpClient? HttpClient { get; init; }

    /// <summary>
    /// Optional message handler for tests. Ignored when <see cref="HttpClient"/> is set.
    /// The SDK owns and disposes the handler-backed client when this is used alone.
    /// </summary>
    public HttpMessageHandler? HttpMessageHandler { get; init; }
}

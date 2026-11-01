using Imazeo.Http;
using Imazeo.Resources;

namespace Imazeo;

/// <summary>
/// Official Imazeo server-side client.
/// Uses secret <c>X-API-Key</c> authentication. Do not embed API keys in browsers.
/// </summary>
public sealed class ImazeoClient : IDisposable
{
    private readonly HttpTransport _http;
    private readonly string? _configuredProjectId;
    private readonly SemaphoreSlim _projectLock = new(1, 1);
    private string? _resolvedProjectId;
    private bool _disposed;

    public ContextResource Context { get; }
    public ImagesResource Images { get; }
    public TransformsResource Transforms { get; }
    public QrResource Qr { get; }
    public BarcodesResource Barcodes { get; }
    public PdfResource Pdf { get; }

    /// <summary>SDK package version (SemVer).</summary>
    public static string Version => ImazeoVersion.SdkVersion;

    /// <summary>Default and supported API versions.</summary>
    public static (string DefaultApi, IReadOnlyList<string> Supported) ApiCompatibility =>
        (ImazeoVersion.DefaultApiVersion, ImazeoVersion.SupportedApiVersions);

    /// <summary>Normalizes an API origin (strips trailing slash and accidental <c>/v1</c>).</summary>
    public static string NormalizeBaseUrl(string baseUrl) =>
        TransportConfigFactory.NormalizeBaseUrl(baseUrl);

    public ImazeoClient(ImazeoOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var config = TransportConfigFactory.Create(options);

        HttpClient httpClient;
        bool ownsClient;
        if (options.HttpClient is not null)
        {
            httpClient = options.HttpClient;
            ownsClient = false;
        }
        else if (options.HttpMessageHandler is not null)
        {
            httpClient = new HttpClient(options.HttpMessageHandler, disposeHandler: true);
            ownsClient = true;
        }
        else
        {
            httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
            ownsClient = true;
        }

        _http = new HttpTransport(config, httpClient, ownsClient);
        _configuredProjectId = string.IsNullOrWhiteSpace(options.ProjectId)
            ? null
            : options.ProjectId.Trim();

        Context = new ContextResource(_http);
        Images = new ImagesResource(_http, ResolveProjectIdAsync);
        Transforms = new TransformsResource(_http, ResolveProjectIdAsync);
        Qr = new QrResource(_http, ResolveProjectIdAsync);
        Barcodes = new BarcodesResource(_http, ResolveProjectIdAsync);
        Pdf = new PdfResource(_http, ResolveProjectIdAsync);
    }

    private async Task<string> ResolveProjectIdAsync(CancellationToken cancellationToken)
    {
        if (_configuredProjectId is not null)
            return _configuredProjectId;
        if (_resolvedProjectId is not null)
            return _resolvedProjectId;

        await _projectLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_resolvedProjectId is not null)
                return _resolvedProjectId;

            var ctx = await Context.GetAsync(cancellationToken).ConfigureAwait(false);
            _resolvedProjectId = ctx.ProjectId;
            return _resolvedProjectId;
        }
        finally
        {
            _projectLock.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _projectLock.Dispose();
        _http.Dispose();
    }
}

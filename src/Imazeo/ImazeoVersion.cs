namespace Imazeo;

/// <summary>SDK package and API version constants.</summary>
public static class ImazeoVersion
{
    /// <summary>SDK package SemVer (keep in sync with the NuGet package version).</summary>
    public const string SdkVersion = "1.0.1";

    /// <summary>Default API path version.</summary>
    public const string DefaultApiVersion = "v1";

    /// <summary>Allowlisted API versions.</summary>
    public static readonly IReadOnlyList<string> SupportedApiVersions = new[] { "v1" };

    /// <summary>Default API origin (no trailing slash, no /v1).</summary>
    public const string DefaultBaseUrl = "https://api.imazeo.com";

    /// <summary>Default client-wide timeout in milliseconds.</summary>
    public const int DefaultTimeoutMs = 60_000;

    /// <summary>Timeout for PDF operations.</summary>
    public const int PdfTimeoutMs = 120_000;

    /// <summary>Timeout for lightweight JSON/tool operations (QR, barcode, context).</summary>
    public const int LightTimeoutMs = 30_000;
}

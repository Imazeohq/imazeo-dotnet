using System.Text.Json.Serialization;

namespace Imazeo.Models;

public sealed class PageParams
{
    public int? Page { get; init; }
    public int? Limit { get; init; }

    /// <summary>Filename / display-name prefix filter where the resource supports it.</summary>
    public string? Q { get; init; }
}

/// <summary>Paginated list result (items + page/limit/total/pages).</summary>
public sealed class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int Page { get; init; }
    public required int Limit { get; init; }
    public required int Total { get; init; }
    public required int Pages { get; init; }
    public required string RequestId { get; init; }
}

public sealed class BinaryResult
{
    public required byte[] Data { get; init; }
    public required string ContentType { get; init; }
    public required string RequestId { get; init; }
    public required IReadOnlyDictionary<string, string> Headers { get; init; }
    public string? ETag { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public string? Format { get; init; }
    public string? TransformationId { get; init; }
    public string? Cache { get; init; }
    public int? PdfPage { get; init; }

    /// <summary>True when the server returned HTTP 304 for an If-None-Match request.</summary>
    public bool NotModified { get; init; }
}

public sealed class ImageMetadata
{
    [JsonPropertyName("format")]
    public string Format { get; set; } = "";

    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; } = "";

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("frames")]
    public int Frames { get; set; }

    [JsonPropertyName("pixelCount")]
    public int PixelCount { get; set; }

    [JsonPropertyName("hasAlpha")]
    public bool HasAlpha { get; set; }

    [JsonPropertyName("orientation")]
    public int? Orientation { get; set; }
}

public sealed class ImageRecord
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = "";

    [JsonPropertyName("sourceType")]
    public string SourceType { get; set; } = "";

    [JsonPropertyName("originalName")]
    public string OriginalName { get; set; } = "";

    [JsonPropertyName("bytes")]
    public long Bytes { get; set; }

    [JsonPropertyName("sha256")]
    public string Sha256 { get; set; } = "";

    [JsonPropertyName("metadata")]
    public ImageMetadata Metadata { get; set; } = new();

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("createdAt")]
    public string CreatedAt { get; set; } = "";

    [JsonPropertyName("updatedAt")]
    public string UpdatedAt { get; set; } = "";
}

public sealed class CropSpec
{
    [JsonPropertyName("left")]
    public int Left { get; set; }

    [JsonPropertyName("top")]
    public int Top { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}

public sealed class TrimSpec
{
    [JsonPropertyName("threshold")]
    public int Threshold { get; set; }
}

public sealed class ExtendSpec
{
    [JsonPropertyName("top")]
    public int Top { get; set; }

    [JsonPropertyName("right")]
    public int Right { get; set; }

    [JsonPropertyName("bottom")]
    public int Bottom { get; set; }

    [JsonPropertyName("left")]
    public int Left { get; set; }
}

public sealed class TransformSpec
{
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    /// <summary>cover | contain | inside | fill</summary>
    [JsonPropertyName("fit")]
    public string? Fit { get; set; }

    [JsonPropertyName("crop")]
    public CropSpec? Crop { get; set; }

    /// <summary>jpeg | png | webp | avif | gif</summary>
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("quality")]
    public int? Quality { get; set; }

    [JsonPropertyName("rotate")]
    public int? Rotate { get; set; }

    [JsonPropertyName("flip")]
    public bool? Flip { get; set; }

    [JsonPropertyName("flop")]
    public bool? Flop { get; set; }

    [JsonPropertyName("optimize")]
    public bool? Optimize { get; set; }

    [JsonPropertyName("stripMetadata")]
    public bool? StripMetadata { get; set; }

    [JsonPropertyName("grayscale")]
    public bool? Grayscale { get; set; }

    [JsonPropertyName("negate")]
    public bool? Negate { get; set; }

    [JsonPropertyName("blur")]
    public double? Blur { get; set; }

    [JsonPropertyName("sharpen")]
    public double? Sharpen { get; set; }

    [JsonPropertyName("brightness")]
    public double? Brightness { get; set; }

    [JsonPropertyName("saturation")]
    public double? Saturation { get; set; }

    [JsonPropertyName("hue")]
    public int? Hue { get; set; }

    /// <summary>True for default trim, or a threshold object.</summary>
    [JsonPropertyName("trim")]
    public object? Trim { get; set; }

    [JsonPropertyName("extend")]
    public ExtendSpec? Extend { get; set; }

    [JsonPropertyName("background")]
    public string? Background { get; set; }

    /// <summary>center | entropy | attention</summary>
    [JsonPropertyName("position")]
    public string? Position { get; set; }
}

/// <summary>PERSIST | CACHE | EPHEMERAL</summary>
public static class TransformModes
{
    public const string Persist = "PERSIST";
    public const string Cache = "CACHE";
    public const string Ephemeral = "EPHEMERAL";
}

public sealed class TransformRequest
{
    [JsonPropertyName("imageId")]
    public string? ImageId { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("maxOutputBytes")]
    public long? MaxOutputBytes { get; set; }

    [JsonPropertyName("transform")]
    public TransformSpec? Transform { get; set; }
}

public sealed class TransformationRecord
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = "";

    [JsonPropertyName("imageId")]
    public string? ImageId { get; set; }

    [JsonPropertyName("sourceHash")]
    public string SourceHash { get; set; } = "";

    [JsonPropertyName("transformHash")]
    public string TransformHash { get; set; } = "";

    [JsonPropertyName("spec")]
    public TransformSpec Spec { get; set; } = new();

    [JsonPropertyName("mode")]
    public string Mode { get; set; } = "";

    [JsonPropertyName("bytes")]
    public long Bytes { get; set; }

    [JsonPropertyName("processingMs")]
    public int ProcessingMs { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("sha256")]
    public string Sha256 { get; set; } = "";

    [JsonPropertyName("metadata")]
    public ImageMetadata Metadata { get; set; } = new();

    [JsonPropertyName("expiresAt")]
    public string? ExpiresAt { get; set; }

    [JsonPropertyName("createdAt")]
    public string CreatedAt { get; set; } = "";
}

public sealed class ApiKeyContext
{
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = "";

    [JsonPropertyName("keyId")]
    public string KeyId { get; set; } = "";
}

public sealed class QrGenerateInput
{
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    /// <summary>png | svg</summary>
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }

    [JsonPropertyName("margin")]
    public int? Margin { get; set; }

    /// <summary>L | M | Q | H</summary>
    [JsonPropertyName("errorCorrection")]
    public string? ErrorCorrection { get; set; }

    [JsonPropertyName("foreground")]
    public string? Foreground { get; set; }

    [JsonPropertyName("background")]
    public string? Background { get; set; }
}

/// <summary>Supported barcode symbologies.</summary>
public static class BarcodeTypes
{
    public const string Code128 = "code128";
    public const string Code39 = "code39";
    public const string Ean13 = "ean13";
    public const string Ean8 = "ean8";
    public const string UpcA = "upca";
    public const string Itf14 = "itf14";
}

public sealed class BarcodeGenerateInput
{
    /// <summary>code128 | code39 | ean13 | ean8 | upca | itf14</summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }

    /// <summary>png | svg</summary>
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("scale")]
    public int? Scale { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("includeText")]
    public bool? IncludeText { get; set; }

    [JsonPropertyName("foreground")]
    public string? Foreground { get; set; }

    [JsonPropertyName("background")]
    public string? Background { get; set; }
}

public sealed class PdfPageInfo
{
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }

    [JsonPropertyName("width")]
    public double Width { get; set; }

    [JsonPropertyName("height")]
    public double Height { get; set; }
}

public sealed class PdfInfo
{
    [JsonPropertyName("pageCount")]
    public int PageCount { get; set; }

    [JsonPropertyName("encrypted")]
    public bool Encrypted { get; set; }

    [JsonPropertyName("pages")]
    public List<PdfPageInfo> Pages { get; set; } = new();
}

public sealed class PdfRenderOptions
{
    public string? Pages { get; init; }
    public int? RangeStart { get; init; }
    public int? RangeEnd { get; init; }
    public bool? All { get; init; }

    /// <summary>png | jpeg | webp</summary>
    public string? Format { get; init; }

    public double? Scale { get; init; }
    public int? Quality { get; init; }
    public string? Filename { get; init; }
}

public sealed class PdfRenderPage
{
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }

    [JsonPropertyName("width")]
    public double Width { get; set; }

    [JsonPropertyName("height")]
    public double Height { get; set; }

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = "";

    [JsonPropertyName("data")]
    public string Data { get; set; } = "";
}

public sealed class PdfMultiPageRenderResult
{
    public required IReadOnlyList<PdfRenderPage> Pages { get; init; }
    public required string RequestId { get; init; }
}

public sealed class PdfExtractOptions
{
    public string? Pages { get; init; }
    public int? RangeStart { get; init; }
    public int? RangeEnd { get; init; }
    public string? Filename { get; init; }
}

public sealed class MessageResult
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
}

/// <summary>Local file bytes for multipart uploads (never sent as a path string).</summary>
public sealed class UploadSource
{
    public required byte[] Data { get; init; }
    public string? Filename { get; init; }
    public string? ContentType { get; init; }

    public static implicit operator UploadSource(byte[] data) => new() { Data = data };
}

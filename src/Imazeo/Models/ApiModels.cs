using System.Text.Json.Serialization;
namespace Imazeo.Models;

public sealed class CodeDesign
{
    [JsonPropertyName("moduleShape")]
    public string? ModuleShape { get; set; }
    [JsonPropertyName("finderOuter")]
    public string? FinderOuter { get; set; }
    [JsonPropertyName("finderInner")]
    public string? FinderInner { get; set; }
    [JsonPropertyName("fill")]
    public string? Fill { get; set; }
    [JsonPropertyName("textAlign")]
    public string? TextAlign { get; set; }
    [JsonPropertyName("fontFamily")]
    public string? FontFamily { get; set; }
    [JsonPropertyName("fontWeight")]
    public string? FontWeight { get; set; }
    [JsonPropertyName("barTextAlign")]
    public string? BarTextAlign { get; set; }
    [JsonPropertyName("barTextPosition")]
    public string? BarTextPosition { get; set; }
    [JsonPropertyName("moduleScale")]
    public int? ModuleScale { get; set; }
    [JsonPropertyName("gradientAngle")]
    public int? GradientAngle { get; set; }
    [JsonPropertyName("logoScale")]
    public int? LogoScale { get; set; }
    [JsonPropertyName("logoPadding")]
    public int? LogoPadding { get; set; }
    [JsonPropertyName("captionSize")]
    public int? CaptionSize { get; set; }
    [JsonPropertyName("quietZone")]
    public int? QuietZone { get; set; }
    [JsonPropertyName("barTextSize")]
    public int? BarTextSize { get; set; }
    [JsonPropertyName("finderColor")]
    public string? FinderColor { get; set; }
    [JsonPropertyName("finderInnerColor")]
    public string? FinderInnerColor { get; set; }
    [JsonPropertyName("gradientColor")]
    public string? GradientColor { get; set; }
    [JsonPropertyName("frameColor")]
    public string? FrameColor { get; set; }
    [JsonPropertyName("captionColor")]
    public string? CaptionColor { get; set; }
    [JsonPropertyName("headline")]
    public string? Headline { get; set; }
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }
    [JsonPropertyName("schemaVersion")]
    public int? SchemaVersion { get; set; }
    [JsonPropertyName("template")]
    public string? Template { get; set; }
    [JsonPropertyName("templateVersion")]
    public int? TemplateVersion { get; set; }
    [JsonPropertyName("frame")]
    public string? Frame { get; set; }
    [JsonPropertyName("caption")]
    public string? Caption { get; set; }
    [JsonPropertyName("logoImageId")]
    public string? LogoImageId { get; set; }
}

public sealed class DocxOptions
{
    [JsonPropertyName("page")]
    public int? Page { get; set; }
    [JsonPropertyName("scale")]
    public double? Scale { get; set; }
    [JsonPropertyName("format")]
    public string? Format { get; set; }
    [JsonPropertyName("quality")]
    public int? Quality { get; set; }
    [JsonPropertyName("fitSinglePage")]
    public bool? FitSinglePage { get; set; }
    [JsonPropertyName("honorWordPageBreaks")]
    public bool? HonorWordPageBreaks { get; set; }
    [JsonPropertyName("renderHeaders")]
    public bool? RenderHeaders { get; set; }
    [JsonPropertyName("renderFooters")]
    public bool? RenderFooters { get; set; }
    [JsonPropertyName("renderFootnotes")]
    public bool? RenderFootnotes { get; set; }
    [JsonPropertyName("renderEndnotes")]
    public bool? RenderEndnotes { get; set; }
    [JsonPropertyName("repairBrokenRelationships")]
    public bool? RepairBrokenRelationships { get; set; }
    [JsonPropertyName("unwrapSdt")]
    public bool? UnwrapSdt { get; set; }
    [JsonPropertyName("collapseMcAlternate")]
    public bool? CollapseMcAlternate { get; set; }
    [JsonPropertyName("ignoreImageWrap")]
    public bool? IgnoreImageWrap { get; set; }
    [JsonPropertyName("ignoreTableWrap")]
    public bool? IgnoreTableWrap { get; set; }
    [JsonPropertyName("applyWordChrome")]
    public bool? ApplyWordChrome { get; set; }
    [JsonPropertyName("normalizeTheme")]
    public bool? NormalizeTheme { get; set; }
    [JsonPropertyName("renderChanges")]
    public bool? RenderChanges { get; set; }
}

public sealed class XlsxOptions
{
    [JsonPropertyName("page")]
    public int? Page { get; set; }
    [JsonPropertyName("scale")]
    public double? Scale { get; set; }
    [JsonPropertyName("format")]
    public string? Format { get; set; }
    [JsonPropertyName("quality")]
    public int? Quality { get; set; }
    [JsonPropertyName("engine")]
    public string? Engine { get; set; }
    [JsonPropertyName("sheet")]
    public int? Sheet { get; set; }
    [JsonPropertyName("renderMode")]
    public string? RenderMode { get; set; }
    [JsonPropertyName("range")]
    public string? Range { get; set; }
    [JsonPropertyName("showGridLines")]
    public bool? ShowGridLines { get; set; }
    [JsonPropertyName("paperSize")]
    public string? PaperSize { get; set; }
}

public sealed class PptxOptions
{
    [JsonPropertyName("page")]
    public int? Page { get; set; }
    [JsonPropertyName("scale")]
    public double? Scale { get; set; }
    [JsonPropertyName("format")]
    public string? Format { get; set; }
    [JsonPropertyName("quality")]
    public int? Quality { get; set; }
    [JsonPropertyName("engine")]
    public string? Engine { get; set; }
}

public sealed class WebsiteOptions
{
    [JsonPropertyName("page")]
    public int? Page { get; set; }
    [JsonPropertyName("scale")]
    public double? Scale { get; set; }
    [JsonPropertyName("format")]
    public string? Format { get; set; }
    [JsonPropertyName("quality")]
    public int? Quality { get; set; }
    [JsonPropertyName("engine")]
    public string? Engine { get; set; }
    [JsonPropertyName("mode")]
    public string? Mode { get; set; }
    [JsonPropertyName("width")]
    public int? Width { get; set; }
    [JsonPropertyName("height")]
    public int? Height { get; set; }
    [JsonPropertyName("selector")]
    public string? Selector { get; set; }
    [JsonPropertyName("clipX")]
    public int? ClipX { get; set; }
    [JsonPropertyName("clipY")]
    public int? ClipY { get; set; }
    [JsonPropertyName("clipWidth")]
    public int? ClipWidth { get; set; }
    [JsonPropertyName("clipHeight")]
    public int? ClipHeight { get; set; }
    [JsonPropertyName("javascriptEnabled")]
    public bool? JavascriptEnabled { get; set; }
    [JsonPropertyName("blockTrackers")]
    public bool? BlockTrackers { get; set; }
    [JsonPropertyName("autoScroll")]
    public bool? AutoScroll { get; set; }
    [JsonPropertyName("freezeAnimations")]
    public bool? FreezeAnimations { get; set; }
    [JsonPropertyName("omitBackground")]
    public bool? OmitBackground { get; set; }
    [JsonPropertyName("colorScheme")]
    public string? ColorScheme { get; set; }
    [JsonPropertyName("reducedMotion")]
    public bool? ReducedMotion { get; set; }
    [JsonPropertyName("waitUntil")]
    public string? WaitUntil { get; set; }
    [JsonPropertyName("waitMs")]
    public int? WaitMs { get; set; }
}

public sealed class ConversionCapabilities
{
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }
    [JsonPropertyName("maxUploadBytes")]
    public int? MaxUploadBytes { get; set; }
    [JsonPropertyName("maxPixels")]
    public int? MaxPixels { get; set; }
    [JsonPropertyName("maxOutputBytes")]
    public int? MaxOutputBytes { get; set; }
    [JsonPropertyName("formats")]
    public List<string>? Formats { get; set; }
    [JsonPropertyName("maxPage")]
    public int? MaxPage { get; set; }
    [JsonPropertyName("temporaryMinutes")]
    public int? TemporaryMinutes { get; set; }
    [JsonPropertyName("engines")]
    public List<ConversionEngineInfo>? Engines { get; set; }
}

public sealed class ConversionEngineInfo
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }
    [JsonPropertyName("label")]
    public string? Label { get; set; }
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
    [JsonPropertyName("maxUnits")]
    public int? MaxUnits { get; set; }
}

public sealed class ConversionJob
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("projectId")]
    public string? ProjectId { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("engine")]
    public string? Engine { get; set; }
    [JsonPropertyName("options")]
    public Dictionary<string, object?>? Options { get; set; }
    [JsonPropertyName("credits")]
    public int? Credits { get; set; }
    [JsonPropertyName("createdAt")]
    public string? CreatedAt { get; set; }
    [JsonPropertyName("expiresAt")]
    public string? ExpiresAt { get; set; }
    [JsonPropertyName("artifactId")]
    public string? ArtifactId { get; set; }
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }
    [JsonPropertyName("result")]
    public ConversionResult? Result { get; set; }
}

public sealed class ConversionResult
{
    [JsonPropertyName("bytes")]
    public int? Bytes { get; set; }
    [JsonPropertyName("width")]
    public int? Width { get; set; }
    [JsonPropertyName("height")]
    public int? Height { get; set; }
    [JsonPropertyName("pageCount")]
    public int? PageCount { get; set; }
    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }
    [JsonPropertyName("warnings")]
    public List<string>? Warnings { get; set; }
}

public sealed class DynamicCapabilities
{
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }
    [JsonPropertyName("canEdit")]
    public bool? CanEdit { get; set; }
    [JsonPropertyName("analytics")]
    public bool? Analytics { get; set; }
    [JsonPropertyName("activeCount")]
    public int? ActiveCount { get; set; }
    [JsonPropertyName("maxActiveCodes")]
    public int? MaxActiveCodes { get; set; }
    [JsonPropertyName("retentionDays")]
    public int? RetentionDays { get; set; }
}

public sealed class DynamicCode
{
    [JsonPropertyName("analyticsEnabled")]
    public bool? AnalyticsEnabled { get; set; }
    [JsonPropertyName("qrOptions")]
    public QrGenerateInput? QrOptions { get; set; }
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("projectId")]
    public string? ProjectId { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("slug")]
    public string? Slug { get; set; }
    [JsonPropertyName("redirectUrl")]
    public string? RedirectUrl { get; set; }
    [JsonPropertyName("destination")]
    public string? Destination { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("suspended")]
    public bool? Suspended { get; set; }
    [JsonPropertyName("revision")]
    public int? Revision { get; set; }
    [JsonPropertyName("createdAt")]
    public string? CreatedAt { get; set; }
    [JsonPropertyName("updatedAt")]
    public string? UpdatedAt { get; set; }
    [JsonPropertyName("recordedScans")]
    public int? RecordedScans { get; set; }
}

public sealed class DynamicCreateInput
{
    [JsonPropertyName("analyticsEnabled")]
    public bool? AnalyticsEnabled { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("destination")]
    public string? Destination { get; set; }
}

public sealed class DynamicEditInput
{
    [JsonPropertyName("analyticsEnabled")]
    public bool? AnalyticsEnabled { get; set; }
    [JsonPropertyName("qrOptions")]
    public QrGenerateInput? QrOptions { get; set; }
    [JsonPropertyName("expectedRevision")]
    public int? ExpectedRevision { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("destination")]
    public string? Destination { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

public sealed class ScanAnalytics
{
    [JsonPropertyName("days")]
    public int? Days { get; set; }
    [JsonPropertyName("total")]
    public int? Total { get; set; }
    [JsonPropertyName("retentionDays")]
    public int? RetentionDays { get; set; }
    [JsonPropertyName("bestEffort")]
    public bool? BestEffort { get; set; }
    [JsonPropertyName("series")]
    public List<ScanPoint>? Series { get; set; }
    [JsonPropertyName("devices")]
    public List<ScanBreakdown>? Devices { get; set; }
    [JsonPropertyName("browsers")]
    public List<ScanBreakdown>? Browsers { get; set; }
    [JsonPropertyName("countries")]
    public List<ScanBreakdown>? Countries { get; set; }
    [JsonPropertyName("recent")]
    public List<ScanRecent>? Recent { get; set; }
}

public sealed class ScanPoint
{
    [JsonPropertyName("date")]
    public string? Date { get; set; }
    [JsonPropertyName("count")]
    public int? Count { get; set; }
}

public sealed class ScanBreakdown
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }
    [JsonPropertyName("count")]
    public int? Count { get; set; }
}

public sealed class ScanRecent
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("createdAt")]
    public string? CreatedAt { get; set; }
    [JsonPropertyName("device")]
    public string? Device { get; set; }
    [JsonPropertyName("browser")]
    public string? Browser { get; set; }
    [JsonPropertyName("bot")]
    public bool? Bot { get; set; }
    [JsonPropertyName("country")]
    public string? Country { get; set; }
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }
    [JsonPropertyName("forwardedFor")]
    public string? ForwardedFor { get; set; }
    [JsonPropertyName("userAgent")]
    public string? UserAgent { get; set; }
    [JsonPropertyName("acceptLanguage")]
    public string? AcceptLanguage { get; set; }
    [JsonPropertyName("language")]
    public string? Language { get; set; }
    [JsonPropertyName("referer")]
    public string? Referer { get; set; }
    [JsonPropertyName("platform")]
    public string? Platform { get; set; }
    [JsonPropertyName("clientHints")]
    public string? ClientHints { get; set; }
    [JsonPropertyName("mobileHint")]
    public bool? MobileHint { get; set; }
    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; }
    [JsonPropertyName("accept")]
    public string? Accept { get; set; }
}

public sealed class CreditQuote
{
    [JsonPropertyName("replayEnabled")]
    public bool? ReplayEnabled { get; set; }
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("kind")]
    public string? Kind { get; set; }
    [JsonPropertyName("units")]
    public int? Units { get; set; }
    [JsonPropertyName("credits")]
    public int? Credits { get; set; }
    [JsonPropertyName("unitCost")]
    public int? UnitCost { get; set; }
    [JsonPropertyName("remoteUnitCost")]
    public int? RemoteUnitCost { get; set; }
    [JsonPropertyName("scheduleId")]
    public string? ScheduleId { get; set; }
    [JsonPropertyName("expiresAt")]
    public string? ExpiresAt { get; set; }
}

public sealed class CreditQuoteInput
{
    [JsonPropertyName("kind")]
    public string? Kind { get; set; }
    [JsonPropertyName("units")]
    public int? Units { get; set; }
    [JsonPropertyName("remote")]
    public bool? Remote { get; set; }
}

public sealed class ProcessingStatus
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("kind")]
    public string? Kind { get; set; }
    [JsonPropertyName("state")]
    public string? State { get; set; }
    [JsonPropertyName("resultExpiresAt")]
    public string? ResultExpiresAt { get; set; }
    [JsonPropertyName("hasResult")]
    public bool? HasResult { get; set; }
    [JsonPropertyName("failureCode")]
    public string? FailureCode { get; set; }
}

public sealed class ProcessingLookupInput
{
    [JsonPropertyName("kind")]
    public string? Kind { get; set; }
    [JsonPropertyName("key")]
    public string? Key { get; set; }
}

public sealed class ToolArtifact
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }
    [JsonPropertyName("projectId")]
    public string? ProjectId { get; set; }
    [JsonPropertyName("kind")]
    public string? Kind { get; set; }
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("bytes")]
    public int? Bytes { get; set; }
    [JsonPropertyName("sha256")]
    public string? Sha256 { get; set; }
    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }
    [JsonPropertyName("outputType")]
    public string? OutputType { get; set; }
    [JsonPropertyName("requestSnapshot")]
    public Dictionary<string, object?>? RequestSnapshot { get; set; }
    [JsonPropertyName("sourceImageId")]
    public string? SourceImageId { get; set; }
    [JsonPropertyName("createdAt")]
    public string? CreatedAt { get; set; }
    [JsonPropertyName("updatedAt")]
    public string? UpdatedAt { get; set; }
}

public sealed class OutputsSummary
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }
    [JsonPropertyName("originals")]
    public int? Originals { get; set; }
    [JsonPropertyName("transforms")]
    public int? Transforms { get; set; }
    [JsonPropertyName("images")]
    public int? Images { get; set; }
    [JsonPropertyName("qr")]
    public int? Qr { get; set; }
    [JsonPropertyName("barcode")]
    public int? Barcode { get; set; }
    [JsonPropertyName("pdf")]
    public int? Pdf { get; set; }
    [JsonPropertyName("docx")]
    public int? Docx { get; set; }
    [JsonPropertyName("xlsx")]
    public int? Xlsx { get; set; }
    [JsonPropertyName("pptx")]
    public int? Pptx { get; set; }
    [JsonPropertyName("website")]
    public int? Website { get; set; }
}

public sealed class OutputRow
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("kind")]
    public string? Kind { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("createdAt")]
    public string? CreatedAt { get; set; }
    [JsonPropertyName("bytes")]
    public int? Bytes { get; set; }
    [JsonPropertyName("thumbAvailable")]
    public bool? ThumbAvailable { get; set; }
    [JsonPropertyName("sourceImageId")]
    public string? SourceImageId { get; set; }
    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }
    [JsonPropertyName("meta")]
    public Dictionary<string, object?>? Meta { get; set; }
}

public sealed class ProcessingOptions
{
    public string? IdempotencyKey { get; init; }
    public string? QuoteId { get; init; }
    internal Dictionary<string, string> Headers()
    {
        var headers = new Dictionary<string, string>();
        if (IdempotencyKey is not null) headers["Idempotency-Key"] = IdempotencyKey;
        if (QuoteId is not null) headers["X-Quote-ID"] = QuoteId;
        return headers;
    }
}
public sealed class ArtifactSaveInput
{
    public required string Kind { get; init; }
    public required string DisplayName { get; init; }
    public required string ContentType { get; init; }
    public Dictionary<string, object?>? RequestSnapshot { get; init; }
    public string? SourceImageId { get; init; }
}

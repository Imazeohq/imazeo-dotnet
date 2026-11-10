# Imazeo (.NET)

Official **server-side** .NET SDK for Imazeo.

|                |                           |
| -------------- | ------------------------- |
| Package        | `Imazeo`                  |
| SDK version    | `1.0.0`                   |
| Default API    | `v1`                      |
| Supported APIs | `v1`                      |
| Target         | **.NET 8**                |
| Registry       | [NuGet](https://www.nuget.org/packages/Imazeo) |
| Source         | [GitHub](https://github.com/Imazeohq/imazeo-dotnet) |

> **Security:** Secret API keys (`X-API-Key`) are for trusted servers only.
> Do **not** ship this client with a secret key in browsers or Blazor WASM.

Integration contract and learning path: [GUIDE.md](GUIDE.md).

## Install

```bash
dotnet add package Imazeo
```

Requires .NET 8+. Create a project API key in the
[Imazeo Customer portal](https://app.imazeo.com).
Default API origin is `https://api.imazeo.com` (override with `BaseUrl` when needed).

## Learning path

Read the self-contained [integration guide](GUIDE.md) for project/key setup,
language examples, PDF response types, pagination, output retention, troubleshooting
and production practices.

The default `HttpClient` refuses redirects (`AllowAutoRedirect = false`) so
credentials stay on the configured API origin.

## Quick start

```csharp
using Imazeo;
using Imazeo.Models;

using var client = new ImazeoClient(new ImazeoOptions
{
    ApiKey = Environment.GetEnvironmentVariable("IMAZEO_API_KEY")!,
    BaseUrl = Environment.GetEnvironmentVariable("IMAZEO_API_ORIGIN")!,
    // ProjectId = "…", // optional; otherwise resolved via /v1/api-key/context
});

var qr = await client.Qr.GenerateAsync(new QrGenerateInput
{
    Content = "https://imazeo.com",
    Format = "png",
    Size = 256,
});
await File.WriteAllBytesAsync("qr.png", qr.Data);
```

## Authentication

```csharp
new ImazeoClient(new ImazeoOptions { ApiKey = Environment.GetEnvironmentVariable("IMAZEO_API_KEY")! });
```

Sends `X-API-Key` only. Never send `Authorization` together with an API key
(backend rejects that combination).

## Resources

| Resource            | Methods                                                                                                |
| ------------------- | ------------------------------------------------------------------------------------------------------ |
| `client.Context`    | `GetAsync()` → project/key environment                                                                 |
| `client.Images`     | `UploadAsync`, `ListAsync` (`Page`/`Limit`/`Q`), `GetAsync`, `DeleteAsync`, `DownloadAsync`            |
| `client.Transforms` | `CreateAsync`, `ListAsync`, `GetAsync`, `DownloadAsync` (`ifNoneMatch` → `NotModified`), `DeleteAsync` |
| `client.Qr`         | `GenerateAsync`                                                                                        |
| `client.Barcodes`   | `GenerateAsync`                                                                                        |
| `client.Pdf`        | `InfoAsync`, `RenderAsync`, `MergeAsync`, `ExtractAsync`                                               |

### Upload + transform

```csharp
var image = await client.Images.UploadAsync(bytes, filename: "photo.jpg", contentType: "image/jpeg");

var output = await client.Transforms.CreateAsync(new TransformRequest
{
    ImageId = image.Id,
    Mode = TransformModes.Ephemeral,
    Transform = new TransformSpec { Width = 1200, Format = "webp", Quality = 85 },
});
// output.Data → byte[]
```

### Remote URL transform

```csharp
var output = await client.Transforms.CreateAsync(new TransformRequest
{
    Url = "https://example.com/image.jpg",
    Transform = new TransformSpec { Width = 640, Format = "jpeg" },
});
```

### PDF

```csharp
var info = await client.Pdf.InfoAsync(pdfBytes);
var page = await client.Pdf.RenderAsync(pdfBytes, new PdfRenderOptions { Pages = "1", Format = "png" });
var merged = await client.Pdf.MergeAsync(new UploadSource[] { pdfA, pdfB });
var extracted = await client.Pdf.ExtractAsync(pdfBytes, new PdfExtractOptions { Pages = "1,2" });
```

## Errors

```csharp
using Imazeo.Errors;

try
{
    await client.Images.GetAsync(id);
}
catch (RateLimitError ex)
{
    Console.Error.WriteLine($"{ex.Code} {ex.RequestId}");
}
catch (ImazeoError ex)
{
    Console.Error.WriteLine($"{ex.Status} {ex.Code} {ex.Message}");
}
```

## Configuration

| Option               | Default                  | Notes                                         |
| -------------------- | ------------------------ | --------------------------------------------- |
| `ApiKey`             | required                 | Secret key                                    |
| `ProjectId`          | from context             | Path project                                  |
| `BaseUrl`            | `https://api.imazeo.com` | Origin only; `/v1` stripped if present        |
| `ApiVersion`         | `v1`                     | Allowlisted                                   |
| `TimeoutMs`          | `60000`                  | PDF uses 120s; QR/barcode/context 30s         |
| `AppName`            | —                        | Appended to User-Agent                        |
| `HttpClient`         | new owned client         | Prefer a shared client / `IHttpClientFactory` |
| `HttpMessageHandler` | —                        | Tests and custom handlers                     |

### HttpClient guidance

Do **not** create a new `HttpClient` per request. Either:

- let `ImazeoClient` own one long-lived instance, or
- pass a shared `HttpClient` from `IHttpClientFactory` via `ImazeoOptions.HttpClient` (the SDK will not dispose it).

Custom handlers should keep redirects disabled so `X-API-Key` stays on the configured origin.

## API version vs package version

- **API** path version: `v1` (configured via `ApiVersion`)
- **SDK** SemVer: NuGet package → `1.0.0`

`ImazeoClient.ApiCompatibility` reports supported API versions.
User-Agent: `Imazeo-DotNet/1.0.0`.

## Not included

Portal JWT APIs (projects CRUD, API-key management, usage, plans, tickets, auth),
admin APIs, tool artifacts, project outputs, and browser secret-key usage.

## Development

```bash
dotnet test Imazeo.sln
dotnet build Imazeo.sln -c Release
```

Examples (require a live API key):

```bash
dotnet run --project examples/QrExample
dotnet run --project examples/UploadTransform -- path/to/image.jpg
```

## License

MIT

## Barcode and PDF notes

Barcode generation requires `type` and string `content`. Optional `format` is
`png` or `svg` (default `png`); `includeText` defaults to `true`.
PDF merge uploads files in order as repeated multipart `file` parts.
Omit optional fields to use API defaults. Integer fields such as barcode scale
and image hue must be integers.

```csharp
await client.Barcodes.GenerateAsync(new BarcodeGenerateInput { Type = BarcodeTypes.Code128, Content = "IMAZEO-2026", Format = "svg", IncludeText = false });
```

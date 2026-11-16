using System.Text.Json;
using Imazeo.Http;
using Imazeo.Models;
namespace Imazeo.Resources;
public sealed class ConversionsResource : ProjectResource
{
    internal ConversionsResource(HttpTransport http, Func<CancellationToken, Task<string>> projectId) : base(http, projectId) {}
    public Task<ConversionCapabilities> CapabilitiesAsync(CancellationToken cancellationToken = default) => Json<ConversionCapabilities>(HttpMethod.Get, new[] { "conversions","capabilities" }, null, null, null, cancellationToken);
    public Task<ConversionJob> GetAsync(string id, CancellationToken cancellationToken = default) => Json<ConversionJob>(HttpMethod.Get, new[] { "conversions",id }, null, null, null, cancellationToken);
    public Task<ConversionJob> CancelAsync(string id, CancellationToken cancellationToken = default) => Json<ConversionJob>(HttpMethod.Post, new[] { "conversions",id,"cancel" }, null, null, null, cancellationToken);
    public Task<ToolArtifact> SaveAsync(string id, CancellationToken cancellationToken = default) => Json<ToolArtifact>(HttpMethod.Post, new[] { "conversions",id,"save" }, null, null, null, cancellationToken);
    public Task<BinaryResult> DownloadAsync(string id, CancellationToken cancellationToken = default) => Binary(new[] { "conversions",id,"download" }, null, cancellationToken);
    public Task<List<ConversionJob>> ListAsync(string engine = "docx", CancellationToken cancellationToken = default) => Json<List<ConversionJob>>(HttpMethod.Get, new[] {"conversions"}, query: new {engine}, ct:cancellationToken);
    public Task<ConversionJob> LookupAsync(string key, CancellationToken cancellationToken = default) => Json<ConversionJob>(HttpMethod.Get, new[] {"conversions","lookup"}, query: new {key}, ct:cancellationToken);
    public Task<ConversionJob> WebsiteAsync(string url, ProcessingOptions request, WebsiteOptions? options = null, CancellationToken cancellationToken = default) => Json<ConversionJob>(HttpMethod.Post, new[] {"conversions","website"}, new {url}, options, request, cancellationToken);
    private async Task<ConversionJob> Upload(string engine, UploadSource source, ProcessingOptions request, object? options, CancellationToken ct) {
        using var form = new MultipartFormDataContent();
        var prepared = MultipartHelper.PrepareUpload(source, "document." + engine, "application/octet-stream");
        MultipartHelper.AppendFile(form,"file",prepared);
        var parts = engine == "docx" ? new[] {"conversions"} : new[] {"conversions",engine};
        return (await Http.RequestMultipartJsonAsync<ConversionJob>(HttpMethod.Post,await Url(parts,options,ct).ConfigureAwait(false),form,uploadLength: source.Data.Length,headers: request.Headers(), cancellationToken: ct).ConfigureAwait(false)).Data;
    }
    public Task<ConversionJob> DOCXAsync(UploadSource source, ProcessingOptions request, DocxOptions? options = null, CancellationToken cancellationToken = default) => Upload("docx",source,request,options,cancellationToken);
    public Task<ConversionJob> XLSXAsync(UploadSource source, ProcessingOptions request, XlsxOptions? options = null, CancellationToken cancellationToken = default) => Upload("xlsx",source,request,options,cancellationToken);
    public Task<ConversionJob> PPTXAsync(UploadSource source, ProcessingOptions request, PptxOptions? options = null, CancellationToken cancellationToken = default) => Upload("pptx",source,request,options,cancellationToken);
}

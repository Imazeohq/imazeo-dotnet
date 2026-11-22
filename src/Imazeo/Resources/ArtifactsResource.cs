using System.Text.Json;
using Imazeo.Http;
using Imazeo.Models;
namespace Imazeo.Resources;
public sealed class ArtifactsResource : ProjectResource
{
    internal ArtifactsResource(HttpTransport http, Func<CancellationToken, Task<string>> projectId) : base(http, projectId) {}
    public Task<PagedResult<ToolArtifact>> ListAsync(Dictionary<string, object?>? query = null, CancellationToken cancellationToken = default) => Page<ToolArtifact>(new[] { "tools","artifacts" }, query, cancellationToken);
    public Task<ToolArtifact> GetAsync(string id, CancellationToken cancellationToken = default) => Json<ToolArtifact>(HttpMethod.Get, new[] { "tools","artifacts",id }, null, null, null, cancellationToken);
    public Task<MessageResult> DeleteAsync(string id, CancellationToken cancellationToken = default) => Json<MessageResult>(HttpMethod.Delete, new[] { "tools","artifacts",id }, null, null, null, cancellationToken);
    public Task<BinaryResult> DownloadAsync(string id, CancellationToken cancellationToken = default) => Binary(new[] { "tools","artifacts",id,"content" }, null, cancellationToken);
    public async Task<ToolArtifact> SaveAsync(UploadSource source, ArtifactSaveInput input, CancellationToken cancellationToken = default) {
        using var form = new MultipartFormDataContent();
        MultipartHelper.AppendFile(form,"file",MultipartHelper.PrepareUpload(source,"output.bin",input.ContentType));
        form.Add(new StringContent(input.Kind),"kind"); form.Add(new StringContent(input.DisplayName),"displayName"); form.Add(new StringContent(input.ContentType),"contentType");
        if(input.RequestSnapshot is not null)form.Add(new StringContent(JsonSerializer.Serialize(input.RequestSnapshot)),"requestSnapshot");
        if(input.SourceImageId is not null)form.Add(new StringContent(input.SourceImageId),"sourceImageId");
        return (await Http.RequestMultipartJsonAsync<ToolArtifact>(HttpMethod.Post,await Url(new[] {"tools","artifacts"},null,cancellationToken).ConfigureAwait(false),form,uploadLength:source.Data.Length,cancellationToken:cancellationToken).ConfigureAwait(false)).Data;
    }
}

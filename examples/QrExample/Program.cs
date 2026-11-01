using Imazeo;
using Imazeo.Models;

/// <summary>
/// Minimal server-side example.
/// Requires: IMAZEO_API_KEY, optional IMAZEO_BASE_URL, IMAZEO_PROJECT_ID
/// </summary>
static class Program
{
    static async Task Main()
    {
        var apiKey = Environment.GetEnvironmentVariable("IMAZEO_API_KEY")
            ?? throw new InvalidOperationException("Set IMAZEO_API_KEY");

        using var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = apiKey,
            BaseUrl = Environment.GetEnvironmentVariable("IMAZEO_BASE_URL") ?? "http://localhost:3001",
            ProjectId = Environment.GetEnvironmentVariable("IMAZEO_PROJECT_ID"),
        });

        var ctx = await client.Context.GetAsync();
        Console.WriteLine($"project {ctx.ProjectId} env {ctx.Environment}");

        var qr = await client.Qr.GenerateAsync(new QrGenerateInput
        {
            Content = "https://imazeo.com",
            Size = 256,
            Format = "png",
        });
        await File.WriteAllBytesAsync("qr.png", qr.Data);
        Console.WriteLine($"wrote qr.png {qr.ContentType}");
    }
}

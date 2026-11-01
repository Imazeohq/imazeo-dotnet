using Imazeo;
using Imazeo.Models;

static class Program
{
    static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("IMAZEO_API_KEY")
            ?? throw new InvalidOperationException("Set IMAZEO_API_KEY");
        if (args.Length < 1)
            throw new InvalidOperationException("Usage: UploadTransform <image-path>");

        using var client = new ImazeoClient(new ImazeoOptions
        {
            ApiKey = apiKey,
            BaseUrl = Environment.GetEnvironmentVariable("IMAZEO_BASE_URL") ?? "http://localhost:3001",
            ProjectId = Environment.GetEnvironmentVariable("IMAZEO_PROJECT_ID"),
        });

        var bytes = await File.ReadAllBytesAsync(args[0]);
        var image = await client.Images.UploadAsync(new UploadSource
        {
            Data = bytes,
            Filename = "input.bin",
            ContentType = "application/octet-stream",
        });
        Console.WriteLine($"uploaded {image.Id} {image.Metadata.Format} {image.Metadata.Width}x{image.Metadata.Height}");

        var transformed = await client.Transforms.CreateAsync(new TransformRequest
        {
            ImageId = image.Id,
            Mode = TransformModes.Ephemeral,
            Transform = new TransformSpec { Width = 800, Format = "webp", Quality = 80 },
        });
        await File.WriteAllBytesAsync("out.webp", transformed.Data);
        Console.WriteLine($"wrote out.webp {transformed.TransformationId}");
    }
}

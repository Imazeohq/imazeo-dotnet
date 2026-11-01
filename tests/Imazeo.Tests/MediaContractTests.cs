using System.Text.Json;
using Imazeo.Models;
using Xunit;

namespace Imazeo.Tests;

public class MediaContractTests
{
    [Fact]
    public void Barcode_serializes_canonical_names_and_false()
    {
        var body = JsonSerializer.SerializeToElement(new BarcodeGenerateInput
        {
            Type = "ean8", Content = "0123456", Format = "svg", IncludeText = false,
        });
        Assert.Equal("ean8", body.GetProperty("type").GetString());
        Assert.Equal("0123456", body.GetProperty("content").GetString());
        Assert.Equal("svg", body.GetProperty("format").GetString());
        Assert.False(body.GetProperty("includeText").GetBoolean());
        Assert.False(body.TryGetProperty("output", out _));
        Assert.False(body.TryGetProperty("data", out _));
        Assert.False(body.TryGetProperty("includetext", out _));
    }
}

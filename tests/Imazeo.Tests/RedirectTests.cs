using System.Net;
using System.Net.Sockets;
using System.Text;
using Xunit;

namespace Imazeo.Tests;

public class RedirectTests
{
    [Fact]
    public async Task Default_transport_does_not_forward_project_key_across_redirect()
    {
        using var stop = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        using var destination = new TcpListener(IPAddress.Loopback, 0);
        using var origin = new TcpListener(IPAddress.Loopback, 0);
        destination.Start();
        origin.Start();
        var destinationPort = ((IPEndPoint)destination.LocalEndpoint).Port;
        var originPort = ((IPEndPoint)origin.LocalEndpoint).Port;
        var destinationRequests = 0;
        string? originHeaders = null;
        var target = ServeOne(destination,
            "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nConnection: close\r\n\r\n{\"success\":true,\"data\":{\"projectId\":\"p\",\"keyId\":\"k\"}}",
            _ => Interlocked.Increment(ref destinationRequests), stop.Token);
        var source = ServeOne(origin,
            $"HTTP/1.1 307 Temporary Redirect\r\nLocation: http://127.0.0.1:{destinationPort}/\r\nContent-Length: 0\r\nConnection: close\r\n\r\n",
            headers => originHeaders = headers, stop.Token);
        try
        {
            using var client = new ImazeoClient(new ImazeoOptions
            {
                ApiKey = "secret-key",
                BaseUrl = $"http://127.0.0.1:{originPort}",
            });
            await Assert.ThrowsAnyAsync<Exception>(() => client.Context.GetAsync(stop.Token));
            await source;
            Assert.Contains("secret-key", originHeaders!);
            Assert.Equal(0, Volatile.Read(ref destinationRequests));
        }
        finally
        {
            await stop.CancelAsync();
            try { await Task.WhenAll(source, target); }
            catch (OperationCanceledException) { }
        }
    }

    private static async Task ServeOne(TcpListener listener, string response,
        Action<string> received, CancellationToken cancellationToken)
    {
        using var connection = await listener.AcceptTcpClientAsync(cancellationToken);
        await using var stream = connection.GetStream();
        using var reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true);
        var headers = new StringBuilder();
        while (await reader.ReadLineAsync(cancellationToken) is { Length: > 0 } line)
            headers.AppendLine(line);
        received(headers.ToString());
        await stream.WriteAsync(Encoding.UTF8.GetBytes(response), cancellationToken);
    }
}

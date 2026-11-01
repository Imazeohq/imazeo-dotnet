# SDK-5 plan — .NET

**Status: implemented (1.0.0)**

1. Class library targeting modern .NET LTS (`net8.0`) — done
2. `ImazeoClient` + `ImazeoOptions` — done
3. Stream-friendly multipart + required `X-Upload-Length` — done
4. xUnit tests with `HttpMessageHandler` mocks — done
5. Prefer shared `HttpClient` / `IHttpClientFactory` guidance in README — done
6. Default client refuses redirects — done

Scope: API-key resources only. No admin/JWT/tool-artifacts/outputs.

Reference: `../sdk-typescript/` and `../docs/SDK_BEHAVIOR.md`.
Publish by tagging `v1.0.0` on `Imazeohq/imazeo-dotnet` and pushing `Imazeo` to NuGet.

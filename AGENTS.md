# Imazeo — agent notes

Independent public NuGet package (`Imazeo`).

- Contract: published API `v1` (see [GUIDE.md](GUIDE.md))
- Scope: Context, Images, Transforms, QR, Barcodes, PDF only
- Do not add admin or customer JWT portal APIs
- Do not publish without maintainer approval — see [RELEASE.md](RELEASE.md)
- Never commit NuGet API keys; publish via Trusted Publishing (`NUGET_USER` secret only)
- Release: `$env:RELEASE_MODE="true"; npm run release` (syncs csproj + `ImazeoVersion.cs`); Actions publishes to NuGet

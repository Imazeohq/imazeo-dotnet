# SDK release procedure (`Imazeo` on NuGet)

Independent repository: [Imazeohq/imazeo-dotnet](https://github.com/Imazeohq/imazeo-dotnet).  
NuGet package: [`Imazeo`](https://www.nuget.org/packages/Imazeo).  
Supported API prefix: `v1`. SemVer lives in `Imazeo.csproj`, `ImazeoVersion.SdkVersion`, and `package.json`
(kept in sync by the release script).

Never overwrite a published version; ship a patch instead.

## One-time: Trusted Publishing

On nuget.org → **Trusted Publishing** → add policy:

| Field | Value |
| --- | --- |
| Repository owner | `Imazeohq` |
| Repository | `imazeo-dotnet` |
| Workflow file | `publish.yml` |
| Environment | leave empty |
| Package / glob | `Imazeo` |

In GitHub → **Settings → Secrets** set `NUGET_USER` to your nuget.org **profile username** (not email).

## Release (PowerShell)

Commit your work on `main` first (clean working tree required). One-time: `npm install`.

```powershell
$env:RELEASE_MODE = "true"
npm run release
```

You will be prompted for version bump and GitHub Release.  
This syncs versions, commits, tags `vX.Y.Z`, and pushes.  
**Publish NuGet** then pushes to nuget.org with OIDC.

## Release from GitHub only

If the version bump is already on `main`:

1. **Releases** → Draft a new release → tag `vX.Y.Z` → Publish  
   or
2. **Actions** → **Publish NuGet** → **Run workflow**

## After publish

Verify: https://www.nuget.org/packages/Imazeo/

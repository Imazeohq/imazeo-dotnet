# SDK release procedure (`Imazeo` on NuGet)

Independent repository: [Imazeohq/imazeo-dotnet](https://github.com/Imazeohq/imazeo-dotnet).  
NuGet package: [`Imazeo`](https://www.nuget.org/packages/Imazeo).  
Supported API prefix: `v1`. SemVer lives in this repo (`Imazeo.csproj` + `ImazeoVersion.SdkVersion`)
and is published from git tag `v*` via GitHub Actions.

Never overwrite a published version; ship a patch instead.

## One-time: Trusted Publishing

On nuget.org → **Trusted Publishing** → add policy:

| Field | Value |
| --- | --- |
| Repository owner | `Imazeohq` |
| Repository | `imazeo-dotnet` |
| Workflow file | `publish.yml` |
| Environment | leave empty |

In GitHub → **Settings → Secrets** set `NUGET_USER` to your nuget.org **profile username** (not email).

## Pre-publish checks

```bash
python scripts/check-release.py
dotnet test -c Release
dotnet list package --vulnerable --include-transitive
dotnet pack src/Imazeo/Imazeo.csproj -c Release -o artifacts
```

## Publish

1. Bump `<Version>` in `src/Imazeo/Imazeo.csproj` and `SdkVersion` in `src/Imazeo/ImazeoVersion.cs` together.
2. Commit on `main`, then tag and push:

```bash
git tag -a v1.0.1 -m "Imazeo 1.0.1"
git push origin v1.0.1
```

3. Actions → **Publish NuGet** packs and pushes with OIDC.

Or: Actions → **Publish NuGet** → **Run workflow**.

## After publish

Verify: https://www.nuget.org/packages/Imazeo/  
If the release is bad, unlist/deprecate on NuGet per policy and ship a reviewed patch.

# SDK release procedure (`Imazeo` on NuGet)

Independent repository: [Imazeohq/imazeo-dotnet](https://github.com/Imazeohq/imazeo-dotnet).  
NuGet package: [`Imazeo`](https://www.nuget.org/packages/Imazeo).  
Supported API prefix: `v1`. SemVer lives in this repo (`Imazeo.csproj` + `ImazeoVersion.SdkVersion`)
and is published from git tag `v1.0.0` plus `dotnet nuget push`.

Do not publish automatically from CI until a protected release workflow is added.
Never overwrite a published version; ship a patch instead.

## Pre-publish checks

```powershell
cd D:\projects\imazeo\sdk-dotnet
python scripts\check-release.py
dotnet test -c Release
dotnet list package --vulnerable --include-transitive
dotnet pack src/Imazeo/Imazeo.csproj -c Release -o artifacts
```

Confirm `artifacts/Imazeo.1.0.0.nupkg` contents (README/GUIDE/LICENSE, no secrets).

## Manual publish (first release `1.0.0`)

1. Create empty public GitHub repo `Imazeohq/imazeo-dotnet` and push this tree to `main`.
2. Tag and push: `git tag -a v1.0.0 -m "Imazeo 1.0.0"` then `git push origin main` and `git push origin v1.0.0`.
3. Create a [nuget.org](https://www.nuget.org) account (GitHub login recommended) and create an **API key** with push scope for new packages / `Imazeo`.
4. Publish:
   ```powershell
   dotnet nuget push artifacts\Imazeo.1.0.0.nupkg --api-key <NUGET_API_KEY> --source https://api.nuget.org/v3/index.json
   ```
5. Verify: https://www.nuget.org/packages/Imazeo/1.0.0 and `dotnet add package Imazeo --version 1.0.0`

## After publish

Smoke-install:

```powershell
mkdir $env:TEMP\imazeo-dotnet-smoke
cd $env:TEMP\imazeo-dotnet-smoke
dotnet new console -n Smoke
cd Smoke
dotnet add package Imazeo --version 1.0.0
```

If the release is bad, unlist/deprecate on NuGet per policy and ship a reviewed
patch — do not reuse `1.0.0`.

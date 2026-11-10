"""Validate the SDK's independent SemVer, version constants and release metadata."""
import os
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = 'src/Imazeo/Imazeo.csproj'
MANIFEST_PATTERN = '<Version>([^<]+)</Version>'
CONSTANT = 'src/Imazeo/ImazeoVersion.cs'
CONSTANT_PATTERN = 'SdkVersion\\s*=\\s*"([^"]+)"'

version = re.search(MANIFEST_PATTERN, (ROOT / MANIFEST).read_text(), re.MULTILINE).group(1)
constant = re.search(CONSTANT_PATTERN, (ROOT / CONSTANT).read_text()).group(1)
assert re.fullmatch(r"(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-[0-9A-Za-z.-]+)?", version), "Invalid SemVer"
assert version == constant, "Package metadata and SDK User-Agent version disagree"
pkg_path = ROOT / "package.json"
if pkg_path.is_file():
    pkg_version = re.search(r'"version"\s*:\s*"([^"]+)"', pkg_path.read_text()).group(1)
    assert pkg_version == version, "package.json version and csproj Version disagree"
if os.environ.get("GITHUB_REF_TYPE") == "tag":
    assert os.environ.get("GITHUB_REF_NAME") == "v" + version, "Tag and SDK version disagree"
for required in ("LICENSE", "README.md", "GUIDE.md", "CHANGELOG.md"):
    assert (ROOT / required).is_file(), "Missing package document: " + required
assert "MIT License" in (ROOT / "LICENSE").read_text(), "Review SDK licensing"
csproj = (ROOT / MANIFEST).read_text()
assert "https://github.com/Imazeohq/imazeo-dotnet" in csproj, "RepositoryUrl must point at Imazeohq/imazeo-dotnet"
assert "<PackageId>Imazeo</PackageId>" in csproj, "PackageId must be Imazeo"
print("SDK release metadata valid:", version)

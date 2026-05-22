# New Extension Checklist

When forking this template repository to create a new Azure Functions extension, you **MUST** customize the following items. Failing to do so will result in naming conflicts, signing issues, and broken pipelines.

---

## 🔑 1. Generate a Unique `key.snk`

Each extension **MUST** have its own strong-name signing key. Never share keys between extensions.

```bash
# Using .NET SDK (cross-platform)
dotnet tool install --global dotnet-sn
sn -k eng/res/key.snk

# Or using PowerShell
$rsa = [System.Security.Cryptography.RSA]::Create(2048)
[System.IO.File]::WriteAllBytes("eng/res/key.snk", $rsa.ExportRSAPrivateKey())
$rsa.Dispose()
```

> ⚠️ The `key.snk` in this template is specific to the HelloWorld extension. You must regenerate it.

---

## 📦 2. Update Package Names

Replace all instances of `HelloWorld` with your extension name in these locations:

| Location | What to Change |
| --- | --- |
| `src/Microsoft.Azure.Functions.Extensions.HelloWorld/` | Folder name |
| `src/Microsoft.Azure.Functions.Worker.Extensions.HelloWorld/` | Folder name |
| `test/Microsoft.Azure.Functions.Extensions.HelloWorld.Tests/` | Folder name |
| `Extensions.HelloWorld.csproj` → `<AssemblyName>` | Package assembly name |
| `Extensions.HelloWorld.csproj` → `<Description>` | Package description |
| `Worker.Extensions.HelloWorld.csproj` → `<AssemblyName>` | Package assembly name |
| `Worker.Extensions.HelloWorld.csproj` → `<Description>` | Package description |
| `Worker.Extensions.HelloWorld.csproj` → `<WebJobsReference>` | Path to host csproj |
| `*.sln` | Solution file name and project references |
| `eng/build/Release.props` → `<RepositoryUrl>` | GitHub repository URL |
| `eng/build/RepositoryInfo.targets` | AzDO URL patterns for SourceLink |
| `README.md` | Project documentation |

---

## 🔢 3. Set Up Versioning

Versioning is centralized in `eng/build/Version.props`:

```xml
<!-- Set your initial version -->
<VersionPrefix Condition="'$(VersionPrefix)' == ''">0.1.0</VersionPrefix>
<VersionSuffix Condition="'$(VersionSuffix)' == ''">alpha</VersionSuffix>
```

**Important:**

- Do **NOT** set `<Version>` in individual `.csproj` files — it's managed centrally
- Use `VersionPrefix` for the semantic version (e.g., `0.1.0`, `1.0.0`)
- Use `VersionSuffix` for prerelease tags (e.g., `alpha`, `beta`, `rc.1`)
- Set `VersionSuffix` to empty for stable releases
- For per-project version overrides, create a `Directory.Version.props` in the project directory

### Version format

- **Local dev:** `{VersionPrefix}-{VersionSuffix}.dev` (e.g., `0.1.0-alpha.dev`)
- **CI (PR):** `{VersionPrefix}-{VersionSuffix}.pr.{BuildNumber}.{Counter}` (e.g., `0.1.0-alpha.pr.26272.3`)
- **CI (main):** `{VersionPrefix}-{VersionSuffix}.ci.{BuildNumber}.{Counter}`
- **Release (tag):** `{VersionPrefix}` (e.g., `0.1.0` for stable, `0.1.0-alpha` for prerelease)

---

## 🚀 4. Configure Pipelines

> **Note:** The release pipeline files (`official-release-host.yml`, `official-release-worker.yml`,
> and `release-packages-*.yml`) are specific to the Azure Functions engineering system (1ES,
> partner drops, NuGet signing). If you are **not** on the azfunc team, replace these with your
> own release pipeline that publishes to your NuGet feed. At minimum you need:
>
> - A pipeline that runs `dotnet pack -c Release`
> - A publish step (`dotnet nuget push`) to your feed
> - Appropriate approval gates for production releases

### `eng/ci/templates/variables/build.yml`

```yaml
variables:
  - name: project
    value: YourExtension.sln  # ← Update solution name
```

### `eng/ci/official-release-host.yml` & `official-release-worker.yml`

```yaml
resources:
  pipelines:
    - pipeline: build
      source: your-extension.official  # ← Update pipeline source name
```

### `eng/ci/templates/jobs/build-artifacts.yml`

Update all references to:

- `out/bin/Worker.Extensions.HelloWorld/` → your worker output folder
- `Microsoft.Azure.Functions.Worker.Extensions.HelloWorld.dll` → your worker DLL name
- `out/bin/Extensions.HelloWorld/` → your host output folder
- `Microsoft.Azure.Functions.Extensions.HelloWorld.dll` → your host DLL name

### `eng/ci/templates/jobs/release-packages-host.yml` & `release-packages-worker.yml`

Update:

- Package name patterns (`**/Microsoft.Azure.Functions.Extensions.HelloWorld.*.nupkg`)
- `approvers` to your team's security group (e.g., `'[TEAM FOUNDATION]\Your Team Name'`)
- `targetFolder` path for partner drops

### Pool names (if not using shared azfunc pools)

- `eng/ci/public-build.yml` → `pool.name`
- `eng/ci/official-build.yml` → `pool.name`

---

## 📋 5. Other Customizations

| File | What to Update |
| --- | --- |
| `NuGet.config` | Add your team's private feed if needed |
| `.github/ISSUE_TEMPLATE/bug_report.md` | Extension-specific environment fields |
| `README.md` | Full project documentation |

---

## ✅ Validation

After making all changes, verify:

```bash
# Build should succeed
dotnet build -c Release

# Tests should pass
dotnet test -c Release

# Package should produce correctly-named nupkg
dotnet pack -c Release -o ./packages
ls ./packages/*.nupkg
```

The output `.nupkg` files should be named with your extension's package ID and version.

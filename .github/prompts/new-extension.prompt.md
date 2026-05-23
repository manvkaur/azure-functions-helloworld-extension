# Create New Azure Functions Extension

You are helping the user scaffold a new Azure Functions extension from this template repository. Follow these steps in order, gathering information before making changes.

## Step 1: Gather Requirements

Ask the following questions **one at a time**, waiting for each answer before proceeding:

### Q1 — Extension Name

> What is the name of your extension?
> Use concise PascalCase matching the service name (e.g., `Mcp`, `ServiceBus`, `EventGrid`, `Redis`).

### Q2 — Host SDK Layer

> Which host SDK layer does your extension build on?
>
> 1. **`Microsoft.Azure.WebJobs.Extensions`** — Use when building on WebJobs SDK infrastructure (e.g., `IAsyncCollector`, `ITriggeredFunctionExecutor`). Most existing extensions use this.
> 2. **`Microsoft.Azure.Functions.Extensions`** — Use for the newer Functions-specific extensibility model without a direct WebJobs SDK dependency.

### Q3 — Binding Types

> Which binding types will your extension support?
>
> 1. Trigger only
> 2. Input binding only
> 3. Output binding only
> 4. Trigger + Input binding
> 5. Trigger + Output binding
> 6. Trigger + Input + Output binding

### Q4 — Target Service

> What Azure service or external system does this extension connect to?
> (e.g., Azure Service Bus, Redis, a custom REST API, Azure AI Search)

### Q5 — Configuration Settings

> What connection or configuration settings does your extension need?
> List them comma-separated (e.g., `ConnectionString`, `Endpoint`, `ApiKey`).

### Q6 — Team Context

> Are you on the Azure Functions engineering team (azfunc)?
>
> 1. **Yes** — Will use 1ES pipelines, partner drops, and NuGet signing
> 2. **No** — Will need a custom release pipeline for NuGet publishing

## Step 2: Execute Changes

After gathering all answers, perform the following using the checklist at `docs/new-extension-checklist.md` as your guide:

### 2.1 — Generate Strong-Name Key

```powershell
$rsa = [System.Security.Cryptography.RSA]::Create(2048)
[System.IO.File]::WriteAllBytes("eng/res/key.snk", $rsa.ExportRSAPrivateKey())
$rsa.Dispose()
```

### 2.2 — Rename Projects and Namespaces

Replace all instances of `HelloWorld` with the user's `<ExtensionName>`:

- `src/Microsoft.Azure.Functions.Extensions.HelloWorld/` → folder and `.csproj`
- `src/Microsoft.Azure.Functions.Worker.Extensions.HelloWorld/` → folder and `.csproj`
- `test/Microsoft.Azure.Functions.Extensions.HelloWorld.Tests/` → folder and `.csproj`
- Solution file (`.sln`) — update project references and solution name
- All `namespace` declarations and `using` statements in `.cs` files
- `eng/build/Release.props` — repository URL
- `eng/build/RepositoryInfo.targets` — AzDO URL patterns

Use the host SDK prefix from Q2 for the host project package name.

### 2.3 — Scaffold Binding Classes

Based on Q3 (binding types selected), keep or remove:

| Binding Type | Host Classes to Keep | Worker Attributes to Keep |
| --- | --- | --- |
| Trigger | `*TriggerAttribute`, `*TriggerBinding`, `*TriggerBindingProvider`, `*Listener` | `*TriggerAttribute` (worker) |
| Input | `*Attribute` (input), `*Binding`, `*BindingProvider` | `*InputAttribute` (worker) |
| Output | `*Attribute` (output), `*AsyncCollector`, `*BindingProvider` (output) | `*OutputAttribute` (worker) |

Remove unused binding classes and update `ExtensionConfigProvider` registration.

### 2.4 — Update Configuration

- Update attribute properties to match Q5 (configuration settings)
- Add connection resolution logic in binding providers
- Update `host.json` logging namespace

### 2.5 — Set Version

In `eng/build/Version.props`:

```xml
<VersionPrefix>0.1.0</VersionPrefix>
<VersionSuffix>alpha</VersionSuffix>
```

### 2.6 — Configure Pipelines

**If azfunc team (Q6 = Yes):**

- Update `eng/ci/templates/variables/build.yml` with new solution name
- Update release pipeline source names and package patterns
- Update approvers to team security group

**If not azfunc team (Q6 = No):**

- Replace `official-release-*.yml` and `release-packages-*.yml` with a simple NuGet publish workflow
- Keep `public-build.yml` and `official-build.yml` as CI templates (update pool names)

### 2.7 — Update Documentation

- Rewrite `README.md` with extension-specific content (keep structure)
- Update sample function apps to demonstrate the new bindings
- Reference the target service from Q4 in docs

## Step 3: Validate

Run the following to confirm everything works:

```bash
dotnet build -c Release
dotnet test -c Release
dotnet pack -c Release -o ./packages
```

Verify that:

- Build produces 0 errors, 0 warnings
- All tests pass
- `.nupkg` files are named correctly with the new extension name

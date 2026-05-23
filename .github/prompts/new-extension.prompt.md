# Create New Azure Functions Extension

You are helping the user scaffold a new Azure Functions extension from this template repository. Follow these steps in order, gathering information before making changes.

## Step 1: Gather Requirements

Ask the following questions **one at a time**, waiting for each answer before proceeding:

### Q1 — Extension Name

> What is the name of your extension?
> Use concise PascalCase matching the service name (e.g., `Mcp`, `ServiceBus`, `EventGrid`, `Redis`).

### Q2 — Host Package Naming

**Default: Use `Microsoft.Azure.Functions.Extensions.<Name>`** — this is the recommended naming for all new extensions.

Inform the user:

> Your host package will be named **`Microsoft.Azure.Functions.Extensions.<Name>`**.
>
> **About the WebJobs SDK dependency:** All host extensions — regardless of package name — depend on `Microsoft.Azure.WebJobs` as a NuGet reference. This package provides the host extensibility interfaces (`IExtensionConfigProvider`, `ITriggerBinding`, `IListener`, `IAsyncCollector<T>`, etc.). This is the foundational layer that the Azure Functions host is built on. Your extension implements these interfaces to integrate with the host runtime.
>
> **The package name is just a naming convention:**
>
> | Convention | When to use |
> | --- | --- |
> | `Microsoft.Azure.Functions.Extensions.<Name>` | ✅ All **new** extensions (recommended) |
> | `Microsoft.Azure.WebJobs.Extensions.<Name>` | ❌ Legacy naming — only used by older extensions (ServiceBus, CosmosDB, EventGrid) that can't rename without breaking consumers |
>
> Both patterns reference the same `Microsoft.Azure.WebJobs` SDK and implement the same interfaces. The only difference is what you name your published NuGet package.

No question needed here — just inform and move on. Only ask if they explicitly say they're extending an existing `WebJobs.Extensions.*` package (in which case, match the parent's namespace for consistency).

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

### Q5 — Binding Properties

For **each** binding type selected in Q3, ask the user to define its attribute properties. Present this as a table they fill in:

> For your **[Trigger/Input/Output]** binding, list the properties that go on the attribute.
> For each property, specify:
>
> | Property Name | Type | Required? | Supports `%appSetting%` resolution? | Description |
> | --- | --- | --- | --- | --- |
> | _example:_ `Connection` | `string` | Yes | Yes (`%%`) | Connection string name from app settings |
> | _example:_ `QueueName` | `string` | Yes | Yes (`%%`) | Name of the queue to listen on |
> | _example:_ `MaxBatchSize` | `int` | No | No | Max messages per batch (default: 16) |
> | _example:_ `CreateIfNotExists` | `bool` | No | No | Auto-create resource if missing |

**Guidance to share with the user:**

- **Required properties** → decorated with `[AutoResolve]` (if string) and validated at startup. The binding fails if not provided.
- **Optional properties** → have sensible defaults. Document the default value in the description.
- **`%appSetting%` resolution** (also called `AutoResolve`) — Mark "Yes" for any string property where users should be able to reference app settings via `%SettingName%` syntax or bind to `{expressions}`. Typically used for:
  - Connection strings / endpoints (`Connection`, `Endpoint`)
  - Resource names that vary per environment (`QueueName`, `TopicName`, `ContainerName`)
  - **NOT** used for: numeric values, booleans, enums, or computed properties
- **Connection property pattern** — If the extension connects to an external service, include a `Connection` property that resolves to a named connection string in app settings. This follows the established pattern (e.g., `ServiceBusConnection`, `CosmosDBConnection`).

**Ask this question once per binding type.** If the user selected "Trigger + Input + Output" in Q3, ask three times (once for trigger properties, once for input, once for output).

### Q6 — App-Level Configuration

> Beyond binding properties, does your extension need any host-level configuration in `host.json`?
> These are settings that apply globally (not per-function). Examples:
>
> - `maxConcurrentCalls` (int, default: 16)
> - `autoCompleteMessages` (bool, default: true)
> - `transportType` (enum: Amqp | AmqpWebSockets)
>
> List them or say "none".

### Q7 — Team Context

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

- Update attribute properties to match Q5 (binding properties per binding type)
- Apply `[AutoResolve]` to properties marked as supporting `%appSetting%` resolution
- Mark required properties with validation logic in binding providers
- Set default values for optional properties
- Add connection resolution logic for `Connection`-type properties
- Update `host.json` extension section with Q6 app-level settings (if any)
- Update `host.json` logging namespace

### 2.5 — Set Version

In `eng/build/Version.props`:

```xml
<VersionPrefix>0.1.0</VersionPrefix>
<VersionSuffix>alpha</VersionSuffix>
```

### 2.6 — Configure Pipelines

**If azfunc team (Q7 = Yes):**

- Update `eng/ci/templates/variables/build.yml` with new solution name
- Update release pipeline source names and package patterns
- Update approvers to team security group

**If not azfunc team (Q7 = No):**

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

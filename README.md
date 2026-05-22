# Azure Functions Hello World Extension

A minimal sample extension for Azure Functions that demonstrates how to create custom triggers and bindings. This project is designed as a learning resource for new extension authors.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (pinned via `global.json`)
- [Azure Functions Core Tools v4](https://docs.microsoft.com/azure/azure-functions/functions-run-local)

## Project Structure

```text
azure-functions-helloworld-extension/
├── Directory.Build.props                 # Common MSBuild properties + Engineering.props import
├── Directory.Build.targets               # Imports Engineering.targets (chains WebJobsReference)
├── global.json                           # Pins .NET SDK version
├── NuGet.config                          # Package source configuration
├── .editorconfig                         # Code style enforcement
├── docs/
│   └── new-extension-checklist.md        # Checklist for forking this template
├── src/
│   ├── Microsoft.Azure.Functions.Extensions.HelloWorld/    # WebJobs extension (host-side)
│   │   ├── HelloWorldStartup.cs              # Entry point - registers extension with host
│   │   ├── HelloWorldWebJobsBuilderExtensions.cs  # Extension method for registration
│   │   ├── HelloWorldExtensionConfigProvider.cs   # Initializes bindings and triggers
│   │   ├── HelloWorldTriggerAttribute.cs     # The [HelloWorldTrigger] attribute
│   │   ├── HelloWorldTriggerBindingProvider.cs   # Creates trigger bindings
│   │   ├── HelloWorldTriggerBinding.cs       # Defines the trigger binding behavior
│   │   ├── HelloWorldListener.cs             # Watches for events and invokes functions
│   │   ├── HelloWorldValueProvider.cs        # Provides values to function parameters
│   │   ├── HelloWorldContext.cs              # Data passed to functions
│   │   ├── HelloWorldAttribute.cs            # Unified attribute for input/output bindings
│   │   └── HelloWorldAsyncCollector.cs       # Collects output binding values
│   └── Microsoft.Azure.Functions.Worker.Extensions.HelloWorld/  # Worker extension (client-side)
│       ├── Worker.Extensions.HelloWorld.csproj  # References WebJobs extension
│       ├── HelloWorldTriggerAttribute.cs     # Trigger attribute for isolated worker
│       ├── HelloWorldInputAttribute.cs       # Input attribute for isolated worker
│       ├── HelloWorldOutputAttribute.cs      # Output attribute for isolated worker
│       ├── HelloWorldContext.cs              # Context type for isolated worker
│       └── Converters/                       # IInputConverter implementations
│           ├── HelloWorldContextConverter.cs # Converts trigger data
│           └── HelloWorldInputConverter.cs   # Converts input binding data
├── samples/
│   ├── dotnet/                                # .NET isolated worker sample
│   ├── nodejs/                                # Node.js (TypeScript) sample
│   └── python/                                # Python sample
│       ├── Directory.Build.targets           # Local development workaround
│       └── ...
├── test/
│   └── Microsoft.Azure.Functions.Extensions.HelloWorld.Tests/  # Unit tests (xunit v3, 29 tests)
├── eng/
│   ├── build/                                # Centralized build infrastructure
│   │   ├── Engineering.props                 # Lang version, signing, NuGet audit
│   │   ├── Engineering.targets               # Imports Release + WebJobsReference targets
│   │   ├── Version.props                     # Centralized versioning
│   │   ├── Version.targets                   # Local vs CI version stamping
│   │   ├── Release.props                     # NuGet package metadata + CI detection
│   │   ├── Release.targets                   # Release notes injection
│   │   ├── RepositoryInfo.targets            # SourceLink URL translation
│   │   └── WebJobsReference.targets          # Links Worker extension to WebJobs extension
│   ├── ci/                                   # Azure Pipelines
│   │   ├── public-build.yml                  # PR/CI builds
│   │   ├── official-build.yml                # Official CI + tag-triggered builds
│   │   ├── code-mirror.yml                   # GitHub → AzDO mirror
│   │   ├── official-release-host.yml         # Host extension NuGet release
│   │   ├── official-release-worker.yml       # Worker extension NuGet release
│   │   └── templates/                        # Shared pipeline templates
│   └── res/                                  # Build resources
│       ├── key.snk                           # Strong-name signing key (unique per extension)
│       └── icon.png                          # NuGet package icon
└── README.md
```

## Key Concepts

### Extension Architecture

An Azure Functions extension for the isolated worker model consists of two packages:

1. **WebJobs Extension** (host-side) - Contains the actual binding logic, runs in the host process
2. **Worker Extension** (client-side) - Contains attributes that developers use in their code, runs in the worker process

### WebJobs Extension Components

1. **Startup class** - Entry point using `[WebJobsStartup]` attribute
2. **Extension config provider** - Registers bindings with the host
3. **Trigger attribute** - The attribute that defines binding metadata
4. **Binding provider** - Factory that creates bindings
5. **Binding** - Defines how data flows to function parameters
6. **Listener** - The actual trigger logic that fires events
7. **Value provider** - Converts data to parameter types

### Worker Extension Components

1. **Trigger attribute** - Inherits from `TriggerBindingAttribute`
2. **Input attribute** - Inherits from `InputBindingAttribute`
3. **Output attribute** - Inherits from `OutputBindingAttribute`
4. **Context types** - POCOs that represent binding data

### 1. Extension Startup (`HelloWorldStartup.cs`)

The entry point for your extension. Uses `[WebJobsStartup]` attribute to register with the Azure Functions host.

### 2. Extension Configuration Provider (`HelloWorldExtensionConfigProvider.cs`)

Initializes your extension and registers all bindings. This is where you wire up triggers, input bindings, and output bindings.

### 3. Trigger Attribute (`HelloWorldTriggerAttribute.cs`)

The attribute users apply to function parameters. Marked with `[Binding]` to identify it as a binding attribute.

### 4. Trigger Binding Provider (`HelloWorldTriggerBindingProvider.cs`)

Factory that creates trigger bindings when the host indexes functions.

### 5. Trigger Binding (`HelloWorldTriggerBinding.cs`)

Defines how the trigger binds data to function parameters and creates the listener.

### 6. Listener (`HelloWorldListener.cs`)

Watches for events and invokes the function when triggered. This is where your trigger's actual logic lives.

### 7. Value Provider (`HelloWorldValueProvider.cs`)

Converts trigger data to the parameter type the function expects.

### 8. Input Binding

A simple input binding that provides data to the function. Users apply `[HelloWorldInput]` to a parameter:

```csharp
[HelloWorldInput(Greeting = "Welcome", GreetingName = "Developer")] string message
```

### 9. Output Binding (`HelloWorldAsyncCollector.cs`)

An output binding that collects values from the function. The WebJobs extension uses `IAsyncCollector<string>` to collect output values.

## Writing Binding Attributes: Trigger vs Input vs Output

When creating an Azure Functions extension for the isolated worker model, you need to understand the **naming conventions** that differ between the Worker SDK and WebJobs SDK.

### The Naming Convention Problem

The Worker SDK derives binding type names by stripping suffixes from attribute class names:

| Worker Attribute Class | Generated Binding Type |
| --- | --- |
| `HelloWorldTriggerAttribute` | `helloWorldTrigger` |
| `HelloWorldInputAttribute` | `helloWorld` (strips "Input") |
| `HelloWorldOutputAttribute` | `helloWorld` (strips "Output") |

Notice that **both input and output attributes generate the same type name** (`helloWorld`). This has implications for how you structure your WebJobs extension.

### Writing a Trigger Attribute

Triggers are straightforward - the Worker SDK preserves "Trigger" in the type name.

**Worker extension (client-side):**

```csharp
[InputConverter(typeof(HelloWorldContextConverter))]
public sealed class HelloWorldTriggerAttribute : TriggerBindingAttribute
{
    public string GreetingName { get; }
}
```

**WebJobs extension (host-side):**

```csharp
[Binding]
public sealed class HelloWorldTriggerAttribute : Attribute
{
    [AutoResolve]
    public string GreetingName { get; set; }
}

// Registration in IExtensionConfigProvider.Initialize():
context.AddBindingRule<HelloWorldTriggerAttribute>()
    .BindToTrigger(new HelloWorldTriggerBindingProvider(_loggerFactory));
```

### Writing Input and Output Attributes

Because the Worker SDK strips "Input" and "Output" suffixes, both `HelloWorldInputAttribute` and `HelloWorldOutputAttribute` generate `"type": "helloWorld"` in function.json.

**Worker extension (client-side) - Use separate attributes (convention):**

```csharp
// Input attribute
[InputConverter(typeof(HelloWorldInputConverter))]
public sealed class HelloWorldInputAttribute : InputBindingAttribute
{
    public string Greeting { get; set; } = "Hello";
    public string GreetingName { get; set; } = "World";
}

// Output attribute
public sealed class HelloWorldOutputAttribute : OutputBindingAttribute
{
    public string Prefix { get; set; } = "";
}
```

**WebJobs extension (host-side) - Use a unified attribute:**

Since both input and output generate the same binding type (`helloWorld`), the WebJobs extension must use a **single unified attribute** that handles both directions:

```csharp
[Binding]
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
public sealed class HelloWorldAttribute : Attribute
{
    // Properties for input binding
    public string Greeting { get; set; } = "Hello";
    public string GreetingName { get; set; } = "World";
    
    // Properties for output binding
    public string Prefix { get; set; } = "";
}

// Registration in IExtensionConfigProvider.Initialize():
var bindingRule = context.AddBindingRule<HelloWorldAttribute>();
bindingRule.BindToInput<string>(attr => $"{attr.Greeting}, {attr.GreetingName}!");
bindingRule.BindToCollector<string>(attr => new HelloWorldAsyncCollector(attr, outputLogger));
```

This pattern follows the official [BlobAttribute](https://github.com/Azure/azure-webjobs-sdk/blob/master/src/Microsoft.Azure.WebJobs.Extensions.Storage/Blobs/BlobAttribute.cs) which uses a single unified attribute on the WebJobs side while the Worker SDK has separate `BlobInputAttribute` and `BlobOutputAttribute`.

### Writing Converters for Input Bindings

Input bindings in the isolated worker model require `IInputConverter` implementations to deserialize data from the host:

```csharp
[InputConverter(typeof(HelloWorldInputConverter))]
public sealed class HelloWorldInputAttribute : InputBindingAttribute { }

internal sealed class HelloWorldInputConverter : IInputConverter
{
    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        if (context.TargetType == typeof(string) && context.Source is string greeting)
        {
            return new ValueTask<ConversionResult>(ConversionResult.Success(greeting));
        }
        return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
    }
}
```

### Summary Table

| Binding Type | Worker Extension | WebJobs Extension |
| --- | --- | --- |
| **Trigger** | `HelloWorldTriggerAttribute` : `TriggerBindingAttribute` | `HelloWorldTriggerAttribute` : `Attribute` with `[Binding]` |
| **Input** | `HelloWorldInputAttribute` : `InputBindingAttribute` | Unified `HelloWorldAttribute` with `BindToInput()` |
| **Output** | `HelloWorldOutputAttribute` : `OutputBindingAttribute` | Unified `HelloWorldAttribute` with `BindToCollector()` |

## Building

```bash
dotnet build -c Release
```

## Running Tests

```bash
dotnet test -c Release
```

## Running the Sample

The sample uses the isolated worker model (dotnet-isolated):

```bash
cd samples/dotnet
func start
```

The sample functions will be triggered 5 seconds after startup:

```text
HelloWorld trigger fired!
  Name: Azure Functions Developer
  Timestamp: 02/28/2026 01:45:11 +00:00
  InvocationId: 45e9cdb0-7c8d-4032-ad1c-e65ebd02de08
HelloWorld input binding demo!
  Trigger Name: Developer
  Input Greeting: Welcome, Azure Developer!
HelloWorld output binding demo!
  Trigger Name: Messenger
[HelloWorld Output] [HelloWorld] : Message from Messenger at 2/28/2026 3:46:57 AM +00:00
```

## Sample Functions (Isolated Worker)

The sample demonstrates all three binding types:

### Trigger Binding

```csharp
[Function("SayHello")]
public void SayHello(
    [HelloWorldTrigger("Azure Functions Developer")] HelloWorldContext context)
{
    _logger.LogInformation("HelloWorld trigger fired!");
    _logger.LogInformation("  Name: {Name}", context.Name);
}
```

### Input Binding

```csharp
[Function("GetGreeting")]
public void GetGreeting(
    [HelloWorldTrigger("Developer")] HelloWorldContext context,
    [HelloWorldInput(Greeting = "Welcome", GreetingName = "Azure Developer")] string greeting)
{
    _logger.LogInformation("HelloWorld input binding demo!");
    _logger.LogInformation("  Input Greeting: {Greeting}", greeting);
}
```

### Output Binding

```csharp
[Function("SendMessage")]
[HelloWorldOutput(Prefix = "[HelloWorld] ")]
public string SendMessage(
    [HelloWorldTrigger("Messenger")] HelloWorldContext context)
{
    _logger.LogInformation("HelloWorld output binding demo!");
    // Return value is sent to the output binding
    return $"Message from {context.Name} at {context.Timestamp}";
}
```

## Creating Your Own Extension

For a comprehensive checklist of everything you need to customize when forking this template, see **[New Extension Checklist](docs/new-extension-checklist.md)**.

Quick steps:

1. **Copy this project** as a starting template
2. **Generate a new `key.snk`** — each extension must have a unique signing key
3. **Rename** all "HelloWorld" references to your extension name
4. **Create both packages**:
   - WebJobs extension (host-side) with binding logic
   - Worker extension (client-side) with attributes
5. **Modify the Listener** to implement your trigger logic (e.g., poll a service, listen to a queue)
6. **Customize input bindings** - modify `BindToInput()` to fetch data from external sources
7. **Customize output bindings** - modify `HelloWorldAsyncCollector` to send data to external systems
8. **Link the packages** using `WebJobsReference` in your Worker extension csproj
9. **Update versioning** in `eng/build/Version.props`
10. **Update CI pipelines** with your package names and approvers
11. **Package as NuGet** and publish for others to use

## Design Decision Guide: When to Use Each Binding Type

### Triggers

**Use triggers when:** You need to react to external events or invoke functions on a schedule.

**Examples:**

- Timer-based polling (like this HelloWorld extension)
- Message queue listeners (ServiceBus, RabbitMQ)
- Webhook receivers
- File system watchers

**Trade-offs:**

- ✅ Functions are invoked automatically when events occur
- ✅ Built-in scaling based on event load

### Input Bindings

**Use input bindings when:** You need to fetch data declaratively before function execution.

**Examples:**

- Read configuration from a database
- Fetch a document by ID
- Load secrets from Key Vault

**Trade-offs:**

- ✅ Data is ready when function starts (no async fetching in function code)
- ✅ Easy to swap implementations (just change the binding)
- ✅ Cleaner function signatures
- ❌ Data is fetched even if function might not need it

### Output Bindings

**Use output bindings when:** You want the runtime to handle output persistence.

**Examples:**

- Write to queues, databases, or blob storage
- Send notifications
- Log to external systems

**Trade-offs:**

- ✅ Runtime handles retries and batching
- ✅ Clean separation - function logic doesn't know about destination
- ✅ Easy to change destinations without modifying function code

## How the Two-Package Linking Works

The `eng/build/WebJobsReference.targets` file is critical infrastructure that links the Worker extension to the WebJobs extension.

### The Problem

When a user installs your Worker extension NuGet package, the Functions host needs to know which WebJobs extension package contains the actual binding logic. How does it know?

### The Solution: ExtensionInformation Attribute

The `WebJobsReference.targets` file generates an assembly attribute in your Worker extension:

```csharp
[assembly: ExtensionInformation("Microsoft.Azure.Functions.Extensions.HelloWorld", "1.0.0")]
```

At runtime, the Functions host:

1. Finds this attribute in your Worker extension assembly
2. Downloads the specified WebJobs package from NuGet
3. Loads and initializes the WebJobs extension

### Setting Up the Link

In your Worker extension csproj:

```xml
<ItemGroup>
  <WebJobsReference Include="$(SrcRoot)YourExtension\YourExtension.csproj" />
</ItemGroup>
```

The targets file (imported via `Directory.Build.targets`) automatically:

1. Reads `PackageId` and `Version` from your WebJobs project
2. Generates the `ExtensionInformation` attribute during build
3. Embeds it in your Worker extension assembly

## Local Development

### Local Development Challenge

When building locally (before publishing to NuGet), you'll see this error:

```text
error NU1101: Unable to find package Microsoft.Azure.Functions.Extensions.HelloWorld.
```

This happens because:

1. Your Worker extension has `[assembly: ExtensionInformation("...HelloWorld", "1.0.0")]`
2. The Functions SDK generates `WorkerExtensions.csproj` in the `obj` folder
3. That file contains `<PackageReference Include="...HelloWorld" Version="1.0.0" />`
4. NuGet tries to restore this package, but it doesn't exist yet!

### The Solution

The sample includes a workaround in `samples/dotnet/Directory.Build.targets` that:

1. Runs after `WorkerExtensions.csproj` is generated
2. Removes the `PackageReference` to the non-existent NuGet package
3. Adds a `ProjectReference` to the local WebJobs project instead

### When to Use This Workaround

- ✅ During local development before publishing to NuGet
- ✅ In CI/CD when building the sample alongside the extension
- ❌ NOT needed by end users of your published extension

Once your packages are on NuGet, the standard package restore works normally.

## Publishing Your Extension

### Package Structure

You need to publish two NuGet packages:

| Package | Description | Dependencies |
| --- | --- | --- |
| `Microsoft.Azure.Functions.Extensions.YourExtension` | WebJobs extension (host-side) | `Microsoft.Azure.WebJobs` |
| `Microsoft.Azure.Functions.Worker.Extensions.YourExtension` | Worker extension (client-side) | `Microsoft.Azure.Functions.Worker.Extensions.Abstractions` |

### Versioning Strategy

Both packages should share the same version number. The `ExtensionInformation` attribute embeds the version, so they must match.

### Publishing Commands

```bash
# Pack all packages (version is centralized in eng/build/Version.props)
dotnet pack -c Release -o ./packages

# Publish to NuGet
dotnet nuget push ./packages/*.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

### Testing Before Publish

1. Create a local NuGet feed folder
2. Pack to that folder: `dotnet pack -o ./local-feed`
3. Add local feed to NuGet.config
4. Test with a separate function app project

## Configuration with host.json

Extensions can read configuration from `host.json`. The `[Extension]` attribute on your config provider defines the configuration section name:

```csharp
[Extension("HelloWorld", "helloWorld")]  // Section: extensions.helloWorld
internal sealed class HelloWorldExtensionConfigProvider : IExtensionConfigProvider
```

Example `host.json`:

```json
{
  "version": "2.0",
  "extensions": {
    "helloWorld": {
      "triggerDelaySeconds": 10,
      "maxRetries": 3
    }
  }
}
```

To read configuration in your extension:

```csharp
public HelloWorldExtensionConfigProvider(
    ILoggerFactory loggerFactory,
    IOptions<HelloWorldOptions> options)  // Inject IOptions<T>
{
    _options = options.Value;
}
```

Register the options in your startup:

```csharp
public static IWebJobsBuilder AddHelloWorld(this IWebJobsBuilder builder)
{
    builder.AddExtension<HelloWorldExtensionConfigProvider>();
    builder.Services.AddOptions<HelloWorldOptions>()
        .Configure<IConfiguration>((options, config) =>
        {
            config.GetSection("extensions:helloWorld").Bind(options);
        });
    return builder;
}
```

## Troubleshooting

### Binding Type Not Registered

Error: `The binding type(s) 'xxx' are not registered`

- The binding type name in function.json doesn't match what's registered on the host
- Check that your Worker extension attribute names follow the naming convention
- See "Writing Binding Attributes" section for naming rules

### Package Not Found

Error: `Unable to find package Microsoft.Azure.Functions.Extensions.YourExtension`

- You're building locally before publishing to NuGet
- Apply the local development workaround (see "Local Development" section)

### ExtensionInformation Missing

Error: `ExtensionInformation attribute not found`

- Missing `<WebJobsReference>` in your Worker extension csproj
- Check that `Directory.Build.targets` imports `WebJobsReference.targets`

### Function Not Triggering

- Check that your Listener is actually starting (add logging)
- Verify the trigger attribute is correctly registered in `IExtensionConfigProvider.Initialize()`
- Check host.json logging to see extension initialization messages

## Two-Package Architecture

For isolated worker model, extensions need two packages:

```text
┌─────────────────────────────────────────────────────────────────┐
│                        Host Process                              │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │   Microsoft.Azure.Functions.Extensions.HelloWorld       │    │
│  │   (WebJobs Extension)                                   │    │
│  │   - HelloWorldListener (trigger logic)                  │    │
│  │   - HelloWorldExtensionConfigProvider                   │    │
│  │   - Binding providers and value providers               │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
                              ↕ gRPC
┌─────────────────────────────────────────────────────────────────┐
│                       Worker Process                             │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │   Microsoft.Azure.Functions.Worker.Extensions.HelloWorld│    │
│  │   (Worker Extension)                                    │    │
│  │   - HelloWorldTriggerAttribute                          │    │
│  │   - HelloWorldContext                                   │    │
│  └─────────────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │   Your Function App                                     │    │
│  │   - Uses attributes from Worker extension               │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
```

## Extension Flow

```text
Host Startup
    ↓
[WebJobsStartup] calls HelloWorldStartup.Configure()
    ↓
AddHelloWorld() registers HelloWorldExtensionConfigProvider
    ↓
Host calls Initialize() on the config provider
    ↓
Config provider registers HelloWorldTriggerAttribute → HelloWorldTriggerBindingProvider
    ↓
Host indexes functions, finds [HelloWorldTrigger] parameters
    ↓
Binding provider creates HelloWorldTriggerBinding
    ↓
Binding creates HelloWorldListener
    ↓
Listener starts watching for events
    ↓
Event occurs → Listener creates HelloWorldContext
    ↓
Binding converts context to function parameter type
    ↓
Function executes!
```

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit <https://cla.opensource.microsoft.com>.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft trademarks or logos is subject to and must follow [Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general). Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship. Any use of third-party trademarks or logos are subject to those third-party's policies.

## License

MIT License - See LICENSE file for details.

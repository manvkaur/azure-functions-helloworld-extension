# Azure Functions Hello World Extension

A minimal sample extension for Azure Functions that demonstrates how to create custom triggers, bindings, and extensions. This project is designed as a learning resource for new extension authors.

## Project Structure

```text
azure-functions-helloworld-extension/
├── src/
│   └── Microsoft.Azure.Functions.Extensions.HelloWorld/    # WebJobs extension (host-side)
│       ├── HelloWorldStartup.cs              # Entry point - registers extension with host
│       ├── HelloWorldWebJobsBuilderExtensions.cs  # Extension method for registration
│       ├── HelloWorldExtensionConfigProvider.cs   # Initializes bindings and triggers
│       ├── HelloWorldTriggerAttribute.cs     # The [HelloWorldTrigger] attribute
│       ├── HelloWorldTriggerBindingProvider.cs   # Creates trigger bindings
│       ├── HelloWorldTriggerBinding.cs       # Defines the trigger binding behavior
│       ├── HelloWorldListener.cs             # Watches for events and invokes functions
│       ├── HelloWorldValueProvider.cs        # Provides values to function parameters
│       ├── HelloWorldContext.cs              # Data passed to functions
│       ├── HelloWorldInputAttribute.cs       # The [HelloWorldInput] attribute
│       ├── HelloWorldOutputAttribute.cs      # The [HelloWorldOutput] attribute
│       └── HelloWorldAsyncCollector.cs       # Collects output binding values
├── samples/
│   └── SampleFunctionApp/                    # Sample function using the extension (in-process)
├── test/
│   └── Microsoft.Azure.Functions.Extensions.HelloWorld.Tests/  # Unit tests
└── README.md
```

## Key Concepts

### Extension Architecture

An Azure Functions extension implements custom triggers and bindings that the host can load. The key components are:

1. **Startup class** - Entry point using `[WebJobsStartup]` attribute
2. **Extension config provider** - Registers bindings with the host
3. **Trigger attribute** - The attribute users apply to function parameters
4. **Binding provider** - Factory that creates bindings
5. **Binding** - Defines how data flows to function parameters
6. **Listener** - The actual trigger logic that fires events
7. **Value provider** - Converts data to parameter types

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

### 8. Input Binding (`HelloWorldInputAttribute.cs`)

A simple input binding that provides data to the function. Users apply `[HelloWorldInput]` to a parameter:

```csharp
[HelloWorldInput(Greeting = "Welcome", Name = "Developer")] string message
```

### 9. Output Binding (`HelloWorldOutputAttribute.cs` + `HelloWorldAsyncCollector.cs`)

An output binding that collects values from the function. Users apply `[HelloWorldOutput]` to an `IAsyncCollector<string>`:

```csharp
[HelloWorldOutput(Prefix = "LOG")] IAsyncCollector<string> collector
```

## Building

```bash
dotnet build
```

## Running the Sample

```bash
cd samples/SampleFunctionApp
func start
```

The sample function will be triggered 5 seconds after startup:

```
HelloWorld trigger fired!
  Name: Azure Functions Developer
  Timestamp: 02/28/2026 00:39:07 +00:00
  InvocationId: c68e251d-05dc-44f8-acf4-9c868e45b85d
  Input binding message: Welcome, Developer!
[HelloWorld Output] LOG: Function triggered at 2/28/2026 12:39:07 AM +00:00
[HelloWorld Output] LOG: Greeting: Hello, Azure Functions Developer!
Executed 'SayHello' (Succeeded, ...)
```

## Sample Function

The sample demonstrates all three binding types:

```csharp
[FunctionName("SayHello")]
public async Task SayHello(
    [HelloWorldTrigger("Azure Functions Developer")] HelloWorldContext context,
    [HelloWorldInput(Greeting = "Welcome", Name = "Developer")] string inputMessage,
    [HelloWorldOutput(Prefix = "LOG")] IAsyncCollector<string> outputCollector)
{
    _logger.LogInformation("Input binding message: {Message}", inputMessage);
    await outputCollector.AddAsync($"Function triggered at {context.Timestamp}");
}
```

## Creating Your Own Extension

1. **Copy this project** as a starting template
2. **Rename** all "HelloWorld" references to your extension name
3. **Modify the Listener** to implement your trigger logic (e.g., poll a service, listen to a queue)
4. **Customize input bindings** - modify `BindToInput()` to fetch data from external sources
5. **Customize output bindings** - modify `HelloWorldAsyncCollector` to send data to external systems
6. **Package as NuGet** and publish for others to use

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

## License

MIT License - See LICENSE file for details.

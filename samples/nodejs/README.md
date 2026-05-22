# Azure Functions HelloWorld Extension - Node.js Sample

This sample demonstrates how to use the HelloWorld extension from a Node.js (TypeScript) Azure Function using the generic binding model.

## Prerequisites

- [Node.js 20+](https://nodejs.org/)
- [Azure Functions Core Tools v4](https://docs.microsoft.com/azure/azure-functions/functions-run-local)

## Setup

```bash
npm install
npm run build
```

## Running

```bash
func start
```

## How It Works

Non-.NET languages consume custom extensions via **generic bindings**. Instead of typed attributes, you define the binding type and properties in the programmatic model:

```typescript
app.generic('SayHello', {
    trigger: {
        type: 'helloWorldTrigger',
        name: 'context',
        greetingName: 'Node.js Developer',
    },
    handler: async (context, invocationContext) => {
        // context is the HelloWorldContext object serialized as JSON
    },
});
```

The `extensionBundle` is **not** used. Instead, an `extensions.csproj` file references the host-side extension project directly for local testing against the extension source code. Build it before running:

```bash
dotnet build extensions.csproj
```

This compiles the HelloWorld extension into the `bin/` folder where Azure Functions Core Tools expects it for non-.NET languages. Once the extension is published to NuGet, you would switch to an extension bundle or a `PackageReference` in `extensions.csproj` instead of `ProjectReference`.

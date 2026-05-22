# Azure Functions HelloWorld Extension - Python Sample

This sample demonstrates how to use the HelloWorld extension from a Python Azure Function using the generic binding model.

## Prerequisites

- [Python 3.13+](https://learn.microsoft.com/azure/azure-functions/supported-languages?pivots=programming-language-python#languages-by-runtime-version)
- Azure Functions Core Tools v4

## Setup

1. **Build the extension** (from repo root):

   ```bash
   cd samples/python
   dotnet build extensions.csproj
   ```

2. **Create virtual environment:**

   ```bash
   python -m venv .venv
   source .venv/bin/activate  # Linux/macOS
   .venv\Scripts\activate     # Windows
   pip install -r requirements.txt
   ```

3. **Run the function app:**

   ```bash
   func start
   ```

## How It Works

Non-.NET languages consume custom extensions via **generic bindings**. Instead of typed decorators, you use the generic trigger/input/output decorators with the binding type name:

```python
@app.generic_trigger(arg_name="context", type="helloWorldTrigger",
                     greetingName="Python Developer")
def say_hello(context: str) -> None:
    # context is the HelloWorldContext object serialized as JSON string
    hello_context = json.loads(context)
```

The `extensionBundle` is **not** used. Instead, an `extensions.csproj` file references the host-side extension project directly for local testing against the extension source code. Build it before running:

```bash
dotnet build extensions.csproj
```

This compiles the HelloWorld extension into the `bin/` folder where Azure Functions Core Tools expects it for non-.NET languages. Once the extension is published to NuGet, you would switch to an extension bundle or a `PackageReference` in `extensions.csproj` instead of `ProjectReference`.

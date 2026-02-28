# Contributing to Azure Functions HelloWorld Extension

This project welcomes contributions and suggestions. Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools v4](https://docs.microsoft.com/azure/azure-functions/functions-run-local)

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Running the Sample

```bash
cd samples/SampleFunctionApp
func start
```

## How to Contribute

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Run tests to ensure they pass (`dotnet test`)
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## Code Style

- Follow the existing code style in the project
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Include unit tests for new functionality

## Reporting Issues

Please use the GitHub issue tracker to report bugs or request features. When reporting a bug, include:

- A clear description of the issue
- Steps to reproduce
- Expected behavior
- Actual behavior
- .NET version and OS information

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

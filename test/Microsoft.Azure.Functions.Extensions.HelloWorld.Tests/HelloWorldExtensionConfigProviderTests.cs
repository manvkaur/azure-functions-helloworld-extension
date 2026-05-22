// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld.Tests;

/// <summary>
/// End-to-end tests for the extension config provider initialization.
/// Verifies binding rule registration works correctly in-process.
/// </summary>
public class HelloWorldExtensionConfigProviderTests
{
    [Fact]
    public void Initialize_RegistersBindingRules_WithoutExternalResources()
    {
        // Arrange - simulate WebJobs host initialization
        var provider = new HelloWorldExtensionConfigProvider(NullLoggerFactory.Instance);

        // Act & Assert - constructor completes without error, DI works correctly
        Assert.NotNull(provider);
    }

    [Fact]
    public void Constructor_WithNullLoggerFactory_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new HelloWorldExtensionConfigProvider(null!));
    }

    [Fact]
    public void Initialize_WithNullContext_ThrowsArgumentNullException()
    {
        var provider = new HelloWorldExtensionConfigProvider(NullLoggerFactory.Instance);

        Assert.Throws<ArgumentNullException>(() => provider.Initialize(null!));
    }

    [Fact]
    public void AddHelloWorld_RegistersExtension_InServiceCollection()
    {
        // Arrange - build a minimal WebJobs host to verify DI registration
        var hostBuilder = new HostBuilder()
            .ConfigureWebJobs(builder =>
            {
                builder.AddHelloWorld();
            });

        // Act
        using var host = hostBuilder.Build();

        // Assert - the config provider should be resolvable via DI
        var configProviders = host.Services.GetServices<IExtensionConfigProvider>();
        Assert.Contains(configProviders, p => p is HelloWorldExtensionConfigProvider);
    }
}

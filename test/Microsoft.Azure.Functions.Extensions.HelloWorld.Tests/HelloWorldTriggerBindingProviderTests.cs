// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld.Tests;

/// <summary>
/// Tests for HelloWorldTriggerBindingProvider.
/// </summary>
public class HelloWorldTriggerBindingProviderTests
{
    private readonly ILoggerFactory _loggerFactory;

    public HelloWorldTriggerBindingProviderTests()
    {
        _loggerFactory = NullLoggerFactory.Instance;
    }

    [Fact]
    public async Task TryCreateAsync_WithHelloWorldTriggerAttribute_ReturnsBinding()
    {
        // Arrange
        var provider = new HelloWorldTriggerBindingProvider(_loggerFactory);
        var parameterInfo = GetParameterWithAttribute();
        var context = new TriggerBindingProviderContext(parameterInfo, CancellationToken.None);

        // Act
        var binding = await provider.TryCreateAsync(context);

        // Assert
        Assert.NotNull(binding);
        Assert.IsType<HelloWorldTriggerBinding>(binding);
    }

    [Fact]
    public async Task TryCreateAsync_WithoutAttribute_ReturnsNull()
    {
        // Arrange
        var provider = new HelloWorldTriggerBindingProvider(_loggerFactory);
        var parameterInfo = GetParameterWithoutAttribute();
        var context = new TriggerBindingProviderContext(parameterInfo, CancellationToken.None);

        // Act
        var binding = await provider.TryCreateAsync(context);

        // Assert
        Assert.Null(binding);
    }

    [Fact]
    public async Task TryCreateAsync_WithNullContext_ThrowsArgumentNullException()
    {
        // Arrange
        var provider = new HelloWorldTriggerBindingProvider(_loggerFactory);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => provider.TryCreateAsync(null!));
    }

    [Fact]
    public void Constructor_WithNullLoggerFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new HelloWorldTriggerBindingProvider(null!));
    }

    // Helper methods to get ParameterInfo objects for testing
    private static ParameterInfo GetParameterWithAttribute()
    {
        var method = typeof(HelloWorldTriggerBindingProviderTests)
            .GetMethod(nameof(SampleFunctionWithAttribute), BindingFlags.NonPublic | BindingFlags.Static)!;
        return method.GetParameters()[0];
    }

    private static ParameterInfo GetParameterWithoutAttribute()
    {
        var method = typeof(HelloWorldTriggerBindingProviderTests)
            .GetMethod(nameof(SampleFunctionWithoutAttribute), BindingFlags.NonPublic | BindingFlags.Static)!;
        return method.GetParameters()[0];
    }

    // Sample methods used for reflection
    private static void SampleFunctionWithAttribute(
        [HelloWorldTrigger("TestUser")] HelloWorldContext context) { }

    private static void SampleFunctionWithoutAttribute(string plainParameter) { }
}

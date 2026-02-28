// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xunit;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld.Tests;

/// <summary>
/// Tests for HelloWorldValueProvider type conversion.
/// </summary>
public class HelloWorldValueProviderTests
{
    private readonly HelloWorldContext _context;

    public HelloWorldValueProviderTests()
    {
        _context = new HelloWorldContext
        {
            Name = "TestUser",
            MessagePrefix = "👋",
            Timestamp = DateTimeOffset.UtcNow,
            InvocationId = "test-invocation-id"
        };
    }

    [Fact]
    public async Task GetValueAsync_WithContextType_ReturnsContext()
    {
        // Arrange
        var valueProvider = new HelloWorldValueProvider(_context, typeof(HelloWorldContext));

        // Act
        var result = await valueProvider.GetValueAsync();

        // Assert
        Assert.Same(_context, result);
    }

    [Fact]
    public async Task GetValueAsync_WithStringType_ReturnsJsonSerializedContext()
    {
        // Arrange
        var valueProvider = new HelloWorldValueProvider(_context, typeof(string));

        // Act
        var result = await valueProvider.GetValueAsync();

        // Assert - string type returns JSON serialization
        Assert.IsType<string>(result);
        var json = (string)result!;
        Assert.Contains("TestUser", json);
        Assert.Contains("test-invocation-id", json);
    }

    [Fact]
    public async Task GetValueAsync_WithUnknownType_ReturnsContext()
    {
        // Arrange - DateTimeOffset is not explicitly handled, so falls to default
        var valueProvider = new HelloWorldValueProvider(_context, typeof(DateTimeOffset));

        // Act
        var result = await valueProvider.GetValueAsync();

        // Assert - default case returns the context
        Assert.Same(_context, result);
    }

    [Fact]
    public async Task GetValueAsync_WithObjectType_ReturnsContext()
    {
        // Arrange
        var valueProvider = new HelloWorldValueProvider(_context, typeof(object));

        // Act
        var result = await valueProvider.GetValueAsync();

        // Assert
        Assert.Same(_context, result);
    }

    [Fact]
    public void Type_ReturnsExpectedType()
    {
        // Arrange
        var valueProvider = new HelloWorldValueProvider(_context, typeof(HelloWorldContext));

        // Act & Assert
        Assert.Equal(typeof(HelloWorldContext), valueProvider.Type);
    }

    [Fact]
    public void ToInvokeString_ReturnsFormattedString()
    {
        // Arrange
        var valueProvider = new HelloWorldValueProvider(_context, typeof(HelloWorldContext));

        // Act
        var result = valueProvider.ToInvokeString();

        // Assert
        Assert.Contains("HelloWorld:", result);
        Assert.Contains("TestUser", result);
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new HelloWorldValueProvider(null!, typeof(HelloWorldContext)));
    }

    [Fact]
    public void Constructor_WithNullParameterType_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new HelloWorldValueProvider(_context, null!));
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld.Tests;

/// <summary>
/// End-to-end tests for the trigger binding pipeline.
/// Exercises BindAsync, CreateListenerAsync, and ParameterDescriptor.
/// </summary>
public class HelloWorldTriggerBindingTests
{
    private readonly ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

    private static ParameterInfo GetSampleParameter()
    {
        // Get a real ParameterInfo for test purposes
        return typeof(HelloWorldTriggerBindingTests)
            .GetMethod(nameof(SampleFunction), BindingFlags.NonPublic | BindingFlags.Static)!
            .GetParameters()[0];
    }

    private static void SampleFunction(HelloWorldContext context) { }

    [Fact]
    public void TriggerValueType_IsHelloWorldContext()
    {
        var binding = new HelloWorldTriggerBinding(
            GetSampleParameter(),
            new HelloWorldTriggerAttribute("Test"),
            _loggerFactory);

        Assert.Equal(typeof(HelloWorldContext), binding.TriggerValueType);
    }

    [Fact]
    public void BindingDataContract_ContainsExpectedKeys()
    {
        var binding = new HelloWorldTriggerBinding(
            GetSampleParameter(),
            new HelloWorldTriggerAttribute("Test"),
            _loggerFactory);

        Assert.Contains("Name", binding.BindingDataContract.Keys);
        Assert.Contains("Timestamp", binding.BindingDataContract.Keys);
        Assert.Contains("InvocationId", binding.BindingDataContract.Keys);
    }

    [Fact]
    public async Task BindAsync_ReturnsCorrectTriggerData()
    {
        // Arrange
        var binding = new HelloWorldTriggerBinding(
            GetSampleParameter(),
            new HelloWorldTriggerAttribute("TestUser"),
            _loggerFactory);

        var context = new HelloWorldContext
        {
            Name = "TestUser",
            Timestamp = DateTimeOffset.UtcNow,
            InvocationId = "test-123"
        };

        // Act - ValueBindingContext is not used in BindAsync implementation
        var triggerData = await binding.BindAsync(context, null!);

        // Assert
        Assert.NotNull(triggerData);
        Assert.NotNull(triggerData.ValueProvider);

        var value = await triggerData.ValueProvider.GetValueAsync();
        Assert.IsType<HelloWorldContext>(value);
        Assert.Equal("TestUser", ((HelloWorldContext)value!).Name);
    }

    [Fact]
    public async Task BindAsync_WithInvalidValue_ThrowsInvalidOperationException()
    {
        var binding = new HelloWorldTriggerBinding(
            GetSampleParameter(),
            new HelloWorldTriggerAttribute("Test"),
            _loggerFactory);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => binding.BindAsync("not a HelloWorldContext", null!));
    }

    [Fact]
    public void ToParameterDescriptor_ReturnsValidDescriptor()
    {
        var binding = new HelloWorldTriggerBinding(
            GetSampleParameter(),
            new HelloWorldTriggerAttribute("TestUser"),
            _loggerFactory);

        var descriptor = binding.ToParameterDescriptor();

        Assert.Equal("context", descriptor.Name);
        Assert.Equal("HelloWorldTrigger", descriptor.Type);
    }

    [Fact]
    public void Constructor_WithNullParameter_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HelloWorldTriggerBinding(null!, new HelloWorldTriggerAttribute("Test"), _loggerFactory));
    }

    [Fact]
    public void Constructor_WithNullAttribute_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HelloWorldTriggerBinding(GetSampleParameter(), null!, _loggerFactory));
    }

    [Fact]
    public void Constructor_WithNullLoggerFactory_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HelloWorldTriggerBinding(GetSampleParameter(), new HelloWorldTriggerAttribute("Test"), null!));
    }
}

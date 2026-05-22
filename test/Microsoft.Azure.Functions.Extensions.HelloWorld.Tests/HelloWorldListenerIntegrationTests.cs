// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld.Tests;

/// <summary>
/// Integration tests for the listener executing the full trigger pipeline.
/// Verifies the listener fires and invokes the function executor with correct data.
/// No external resources required.
/// </summary>
public class HelloWorldListenerIntegrationTests
{
    [Fact]
    public async Task Listener_FiresTrigger_AndExecutesFunctionWithCorrectContext()
    {
        // Arrange
        var executionResult = new FunctionResult(true);
        HelloWorldContext? capturedContext = null;

        var executorMock = new Mock<ITriggeredFunctionExecutor>();
        executorMock
            .Setup(e => e.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()))
            .Callback<TriggeredFunctionData, CancellationToken>((data, _) =>
            {
                capturedContext = data.TriggerValue as HelloWorldContext;
            })
            .ReturnsAsync(executionResult);

        var attribute = new HelloWorldTriggerAttribute("IntegrationTestUser")
        {
            MessagePrefix = "TestPrefix"
        };
        var logger = NullLogger.Instance;
        var listener = new HelloWorldListener(executorMock.Object, attribute, logger);

        // Act - start listener and wait for the timer to fire (5s + buffer)
        await listener.StartAsync(TestContext.Current.CancellationToken);

        // Wait for the trigger to fire (timer is 5s)
        var timeout = TimeSpan.FromSeconds(10);
        var elapsed = TimeSpan.Zero;
        var interval = TimeSpan.FromMilliseconds(100);
        while (capturedContext == null && elapsed < timeout)
        {
            await Task.Delay(interval, TestContext.Current.CancellationToken);
            elapsed += interval;
        }

        await listener.StopAsync(TestContext.Current.CancellationToken);
        listener.Dispose();

        // Assert
        Assert.NotNull(capturedContext);
        Assert.Equal("IntegrationTestUser", capturedContext!.Name);
        Assert.Equal("TestPrefix", capturedContext.MessagePrefix);
        Assert.NotEqual(default, capturedContext.Timestamp);
        Assert.False(string.IsNullOrEmpty(capturedContext.InvocationId));

        executorMock.Verify(
            e => e.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Listener_WhenExecutorFails_LogsErrorAndContinues()
    {
        // Arrange
        var exception = new InvalidOperationException("Simulated failure");
        var executionResult = new FunctionResult(false, exception);

        var executorMock = new Mock<ITriggeredFunctionExecutor>();
        executorMock
            .Setup(e => e.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(executionResult);

        var attribute = new HelloWorldTriggerAttribute("FailUser");
        var listener = new HelloWorldListener(executorMock.Object, attribute, NullLogger.Instance);

        // Act - should not throw even when executor fails
        await listener.StartAsync(TestContext.Current.CancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(7), TestContext.Current.CancellationToken); // Wait for trigger
        await listener.StopAsync(TestContext.Current.CancellationToken);
        listener.Dispose();

        // Assert - executor was called (failure is handled gracefully)
        executorMock.Verify(
            e => e.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Listener_StoppedBeforeTrigger_DoesNotExecuteFunction()
    {
        // Arrange
        var executorMock = new Mock<ITriggeredFunctionExecutor>();
        var attribute = new HelloWorldTriggerAttribute("NeverFires");
        var listener = new HelloWorldListener(executorMock.Object, attribute, NullLogger.Instance);

        // Act - start and immediately stop before 5s timer fires
        await listener.StartAsync(TestContext.Current.CancellationToken);
        await listener.StopAsync(TestContext.Current.CancellationToken);
        listener.Dispose();

        // Assert - executor should never be called
        executorMock.Verify(
            e => e.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Listener_OnlyFiresOnce()
    {
        // Arrange
        var callCount = 0;
        var executionResult = new FunctionResult(true);

        var executorMock = new Mock<ITriggeredFunctionExecutor>();
        executorMock
            .Setup(e => e.TryExecuteAsync(It.IsAny<TriggeredFunctionData>(), It.IsAny<CancellationToken>()))
            .Callback(() => Interlocked.Increment(ref callCount))
            .ReturnsAsync(executionResult);

        var attribute = new HelloWorldTriggerAttribute("OnceOnly");
        var listener = new HelloWorldListener(executorMock.Object, attribute, NullLogger.Instance);

        // Act - wait well past the trigger time
        await listener.StartAsync(TestContext.Current.CancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(8), TestContext.Current.CancellationToken);
        await listener.StopAsync(TestContext.Current.CancellationToken);
        listener.Dispose();

        // Assert - should only fire once (period is Timeout.InfiniteTimeSpan)
        Assert.Equal(1, callCount);
    }
}

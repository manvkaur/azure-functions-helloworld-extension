// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld.Tests;

/// <summary>
/// Tests for HelloWorldAsyncCollector (output binding).
/// Runs entirely in-process without external resources.
/// </summary>
public class HelloWorldAsyncCollectorTests
{
    [Fact]
    public async Task AddAsync_WithPrefix_LogsPrefixedMessage()
    {
        // Arrange
        var logMessages = new List<string>();
        var logger = new CapturingLogger(logMessages);
        var attribute = new HelloWorldAttribute { Prefix = "TestPrefix" };
        var collector = new HelloWorldAsyncCollector(attribute, logger);

        // Act
        await collector.AddAsync("Hello World", TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(logMessages);
        Assert.Contains("TestPrefix", logMessages[0]);
        Assert.Contains("Hello World", logMessages[0]);
    }

    [Fact]
    public async Task AddAsync_WithoutPrefix_LogsRawMessage()
    {
        // Arrange
        var logMessages = new List<string>();
        var logger = new CapturingLogger(logMessages);
        var attribute = new HelloWorldAttribute { Prefix = "" };
        var collector = new HelloWorldAsyncCollector(attribute, logger);

        // Act
        await collector.AddAsync("Raw message", TestContext.Current.CancellationToken);

        // Assert
        Assert.Single(logMessages);
        Assert.Contains("Raw message", logMessages[0]);
        Assert.DoesNotContain(":", logMessages[0].Replace("[HelloWorld Output]", ""));
    }

    [Fact]
    public async Task AddAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        // Arrange
        var logger = NullLogger.Instance;
        var attribute = new HelloWorldAttribute();
        var collector = new HelloWorldAsyncCollector(attribute, logger);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => collector.AddAsync("test", cts.Token));
    }

    [Fact]
    public async Task AddAsync_MultipleMessages_LogsAll()
    {
        // Arrange
        var logMessages = new List<string>();
        var logger = new CapturingLogger(logMessages);
        var attribute = new HelloWorldAttribute { Prefix = "Log" };
        var collector = new HelloWorldAsyncCollector(attribute, logger);

        // Act
        await collector.AddAsync("First", TestContext.Current.CancellationToken);
        await collector.AddAsync("Second", TestContext.Current.CancellationToken);
        await collector.AddAsync("Third", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(3, logMessages.Count);
    }

    [Fact]
    public async Task FlushAsync_CompletesSuccessfully()
    {
        // Arrange
        var logger = NullLogger.Instance;
        var attribute = new HelloWorldAttribute();
        var collector = new HelloWorldAsyncCollector(attribute, logger);

        // Act & Assert - should not throw
        await collector.FlushAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public void Constructor_WithNullAttribute_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new HelloWorldAsyncCollector(null!, NullLogger.Instance));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new HelloWorldAsyncCollector(new HelloWorldAttribute(), null!));
    }

    /// <summary>
    /// Simple logger that captures formatted log messages for assertions.
    /// </summary>
    private sealed class CapturingLogger : ILogger
    {
        private readonly List<string> _messages;

        public CapturingLogger(List<string> messages) => _messages = messages;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _messages.Add(formatter(state, exception));
        }
    }
}

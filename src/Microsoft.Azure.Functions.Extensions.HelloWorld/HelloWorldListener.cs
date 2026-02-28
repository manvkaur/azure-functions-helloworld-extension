// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// The listener that watches for trigger events and invokes the function.
/// 
/// For a real extension, this might:
/// - Poll an external service
/// - Listen on a message queue
/// - Subscribe to webhooks
/// - Watch for file system changes
/// 
/// This simple example uses a timer to demonstrate the concept.
/// </summary>
internal sealed class HelloWorldListener : IListener
{
    private readonly ITriggeredFunctionExecutor _executor;
    private readonly HelloWorldTriggerAttribute _attribute;
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _cts = new();
    private readonly SemaphoreSlim _executionLock = new(1, 1);
    private readonly object _timerLock = new();
    private Timer? _timer;
    private Task? _executingTask;
    private int _disposed;

    public HelloWorldListener(
        ITriggeredFunctionExecutor executor,
        HelloWorldTriggerAttribute attribute,
        ILogger logger)
    {
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Starts the listener. For this example, we trigger once after a delay.
    /// In a real extension, this would start polling or subscribe to events.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for startup cancellation.</param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation("HelloWorld listener starting for name: {GreetingName}", _attribute.GreetingName);

        // LEARNING NOTE: This 5-second delay is hardcoded for demo simplicity.
        // In a real extension, you should:
        // 1. Make timing configurable via the trigger attribute or host.json
        // 2. Implement actual trigger logic (e.g., poll a service, listen to events)
        // 3. Consider using IOptionsMonitor<T> for runtime configuration changes
        //
        // Example of configurable timing:
        //   dueTime: TimeSpan.FromSeconds(_attribute.DelaySeconds ?? 5)
        _timer = new Timer(
            callback: OnTimerTick,
            state: null,
            dueTime: TimeSpan.FromSeconds(5),
            period: Timeout.InfiniteTimeSpan); // Only fire once

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the listener and waits for any executing function to complete.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for stop timeout.</param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("HelloWorld listener stopping");

        _cts.Cancel();
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);

        if (_executingTask != null)
        {
            try
            {
                await _executingTask.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("HelloWorld listener stop timed out while waiting for execution to complete");
            }
        }
    }

    public void Cancel()
    {
        _cts.Cancel();
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
    }

    private void OnTimerTick(object? state)
    {
        lock (_timerLock)
        {
            if (Volatile.Read(ref _disposed) == 1)
            {
                return;
            }
            _executingTask = ExecuteTriggerAsync();
        }
    }

    private async Task ExecuteTriggerAsync()
    {
        if (!await _executionLock.WaitAsync(0))
        {
            _logger.LogDebug("HelloWorld trigger skipped - previous execution still in progress");
            return;
        }

        try
        {
            if (_cts.IsCancellationRequested || Volatile.Read(ref _disposed) == 1)
            {
                return;
            }

            _logger.LogDebug("HelloWorld trigger firing");

            var context = new HelloWorldContext
            {
                Name = _attribute.GreetingName,
                MessagePrefix = _attribute.MessagePrefix,
                Timestamp = DateTimeOffset.UtcNow,
                InvocationId = Guid.NewGuid().ToString()
            };

            var triggerData = new TriggeredFunctionData { TriggerValue = context };
            var result = await _executor.TryExecuteAsync(triggerData, _cts.Token);

            if (!result.Succeeded && result.Exception != null)
            {
                _logger.LogError(result.Exception, "HelloWorld trigger function failed");
            }
        }
        catch (OperationCanceledException) when (_cts.IsCancellationRequested)
        {
            _logger.LogDebug("HelloWorld trigger execution cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing HelloWorld trigger");
        }
        finally
        {
            _executionLock.Release();
        }
    }

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
        {
            return;
        }

        _cts.Cancel();

        Task? executingTask;
        lock (_timerLock)
        {
            _timer?.Dispose();
            executingTask = _executingTask;
        }

        // Allow in-flight execution to complete gracefully
        try
        {
            executingTask?.Wait(TimeSpan.FromSeconds(5));
        }
        catch
        {
            // Shutting down - ignore exceptions
        }

        _executionLock.Dispose();
        _cts.Dispose();
    }
}

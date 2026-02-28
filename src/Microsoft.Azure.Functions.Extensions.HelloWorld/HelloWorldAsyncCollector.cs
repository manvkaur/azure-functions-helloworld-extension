// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// Async collector for the HelloWorld output binding.
/// 
/// This collector receives output values from the function and processes them.
/// In a real extension, this might:
/// - Send messages to a queue
/// - Write to a database
/// - Call an external API
/// 
/// For this demo, we simply log the messages.
/// </summary>
internal sealed class HelloWorldAsyncCollector : IAsyncCollector<string>
{
    private readonly HelloWorldOutputAttribute _attribute;
    private readonly ILogger _logger;

    public HelloWorldAsyncCollector(HelloWorldOutputAttribute attribute, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(attribute);
        ArgumentNullException.ThrowIfNull(logger);

        _attribute = attribute;
        _logger = logger;
    }

    /// <summary>
    /// Called when the function adds an item to the collector.
    /// </summary>
    public Task AddAsync(string item, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var message = string.IsNullOrEmpty(_attribute.Prefix)
            ? item
            : $"{_attribute.Prefix}: {item}";

        _logger.LogInformation("[HelloWorld Output] {Message}", message);

        return Task.CompletedTask;
    }

    public Task FlushAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

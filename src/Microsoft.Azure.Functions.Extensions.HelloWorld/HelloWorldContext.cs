// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// The context object passed to functions using the HelloWorldTrigger.
/// This contains all the information the function needs to process the trigger event.
/// </summary>
public sealed class HelloWorldContext
{
    /// <summary>
    /// Gets or sets the name from the trigger attribute.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the optional message prefix from the trigger attribute.
    /// </summary>
    public string? MessagePrefix { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the trigger fired.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the unique invocation ID for this trigger event.
    /// </summary>
    public string InvocationId { get; set; } = Guid.NewGuid().ToString();
}

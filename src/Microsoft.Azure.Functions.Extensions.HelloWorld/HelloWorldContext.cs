// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// =============================================================================
// IMPORTANT: This is the HOST-SIDE (WebJobs) version of HelloWorldContext.
// 
// In the isolated worker model, context types must exist in BOTH packages:
// - This version (WebJobs extension): Used by the host process to create and
//   populate the context before serializing it to the worker process.
// - Worker version: Used by the worker process to deserialize and provide
//   the context to user functions.
//
// The two versions should have matching properties but may have different
// default values since the host creates the instance and sets all properties.
// =============================================================================

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
    public string Name { get; set; } = string.Empty;

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

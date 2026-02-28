// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// =============================================================================
// IMPORTANT: This is the WORKER-SIDE version of HelloWorldContext.
// 
// In the isolated worker model, context types must exist in BOTH packages:
// - WebJobs version: Used by the host process to create the context.
// - This version (Worker extension): Used by user functions in the worker
//   process. The IInputConverter deserializes data from the host into this type.
//
// The two versions should have matching properties. Default values here are
// for deserialization fallback - the host should always provide actual values.
// =============================================================================

namespace Microsoft.Azure.Functions.Worker.Extensions.HelloWorld;

/// <summary>
/// The data passed to functions triggered by HelloWorldTrigger (isolated worker model).
/// </summary>
public sealed class HelloWorldContext
{
    /// <summary>
    /// The name from the trigger attribute.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The optional message prefix from the trigger attribute.
    /// </summary>
    public string? MessagePrefix { get; set; }

    /// <summary>
    /// The timestamp when the trigger fired.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// A unique identifier for this trigger invocation.
    /// </summary>
    public string InvocationId { get; set; } = string.Empty;
}

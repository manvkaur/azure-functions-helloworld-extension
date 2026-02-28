// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// Output binding attribute that collects messages from the function.
/// 
/// Usage:
/// <code>
/// public void MyFunction(
///     [HelloWorldTrigger("User")] HelloWorldContext context,
///     [HelloWorldOutput] out string message)
/// {
///     message = "This will be logged by the output binding";
/// }
/// </code>
/// 
/// Or with IAsyncCollector for multiple outputs:
/// <code>
/// public async Task MyFunction(
///     [HelloWorldTrigger("User")] HelloWorldContext context,
///     [HelloWorldOutput] IAsyncCollector&lt;string&gt; messages)
/// {
///     await messages.AddAsync("First message");
///     await messages.AddAsync("Second message");
/// }
/// </code>
/// 
/// This demonstrates how to create an output binding that processes
/// data produced by the function.
/// </summary>
[Binding]
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
public sealed class HelloWorldOutputAttribute : Attribute
{
    /// <summary>
    /// Optional prefix to add to all output messages.
    /// </summary>
    public string Prefix { get; set; } = "";
}

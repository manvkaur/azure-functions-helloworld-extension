// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// Input binding attribute that provides a greeting message to the function.
/// 
/// Usage:
/// <code>
/// public void MyFunction(
///     [HelloWorldTrigger("User")] HelloWorldContext context,
///     [HelloWorldInput(Greeting = "Welcome")] string message)
/// {
///     // message will contain the greeting
/// }
/// </code>
/// 
/// This demonstrates how to create a simple input binding that provides
/// data to the function without requiring external resources.
/// </summary>
[Binding]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class HelloWorldInputAttribute : Attribute
{
    /// <summary>
    /// The greeting prefix to use. Defaults to "Hello".
    /// </summary>
    public string Greeting { get; set; } = "Hello";

    /// <summary>
    /// The name to greet. If not specified, uses "World".
    /// </summary>
    public string Name { get; set; } = "World";
}

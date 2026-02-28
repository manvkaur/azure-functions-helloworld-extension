// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// Unified binding attribute for HelloWorld input and output bindings.
/// This attribute handles both input (direction=in) and output (direction=out) bindings.
/// 
/// For input binding usage:
/// <code>
/// public void MyFunction(
///     [HelloWorldTrigger("User")] HelloWorldContext context,
///     [HelloWorld(Greeting = "Welcome")] string message)
/// {
///     // message will contain the greeting
/// }
/// </code>
/// 
/// For output binding usage:
/// <code>
/// [return: HelloWorld(Prefix = "Logged: ")]
/// public string MyFunction(
///     [HelloWorldTrigger("User")] HelloWorldContext context)
/// {
///     return "This will be logged by the output binding";
/// }
/// </code>
/// 
/// This unified approach matches how the Worker SDK derives binding type names,
/// where both HelloWorldInputAttribute and HelloWorldOutputAttribute map to "helloWorld".
/// </summary>
[Binding]
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
public sealed class HelloWorldAttribute : Attribute
{
    /// <summary>
    /// The greeting prefix to use for input bindings. Defaults to "Hello".
    /// </summary>
    public string Greeting { get; set; } = "Hello";

    /// <summary>
    /// The name to greet for input bindings. If not specified, uses "World".
    /// </summary>
    public string GreetingName { get; set; } = "World";

    /// <summary>
    /// Optional prefix to add to all output messages for output bindings.
    /// </summary>
    public string Prefix { get; set; } = "";
}

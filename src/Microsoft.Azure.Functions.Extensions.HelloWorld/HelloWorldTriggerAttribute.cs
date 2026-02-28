// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// Attribute that marks a function parameter as a Hello World trigger.
/// 
/// Usage in a function:
/// <code>
/// [FunctionName("MyFunction")]
/// public static string Run([HelloWorldTrigger("greeting")] HelloWorldContext context)
/// {
///     return $"Hello, {context.Name}!";
/// }
/// </code>
/// 
/// The [Binding] attribute tells the Azure Functions host that this is a binding attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
[Binding]
public sealed class HelloWorldTriggerAttribute : Attribute
{
    /// <summary>
    /// Creates a new HelloWorldTriggerAttribute with the specified name.
    /// </summary>
    /// <param name="greetingName">The name to use in the greeting. Supports app settings with %Name% syntax.</param>
    /// <exception cref="ArgumentNullException">Thrown when greetingName is null.</exception>
    public HelloWorldTriggerAttribute(string greetingName)
    {
        ArgumentNullException.ThrowIfNull(greetingName);
        GreetingName = greetingName;
    }

    /// <summary>
    /// Gets the name to use in the greeting.
    /// The [AutoResolve] attribute enables %AppSettingName% syntax for configuration values.
    /// </summary>
    [AutoResolve]
    public string GreetingName { get; }

    /// <summary>
    /// Gets or sets an optional custom message prefix.
    /// </summary>
    public string? MessagePrefix { get; set; }
}

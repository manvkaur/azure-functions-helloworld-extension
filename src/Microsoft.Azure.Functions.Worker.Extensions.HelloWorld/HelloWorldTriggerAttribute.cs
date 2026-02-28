// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;
using Microsoft.Azure.Functions.Worker.Extensions.HelloWorld.Converters;

namespace Microsoft.Azure.Functions.Worker.Extensions.HelloWorld;

/// <summary>
/// Trigger attribute for the HelloWorld extension (isolated worker model).
/// Apply this to a function parameter to trigger the function via the HelloWorld extension.
/// </summary>
/// <remarks>
/// The InputConverter attribute specifies which converter to use for deserializing
/// the trigger data from the host process into the worker process.
/// </remarks>
[InputConverter(typeof(HelloWorldContextConverter))]
[ConverterFallbackBehavior(ConverterFallbackBehavior.Default)]
public sealed class HelloWorldTriggerAttribute : TriggerBindingAttribute
{
    /// <summary>
    /// Creates a new HelloWorldTriggerAttribute.
    /// </summary>
    /// <param name="greetingName">The name to use in the greeting.</param>
    public HelloWorldTriggerAttribute(string greetingName)
    {
        GreetingName = greetingName ?? throw new ArgumentNullException(nameof(greetingName));
    }

    /// <summary>
    /// The name to use in the greeting.
    /// </summary>
    public string GreetingName { get; }

    /// <summary>
    /// Optional prefix for the greeting message.
    /// </summary>
    public string? MessagePrefix { get; set; }
}

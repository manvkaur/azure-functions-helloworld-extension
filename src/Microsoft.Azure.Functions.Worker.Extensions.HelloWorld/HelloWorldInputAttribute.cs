// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;
using Microsoft.Azure.Functions.Worker.Extensions.HelloWorld.Converters;

namespace Microsoft.Azure.Functions.Worker.Extensions.HelloWorld;

/// <summary>
/// Input binding attribute for the HelloWorld extension (isolated worker model).
/// Provides a greeting message to the function.
/// </summary>
/// <remarks>
/// The InputConverter attribute specifies which converter to use for deserializing
/// the input binding data from the host process into the worker process.
/// </remarks>
[InputConverter(typeof(HelloWorldInputConverter))]
[ConverterFallbackBehavior(ConverterFallbackBehavior.Default)]
public sealed class HelloWorldInputAttribute : InputBindingAttribute
{
    /// <summary>
    /// The greeting prefix to use. Defaults to "Hello".
    /// </summary>
    public string Greeting { get; set; } = "Hello";

    /// <summary>
    /// The name to greet. If not specified, uses "World".
    /// </summary>
    public string GreetingName { get; set; } = "World";
}

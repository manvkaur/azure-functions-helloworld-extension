// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;

namespace Microsoft.Azure.Functions.Worker.Extensions.HelloWorld;

/// <summary>
/// Output binding attribute for the HelloWorld extension (isolated worker model).
/// Collects messages from the function.
/// </summary>
public sealed class HelloWorldOutputAttribute : OutputBindingAttribute
{
    /// <summary>
    /// Optional prefix to add to all output messages.
    /// </summary>
    public string Prefix { get; set; } = "";
}

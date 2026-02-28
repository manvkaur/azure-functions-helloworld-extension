// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// Provides the value for the function parameter bound to the HelloWorld trigger.
/// 
/// The value provider is responsible for:
/// 1. Converting the trigger value to the parameter type
/// 2. Providing a string representation for logging
/// </summary>
internal sealed class HelloWorldValueProvider : IValueProvider
{
    private readonly HelloWorldContext _context;
    private readonly Type _parameterType;

    public HelloWorldValueProvider(HelloWorldContext context, Type parameterType)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(parameterType);

        _context = context;
        _parameterType = parameterType;
    }

    /// <summary>
    /// The type of value this provider returns.
    /// </summary>
    public Type Type => _parameterType;

    /// <summary>
    /// Gets the value to pass to the function parameter.
    /// This method supports different parameter types.
    /// </summary>
    /// <returns>The value for the function parameter.</returns>
    public Task<object?> GetValueAsync()
    {
        // Support different parameter types
        object? value = _parameterType switch
        {
            // If the parameter is HelloWorldContext, return it directly
            Type t when t == typeof(HelloWorldContext) => _context,
            
            // If the parameter is a string, serialize to JSON
            Type t when t == typeof(string) => JsonSerializer.Serialize(_context),
            
            // Default: return the context object
            _ => _context
        };

        return Task.FromResult<object?>(value);
    }

    /// <summary>
    /// Returns a string representation of the value for logging.
    /// </summary>
    public string ToInvokeString()
    {
        return $"HelloWorld: {_context.Name} at {_context.Timestamp}";
    }
}

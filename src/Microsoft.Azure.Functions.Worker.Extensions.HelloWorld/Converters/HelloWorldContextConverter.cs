// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// =============================================================================
// UNDERSTANDING CONVERTERS IN ISOLATED WORKER MODEL
// =============================================================================
//
// WHY CONVERTERS ARE NEEDED:
// In the isolated worker model, the Azure Functions host runs in a separate
// process from your function code. They communicate via gRPC. When the host
// triggers your function, it serializes the binding data and sends it to the
// worker process. Converters deserialize this data into the types your
// function expects.
//
// CONVERTER RETURN VALUES:
// - ConversionResult.Success(value): Successfully converted, use this value
// - ConversionResult.Unhandled(): This converter doesn't handle this type,
//   let other converters try (used for converter chaining)
// - ConversionResult.Failed(exception): Conversion failed with an error,
//   stop trying and report the error
//
// WHEN TO USE EACH:
// - Unhandled: When TargetType doesn't match what this converter handles
// - Failed: When TargetType matches but conversion actually fails
// - Success: When conversion succeeds
//
// The [InputConverter] attribute on the binding attribute specifies which
// converters to try. Multiple converters can be registered for fallback.
// =============================================================================

using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Converters;

namespace Microsoft.Azure.Functions.Worker.Extensions.HelloWorld.Converters;

/// <summary>
/// Converter that deserializes trigger data into HelloWorldContext.
/// This converter is used by the isolated worker model to convert the
/// binding data from the host into a strongly-typed object.
/// </summary>
internal sealed class HelloWorldContextConverter : IInputConverter
{
    /// <summary>
    /// Attempts to convert the binding data to HelloWorldContext.
    /// </summary>
    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        // Step 1: Check if this converter handles the target type
        // Return Unhandled() to let other converters try if type doesn't match
        if (context.TargetType != typeof(HelloWorldContext))
        {
            return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
        }

        try
        {
            // Step 2: Try to deserialize from JSON (primary method)
            // The host serializes the context as JSON before sending via gRPC
            if (context.Source is string jsonString && !string.IsNullOrEmpty(jsonString))
            {
                var helloWorldContext = JsonSerializer.Deserialize<HelloWorldContext>(jsonString);
                if (helloWorldContext is not null)
                {
                    return new ValueTask<ConversionResult>(ConversionResult.Success(helloWorldContext));
                }
            }

            // Step 3: Fallback - create context from binding data dictionary
            // This handles cases where data comes as key-value pairs instead of JSON
            var bindingContext = CreateFromBindingData(context);
            if (bindingContext is not null)
            {
                return new ValueTask<ConversionResult>(ConversionResult.Success(bindingContext));
            }

            // Step 4: All conversion attempts failed - return Failed()
            // This is different from Unhandled() because we DID try to convert
            return new ValueTask<ConversionResult>(
                ConversionResult.Failed(new InvalidOperationException("Unable to convert to HelloWorldContext")));
        }
        catch (Exception ex)
        {
            // Conversion threw an exception - report the failure
            return new ValueTask<ConversionResult>(ConversionResult.Failed(ex));
        }
    }

    /// <summary>
    /// Creates HelloWorldContext from the binding data dictionary.
    /// This is a fallback when JSON deserialization doesn't work.
    /// </summary>
    private static HelloWorldContext? CreateFromBindingData(ConverterContext context)
    {
        var bindingData = context.FunctionContext.BindingContext.BindingData;
        
        if (bindingData.TryGetValue("GreetingName", out var nameObj) && nameObj is string name)
        {
            return new HelloWorldContext
            {
                Name = name,
                MessagePrefix = bindingData.TryGetValue("MessagePrefix", out var prefixObj) 
                    ? prefixObj as string 
                    : null,
                Timestamp = DateTimeOffset.UtcNow,
                InvocationId = context.FunctionContext.InvocationId
            };
        }

        return null;
    }
}

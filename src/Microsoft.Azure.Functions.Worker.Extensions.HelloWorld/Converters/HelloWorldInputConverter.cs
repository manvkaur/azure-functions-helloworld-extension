// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Functions.Worker.Converters;

namespace Microsoft.Azure.Functions.Worker.Extensions.HelloWorld.Converters;

/// <summary>
/// Converter for HelloWorld input binding.
/// Converts the input binding data (greeting string) to the target type.
/// </summary>
internal sealed class HelloWorldInputConverter : IInputConverter
{
    /// <summary>
    /// Attempts to convert the input binding data.
    /// The input binding returns a greeting string from the host.
    /// </summary>
    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        // Handle string target type (the greeting message)
        if (context.TargetType == typeof(string))
        {
            if (context.Source is string greeting)
            {
                return new ValueTask<ConversionResult>(ConversionResult.Success(greeting));
            }

            // If source is null or not a string, return unhandled
            return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
        }

        return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
    }
}

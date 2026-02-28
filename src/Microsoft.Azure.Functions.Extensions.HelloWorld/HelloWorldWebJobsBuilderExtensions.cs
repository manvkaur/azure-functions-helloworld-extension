// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Functions.Extensions.HelloWorld;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods for registering the Hello World extension with the WebJobs host.
/// </summary>
public static class HelloWorldWebJobsBuilderExtensions
{
    /// <summary>
    /// Adds the Hello World extension to the provided <see cref="IWebJobsBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IWebJobsBuilder"/> to configure.</param>
    /// <returns>The configured <see cref="IWebJobsBuilder"/>.</returns>
    public static IWebJobsBuilder AddHelloWorld(this IWebJobsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Register any services needed by your extension
        // For example: builder.Services.AddSingleton<IMyService, MyService>();

        // Register the extension configuration provider
        // This is the core class that initializes bindings and triggers
        builder.AddExtension<HelloWorldExtensionConfigProvider>();

        return builder;
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Functions.Extensions.HelloWorld;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Hosting;

// This assembly attribute registers the extension startup class with the Azure Functions host.
// When the host loads this extension, it will call Configure() to initialize the extension.
[assembly: WebJobsStartup(typeof(HelloWorldStartup))]

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// Startup class for the Hello World extension.
/// This is the entry point for the extension, called by the Azure Functions host during startup.
/// </summary>
public class HelloWorldStartup : IWebJobsStartup
{
    /// <summary>
    /// Configures the WebJobs host with the Hello World extension.
    /// </summary>
    /// <param name="builder">The WebJobs builder used to register services and extensions.</param>
    public void Configure(IWebJobsBuilder builder)
    {
        // Register the extension using the extension method
        builder.AddHelloWorld();
    }
}

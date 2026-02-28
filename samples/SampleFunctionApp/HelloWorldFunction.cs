// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Functions.Extensions.HelloWorld;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SampleFunctionApp;

/// <summary>
/// Sample functions that demonstrate the custom HelloWorld extension.
/// 
/// This file shows how to use:
/// - HelloWorldTrigger: Automatically invokes the function
/// - HelloWorldInput: Provides input data to the function
/// - HelloWorldOutput: Collects output from the function
/// </summary>
public class HelloWorldFunction
{
    private readonly ILogger<HelloWorldFunction> _logger;

    public HelloWorldFunction(ILogger<HelloWorldFunction> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// A function that demonstrates all three binding types:
    /// - Trigger: HelloWorldTrigger fires this function
    /// - Input: HelloWorldInput provides a greeting message
    /// - Output: HelloWorldOutput collects messages via IAsyncCollector
    /// </summary>
    [FunctionName("SayHello")]
    public async Task SayHello(
        [HelloWorldTrigger("Azure Functions Developer")] HelloWorldContext context,
        [HelloWorldInput(Greeting = "Welcome", Name = "Developer")] string inputMessage,
        [HelloWorldOutput(Prefix = "LOG")] IAsyncCollector<string> outputCollector)
    {
        _logger.LogInformation("HelloWorld trigger fired!");
        _logger.LogInformation("  Name: {Name}", context.Name);
        _logger.LogInformation("  Timestamp: {Timestamp}", context.Timestamp);
        _logger.LogInformation("  InvocationId: {InvocationId}", context.InvocationId);

        // Demonstrate input binding - the message comes from HelloWorldInputAttribute
        _logger.LogInformation("  Input binding message: {InputMessage}", inputMessage);

        // Demonstrate output binding - send messages to the collector
        await outputCollector.AddAsync($"Function triggered at {context.Timestamp}");
        await outputCollector.AddAsync($"Greeting: Hello, {context.Name}!");
    }
}

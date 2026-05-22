// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.HelloWorld;
using Microsoft.Extensions.Logging;

namespace SampleFunctionApp;

/// <summary>
/// Sample functions that demonstrate the custom HelloWorld extension.
/// 
/// This file shows how to use:
/// - HelloWorldTrigger: Automatically invokes the function
/// - HelloWorldInput: Provides a greeting message to the function
/// - HelloWorldOutput: Collects output messages from the function
/// </summary>
public class HelloWorldFunction
{
    private readonly ILogger<HelloWorldFunction> _logger;

    public HelloWorldFunction(ILogger<HelloWorldFunction> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// A function that demonstrates the HelloWorld trigger binding.
    /// The trigger fires automatically when the extension is initialized.
    /// </summary>
    [Function("SayHello")]
    public void SayHello(
        [HelloWorldTrigger("Azure Functions Developer")] HelloWorldContext context)
    {
        _logger.LogInformation("HelloWorld trigger fired!");
        _logger.LogInformation("  Name: {Name}", context.Name);
        _logger.LogInformation("  Timestamp: {Timestamp}", context.Timestamp);
        _logger.LogInformation("  InvocationId: {InvocationId}", context.InvocationId);
    }

    /// <summary>
    /// A function that demonstrates the HelloWorld input binding.
    /// The input binding provides a pre-formatted greeting message.
    /// </summary>
    [Function("GetGreeting")]
    public void GetGreeting(
        [HelloWorldTrigger("Developer")] HelloWorldContext context,
        [HelloWorldInput(Greeting = "Welcome", GreetingName = "Azure Developer")] string greeting)
    {
        _logger.LogInformation("HelloWorld input binding demo!");
        _logger.LogInformation("  Trigger Name: {Name}", context.Name);
        _logger.LogInformation("  Input Greeting: {Greeting}", greeting);
    }

    /// <summary>
    /// A function that demonstrates the HelloWorld output binding.
    /// The output binding collects messages and logs them.
    /// </summary>
    [Function("SendMessage")]
    [HelloWorldOutput(Prefix = "[HelloWorld] ")]
    public string SendMessage(
        [HelloWorldTrigger("Messenger")] HelloWorldContext context)
    {
        _logger.LogInformation("HelloWorld output binding demo!");
        _logger.LogInformation("  Trigger Name: {Name}", context.Name);
        
        // Return value is sent to the output binding
        return $"Message from {context.Name} at {context.Timestamp}";
    }
}

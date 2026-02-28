// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// Extension configuration provider for the Hello World extension.
/// This class is responsible for initializing the extension and registering bindings.
/// 
/// The [Extension] attribute defines:
/// - "HelloWorld": The extension name used in logs and diagnostics
/// - "helloWorld": The configuration section name in host.json (extensions.helloWorld)
/// </summary>
[Extension("HelloWorld", "helloWorld")]
internal sealed class HelloWorldExtensionConfigProvider : IExtensionConfigProvider
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<HelloWorldExtensionConfigProvider> _logger;

    /// <summary>
    /// Creates a new instance of the HelloWorldExtensionConfigProvider.
    /// Dependencies are injected by the DI container.
    /// </summary>
    /// <param name="loggerFactory">Factory for creating properly-categorized loggers.</param>
    public HelloWorldExtensionConfigProvider(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _logger = loggerFactory.CreateLogger<HelloWorldExtensionConfigProvider>();
    }

    /// <summary>
    /// Initializes the extension. This is called by the Azure Functions host during startup.
    /// Here you register your trigger bindings, input bindings, and output bindings.
    /// </summary>
    /// <param name="context">The extension configuration context.</param>
    public void Initialize(ExtensionConfigContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _logger.LogInformation("Hello World extension initialized!");

        // Trigger binding - uses HelloWorldTriggerAttribute
        context.AddBindingRule<HelloWorldTriggerAttribute>()
            .BindToTrigger(new HelloWorldTriggerBindingProvider(_loggerFactory));

        // Unified binding for input and output - uses HelloWorldAttribute
        // The Worker SDK strips "Input"/"Output" suffixes, so both HelloWorldInputAttribute
        // and HelloWorldOutputAttribute map to binding type "helloWorld" which matches HelloWorldAttribute.
        var outputLogger = _loggerFactory.CreateLogger<HelloWorldAsyncCollector>();
        var bindingRule = context.AddBindingRule<HelloWorldAttribute>();
        
        // Input binding - provides a greeting message
        bindingRule.BindToInput<string>(attr => $"{attr.Greeting}, {attr.GreetingName}!");
        
        // Output binding - collects messages
        bindingRule.BindToCollector<string>(attr => new HelloWorldAsyncCollector(attr, outputLogger));
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// Provides trigger bindings for the HelloWorldTrigger.
/// 
/// The binding provider is responsible for:
/// 1. Checking if a parameter has the HelloWorldTriggerAttribute
/// 2. Creating a trigger binding if the attribute is found
/// 
/// This is called by the Azure Functions host during function indexing.
/// </summary>
internal sealed class HelloWorldTriggerBindingProvider : ITriggerBindingProvider
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<HelloWorldTriggerBindingProvider> _logger;

    public HelloWorldTriggerBindingProvider(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _logger = loggerFactory.CreateLogger<HelloWorldTriggerBindingProvider>();
    }

    /// <summary>
    /// Attempts to create a trigger binding for the given parameter.
    /// </summary>
    /// <param name="context">Context containing the parameter information.</param>
    /// <returns>A trigger binding if the parameter has the HelloWorldTriggerAttribute; otherwise, null.</returns>
    public Task<ITriggerBinding?> TryCreateAsync(TriggerBindingProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var parameterInfo = context.Parameter;
        var attribute = parameterInfo.GetCustomAttribute<HelloWorldTriggerAttribute>(inherit: false);

        if (attribute is null)
        {
            // This parameter doesn't have our attribute, return null to let other providers try
            return Task.FromResult<ITriggerBinding?>(null);
        }

        _logger.LogDebug("Creating HelloWorld trigger binding for parameter '{ParameterName}'", 
            parameterInfo.Name);

        var binding = new HelloWorldTriggerBinding(parameterInfo, attribute, _loggerFactory);
        return Task.FromResult<ITriggerBinding?>(binding);
    }
}

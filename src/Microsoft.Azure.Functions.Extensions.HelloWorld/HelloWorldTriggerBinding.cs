// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld;

/// <summary>
/// Defines the trigger binding for HelloWorld.
/// 
/// A trigger binding is responsible for:
/// 1. Defining what data is available to the function (BindingDataContract)
/// 2. Binding trigger values when the function is invoked (BindAsync)
/// 3. Creating the listener that watches for trigger events (CreateListenerAsync)
/// </summary>
internal sealed class HelloWorldTriggerBinding : ITriggerBinding
{
    private readonly ParameterInfo _parameterInfo;
    private readonly HelloWorldTriggerAttribute _attribute;
    private readonly ILoggerFactory _loggerFactory;

    public HelloWorldTriggerBinding(
        ParameterInfo parameterInfo,
        HelloWorldTriggerAttribute attribute,
        ILoggerFactory loggerFactory)
    {
        _parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));
        _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

        // Define the binding data contract - what data is available to the function
        // This enables {Name} and {Timestamp} to be used in other binding expressions
        BindingDataContract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "Name", typeof(string) },
            { "Timestamp", typeof(DateTimeOffset) },
            { "InvocationId", typeof(string) }
        };
    }

    /// <summary>
    /// The type of the trigger value. This is what the listener will provide.
    /// </summary>
    public Type TriggerValueType => typeof(HelloWorldContext);

    /// <summary>
    /// Defines the binding data that functions can access via binding expressions.
    /// For example, a function could use {Name} in another binding to access the name.
    /// </summary>
    public IReadOnlyDictionary<string, Type> BindingDataContract { get; }

    /// <summary>
    /// Binds the trigger value when a function is invoked.
    /// This converts the raw trigger value into what the function parameter expects.
    /// </summary>
    /// <param name="value">The trigger value from the listener.</param>
    /// <param name="context">The binding context.</param>
    /// <returns>Trigger data containing the bound value and binding data.</returns>
    public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
    {
        var helloWorldContext = value as HelloWorldContext 
            ?? throw new InvalidOperationException("Expected HelloWorldContext");

        // Create binding data that other bindings can use
        var bindingData = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            { "Name", helloWorldContext.Name },
            { "Timestamp", helloWorldContext.Timestamp },
            { "InvocationId", helloWorldContext.InvocationId }
        };

        // Create a value provider that returns the context object to the function
        IValueProvider valueProvider = new HelloWorldValueProvider(helloWorldContext, _parameterInfo.ParameterType);

        var triggerData = new TriggerData(valueProvider, bindingData);
        return Task.FromResult<ITriggerData>(triggerData);
    }

    /// <summary>
    /// Creates the listener that will watch for trigger events and invoke the function.
    /// </summary>
    /// <param name="context">The listener factory context.</param>
    /// <returns>A listener instance.</returns>
    public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Create the listener with the function executor and attribute configuration
        // Each listener gets its own properly-categorized logger
        var listener = new HelloWorldListener(
            context.Executor,
            _attribute,
            _loggerFactory.CreateLogger<HelloWorldListener>());

        return Task.FromResult<IListener>(listener);
    }

    /// <summary>
    /// Returns a description of this parameter for diagnostic purposes.
    /// </summary>
    public ParameterDescriptor ToParameterDescriptor()
    {
        return new ParameterDescriptor
        {
            Name = _parameterInfo.Name,
            Type = "HelloWorldTrigger",
            DisplayHints = new ParameterDisplayHints
            {
                Description = "Hello World trigger",
                Prompt = "Enter a name for the greeting"
            }
        };
    }
}

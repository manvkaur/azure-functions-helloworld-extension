# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

"""
Azure Functions HelloWorld Extension - Python Sample

Demonstrates how to use the HelloWorld extension from a Python Azure Function
using the generic binding model.
"""

import json
import logging

import azure.functions as func

app = func.FunctionApp()


@app.function_name(name="SayHello")
@app.generic_trigger(arg_name="context", type="helloWorldTrigger",
                     greetingName="Python Developer")
def say_hello(context: str) -> None:
    """
    Sample function using the HelloWorld trigger binding.
    The trigger fires automatically and provides a HelloWorldContext as JSON.
    """
    logging.info("HelloWorld trigger fired!")

    hello_context = json.loads(context)
    logging.info(f"  Name: {hello_context['name']}")
    logging.info(f"  Timestamp: {hello_context['timestamp']}")
    logging.info(f"  InvocationId: {hello_context['invocationId']}")


@app.function_name(name="GetGreeting")
@app.generic_trigger(arg_name="context", type="helloWorldTrigger",
                     greetingName="Developer")
@app.generic_input_binding(arg_name="greeting", type="helloWorld",
                           greeting="Welcome", greetingName="Azure Developer")
def get_greeting(context: str, greeting: str) -> None:
    """
    Sample function demonstrating the HelloWorld input binding.
    The input binding provides a pre-formatted greeting message.
    """
    logging.info("HelloWorld input binding demo!")

    hello_context = json.loads(context)
    logging.info(f"  Trigger Name: {hello_context['name']}")
    logging.info(f"  Input Greeting: {greeting}")

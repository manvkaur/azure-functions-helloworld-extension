// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { app, InvocationContext } from '@azure/functions';

/**
 * Sample function demonstrating HelloWorld input and output bindings
 * using the generic binding model for non-.NET languages.
 */
app.generic('GetGreeting', {
    trigger: {
        type: 'helloWorldTrigger',
        name: 'context',
        greetingName: 'Developer',
    },
    extraInputs: [
        {
            type: 'helloWorld',
            name: 'greeting',
            greeting: 'Welcome',
            greetingName: 'Azure Developer',
        },
    ],
    handler: async (context: unknown, invocationContext: InvocationContext) => {
        invocationContext.log('HelloWorld input binding demo!');

        const helloContext = context as { name: string };
        const greeting = invocationContext.extraInputs.get('greeting') as string;

        invocationContext.log(`  Trigger Name: ${helloContext.name}`);
        invocationContext.log(`  Input Greeting: ${greeting}`);
    },
});

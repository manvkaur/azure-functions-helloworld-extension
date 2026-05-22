// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { app, InvocationContext } from '@azure/functions';

/**
 * Sample function using the HelloWorld trigger binding via generic input.
 *
 * In the isolated worker model for non-.NET languages, custom extensions
 * are consumed through generic bindings defined in function.json (or the
 * programmatic model equivalent). The extension bundle must include the
 * HelloWorld extension package.
 *
 * The trigger fires automatically and provides a HelloWorldContext object.
 */
app.generic('SayHello', {
    trigger: {
        type: 'helloWorldTrigger',
        name: 'context',
        greetingName: 'Node.js Developer',
    },
    handler: async (context: unknown, invocationContext: InvocationContext) => {
        invocationContext.log('HelloWorld trigger fired!');

        const helloContext = context as {
            name: string;
            timestamp: string;
            invocationId: string;
        };

        invocationContext.log(`  Name: ${helloContext.name}`);
        invocationContext.log(`  Timestamp: ${helloContext.timestamp}`);
        invocationContext.log(`  InvocationId: ${helloContext.invocationId}`);
    },
});

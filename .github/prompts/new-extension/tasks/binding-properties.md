# Task: Verify Binding Properties and AutoResolve Handling

## User Message

Create an extension called "Webhook" with only an output binding. Properties:

- `Endpoint` (string, required, %%) — The webhook URL from app settings
- `Method` (string, optional, no %%) — HTTP method, default "POST"
- `Headers` (string, optional, %%) — Custom headers expression
- `TimeoutSeconds` (int, optional, no %%) — Request timeout, default 30
- `RetryCount` (int, optional, no %%) — Number of retries, default 3
- `IncludeTimestamp` (bool, optional, no %%) — Add timestamp to payload, default false

Not on azfunc team. No host.json settings.

## Expected Behavior

1. Only output binding scaffolded (no trigger, no input)
2. `[AutoResolve]` applied to `Endpoint` and `Headers` only (the string properties with %%)
3. `Method` is a string but does NOT get `[AutoResolve]` (user said no %%)
4. `Endpoint` is validated as required at startup
5. Defaults: Method="POST", TimeoutSeconds=30, RetryCount=3, IncludeTimestamp=false
6. Worker attribute: `WebhookOutputAttribute` with matching properties
7. Generated `IAsyncCollector<T>` implementation for output
8. No trigger listener scaffolded

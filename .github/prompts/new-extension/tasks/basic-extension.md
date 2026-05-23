# Task: Scaffold a Basic Trigger-Only Extension

## User Message

I want to create a new Azure Functions extension called "RedisStreams" that connects to Redis Streams. It should only have a trigger binding (no input/output). The trigger fires when new messages arrive on a stream.

Properties for the trigger:
- `Connection` (string, required, supports %% app settings) — Redis connection string name
- `StreamName` (string, required, supports %%) — The stream key to listen on
- `ConsumerGroup` (string, optional, supports %%) — Consumer group name, defaults to "$Default"
- `PollIntervalMs` (int, optional, no %%) — Polling interval in milliseconds, default 1000

I'm not on the azfunc team. No host.json settings needed.

## Expected Behavior

1. Should NOT ask about host SDK — defaults to `Functions.Extensions`
2. Should confirm package names: `Microsoft.Azure.Functions.Extensions.RedisStreams` + `Microsoft.Azure.Functions.Worker.Extensions.RedisStreams`
3. Should scaffold trigger classes only (no input/output binding classes)
4. Should apply `[AutoResolve]` to Connection, StreamName, ConsumerGroup
5. Should make Connection and StreamName required (validation at startup)
6. Should set default for ConsumerGroup = "$Default" and PollIntervalMs = 1000
7. Should generate new key.snk
8. Should provide custom release pipeline guidance (not 1ES)
9. Should run build/test/pack validation

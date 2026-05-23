# Task: Scaffold Full Extension with All Binding Types

## User Message

Create an extension called "AiSearch" for Azure AI Search. It should have:
- A trigger that fires when an index is updated
- An input binding that reads search results
- An output binding that pushes documents to an index

Trigger properties:
- `Connection` (string, required, %%) — AI Search endpoint connection name
- `IndexName` (string, required, %%) — Index to monitor
- `PollingIntervalSeconds` (int, optional, no %%) — Default 30

Input binding properties:
- `Connection` (string, required, %%) — Same connection reference
- `IndexName` (string, required, %%) — Index to query
- `SearchQuery` (string, optional, %%) — OData filter expression
- `Top` (int, optional, no %%) — Max results, default 50

Output binding properties:
- `Connection` (string, required, %%) — Same connection reference
- `IndexName` (string, required, %%) — Target index
- `BatchSize` (int, optional, no %%) — Upload batch size, default 100

Host.json settings: `maxConcurrentIndexers` (int, default 5), `enableChangeTracking` (bool, default true).

I'm on the azfunc team.

## Expected Behavior

1. Package: `Microsoft.Azure.Functions.Extensions.AiSearch` + `Microsoft.Azure.Functions.Worker.Extensions.AiSearch`
2. Scaffolds all three binding types with separate attribute classes
3. `[AutoResolve]` on all string properties marked with %%
4. Shared `Connection` property pattern across all three bindings
5. Host.json section for `AiSearch` with `maxConcurrentIndexers` and `enableChangeTracking`
6. Uses 1ES pipeline configuration (azfunc team)
7. Worker extension has `AiSearchTriggerAttribute`, `AiSearchInputAttribute`, `AiSearchOutputAttribute`

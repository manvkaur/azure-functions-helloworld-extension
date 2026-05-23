# Task: Non-AzFunc Team Pipeline Handling

## User Message

I'm creating an extension called "Stripe" with a trigger binding for Stripe webhook events. I'm NOT on the Azure Functions team — I'm an external contributor.

Trigger properties:
- `WebhookSecret` (string, required, %%) — Stripe webhook signing secret from app settings
- `EventTypes` (string, optional, %%) — Comma-separated event types to filter, default "*"

No host.json settings needed.

## Expected Behavior

1. Should NOT generate 1ES pipeline files
2. Should replace `official-release-*.yml` with a simple GitHub Actions or generic NuGet publish workflow
3. Should explicitly note that `release-packages-*.yml` are azfunc-specific and removed
4. Should keep CI build pipelines (public-build, official-build) but suggest updating pool names
5. Package names: `Microsoft.Azure.Functions.Extensions.Stripe` + `Microsoft.Azure.Functions.Worker.Extensions.Stripe`
6. `[AutoResolve]` on WebhookSecret and EventTypes
7. Should NOT reference partner drops, NuGet signing via 1ES, or azfunc-specific approval groups

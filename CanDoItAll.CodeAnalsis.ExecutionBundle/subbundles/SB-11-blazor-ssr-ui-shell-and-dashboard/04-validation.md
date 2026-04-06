# Validation for SB-11 — Blazor SSR UI shell and dashboard

## Acceptance criteria
- A user can trigger an analysis and inspect a summary from the UI.
- The UI remains useful without client-side graph rendering.
- UI logic does not leak into the core application contracts.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Web/CanDoItAll.CodeAnalytics.Tests.Web.csproj --filter Dashboard`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if the UI starts driving the domain model design.
- Do not continue if the first UI requires heavy JavaScript for basic usefulness.

## Evidence expected
- SSR page screenshots or test output for home and dashboard routes.
- Web tests proving the main render path and command flow.

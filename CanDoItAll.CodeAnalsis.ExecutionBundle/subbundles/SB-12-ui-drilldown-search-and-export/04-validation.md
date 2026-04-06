# Validation for SB-12 — UI drilldown search and export

## Acceptance criteria
- A user can drill into the main architecture views without reading raw snapshot JSON.
- Exports are discoverable and diagnostics remain visible.
- UI complexity stays modular and maintainable.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Web/CanDoItAll.CodeAnalytics.Tests.Web.csproj --filter Drilldown`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Web/CanDoItAll.CodeAnalytics.Tests.Web.csproj --filter Export`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if drilldowns require re-running raw collectors in the UI layer.
- Do not continue if export selection is hardcoded or hidden.

## Evidence expected
- Web tests for navigation, filtering, and export flows.
- Screenshots or render proofs for key drilldown surfaces.

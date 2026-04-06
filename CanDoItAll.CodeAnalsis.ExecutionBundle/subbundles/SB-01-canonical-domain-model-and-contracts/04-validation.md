# Validation for SB-01 — Canonical domain model and contracts

## Acceptance criteria
- The model can represent solution inventory, symbol facts, DI facts, persistence facts, findings, diagnostics, and exports without ambiguity.
- Facts and insights cannot be confused at the root shape.
- The future MCP driver can consume the application output without changing core model types.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj --filter CanonicalModel`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj --filter Serialization`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if any Roslyn symbol types leak into public contracts.
- Do not continue if the snapshot root mixes facts and insights.

## Evidence expected
- Golden JSON for at least one minimal and one richer snapshot.
- Unit tests covering ordering, required defaults, and enum/string compatibility.

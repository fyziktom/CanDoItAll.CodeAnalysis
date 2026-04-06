# Validation for SB-10 — Application orchestrator and query API

## Acceptance criteria
- Callers can build and query snapshots without referencing Roslyn, EF, or renderer internals.
- Errors and diagnostics are transport-ready and deterministic.
- The future MCP driver seam is obvious and thin.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj --filter Application`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Integration/CanDoItAll.CodeAnalytics.Tests.Integration.csproj --filter Orchestrator`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if application services leak Roslyn types or UI models.
- Do not continue if error categories are too vague for future MCP wrapping.

## Evidence expected
- Service orchestration tests.
- Query contract examples that map cleanly to future tool surfaces.

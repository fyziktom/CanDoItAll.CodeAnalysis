# Validation for SB-06 — EF Core and persistence view

## Acceptance criteria
- The snapshot can express a useful ER-level persistence view.
- Unsupported mappings degrade gracefully with diagnostics.
- ER export inputs are deterministic and UI-ready.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Integration/CanDoItAll.CodeAnalytics.Tests.Integration.csproj --filter Persistence`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj --filter EfCore`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if EF extraction requires a live database.
- Do not continue if relationship facts are only encoded as Mermaid text instead of canonical records.

## Evidence expected
- Fixture-based persistence tests and ER golden outputs.
- At least one diagnostic example for unsupported/ambiguous mappings.

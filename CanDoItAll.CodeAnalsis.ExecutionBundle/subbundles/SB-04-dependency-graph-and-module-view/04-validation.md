# Validation for SB-04 — Dependency graph and module view

## Acceptance criteria
- Project and module graphs are deterministic and analyzable.
- Cycles and graph metrics are exposed as first-class facts/insights where appropriate.
- Later renderers can consume the graph data without depending on Roslyn directly.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj --filter DependencyGraph`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Integration/CanDoItAll.CodeAnalytics.Tests.Integration.csproj --filter ModuleView`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if module grouping is non-deterministic or hidden inside UI code.
- Do not continue if graph algorithms only work for tiny toy solutions.

## Evidence expected
- Golden graph output for a fixture solution.
- Tests proving cycle detection and module grouping.

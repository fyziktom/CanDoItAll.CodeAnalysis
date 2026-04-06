# Validation for SB-02 — Workspace loading and solution inventory

## Acceptance criteria
- A valid solution path produces a repeatable inventory snapshot.
- Invalid paths and load failures are explicit and actionable.
- Inventory output is deterministic across repeated runs.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Integration/CanDoItAll.CodeAnalytics.Tests.Integration.csproj --filter Workspace`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Integration/CanDoItAll.CodeAnalytics.Tests.Integration.csproj --filter SolutionInventory`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if workspace loading still leaks Roslyn types into the application layer.
- Do not continue if invalid paths produce silent or cryptic failures.

## Evidence expected
- Integration tests for valid solution, invalid path, and partial-load cases.
- A deterministic inventory sample committed as a golden file.

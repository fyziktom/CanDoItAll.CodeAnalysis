# Validation for SB-07 — Risk rules and insights

## Acceptance criteria
- Findings are deterministic, evidence-backed, and clearly separated from facts.
- The engine can represent uncertainty without going silent.
- At least a useful first set of architecture risks is covered.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj --filter RiskRules`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Integration/CanDoItAll.CodeAnalytics.Tests.Integration.csproj --filter Findings`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if findings lack evidence links.
- Do not continue if facts and insights are merged in one collection.

## Evidence expected
- Rule tests with positive and negative cases.
- Golden snapshot fragments showing both findings and open questions.

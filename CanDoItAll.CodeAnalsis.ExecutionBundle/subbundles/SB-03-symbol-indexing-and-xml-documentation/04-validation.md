# Validation for SB-03 — Symbol indexing and XML documentation ingestion

## Acceptance criteria
- The snapshot can represent meaningful type and member facts for a realistic fixture solution.
- Missing or malformed XML documentation is reported without breaking the run.
- Symbol ordering and identifiers remain deterministic.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Integration/CanDoItAll.CodeAnalytics.Tests.Integration.csproj --filter Symbol`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj --filter XmlDocumentation`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if XML doc ingestion breaks runs on projects that omit XML docs.
- Do not continue if symbol identity is unstable across repeated runs.

## Evidence expected
- Fixture-based tests covering common C# symbol shapes.
- At least one golden snapshot fragment showing type/member facts and XML summaries.

# Validation for SB-05 — DI and service registration analysis

## Acceptance criteria
- Conventional registration patterns are captured correctly.
- Unsupported patterns produce explicit diagnostics rather than silent omission.
- Registrations are linked to source locations and projects.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Integration/CanDoItAll.CodeAnalytics.Tests.Integration.csproj --filter DiRegistration`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj --filter ServiceRegistration`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if DI facts are mixed with inferred roles without clear provenance.
- Do not continue if unsupported DI patterns disappear silently.

## Evidence expected
- Fixture tests proving service lifetime extraction and ambiguous-pattern diagnostics.
- Golden snapshot fragment showing DI facts and warnings.

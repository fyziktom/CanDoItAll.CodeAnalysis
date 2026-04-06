# Validation for SB-08 — Snapshot assembly, serialization, and caching

## Acceptance criteria
- A completed analysis run produces a stable snapshot package on disk.
- Repeated identical requests can reuse cached results safely when appropriate.
- Storage contracts remain host-agnostic.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj --filter SnapshotRepository`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Integration/CanDoItAll.CodeAnalytics.Tests.Integration.csproj --filter SnapshotAssembly`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if storage format is non-deterministic.
- Do not continue if cache behavior is opaque or impossible to diagnose.

## Evidence expected
- Golden JSON snapshot package.
- Cache behavior tests proving deterministic hits/misses and version invalidation.

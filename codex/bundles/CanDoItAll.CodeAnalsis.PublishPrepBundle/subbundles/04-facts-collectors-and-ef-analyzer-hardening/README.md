# Facts Collectors And EF Analyzer Hardening

## Status

- `Completed`

## Objective

- Harden Roslyn facts collectors and persistence/EF analyzer behavior, and decide whether EF-specific analysis should become an addon project.

## Success Criteria

- Persistence analyzer has clear ownership and tests for DbContext/entity/relationship/model-snapshot/configuration behavior.
- EF Core query optimization guidance is reflected as analyzer capability or documented as out of current scope.
- No production code is mislabeled as runtime EF query execution when it only statically analyzes EF code.

## Covered Inputs

- `IN-002`
- `IN-005`
- `IN-008`
- `REQ-006`
- `REQ-007`

## Prerequisites

- `SB01` validation baseline passed.
- `SB02` extraction decision completed.
- Re-read EF conclusion in `bundle://analysis/01-current-state.md`.

## Exact Source References

- `repo://src/CanDoItAll.CodeAnalytics.Facts/Persistence/PersistenceFactCollector.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Facts/Persistence/PersistenceFactCollector.EntityRelationships.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Facts/Persistence/PersistenceFactCollector.ProjectAnalysis.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Facts/Persistence/PersistenceFactCollector.ModelSnapshots.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Facts/Persistence/PersistenceSyntaxExplorer.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Facts/Persistence/PersistenceSyntaxExplorer.ModelSnapshots.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Facts/Services/ServiceRegistrationCollector.cs`
- `repo://tests/fixtures/Fixture.Shop/src/Fixture.Shop.Infrastructure/Persistence/ShopDbContext.cs`
- `repo://tests/fixtures/Fixture.Shop/src/Fixture.Shop.Infrastructure/Persistence/ReportingDbContext.cs`
- `repo://tests/CanDoItAll.CodeAnalytics.Tests.Unit/EfCoreFacts.cs`
- `repo://tests/CanDoItAll.CodeAnalytics.Tests.Integration/PersistenceFacts.cs`

## Deliverables

- Decision: keep persistence analyzer inside Facts or extract `Facts.EfCore` addon.
- Refactored analyzer components if approved by `SB02`.
- Fixture coverage for EF relationships, model snapshots, configuration discovery, and optional query anti-pattern facts.
- Tests proving no N+1/tracking/split-query claims are made unless analyzer detects real query shapes.
- `proof/SB04/manifest.md` and `proof/SB04/semantic-invariants.md`.

## Dependency Impact

- `SB05` performance work may optimize collector traversals only after analyzer behavior is stable.
- `SB07` package metadata depends on whether EF analysis is core or optional addon.
- `SB08` docs depend on accurate EF capability claims.

## Validation Depth

- Critical foundation.
- Requires Semantic Adequacy Gate proof for EF analyzer facts and diagnostics.

## Implementation Steps

1. Confirm whether production code still has no runtime EF query execution.
2. Decide analyzer ownership from `SB02`.
3. Split persistence collector responsibilities if needed: project analysis, DbContext discovery, entity resolution, relationship mapping, model snapshots, diagnostics.
4. Add fixture cases for EF query anti-pattern detection only if the product will claim that capability.
5. Add negative tests proving runtime EF query advice is not emitted for static metadata-only facts.
6. Run targeted EF unit/integration tests and full validation matrix.

## Scope Exceptions

- Do not add runtime EF data access to the app.
- Do not add `AsNoTracking` changes to fixtures unless they are part of a deliberate analyzer fixture.

## Do Not Do

- Do not label fixture EF usage as production app query behavior.
- Do not claim N+1 detection, compiled-query advice, or split-query advice in docs unless implementation and tests prove it.
- Do not make EF packages required by core layers unless the addon decision says so.

## Acceptance Checklist

- EF analyzer responsibilities are smaller and documented.
- Tests include realistic positive and adversarial EF cases.
- Package/reference direction respects the `SB02` architecture decision.
- Public docs capability claims are deferred to `SB08`.
- Build, targeted tests, full validation matrix, and file-length validation pass.

## Proof Required

- `bundle://proof/SB04/manifest.md`
- `bundle://proof/SB04/semantic-invariants.md`
- Failing-first and passing transcripts for EF analyzer semantic cases.
- Source assertions for production analyzer code.
- Anti-stub audit transcript.
- Changed-file SHA-256 hashes.

## Browser Validation Logging

- N/A: analyzer behavior is not browser-visible unless Web persistence page response shape changes; if it does, record a desktop-large smoke pass for `/snapshots/{id}/persistence`.

## Progression Gate

- `SB05`, `SB07`, and `SB08` may rely on EF capability claims only after this subbundle passes semantic proof.

## Suggested Agent Prompt

```text
Implement SB04 only. Harden the facts/persistence/EF analyzer boundary, prove analyzer behavior with semantic tests, update proof/SB04, and stop before performance or documentation claims.
```

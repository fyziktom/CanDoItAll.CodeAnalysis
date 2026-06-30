# Application Service And Focused Context Refactor

## Status

- `Completed`

## Objective

- Split oversized application-service partial files into maintainable services/helpers while preserving symbol, focused-context, document, export, and snapshot query behavior.

## Success Criteria

- `CodeAnalyticsApplicationService` becomes a facade/orchestrator instead of owning scoring, traversal, source file I/O, and response shaping directly.
- Oversized production files no longer exceed release guardrails or have explicit accepted exceptions.
- Existing application behavior is proven by semantic positive and negative tests.

## Covered Inputs

- `IN-002`
- `IN-003`
- `IN-005`
- `IN-007`
- `REQ-003`
- `REQ-005`
- `REQ-007`

## Prerequisites

- `SB01` validation baseline passed.
- `SB02` boundary decision completed.
- If `SB02` creates a focused-context project, use that destination; otherwise use internal services under Application.

## Exact Source References

- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Build.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Queries.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.Source.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.Strategy.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.SeedResolution.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.Excerpts.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Abstractions/ICodeAnalyticsApplicationService.cs`
- `repo://tests/CanDoItAll.CodeAnalytics.Tests.Unit/ApplicationFacts.cs`

## Deliverables

- Focused-context strategy/selection/scoring components or project.
- Symbol query/reference components separated from the application facade.
- Source excerpt reader abstraction or helper with bounded behavior.
- Tests covering existing semantic behavior and adverse shallow-pass cases.
- `proof/SB03/manifest.md` and `proof/SB03/semantic-invariants.md`.

## Dependency Impact

- `SB05` performance work depends on stable query/focused-context boundaries.
- `SB06` UI work depends on stable response behavior.
- `SB08` docs depend on the final public application API and examples.

## Validation Depth

- Critical foundation.
- Requires Semantic Adequacy Gate proof for focused-context and symbol workflows.

## Implementation Steps

1. Re-run file-length inventory after `SB02` to identify exact split targets.
2. Extract focused-context strategy, seed resolution, selection, and excerpt reading behind small internal contracts.
3. Extract symbol query/search/reference shaping behind small internal contracts.
4. Keep `ICodeAnalyticsApplicationService` compatible unless `SB02` explicitly approved a contract change.
5. Add/adjust tests for symbol search, references, focused-context seed resolution, representative consumers, and broad-selection warnings.
6. Run targeted unit/integration tests, full validation matrix from `SB01`, and file-length guardrail.

## Scope Exceptions

- Do not tune Web component layout; `SB06` owns UI.
- Do not optimize LINQ blindly; only restructure for ownership unless a measured bottleneck is proven.

## Do Not Do

- Do not change stable IDs, snapshot serialization, or export paths as incidental refactor fallout.
- Do not accept tests that only assert non-empty responses.
- Do not remove behavior currently exercised by comparison/scenario harnesses.

## Acceptance Checklist

- Production oversized Application files are split or explicitly justified.
- Facade dependencies are narrower and injectable/testable.
- Semantic tests include realistic positive focused-context/symbol scenarios.
- Adversarial tests reject shallow matching, fixture-only behavior, or broad whole-file context.
- Build, targeted tests, full validation matrix, and file-length validation pass.

## Proof Required

- `bundle://proof/SB03/manifest.md`
- `bundle://proof/SB03/semantic-invariants.md`
- Failing-first and passing transcripts for semantic focused-context/symbol tests.
- Changed-file SHA-256 hashes.
- Source assertions showing production code owns the new behavior outside tests.
- Anti-stub audit transcript.
- Downstream smoke proof for Web routes that consume changed responses if response shapes are touched.

## Browser Validation Logging

- If response shape affects Web rendering, record smoke navigation for `/snapshots/{id}/context` and `/snapshots/{id}/symbols` at desktop-large viewport; otherwise N/A.

## Progression Gate

- `SB05` and `SB06` may proceed. Application behavior tests pass and no oversized Application service file remains. Remaining hard-limit files are Web/test owned.

## Suggested Agent Prompt

```text
Implement SB03 only. Refactor application/focused-context/symbol responsibilities behind stable contracts, prove semantic behavior, update proof/SB03, and stop before storage/performance or UI work.
```

# Architecture Seams And Project Extraction

## Status

- `Completed`

## Objective

- Decide the maintainable project, helper, driver, and addon boundaries before moving code.

## Success Criteria

- Extraction decisions are documented with allowed references, package intent, migration order, and rollback plan.
- No big-bang move is required; later subbundles know whether to create projects or only internal services.

## Covered Inputs

- `IN-002`
- `IN-003`
- `IN-005`
- `REQ-004`
- `REQ-009`
- `REQ-010`

## Prerequisites

- `SB01` validation baseline must pass.
- Current source references in `bundle://architecture/01-target-solution.md` must be rechecked against the repo before editing.

## Exact Source References

- `repo://CanDoItAll.CodeAnalsis.slnx`
- `repo://Directory.Build.props`
- `repo://src/CanDoItAll.CodeAnalytics.Abstractions/CanDoItAll.CodeAnalytics.Abstractions.csproj`
- `repo://src/CanDoItAll.CodeAnalytics.Application/CanDoItAll.CodeAnalytics.Application.csproj`
- `repo://src/CanDoItAll.CodeAnalytics.Facts/CanDoItAll.CodeAnalytics.Facts.csproj`
- `repo://src/CanDoItAll.CodeAnalytics.Rendering/CanDoItAll.CodeAnalytics.Rendering.csproj`
- `repo://src/CanDoItAll.CodeAnalytics.Storage/CanDoItAll.CodeAnalytics.Storage.csproj`
- `repo://src/CanDoItAll.CodeAnalytics.Web/CanDoItAll.CodeAnalytics.Web.csproj`
- `repo://reference/compatibility-matrix.md`
- `repo://reference/reuse-later-vs-do-not-duplicate-now.md`
- `bundle://architecture/01-target-solution.md`

## Deliverables

- ADR or architecture note deciding which candidates become projects, which remain internal helpers, and which stay future-only.
- Updated solution/project references if project extraction is approved.
- Architecture tests protecting reference direction and non-shipping project boundaries.
- `proof/SB02/manifest.md` and `proof/SB02/semantic-invariants.md`.

## Dependency Impact

- `SB03`, `SB04`, `SB05`, `SB06`, `SB07`, and `SB08` depend on these boundaries. If boundaries are wrong, downstream code movement, package metadata, and docs will be misleading.

## Validation Depth

- Critical foundation.
- Requires Semantic Adequacy Gate proof, including an adversarial reference-cycle case and a positive build/package layout case.

## Implementation Steps

1. Review current project graph and the candidate table in `bundle://architecture/01-target-solution.md`.
2. Decide for each candidate: new project now, internal service/helper now, future addon, or no action.
3. Add or update ADR(s) under `repo://architecture/adrs`.
4. If new projects are created, update `.slnx`, references, tests, and file-length exclusions consciously.
5. Add/adjust architecture tests for allowed dependency direction and non-shipping tools/Web boundaries.
6. Run build, architecture tests, solution-structure validation, and bundle prepared-stage validator.

## Scope Exceptions

- Do not implement future MCP driver runtime code unless a new explicit requirement is added.
- Do not publish packages yet; `SB07` owns package metadata and validation.

## Do Not Do

- Do not copy host-specific `CanDoItAll.Mcp.Core` contracts into engine libraries.
- Do not create addon projects that only move files without reducing dependency or ownership complexity.
- Do not rename the repository or canonical solution spelling as a side effect.

## Acceptance Checklist

- Every extraction candidate has a documented decision.
- New or retained boundaries have tests.
- No unexpected project reference cycles exist.
- README/reference docs are updated only enough to reflect architectural decisions; full docs wait for `SB08`.

## Proof Required

- `bundle://proof/SB02/manifest.md`
- `bundle://proof/SB02/semantic-invariants.md`
- Build and architecture-test transcripts.
- Source assertions for project references and ADR text.
- Anti-stub audit transcript.

## Browser Validation Logging

- N/A: this subbundle does not change browser-visible UI unless project movement breaks the Web build; if it does, record that as command proof rather than browser proof.

## Progression Gate

- Downstream refactor subbundles may start. ADR 0001 documents current, internal, future-addon, and future-driver boundaries; build, structure guardrail, and architecture tests pass.

## Suggested Agent Prompt

```text
Implement SB02 only. Decide and protect project boundaries, update architecture artifacts and tests, capture proof/SB02, and stop before service or UI refactoring.
```

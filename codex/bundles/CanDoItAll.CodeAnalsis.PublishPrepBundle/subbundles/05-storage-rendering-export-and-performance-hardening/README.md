# Storage Rendering Export And Performance Hardening

## Status

- `Completed`

## Objective

- Address measured performance and robustness issues in storage, rendering, export, source reading, and query hot paths after architecture and analyzer behavior stabilize.

## Success Criteria

- Performance changes are supported by scenario or benchmark evidence.
- Snapshot storage/export behavior remains deterministic and secure.
- Dynamic regex/source reading risks are addressed or explicitly bounded.

## Covered Inputs

- `IN-002`
- `IN-007`
- `REQ-007`
- `REQ-009`

## Prerequisites

- `SB01` validation baseline passed.
- `SB03` application boundaries passed.
- `SB04` analyzer boundaries passed if collector performance is touched.

## Exact Source References

- `repo://src/CanDoItAll.CodeAnalytics.Storage/Snapshots/FileSnapshotRepository.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Storage/Snapshots/SnapshotJsonSerializer.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Rendering/Exports/ExportBundleBuilder.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Rendering/Markdown/MarkdownSummaryWriter.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Rendering/Mermaid/ClassDiagramMermaidRenderer.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Rendering/Mermaid/ErDiagramMermaidRenderer.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.Source.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.Excerpts.cs`
- `repo://tools/ScenarioEvaluationHarness/Program.cs`
- `repo://tools/ComparisonHarness/Program.cs`

## Deliverables

- Performance baseline and before/after scenario evidence for selected hot paths.
- Dynamic regex safety decision, including timeout and caching/non-compiled tradeoff.
- Bounded source reading behavior or explicit max-file policy.
- Storage/export path traversal and determinism tests where relevant.
- Updated performance findings in execution report.

## Dependency Impact

- `SB07` package validation and `SB08` docs depend on stable export/storage behavior.
- `SB06` may depend on response performance for desktop sandbox interactions.

## Validation Depth

- Performance and robustness hardening with targeted semantic proof when behavior changes.

## Implementation Steps

1. Select hot paths from `PERF-001` through `PERF-004`.
2. Add scenario harness or benchmark baseline before production changes.
3. Implement only changes with observable benefit or clear safety value.
4. Add tests for deterministic export ordering, path traversal rejection, regex timeout/safety, and bounded source reads as applicable.
5. Run build, targeted tests, validation matrix, and scenario/benchmark comparisons.

## Scope Exceptions

- Do not optimize every LINQ call.
- Do not rewrite storage or rendering architecture unless `SB02` approved a driver/package split.

## Do Not Do

- Do not trade readability for unmeasured micro-optimizations.
- Do not change snapshot IDs, export relative paths, or JSON contract without explicit tests and downstream updates.
- Do not use unsafe code for minor allocation wins.

## Acceptance Checklist

- Every performance claim has before/after evidence or is removed.
- Regex and source-read behavior are safe for public use.
- Storage/export tests cover security and determinism.
- Build, tests, validation matrix, and file-length validation pass.

## Proof Required

- Command transcripts for build/tests/scenario or benchmark runs.
- Source assertions for safety changes.
- If behavior changes, semantic invariant proof with negative and positive cases.
- Anti-stub audit transcript.

## Browser Validation Logging

- If response latency or export routes are changed, record desktop-large smoke proof for `/snapshots/{id}/exports`; otherwise N/A.

## Progression Gate

- `SB07` may proceed only after storage/export behavior is deterministic and performance claims are evidence-backed.

## Suggested Agent Prompt

```text
Implement SB05 only. Measure first, harden selected storage/rendering/export/performance paths, capture proof, and stop before UI or publishing metadata work.
```

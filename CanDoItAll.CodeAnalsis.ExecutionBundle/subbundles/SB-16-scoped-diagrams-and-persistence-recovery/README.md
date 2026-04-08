# SB-16 Scoped diagrams and persistence recovery

## Status

- Completed

## Objective

- Improve diagram usefulness through scope-aware selection and recover more real EF relationships on the host solution.

## Covered Inputs

- Improve class diagram usefulness
- Improve entity diagram usefulness
- Improve database schema search and relation recovery

## Prerequisites

- `SB-15` completed
- Baseline `SB-06` and `SB-09` remain trusted

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Rendering\Mermaid`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Rendering\Exports`
- `C:\repositories\CanDoItAll.CodeAnalsis\output\playwright\host-analysis-v2`

## Deliverables

- Project-scoped or neighborhood-scoped diagram generation
- Better connected-node selection heuristics
- Improved EF relationship recovery beyond the current small host count

## Dependency Impact

- `SB-18` depends on clearer diagrams and stronger persistence data.
- `SB-19` depends on the host usefulness comparison after this phase.

## Validation Depth

- Build, tests, Mermaid CLI rendering, host-solution rerun, and exports-page review

## Implementation Steps

1. Improve diagram selection and scoping.
2. Strengthen persistence relation discovery.
3. Re-run the host analysis and review the outputs as architecture artifacts.

## Do Not Do

- Do not push new truth into renderers that belongs in facts.
- Do not accept a higher relationship count if the cardinality or source links become misleading.

## Acceptance Checklist

- Scoped diagram outputs exist and are more readable than the whole-solution baseline.
- Host persistence relationship recovery is materially improved.
- Mermaid outputs render cleanly.

## Proof Required

- Unit and integration tests
- Mermaid CLI render proof
- Host rerun artifacts and screenshots

## Browser Validation Logging

- Log the exports and persistence routes after the host rerun.

## Progression Gate

- Later UI and comparison work may continue only if the host outputs are actually more useful, not just more complex.

## Suggested Agent Prompt

Improve usefulness, not just density. Scope diagrams intentionally and strengthen EF relation recovery from facts.

# SB-04 Dependency graph and module view

## Status

- Completed

## Objective

- Build the first dependency graph, module view, and type relationship baseline.

## Covered Inputs

- Dependency graph requirement
- Module-level view requirement

## Prerequisites

- `SB-03` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Dependencies`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Analysis\Graphs`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Rendering\Mermaid`

## Deliverables

- Dependency edges
- Module facts
- Initial type relationship graph

## Dependency Impact

- Summary views and future scoped diagrams depend on this baseline.

## Validation Depth

- Build, unit tests, and snapshot export proof

## Implementation Steps

1. Group symbols into modules.
2. Build dependency and relationship edges.
3. Expose graph facts to insights and renderers.

## Do Not Do

- Do not let Mermaid rendering become the source of truth.

## Acceptance Checklist

- Dependency graph is deterministic.
- Module view and type relationships are exported from facts.

## Proof Required

- Unit tests for graph generation
- Export tests for dependency outputs

## Browser Validation Logging

- N/A

## Progression Gate

- Later diagram work can build on a stable dependency baseline.

## Suggested Agent Prompt

Implement graph facts first. Treat renderers as pure consumers.

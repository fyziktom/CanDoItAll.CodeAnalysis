# SB-02 Workspace loading and solution inventory

## Status

- Completed

## Objective

- Load `.slnx` and project inputs through Roslyn and expose normalized workspace inventory.

## Covered Inputs

- Roslyn-first solution loading
- Support for solution and project-level analysis scope

## Prerequisites

- `SB-01` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Workspace`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\fixtures`
- `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\overview\05-analysis-pipeline.md`

## Deliverables

- MSBuild workspace loader
- Workspace load results and normalized project inventory

## Dependency Impact

- All fact collectors depend on correct workspace loading.

## Validation Depth

- Build, unit tests, and fixture-solution validation

## Implementation Steps

1. Register MSBuild and open the requested workspace.
2. Normalize paths and project metadata.
3. Preserve project-scoped execution support.

## Do Not Do

- Do not let renderers or UI own workspace loading concerns.

## Acceptance Checklist

- Solution and project inputs both load.
- Workspace inventory is deterministic.

## Proof Required

- Integration tests against the fixture solution

## Browser Validation Logging

- N/A

## Progression Gate

- Collectors can rely on stable workspace inputs.

## Suggested Agent Prompt

Focus on Roslyn workspace loading and deterministic inventory only.

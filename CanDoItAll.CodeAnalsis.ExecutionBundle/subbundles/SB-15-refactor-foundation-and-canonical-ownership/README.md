# SB-15 Refactor foundation and canonical ownership

## Status

- Completed

## Objective

- Perform the comparison-driven readability and ownership refactor pass so the focused-context implementation remains understandable while the new heuristics land.

## Covered Inputs

- Refactor first
- Analyze whole solution and find architecture gaps
- Focus on long files, sources of truth, and isolation of helpers
- Include generic readability, structure, and standard best-practice improvements in the focused-context code

## Prerequisites

- `SB-14` completed baseline remains trusted
- Bundle repair completed and readiness gate passed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\Components\Pages\ContextLab.razor`
- `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\01-current-state.md`

## Deliverables

- Smaller and clearer focused-context service slices
- Reduced heuristic complexity where the comparison exposed muddled responsibilities
- Explicit ownership for scoring, seeding, excerpt assembly, and UI quality cues

## Dependency Impact

- Blocks the new heuristic work because comparison-driven fixes are easier to validate when the scoring and excerpt code is not tangled.

## Validation Depth

- Full build and test run plus hotspot review

## Implementation Steps

1. Refactor the focused-context service code only where readability or ownership clarity is weak.
2. Keep source-of-truth placement explicit between seeding, scoring, and excerpt assembly.
3. Update tests if refactor changes internal ownership boundaries.

## Do Not Do

- Do not widen feature scope during the refactor pass.
- Do not hide logic in generic `Helpers` folders.

## Acceptance Checklist

- The focused-context implementation is easier to explain by responsibility.
- New heuristics do not pile additional branching into already unclear files.
- Build and tests still pass.

## Proof Required

- `dotnet build .\CanDoItAll.CodeAnalsis.slnx -warnaserror`
- `dotnet test .\CanDoItAll.CodeAnalsis.slnx --no-build`
- Updated hotspot inventory or refactor report

## Browser Validation Logging

- N/A

## Progression Gate

- Later work may continue only after refactor proof shows the new features will land on cleaner ownership seams.

## Completion Notes

- Focused-context responsibilities are now easier to follow:
  - seed scoring and exact-type gating live in `CodeAnalyticsApplicationService.Context.SeedResolution.cs`
  - member fan-out limits live in `CodeAnalyticsApplicationService.Context.Selection.Members.cs`
  - excerpt assembly and representative fallbacks live in `CodeAnalyticsApplicationService.Context.Excerpts.cs`
  - lab quality feedback lives in `ContextLab.razor`
- Build and full tests passed after the refactor-driven cleanup.

## Suggested Agent Prompt

Refactor only where the new focused-context comparison exposed readability or ownership debt. Keep behavior stable while preparing the heuristic fixes.

# SB-19 Validation and MCP seam review

## Status

- Completed

## Objective

- Rerun the validation matrix after the comparison-driven fixes, compare the three host cases against SharpTools with an explicit rubric, and confirm the future MCP seam remains thin.

## Covered Inputs

- Test on `C:\repositories\CanDoItAll\CanDoItAll.slnx`
- Analyze whether the information is really helpful
- Compare with SharpTools call count and context cost
- Explain how to make it better if the savings are still weak
- Explain what feedback the new tuning page needs for the next heuristic pass
- Analyze one database, one common-helper, and one UI case explicitly
- Compare helpfulness and noise, not only counts

## Prerequisites

- `SB-16` completed
- `SB-17` completed
- `SB-18` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx`
- `C:\repositories\CanDoItAll.CodeAnalsis\output\playwright`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests`
- `C:\repositories\CanDoItAll\CanDoItAll.slnx`

## Deliverables

- Final validation matrix
- Host usefulness assessment
- SharpTools comparison analysis
- Tuning-feedback guidance
- Updated bundle closure evidence
- Explicit three-case rubric and before-versus-after rerun notes

## Dependency Impact

- This phase closes the reopened initiative and determines whether follow-up work is required.

## Validation Depth

- Full build, tests, Mermaid rendering, Playwright proof, host rerun, SharpTools comparison, and bundle validator completion

## Implementation Steps

1. Run the full validation matrix.
2. Rerun the database, helper, and UI comparison cases in the improved lab.
3. Compare those same cases against SharpTools with an explicit usefulness, noise, and operator-cost rubric.
4. Review the future MCP seam and record residual risks honestly.

## Do Not Do

- Do not claim success if context savings are still weak without explaining why.
- Do not skip the bundle final-closure updates.

## Acceptance Checklist

- The reopened validation matrix passes.
- The comparison against SharpTools is evidence-based.
- The tuning guidance is explicit enough to shape the next heuristic pass.
- Residual risks and next improvements are explicit.
- The helper case is no longer blocked by a correctness failure.
- The comparison explains where focused context should stop and hand over to SharpTools instead of pretending one tool should do both jobs.

## Proof Required

- Full build and test commands
- Mermaid CLI render commands
- Playwright artifacts
- SharpTools call log and comparison notes
- Tuning-feedback write-up tied to the lab page output
- Final bundle validator pass
- Before-and-after notes for the database, helper, and UI cases

## Browser Validation Logging

- Required for the final focused-context UI flow and updated exports review.

## Progression Gate

- Final closure passes only if the reopened value claim is supported by actual evidence.

## Completion Notes

- Final validation commands passed:
  - `dotnet build C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx -nologo`
  - `dotnet test C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx -nologo`
- Host rerun summary:
  - `AppDbContext` improved materially and is now a useful first-pass bundle.
  - `IClock` no longer fails, but still needs another helper-specific tuning pass.
  - `CanvasSceneHost` remained a strong focused case.
- The future MCP seam remains thin. The remaining issue is scoring policy for high-reuse helper symbols, not architecture boundary drift.

## Suggested Agent Prompt

Close the new reopen with evidence. Measure where the improved focused-context flow beats SharpTools, where it still should hand off, and whether the helper crash and broad-noise defects are truly closed.

# SB-27 Validation and comparison rerun

## Status

- Completed

## Objective

- Revalidate the product after the new improvements land and restage the comparison against `AppDbContext`, `IClock`, and `CanvasSceneHost`.

## Covered Inputs

- revalidate how we stand after they are implemented

## Prerequisites

- SB-24 passed
- SB-25 passed
- SB-26 passed

## Exact Source References

- C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle
- C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle
- C:\repositories\CanDoItAll\CanDoItAll.slnx

## Deliverables

- Updated build and test validation
- Fresh rerun metrics for the three comparison scenarios
- An updated position statement for focused-context versus SharpTools

## Dependency Impact

- This phase closes the reopen and determines whether the new improvements actually moved the quality bar.

## Validation Depth

- Build
- Tests
- Host rerun metrics
- Browser proof where the UI changed

## Implementation Steps

1. Run the clean build and test confirmation after the new implementation lands.
2. Rerun the focused-context comparison scenarios against the host solution.
3. Compare the new standing with the previous comparison bundle results.
4. Update the execution report, raw-note closure, and residual risks honestly.

## Do Not Do

- Do not claim improvement without rerunning the same named scenarios.
- Do not hide regressions inside aggregate numbers.

## Acceptance Checklist

- Build and tests pass.
- `AppDbContext`, `IClock`, and `CanvasSceneHost` are rerun after the new implementation.
- The new standing versus SharpTools is written down clearly.
- The execution report and bundle status are synchronized.

## Proof Required

- Build and test output
- Host rerun metrics and artifacts
- Updated comparison narrative
- Completed-stage validator output

## Browser Validation Logging

- Reuse the changed lab proof from SB-26 and add any extra route checks needed by the rerun

## Progression Gate

- The bundle closes only when the rerun evidence shows the new standing honestly, including remaining gaps.

## Suggested Agent Prompt

After implementation, rerun the same three comparison scenarios, measure the new standing honestly, update the bundle evidence, and close the reopen only if the rerun supports the claimed improvement.

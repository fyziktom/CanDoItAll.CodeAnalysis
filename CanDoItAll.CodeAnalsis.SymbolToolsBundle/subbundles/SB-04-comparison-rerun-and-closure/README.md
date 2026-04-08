# SB-04 Comparison rerun and closure

## Status

- Completed

## Objective

- Rerun validation on the original comparison scenarios plus additional scenarios so the new symbol tools are judged on a wider sample.

## Covered Inputs

- execute and validate again
- on same and few different scenarios
- eliminate that we are tuning it just on few scenarios

## Exact Source References

- C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle\analysis\04-comparison-results.md
- C:\repositories\CanDoItAll.CodeAnalsis\tools\ComparisonHarness\Program.cs
- C:\repositories\CanDoItAll\CanDoItAll.slnx

## Prerequisites

- SB-01 through SB-03 are complete and trusted.

## Deliverables

- Updated rerun harness outputs
- Scenario analysis notes
- Final execution report and raw-note closure

## Dependency Impact

- Owns the final proof that this work generalizes beyond the original narrow sample.

## Validation Depth

- Build
- Tests
- Browser proof review
- Comparison notes review

## Implementation Steps

1. Freeze the rerun scenario set.
2. Capture symbol-tool outputs on the original three scenarios and additional scenarios.
3. Compare usefulness, noise, call count, time, and output size against SharpTools again where relevant.
4. Update the execution report and close the bundle.

## Do Not Do

- Do not reuse only the original three scenarios.
- Do not call the work complete without storing the widened rerun evidence.

## Acceptance Checklist

- The original scenarios are rerun.
- Additional scenarios are included.
- Findings are written into this bundle.
- Final build, tests, and validator runs are recorded.

## Proof Required

- Comparison artifacts
- Updated execution report
- Completed-stage validator pass

## Browser Validation Logging

- Review the browser proof captured in SB-03 as part of closure

## Progression Gate

- The bundle may close only when wider-scenario rerun evidence is stored and the completed validator passes.

## Suggested Agent Prompt

Prove the new symbol tools on a wider sample. Rerun the original host scenarios, add extra scenarios that stress implementations and references, compare the results against SharpTools where useful, and store the findings before closing the bundle.

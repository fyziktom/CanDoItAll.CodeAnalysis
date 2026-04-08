# SB-01 Focused context scenario runs

## Status

- Completed

## Objective

- Gather focused-context outputs, timings, and artifact metrics for the three frozen scenarios.

## Covered Inputs

- do comparison with sharptools
- Analyze the outputs if they are trully helpful
- how much noise they contains
- how many tokens do it takes
- how many calls
- how much time

## Prerequisites

- SB-00 passed
- The standalone application service can build a host snapshot

## Exact Source References

- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web
- C:\repositories\CanDoItAll\CanDoItAll.slnx
- C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle

## Deliverables

- Focused-context outputs for all three scenarios
- Warm per-scenario call counts and elapsed times
- Estimated token load per scenario
- Normalized markdown and JSON artifacts for each scenario

## Dependency Impact

- Blocks final analysis because the comparison needs real focused-context evidence, not remembered results.

## Validation Depth

- Real in-process application-service execution against the host solution
- Captured metrics and output summaries

## Implementation Steps

1. Build the host snapshot once through the focused-context service.
2. Run the three frozen scenarios through the focused-context query API.
3. Capture output shape, elapsed time, call count, and token estimate.
4. Record the results into the bundle.

## Do Not Do

- Do not rely on memory from earlier runs.
- Do not measure the page wrapper instead of the analytics output.

## Acceptance Checklist

- All three scenarios have focused-context evidence.
- Scenario artifacts are recorded.
- Timing, call count, and token estimate are captured.

## Proof Required

- Focused-context scenario artifacts and summary JSON
- Scenario metric tables written into the bundle

## Browser Validation Logging

- Not applicable in this analysis bundle

## Progression Gate

- SharpTools comparison may continue only when focused-context evidence exists for all three scenarios.

## Closure Notes

- Completed on 2026-04-08.
- Proof is stored in `analysis/focused-context/focused-context-summary.json` and the three `analysis/focused-context/*.md` artifacts.

## Suggested Agent Prompt

Run the frozen scenarios through the focused-context lab, capture the outputs and measurements, and record them cleanly in the bundle.

# SB-02 SharpTools scenario runs

## Status

- Completed

## Objective

- Gather SharpTools outputs, timings, and call counts for the same three scenarios using realistic first-pass symbol discovery and inspection sequences.

## Covered Inputs

- do comparison with sharptools
- Analyze the outputs if they are trully helpful
- how much noise they contains
- how many tokens do it takes
- how many calls
- how much time

## Prerequisites

- SB-00 passed
- The host solution can be loaded by SharpTools

## Exact Source References

- C:\repositories\CanDoItAll\CanDoItAll.slnx
- C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle

## Deliverables

- SharpTools sequences and outputs for all three scenarios
- Warm per-scenario call counts and elapsed times
- Estimated token load per scenario

## Dependency Impact

- Blocks final analysis because the comparison needs real SharpTools evidence, not guessed tool sequences.

## Validation Depth

- Real SharpTools MCP runs
- Captured call-count and payload summaries

## Implementation Steps

1. Load the host solution into SharpTools.
2. Resolve each scenario into a realistic first-pass sequence.
3. Capture output shape, elapsed time, call count, and token estimate.
4. Record the results into the bundle.

## Do Not Do

- Do not assume perfect FQNs if the user query would not have them.
- Do not optimize SharpTools with hindsight that the focused-context side did not have.

## Acceptance Checklist

- All three scenarios have SharpTools evidence.
- The tool sequence per scenario is explicit.
- Timing, call count, and token estimate are captured.

## Proof Required

- Recorded SharpTools tool sequence per scenario
- Scenario metric tables written into the bundle

## Browser Validation Logging

- N/A for SharpTools-only evidence collection

## Progression Gate

- Final analysis may continue only when SharpTools evidence exists for all three scenarios.

## Closure Notes

- Completed on 2026-04-08.
- Proof is stored in the three `analysis/sharptools/*.md` scenario artifacts and summarized in `analysis/04-comparison-results.md`.

## Suggested Agent Prompt

Run realistic first-pass SharpTools sequences for the frozen scenarios, measure them, and record the resulting payloads and metrics in the bundle.

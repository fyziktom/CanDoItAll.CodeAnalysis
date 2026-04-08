# SB-19 Validation and MCP seam review

## Status

- Completed

## Objective

- Run the full reopened validation matrix, compare the new workflow against SharpTools, and confirm the future MCP seam remains thin.

## Covered Inputs

- Test on `C:\repositories\CanDoItAll\CanDoItAll.slnx`
- Analyze whether the information is really helpful
- Compare with SharpTools call count and context cost
- Explain how to make it better if the savings are still weak
- Explain what feedback the new tuning page needs for the next heuristic pass

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

## Dependency Impact

- This phase closes the reopened initiative and determines whether follow-up work is required.

## Validation Depth

- Full build, tests, Mermaid rendering, Playwright proof, host rerun, SharpTools comparison, and bundle validator completion

## Implementation Steps

1. Run the full validation matrix.
2. Rerun host analysis and compare outputs against SharpTools probing.
3. Review what the tuning page reveals about noisy versus useful excerpts and record the next feedback loop explicitly.
4. Review the future MCP seam and record residual risks honestly.

## Do Not Do

- Do not claim success if context savings are still weak without explaining why.
- Do not skip the bundle final-closure updates.

## Acceptance Checklist

- The reopened validation matrix passes.
- The comparison against SharpTools is evidence-based.
- The tuning guidance is explicit enough to shape the next heuristic pass.
- Residual risks and next improvements are explicit.

## Proof Required

- Full build and test commands
- Mermaid CLI render commands
- Playwright artifacts
- SharpTools call log and comparison notes
- Tuning-feedback write-up tied to the lab page output
- Final bundle validator pass

## Browser Validation Logging

- Required for the final focused-context UI flow and updated exports review.

## Progression Gate

- Final closure passes only if the reopened value claim is supported by actual evidence.

## Suggested Agent Prompt

Close the reopened initiative with evidence. Measure context savings honestly and preserve the thin future MCP seam.

# SB-07 Risk rules and insights

## Status

- Completed

## Objective

- Derive architectural findings, cycles, hotspots, and open questions from canonical facts.

## Covered Inputs

- Risk rules and insights requirement

## Prerequisites

- `SB-04` completed
- `SB-05` completed
- `SB-06` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Analysis`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Domain\Insights`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\Components\Pages\Snapshots\Findings.razor`

## Deliverables

- Findings
- Cycles
- Hotspots
- Open questions

## Dependency Impact

- The final usefulness comparison relies on these insights staying derived rather than hand-authored.

## Validation Depth

- Build, unit tests, and snapshot assertions

## Implementation Steps

1. Evaluate rules over canonical facts.
2. Build deterministic insight outputs.
3. Expose findings through the application and UI layers.

## Do Not Do

- Do not let rule code mutate underlying facts.

## Acceptance Checklist

- Findings are reproducible from the same snapshot.
- Open questions remain explicit when collectors are ambiguous.

## Proof Required

- Unit tests for rule evaluation
- Snapshot summary verification

## Browser Validation Logging

- Logged through findings page at baseline only

## Progression Gate

- Insight outputs are trustworthy enough to guide later orientation work.

## Suggested Agent Prompt

Keep rules derived and deterministic. Diagnostics and open questions must stay visible.

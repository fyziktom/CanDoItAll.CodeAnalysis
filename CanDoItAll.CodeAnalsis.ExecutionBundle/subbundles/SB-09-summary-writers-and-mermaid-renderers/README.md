# SB-09 Summary writers and Mermaid renderers

## Status

- Completed

## Objective

- Render snapshot summaries and Mermaid diagrams from canonical facts and exports.

## Covered Inputs

- Markdown and Mermaid export requirements

## Prerequisites

- `SB-04`, `SB-06`, and `SB-08` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Rendering`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Unit\MermaidFacts.cs`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Support\Golden\exports`

## Deliverables

- Markdown summary writer
- Project graph renderer
- Class diagram renderer
- ER diagram renderer

## Dependency Impact

- Reopened scoped-diagram work in `SB-16` builds directly on this phase.

## Validation Depth

- Build, unit tests, and Mermaid syntax proof

## Implementation Steps

1. Render exports from canonical facts.
2. Keep Mermaid identifiers and labels valid.
3. Capture golden outputs for regression tests.

## Do Not Do

- Do not move relationship inference into renderers.

## Acceptance Checklist

- Export bundle is generated from snapshot facts.
- Mermaid outputs are test-covered.

## Proof Required

- Mermaid unit tests
- Rendered export artifacts

## Browser Validation Logging

- Logged through exports page at baseline only

## Progression Gate

- Renderers are stable enough to be improved for scoped usefulness later.

## Suggested Agent Prompt

Keep renderers pure and small. Fix syntax and selection at the renderer boundary only when facts already exist.

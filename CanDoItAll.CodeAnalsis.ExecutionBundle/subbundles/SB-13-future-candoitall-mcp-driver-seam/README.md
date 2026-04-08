# SB-13 Future CanDoItAll MCP driver seam

## Status

- Completed

## Objective

- Keep the standalone engine ready for thin future hosting inside `CanDoItAll.Mcp.CodeAnalytics`.

## Covered Inputs

- Future MCP-driver readiness requirement

## Prerequisites

- `SB-00A` completed
- `SB-10` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\reference`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Architecture`
- `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\overview\11-future-mcp-handoff.md`

## Deliverables

- Thin seam documentation
- Compatibility tests
- Future host settings examples

## Dependency Impact

- Final reopened seam review in `SB-19` builds on this baseline.

## Validation Depth

- Architecture tests and documentation audit

## Implementation Steps

1. Freeze seam expectations.
2. Protect them with tests and examples.
3. Keep transport-agnostic logic inside the standalone projects.

## Do Not Do

- Do not pull in host-specific MCP runtime layers.

## Acceptance Checklist

- Thin seam is documented and protected.
- No copied host MCP core exists in the repo.

## Proof Required

- Architecture tests
- Compatibility documentation

## Browser Validation Logging

- N/A

## Progression Gate

- Future MCP hosting remains a thin wrapper task, not an engine redesign.

## Suggested Agent Prompt

Protect the future seam and keep the standalone engine transport-agnostic.

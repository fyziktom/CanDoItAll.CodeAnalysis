# SB-00A CanDoItAll compatibility baseline

## Status

- Completed

## Objective

- Freeze the host-repo compatibility baseline and future MCP seam expectations.

## Covered Inputs

- Host-repo audit instructions from the original request
- Future `CanDoItAll.Mcp.CodeAnalytics` seam requirement

## Prerequisites

- `SB-00` completed
- Host repo available read-only at `C:\repositories\CanDoItAll`

## Exact Source References

- `C:\repositories\CanDoItAll`
- `C:\repositories\CanDoItAll.CodeAnalsis\reference`
- `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\overview\15-current-candoitall-mcp-landscape.md`

## Deliverables

- Compatibility notes
- Tool surface proposal
- Future settings example

## Dependency Impact

- Later MCP seam work depends on this baseline staying thin.

## Validation Depth

- Documentation, architecture tests, and seam inspection

## Implementation Steps

1. Audit host MCP structure and naming.
2. Capture compatibility references.
3. Add seam-protection tests.

## Do Not Do

- Do not import `CanDoItAll.Mcp.Core` into the standalone repo.

## Acceptance Checklist

- Compatibility references exist.
- Thin-seam constraints are documented and protected.

## Proof Required

- Architecture tests for seam protection
- Compatibility reference artifacts under `reference/`

## Browser Validation Logging

- N/A

## Progression Gate

- Host compatibility assumptions are stable enough for deeper engine work.

## Suggested Agent Prompt

Capture only the compatibility baseline and seam protections. Do not redesign the engine around the future host.

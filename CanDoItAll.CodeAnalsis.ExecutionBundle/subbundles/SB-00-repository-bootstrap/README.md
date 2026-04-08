# SB-00 Repository bootstrap

## Status

- Completed

## Objective

- Establish the standalone repo skeleton, naming guardrails, and build-time repository checks.

## Covered Inputs

- `CanDoItAll.CodeAnalsis` naming map from the original request
- Standalone repo bootstrap requirements from the original bundle

## Prerequisites

- `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx` exists

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx`
- `C:\repositories\CanDoItAll.CodeAnalsis\Directory.Build.props`
- `C:\repositories\CanDoItAll.CodeAnalsis\eng`

## Deliverables

- Canonical solution file
- Root build props and editor configuration
- Repository validation scripts

## Dependency Impact

- Later phases rely on this layout and validation scaffolding.

## Validation Depth

- Build and structure validation only

## Implementation Steps

1. Create the solution and project skeleton.
2. Add repo guardrails and validation scripts.
3. Confirm the root naming map is frozen.

## Do Not Do

- Do not copy code from the host repo.

## Acceptance Checklist

- `CanDoItAll.CodeAnalsis.slnx` is the canonical solution.
- Root guardrails exist and run.

## Proof Required

- `dotnet build .\CanDoItAll.CodeAnalsis.slnx -warnaserror`
- `pwsh .\eng\Validate-SolutionStructure.ps1`

## Browser Validation Logging

- N/A

## Progression Gate

- Repo shape and guardrails are stable enough for all later subbundles.

## Suggested Agent Prompt

Bootstrap only the standalone repo skeleton and guardrails. Do not implement analysis features in this phase.

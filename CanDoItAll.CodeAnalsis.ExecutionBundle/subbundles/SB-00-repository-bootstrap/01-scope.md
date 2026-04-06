# SB-00 — Repository bootstrap and guardrails

## Objective
Create the standalone repository foundations, canonical solution skeleton, root build settings, validation scripts, and repository guardrails so later slices land on stable ground.

## Milestone / priority / actor
- Milestone: `M0`
- Priority: `P0`
- Primary actor: `Platform maintainer`

## Depends on
None

## Read first
- README.md
- START-HERE.md
- overview/01-executive-summary.md
- overview/02-architecture-blueprint.md
- overview/03-solution-structure.md
- overview/08-quality-gates.md
- overview/10-repository-conventions.md
- overview/15-current-candoitall-mcp-landscape.md
- overview/16-compatibility-and-shared-parts.md
- overview/18-execution-order-and-closure-evidence.md

## Current CanDoItAll reference files to inspect
- .github/copilot-instructions.md
- Directory.Build.props
- global.json
- CanDoItAll.slnx
- tests/CanDoItAll.Tests.Support/CanDoItAll.Tests.Support.csproj

## In scope
- Create `CanDoItAll.CodeAnalsis.slnx` as the canonical solution file. A compatibility `.sln` may be added only if tooling truly requires it; `.slnx` remains canonical.
- Create the initial `src/`, `tests/`, `eng/`, `architecture/`, and `codex/` root folders so the repo already resembles future CanDoItAll transplantation targets.
- Add `Directory.Build.props`, `.editorconfig`, `.gitignore`, and `global.json` aligned to .NET 10 and nullable-enabled builds.
- Add validation scripts such as `eng/Validate-FileLengths.ps1` and `eng/Validate-SolutionStructure.ps1` with explicit failure messages.
- Create the empty production and test projects defined in the solution blueprint and wire only allowed project references.

## Out of scope
- No Roslyn collector logic yet.
- No MCP server project yet.
- No heavy UI implementation beyond an empty shell if needed for the solution graph.

## Compatibility rules specific to this subbundle
- Mirror the current CanDoItAll repo layout style: `src`, `tests`, root `global.json`, and solution-first organization.
- Prefer `net10.0` and SDK pinning compatible with the current CanDoItAll repo.
- Do not introduce a local clone of `CanDoItAll.Mcp.Core`; the standalone repo should remain host-agnostic.

## Expected deliverables
- `CanDoItAll.CodeAnalsis.slnx` plus the baseline project graph.
- Root configuration files and validation scripts.
- Initial architecture ADR folder in `architecture/adrs/` with README placeholder.
- A short `codex/README.md` stub explaining that the bundle drives execution for now.

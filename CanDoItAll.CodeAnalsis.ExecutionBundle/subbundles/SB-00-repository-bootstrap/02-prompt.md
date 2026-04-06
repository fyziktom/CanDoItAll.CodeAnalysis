# Prompt for SB-00 — Repository bootstrap and guardrails

You are implementing **SB-00** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Create the standalone repository foundations, canonical solution skeleton, root build settings, validation scripts, and repository guardrails so later slices land on stable ground.

## Read before coding
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
- subbundles/SB-00-repository-bootstrap/01-scope.md
- subbundles/SB-00-repository-bootstrap/03-checklist.md
- subbundles/SB-00-repository-bootstrap/04-validation.md
- subbundles/SB-00-repository-bootstrap/05-forbidden-patterns.md
- subbundles/SB-00-repository-bootstrap/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- .github/copilot-instructions.md
- Directory.Build.props
- global.json
- CanDoItAll.slnx
- tests/CanDoItAll.Tests.Support/CanDoItAll.Tests.Support.csproj

## Required implementation steps
- Create all production and test projects named in the solution structure doc using the `CanDoItAll.CodeAnalytics.*` namespace family.
- Pin the SDK to 10.0.200 with latestPatch roll-forward unless a concrete blocker appears.
- Wire architecture validation so illegal project references can be detected automatically.
- Set file-length policy guidance and make it executable through a script, not just prose.
- Create a minimal README and architecture ADR index inside the new repo.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not proceed if the solution graph still violates the intended layer boundaries.
- Do not proceed if the repo depends on a local copy of host-repo-only MCP infrastructure.
- Do not proceed if file-length or structure validation is only manual.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.

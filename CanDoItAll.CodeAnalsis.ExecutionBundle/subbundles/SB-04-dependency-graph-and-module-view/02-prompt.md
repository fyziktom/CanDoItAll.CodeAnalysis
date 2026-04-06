# Prompt for SB-04 — Dependency graph and module view

You are implementing **SB-04** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Derive project, assembly, namespace, and module dependency views with cycles, fan-in/fan-out, and useful module-level grouping.

## Read before coding
- overview/02-architecture-blueprint.md
- overview/05-analysis-pipeline.md
- overview/17-naming-settings-and-tool-surface-map.md
- subbundles/SB-04-dependency-graph-and-module-view/01-scope.md
- subbundles/SB-04-dependency-graph-and-module-view/03-checklist.md
- subbundles/SB-04-dependency-graph-and-module-view/04-validation.md
- subbundles/SB-04-dependency-graph-and-module-view/05-forbidden-patterns.md
- subbundles/SB-04-dependency-graph-and-module-view/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- src/CanDoItAll.Composition/CanDoItAll.Composition.csproj
- src/CanDoItAll.Web/CanDoItAll.Web.csproj

## Required implementation steps
- Map references and symbol usage edges into normalized graph records.
- Design a module classifier that can start with namespace/assembly conventions and remain extensible.
- Compute cycles and graph metrics deterministically.
- Add fixture cases with at least one intentional cycle or layer violation.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if module grouping is non-deterministic or hidden inside UI code.
- Do not continue if graph algorithms only work for tiny toy solutions.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.

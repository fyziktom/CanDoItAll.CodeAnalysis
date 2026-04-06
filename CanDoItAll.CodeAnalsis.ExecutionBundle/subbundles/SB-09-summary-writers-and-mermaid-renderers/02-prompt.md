# Prompt for SB-09 — Summary writers and Mermaid renderers

You are implementing **SB-09** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Produce human-readable summaries and Mermaid outputs from the canonical snapshot without making Mermaid the source of truth.

## Read before coding
- overview/02-architecture-blueprint.md
- overview/06-ui-blueprint.md
- overview/17-naming-settings-and-tool-surface-map.md
- subbundles/SB-09-summary-writers-and-mermaid-renderers/01-scope.md
- subbundles/SB-09-summary-writers-and-mermaid-renderers/03-checklist.md
- subbundles/SB-09-summary-writers-and-mermaid-renderers/04-validation.md
- subbundles/SB-09-summary-writers-and-mermaid-renderers/05-forbidden-patterns.md
- subbundles/SB-09-summary-writers-and-mermaid-renderers/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- codex/architecture-review/example-prompts.md

## Required implementation steps
- Design renderer inputs around canonical snapshot records, not collector internals.
- Add diagram-size guards and explanatory diagnostics for skipped or truncated outputs.
- Support at least class, ER, and project/module dependency diagrams.
- Add golden tests for summary and Mermaid outputs.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if diagram text generation becomes the canonical data source.
- Do not continue if summaries omit provenance about uncertainty or truncation.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.

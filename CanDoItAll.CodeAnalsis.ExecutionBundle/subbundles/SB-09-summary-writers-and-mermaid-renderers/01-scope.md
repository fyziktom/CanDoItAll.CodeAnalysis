# SB-09 — Summary writers and Mermaid renderers

## Objective
Produce human-readable summaries and Mermaid outputs from the canonical snapshot without making Mermaid the source of truth.

## Milestone / priority / actor
- Milestone: `M3`
- Priority: `P0`
- Primary actor: `Rendering maintainer`

## Depends on
- SB-04
- SB-06
- SB-07
- SB-08

## Read first
- overview/02-architecture-blueprint.md
- overview/06-ui-blueprint.md
- overview/17-naming-settings-and-tool-surface-map.md

## Current CanDoItAll reference files to inspect
- codex/architecture-review/example-prompts.md

## In scope
- Write markdown/text summary exporters for executive overview, dependency overview, persistence overview, and findings overview.
- Render Mermaid class, ER, and dependency/module diagrams from canonical records.
- Package export metadata so UI and future MCP tools can enumerate available outputs.
- Guard against oversized or unreadable diagrams with diagnostics and truncation strategies.

## Out of scope
- No client-side graph rendering dependency.
- No attempt to encode every possible analysis detail into Mermaid.

## Compatibility rules specific to this subbundle
- Output names and metadata should map cleanly to future MCP tools such as `code_analytics_diagram_get` and `code_analytics_export_get`.
- Keep renderers independent so a future host-repo UI or MCP driver can request only the outputs it needs.

## Expected deliverables
- Summary writers.
- Mermaid renderers and examples.
- Export catalog model with type, path, and summary metadata.

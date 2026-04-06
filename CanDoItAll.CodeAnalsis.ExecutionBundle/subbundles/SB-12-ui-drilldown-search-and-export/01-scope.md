# SB-12 — UI drilldown search and export

## Objective
Add focused drilldown pages for dependencies, services, entities, findings, and exports plus lightweight search/filter support.

## Milestone / priority / actor
- Milestone: `M4`
- Priority: `P1`
- Primary actor: `UI maintainer`

## Depends on
- SB-11

## Read first
- overview/06-ui-blueprint.md
- overview/17-naming-settings-and-tool-surface-map.md

## Current CanDoItAll reference files to inspect
- codex/skills/candoitall-watch-playwright-loop/SKILL.md

## In scope
- Add drilldown pages or tabs for graph summaries, DI, persistence, findings, and export lists.
- Support lightweight search/filter/query behavior backed by the application query API.
- Allow export download or reveal of generated markdown/Mermaid assets.

## Out of scope
- No bespoke client-side graph editing surface.
- No giant “single page does everything” implementation.

## Compatibility rules specific to this subbundle
- The UI drilldowns should correspond to future MCP queries and export-selection tools.
- Keep search/filter semantics close to the application query contracts so transport layers stay aligned.

## Expected deliverables
- Dependency/service/entity/finding drilldown surfaces.
- Export list/download surface.
- Web tests for drilldown navigation and filtering.

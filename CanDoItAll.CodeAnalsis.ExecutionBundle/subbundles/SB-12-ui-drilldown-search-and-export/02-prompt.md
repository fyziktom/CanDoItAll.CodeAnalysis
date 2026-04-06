# Prompt for SB-12 — UI drilldown search and export

You are implementing **SB-12** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Add focused drilldown pages for dependencies, services, entities, findings, and exports plus lightweight search/filter support.

## Read before coding
- overview/06-ui-blueprint.md
- overview/17-naming-settings-and-tool-surface-map.md
- subbundles/SB-12-ui-drilldown-search-and-export/01-scope.md
- subbundles/SB-12-ui-drilldown-search-and-export/03-checklist.md
- subbundles/SB-12-ui-drilldown-search-and-export/04-validation.md
- subbundles/SB-12-ui-drilldown-search-and-export/05-forbidden-patterns.md
- subbundles/SB-12-ui-drilldown-search-and-export/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- codex/skills/candoitall-watch-playwright-loop/SKILL.md

## Required implementation steps
- Expose focused pages/components instead of one oversized dashboard.
- Use server-side filtering or simple query parameters over opaque client-only state.
- Render diagnostics and unsupported/truncated export notes clearly.
- Add test coverage for search/filter and export selection.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if drilldowns require re-running raw collectors in the UI layer.
- Do not continue if export selection is hardcoded or hidden.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.

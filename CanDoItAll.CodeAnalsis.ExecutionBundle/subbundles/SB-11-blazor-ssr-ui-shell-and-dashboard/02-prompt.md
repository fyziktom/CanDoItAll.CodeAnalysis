# Prompt for SB-11 — Blazor SSR UI shell and dashboard

You are implementing **SB-11** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Build a simple SSR-first Blazor Web UI that can run analyses, show recent snapshots, and present the most important architecture summary information.

## Read before coding
- overview/06-ui-blueprint.md
- overview/10-repository-conventions.md
- overview/16-compatibility-and-shared-parts.md
- subbundles/SB-11-blazor-ssr-ui-shell-and-dashboard/01-scope.md
- subbundles/SB-11-blazor-ssr-ui-shell-and-dashboard/03-checklist.md
- subbundles/SB-11-blazor-ssr-ui-shell-and-dashboard/04-validation.md
- subbundles/SB-11-blazor-ssr-ui-shell-and-dashboard/05-forbidden-patterns.md
- subbundles/SB-11-blazor-ssr-ui-shell-and-dashboard/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- .github/copilot-instructions.md
- codex/skills/candoitall-components-mcp/SKILL.md
- codex/skills/candoitall-watch-playwright-loop/SKILL.md

## Required implementation steps
- Create the Blazor Web App project and wire application services cleanly.
- Build a minimal run-analysis form and recent-snapshot list.
- Render summary cards and diagnostics in a server-rendered-friendly way.
- Keep UI components small and feature-oriented.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if the UI starts driving the domain model design.
- Do not continue if the first UI requires heavy JavaScript for basic usefulness.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.

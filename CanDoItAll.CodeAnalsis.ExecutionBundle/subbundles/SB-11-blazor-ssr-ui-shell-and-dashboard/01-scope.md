# SB-11 — Blazor SSR UI shell and dashboard

## Objective
Build a simple SSR-first Blazor Web UI that can run analyses, show recent snapshots, and present the most important architecture summary information.

## Milestone / priority / actor
- Milestone: `M4`
- Priority: `P1`
- Primary actor: `UI maintainer`

## Depends on
- SB-10

## Read first
- overview/06-ui-blueprint.md
- overview/10-repository-conventions.md
- overview/16-compatibility-and-shared-parts.md

## Current CanDoItAll reference files to inspect
- .github/copilot-instructions.md
- codex/skills/candoitall-components-mcp/SKILL.md
- codex/skills/candoitall-watch-playwright-loop/SKILL.md

## In scope
- Create the basic Blazor Web App shell with server-side rendering as the default.
- Implement a home/run page and a dashboard page backed by application services.
- Use plain Razor components and lightweight styling; keep the UI inspectable and portable.
- Show latest diagnostics, key counts, recent snapshots, and primary action flows.

## Out of scope
- No heavy client-side graph library.
- No polished product-grade design system.
- No Radzen dependency.

## Compatibility rules specific to this subbundle
- The UI should be easy to transplant later into CanDoItAll or to sit beside a future host-repo MCP driver.
- Follow the current CanDoItAll style direction: explicit state changes, small components, Tailwind-friendly markup, no Radzen by default.

## Expected deliverables
- SSR app shell and core routes.
- Dashboard cards/tables for recent snapshots and top findings.
- Web tests for main SSR flows.

# UI blueprint

## Intent

The first UI is an inspection and verification surface.
It is there to make architecture context legible for humans and future agents, not to compete with dedicated modeling tools.

## UI rules

- SSR first.
- Plain Razor components and simple styling.
- No Radzen by default.
- Tailwind-friendly markup is acceptable.
- Keep pages and components small and feature-oriented.
- No giant client-side graph dependency for v1.

## Primary routes

### `/`
Home / run-analysis page:
- solution path
- optional scope filters
- toggles for expensive collectors
- run action
- recent snapshots

### `/snapshots/{id}`
Dashboard:
- counts
- top findings
- diagnostics summary
- export list
- links to drilldowns

### `/snapshots/{id}/dependencies`
Project/module overview:
- dependency summary cards
- cycle list
- graph facts
- Mermaid preview or export link

### `/snapshots/{id}/services`
DI/service overview.

### `/snapshots/{id}/persistence`
Persistence / ER overview.

### `/snapshots/{id}/findings`
Findings, hot spots, open questions.

### `/snapshots/{id}/exports`
Export catalog and download/reveal links.

## UX notes

- Prefer progressive disclosure over cramming everything into one page.
- Make uncertainty visible.
- Show direct links back to source references where practical.
- Show diagnostics near the affected section, not only in one global error block.

# Target solution

## Architectural direction

- `Workspace` stays the source of truth for loaded projects, compilations, and normalized paths.
- `Facts` owns canonical symbol, dependency, service, persistence, and new member-context graph collection.
- `Domain` owns the portable snapshot contracts and relationship facts.
- `Analysis` continues to own insights and rules, not graph extraction.
- `Application` owns orchestration and focused query composition, not raw Roslyn traversal.
- `Rendering` consumes canonical facts and focused query results but does not infer new truth.
- `Web` stays a thin SSR shell over application queries and operation state.

## Canonical source-of-truth map

- Project and compilation inventory:
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Workspace`
- Canonical type, member, and documentation facts:
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Symbols`
- Canonical dependency and type relationship facts:
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Dependencies`
- Canonical persistence, entity, and EF relationship facts:
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence`
- Canonical member context graph facts:
  - new ownership under `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts`
- Snapshot contract:
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Domain`
- Focused query orchestration:
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application`
- Context exploration UI:
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web`

## Planned refactor boundaries

- Split persistence collector orchestration from relationship extraction and entity resolution helpers.
- Split dependency collector graph assembly from member and type relationship extraction helpers.
- Split application build and query orchestration so focused context has a dedicated service path.
- Keep Mermaid renderers small and selection-driven.
- Move large CSS blocks behind clearer component and page ownership if the CSS still grows during UI work.

## Focused context design

- Start points:
  - type id or type display name
  - member id or member display name
  - optional project filter
- Expansion rules:
  - bounded recursion depth
  - bounded node count
  - explicit relationship kinds
  - deterministic ordering
- Output shape:
  - root symbol
  - related members
  - related types
  - relationship edges
  - exact source references
  - optional XML summaries when available
  - suggested “high reuse” helpers for temporary memory reuse

## Diagram strategy

- Keep the existing global exports for repository-wide overview.
- Add project-scoped and neighborhood-scoped diagram generation.
- Prefer connected-neighborhood selection over alphabetic truncation.
- Use the focused context graph as the primary class and member orientation artifact for troubleshooting.

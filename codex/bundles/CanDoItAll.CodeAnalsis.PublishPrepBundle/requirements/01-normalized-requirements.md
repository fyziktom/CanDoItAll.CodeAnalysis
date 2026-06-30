# Normalized Requirements

## Release Preparation Requirements

| ID | Requirement | Acceptance criteria | Source inputs |
| --- | --- | --- | --- |
| `REQ-001` | Create an implementation-ready bundle for open-source publishing preparation, without implementing code during preparation. | Bundle validates at prepared stage; all subbundles are actionable; execution status remains not started. | `IN-001`, `IN-002` |
| `REQ-002` | Establish a reliable validation baseline before refactoring. | Build passes; full-suite test hang is diagnosed or isolated; file-length and solution-structure scripts run in documented shells; release guardrails are documented. | `IN-002`, `IN-003` |
| `REQ-003` | Identify large files and mixed-responsibility implementation hotspots. | Hotspot inventory lists exact files, line counts, responsibility issues, owning subbundle, and proof required before closure. | `IN-003` |
| `REQ-004` | Decide maintainable project and component boundaries before moving code. | Architecture target lists extraction candidates, dependencies, allowed project references, non-shipping tools, and future driver/addon boundaries. | `IN-005` |
| `REQ-005` | Refactor application orchestration and focused-context responsibilities into smaller, testable services or helpers. | `CodeAnalyticsApplicationService` no longer mixes orchestration, query shaping, scoring, traversal, and file I/O in oversized partial files; behavior is proven by semantic tests. | `IN-003`, `IN-007` |
| `REQ-006` | Harden facts collectors and EF/persistence analyzer behavior. | Persistence analyzer responsibilities are isolated or clearly bounded; EF analyzer coverage includes DbContext, entities, relationships, model snapshots, configuration discovery, and planned EF query anti-pattern rules if in scope. | `IN-005`, `IN-008` |
| `REQ-007` | Address performance risks using evidence, not broad rewrites. | Static findings are triaged; hot paths have scenario/benchmark proof; only measured bottlenecks are optimized. | `IN-007` |
| `REQ-008` | Decompose the desktop sandbox UI into maintainable components while preserving large-screen behavior. | Large Razor pages are split; desktop viewport browser proof confirms readability, spacing, route behavior, workspace picker behavior, and no clipping. | `IN-003`, `IN-006` |
| `REQ-009` | Prepare open-source package metadata, publish policy, and repository hygiene. | Production project packability, metadata, license, security, contributing, package validation, release commands, and non-shipping project exclusions are explicit. | `IN-001`, `IN-002`, `IN-005` |
| `REQ-010` | Improve documentation based on shipped changes. | README, ADRs, public API guide, driver/addon guide, desktop sandbox guide, validation docs, and release docs match the final implementation and cite proof. | `IN-009` |
| `REQ-011` | Provide detailed `.xlsx` checklists and plan. | Final workbook exists under the bundle/output area and includes subbundle plan, checklists, hotspots, extraction candidates, performance/EF findings, and docs plan. | `IN-004` |

## Scope Exceptions And Clarifications

- Small and medium responsive UI tuning is explicitly out of scope except for avoiding obvious breakage introduced by large-screen UI refactors.
- Runtime EF query tuning is not currently a production-app concern because production `src/` does not execute EF Core queries; EF work targets the analyzer and fixture coverage.
- The future `CanDoItAll.Mcp.CodeAnalytics` driver is planned as thin host glue; this bundle should not copy host runtime contracts into the engine libraries unless a later subbundle explicitly changes the architecture.
- Documentation should not claim final project/package names, commands, or APIs before those subbundles ship.

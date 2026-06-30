# ADR 0001: Publishing Boundaries For CodeAnalytics

## Status

Accepted

## Context

The repository is being prepared for open-source publication as a reusable code-analysis engine plus a desktop-large sandbox UI. The current solution already separates contracts, domain facts, workspace loading, facts collection, analysis, rendering, storage, application orchestration, and Web UI. Several large files show that responsibilities need smaller implementation units, but creating new projects before those seams are proven would add package and reference complexity without reducing the immediate maintenance risk.

## Decision

Keep the current source project graph for this publishing wave and split large responsibilities into internal services/components before creating additional packages.

| Candidate | Decision | Rationale |
| --- | --- | --- |
| `CanDoItAll.CodeAnalytics.Engine` | Keep `CanDoItAll.CodeAnalytics.Application` as the engine facade. | The public service contract already lives in `Abstractions`; renaming or adding a facade project would churn package identity before publish metadata is settled. |
| `CanDoItAll.CodeAnalytics.FocusedContext` | Extract into internal Application services now; defer a separate project. | Focused-context code is policy-heavy and oversized, but it still depends on application query contracts and snapshot response shapes. Internal services reduce file size without creating premature package boundaries. |
| `CanDoItAll.CodeAnalytics.SymbolQueries` | Extract into internal Application services now; defer a separate project. | Symbol query behavior is part of the high-level application API and should stay co-located until reuse pressure appears outside the engine facade. |
| `CanDoItAll.CodeAnalytics.Facts.EfCore` | Keep under `Facts` for this wave; design as the first future optional fact addon. | The EF analyzer is syntax/Roslyn-based and has no production runtime EF dependency. A future addon package is appropriate only when fact-pack registration becomes configurable. |
| `CanDoItAll.CodeAnalytics.Facts.DependencyInjection` | Keep under `Facts`. | DI facts share collector infrastructure and do not currently introduce optional dependencies. |
| `CanDoItAll.CodeAnalytics.Storage.FileSystem` | Keep `Storage` as the file-system driver project. | There is one storage driver today. Splitting before a second driver exists would only move files. |
| `CanDoItAll.CodeAnalytics.Rendering.Mermaid` | Keep under `Rendering`. | Markdown and Mermaid rendering are small and stable enough to remain a single renderer helper project for now. |
| `CanDoItAll.CodeAnalytics.Web.DesktopSandbox` | Keep as non-core Web project. | The sandbox is a large-screen desktop UI and must remain outside reusable engine packages. |
| `CanDoItAll.Mcp.CodeAnalytics` | Future host-driver project only. | This repo must stay transport-agnostic and must not copy `CanDoItAll.Mcp.Core` host contracts. |

## Allowed References

- `Domain` has no project references.
- `Abstractions` may reference `Domain`.
- `Workspace` may reference `Domain`.
- `Facts` may reference `Domain` and `Workspace`.
- `Analysis` may reference `Domain` and `Facts`.
- `Rendering` may reference `Domain`.
- `Storage` may reference `Domain`.
- `Application` may reference `Abstractions`, `Analysis`, `Domain`, `Facts`, `Rendering`, `Storage`, and `Workspace`.
- `Web` may reference `Application` and is the only source project that may use `Microsoft.NET.Sdk.Web`.
- Tools and future host drivers may depend on the engine facade; reusable source projects may not depend on tools, tests, Web, or MCP host runtime contracts.

## Migration Order

1. Split Application focused-context and symbol-query responsibilities into internal services while preserving `ICodeAnalyticsApplicationService`.
2. Harden Facts persistence/EF and DI collectors behind smaller internal collector helpers.
3. Reduce storage/rendering/export hot paths within the existing projects.
4. Decompose the desktop sandbox into focused Razor components without making Web a reusable package.
5. Revisit optional `Facts.EfCore`, `FocusedContext`, or renderer packages only after a second consumer or configurable fact-pack registration exists.

## Rollback Plan

Because no new project is created by this ADR, rollback is limited to reverting the internal service/component splits introduced by later subbundles. The current `.slnx`, package identities, and project references remain stable during the publishing hardening work.

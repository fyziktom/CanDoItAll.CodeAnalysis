# Executive summary

## Mission

Build a standalone repository and solution named **`CanDoItAll.CodeAnalsis`** that implements reusable code-analysis libraries plus a simple SSR-first Blazor UI.
The implementation happens outside the current CanDoItAll repo, but it must already be shaped for a future thin MCP driver named **`CanDoItAll.Mcp.CodeAnalytics`** inside the main host repo.

## What changed in this revised bundle

This revision bakes in the current CanDoItAll MCP ecosystem instead of treating future integration abstractly.
The bundle now explicitly aligns with:

- the current host repo using **`CanDoItAll.slnx`** and **`.NET 10.0.200`** / **`net10.0`**,
- current MCP server bootstrapping patterns (`Program` + `Configuration` + `Tools` + coordinator/runtime services),
- current shared-core types in **`CanDoItAll.Mcp.Core`**,
- current `.vscode/mcp.json`, settings-file, install-script, and Codex asset patterns.

## Primary outcome

At the end of the first implementation wave, the standalone repo must be able to:

1. load a C# solution via Roslyn/MSBuild,
2. extract architectural facts without manual file-by-file review,
3. assemble a deterministic canonical snapshot,
4. derive higher-level dependency, DI, persistence, and risk views,
5. export JSON, Markdown, and Mermaid assets,
6. present the results in a lightweight SSR-first Blazor UI,
7. prove that a future **`CanDoItAll.Mcp.CodeAnalytics`** host-repo driver can be added with thin glue only.

## Non-negotiables

- Roslyn-first static analysis is the primary source of truth.
- Facts and insights stay separate.
- The standalone repo must **not** clone `CanDoItAll.Mcp.Core`.
- The repo root keeps the requested typo (`CodeAnalsis`), but the **namespace family remains `CanDoItAll.CodeAnalytics.*`**.
- Comments in source code must be in English; keep comments rare and avoid XML-doc sprawl unless it is clearly justified for public contracts or fixtures.
- Codex must finish with a mandatory refactor/review pass that splits oversized files and removes dumping-ground folders.

## Fast reading order

1. `START-HERE.md`
2. `overview/01-executive-summary.md`
3. `overview/03-solution-structure.md`
4. `overview/15-current-candoitall-mcp-landscape.md`
5. `overview/16-compatibility-and-shared-parts.md`
6. `overview/17-naming-settings-and-tool-surface-map.md`
7. `prompts/04-codex-master-start-prompt.md`
8. the spreadsheet backlog
9. the numbered subbundles in dependency order

# Master start prompt for Codex

You are building a new standalone repository named **`CanDoItAll.CodeAnalsis`**.

The repo is intentionally being developed outside the current CanDoItAll repo first, but you also have the **current CanDoItAll repository** available in the workspace.
You must use that host repo as a compatibility reference so the standalone codebase is ready for future transplantation as a new MCP server named **`CanDoItAll.Mcp.CodeAnalytics`**.

## Mission

Implement the standalone repo so it provides:

- Roslyn-first solution loading,
- canonical architecture snapshot generation,
- fact collectors for symbols / docs / dependencies / DI / EF Core,
- rule-derived architectural findings,
- Markdown and Mermaid exports,
- deterministic file-based snapshot storage,
- a simple SSR-first Blazor UI,
- explicit future MCP-driver readiness.

## Non-negotiable naming map

- repo root: `CanDoItAll.CodeAnalsis`
- canonical solution file: `CanDoItAll.CodeAnalsis.slnx`
- project/namespace/assembly family: `CanDoItAll.CodeAnalytics.*`
- future host-repo driver: `CanDoItAll.Mcp.CodeAnalytics`

Do not blur these names together.

## Required bundle reading order

1. `README.md`
2. `START-HERE.md`
3. `overview/01-executive-summary.md`
4. `overview/03-solution-structure.md`
5. `overview/15-current-candoitall-mcp-landscape.md`
6. `overview/16-compatibility-and-shared-parts.md`
7. `overview/17-naming-settings-and-tool-surface-map.md`
8. `overview/18-execution-order-and-closure-evidence.md`
9. `overview/19-host-repo-shared-surface-catalog.md`
10. the backlog workbook
11. the current subbundle docs

## Required host-repo audit before coding

Inspect these files from the current CanDoItAll repo:

- `.github/copilot-instructions.md`
- `.vscode/mcp.json`
- `global.json`
- `Directory.Build.props`
- `src/CanDoItAll.Mcp.Core/Contracts/McpToolEnvelope.cs`
- `src/CanDoItAll.Mcp.Core/Identity/IdentifierFactories.cs`
- `src/CanDoItAll.Mcp.Core/Operations/OperationPrimitives.cs`
- `src/CanDoItAll.Mcp.Components/Program.cs`
- `src/CanDoItAll.Mcp.Components/Tools/ComponentsTools.cs`
- `src/CanDoItAll.Mcp.ProjectStructure/Program.cs`
- `src/CanDoItAll.Mcp.ProjectStructure/ProjectStructureTools.cs`
- `src/CanDoItAll.Mcp.DotNetWatch/Program.cs`
- `tools/Reinstall-CanDoItAllMcps.ps1`
- `codex/README.md`
- `.codex/agents/*.toml`

From that audit, extract and keep in mind:

- the host repo uses `.slnx`,
- the host repo uses `.NET 10`,
- current MCP servers follow a recognizable `Program` + `Configuration` + `Tools` + service/coordinator pattern,
- current MCP servers rely on `CanDoItAll.Mcp.Core`,
- current MCP settings files and `.vscode/mcp.json` entries follow a consistent naming style,
- future transplantation should therefore need thin glue, not engine redesign.

## Critical architecture rules

1. Roslyn-first analysis is the baseline.
2. Facts, insights, and diagnostics stay separate.
3. The standalone repo must **not** copy `CanDoItAll.Mcp.Core`.
4. The standalone application layer must stay transport-agnostic.
5. Mermaid is a renderer, not the source of truth.
6. The UI must stay SSR-first and lightweight.
7. Comments must be in English and rare.
8. Avoid XML docs unless a public-contract reason strongly justifies them.
9. Avoid generic folders such as `Helpers` or `Misc`.
10. Oversized files must be split in the final pass.

## Required execution order

Implement in the bundle’s subbundle order:

- SB-00 Repository bootstrap and guardrails
- SB-00A Current CanDoItAll compatibility baseline
- SB-01 Canonical domain model and contracts
- SB-02 Workspace loading and solution inventory
- SB-03 Symbol indexing and XML documentation ingestion
- SB-04 Dependency graph and module view
- SB-05 DI and service registration analysis
- SB-06 EF Core and persistence view
- SB-07 Risk rules and insights
- SB-08 Snapshot assembly, serialization, and caching
- SB-09 Summary writers and Mermaid renderers
- SB-10 Application orchestrator and query API
- SB-11 Blazor SSR UI shell and dashboard
- SB-12 UI drilldown search and export
- SB-13 Future CanDoItAll MCP driver seam and compatibility proof
- SB-14 Tests hardening, repo-local Codex assets, and final refactor

## Your first response must include

- the naming map confirmation,
- the current subbundle you will implement first,
- the specific files/folders you expect to create or edit,
- the validation commands you plan to run for that slice,
- any blockers or ambiguities discovered from the host-repo audit.

## Implementation style

- Prefer small, verifiable slices.
- Keep domain/application contracts plain.
- Use explicit result types and diagnostics.
- Preserve deterministic ordering everywhere.
- If you need placeholders, make them real enough to compile and test.
- If you move fast and create long files, you must clean them up before closure.

## Mandatory ending

Before claiming completion, you must:

1. run the refactor pass prompt,
2. run the review pass prompt,
3. run the full validation matrix,
4. verify the future `CanDoItAll.Mcp.CodeAnalytics` seam is still thin,
5. report any remaining non-blocking risks honestly.

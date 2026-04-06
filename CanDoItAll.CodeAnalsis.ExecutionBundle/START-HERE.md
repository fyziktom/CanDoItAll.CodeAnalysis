# Start here

## What this bundle is for

Use this bundle to implement the standalone **`CanDoItAll.CodeAnalsis`** repository:
- reusable code-analysis libraries,
- deterministic architecture snapshot generation,
- Markdown/Mermaid export generation,
- a simple SSR-first Blazor UI,
- explicit readiness for future integration as **`CanDoItAll.Mcp.CodeAnalytics`** inside the main CanDoItAll repo.

## Required working context

Codex should have access to:
1. the new standalone repo workspace,
2. this bundle,
3. the current CanDoItAll repo snapshot used as the compatibility reference.

## Required reading order

1. `README.md`
2. `overview/01-executive-summary.md`
3. `overview/03-solution-structure.md`
4. `overview/15-current-candoitall-mcp-landscape.md`
5. `overview/16-compatibility-and-shared-parts.md`
6. `overview/17-naming-settings-and-tool-surface-map.md`
7. `overview/19-host-repo-shared-surface-catalog.md`
8. `prompts/04-codex-master-start-prompt.md`
9. the backlog workbook
10. the numbered subbundles

## First practical move

Before writing implementation code, Codex should inspect these host-repo files:
- `.github/copilot-instructions.md`
- `.vscode/mcp.json`
- `global.json`
- `Directory.Build.props`
- `src/CanDoItAll.Mcp.Core/**/*`
- `src/CanDoItAll.Mcp.Components/**/*`
- `src/CanDoItAll.Mcp.ProjectStructure/**/*`
- `src/CanDoItAll.Mcp.DotNetWatch/**/*`
- `tools/Reinstall-CanDoItAllMcps.ps1`
- `codex/README.md`
- `.codex/agents/*.toml`

## Execution rule

Do not try to solve the whole repo in one pass.
Follow the subbundles in order and keep the future host-repo MCP seam visible from the start.

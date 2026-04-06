# CanDoItAll.CodeAnalsis — execution-grade Codex bundle

This bundle is a revised implementation bundle for building the standalone **`CanDoItAll.CodeAnalsis`** repository.

The bundle is intentionally optimized for **future transplantation into the current CanDoItAll MCP ecosystem**.
Compared with the previous version, this revision now bakes in:

- the current CanDoItAll MCP server patterns,
- the current host repo naming/settings/install conventions,
- the current `CanDoItAll.Mcp.Core` shared surface,
- a detailed master start prompt,
- explicit compatibility subbundles and reference artifacts,
- an expanded backlog workbook with compatibility and naming sheets.

## Intended use

1. Give Codex the repository workspace for the new standalone repo.
2. Also make the current CanDoItAll repo available in the workspace.
3. Start with `START-HERE.md`.
4. Then use `prompts/04-codex-master-start-prompt.md`.
5. Execute the numbered subbundles in dependency order.
6. Finish with the refactor and review prompts.

## Important naming rule

The user explicitly wants:
- repo/solution identity: **`CanDoItAll.CodeAnalsis`**
- project/namespace family: **`CanDoItAll.CodeAnalytics.*`**
- future driver example: **`CanDoItAll.Mcp.CodeAnalytics`**

Do not blur those together.

## Most important documents

- `START-HERE.md`
- `overview/03-solution-structure.md`
- `overview/15-current-candoitall-mcp-landscape.md`
- `overview/16-compatibility-and-shared-parts.md`
- `overview/17-naming-settings-and-tool-surface-map.md`
- `overview/19-host-repo-shared-surface-catalog.md`
- `prompts/04-codex-master-start-prompt.md`
- `spreadsheets/CanDoItAll.CodeAnalsis.ExecutionBacklog.xlsx`

## Notes for implementation

- Prompts are intentionally written in English so they can be pasted directly into Codex.
- Code comments must be in English.
- Keep comments rare.
- Avoid XML docs unless there is a clear public-contract reason.
- Codex may work fast in rough slices, but the final pass must refactor long files and restore clean folder structure.

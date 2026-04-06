# Naming, settings, and tool surface map

## Exact naming map

| Concern | Exact name | Notes |
|---|---|---|
| Repo root | `CanDoItAll.CodeAnalsis` | Intentional typo preserved by user request |
| Canonical solution file | `CanDoItAll.CodeAnalsis.slnx` | Mirrors current host-repo `.slnx` usage |
| Project/namespace family | `CanDoItAll.CodeAnalytics.*` | Correct spelling for reusable code |
| Future host-repo driver | `CanDoItAll.Mcp.CodeAnalytics` | Thin MCP wrapper added later |
| Future settings file | `CanDoItAll.Mcp.CodeAnalytics.settings.example.json` | Host-repo root file |
| Future local settings file | `CanDoItAll.Mcp.CodeAnalytics.settings.local.json` | If secrets/local overrides are needed |
| Future tool prefix | `code_analytics_` | Matches current host-repo lowercase snake_case style |

## Proposed future MCP tools

- `code_analytics_snapshot_build`
- `code_analytics_summary_get`
- `code_analytics_diagram_get`
- `code_analytics_query`
- `code_analytics_export_get`
- `code_analytics_recent_list`

## Application-operation mapping

| Future MCP tool | Standalone application operation |
|---|---|
| `code_analytics_snapshot_build` | Build snapshot / analyze solution |
| `code_analytics_summary_get` | Get summary for an existing snapshot |
| `code_analytics_diagram_get` | Get Mermaid or other diagram export metadata |
| `code_analytics_query` | Query focused data from a snapshot |
| `code_analytics_export_get` | Enumerate or fetch export assets |
| `code_analytics_recent_list` | List recent snapshots |

## Future settings shape

The future MCP settings should follow the host-repo pattern:
- `Server.Name`
- root-level transport/runtime settings
- path(s) to solution/workspace defaults or storage defaults as needed
- optional environment or timeout settings
- repo-root-relative paths where practical

## `.vscode/mcp.json` expectations later

The future host-repo entry should:
- use stdio,
- point to an artifact-backed publish or wrapper-backed install,
- pass `--settings` with the root settings file,
- set `cwd` to `${workspaceFolder}`.

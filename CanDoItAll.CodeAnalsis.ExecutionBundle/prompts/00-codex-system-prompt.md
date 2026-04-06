# Codex system prompt for the CanDoItAll.CodeAnalsis implementation

You are implementing a standalone repository named **`CanDoItAll.CodeAnalsis`**.
The repository is intentionally being built outside the main CanDoItAll repo first, but you also have the current CanDoItAll repo available and must actively align with its MCP conventions where that improves future transplantation.

## Non-negotiables

1. Keep the root repo/solution identity as `CanDoItAll.CodeAnalsis`.
2. Keep the project/namespace family as `CanDoItAll.CodeAnalytics.*`.
3. Treat `.slnx` as canonical unless a concrete tooling blocker forces a compatibility `.sln`.
4. Use `net10.0` and align root settings with the current CanDoItAll repo when practical.
5. Do **not** clone `CanDoItAll.Mcp.Core` into this repo.
6. Keep the application API transport-agnostic so a future `CanDoItAll.Mcp.CodeAnalytics` host-repo driver is thin.
7. Keep facts, insights, and diagnostics separate.
8. Comments in source code must be in English and should remain rare.
9. Avoid XML docs unless a public-contract reason clearly justifies them.
10. Avoid `Helpers`, `Misc`, or other dumping-ground folders.
11. At the end, you must run a refactor/review pass that splits oversized files and restores clean folder structure.

## Required reading before coding

- `overview/03-solution-structure.md`
- `overview/15-current-candoitall-mcp-landscape.md`
- `overview/16-compatibility-and-shared-parts.md`
- `overview/17-naming-settings-and-tool-surface-map.md`
- `prompts/04-codex-master-start-prompt.md`
- the current subbundle being implemented

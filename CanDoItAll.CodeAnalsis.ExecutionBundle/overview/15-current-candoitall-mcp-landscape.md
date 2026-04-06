# Current CanDoItAll MCP landscape

This document distills the uploaded CanDoItAll repo snapshot into explicit compatibility facts for the standalone `CanDoItAll.CodeAnalsis` repo.

## Observed repo facts

- current host solution file: **`CanDoItAll.slnx`**
- pinned SDK: **`10.0.200`** with roll-forward **`latestPatch`**
- current MCP/server project family:
- `CanDoItAll.Mcp.Components`
- `CanDoItAll.Mcp.Core`
- `CanDoItAll.Mcp.DotNetWatch`
- `CanDoItAll.Mcp.LocalRuntime`
- `CanDoItAll.Mcp.ProjectStructure`
- `CanDoItAll.Mcp.SshOps`

## Current local MCP registrations from `.vscode/mcp.json`

- `candoitall_dotnetwatch`
- `candoitall_sshops`
- `candoitall_components`
- `candoitall_projectstructure`

Expanded notes:
- - `candoitall_dotnetwatch` — command `powershell`; args start with `-NoProfile -ExecutionPolicy Bypass -File ...`.
- - `candoitall_sshops` — command `${workspaceFolder}\.artifacts\mcp-installs\CanDoItAll.Mcp.SshOps\current\CanDoItAll.Mcp.SshOps.exe`; args start with `--settings ${workspaceFolder}\CanDoItAll.Mcp.SshOps.settings.json`.
- - `candoitall_components` — command `${workspaceFolder}\.artifacts\mcp-installs\CanDoItAll.Mcp.Components\current\CanDoItAll.Mcp.Components.exe`; args start with `--settings ${workspaceFolder}\CanDoItAll.Mcp.Components.settings.json`.
- - `candoitall_projectstructure` — command `${workspaceFolder}\.artifacts\mcp-installs\CanDoItAll.Mcp.ProjectStructure\20260405-013848\CanDoItAll.Mcp.ProjectStructure.exe`; args start with `--settings ${workspaceFolder}\CanDoItAll.Mcp.ProjectStructure.settings.local.json`.

## Shared core already present in the host repo

The uploaded repo already centralizes reusable MCP helpers under **`CanDoItAll.Mcp.Core`**.
Important files observed:

- `src/CanDoItAll.Mcp.Core/Contracts/McpToolEnvelope.cs`
- `src/CanDoItAll.Mcp.Core/Identity/IdentifierFactories.cs`
- `src/CanDoItAll.Mcp.Core/Operations/OperationPrimitives.cs`
- `src/CanDoItAll.Mcp.Core/Net/HttpProbeService.cs`
- `src/CanDoItAll.Mcp.Core/Concurrency/ResourceMutationGate.cs`
- `src/CanDoItAll.Mcp.Core/Observability/LogModels.cs`

These cover areas such as:
- structured tool envelopes and deterministic tool errors,
- correlation and server identity,
- async operation primitives,
- HTTP/TLS probing,
- concurrency gates,
- log buffering and secret redaction.

## Repeating bootstrapping pattern across current MCP servers

Across `CanDoItAll.Mcp.Components`, `CanDoItAll.Mcp.ProjectStructure`, and `CanDoItAll.Mcp.DotNetWatch`, the host repo repeatedly uses:

- `Host.CreateEmptyApplicationBuilder(settings: null)`
- JSON settings file resolution (often via `--settings`)
- environment variables with prefix **`CanDoItAllMcp_`**
- explicit options binding + validation
- `AddMcpServer().WithStdioServerTransport().WithTools<...>()`

## Current settings/config pattern

Observed root settings/examples:
- `CanDoItAll.Mcp.Components.settings.json`
- `CanDoItAll.Mcp.DotNetWatch.settings.json`
- `CanDoItAll.Mcp.ProjectStructure.settings.example.json`
- `CanDoItAll.Mcp.SshOps.settings.json`

The future `CanDoItAll.Mcp.CodeAnalytics` driver should follow the same naming family.

## Current Codex assets pattern

- repo-managed Codex readme: `codex/README.md`
- repo-managed skills:
- `codex/skills/candoitall-components-mcp/SKILL.md`
- `codex/skills/candoitall-dotnetwatch-setup/SKILL.md`
- `codex/skills/candoitall-frontend-theme/SKILL.md`
- `codex/skills/candoitall-watch-playwright-loop/SKILL.md`
- repo-local agents:
- `.codex/agents/arch-mapper.toml`
- `.codex/agents/canonical-model-skeptic.toml`
- `.codex/agents/runtime-validator.toml`

## Implication for the standalone repo

The standalone repo should align with these conventions where it improves later transplantation, but it should **not** copy host-only infrastructure just to look similar.
Instead it should expose clean library seams that the future host-repo MCP driver can wrap.

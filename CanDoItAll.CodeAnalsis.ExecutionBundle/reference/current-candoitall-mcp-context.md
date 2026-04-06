# Current CanDoItAll MCP context (captured from uploaded repo)

## Repo snapshot used

- repo snapshot folder: `CanDoItAll-canonical-model-refactor`
- host solution file: `CanDoItAll.slnx`
- pinned SDK: `10.0.200`
- target framework family in current MCP projects: `net10.0`

## Current MCP-related projects observed

- `CanDoItAll.Mcp.Components`
- `CanDoItAll.Mcp.Core`
- `CanDoItAll.Mcp.DotNetWatch`
- `CanDoItAll.Mcp.LocalRuntime`
- `CanDoItAll.Mcp.ProjectStructure`
- `CanDoItAll.Mcp.SshOps`

## Current MCP/server registrations observed in `.vscode/mcp.json`

- `candoitall_dotnetwatch`
- `candoitall_sshops`
- `candoitall_components`
- `candoitall_projectstructure`

## Shared core files worth reading

- `src/CanDoItAll.Mcp.Core/Contracts/McpToolEnvelope.cs`
- `src/CanDoItAll.Mcp.Core/Identity/IdentifierFactories.cs`
- `src/CanDoItAll.Mcp.Core/Operations/OperationPrimitives.cs`
- `src/CanDoItAll.Mcp.Core/Net/HttpProbeService.cs`
- `src/CanDoItAll.Mcp.Core/Concurrency/ResourceMutationGate.cs`
- `src/CanDoItAll.Mcp.Core/Observability/LogModels.cs`

## Current Codex assets worth reading

- readme: `codex/README.md`
- skills:
  - `codex/skills/candoitall-components-mcp/SKILL.md`
  - `codex/skills/candoitall-dotnetwatch-setup/SKILL.md`
  - `codex/skills/candoitall-frontend-theme/SKILL.md`
  - `codex/skills/candoitall-watch-playwright-loop/SKILL.md`
- repo-local agents:
  - `.codex/agents/arch-mapper.toml`
  - `.codex/agents/canonical-model-skeptic.toml`
  - `.codex/agents/runtime-validator.toml`

## Compatibility conclusion

The standalone repo should:
- mirror root naming/config conventions where useful,
- remain engine-first and transport-agnostic,
- prepare for a future thin host-repo MCP driver,
- avoid duplicating `CanDoItAll.Mcp.Core`.

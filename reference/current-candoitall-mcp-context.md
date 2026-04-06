# Current CanDoItAll MCP Context

## Host repo facts

- Host solution file: `CanDoItAll.slnx`
- SDK pin: `10.0.200` with `latestPatch`
- Current MCP project family:
  - `CanDoItAll.Mcp.Components`
  - `CanDoItAll.Mcp.Core`
  - `CanDoItAll.Mcp.DotNetWatch`
  - `CanDoItAll.Mcp.LocalRuntime`
  - `CanDoItAll.Mcp.ProjectStructure`
  - `CanDoItAll.Mcp.SshOps`

## Current MCP registrations

- `candoitall_dotnetwatch`
- `candoitall_sshops`
- `candoitall_components`
- `candoitall_projectstructure`

## Repeating host patterns

- `Host.CreateEmptyApplicationBuilder(settings: null)`
- JSON settings files resolved through `--settings`
- `CanDoItAllMcp_` environment-variable prefix
- Explicit options binding and startup validation
- `AddMcpServer().WithStdioServerTransport().WithTools<...>()`

## Implication for this repo

`CanDoItAll.CodeAnalsis` should stay engine-first and transport-agnostic so the future `CanDoItAll.Mcp.CodeAnalytics` layer is mostly mapping, settings, and registration work.

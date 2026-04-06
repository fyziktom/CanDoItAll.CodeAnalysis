# Reuse Later vs Do Not Duplicate Now

## Reuse later from `CanDoItAll.Mcp.Core`

- `Contracts/McpToolEnvelope.cs`
- `Identity/IdentifierFactories.cs`
- `Operations/OperationPrimitives.cs`
- Host-side logging, correlation, and secret-redaction helpers when the future driver needs them

## Do not duplicate now in the standalone repo

- MCP tool envelopes or host transport contracts
- Host correlation and operation identifier factories
- Host startup and settings-binding boilerplate
- `.vscode/mcp.json` runtime registration
- Artifact publishing or reinstall wiring
- Tool-install script logic from `tools/Reinstall-CanDoItAllMcps.ps1`

## Standalone responsibilities right now

- Transport-agnostic analysis contracts
- Deterministic snapshot and export pipeline
- SSR-first inspection UI
- Compatibility reference artifacts that keep the future driver seam thin

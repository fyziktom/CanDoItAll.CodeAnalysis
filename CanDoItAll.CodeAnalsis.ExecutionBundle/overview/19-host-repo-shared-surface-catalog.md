# Host-repo shared surface catalog

This document turns the uploaded CanDoItAll repository into a concrete compatibility catalog for the future
`CanDoItAll.Mcp.CodeAnalytics` transplantation step.

## Host repo facts captured

- canonical host solution file: `CanDoItAll.slnx`
- pinned SDK: `10.0.200`
- current MCP-related projects:
  - `CanDoItAll.Mcp.Core`
  - `CanDoItAll.Mcp.Components`
  - `CanDoItAll.Mcp.DotNetWatch`
  - `CanDoItAll.Mcp.LocalRuntime`
  - `CanDoItAll.Mcp.ProjectStructure`
  - `CanDoItAll.Mcp.SshOps`
- current VS Code MCP registrations:
  - `candoitall_dotnetwatch`
  - `candoitall_sshops`
  - `candoitall_components`
  - `candoitall_projectstructure`

## Shared parts to reuse later from `CanDoItAll.Mcp.Core`

These parts belong to the host repo and should be treated as **future integration dependencies**, not standalone-repo implementation targets:

- `Contracts/McpToolEnvelope.cs`
- `Identity/IdentifierFactories.cs`
- `Operations/OperationPrimitives.cs`
- `Net/HttpProbeService.cs`
- `Concurrency/ResourceMutationGate.cs`
- `Observability/LogModels.cs`

## What the standalone repo should prepare for

The standalone `CanDoItAll.CodeAnalsis` repo should shape its internal engine so that the future
`CanDoItAll.Mcp.CodeAnalytics` driver can stay thin.

That means the standalone repo should already expose:

- deterministic request/response DTOs for snapshot build, summary retrieval, diagram retrieval, query, and export retrieval,
- diagnostics that can later be wrapped into `McpToolEnvelope<T>`,
- correlation-friendly operation identifiers without re-implementing host-side identifier factories,
- transport-agnostic orchestration services that do not know about STDIO servers or MCP envelopes,
- settings objects that can later be mapped into the current host-repo configuration style.

## Host-pattern observations worth preserving

### Program/bootstrap pattern
Current MCP servers use a recognizable pattern:
- `Program.cs`
- `Configuration` or runtime settings objects
- `Tools` class with tool methods
- coordinator/service classes behind the tools
- host startup via `AddMcpServer().WithStdioServerTransport().WithTools<...>()`

### Settings pattern
Current settings files and environment mapping use a consistent prefix:
- `CanDoItAllMcp_`

The future driver should align to that style:
- example settings file name: `CanDoItAll.Mcp.CodeAnalytics.settings.json`
- example env prefix: `CanDoItAllMcp_`

### Registration pattern
The host repo keeps MCP registration details in `.vscode/mcp.json`.

The future driver should therefore assume that a thin host-repo registration entry will be added later,
rather than inventing a standalone-only registration convention that cannot be transplanted cleanly.

## Do-not-duplicate list

Do **not** copy these concerns into the standalone engine repo:

- host MCP envelopes,
- host correlation factories,
- host runtime bootstrapping boilerplate,
- host reinstall/publish wiring,
- host `.vscode/mcp.json` registration entries,
- host probe and concurrency infrastructure unless a truly engine-local equivalent is needed.

## Thin-driver design target

The future `CanDoItAll.Mcp.CodeAnalytics` project should mainly do five things:

1. load validated settings,
2. resolve application services from the transplanted engine libraries,
3. map MCP tool parameters to engine requests,
4. wrap results/diagnostics into `McpToolEnvelope<T>` responses,
5. register itself in the host repo like the existing MCP servers.

If the future driver needs to re-architect the engine, the standalone repo drifted too far.

## Naming reminders

- repo + canonical solution identity: `CanDoItAll.CodeAnalsis`
- project/assembly/namespace family: `CanDoItAll.CodeAnalytics.*`
- future host driver: `CanDoItAll.Mcp.CodeAnalytics`
- future tool prefix: `code_analytics_`

## Validation question

A useful checkpoint for every major slice is:

> Could this implementation be transplanted into the main CanDoItAll repo by adding a thin `CanDoItAll.Mcp.CodeAnalytics`
> driver and wiring it like the existing MCP servers — without redesigning the analysis engine?

If the answer is not clearly **yes**, the slice needs adjustment.

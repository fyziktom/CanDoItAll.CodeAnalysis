# Public API Reference

The primary engine contract is `ICodeAnalyticsApplicationService` from `CanDoItAll.CodeAnalytics.Abstractions`.

## Main Workflows

| Workflow | API |
| --- | --- |
| Build or reuse a snapshot | `BuildSnapshotAsync(BuildArchitectureSnapshotCommand)` |
| Dashboard and recent snapshots | `GetDashboardAsync`, `ListRecentSnapshotsAsync` |
| Dependency, DI service, persistence, and finding views | `GetDependenciesAsync`, `GetServicesAsync`, `GetPersistenceAsync`, `GetFindingsAsync` |
| Solution, project, and document inventory | `GetSolutionInventoryAsync`, `GetProjectInventoryAsync`, `GetDocumentSourceAsync`, `GetDocumentSymbolsAsync` |
| Type and symbol exploration | `GetTypesAsync`, `SearchSymbolsAsync`, `GetSymbolDefinitionAsync`, `GetSymbolMembersAsync`, `GetSymbolImplementationsAsync`, `GetSymbolReferencesAsync` |
| Focused context for prompts/troubleshooting | `GetFocusedContextAsync` |
| Stored outputs | `GetExportsAsync`, `GetSnapshotAsync` |

## Composition

There is not yet a reusable `AddCodeAnalytics()` DI extension. The supported composition reference is the Web app registration in `src/CanDoItAll.CodeAnalytics.Web/Program.cs`.

Required building blocks include:

- `CodeAnalyticsApplicationOptions`
- `MsBuildWorkspaceLoader`
- `SymbolFactsCollector`
- `MemberRelationshipCollector`
- `DependencyFactCollector`
- `ServiceRegistrationCollector`
- `PersistenceFactCollector`
- `ArchitectureInsightBuilder`
- `ExportBundleBuilder`
- `SnapshotJsonSerializer`
- `FileSnapshotRepository`
- `CodeAnalyticsApplicationService`

## Versioning Posture

Packages are `0.1.0` during publishing preparation. Public contracts should be treated as pre-1.0 and can still change, but package descriptions and docs should only claim implemented behavior.

## Future Driver

`CanDoItAll.Mcp.CodeAnalytics` is a future host-driver project. It should depend on the engine facade and map MCP tools to application-service calls. This repository should remain transport-agnostic and should not copy `CanDoItAll.Mcp.Core` host envelopes or runtime contracts.

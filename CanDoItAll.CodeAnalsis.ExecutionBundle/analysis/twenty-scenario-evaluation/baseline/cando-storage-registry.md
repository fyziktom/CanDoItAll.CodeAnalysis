# cando-storage-registry

## Simulated Prompt

Storage driver registry behavior is unclear. Show registry definition and the catalog/service consumer path.

## Simulated Agent Approach

Search storage registry and use relation hints around storage catalog service.

## Query

- Repository: `CanDoItAll`
- Category: `Specific`
- Query text: `IStorageDriverRegistry`
- Focus tags: `Infra`, `Service`
- Relation hints: `StorageCatalogService`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Good`
- Helpfulness score: 0,825
- Expected terms: 2/2
- Expected files: 1/1
- Useful files: 5
- Non-useful files: 3
- Noise term hits: 0
- Token budget ratio: 2,000

## Output Metrics

- Search results: 19
- Seed type: CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry
- Seed member: CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry.Resolve(CanDoItAll.Infrastructure.Storage.StorageProviderKind)
- Files: 8
- Blocks: 16
- Selected lines: 336
- Estimated tokens: 6596
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry.RegisteredKinds` (Member)
- `CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry.Resolve(CanDoItAll.Infrastructure.Storage.StorageProviderKind)` (Member)
- `CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry.TryResolve(CanDoItAll.Infrastructure.Storage.StorageProviderKind, out CanDoItAll.Infrastructure.Storage.IStorageDriver)` (Member)
- `CanDoItAll.AgentFramework.Maf.MafAgentRuntime.StorageRuntimePlugin.StorageRuntimePlugin(CanDoItAll.Infrastructure.Storage.IStorageCatalogService, CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry, CanDoItAll.AgentFramework.Models.AgentWorkspaceToolAccessSettings)` (Member)
- `CanDoItAll.Infrastructure.Storage.LocalFileStore.LocalFileStore(CanDoItAll.Infrastructure.Storage.IWorkspacePathAccessGuard, CanDoItAll.Infrastructure.Storage.IStorageCatalogService, CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry)` (Member)
- `CanDoItAll.Infrastructure.Storage.StorageAccessService.StorageAccessService(CanDoItAll.Infrastructure.Storage.IStorageCatalogService, CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry, CanDoItAll.Infrastructure.Storage.IWorkspacePathResolver)` (Member)
- `CanDoItAll.Infrastructure.Storage.StorageConnectionTestService.StorageConnectionTestService(CanDoItAll.Infrastructure.Storage.IStorageCatalogService, CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry, CanDoItAll.Infrastructure.Storage.IStorageSecretResolver, Microsoft.Extensions.Logging.ILogger<CanDoItAll.Infrastructure.Storage.StorageConnectionTestService>)` (Member)
- `CanDoItAll.Infrastructure.Storage.StoragePlacementService.StoragePlacementService(CanDoItAll.Infrastructure.Storage.IStorageCatalogService, CanDoItAll.Infrastructure.Storage.IStorageRoutingService, CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry, Microsoft.Extensions.Logging.ILogger<CanDoItAll.Infrastructure.Storage.StoragePlacementService>)` (Member)

## Selected Files

- `src/CanDoItAll.Infrastructure/Storage/Transfers/StorageTransferPipeline.cs`: 109/343 lines, 1 blocks
- `src/CanDoItAll.Web/Infrastructure/ManagedFilesEndpointRoutes.cs`: 74/185 lines, 3 blocks
- `src/CanDoItAll.AgentFramework.Maf/Runtime/Workspace/MafAgentRuntime.StorageRuntimePlugin.cs`: 58/268 lines, 3 blocks
- `src/CanDoItAll.Infrastructure/Storage/Drivers/StorageConnectionTestService.cs`: 49/58 lines, 1 blocks
- `src/CanDoItAll.Infrastructure/Storage/Abstractions/StorageContracts.cs`: 19/96 lines, 5 blocks
- `src/CanDoItAll.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs`: 10/109 lines, 1 blocks
- `tests/CanDoItAll.Tests.Integration/MafAgentRuntimeTests.cs`: 10/2061 lines, 1 blocks
- `src/CanDoItAll.Infrastructure/Storage/WorkspaceStorage.cs`: 7/264 lines, 1 blocks

# Storage registry scenario

## Query

- Query text: `IStorageDriverRegistry`
- Elapsed milliseconds: 2262
- Search results: 1
- Selected symbol: `CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry`
- Target kind: `Type`

## Definition

- Declaration: `interface CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry`
- Path: src/CanDoItAll.Infrastructure/Storage/Abstractions/StorageContracts.cs:30
- Truncated: False

```csharp
public interface IStorageDriverRegistry
{
    IReadOnlyCollection<StorageProviderKind> RegisteredKinds { get; }

    bool TryResolve(StorageProviderKind providerKind, out IStorageDriver driver);

    IStorageDriver Resolve(StorageProviderKind providerKind);
}
```

## Members

- Member count: 3
- `CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry.Resolve(CanDoItAll.Infrastructure.Storage.StorageProviderKind)` (Method)
- `CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry.TryResolve(CanDoItAll.Infrastructure.Storage.StorageProviderKind, out CanDoItAll.Infrastructure.Storage.IStorageDriver)` (Method)
- `CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry.RegisteredKinds` (Property)

## Implementations

- Count: 4
- `CanDoItAll.Infrastructure.Storage.StorageDriverRegistry` (InterfaceImplementation)
- `CanDoItAll.Tests.Unit.StorageAccessServiceTests.TestStorageDriverRegistry` (InterfaceImplementation)
- `CanDoItAll.Tests.Unit.StoragePlacementServiceTests.TestStorageDriverRegistry` (InterfaceImplementation)
- `CanDoItAll.Tests.Unit.StorageTransferPipelineTests.TestStorageDriverRegistry` (InterfaceImplementation)

## References

- Total references: 15
- Returned references: 15
- `CanDoItAll.Infrastructure.Storage.LocalFileStore` :: `CanDoItAll.Infrastructure.Storage.LocalFileStore.ResolveFileSystemDriverAsync(System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Storage/WorkspaceStorage.cs:253
- `CanDoItAll.Infrastructure.Storage.StorageAccessService` :: `CanDoItAll.Infrastructure.Storage.StorageAccessService.ResolveCapabilityMask(CanDoItAll.Infrastructure.Storage.StorageObjectReference, CanDoItAll.Infrastructure.Storage.StorageCatalogRecord?)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Storage/Access/StorageAccessService.cs:39
- `CanDoItAll.Infrastructure.Storage.StorageConnectionTestService` :: `CanDoItAll.Infrastructure.Storage.StorageConnectionTestService.TestAsync(System.Guid, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Storage/Drivers/StorageConnectionTestService.cs:27
- `CanDoItAll.Infrastructure.Storage.StoragePlacementService` :: `CanDoItAll.Infrastructure.Storage.StoragePlacementService.PlaceAsync(CanDoItAll.Infrastructure.Storage.StoragePlacementRequest, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Storage/Placement/StoragePlacementService.cs:24
- `CanDoItAll.Infrastructure.Storage.StorageTransferPipeline` :: `CanDoItAll.Infrastructure.Storage.StorageTransferPipeline.ExecuteAsync(CanDoItAll.Infrastructure.Storage.StorageTransferManifest, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Storage/Transfers/StorageTransferPipeline.cs:27
- `CanDoItAll.Infrastructure.Storage.LocalFileStore` :: `CanDoItAll.Infrastructure.Storage.LocalFileStore.LocalFileStore(CanDoItAll.Infrastructure.Storage.IWorkspacePathAccessGuard, CanDoItAll.Infrastructure.Storage.IStorageCatalogService, CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry)` (ConstructorParameter)
  Path: src/CanDoItAll.Infrastructure/Storage/WorkspaceStorage.cs:183
- `CanDoItAll.Infrastructure.Storage.StorageAccessService` :: `CanDoItAll.Infrastructure.Storage.StorageAccessService.StorageAccessService(CanDoItAll.Infrastructure.Storage.IStorageCatalogService, CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry, CanDoItAll.Infrastructure.Storage.IWorkspacePathResolver)` (ConstructorParameter)
  Path: src/CanDoItAll.Infrastructure/Storage/Access/StorageAccessService.cs:4
- `CanDoItAll.Infrastructure.Storage.StorageConnectionTestService` :: `CanDoItAll.Infrastructure.Storage.StorageConnectionTestService.StorageConnectionTestService(CanDoItAll.Infrastructure.Storage.IStorageCatalogService, CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry, CanDoItAll.Infrastructure.Storage.IStorageSecretResolver, Microsoft.Extensions.Logging.ILogger<CanDoItAll.Infrastructure.Storage.StorageConnectionTestService>)` (ConstructorParameter)
  Path: src/CanDoItAll.Infrastructure/Storage/Drivers/StorageConnectionTestService.cs:6
- `CanDoItAll.Infrastructure.Storage.StoragePlacementService` :: `CanDoItAll.Infrastructure.Storage.StoragePlacementService.StoragePlacementService(CanDoItAll.Infrastructure.Storage.IStorageCatalogService, CanDoItAll.Infrastructure.Storage.IStorageRoutingService, CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry, Microsoft.Extensions.Logging.ILogger<CanDoItAll.Infrastructure.Storage.StoragePlacementService>)` (ConstructorParameter)
  Path: src/CanDoItAll.Infrastructure/Storage/Placement/StoragePlacementService.cs:7
- `CanDoItAll.Infrastructure.Storage.StorageTransferPipeline` :: `CanDoItAll.Infrastructure.Storage.StorageTransferPipeline.StorageTransferPipeline(CanDoItAll.Infrastructure.Storage.IStorageCatalogService, CanDoItAll.Infrastructure.Storage.IStorageDriverRegistry, CanDoItAll.Infrastructure.Storage.IStorageSecretResolver, Microsoft.Extensions.Logging.ILogger<CanDoItAll.Infrastructure.Storage.StorageTransferPipeline>)` (ConstructorParameter)
  Path: src/CanDoItAll.Infrastructure/Storage/Transfers/StorageTransferPipeline.cs:8
- `CanDoItAll.Infrastructure.DependencyInjection.InfrastructureServiceCollectionExtensions` :: `CanDoItAll.Infrastructure.DependencyInjection.InfrastructureServiceCollectionExtensions.AddCanDoItAllInfrastructure(Microsoft.Extensions.DependencyInjection.IServiceCollection, Microsoft.Extensions.Configuration.IConfiguration, Microsoft.Extensions.Hosting.IHostEnvironment, System.Collections.Generic.IReadOnlyList<System.Reflection.Assembly>)` (ServiceRegistration)
  Path: src/CanDoItAll.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs:89
- `CanDoItAll.Modules.Workspace.WorkspaceService` :: `CanDoItAll.Modules.Workspace.WorkspaceService.TestStorageAsync(CanDoItAll.Modules.Workspace.StorageCatalogEditorModel, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Modules.Workspace/WorkspaceService.Storage.cs:198

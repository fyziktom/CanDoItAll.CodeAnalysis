# Common helper scenario

## Query

- Query text: `IClock`
- Elapsed milliseconds: 2196
- Search results: 1
- Selected symbol: `CanDoItAll.SharedKernel.IClock`
- Target kind: `Type`

## Definition

- Declaration: `interface CanDoItAll.SharedKernel.IClock`
- Path: src/CanDoItAll.SharedKernel/IClock.cs:3
- Truncated: False

```csharp
public interface IClock
{
    DateTimeOffset GetUtcNow();
}
```

## Members

- Member count: 1
- `CanDoItAll.SharedKernel.IClock.GetUtcNow()` (Method)

## Implementations

- Count: 4
- `CanDoItAll.SharedKernel.SystemClock` (InterfaceImplementation)
- `CanDoItAll.Tests.Integration.AutomationRuntimeIntegrationTests.ThrowOnceArmedClock` (InterfaceImplementation)
- `CanDoItAll.Tests.Unit.StorageCatalogServiceTests.TestClock` (InterfaceImplementation)
- `CanDoItAll.Tests.Unit.WorkbenchStateServiceTests.TestClock` (InterfaceImplementation)

## References

- Total references: 160
- Returned references: 40
- `CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobTracker` :: `CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobTracker.UpdateStateAsync(System.Guid, CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobState, string?, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/BackgroundJobs/BackgroundJobs.cs:215
- `CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobTracker` :: `CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobTracker.PrepareJob(string, string, System.Collections.Generic.IReadOnlyDictionary<string, string>?, System.Guid?)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/BackgroundJobs/BackgroundJobs.cs:238
- `CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService` :: `CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService.ActivateAsync(System.Guid, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/ControlPlane/DatabaseProfileControlPlaneService.cs:217
- `CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService` :: `CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService.TryResolveExplicitOverrideLocked()` (Invocation)
  Path: src/CanDoItAll.Infrastructure/ControlPlane/DatabaseProfileControlPlaneService.cs:358
- `CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService` :: `CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService.TryCreateLegacyProfileLocked()` (Invocation)
  Path: src/CanDoItAll.Infrastructure/ControlPlane/DatabaseProfileControlPlaneService.cs:505
- `CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService` :: `CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService.CreateManagedSqliteProfileLocked()` (Invocation)
  Path: src/CanDoItAll.Infrastructure/ControlPlane/DatabaseProfileControlPlaneService.cs:537
- `CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService` :: `CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService.BuildPersistedProfile(CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileEditorModel, CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileRecord?)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/ControlPlane/DatabaseProfileControlPlaneService.cs:573
- `CanDoItAll.Infrastructure.ControlPlane.DatabaseSnapshotService` :: `CanDoItAll.Infrastructure.ControlPlane.DatabaseSnapshotService.ExportSnapshotPackageAsync(CanDoItAll.Infrastructure.ControlPlane.ResolvedDatabaseProfile, System.Guid, string, CanDoItAll.Infrastructure.ControlPlane.DatabaseSnapshotTransportKind, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/ControlPlane/DatabaseSnapshots.cs:382
- `CanDoItAll.Infrastructure.Search.SearchIndexService` :: `CanDoItAll.Infrastructure.Search.SearchIndexService.UpsertAsync(CanDoItAll.Infrastructure.Search.SearchDocumentInput, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Search/SearchIndexing.cs:108
- `CanDoItAll.Infrastructure.Storage.StorageCatalogService` :: `CanDoItAll.Infrastructure.Storage.StorageCatalogService.EnsureBootstrapFileSystemStorageAsync(System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Storage/Persistence/StorageCatalogService.cs:70
- `CanDoItAll.Infrastructure.Storage.StorageCatalogService` :: `CanDoItAll.Infrastructure.Storage.StorageCatalogService.SaveAsync(CanDoItAll.Infrastructure.Storage.StorageCatalogRecord, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Storage/Persistence/StorageCatalogService.cs:95
- `CanDoItAll.Infrastructure.Storage.StorageCatalogService` :: `CanDoItAll.Infrastructure.Storage.StorageCatalogService.SaveRuleAsync(CanDoItAll.Infrastructure.Storage.StorageRoutingRule, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Storage/Persistence/StorageCatalogService.cs:168

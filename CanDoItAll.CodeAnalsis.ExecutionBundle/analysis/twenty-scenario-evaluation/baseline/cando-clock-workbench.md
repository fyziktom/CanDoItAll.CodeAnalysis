# cando-clock-workbench

## Simulated Prompt

IClock is used everywhere, but I only care about Workbench behavior. Show the focused helper usage around Workbench.

## Simulated Agent Approach

Use helper seed IClock with a concrete Workbench relation hint to avoid broad helper sampling.

## Query

- Repository: `CanDoItAll`
- Category: `Specific`
- Query text: `IClock`
- Focus tags: None
- Relation hints: `Workbench`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Good`
- Helpfulness score: 1,000
- Expected terms: 2/2
- Expected files: 1/1
- Useful files: 1
- Non-useful files: 0
- Noise term hits: 0
- Token budget ratio: 0,222

## Output Metrics

- Search results: 40
- Seed type: CanDoItAll.SharedKernel.IClock
- Seed member: CanDoItAll.SharedKernel.IClock.GetUtcNow()
- Files: 1
- Blocks: 2
- Selected lines: 6
- Estimated tokens: 356
- Usage callers: 42
- Usage clusters: 1

## Symbol Search Top Results

- `CanDoItAll.SharedKernel.IClock.GetUtcNow()` (Member)
- `CanDoItAll.SharedKernel.IClock` (Type)
- `CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobTracker.BackgroundJobTracker(Microsoft.EntityFrameworkCore.IDbContextFactory<CanDoItAll.Infrastructure.Persistence.AppDbContext>, CanDoItAll.Infrastructure.BackgroundJobs.IBackgroundJobQueue, CanDoItAll.SharedKernel.IClock)` (Member)
- `CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService.DatabaseProfileControlPlaneService(Microsoft.Extensions.Configuration.IConfiguration, Microsoft.Extensions.Options.IOptions<CanDoItAll.Infrastructure.Configuration.StorageOptions>, Microsoft.Extensions.Hosting.IHostEnvironment, CanDoItAll.Infrastructure.ControlPlane.IControlPlanePathResolver, CanDoItAll.Infrastructure.ControlPlane.IControlPlaneSecretProtector, CanDoItAll.SharedKernel.IClock, Microsoft.Extensions.Logging.ILogger<CanDoItAll.Infrastructure.ControlPlane.DatabaseProfileControlPlaneService>)` (Member)
- `CanDoItAll.Infrastructure.ControlPlane.DatabaseSnapshotService.DatabaseSnapshotService(CanDoItAll.Infrastructure.ControlPlane.IDatabaseProfileRuntimeAccessor, CanDoItAll.Infrastructure.ControlPlane.IDatabaseProfileService, CanDoItAll.Infrastructure.ControlPlane.IAppDatabaseBootstrapper, CanDoItAll.Infrastructure.Persistence.ISwitchableAppDbContextFactory, CanDoItAll.Infrastructure.ControlPlane.IControlPlanePathResolver, CanDoItAll.Infrastructure.Storage.IStorageTransferPipeline, Microsoft.Extensions.Options.IOptions<CanDoItAll.Infrastructure.Configuration.ControlPlaneOptions>, CanDoItAll.SharedKernel.IClock, Microsoft.Extensions.Logging.ILogger<CanDoItAll.Infrastructure.ControlPlane.DatabaseSnapshotService>)` (Member)
- `CanDoItAll.Infrastructure.Search.SearchIndexService.SearchIndexService(Microsoft.EntityFrameworkCore.IDbContextFactory<CanDoItAll.Infrastructure.Persistence.AppDbContext>, CanDoItAll.SharedKernel.IClock)` (Member)
- `CanDoItAll.Infrastructure.Storage.StorageCatalogService.StorageCatalogService(Microsoft.EntityFrameworkCore.IDbContextFactory<CanDoItAll.Infrastructure.Persistence.AppDbContext>, CanDoItAll.Infrastructure.Storage.IWorkspacePathResolver, CanDoItAll.SharedKernel.IClock)` (Member)
- `CanDoItAll.Modules.Activity.ActivityService.ActivityService(Microsoft.EntityFrameworkCore.IDbContextFactory<CanDoItAll.Infrastructure.Persistence.AppDbContext>, CanDoItAll.SharedKernel.IClock, CanDoItAll.Infrastructure.Search.ISearchIndexService, Microsoft.Extensions.Logging.ILogger<CanDoItAll.Modules.Activity.ActivityService>)` (Member)

## Selected Files

- `src/CanDoItAll.SharedKernel/Time/IClock.cs`: 6/11 lines, 2 blocks

## Usage Summary Samples

- `CanDoItAll.Modules.Workbench` / `CanDoItAll.Modules.Workbench`: 42 callers
  - `CanDoItAll.Modules.Workbench.ProjectGanttPreviewService` -> `CanDoItAll.Modules.Workbench.ProjectGanttPreviewService.BuildAsync(System.Guid, System.Threading.CancellationToken)`

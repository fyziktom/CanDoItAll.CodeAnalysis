# cando-db-save

## Simulated Prompt

SaveChanges coordination in AppDbContext is risky. Show the save path and runtime state collaborators.

## Simulated Agent Approach

Use DB tags on AppDbContext with relation hints for runtime state and coordination.

## Query

- Repository: `CanDoItAll`
- Category: `Specific`
- Query text: `AppDbContext`
- Focus tags: `EntityFramework`
- Relation hints: `DatabaseRuntimeState`, `SaveChanges`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Good`
- Helpfulness score: 0,759
- Expected terms: 2/3
- Expected files: 2/2
- Useful files: 3
- Non-useful files: 4
- Noise term hits: 0
- Token budget ratio: 0,908

## Output Metrics

- Search results: 40
- Seed type: CanDoItAll.Infrastructure.Persistence.AppDbContext
- Seed member: CanDoItAll.Infrastructure.Persistence.AppDbContext.OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder)
- Files: 7
- Blocks: 11
- Selected lines: 153
- Estimated tokens: 2905
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `CanDoItAll.Infrastructure.Persistence.ISwitchableAppDbContextFactory.CreateDbContextForProfileAsync(CanDoItAll.Infrastructure.ControlPlane.ResolvedDatabaseProfile, System.Threading.CancellationToken)` (Member)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.AppDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<CanDoItAll.Infrastructure.Persistence.AppDbContext>, System.IDisposable?)` (Member)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.Dispose()` (Member)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.DisposeAsync()` (Member)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithCoordination(System.Func<int>)` (Member)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithCoordinationAsync(System.Func<System.Threading.Tasks.Task<int>>, System.Threading.CancellationToken)` (Member)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithRetry(System.Func<int>)` (Member)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithRetryAsync(System.Func<System.Threading.Tasks.Task<int>>, System.Threading.CancellationToken)` (Member)

## Selected Files

- `src/CanDoItAll.Infrastructure/Storage/Persistence/StorageCatalogService.cs`: 40/308 lines, 1 blocks
- `src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs`: 33/193 lines, 4 blocks
- `src/CanDoItAll.Infrastructure/Search/SearchIndexing.cs`: 28/157 lines, 1 blocks
- `src/CanDoItAll.Infrastructure/BackgroundJobs/BackgroundJobs.cs`: 25/267 lines, 1 blocks
- `src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs`: 13/170 lines, 1 blocks
- `src/CanDoItAll.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs`: 11/109 lines, 2 blocks
- `src/CanDoItAll.Infrastructure/ControlPlane/DatabaseTransferModels.cs`: 3/91 lines, 1 blocks

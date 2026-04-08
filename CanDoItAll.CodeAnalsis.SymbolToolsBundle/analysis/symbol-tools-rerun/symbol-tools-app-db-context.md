# Database scenario

## Query

- Query text: `AppDbContext`
- Elapsed milliseconds: 2181
- Search results: 2
- Selected symbol: `CanDoItAll.Infrastructure.Persistence.AppDbContext`
- Target kind: `Type`

## Definition

- Declaration: `class CanDoItAll.Infrastructure.Persistence.AppDbContext : Microsoft.EntityFrameworkCore.DbContext`
- Path: src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs:5
- Truncated: False

```csharp
public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IDisposable? runtimeLease = null) : DbContext(options)
{
    private IDisposable? _runtimeLease = runtimeLease;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        foreach (var assembly in AppDbContextModelRegistry.Assemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        base.OnModelCreating(modelBuilder);
    }

    public override void Dispose()
    {
        base.Dispose();
        ReleaseRuntimeLease();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        ReleaseRuntimeLease();
    }

    private void ReleaseRuntimeLease()
    {
        Interlocked.Exchange(ref _runtimeLease, null)?.Dispose();
    }
}
```

## Members

- Member count: 6
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.AppDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<CanDoItAll.Infrastructure.Persistence.AppDbContext>, System.IDisposable?)` (Constructor)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.Dispose()` (Method)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.DisposeAsync()` (Method)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder)` (Method)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ReleaseRuntimeLease()` (Method)
- `CanDoItAll.Infrastructure.Persistence.AppDbContext._runtimeLease` (Field)

## Implementations

- Count: 0

## References

- Total references: 110
- Returned references: 40
- `CanDoItAll.Infrastructure.Persistence.AppDbContext` :: `CanDoItAll.Infrastructure.Persistence.AppDbContext.Dispose()` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs:25
- `CanDoItAll.Infrastructure.Persistence.AppDbContext` :: `CanDoItAll.Infrastructure.Persistence.AppDbContext.DisposeAsync()` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs:31
- `CanDoItAll.Infrastructure.Persistence.AppDbContextFactory` :: `CanDoItAll.Infrastructure.Persistence.AppDbContextFactory.CreateDbContext(string[])` (ObjectCreation)
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs:151
- `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory` :: `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory.CreateDbContextAsync(System.Threading.CancellationToken)` (ObjectCreation)
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs:124
- `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory` :: `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory.CreateDbContextForProfileAsync(CanDoItAll.Infrastructure.ControlPlane.ResolvedDatabaseProfile, System.Threading.CancellationToken)` (ObjectCreation)
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs:138
- `CanDoItAll.Infrastructure.Persistence.AppDbContextFactory` :: `CanDoItAll.Infrastructure.Persistence.AppDbContextFactory.CreateDbContext(string[])` (MethodReturn)
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs:144
- `CanDoItAll.Infrastructure.Persistence.AppDbContextOptionsConfigurator` :: `CanDoItAll.Infrastructure.Persistence.AppDbContextOptionsConfigurator.CreateOptions(CanDoItAll.Infrastructure.ControlPlane.ResolvedDatabaseProfile)` (MethodReturn)
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs:23
- `CanDoItAll.Infrastructure.Persistence.ISwitchableAppDbContextFactory` :: `CanDoItAll.Infrastructure.Persistence.ISwitchableAppDbContextFactory.CreateDbContextForProfileAsync(CanDoItAll.Infrastructure.ControlPlane.ResolvedDatabaseProfile, System.Threading.CancellationToken)` (MethodReturn)
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs:16
- `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory` :: `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory.CreateDbContext()` (MethodReturn)
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs:111
- `CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobTracker` :: `CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobTracker.BackgroundJobTracker(Microsoft.EntityFrameworkCore.IDbContextFactory<CanDoItAll.Infrastructure.Persistence.AppDbContext>, CanDoItAll.Infrastructure.BackgroundJobs.IBackgroundJobQueue, CanDoItAll.SharedKernel.IClock)` (ConstructorParameter)
  Path: src/CanDoItAll.Infrastructure/BackgroundJobs/BackgroundJobs.cs:125
- `CanDoItAll.Infrastructure.Search.SearchIndexService` :: `CanDoItAll.Infrastructure.Search.SearchIndexService.SearchIndexService(Microsoft.EntityFrameworkCore.IDbContextFactory<CanDoItAll.Infrastructure.Persistence.AppDbContext>, CanDoItAll.SharedKernel.IClock)` (ConstructorParameter)
  Path: src/CanDoItAll.Infrastructure/Search/SearchIndexing.cs:84
- `CanDoItAll.Infrastructure.Storage.StorageCatalogService` :: `CanDoItAll.Infrastructure.Storage.StorageCatalogService.StorageCatalogService(Microsoft.EntityFrameworkCore.IDbContextFactory<CanDoItAll.Infrastructure.Persistence.AppDbContext>, CanDoItAll.Infrastructure.Storage.IWorkspacePathResolver, CanDoItAll.SharedKernel.IClock)` (ConstructorParameter)
  Path: src/CanDoItAll.Infrastructure/Storage/Persistence/StorageCatalogService.cs:7

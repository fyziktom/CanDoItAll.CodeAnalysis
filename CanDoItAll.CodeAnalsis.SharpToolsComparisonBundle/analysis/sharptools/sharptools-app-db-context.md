# Database scenario

## Query

- Query text: `AppDbContext`
- Warm elapsed milliseconds: 41394
- Warm call count: 5

## Search sequence

1. `SearchDefinitions("\\bAppDbContext\\b")`
   Result: 20 matches dominated by usage sites such as `IDbContextFactory<AppDbContext>` and helper method parameters. The exact type definition was not obvious from the first page.
2. `SearchDefinitions("\\b(class|record|interface)\\s+AppDbContext\\b")`
   Result: exact definition at `src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs:5`.
3. `ViewDefinition("CanDoItAll.Infrastructure.Persistence.AppDbContext")`
4. `GetMembers("CanDoItAll.Infrastructure.Persistence.AppDbContext", includePrivateMembers: false)`
5. `FindReferences("CanDoItAll.Infrastructure.Persistence.AppDbContext")`

## Definition

`ViewDefinition` returned the full type and a large referencing-type list with 20 shown and 77 more omitted.

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

`GetMembers` returned four public/protected members:

- `CanDoItAll.Infrastructure.Persistence.AppDbContext.AppDbContext(DbContextOptions<AppDbContext>, IDisposable?)`
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.OnModelCreating(ModelBuilder)`
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.Dispose()`
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.DisposeAsync()`

## References

`FindReferences` reported `288` total references with `20` displayed in the first response. Representative references from the displayed set:

- `CanDoItAll.Infrastructure.DependencyInjection.InfrastructureServiceCollectionExtensions.AddCanDoItAllInfrastructure(...)`
  Context: `services.AddSingleton<IDbContextFactory<AppDbContext>>(...)`
- `CanDoItAll.Infrastructure.Persistence.ISwitchableAppDbContextFactory`
  Context: `public interface ISwitchableAppDbContextFactory : IDbContextFactory<AppDbContext>`
- `CanDoItAll.Infrastructure.Search.SearchIndexService`
  Context: constructor injection of `IDbContextFactory<AppDbContext>`
- `CanDoItAll.Modules.Activity.ActivityService`
  Context: constructor injection of `IDbContextFactory<AppDbContext>`
- `CanDoItAll.Modules.Projects.ProjectsService.LoadPhaseCountsAsync(...)`
  Context: method parameter `AppDbContext dbContext`
- `CanDoItAll.Modules.Projects.ProjectsService.ValidateHierarchyConnectionAsync(...)`
  Context: method parameter `AppDbContext dbContext`
- `CanDoItAll.Modules.Projects.ProjectsSchemaInitializer.EnsureAsync(...)`
  Context: schema bootstrap method parameter `AppDbContext dbContext`
- `CanDoItAll.Modules.Workspace.ConnectorCommandSchemaInitializer.EnsureAsync(...)`
  Context: schema bootstrap method parameter `AppDbContext dbContext`
- `CanDoItAll.Modules.Workspace.ConnectorCommandProcessor`
  Context: constructor injection of `IDbContextFactory<AppDbContext>`
- `CanDoItAll.Modules.Workspace.WorkspaceService`
  Context: constructor injection of `IDbContextFactory<AppDbContext>`

## Working impression

The SharpTools sequence exposes the exact type cleanly after one refinement search, but the usage side is broad. It tells the agent where `AppDbContext` appears, yet it does not pre-rank the most relevant consumers for a database-trouble path. The agent still has to decide which references matter.

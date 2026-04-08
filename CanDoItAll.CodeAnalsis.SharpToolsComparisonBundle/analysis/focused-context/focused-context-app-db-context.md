# Database scenario

## Query

- Query text: `AppDbContext`
- Focus tags: `Db`
- Depth: 2
- Requested intent: `Auto`
- Requested precision: `Auto`
- Elapsed milliseconds: 851

## Resolution

- Seed type: CanDoItAll.Infrastructure.Persistence.AppDbContext
- Seed member: CanDoItAll.Infrastructure.Persistence.AppDbContext.OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder)
- Seed explanation: Resolved from prompt text to member CanDoItAll.Infrastructure.Persistence.AppDbContext.OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder).
- Strategy explanation: Used default trouble-path expansion.
- Resolved intent: `TroublePath`
- Resolved precision: `Balanced`

## Stats

- Files: 5
- Blocks: 8
- Selected lines: 139
- Total lines in selected files: 910

## Implementation Types

- None

## Selected Types

- `CanDoItAll.Infrastructure.Persistence.AppDbContext`
  Path: src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs
  Kind: `Class`
  Project: `proj-candoitall-infrastructure`
- `CanDoItAll.Infrastructure.Persistence.AppDbContextFactory`
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs
  Kind: `Class`
  Project: `proj-candoitall-infrastructure`
- `CanDoItAll.Infrastructure.Persistence.AppDbContextOptionsConfigurator`
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs
  Kind: `Class`
  Project: `proj-candoitall-infrastructure`
- `CanDoItAll.Infrastructure.Persistence.ISwitchableAppDbContextFactory`
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs
  Kind: `Interface`
  Project: `proj-candoitall-infrastructure`
- `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory`
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs
  Kind: `Class`
  Project: `proj-candoitall-infrastructure`
- `CanDoItAll.Infrastructure.Storage.StorageCatalogService`
  Path: src/CanDoItAll.Infrastructure/Storage/Persistence/StorageCatalogService.cs
  Kind: `Class`
  Project: `proj-candoitall-infrastructure`
- `CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobTracker`
  Path: src/CanDoItAll.Infrastructure/BackgroundJobs/BackgroundJobs.cs
  Kind: `Class`
  Project: `proj-candoitall-infrastructure`
- `CanDoItAll.Infrastructure.Search.SearchIndexService`
  Path: src/CanDoItAll.Infrastructure/Search/SearchIndexing.cs
  Kind: `Class`
  Project: `proj-candoitall-infrastructure`

## Selected Members

- `CanDoItAll.Infrastructure.Persistence.AppDbContext.OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder)`
  Type: `CanDoItAll.Infrastructure.Persistence.AppDbContext`
  Kind: `Method`
  Path: src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs
  Line: 11

## Usage Summary

- None

## File Excerpts

### src/CanDoItAll.Infrastructure/Storage/Persistence/StorageCatalogService.cs

- Total lines: 258
- Selected lines: 40
- Types: `CanDoItAll.Infrastructure.Storage.StorageCatalogService`

#### CanDoItAll.Infrastructure.Storage.StorageCatalogService.SaveAsync(CanDoItAll.Infrastructure.Storage.StorageCatalogRecord, System.Threading.CancellationToken)

- Kind: `Method`
- Lines: 81-120

```csharp

    public async Task<StorageCatalogRecord> SaveAsync(StorageCatalogRecord record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);

        await EnsureBootstrapFileSystemStorageAsync(cancellationToken);

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await dbContext.Set<StorageCatalogRecord>()
            .FirstOrDefaultAsync(item => item.Id == record.Id, cancellationToken);

        if (entity is null)
        {
            entity = new StorageCatalogRecord
            {
                CreatedAtUtc = clock.GetUtcNow()
            };
            await dbContext.Set<StorageCatalogRecord>().AddAsync(entity, cancellationToken);
        }

        entity.Name = string.IsNullOrWhiteSpace(record.Name) ? $"Storage {record.ProviderKind}" : record.Name.Trim();
        entity.ProviderKind = record.ProviderKind;
        entity.IsEnabled = record.IsEnabled;
        entity.IsSystemDefault = record.IsSystemDefault;
        entity.IsReadOnly = record.IsReadOnly;
        entity.DisplayOrder = record.DisplayOrder;
        entity.ConnectionMode = record.ConnectionMode;
        entity.EndpointOrRoot = record.EndpointOrRoot?.Trim() ?? string.Empty;
        entity.ConfigJson = string.IsNullOrWhiteSpace(record.ConfigJson) ? "{}" : record.ConfigJson;
        entity.CapabilityMask = record.CapabilityMask;
        entity.HealthStatus = record.HealthStatus;
        entity.LastTestedAtUtc = record.LastTestedAtUtc;
        entity.LastHealthMessage = record.LastHealthMessage?.Trim() ?? string.Empty;
        entity.CredentialSecretId = record.CredentialSecretId;
        entity.UpdatedAtUtc = clock.GetUtcNow();

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

```

### src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs

- Total lines: 189
- Selected lines: 33
- Types: `CanDoItAll.Infrastructure.Persistence.AppDbContextFactory`, `CanDoItAll.Infrastructure.Persistence.AppDbContextOptionsConfigurator`, `CanDoItAll.Infrastructure.Persistence.ISwitchableAppDbContextFactory`, `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory`

#### CanDoItAll.Infrastructure.Persistence.ISwitchableAppDbContextFactory.CreateDbContextForProfileAsync(CanDoItAll.Infrastructure.ControlPlane.ResolvedDatabaseProfile, System.Threading.CancellationToken)

- Kind: `Method`
- Lines: 16-20

```csharp
{
    Task<AppDbContext> CreateDbContextForProfileAsync(
        ResolvedDatabaseProfile profile,
        CancellationToken cancellationToken = default);
}
```

#### CanDoItAll.Infrastructure.Persistence.AppDbContextOptionsConfigurator.CreateOptions(CanDoItAll.Infrastructure.ControlPlane.ResolvedDatabaseProfile)

- Kind: `Method`
- Lines: 23-30

```csharp
{
    public static DbContextOptions<AppDbContext> CreateOptions(ResolvedDatabaseProfile profile)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        Configure(optionsBuilder, profile);
        return optionsBuilder.Options;
    }

```

#### CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory.CreateDbContextForProfileAsync(CanDoItAll.Infrastructure.ControlPlane.ResolvedDatabaseProfile, System.Threading.CancellationToken)

- Kind: `Method`
- Lines: 133-141

```csharp

    public Task<AppDbContext> CreateDbContextForProfileAsync(
        ResolvedDatabaseProfile profile,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(profile);
        return Task.FromResult(new AppDbContext(AppDbContextOptionsConfigurator.CreateOptions(profile)));
    }
}
```

#### CanDoItAll.Infrastructure.Persistence.AppDbContextFactory.CreateDbContext(string[])

- Kind: `Method`
- Lines: 144-154

```csharp
{
    public AppDbContext CreateDbContext(string[] args)
    {
        ConfigureModuleAssemblies();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var databaseOptions = BuildDatabaseOptions();
        AppDbContextOptionsConfigurator.Configure(optionsBuilder, databaseOptions, Directory.GetCurrentDirectory());
        return new AppDbContext(optionsBuilder.Options);
    }

```

### src/CanDoItAll.Infrastructure/Search/SearchIndexing.cs

- Total lines: 157
- Selected lines: 28
- Types: `CanDoItAll.Infrastructure.Search.SearchIndexService`

#### CanDoItAll.Infrastructure.Search.SearchIndexService.SearchAsync(string, int, System.Threading.CancellationToken)

- Kind: `Method`
- Lines: 130-157

```csharp

    public async Task<IReadOnlyList<SearchResult>> SearchAsync(string query, int take = 12, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var normalized = query.Trim().ToLowerInvariant();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Set<SearchDocument>()
            .Where(document =>
                EF.Functions.Like(document.Title.ToLower(), $"%{normalized}%") ||
                EF.Functions.Like(document.Summary.ToLower(), $"%{normalized}%") ||
                EF.Functions.Like(document.Body.ToLower(), $"%{normalized}%"))
            .OrderBy(document => document.Title)
            .Take(Math.Clamp(take, 1, 50))
            .Select(document => new SearchResult(
                document.Id,
                document.Category,
                document.Title,
                document.Summary,
                document.Route,
                document.ProjectId))
            .ToListAsync(cancellationToken);
    }
}
```

### src/CanDoItAll.Infrastructure/BackgroundJobs/BackgroundJobs.cs

- Total lines: 267
- Selected lines: 25
- Types: `CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobTracker`

#### CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobTracker.PrepareJob(string, string, System.Collections.Generic.IReadOnlyDictionary<string, string>?, System.Guid?)

- Kind: `Method`
- Lines: 229-253

```csharp

    private PreparedBackgroundJob PrepareJob(
        string jobType,
        string description,
        IReadOnlyDictionary<string, string>? metadata,
        Guid? correlationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jobType);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        var now = clock.GetUtcNow();
        var normalizedMetadata = NormalizeMetadata(metadata);
        var record = new BackgroundJobRecord
        {
            JobType = jobType.Trim(),
            Description = description.Trim(),
            CorrelationId = correlationId ?? Guid.NewGuid(),
            MetadataJson = SerializeMetadata(normalizedMetadata),
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        return new PreparedBackgroundJob(record, normalizedMetadata);
    }

```

### src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs

- Total lines: 39
- Selected lines: 13
- Types: `CanDoItAll.Infrastructure.Persistence.AppDbContext`

#### CanDoItAll.Infrastructure.Persistence.AppDbContext.OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder)

- Kind: `Method`
- Lines: 10-22

```csharp

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        foreach (var assembly in AppDbContextModelRegistry.Assemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        base.OnModelCreating(modelBuilder);
    }

```


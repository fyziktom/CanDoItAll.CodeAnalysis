# Database scenario

## Query

- Query text: `AppDbContext`
- Focus tags: `Db`
- Relation hints: None
- Depth: 2
- Requested intent: `Auto`
- Requested precision: `Auto`
- Elapsed milliseconds: 1931

## Resolution

- Seed type: CanDoItAll.Infrastructure.Persistence.AppDbContext
- Seed member: CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithCoordination(System.Func<int>)
- Seed explanation: Resolved from prompt text to member CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithCoordination(System.Func<int>).
- Strategy explanation: Used default trouble-path expansion.
- Resolved relation hints: None
- Resolved intent: `TroublePath`
- Resolved precision: `Balanced`

## Stats

- Files: 8
- Blocks: 16
- Selected lines: 230
- Total lines in selected files: 1453

## Selection Reasons

- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ResolveSqliteWriteGate()`
  Target kind: `Member`
  Reason: `SeedContext`
  Role: `None`
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithCoordinationAsync(System.Func<System.Threading.Tasks.Task<int>>, System.Threading.CancellationToken)`
  Target kind: `Member`
  Reason: `SeedContext`
  Role: `None`
- `CanDoItAll.Infrastructure.Persistence.SqliteWriteCoordination.GetWriteGate(string?)`
  Target kind: `Member`
  Reason: `RelatedContext`
  Role: `None`
- `CanDoItAll.Infrastructure.Persistence.SqliteWriteCoordination.GetRetryDelay(int)`
  Target kind: `Member`
  Reason: `RelatedContext`
  Role: `None`
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithCoordination(System.Func<int>)`
  Target kind: `Member`
  Reason: `Seed`
  Role: `None`
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithRetry(System.Func<int>)`
  Target kind: `Member`
  Reason: `SeedContext`
  Role: `None`
- `src/CanDoItAll.Infrastructure/BackgroundJobs/BackgroundJobs.cs`
  Target kind: `File`
  Reason: `Implementation`
  Role: `None`
- `src/CanDoItAll.Infrastructure/ControlPlane/DatabaseTransferModels.cs`
  Target kind: `File`
  Reason: `Implementation`
  Role: `None`
- `src/CanDoItAll.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs`
  Target kind: `File`
  Reason: `ServiceRegistration`
  Role: `Registration`
- `src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs`
  Target kind: `File`
  Reason: `Seed`
  Role: `None`
- `src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs`
  Target kind: `File`
  Reason: `SeedContext`
  Role: `None`
- `src/CanDoItAll.Infrastructure/Persistence/SqliteWriteCoordination.cs`
  Target kind: `File`
  Reason: `RelatedContext`
  Role: `None`
- `src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs`
  Target kind: `File`
  Reason: `Implementation`
  Role: `Factory`
- `src/CanDoItAll.Infrastructure/Search/SearchIndexing.cs`
  Target kind: `File`
  Reason: `Implementation`
  Role: `None`
- `src/CanDoItAll.Infrastructure/Storage/Persistence/StorageCatalogService.cs`
  Target kind: `File`
  Reason: `Implementation`
  Role: `None`

## Implementation Types

- None

## Selected Types

- `CanDoItAll.Infrastructure.Persistence.AppDbContext`
  Path: src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs
  Kind: `Class`
  Project: `proj-candoitall-infrastructure`
- `CanDoItAll.Infrastructure.Persistence.SqliteWriteCoordination`
  Path: src/CanDoItAll.Infrastructure/Persistence/SqliteWriteCoordination.cs
  Kind: `Class`
  Project: `proj-candoitall-infrastructure`
- `CanDoItAll.Infrastructure.Persistence.AppDbContextFactory`
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
- `CanDoItAll.Infrastructure.Persistence.AppDbContextOptionsConfigurator`
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs
  Kind: `Class`
  Project: `proj-candoitall-infrastructure`
- `CanDoItAll.Infrastructure.ControlPlane.DatabaseTransferContext`
  Path: src/CanDoItAll.Infrastructure/ControlPlane/DatabaseTransferModels.cs
  Kind: `Record`
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

- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithCoordination(System.Func<int>)`
  Type: `CanDoItAll.Infrastructure.Persistence.AppDbContext`
  Kind: `Method`
  Path: src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs
  Line: 85
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithCoordinationAsync(System.Func<System.Threading.Tasks.Task<int>>, System.Threading.CancellationToken)`
  Type: `CanDoItAll.Infrastructure.Persistence.AppDbContext`
  Kind: `Method`
  Path: src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs
  Line: 106
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithRetry(System.Func<int>)`
  Type: `CanDoItAll.Infrastructure.Persistence.AppDbContext`
  Kind: `Method`
  Path: src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs
  Line: 139
- `CanDoItAll.Infrastructure.Persistence.AppDbContext.ResolveSqliteWriteGate()`
  Type: `CanDoItAll.Infrastructure.Persistence.AppDbContext`
  Kind: `Method`
  Path: src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs
  Line: 129
- `CanDoItAll.Infrastructure.Persistence.SqliteWriteCoordination.GetRetryDelay(int)`
  Type: `CanDoItAll.Infrastructure.Persistence.SqliteWriteCoordination`
  Kind: `Method`
  Path: src/CanDoItAll.Infrastructure/Persistence/SqliteWriteCoordination.cs
  Line: 43
- `CanDoItAll.Infrastructure.Persistence.SqliteWriteCoordination.GetWriteGate(string?)`
  Type: `CanDoItAll.Infrastructure.Persistence.SqliteWriteCoordination`
  Kind: `Method`
  Path: src/CanDoItAll.Infrastructure/Persistence/SqliteWriteCoordination.cs
  Line: 66

## Usage Summary

- None

## File Excerpts

### src/CanDoItAll.Infrastructure/Persistence/AppDbContext.cs

- Total lines: 170
- Selected lines: 70
- Types: `CanDoItAll.Infrastructure.Persistence.AppDbContext`

#### CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithCoordination(System.Func<int>)

- Kind: `Method`
- Lines: 84-105

```csharp

    private int ExecuteSaveChangesWithCoordination(Func<int> saveChanges)
    {
        ArgumentNullException.ThrowIfNull(saveChanges);

        var writeGate = ResolveSqliteWriteGate();
        if (writeGate is null)
        {
            return ExecuteSaveChangesWithRetry(saveChanges);
        }

        writeGate.Wait();
        try
        {
            return ExecuteSaveChangesWithRetry(saveChanges);
        }
        finally
        {
            writeGate.Release();
        }
    }

```

#### CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithCoordinationAsync(System.Func<System.Threading.Tasks.Task<int>>, System.Threading.CancellationToken)

- Kind: `Method`
- Lines: 105-128

```csharp

    private async Task<int> ExecuteSaveChangesWithCoordinationAsync(
        Func<Task<int>> saveChanges,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(saveChanges);

        var writeGate = ResolveSqliteWriteGate();
        if (writeGate is null)
        {
            return await ExecuteSaveChangesWithRetryAsync(saveChanges, cancellationToken);
        }

        await writeGate.WaitAsync(cancellationToken);
        try
        {
            return await ExecuteSaveChangesWithRetryAsync(saveChanges, cancellationToken);
        }
        finally
        {
            writeGate.Release();
        }
    }

```

#### CanDoItAll.Infrastructure.Persistence.AppDbContext.ResolveSqliteWriteGate()

- Kind: `Method`
- Lines: 128-138

```csharp

    private SemaphoreSlim? ResolveSqliteWriteGate()
    {
        if (!Database.IsSqlite())
        {
            return null;
        }

        return SqliteWriteCoordination.GetWriteGate(Database.GetConnectionString());
    }

```

#### CanDoItAll.Infrastructure.Persistence.AppDbContext.ExecuteSaveChangesWithRetry(System.Func<int>)

- Kind: `Method`
- Lines: 138-153

```csharp

    private static int ExecuteSaveChangesWithRetry(Func<int> saveChanges)
    {
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                return saveChanges();
            }
            catch (Exception ex) when (SqliteWriteCoordination.IsBusy(ex) && attempt < SqliteWriteCoordination.RetryAttemptCount)
            {
                Thread.Sleep(SqliteWriteCoordination.GetRetryDelay(attempt));
            }
        }
    }

```

### src/CanDoItAll.Infrastructure/Storage/Persistence/StorageCatalogService.cs

- Total lines: 308
- Selected lines: 40
- Types: `CanDoItAll.Infrastructure.Storage.StorageCatalogService`

#### CanDoItAll.Infrastructure.Storage.StorageCatalogService.SaveAsync(CanDoItAll.Infrastructure.Storage.StorageCatalogRecord, System.Threading.CancellationToken)

- Kind: `Method`
- Lines: 95-134

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

- Total lines: 193
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
- Lines: 137-145

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
- Lines: 148-158

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

### src/CanDoItAll.Infrastructure/Persistence/SqliteWriteCoordination.cs

- Total lines: 158
- Selected lines: 20
- Types: `CanDoItAll.Infrastructure.Persistence.SqliteWriteCoordination`

#### CanDoItAll.Infrastructure.Persistence.SqliteWriteCoordination.GetRetryDelay(int)

- Kind: `Method`
- Lines: 42-52

```csharp

    public static TimeSpan GetRetryDelay(int attempt)
    {
        if ((uint)attempt >= (uint)BusyRetryDelays.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(attempt));
        }

        return BusyRetryDelays[attempt];
    }

```

#### CanDoItAll.Infrastructure.Persistence.SqliteWriteCoordination.GetWriteGate(string?)

- Kind: `Method`
- Lines: 65-73

```csharp

    public static SemaphoreSlim GetWriteGate(string? connectionString)
    {
        var key = string.IsNullOrWhiteSpace(connectionString)
            ? "__sqlite-default__"
            : NormalizeConnectionString(connectionString);
        return WriteGates.GetOrAdd(key, static _ => new SemaphoreSlim(1, 1));
    }

```

### src/CanDoItAll.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs

- Total lines: 109
- Selected lines: 11
- Types: None

#### CanDoItAll.Infrastructure.Persistence.ISwitchableAppDbContextFactory -> CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory

- Kind: `ServiceRegistrationFact`
- Lines: 82-91

```csharp
        services.AddSingleton<IDatabaseDriverRegistry, DatabaseDriverRegistry>();
        services.AddSingleton<ISwitchableAppDbContextFactory, SwitchableAppDbContextFactory>();
        services.AddSingleton<IDbContextFactory<AppDbContext>>(serviceProvider => serviceProvider.GetRequiredService<ISwitchableAppDbContextFactory>());
        services.AddSingleton<IWorkspacePathResolver, WorkspacePathResolver>();
        services.AddSingleton<IWorkspacePathAccessGuard, WorkspacePathAccessGuard>();
        services.AddSingleton<IStorageCatalogService, StorageCatalogService>();
        services.AddSingleton<IStorageDriver, FileSystemStorageDriver>();
        services.AddSingleton<IStorageDriver, IpfsStorageDriver>();
        services.AddSingleton<IStorageDriver, FtpStorageDriver>();
        services.AddSingleton<IStorageDriverRegistry, StorageDriverRegistry>();
```

#### Microsoft.EntityFrameworkCore.IDbContextFactory<CanDoItAll.Infrastructure.Persistence.AppDbContext>

- Kind: `ServiceRegistrationFact`
- Lines: 83-92

```csharp
        services.AddSingleton<ISwitchableAppDbContextFactory, SwitchableAppDbContextFactory>();
        services.AddSingleton<IDbContextFactory<AppDbContext>>(serviceProvider => serviceProvider.GetRequiredService<ISwitchableAppDbContextFactory>());
        services.AddSingleton<IWorkspacePathResolver, WorkspacePathResolver>();
        services.AddSingleton<IWorkspacePathAccessGuard, WorkspacePathAccessGuard>();
        services.AddSingleton<IStorageCatalogService, StorageCatalogService>();
        services.AddSingleton<IStorageDriver, FileSystemStorageDriver>();
        services.AddSingleton<IStorageDriver, IpfsStorageDriver>();
        services.AddSingleton<IStorageDriver, FtpStorageDriver>();
        services.AddSingleton<IStorageDriverRegistry, StorageDriverRegistry>();
        services.AddSingleton<IStorageRoutingService, DefaultStorageRoutingService>();
```

### src/CanDoItAll.Infrastructure/ControlPlane/DatabaseTransferModels.cs

- Total lines: 91
- Selected lines: 3
- Types: `CanDoItAll.Infrastructure.ControlPlane.DatabaseTransferContext`

#### CanDoItAll.Infrastructure.ControlPlane.DatabaseTransferContext.SourceProfile

- Kind: `Property`
- Lines: 57-59

```csharp
public sealed record DatabaseTransferContext(
    ResolvedDatabaseProfile SourceProfile,
    ResolvedDatabaseProfile TargetProfile,
```


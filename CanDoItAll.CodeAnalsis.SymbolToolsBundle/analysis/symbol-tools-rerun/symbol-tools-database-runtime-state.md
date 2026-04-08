# Database runtime switching scenario

## Query

- Query text: `IDatabaseRuntimeState`
- Elapsed milliseconds: 1978
- Search results: 1
- Selected symbol: `CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState`
- Target kind: `Type`

## Definition

- Declaration: `interface CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState`
- Path: src/CanDoItAll.Infrastructure/Persistence/DatabaseRuntimeSwitching.cs:26
- Truncated: False

```csharp
public interface IDatabaseRuntimeState
{
    DatabaseRuntimeSnapshot GetSnapshot();

    Task<DatabaseContextLease> AcquireContextLeaseAsync(CancellationToken cancellationToken = default);

    Task<DatabaseSwitchSession> BeginSwitchAsync(CancellationToken cancellationToken = default);

    void MarkCurrentProfile(ResolvedDatabaseProfile profile);
}
```

## Members

- Member count: 4
- `CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState.AcquireContextLeaseAsync(System.Threading.CancellationToken)` (Method)
- `CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState.BeginSwitchAsync(System.Threading.CancellationToken)` (Method)
- `CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState.GetSnapshot()` (Method)
- `CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState.MarkCurrentProfile(CanDoItAll.Infrastructure.ControlPlane.ResolvedDatabaseProfile)` (Method)

## Implementations

- Count: 1
- `CanDoItAll.Infrastructure.Persistence.DatabaseRuntimeState` (InterfaceImplementation)

## References

- Total references: 9
- Returned references: 9
- `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory` :: `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory.CreateDbContextAsync(System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs:118
- `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory` :: `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory.CreateDbContextAsync(System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs:123
- `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory` :: `CanDoItAll.Infrastructure.Persistence.SwitchableAppDbContextFactory.SwitchableAppDbContextFactory(CanDoItAll.Infrastructure.ControlPlane.IDatabaseProfileRuntimeAccessor, CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState)` (ConstructorParameter)
  Path: src/CanDoItAll.Infrastructure/Persistence/SwitchableAppDbContextFactory.cs:109
- `CanDoItAll.Infrastructure.DependencyInjection.InfrastructureServiceCollectionExtensions` :: `CanDoItAll.Infrastructure.DependencyInjection.InfrastructureServiceCollectionExtensions.AddCanDoItAllInfrastructure(Microsoft.Extensions.DependencyInjection.IServiceCollection, Microsoft.Extensions.Configuration.IConfiguration, Microsoft.Extensions.Hosting.IHostEnvironment, System.Collections.Generic.IReadOnlyList<System.Reflection.Assembly>)` (ServiceRegistration)
  Path: src/CanDoItAll.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs:76
- `CanDoItAll.Tests.Integration.DatabaseSwitchIntegrationTests` :: `CanDoItAll.Tests.Integration.DatabaseSwitchIntegrationTests.SwitchAsync_changes_active_data_source_without_restarting_the_process()` (Invocation)
  Path: tests/CanDoItAll.Tests.Integration/DatabaseRuntimeSwitchingIntegrationTests.cs:99
- `CanDoItAll.Tests.Unit.AppDbContextRuntimeSwitchTests` :: `CanDoItAll.Tests.Unit.AppDbContextRuntimeSwitchTests.CreateDbContextAsync_uses_the_new_active_profile_after_a_switch()` (Invocation)
  Path: tests/CanDoItAll.Tests.Unit/DatabaseRuntimeSwitchingTests.cs:114
- `CanDoItAll.Web.Infrastructure.DatabaseSwitchCoordinator` :: `CanDoItAll.Web.Infrastructure.DatabaseSwitchCoordinator.SwitchAsync(System.Guid, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Web/Infrastructure/RuntimeDatabaseSwitching.cs:70
- `CanDoItAll.Web.Infrastructure.DatabaseSwitchCoordinator` :: `CanDoItAll.Web.Infrastructure.DatabaseSwitchCoordinator.SwitchAsync(System.Guid, System.Threading.CancellationToken)` (Invocation)
  Path: src/CanDoItAll.Web/Infrastructure/RuntimeDatabaseSwitching.cs:88
- `CanDoItAll.Web.Infrastructure.DatabaseSwitchCoordinator` :: `CanDoItAll.Web.Infrastructure.DatabaseSwitchCoordinator.DatabaseSwitchCoordinator(CanDoItAll.Infrastructure.ControlPlane.IDatabaseProfileRuntimeAccessor, CanDoItAll.Infrastructure.ControlPlane.IDatabaseProfileService, CanDoItAll.Infrastructure.ControlPlane.IDatabaseDriverRegistry, CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState, CanDoItAll.Infrastructure.ControlPlane.IAppDatabaseBootstrapper, Microsoft.Extensions.Logging.ILogger<CanDoItAll.Web.Infrastructure.DatabaseSwitchCoordinator>)` (ConstructorParameter)
  Path: src/CanDoItAll.Web/Infrastructure/RuntimeDatabaseSwitching.cs:53

# Common helper scenario

## Query

- Query text: `IClock`
- Warm elapsed milliseconds: 26909
- Warm call count: 4

## Search sequence

1. `SearchDefinitions("\\bIClock\\b")`
   Result: mixed output with constructor-injection usages across infrastructure and modules, plus the exact interface and `SystemClock` in `src/CanDoItAll.SharedKernel/IClock.cs`.
2. `ViewDefinition("CanDoItAll.SharedKernel.IClock")`
3. `ListImplementations("CanDoItAll.SharedKernel.IClock")`
4. `FindReferences("CanDoItAll.SharedKernel.IClock")`

## Definition

```csharp
public interface IClock
{
    DateTimeOffset GetUtcNow();
}
```

`ViewDefinition` also surfaced a short referencing-type list focused on DI registration and tests.

## Implementations

`ListImplementations` returned four implementations:

- `CanDoItAll.SharedKernel.SystemClock`
- `CanDoItAll.Tests.Unit.WorkbenchStateServiceTests.TestClock`
- `CanDoItAll.Tests.Unit.StorageCatalogServiceTests.TestClock`
- `CanDoItAll.Tests.Integration.AutomationRuntimeIntegrationTests.ThrowOnceArmedClock`

## References

`FindReferences` reported `55` total references with `20` displayed in the first response. Representative displayed references:

- `CanDoItAll.Infrastructure.DependencyInjection.InfrastructureServiceCollectionExtensions.AddCanDoItAllInfrastructure(...)`
  Context: `services.AddSingleton<IClock, SystemClock>();`
- `CanDoItAll.Infrastructure.BackgroundJobs.BackgroundJobTracker`
  Context: constructor injection of `IClock`
- `CanDoItAll.Infrastructure.Storage.StorageCatalogService`
  Context: constructor injection of `IClock`
- `CanDoItAll.Modules.Activity.ActivityService`
  Context: constructor injection of `IClock`
- `CanDoItAll.Modules.Automation.AutomationMessagePublisher`
  Context: constructor injection of `IClock`
- `CanDoItAll.Modules.Automation.AutomationMessageDispatcher`
  Context: constructor injection of `IClock`
- `CanDoItAll.Modules.Projects.ProjectsService`
  Context: constructor injection of `IClock`
- `CanDoItAll.Modules.Security.SecretService`
  Context: constructor injection of `IClock`
- `CanDoItAll.Modules.Workspace.ConnectorCommandProcessor`
  Context: constructor injection of `IClock`
- `CanDoItAll.Modules.Workspace.WorkspaceService`
  Context: constructor injection of `IClock`
- `CanDoItAll.SharedKernel.SystemClock`
  Context: direct implementation of `IClock`

## Working impression

This is much more surgical than the focused-context helper output. SharpTools splits the problem into contract, implementations, and usages instead of merging them. The first pass still contains many usage hits, but the interface and DI registration are obvious, and the agent can stop early without carrying large consumer excerpts.

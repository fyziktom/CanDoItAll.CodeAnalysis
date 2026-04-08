# ComparisonHarness

Tracked rerun harness for host-side validation scenarios.

## Focused-context mode

Scenarios:

- `AppDbContext`
- `IClock`
- `CanvasSceneHost`

Run it with:

```powershell
dotnet run --project C:\repositories\CanDoItAll.CodeAnalsis\tools\ComparisonHarness\ComparisonHarness.csproj -- `
  C:\repositories\CanDoItAll\CanDoItAll.slnx `
  C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context `
  C:\repositories\CanDoItAll.CodeAnalsis\output\ComparisonHarnessData
```

## Symbol-tools mode

Scenarios:

- `AppDbContext`
- `IClock`
- `CanvasSceneHost`
- `IStorageDriverRegistry`
- `IDatabaseRuntimeState`

Run it with:

```powershell
dotnet run --project C:\repositories\CanDoItAll.CodeAnalsis\tools\ComparisonHarness\ComparisonHarness.csproj -- `
  symbol-tools `
  C:\repositories\CanDoItAll\CanDoItAll.slnx `
  C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SymbolToolsBundle\analysis\symbol-tools-rerun `
  C:\repositories\CanDoItAll.CodeAnalsis\output\ComparisonHarnessData
```

Arguments:

1. Optional mode. Supported values: `focused-context`, `symbol-tools`.
2. Host solution path.
3. Output directory for markdown and json artifacts.
4. Snapshot output root used by the code-analysis application service.

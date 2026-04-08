# ComparisonHarness

Tracked focused-context rerun harness for the three standing host validation scenarios:

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

Arguments:

1. Host solution path.
2. Output directory for markdown and json artifacts.
3. Snapshot output root used by the code-analysis application service.

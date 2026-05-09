# Relation-hinted focused context

## Summary

SB-28 adds relation hints as an explicit focused-context input. Tags continue to describe the architectural area, while relation hints name related functions, classes, components, namespaces, projects, or paths that should bias the walk.

The implementation intentionally does not turn relation hints into a silent fallback. For high-fan-in helper usage summaries, unmatched relation hints suppress broad unrelated representative consumers instead of returning the same wide helper sample.

## Delivered Scope

- `FocusedContextQuery` and `FocusedContextResponse` now carry normalized relation hints.
- `EntityFramework`, `EFCore`, `database`, `razor`, and `component` are accepted as practical focus-tag aliases.
- Helper representative-consumer clustering scores relation hints against member names, containing types, source paths, project names, and module names.
- The focused-context lab accepts `relationHints` from the route, renders the input, and displays normalized relation-hint chips in the run summary.
- The host `CanDoItAll.Mcp.CodeAnalytics` input model and coordinator pass relation hints into the standalone engine.
- The CodeAnalytics MCP skill in the host repo now tells agents to prefer scoped snapshots, exact symbol lookups, and relation-hinted focused context before broad solution scans.
- The comparison harness records relation hints and includes a host `IClock` plus `Workbench` scenario.

## Quantified Harness Result

Harness command:

```powershell
dotnet run --project C:\repositories\CanDoItAll.CodeAnalsis\tools\ComparisonHarness\ComparisonHarness.csproj -- C:\repositories\CanDoItAll\CanDoItAll.slnx C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\relation-hinted-focused-context C:\repositories\CanDoItAll.CodeAnalsis\output\ComparisonHarnessRelationData
```

Snapshot setup:

- Snapshot: `snap-20260509220818-123ebd81`
- Projects: 67
- Types: 3731
- Members: 31045
- Snapshot elapsed: 100260 ms

Helper comparison:

| Scenario | Files | Blocks | Selected lines | Usage clusters | Callers | Characters | Est. tokens | Elapsed |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `IClock` | 1 | 2 | 6 | 20 | 167 | 4724 | 1181 | 1422 ms |
| `IClock` plus `Workbench` relation hint | 1 | 2 | 6 | 1 | 42 | 3283 | 821 | 1302 ms |

Observed savings:

- Usage clusters: 20 to 1, a 95% reduction.
- Callers shown: 167 to 42, a 74.9% reduction.
- Estimated tokens: 1181 to 821, a 30.5% reduction.
- Selected source excerpt stayed at 6 lines, so the relation hint narrowed supporting usage context without dropping the helper definition context.

The current host repo has grown since the April comparison baseline, so the broader `AppDbContext` count in this run is not a direct regression signal. The relation-hint comparison is intentionally measured within the same 2026-05-09 snapshot.

## Validation

- `dotnet build C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx -nologo`: passed, 0 warnings, 0 errors.
- `dotnet test C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx --no-build -nologo`: passed, 70 tests.
- `dotnet build C:\repositories\CanDoItAll\src\CanDoItAll.Mcp.CodeAnalytics\CanDoItAll.Mcp.CodeAnalytics.csproj -nologo`: passed, 0 warnings, 0 errors.
- Comparison harness: passed and wrote artifacts to `analysis\relation-hinted-focused-context`.
- Browser proof: passed on `/context-lab` with `tags=EntityFramework`, `relationHints=OrderService`, and `depth=2` at `1440x1200` and `390x1000`.

## Browser Artifacts

- `C:\repositories\CanDoItAll.CodeAnalsis\output\playwright\codeanalytics-contextlab-relation-hints.png`
- `C:\repositories\CanDoItAll.CodeAnalsis\output\playwright\codeanalytics-contextlab-relation-hints-mobile.png`

## Remaining Risks

- Relation hints are explicit string inputs, not a semantic ontology. Agents still need skill guidance to choose good hints.
- The `AppDbContext` trouble-path scenario remains broader than ideal on the current host solution. That is a ranking-tuning problem separate from the relation-hinted helper improvement.
- SharpTools-style exact-source navigation still remains valuable for direct definition and reference inspection. Focused context is now stronger as the first bounded orientation call, not as a replacement for every exact-symbol drill-down.

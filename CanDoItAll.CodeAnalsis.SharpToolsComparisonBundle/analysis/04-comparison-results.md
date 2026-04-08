# Comparison results

## Setup comparison

| Approach | Setup calls | Setup time | Notes |
| --- | --- | --- | --- |
| Focused-context | 1 | 66211 ms | Builds a full snapshot of `C:\repositories\CanDoItAll\CanDoItAll.slnx` |
| SharpTools | 1 | 33128 ms | Loads the solution index for symbol navigation |

The focused-context setup cost is roughly double the SharpTools setup cost. That is the main startup disadvantage of the focused-context approach.

## Scenario matrix

| Scenario | Approach | Warm calls | Warm time | Artifact chars | Est. tokens | Helpfulness | Noise | Verdict |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `AppDbContext` | Focused-context | 1 | 850 ms | 11138 | 2785 | High | Medium | Better first-pass bundle |
| `AppDbContext` | SharpTools | 5 | 41394 ms | 4147 | 1037 | Medium | Medium | Better exact-symbol drill-down |
| `IClock` | Focused-context | 1 | 469 ms | 15945 | 3987 | Medium | High | Too much consumer code remains |
| `IClock` | SharpTools | 4 | 26909 ms | 2717 | 680 | High | Low | Much more surgical |
| `CanvasSceneHost` | Focused-context | 1 | 607 ms | 6782 | 1696 | High | Low | Strong first-pass bundle |
| `CanvasSceneHost` | SharpTools | 4 | 25376 ms | 2228 | 557 | High | Low | Cleanest exact view |

## Cross-scenario totals

| Approach | Total calls including setup | Total time including setup | Warm-only calls | Warm-only time | Total artifact chars | Total est. tokens |
| --- | --- | --- | --- | --- | --- | --- |
| Focused-context | 4 | 68137 ms | 3 | 1926 ms | 33865 | 8468 |
| SharpTools | 14 | 126807 ms | 13 | 93679 ms | 9092 | 2274 |

## One-off versus amortized cost

- One-off `AppDbContext` investigation: focused-context `67061 ms`, SharpTools `74522 ms`
- One-off `IClock` investigation: focused-context `66680 ms`, SharpTools `60037 ms`
- One-off `CanvasSceneHost` investigation: focused-context `66818 ms`, SharpTools `58504 ms`

If the operator only needs one small UI or helper lookup, SharpTools can still win on elapsed time because its setup is lighter. Once the snapshot already exists or multiple scenarios are queried, focused-context becomes dramatically faster.

## Detailed findings

### Database scenario: `AppDbContext`

- Focused-context was more helpful as a first-pass work surface. It bundled the `AppDbContext` body, the switchable factory surface, and concrete consumers such as `StorageCatalogService` and `SearchIndexService` without additional calls.
- The downside is that the bundle still includes some peripheral consumer code. The result is useful, but not perfectly ranked.
- SharpTools surfaced the exact type precisely after one refinement search, then exposed a very broad reference set with `288` total references. That is powerful, but it pushes ranking work back onto the agent.

Verdict: focused-context is the better default first step for infrastructure or database trouble paths; SharpTools is the better second step when the next exact symbol is already known.

### Common helper scenario: `IClock`

- Focused-context correctly resolved to `Definition` + `Surgical`, which is a real improvement over the older broad helper behavior.
- Even so, the result still carried `209` selected lines and large consumer excerpts from `CrmHrServices.cs`, `AutomationMessagingServices.cs`, and `ProjectWorkbenchCrossModuleMutations.cs`. That is too much payload for a tiny helper contract.
- SharpTools was materially better here. It split the problem into definition, implementations, and references, and the agent could stop after seeing the DI registration plus a few representative usages.

Verdict: SharpTools is clearly better for high-fan-in helpers. Focused-context still needs another reduction step in helper definition mode.

### UI scenario: `CanvasSceneHost`

- Focused-context produced a compact and directly useful bundle. It included the host behavior, the preview factory, the serialization helper, and the workbench surface property that participates in the lifecycle.
- SharpTools was even cleaner because the type is small and has only one direct reference. The definition plus one reference already answers most questions.
- The practical difference is not correctness but packaging style: focused-context gives a ready-made mini cluster, while SharpTools gives the exact type and lets the agent decide whether to fan out.

Verdict: both are good. SharpTools is cleaner, but focused-context is already good enough to justify itself as a one-call UI work surface.

## Efficiency interpretation

- Warm scenario latency strongly favors focused-context once setup is amortized. SharpTools was `48.7x` slower for `AppDbContext`, `57.4x` slower for `IClock`, and `41.8x` slower for `CanvasSceneHost`.
- Token cost strongly favors SharpTools in all three normalized artifacts. Focused-context carried `2.69x` more tokens for `AppDbContext`, `5.86x` more for `IClock`, and `3.04x` more for `CanvasSceneHost`.
- The token difference is not automatically bad. In the database and UI scenarios, some of that extra payload is genuinely useful because it removes follow-up discovery calls.
- The helper case is different. There the extra payload is mostly noise, not useful acceleration.

## Bottom line

- Focused-context already wins the operational-efficiency argument when the snapshot is reused and the scenario is a trouble path or small UI workflow.
- SharpTools still wins the precision argument for ubiquitous helpers and exact symbol drill-down.
- The next focused-context improvements should target helper payload suppression and reference-role ranking, not generic scoring churn.

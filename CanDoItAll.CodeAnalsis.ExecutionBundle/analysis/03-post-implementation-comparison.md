# Post-implementation comparison

## Scope

- Revalidated the shipped implementation after the comparison-driven improvements landed.
- Reused the same three host scenarios from the closed SharpTools comparison bundle:
  - `AppDbContext`
  - `IClock`
  - `CanvasSceneHost`
- Used the tracked rerun entry point:
  - `dotnet run --project C:\repositories\CanDoItAll.CodeAnalsis\tools\ComparisonHarness\ComparisonHarness.csproj -- C:\repositories\CanDoItAll\CanDoItAll.slnx C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context C:\repositories\CanDoItAll.CodeAnalsis\output\ComparisonHarnessData`

## Current focused-context artifacts

- Summary: [focused-context-summary.json](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context\focused-context-summary.json)
- Database scenario: [focused-context-app-db-context.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context\focused-context-app-db-context.md)
- Helper scenario: [focused-context-i-clock.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context\focused-context-i-clock.md)
- UI scenario: [focused-context-canvas-scene-host.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context\focused-context-canvas-scene-host.md)
- SharpTools baseline remained the earlier comparison bundle because the target host repo under `C:\repositories\CanDoItAll` was not modified in this implementation pass:
  - [04-comparison-results.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle\analysis\04-comparison-results.md)
  - [sharptools-app-db-context.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle\analysis\sharptools\sharptools-app-db-context.md)
  - [sharptools-i-clock.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle\analysis\sharptools\sharptools-i-clock.md)
  - [sharptools-canvas-scene-host.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle\analysis\sharptools\sharptools-canvas-scene-host.md)

## Before vs after focused-context

| Scenario | Before | After | Result |
| --- | --- | --- | --- |
| `AppDbContext` | `139 lines / 5 files / 8 blocks / 2785 tokens` | `150 lines / 6 files / 10 blocks / 3635 tokens` | Slightly broader, but now includes explicit DI registration and factory context instead of only consumer-heavy slices |
| `IClock` | `209 lines / 4 files / 6 blocks / 3987 tokens` | `6 lines / 1 file / 2 blocks / 1084 tokens` | Large improvement; definition mode is now genuinely surgical |
| `CanvasSceneHost` | `59 lines / 3 files / 6 blocks / 1696 tokens` | `59 lines / 3 files / 6 blocks / 2096 tokens` | Structural payload stayed stable; token increase comes from added selection-reason metadata in the rendered artifact |

## Aggregate before vs after

- Setup cost changed from `66211 ms` to `76533 ms`.
- Warm scenario time changed from `1926 ms` to `2389 ms`.
- Aggregate focused-context artifact size changed from `8468` to `6815` estimated tokens.
- The helper reduction more than offset the new selection-reason metadata cost.

## Standing versus SharpTools

### `AppDbContext`

- SharpTools still wins exact-symbol drill-down with a cleaner definition-first view.
- Focused-context remains the better first-pass work surface because it now includes:
  - the `AppDbContext` body,
  - `SwitchableAppDbContextFactory` and related factory surface,
  - the DI registration block from `InfrastructureServiceCollectionExtensions`,
  - representative database consumers.
- Residual issue: the result is now more intentional but also slightly broader than the previous focused-context pass.

### `IClock`

- This is the main success case of the reopen.
- SharpTools still wins absolute minimalism, but the gap is now small enough that focused-context is a credible first pass:
  - `1` focused-context call vs `4` SharpTools calls,
  - `1084` estimated focused-context tokens vs `680` SharpTools baseline tokens,
  - `6` selected code lines instead of the old `209`.
- The remaining breadth is pushed into usage-summary clusters instead of consumer excerpts, which is the intended behavior.

### `CanvasSceneHost`

- The UI case remained structurally stable.
- SharpTools is still the cleanest exact symbol view.
- Focused-context remains valid because one query still returns the relevant mini-cluster without additional probing.

## Quality judgement

- The new helper mode is truly helpful, not just smaller:
  - the contract and implementation stay together,
  - breadth is preserved through cluster counts and representative samples,
  - noisy consumer code is no longer mixed into the main excerpt set.
- The database case is more explainable because the result now shows why factory and registration files were selected.
- The UI case preserved its previous usefulness and did not regress under the new scoring changes.

## Residual risks

- `AppDbContext` is now slightly broader in structural payload than the previous focused-context pass. The added DI and factory evidence is valuable, but the database ranking still needs one more tightening pass if the goal is to beat the previous line count without losing the new intentionality.
- Current artifact token counts include the new selection-reason metadata. That is honest for the real payload, but it means token improvements should be interpreted alongside structural metrics, not alone.
- Role classification is now narrower and avoids obvious false positives like generic `Add*` or `Create*` business methods, but the heuristics are still name-based and therefore should remain covered by the standing comparison set.

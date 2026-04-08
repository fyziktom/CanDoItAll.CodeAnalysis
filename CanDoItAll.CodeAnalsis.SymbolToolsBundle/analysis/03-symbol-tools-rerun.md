# Symbol tools rerun

## Scenario set

- Original scenarios:
  - `AppDbContext`
  - `IClock`
  - `CanvasSceneHost`
- Additional scenarios:
  - `IStorageDriverRegistry`
  - `IDatabaseRuntimeState`

This widened the validation mix across database infrastructure, ubiquitous helpers, UI state objects, storage contracts, and runtime-switching contracts.

## Setup

- Solution: `C:\repositories\CanDoItAll\CanDoItAll.slnx`
- Snapshot build calls: `1`
- Snapshot build time: `65444 ms`
- Snapshot facts: `40 projects`, `2083 types`, `14770 members`

## Warm scenario metrics

| Scenario | Search results | Warm calls | Warm time | Artifact chars | Est. tokens | Members | Implementations | References returned / total |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `AppDbContext` | `2` | `5` | `2180 ms` | `6046` | `1512` | `6` | `0` | `40 / 110` |
| `IClock` | `1` | `5` | `2195 ms` | `4997` | `1250` | `1` | `4` | `40 / 160` |
| `CanvasSceneHost` | `1` | `5` | `2087 ms` | `3373` | `844` | `9` | `0` | `4 / 4` |
| `IStorageDriverRegistry` | `1` | `5` | `2262 ms` | `6559` | `1640` | `3` | `4` | `15 / 15` |
| `IDatabaseRuntimeState` | `1` | `5` | `1977 ms` | `4954` | `1239` | `4` | `1` | `9 / 9` |

## Scenario notes

### AppDbContext

- The new symbol search is precise enough to reduce the starting set to two candidates.
- The definition, members, and references are useful for exact-symbol drill-down.
- Noise is moderate because the type still has `110` references and the route returns the top `40`.

### IClock

- The definition and implementation sections are correct and useful.
- The remaining weakness is type-level reference breadth. The route still treats every `GetUtcNow` consumer as part of the same symbol investigation, which is broader than a contract-first lookup usually needs.

### CanvasSceneHost

- This is a clean parity case. The search result is exact, the member list is complete, and the single reference closes the loop.

### IStorageDriverRegistry

- The route is strong here because the definition, implementations, and references align around a real contract seam.
- Explicit reference roles make the output easier to rank than a raw reference dump.

### IDatabaseRuntimeState

- This is near-ideal for the new route.
- The symbol has one production implementation and a naturally bounded reference set, so the result is compact and helpful.

## Rerun judgment

- The new symbol-tool surface generalizes beyond the original three scenarios.
- Search precision is now consistently strong.
- Contract and infrastructure drill-down is materially better than before this bundle.
- The standing weakness is helper reference minimalism, not missing capability.

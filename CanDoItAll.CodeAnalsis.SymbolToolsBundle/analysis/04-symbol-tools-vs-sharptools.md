# Symbol tools versus SharpTools

## Setup comparison

| Approach | Setup calls | Setup time | Notes |
| --- | --- | --- | --- |
| Symbol tools | `1` | `65444 ms` | Builds a snapshot once, then serves the exact-symbol route from snapshot facts |
| SharpTools | `2` | `59200 ms` | Loads the host solution index, then loads project structure for context |

## Scenario matrix

| Scenario | Approach | Warm calls | Warm time | Search spread | Normalized artifact chars | Est. tokens | Helpfulness | Noise | Verdict |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `AppDbContext` | Symbol tools | `5` | `2180 ms` | `2` search results | `6046` | `1512` | High | Medium | Better exact start, weaker raw breadth |
| `AppDbContext` | SharpTools | `5` | `58813 ms` | `20` search matches | `1210` | `303` | High | Medium | Better when the agent wants raw reference breadth |
| `IClock` | Symbol tools | `5` | `2195 ms` | `1` search result | `4997` | `1250` | Medium | High | Capability is present but still too broad |
| `IClock` | SharpTools | `5` | `56573 ms` | `20` search matches | `1127` | `282` | High | Low | Still the more surgical helper flow |
| `CanvasSceneHost` | Symbol tools | `5` | `2087 ms` | `1` search result | `3373` | `844` | High | Low | Practical parity |
| `CanvasSceneHost` | SharpTools | `5` | `52096 ms` | `2` search matches | `972` | `243` | High | Low | Practical parity |
| `IStorageDriverRegistry` | Symbol tools | `5` | `2262 ms` | `1` search result | `6559` | `1640` | High | Low | Better packaged contract view |
| `IStorageDriverRegistry` | SharpTools | `5` | `57082 ms` | `15` search matches | `1286` | `322` | High | Medium | Strong but more manual |
| `IDatabaseRuntimeState` | Symbol tools | `5` | `1977 ms` | `1` search result | `4954` | `1239` | High | Low | Slight edge on precision |
| `IDatabaseRuntimeState` | SharpTools | `5` | `54068 ms` | `7` search matches | `1067` | `267` | High | Low | Near parity |

## Cross-scenario totals

| Approach | Total calls including setup | Total time including setup | Warm-only time | Normalized artifact chars | Est. tokens | Search spread | References returned / total |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Symbol tools | `26` | `76145 ms` | `10701 ms` | `25929` | `6485` | `6` | `108 / 298` |
| SharpTools | `27` | `337832 ms` | `278632 ms` | `5662` | `1417` | `64` | `61 / 364` |

## Interpretation

- Warm execution strongly favors the new symbol tools. Across the five scenarios, the snapshot-backed route was about `26x` faster than the SharpTools call chain once setup was complete.
- Search precision also favors the new symbol tools. Across the same scenario set, the exact-symbol route produced `6` candidate results total, while the SharpTools regex searches surfaced `64` matches that still required manual ranking.
- SharpTools still carries less normalized payload because the workflow remains multi-step and more surgical. That is especially valuable for ubiquitous helpers.
- The storage and runtime-state scenarios show the biggest parity gain from this bundle. The new route now provides a credible first-pass contract drill-down instead of forcing the agent into focused-context or raw file navigation first.

## Bottom line

- The missing SharpTools-style information path is now present in the product.
- Search, definition viewing, member listing, implementation discovery, and reference tracing all work on the original scenarios plus two additional ones.
- The main remaining gap is not missing capability. It is helper-reference shaping:
  - `IClock` still carries too much invocation noise,
  - production-versus-test filtering would further improve implementations and references for shared contracts.

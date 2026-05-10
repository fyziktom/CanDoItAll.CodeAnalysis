# Twenty-scenario real-world evaluation

## Scope

SB-29 ran 22 simulated agent prompts against three read-only target repositories:

- `C:\repositories\MBusParser`
- `C:\repositories\influxdb-client-csharp`
- `C:\repositories\CanDoItAll`

The scenario set contains 6 first-step introduction prompts and 16 specific repair or implementation prompts. Every scenario records the simulated prompt, the intended agent approach, query text, tags, relation hints, depth, intent, precision, expected relevant terms, expected file fragments, and noise terms.

External repository status was checked before and after the runs. `influxdb-client-csharp` stayed clean. `MBusParser` and `CanDoItAll` had pre-existing dirty files, and those dirty sets did not expand during this work.

## Artifacts

- Baseline run: `analysis\twenty-scenario-evaluation\baseline`
- After-change run: `analysis\twenty-scenario-evaluation\after`
- Before/after comparison: `analysis\twenty-scenario-evaluation\before-after-comparison.md`
- Comparison JSON: `analysis\twenty-scenario-evaluation\before-after-comparison.json`

## Baseline Findings

Baseline aggregate:

- Scenarios: 22
- Introduction scenarios: 6
- Average helpfulness: 0.434
- Average expected-term coverage: 0.394
- Average expected-file coverage: 0.561
- Average non-useful file ratio: 0.712
- Ratings: 9 good, 3 mixed, 1 poor, 9 failed

The dominant failure was not ranking. `influxdb-client-csharp` loaded as an empty snapshot: 0 projects, 0 types, and 0 members. The snapshot diagnostic showed the real cause:

```text
An item with the same key has already been added. Key: C:\repositories\influxdb-client-csharp\Client\Client.csproj
```

That turned 9 Influx scenarios into null focused-context outputs. This is a real-world loader defect, not a scenario-quality problem.

Secondary findings:

- Exact type prompts such as `WriteApi` could seed from a property or nearby type instead of the exact class.
- Exact type prompts such as `QueryApi` could seed from `QueryApiSync`, then carry sync-specific context into async/query scenarios.
- Scenario tags such as `Protocol`, `Parser`, `Crypto`, `Write`, `Query`, `Client`, and `Linq` were too weak because the focus-tag vocabulary only covered a smaller application-style set.
- The `mbus-enum-utils-dif` prompt expected DIF/VIF usages that do not exist in the current target repo. CodeAnalytics correctly found actual usages in headers and parser code; the scenario remains poor because the simulated prompt assumption was wrong.

## Implemented Improvements

1. Workspace loader deduplicates Roslyn projects by full project path before building project dictionaries.
2. Focus-tag vocabulary now includes practical tags for protocol parsing, crypto, client APIs, LINQ, query, write paths, model records, and tests.
3. Focused-context seed resolution now keeps exact type-name prompts anchored to exact type candidates instead of letting members from other types steal the seed.
4. Seed-member scoring now scores focus tags against the member signature itself after the type has already been selected, instead of giving every member the same score from the containing type path.

## After-change Results

After-change aggregate:

- Scenarios: 22
- Introduction scenarios: 6
- Average helpfulness: 0.714
- Average expected-term coverage: 0.701
- Average expected-file coverage: 0.902
- Average non-useful file ratio: 0.552
- Ratings: 12 good, 9 mixed, 1 poor, 0 failed

Before/after deltas:

- Failed scenarios: 9 to 0
- Average helpfulness: +0.280
- Average expected-term coverage: +0.307
- Average expected-file coverage: +0.341
- Average non-useful file ratio: -0.160
- Improved scenarios: 9
- Regressed scenarios: 0

Token totals increased from 21,877 to 38,012 because the 9 previously empty Influx scenarios now return real context. This is expected and should not be read as context regression. The after-change token budget ratio improved from 0.771 to 0.643 because the recovered outputs are mostly within their per-scenario budgets.

## Scenario Coverage

Introduction scenarios:

- `mbus-intro-parser`
- `mbus-intro-decryption`
- `mbus-intro-record-model`
- `influx-intro-write-flow`
- `influx-intro-query-flow`
- `cando-intro-canvas`

Specific scenarios covered parser repair, enum helper usage, crypto, protocol lookup, VIF extension handling, write retry behavior, async write handling, point escaping, query cancellation, delete predicate formatting, client options flow, LINQ provider behavior, EF save coordination, high-fan-in helper relation walking, storage registry behavior, and canvas dirty-state handling.

## Remaining Gaps

- `mbus-enum-utils-dif` remains poor because the prompt assumes EnumUtils is used by DIF/VIF fields, but actual usage is in `MBusHeader`, `Configuration`, and parser code. The right agent behavior is to follow with exact references before insisting on the user-supplied relation.
- `influx-query-cancel` improved from failed to mixed, but it still carries non-useful files around `QueryApi`, sync query, and generated API domain/service code. A future tuning pass should make relation hints stronger for selected file inclusion when the hint names test or parser context.
- Introduction scenarios with `Outline` precision sometimes return useful symbol/type orientation without file excerpts. That is acceptable for low-cost first orientation, but the lab and skill should make clear when to follow up with `Surgical` or exact definition calls.

## Validation

- `dotnet build C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx -nologo`: passed, 0 warnings, 0 errors.
- `dotnet test C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx --no-build -nologo`: passed, 70 tests.
- Prepared-stage bundle validation passed before implementation.
- Completed-stage bundle validation is recorded in the execution report.

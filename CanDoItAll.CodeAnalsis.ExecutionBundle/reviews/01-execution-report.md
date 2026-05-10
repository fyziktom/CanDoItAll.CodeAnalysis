# Execution report

## Status

- The comparison-driven reopen completed on 2026-04-08.
- This pass implemented:
  - helper definition-mode payload suppression,
  - outline precision,
  - role-aware infrastructure ranking,
  - strongly typed selection reasons,
  - a tracked rerun harness under `tools/ComparisonHarness`,
  - updated lab and focused-context UI proof surfaces,
  - refreshed comparison evidence against `AppDbContext`, `IClock`, and `CanvasSceneHost`.
- Build, tests, browser proof, and host rerun evidence all passed after the final code changes landed.
- Reopened on 2026-05-09 for relation-hinted focused walking, host MCP input exposure, agent skill guidance, and quantified context-saving proof.
- SB-28 completed on 2026-05-09. Relation hints are now part of the standalone engine contract, lab UI, host MCP input model, comparison harness, and CodeAnalytics MCP skill guidance.
- Reopened again on 2026-05-09 for SB-29: a 20+ scenario real-world evaluation, measured tuning pass, and before/after rerun.
- SB-29 completed on 2026-05-09. The scenario harness ran 22 simulated prompts across three read-only repositories, implemented measured loader/seed/tag improvements, and reran the exact same scenario set.

## Subbundle Gate Results

| Subbundle | Entry gate | Closure gate | Downstream dependencies checked | Progression result | Notes |
| --- | --- | --- | --- | --- | --- |
| SB-15-refactor-foundation-and-canonical-ownership | Passed | Passed | Passed | Passed | Trusted refactor-first baseline |
| SB-16-scoped-diagrams-and-persistence-recovery | Passed | Passed | Passed | Passed | Trusted baseline from the earlier reopen |
| SB-17-member-context-graph-and-query-api | Passed | Passed | Passed | Passed | Trusted baseline from the earlier reopen |
| SB-18-ui-focused-orientation-and-context-explorer | Passed | Passed | Passed | Passed | Trusted baseline from the earlier reopen |
| SB-19-validation-and-mcp-seam-review | Passed | Passed | Passed | Passed | Trusted baseline from the earlier reopen |
| SB-20-helper-surgical-minimal-change-set | Passed | Passed | Passed | Passed | Trusted baseline from the earlier reopen |
| SB-21-helper-context-maintainability-refactor | Passed | Passed | Passed | Passed | Trusted baseline from the earlier reopen |
| SB-22-helper-precision-response-shaping-and-ui | Passed | Passed | Passed | Passed | Trusted baseline from the earlier reopen |
| SB-23-helper-precision-validation-and-sharptools-rerun | Passed | Passed | Passed | Passed | Trusted baseline from the earlier reopen |
| SB-24-definition-mode-payload-suppression-and-outline-precision | Passed | Passed | Passed | Passed | `Outline` precision landed, helper definition mode no longer drags consumer excerpts into the main file set by default, and unit coverage proves the new boundary |
| SB-25-role-aware-ranking-and-selection-reasons | Passed | Passed | Passed | Passed with residual tuning risk | Strongly typed reason and role metadata now flow through the response; infrastructure ranking became more intentional, though `AppDbContext` still needs one more tightening pass |
| SB-26-regression-harness-and-lab-proof | Passed | Passed | Passed | Passed | The rerun path is now tracked under `tools/ComparisonHarness`, the lab shows selection reasons and outline behavior, and browser proof passed |
| SB-27-validation-and-comparison-rerun | Passed | Passed | Passed | Passed | The same three host scenarios were rerun and written back into the bundle with an explicit before-vs-after judgment |
| SB-28-relation-hinted-focused-walking-and-agent-skill | Passed | Passed | Passed | Passed | Relation hints narrow high-fan-in helper usage summaries, the lab and MCP wrapper expose the contract, and harness metrics quantify the context savings |
| SB-29-twenty-scenario-real-world-evaluation-and-tuning | Passed | Passed | Passed | Passed | 22 scenarios ran before and after; failed scenarios dropped from 9 to 0 and average helpfulness improved from 0.434 to 0.714 |

## Validation commands

- `dotnet build C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx -nologo`
- `dotnet test C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx -nologo`
- `dotnet run --project C:\repositories\CanDoItAll.CodeAnalsis\tools\ComparisonHarness\ComparisonHarness.csproj -- C:\repositories\CanDoItAll\CanDoItAll.slnx C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context C:\repositories\CanDoItAll.CodeAnalsis\output\ComparisonHarnessData`
- `dotnet test C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx --no-build -nologo`
- `dotnet build C:\repositories\CanDoItAll\src\CanDoItAll.Mcp.CodeAnalytics\CanDoItAll.Mcp.CodeAnalytics.csproj -nologo`
- `dotnet run --project C:\repositories\CanDoItAll.CodeAnalsis\tools\ComparisonHarness\ComparisonHarness.csproj -- C:\repositories\CanDoItAll\CanDoItAll.slnx C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\relation-hinted-focused-context C:\repositories\CanDoItAll.CodeAnalsis\output\ComparisonHarnessRelationData`
- `python C:\Users\lucys\.codex\skills\candoitall-bundle-preparation\scripts\validate_bundle.py C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle --profile initiative --stage completed`
- `dotnet run --project C:\repositories\CanDoItAll.CodeAnalsis\tools\ScenarioEvaluationHarness\ScenarioEvaluationHarness.csproj -- run C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\twenty-scenario-evaluation\baseline C:\repositories\CanDoItAll.CodeAnalsis\output\ScenarioEvaluationBaselineSnapshots`
- `dotnet run --project C:\repositories\CanDoItAll.CodeAnalsis\tools\ScenarioEvaluationHarness\ScenarioEvaluationHarness.csproj -- run C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\twenty-scenario-evaluation\after C:\repositories\CanDoItAll.CodeAnalsis\output\ScenarioEvaluationAfterSnapshots`
- `dotnet run --project C:\repositories\CanDoItAll.CodeAnalsis\tools\ScenarioEvaluationHarness\ScenarioEvaluationHarness.csproj -- compare C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\twenty-scenario-evaluation\baseline\scenario-evaluation-summary.json C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\twenty-scenario-evaluation\after\scenario-evaluation-summary.json C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\twenty-scenario-evaluation`

## Browser Validation Analytics

| Subbundle | Route | Viewport | Playwright MCP evidence | Screenshots | Result |
| --- | --- | --- | --- | --- | --- |
| SB-26 | `/context-lab?workspacePath=C:\repositories\CanDoItAll.CodeAnalsis\tests\fixtures\Fixture.Shop\Fixture.Shop.slnx&projectFilter=Fixture.Shop.Application&queryText=PlaceOrderAsync&tags=Db&depth=2&precision=Outline` | `1440x1200` | Playwright navigation, DOM snapshot, DOM evaluation, and full-page capture proved the new outline banner, empty-state message, and selection-reason section | `codeanalytics-contextlab-outline-proof.png` | Passed |
| SB-27 | Same route as SB-26 plus rerun harness artifacts under `analysis/post-implementation-focused-context` | `1440x1200` for UI proof, host-solution rerun for scenario proof | Browser truth was used for the new UI surface and the tracked harness supplied the host scenario metrics for `AppDbContext`, `IClock`, and `CanvasSceneHost` | `codeanalytics-contextlab-outline-proof.png` plus focused-context rerun markdown artifacts | Passed |
| SB-28 | `/context-lab?workspacePath=C:\repositories\CanDoItAll.CodeAnalsis\tests\fixtures\Fixture.Shop\Fixture.Shop.slnx&projectFilter=Fixture.Shop.Application&queryText=PlaceOrderAsync&tags=EntityFramework&relationHints=OrderService&depth=2` | `1440x1200` and `390x1000` | Playwright screenshots proved the relation-hints input, normalized run-summary chips, selected files, and supporting context remain readable without overlap | `codeanalytics-contextlab-relation-hints.png`, `codeanalytics-contextlab-relation-hints-mobile.png` | Passed |

## Analytics Review

- Detailed write-up: [03-post-implementation-comparison.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\03-post-implementation-comparison.md)
- Relation-hint write-up: [04-relation-hinted-focused-context.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\04-relation-hinted-focused-context.md)
- Twenty-scenario write-up: [05-twenty-scenario-evaluation.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\05-twenty-scenario-evaluation.md)
- Current rerun artifacts:
  - [focused-context-summary.json](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context\focused-context-summary.json)
  - [focused-context-app-db-context.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context\focused-context-app-db-context.md)
  - [focused-context-i-clock.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context\focused-context-i-clock.md)
  - [focused-context-canvas-scene-host.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context\focused-context-canvas-scene-host.md)
- Relation-hint rerun artifacts:
  - [focused-context-summary.json](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\relation-hinted-focused-context\focused-context-summary.json)
  - [focused-context-i-clock.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\relation-hinted-focused-context\focused-context-i-clock.md)
  - [focused-context-i-clock-workbench-relation.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\relation-hinted-focused-context\focused-context-i-clock-workbench-relation.md)
- Twenty-scenario artifacts:
  - [scenario-evaluation-summary.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\twenty-scenario-evaluation\baseline\scenario-evaluation-summary.md)
  - [scenario-evaluation-summary.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\twenty-scenario-evaluation\after\scenario-evaluation-summary.md)
  - [before-after-comparison.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\twenty-scenario-evaluation\before-after-comparison.md)

## Outcome summary

- `IClock` is the real win of this reopen:
  - before: `209 selected lines / 4 files / 6 blocks`
  - after: `6 selected lines / 1 file / 2 blocks`
  - the breadth moved into usage-summary clusters instead of consumer excerpts
- `CanvasSceneHost` stayed structurally stable:
  - before: `59 selected lines / 3 files / 6 blocks`
  - after: `59 selected lines / 3 files / 6 blocks`
- `AppDbContext` became more intentional but slightly broader:
  - before: `139 selected lines / 5 files / 8 blocks`
  - after: `150 selected lines / 6 files / 10 blocks`
  - the added breadth now includes DI registration and factory evidence, which was previously missing from the first-pass bundle
- SB-28 relation hints make helper walking materially narrower on the current host snapshot:
  - plain `IClock`: `20 usage clusters / 167 callers / 1181 estimated tokens`
  - `IClock` plus `Workbench`: `1 usage cluster / 42 callers / 821 estimated tokens`
  - the helper definition excerpt stayed stable at `6 selected lines / 1 file / 2 blocks`
- SB-29 real-world scenario evaluation widened the evidence set:
  - baseline: `22 scenarios / 6 introduction / 9 failed / average helpfulness 0.434`
  - after: `22 scenarios / 6 introduction / 0 failed / average helpfulness 0.714`
  - the primary fixed defect was duplicate project paths in MSBuildWorkspace results, which made `influxdb-client-csharp` load as an empty snapshot

## SharpTools standing

- SharpTools still wins exact-symbol drill-down for `AppDbContext` and `CanvasSceneHost`.
- SharpTools still wins absolute minimalism for `IClock`.
- Focused-context now holds a stronger first-pass position:
  - database trouble paths: still the better one-call bundle,
  - helpers: now genuinely usable for first-pass orientation instead of obviously too noisy,
  - UI: preserved as a good one-call mini-cluster.

## Raw Note Closure

| Raw note | Status | Proof |
| --- | --- | --- |
| Start with detailed refactoring first | Solved | The focused-context service is now split across strategy, selection, reasons, and excerpt partials instead of one widening heuristic path |
| Implement the previous recommendations | Solved | The follow-up pass implemented payload suppression, outline precision, role-aware ranking, selection reasons, tracked reruns, and revalidation |
| Add focused trouble-path code context | Solved with residual risk | The feature remains useful for database and UI trouble paths; only the database case still needs another tightening pass |
| Start from exception or compile-error text | Solved | Prior unit coverage still stands |
| Allow tags to focus the result | Solved | Prior proof still stands and the rerun harness preserved tagged scenarios |
| Show accordions grouped by file with selected code parts | Solved | The lab still renders file-based accordions when excerpts are present |
| Show line-count stats for file groups and the full result | Solved | The lab still reports per-file and aggregate counts |
| Provide a dedicated page for tuning with workspace scope, prompt, and tags together | Solved | `/context-lab` remains the tuning surface and now shows outline and reason metadata as well |
| Explain what feedback is needed to tune the heuristics | Solved | The post-implementation comparison write-up now records where the algorithm improved and where it still over-expands |
| Analyze the focused-context flow against SharpTools | Solved | The same three named scenarios were rerun and judged again against the SharpTools baseline |
| Compare helpfulness and noise, not only counts | Solved | The new bundle analysis calls out why `IClock` is truly better and why `AppDbContext` is still mixed despite stronger intentionality |
| Improve the feature based on the comparison | Solved with residual risk | The helper case is materially better and the response is more explainable; the database case still needs another ranking pass |
| Include generic readability and structure refactoring | Solved | Strategy, ranking, reasons, UI rendering, and rerun tooling are now easier to read and modify |
| Make helpers like `IClock` more surgical and precise | Solved | `IClock` now defaults to a definition bundle plus usage summary instead of carrying consumer excerpts |
| Start with the minimal change set first | Solved | `Outline` precision and helper payload suppression landed before the ranking and rerun surface work |
| Then refactor for maintainability | Solved | A new tracked runner and clearer partial ownership keep the feature easier to extend |
| Then add the broader helper-mode improvements | Solved | Role-aware reasons, UI surfacing, and the rerun workflow all landed after the shaping baseline was stable |
| Implement the comparison-bundle improvements and revalidate | Solved | The code changes shipped and the host comparison was rerun through the tracked harness |
| Large projects overload agent context too easily | Solved with residual tuning risk | SB-28 relation-hint metrics show a 30.5% estimated-token reduction and 95% usage-cluster reduction for the relation-hinted helper case |
| Complete scans should be specific cases | Solved | The CodeAnalytics MCP skill now directs agents toward scoped snapshots, exact symbols, and focused context before broad scans |
| Ask for usages with tags like `db` or `EntityFramework` | Solved | Tag aliases and relation-hint lab proof passed with `EntityFramework` |
| Ask for a helper plus related classes or components | Solved | The `IClock` plus `Workbench` harness scenario proves relation-hinted helper narrowing |
| Combine tool and agent skill as a bundle | Solved | The host MCP input model, coordinator, and CodeAnalytics MCP skill were updated together |
| Add basic UI and measure results | Solved | `/context-lab` exposes relation hints and the harness records file, line, cluster, caller, character, token, and elapsed metrics |
| Analyze at least 20 real-world-looking problems | Solved | SB-29 ran 22 scenarios across `MBusParser`, `influxdb-client-csharp`, and `CanDoItAll` |
| Simulate prompts and standard detail-gathering approach | Solved | Every scenario artifact records the simulated prompt and intended agent approach |
| Include at least 5 project introduction scenarios | Solved | The suite includes 6 introduction scenarios |
| Judge helpful and non-useful context | Solved | The harness scores term coverage, file coverage, non-useful file ratio, token budget ratio, and helpfulness |
| Implement improvements and retest | Solved | Loader dedupe, tag aliases, exact-type seed anchoring, and member-signature seed scoring were implemented and rerun against the same scenarios |

## Residual risks

- `AppDbContext` is more intentional but structurally broader than the prior focused-context pass. The new DI and factory evidence is useful, but the ranking still needs a future tightening pass to reduce line count without dropping that intent.
- Current artifact token counts are honest to the current payload because they include selection-reason metadata. That means structural metrics and usefulness still matter more than raw token totals alone.
- Role classification is still heuristic. The new narrowing avoids obvious false positives from generic `Add*` and `Create*` business methods, but the standing comparison set should remain the guardrail for future changes.
- Relation hints are still explicit request hints, not a semantic ontology. The skill guidance must keep teaching agents to provide concrete related symbols, components, namespaces, projects, or paths.
- The remaining poor scenario is a prompt-quality mismatch: `mbus-enum-utils-dif` asks for DIF/VIF usages of `EnumUtils`, but actual usages are in header/parser code. A future skill improvement should tell agents to verify references when relation-hinted context contradicts the prompt assumption.

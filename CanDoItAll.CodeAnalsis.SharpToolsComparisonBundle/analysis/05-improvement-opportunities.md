# Improvement opportunities

## Immediate product improvements

1. Suppress helper consumer excerpts in definition mode

Evidence:
`IClock` still produced `209` selected lines and `3987` estimated tokens even after auto-resolving to `Definition` + `Surgical`.

Recommendation:
When a high-fan-in helper resolves to definition mode, keep only:

- the contract,
- production implementations,
- usage clusters and counts,
- optionally one tiny representative caller snippet when the caller shows a non-obvious pattern.

Target:
Bring common helper definition-mode results under roughly `40` selected lines and `2` files by default.

2. Separate usage summary from excerpt selection more aggressively

Evidence:
The helper response already contains useful usage clusters, but it still includes large consumer excerpts in the main file set.

Recommendation:
Treat usage summary as a first-class answer shape. In helper definition mode, consumer breadth should live in summary metadata unless the caller explicitly asks for usage detail.

3. Add reference-role classification for infrastructure symbols

Evidence:
The `AppDbContext` focused-context result was useful, but SharpTools surfaced some reference roles the focused bundle did not prioritize, especially DI registration, factory wiring, and schema initializer entry points.

Recommendation:
Classify references into explicit roles such as:

- registration,
- factory,
- schema initializer,
- consumer service,
- test.

Use those roles to guarantee one high-signal excerpt from each important category before adding more arbitrary consumers.

4. Add selection reasons to the response payload

Evidence:
The focused-context results show what was selected but not clearly why each consumer or file entered the bundle.

Recommendation:
Attach a short structured reason to each selected member or file, for example:

- `seed member`,
- `implementation`,
- `direct caller`,
- `factory for seed type`,
- `schema bootstrap`,
- `top project cluster sample`.

This would make noise tuning much easier in the lab and in future comparisons.

## Precision-mode improvements

5. Add a lighter `Outline` or `SummaryOnly` precision mode

Evidence:
SharpTools often wins because it can stop at definition plus references without carrying large code excerpts.

Recommendation:
Add an explicit precision level that returns:

- symbol identities,
- file paths,
- counts,
- cluster summaries,
- no code excerpts.

This is particularly useful for helpers and other high-fan-in symbols.

6. Enforce cluster-aware excerpt budgets

Evidence:
The helper result used a large excerpt from `CrmHrServices.cs`, which dominated the payload without being essential for first-pass orientation.

Recommendation:
Set stricter caps in helper-oriented modes:

- maximum one consumer excerpt per cluster,
- maximum one large-file consumer excerpt overall,
- maximum lines per consumer excerpt,
- prefer smaller exemplars when multiple consumers express the same pattern.

## Preserve what already works

7. Keep UI bundles type-centric and compact

Evidence:
`CanvasSceneHost` is already a good focused-context result.

Recommendation:
Add a regression rule so UI symbols with few references do not expand beyond a small bounded cluster. The current behavior should be preserved, not re-tuned broadly.

8. Keep database trouble paths consumer-aware, but add better ranking

Evidence:
`AppDbContext` benefited from preselected consumer excerpts, which is part of the feature value.

Recommendation:
Do not collapse database trouble paths into definition-only results. Instead, rank consumers better by promoting:

- factories and registrations first,
- schema initializers second,
- concrete business consumers third.

## Process improvements

9. Turn this comparison into a repeatable regression suite

Evidence:
The disposable harness and the normalized scenario artifacts made this study reproducible.

Recommendation:
Keep these three scenarios as a standing validation set:

- `AppDbContext`
- `IClock`
- `CanvasSceneHost`

Fail future tuning passes when selected lines, token estimates, or call-shape usefulness regress beyond agreed thresholds.

10. Keep setup and warm costs separate in future reports

Evidence:
The comparison would be misleading if the snapshot-build setup and the solution-load setup were blended into a single number.

Recommendation:
Continue reporting:

- setup cost,
- one-off first-pass cost,
- warm repeated-query cost.

That distinction explains when focused-context or SharpTools is the better operational choice.

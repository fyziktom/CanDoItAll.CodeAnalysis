# Raw follow-up: surgical helper precision

Source: user follow-up on 2026-04-08 after the helper-noise analysis.

## Request

- Use the bundle workflow again.
- Implement all helper-precision improvements that were just described.
- Start with the minimal change set first.
- Then do common refactoring to improve maintainability.
- Then add the broader helper-precision improvements from the `How To Improve It` list.

## Specific intent to preserve

- Helpers like `IClock` must become more surgical and precise for analytics tasks.
- Noise must be reduced when the queried symbol is intentionally generic and high fan-in.
- The focused-context flow should become closer to SharpTools precision for helper-style exploration without pretending to replace SharpTools fully.

## Design direction captured from the analysis

- Add explicit helper-oriented intent instead of treating every query as the same trouble-path traversal.
- Detect helper seeds and switch to directional traversal.
- Prefer definitions, implementations, and sampled usages over indiscriminate consumer spread.
- Group or summarize widespread helper consumers instead of loading all of them into the main excerpt payload.
- Keep the implementation strongly typed and maintainable.

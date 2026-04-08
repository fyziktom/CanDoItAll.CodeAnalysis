# Assumptions and risks

## Assumptions

- The snapshot facts are already rich enough for a first symbol-tools release.
- Exact-symbol navigation is more valuable now than adding a second, deeper Roslyn inspection layer.
- A single snapshot page can exercise the new capabilities without fragmenting the UI into many routes.
- The comparison rerun can reuse the prior three host scenarios and add additional ones that emphasize implementations and references.

## Risks

- Definition search will still be synthesized from snapshot facts and source references, not full declaration-text parsing, so regex precision will be lower than SharpTools.
- Reference tracing for type-level queries depends on relationship facts and service registrations, so some indirect usages may still be absent.
- A naive UI could duplicate the types page instead of adding a clear symbol-inspection workflow.
- Broad host scenarios can create large reference lists, so the response model needs limits and ordering rules.

## Critical Path Risks

- If the contract boundary is fuzzy, the implementation will drift into another focused-context-shaped surface instead of real symbol tools.
- If source-excerpt logic is copied again instead of shared, maintainability will get worse during the parity pass.
- If the extra scenarios are chosen too late, the rerun phase can become a post-hoc justification exercise instead of a fair validation step.

## Validation Risks

- Fixture-only proof would be too weak because the user explicitly asked to prove the result on the prior host scenarios again.
- Service-level tests alone are too weak because the user also needs a second UI path to reach the information.
- Browser proof without scenario-level output review would miss whether the new tools are actually helpful instead of just present.

## Reopen Triggers

- Reopen `SB-00` if implementation starts requiring capabilities that the current snapshot facts clearly do not support.
- Reopen `SB-01` or `SB-02` if browser proof shows the UI can render the route but the underlying symbol output is noisy or misleading.
- Reopen `SB-04` if the widened scenario rerun still shows narrow overfitting or missing symbol-start-point cases.

## Guardrails

- Keep the first release explicit about what is definition search versus reference tracing.
- Prefer deterministic ordering and capped result counts over dumping every possible reference.
- Surface source paths and line numbers everywhere so users and agents can widen context deliberately.

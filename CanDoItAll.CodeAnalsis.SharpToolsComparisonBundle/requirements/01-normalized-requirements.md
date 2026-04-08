# Normalized requirements

## Functional requirements

- `REQ-001`: The study must use three genuinely different scenarios from `C:\repositories\CanDoItAll\CanDoItAll.slnx`.
- `REQ-002`: The study must compare focused context and SharpTools for each scenario.
- `REQ-003`: The study must capture actual outputs, not only theoretical expectations.
- `REQ-004`: The study must assess usefulness and noise, not just raw size.
- `REQ-005`: The study must record estimated token load using one consistent method for both sides.
- `REQ-006`: The study must record call count for each side and each scenario.
- `REQ-007`: The study must record elapsed time for each side and each scenario.
- `REQ-008`: The study must preserve setup-cost context separately from warm per-scenario comparison when relevant.
- `REQ-009`: The findings must be written into this new bundle, not left in chat only.

## Reporting requirements

- `REQ-010`: The final report must summarize which side is better for first-pass navigation in each scenario.
- `REQ-011`: The final report must identify where focused context is helpful, where it is noisy, and where SharpTools still wins.
- `REQ-012`: The final report must include residual uncertainties or methodological limits.

## Non-goals

- `NONGOAL-001`: This bundle does not implement new feature work by itself.
- `NONGOAL-002`: This bundle does not attempt exact tokenizer fidelity if consistent relative estimation is available.

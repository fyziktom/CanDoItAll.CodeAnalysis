# Comparison implementation follow-up

Requested on 2026-04-08 after the comparison bundle closed.

## User request

- implement the improvements you suggest
- revalidate how we stand after they are implemented

## Source bundle

- `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle`

## Priority findings carried into this reopen

1. High-fan-in helper definition mode still carries too much consumer code.
2. Usage-summary breadth is useful, but it should not force large consumer excerpts into the main payload.
3. Infrastructure trouble paths such as `AppDbContext` need better role-aware ranking instead of arbitrary consumer spread.
4. The response should explain why members or files were selected so tuning can be evidence-based.
5. A lighter outline-style precision mode is needed for symbol orientation without excerpt cost.
6. The comparison should be rerunnable without rebuilding the methodology from scratch.

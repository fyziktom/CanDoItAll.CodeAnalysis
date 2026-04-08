# Target solution

- Product repo: `C:\repositories\CanDoItAll.CodeAnalsis`
- Host validation repo: `C:\repositories\CanDoItAll\CanDoItAll.slnx`

## Main product areas expected to change

- `src/CanDoItAll.CodeAnalytics.Abstractions`
- `src/CanDoItAll.CodeAnalytics.Application`
- `src/CanDoItAll.CodeAnalytics.Web`
- `tests/CanDoItAll.CodeAnalytics.Tests.Unit`
- `tests/CanDoItAll.CodeAnalytics.Tests.Web`
- `tools/ComparisonHarness`

## Likely design seam

- New symbol query and response contracts in abstractions.
- New application-service methods implemented over snapshot facts.
- One new snapshot route for symbol exploration.
- Comparison harness support for symbol-tools scenario runs.

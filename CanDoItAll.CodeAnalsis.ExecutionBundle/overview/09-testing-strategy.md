# Testing strategy

## Unit tests
Focus on deterministic logic:
- naming/path normalization
- graph algorithms
- risk rules
- summary writers
- Mermaid renderers
- serialization/canonical ordering
- tool-surface mapping helpers

## Integration tests
Focus on fixture-solution and file-system behaviors:
- workspace loading
- symbol extraction
- DI extraction
- persistence extraction
- snapshot assembly
- cache/storage behavior

## Web tests
Focus on SSR routes and UI flows:
- run analysis page
- dashboard
- drilldowns
- export list

## Architecture tests
Protect:
- project reference boundaries
- naming map (`CodeAnalsis` vs `CodeAnalytics`)
- no host-only core duplication
- future MCP seam expectations

## Golden-file tests
Persist representative:
- snapshot JSON
- Markdown summaries
- Mermaid diagrams
- selected query outputs

## Fixture strategy
Use both:
- a small fixture solution under `tests/fixtures/`
- optional targeted analysis against real host-repo snippets only when safe and deterministic

## Final validation
At closure run:
- restore
- build
- unit tests
- integration tests
- web tests
- architecture tests
- format
- file-length / structure validation scripts

# File index

## Primary entry points

- `README.md`
- `START-HERE.md`
- `MASTER-START-PROMPT.md`
- `spreadsheets/CanDoItAll.CodeAnalsis.ExecutionBacklog.xlsx`
- `reference/current-candoitall-mcp-context.md`

## Bundle statistics

- total files: `147`
- subbundles: `16`
- overview docs: `19`
- ADRs: `8`
- prompts: `7`
- workbook sheets: `10`
- features tracked: `61`
- user stories tracked: `66`
- validation rows: `49`

## Directory tree

```text
CanDoItAll.CodeAnalsis.ExecutionBundle
├── adrs
│   ├── ADR-001-repo-and-naming.md
│   ├── ADR-002-roslyn-first-analysis.md
│   ├── ADR-003-canonical-snapshot-model.md
│   ├── ADR-004-ssr-first-ui.md
│   ├── ADR-005-facts-and-insights-separation.md
│   ├── ADR-006-align-future-driver-with-current-candoitall-mcp-patterns.md
│   ├── ADR-007-do-not-duplicate-candoitall-mcp-core.md
│   └── ADR-008-ship-lightweight-repo-assets-for-portability.md
├── overview
│   ├── 01-executive-summary.md
│   ├── 02-architecture-blueprint.md
│   ├── 03-solution-structure.md
│   ├── 04-canonical-snapshot-model.md
│   ├── 05-analysis-pipeline.md
│   ├── 06-ui-blueprint.md
│   ├── 07-implementation-roadmap.md
│   ├── 08-quality-gates.md
│   ├── 09-testing-strategy.md
│   ├── 10-repository-conventions.md
│   ├── 11-future-mcp-handoff.md
│   ├── 12-risk-register.md
│   ├── 13-project-folder-map.md
│   ├── 14-fixture-solution-design.md
│   ├── 15-current-candoitall-mcp-landscape.md
│   ├── 16-compatibility-and-shared-parts.md
│   ├── 17-naming-settings-and-tool-surface-map.md
│   ├── 18-execution-order-and-closure-evidence.md
│   └── 19-host-repo-shared-surface-catalog.md
├── prompts
│   ├── 00-codex-system-prompt.md
│   ├── 01-codex-session-start.md
│   ├── 02-codex-refactor-pass.md
│   ├── 03-codex-review-pass.md
│   ├── 04-codex-master-start-prompt.md
│   ├── 05-codex-compatibility-audit.md
│   └── 06-codex-final-handoff-prompt.md
├── reference
│   ├── mermaid
│   │   ├── example-class-diagram.mmd
│   │   ├── example-er-diagram.mmd
│   │   ├── example-project-dependencies.mmd
│   │   └── future-candoitall-codeanalytics-integration.mmd
│   ├── architecture-snapshot-v1.schema.json
│   ├── CanDoItAll.Mcp.CodeAnalytics.settings.example.json
│   ├── current-candoitall-mcp-context.json
│   ├── current-candoitall-mcp-context.md
│   ├── sample-architecture-snapshot.json
│   ├── tool-surface-proposal.json
│   └── vscode-mcp-snippet.code-analytics.json
├── spreadsheets
│   └── CanDoItAll.CodeAnalsis.ExecutionBacklog.xlsx
├── subbundles
│   ├── SB-00-repository-bootstrap
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-00A-candoitall-compatibility-baseline
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-01-canonical-domain-model-and-contracts
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-02-workspace-loading-and-solution-inventory
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-03-symbol-indexing-and-xml-documentation
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-04-dependency-graph-and-module-view
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-05-di-and-service-registration-analysis
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-06-efcore-and-persistence-view
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-07-risk-rules-and-insights
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-08-snapshot-assembly-serialization-and-caching
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-09-summary-writers-and-mermaid-renderers
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-10-application-orchestrator-and-query-api
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-11-blazor-ssr-ui-shell-and-dashboard
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-12-ui-drilldown-search-and-export
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   ├── SB-13-future-candoitall-mcp-driver-seam
│   │   ├── 01-scope.md
│   │   ├── 02-prompt.md
│   │   ├── 03-checklist.md
│   │   ├── 04-validation.md
│   │   ├── 05-forbidden-patterns.md
│   │   └── 06-required-evidence.md
│   └── SB-14-tests-hardening-final-refactor
│       ├── 01-scope.md
│       ├── 02-prompt.md
│       ├── 03-checklist.md
│       ├── 04-validation.md
│       ├── 05-forbidden-patterns.md
│       └── 06-required-evidence.md
├── bundle-manifest.json
├── FILE-INDEX.md
├── MASTER-START-PROMPT.md
├── README.md
└── START-HERE.md
```

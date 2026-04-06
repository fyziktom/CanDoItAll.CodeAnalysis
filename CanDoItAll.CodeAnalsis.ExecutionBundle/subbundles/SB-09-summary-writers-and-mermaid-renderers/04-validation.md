# Validation for SB-09 — Summary writers and Mermaid renderers

## Acceptance criteria
- Renderable summaries and Mermaid outputs are generated from the snapshot deterministically.
- Oversized diagram cases fail helpfully rather than producing junk.
- Export metadata can drive UI and future MCP selection flows.

## Validation commands
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj --filter Mermaid`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj --filter SummaryWriter`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`

## Blocking stop conditions
- Do not continue if diagram text generation becomes the canonical data source.
- Do not continue if summaries omit provenance about uncertainty or truncation.

## Evidence expected
- Golden outputs for class, ER, and dependency diagrams.
- Summary output samples with diagnostics/truncation notes.

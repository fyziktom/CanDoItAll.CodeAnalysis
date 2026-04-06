# Prompt for SB-03 — Symbol indexing and XML documentation ingestion

You are implementing **SB-03** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Collect symbol-level facts such as namespaces, types, members, inheritance, source locations, and XML documentation summaries from the analyzed solution.

## Read before coding
- overview/05-analysis-pipeline.md
- overview/04-canonical-snapshot-model.md
- adrs/ADR-002-roslyn-first-analysis.md
- subbundles/SB-03-symbol-indexing-and-xml-documentation/01-scope.md
- subbundles/SB-03-symbol-indexing-and-xml-documentation/03-checklist.md
- subbundles/SB-03-symbol-indexing-and-xml-documentation/04-validation.md
- subbundles/SB-03-symbol-indexing-and-xml-documentation/05-forbidden-patterns.md
- subbundles/SB-03-symbol-indexing-and-xml-documentation/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- .github/copilot-instructions.md

## Required implementation steps
- Create symbol visitors or analyzers that gather named type facts, inheritance, and member signatures.
- Normalize XML docs into concise text fields and retain provenance/source locations.
- Capture partial resolution states when compilation errors block some symbols.
- Add fixture cases for generics, records, partial classes, nested types, and missing XML docs.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if XML doc ingestion breaks runs on projects that omit XML docs.
- Do not continue if symbol identity is unstable across repeated runs.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.

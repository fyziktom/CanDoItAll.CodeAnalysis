# Prompt for SB-06 — EF Core and persistence view

You are implementing **SB-06** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Extract DbContext, entity, relationship, table-mapping, and migration-related facts sufficient for a useful persistence overview and Mermaid ER export.

## Read before coding
- overview/05-analysis-pipeline.md
- overview/14-fixture-solution-design.md
- subbundles/SB-06-efcore-and-persistence-view/01-scope.md
- subbundles/SB-06-efcore-and-persistence-view/03-checklist.md
- subbundles/SB-06-efcore-and-persistence-view/04-validation.md
- subbundles/SB-06-efcore-and-persistence-view/05-forbidden-patterns.md
- subbundles/SB-06-efcore-and-persistence-view/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- src/CanDoItAll.Web/CanDoItAll.Web.csproj
- src/CanDoItAll.Migrations.PostgreSql/CanDoItAll.Migrations.PostgreSql.csproj
- src/CanDoItAll.Migrations.Sqlite/CanDoItAll.Migrations.Sqlite.csproj

## Required implementation steps
- Choose a safe primary extraction strategy that prefers static model information and optional metadata loading.
- Normalize entity, property, key, relationship, and table mapping facts.
- Capture unsupported model patterns as diagnostics.
- Add tests for one-to-many, many-to-many, owned types, and ambiguous mappings as feasible.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if EF extraction requires a live database.
- Do not continue if relationship facts are only encoded as Mermaid text instead of canonical records.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.

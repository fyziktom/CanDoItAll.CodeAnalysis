# SB-06 — EF Core and persistence view

## Objective
Extract DbContext, entity, relationship, table-mapping, and migration-related facts sufficient for a useful persistence overview and Mermaid ER export.

## Milestone / priority / actor
- Milestone: `M2`
- Priority: `P0`
- Primary actor: `Persistence maintainer`

## Depends on
- SB-01
- SB-02
- SB-03

## Read first
- overview/05-analysis-pipeline.md
- overview/14-fixture-solution-design.md

## Current CanDoItAll reference files to inspect
- src/CanDoItAll.Web/CanDoItAll.Web.csproj
- src/CanDoItAll.Migrations.PostgreSql/CanDoItAll.Migrations.PostgreSql.csproj
- src/CanDoItAll.Migrations.Sqlite/CanDoItAll.Migrations.Sqlite.csproj

## In scope
- Detect DbContexts and entity sets.
- Extract entity properties, keys, foreign keys, and relationship shape from EF Core metadata or static model analysis.
- Record table/schema mapping where resolvable.
- Produce enough normalized data for ER summaries and Mermaid ER diagrams.

## Out of scope
- No live database connections.
- No provider-specific runtime introspection as a requirement.

## Compatibility rules specific to this subbundle
- Design the persistence view so later analysis of the current CanDoItAll repo can cover its multi-provider migrations layout.
- Keep provider-specific details optional and diagnostics-backed rather than mandatory.

## Expected deliverables
- Persistence facts collector and model normalizer.
- Fixture solution with DbContext and entity relationships.
- ER-oriented golden files and Mermaid example output.

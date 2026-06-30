# EF Analyzer Capabilities

The persistence analyzer is a static Roslyn fact collector. It inspects source code and symbols to identify Entity Framework Core persistence structure, but the app does not execute EF Core LINQ queries or connect to a production database.

## Supported Static Facts

- `DbContext` types and their discovered entity sets.
- Entity table names, schemas, primary keys, and source references when they can be inferred from attributes, configuration calls, or model snapshots.
- Entity relationships and navigation property names.
- Model snapshot metadata and configuration discovery, including diagnostics when a persistence pattern is only partially interpreted.

## Not Runtime Query Tuning

The analyzer does not currently claim to detect or optimize runtime EF Core query behavior such as N+1 queries, missing `AsNoTracking`, split-query choices, compiled-query opportunities, generated SQL shape, database indexes, or client-evaluation warnings.

Those topics require real query shapes and/or EF Core execution logs. They should only be documented as supported after a future analyzer feature adds dedicated query-shape detection with positive and negative tests.

## Ownership Decision

For the publishing-prep wave, EF persistence analysis remains inside `CanDoItAll.CodeAnalytics.Facts`. A future `Facts.EfCore` addon can be introduced after fact-pack registration and package boundaries are configurable.

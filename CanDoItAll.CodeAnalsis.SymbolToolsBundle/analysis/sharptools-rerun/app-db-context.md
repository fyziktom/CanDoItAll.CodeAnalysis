# AppDbContext SharpTools rerun

## Query flow

- SearchDefinitions regex: `\bAppDbContext\b`
- ViewDefinition: `CanDoItAll.Infrastructure.Persistence.AppDbContext`
- GetMembers: `CanDoItAll.Infrastructure.Persistence.AppDbContext`
- ListImplementations: `CanDoItAll.Infrastructure.Persistence.AppDbContext`
- FindReferences: `CanDoItAll.Infrastructure.Persistence.AppDbContext`

## Metrics

- Warm calls: `5`
- Warm time: `58813 ms`
- Search matches: `20`
- Members: `4`
- Implementations: `0`
- References: `288 total / 20 shown`

## Helpfulness

- The exact definition is clean and immediately useful.
- The reference excerpts surface the factory and registration path quickly.
- The tool keeps the code excerpts close to the symbol that owns them instead of building a wider bundle.

## Noise

- The search step is broad for a simple class name and lands many constructor-parameter matches before the actual type.
- The reference surface is wider than most first-pass investigations need, so an agent still has to rank the result set manually.

## Verdict

- SharpTools is strong for exact drill-down once the type is known.
- Search precision is weaker than the new symbol search route for this scenario.

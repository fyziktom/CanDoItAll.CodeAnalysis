# IDatabaseRuntimeState SharpTools rerun

## Query flow

- SearchDefinitions regex: `\bIDatabaseRuntimeState\b`
- ViewDefinition: `CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState`
- GetMembers: `CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState`
- ListImplementations: `CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState`
- FindReferences: `CanDoItAll.Infrastructure.Persistence.IDatabaseRuntimeState`

## Metrics

- Warm calls: `5`
- Warm time: `54068 ms`
- Search matches: `7`
- Members: `4`
- Implementations: `1`
- References: `6 total / 6 shown`

## Helpfulness

- The contract, implementation, and references line up cleanly around the runtime-switching workflow.
- The result is easy to reason about because the reference set is naturally small.

## Noise

- Search still includes DI and constructor-parameter matches, but the spread is small enough that it does not get in the way.

## Verdict

- SharpTools performs well for this infrastructure contract.
- The new symbol tools are close to parity and win on search precision.

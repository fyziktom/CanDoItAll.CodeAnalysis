# influx-intro-query-flow

## Simulated Prompt

I need to understand Flux query execution and result parsing before changing query behavior.

## Simulated Agent Approach

Start with QueryApi, then request query/parser context around Flux table and parser types.

## Query

- Repository: `influxdb-client-csharp`
- Category: `Introduction`
- Query text: `QueryApi`
- Focus tags: `Query`
- Relation hints: `FluxCsvParser`, `FluxTable`, `FluxRecord`
- Depth: 2
- Intent: `TroublePath`
- Precision: `Outline`

## Score

- Rating: `Mixed`
- Helpfulness score: 0,655
- Expected terms: 3/4
- Expected files: 2/2
- Useful files: 0
- Non-useful files: 0
- Noise term hits: 1
- Token budget ratio: 0,160

## Output Metrics

- Search results: 40
- Seed type: InfluxDB.Client.QueryApi
- Seed member: InfluxDB.Client.QueryApi.CreateQuery(string, InfluxDB.Client.Api.Domain.Dialect)
- Files: 0
- Blocks: 0
- Selected lines: 0
- Estimated tokens: 560
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `InfluxDB.Client.QueryApiSync` (Type)
- `InfluxDB.Client.IQueryApi.QueryAsync(InfluxDB.Client.Api.Domain.Query, Action<InfluxDB.Client.Core.Flux.Domain.FluxRecord>, Action<Exception>, Action, string, CancellationToken)` (Member)
- `InfluxDB.Client.IQueryApi.QueryAsync(InfluxDB.Client.Api.Domain.Query, Type, Action<object>, Action<Exception>, Action, string, CancellationToken)` (Member)
- `InfluxDB.Client.IQueryApi.QueryAsync(InfluxDB.Client.Api.Domain.Query, Type, string, CancellationToken)` (Member)
- `InfluxDB.Client.IQueryApi.QueryAsync(InfluxDB.Client.Api.Domain.Query, string, CancellationToken)` (Member)
- `InfluxDB.Client.IQueryApi.QueryAsync(string, Action<InfluxDB.Client.Core.Flux.Domain.FluxRecord>, Action<Exception>, Action, string, CancellationToken)` (Member)
- `InfluxDB.Client.IQueryApi.QueryAsync(string, Type, Action<object>, Action<Exception>, Action, string, CancellationToken)` (Member)
- `InfluxDB.Client.IQueryApi.QueryAsync(string, Type, string, CancellationToken)` (Member)

## Selected Files


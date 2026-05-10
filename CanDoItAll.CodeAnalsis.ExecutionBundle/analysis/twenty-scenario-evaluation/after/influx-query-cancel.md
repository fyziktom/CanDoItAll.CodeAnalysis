# influx-query-cancel

## Simulated Prompt

QueryApi cancellation handling needs review. Show QueryApi with parsing and test context.

## Simulated Agent Approach

Use QueryApi with query/parser/test relation hints.

## Query

- Repository: `influxdb-client-csharp`
- Category: `Specific`
- Query text: `QueryApi`
- Focus tags: `Query`
- Relation hints: `FluxCsvParser`, `QueryApiTest`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Mixed`
- Helpfulness score: 0,583
- Expected terms: 2/3
- Expected files: 2/3
- Useful files: 3
- Non-useful files: 5
- Noise term hits: 1
- Token budget ratio: 1,084

## Output Metrics

- Search results: 40
- Seed type: InfluxDB.Client.QueryApi
- Seed member: InfluxDB.Client.QueryApi.CreateQuery(string, InfluxDB.Client.Api.Domain.Dialect)
- Files: 8
- Blocks: 15
- Selected lines: 205
- Estimated tokens: 3251
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

- `Client/QueryApiSync.cs`: 64/219 lines, 4 blocks
- `Client/QueryApi.cs`: 61/808 lines, 5 blocks
- `Client/InfluxDB.Client.Api/Domain/Dialect.cs`: 32/266 lines, 1 blocks
- `Client/InfluxDB.Client.Api/Domain/Query.cs`: 28/229 lines, 1 blocks
- `Client/InfluxDB.Client.Api/Service/QueryService.cs`: 8/2710 lines, 1 blocks
- `Client.Core/Flux/Domain/FluxRecord.cs`: 6/122 lines, 1 blocks
- `Client.Core/Flux/Internal/FluxCsvParser.cs`: 3/415 lines, 1 blocks
- `Client.Core/Flux/Internal/IFluxResultMapper.cs`: 3/27 lines, 1 blocks

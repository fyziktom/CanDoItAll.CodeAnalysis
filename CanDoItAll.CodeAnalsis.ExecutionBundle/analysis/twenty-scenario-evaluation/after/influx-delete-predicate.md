# influx-delete-predicate

## Simulated Prompt

DeleteApi predicate formatting is failing in a specific test. Show DeleteApi and its test surface.

## Simulated Agent Approach

Search DeleteApi and request direct test-related context.

## Query

- Repository: `influxdb-client-csharp`
- Category: `Specific`
- Query text: `DeleteApi`
- Focus tags: `Client`, `Test`
- Relation hints: `DeleteApiTest`, `DeletePredicateRequest`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Good`
- Helpfulness score: 0,802
- Expected terms: 2/2
- Expected files: 1/1
- Useful files: 2
- Non-useful files: 4
- Noise term hits: 1
- Token budget ratio: 1,148

## Output Metrics

- Search results: 24
- Seed type: InfluxDB.Client.DeleteApi
- Seed member: InfluxDB.Client.DeleteApi.Delete(DateTime, DateTime, string, InfluxDB.Client.Api.Domain.Bucket, InfluxDB.Client.Api.Domain.Organization, CancellationToken)
- Files: 6
- Blocks: 8
- Selected lines: 190
- Estimated tokens: 2525
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `InfluxDB.Client.IDeleteApi.Delete(DateTime, DateTime, string, InfluxDB.Client.Api.Domain.Bucket, InfluxDB.Client.Api.Domain.Organization, CancellationToken)` (Member)
- `InfluxDB.Client.IDeleteApi.Delete(DateTime, DateTime, string, string, string, CancellationToken)` (Member)
- `InfluxDB.Client.IDeleteApi.Delete(InfluxDB.Client.Api.Domain.DeletePredicateRequest, string, string, CancellationToken)` (Member)
- `InfluxDB.Client.DeleteApi.Delete(DateTime, DateTime, string, InfluxDB.Client.Api.Domain.Bucket, InfluxDB.Client.Api.Domain.Organization, CancellationToken)` (Member)
- `InfluxDB.Client.DeleteApi.Delete(DateTime, DateTime, string, string, string, CancellationToken)` (Member)
- `InfluxDB.Client.DeleteApi.Delete(InfluxDB.Client.Api.Domain.DeletePredicateRequest, string, string, CancellationToken)` (Member)
- `InfluxDB.Client.DeleteApi.DeleteApi(InfluxDB.Client.Api.Service.DeleteService)` (Member)
- `InfluxDB.Client.DeleteApi._service` (Member)

## Selected Files

- `Client/InfluxDB.Client.Api/Domain/Bucket.cs`: 58/340 lines, 1 blocks
- `Client/DeleteApi.cs`: 39/119 lines, 3 blocks
- `Client/InfluxDB.Client.Api/Domain/Organization.cs`: 36/255 lines, 1 blocks
- `Client/InfluxDB.Client.Api/Service/DeleteService.cs`: 26/827 lines, 1 blocks
- `Client/InfluxDB.Client.Api/Domain/DeletePredicateRequest.cs`: 23/178 lines, 1 blocks
- `Client/InfluxDBClient.cs`: 8/1002 lines, 1 blocks

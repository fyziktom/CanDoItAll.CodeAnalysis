# influx-write-async

## Simulated Prompt

I need to change async write error handling in WriteApiAsync without touching sync write behavior by accident.

## Simulated Agent Approach

Ask for WriteApiAsync with relation hints to sync WriteApi and retry behavior.

## Query

- Repository: `influxdb-client-csharp`
- Category: `Specific`
- Query text: `WriteApiAsync`
- Focus tags: `Write`
- Relation hints: `WriteApi`, `RetryAttempt`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Mixed`
- Helpfulness score: 0,554
- Expected terms: 1/3
- Expected files: 2/2
- Useful files: 2
- Non-useful files: 5
- Noise term hits: 1
- Token budget ratio: 0,955

## Output Metrics

- Search results: 40
- Seed type: InfluxDB.Client.WriteApiAsync
- Seed member: InfluxDB.Client.WriteApiAsync.ToLineProtocolBody(IEnumerable<InfluxDB.Client.BatchWriteData>)
- Files: 7
- Blocks: 9
- Selected lines: 180
- Estimated tokens: 2482
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `InfluxDB.Client.IWriteApiAsync.WriteMeasurementAsync<TM>(TM, InfluxDB.Client.Api.Domain.WritePrecision, string, string, CancellationToken)` (Member)
- `InfluxDB.Client.IWriteApiAsync.WriteMeasurementsAsync<TM>(List<TM>, InfluxDB.Client.Api.Domain.WritePrecision, string, string, CancellationToken)` (Member)
- `InfluxDB.Client.IWriteApiAsync.WriteMeasurementsAsync<TM>(TM[], InfluxDB.Client.Api.Domain.WritePrecision, string, string, CancellationToken)` (Member)
- `InfluxDB.Client.IWriteApiAsync.WriteMeasurementsAsyncWithIRestResponse<TM>(IEnumerable<TM>, InfluxDB.Client.Api.Domain.WritePrecision, string, string, CancellationToken)` (Member)
- `InfluxDB.Client.IWriteApiAsync.WritePointAsync(InfluxDB.Client.Writes.PointData, string, string, CancellationToken)` (Member)
- `InfluxDB.Client.IWriteApiAsync.WritePointsAsync(InfluxDB.Client.Writes.PointData[], string, string, CancellationToken)` (Member)
- `InfluxDB.Client.IWriteApiAsync.WritePointsAsync(List<InfluxDB.Client.Writes.PointData>, string, string, CancellationToken)` (Member)
- `InfluxDB.Client.IWriteApiAsync.WritePointsAsyncWithIRestResponse(IEnumerable<InfluxDB.Client.Writes.PointData>, string, string, CancellationToken)` (Member)

## Selected Files

- `Client/InfluxDBClientOptions.cs`: 63/864 lines, 1 blocks
- `Client/WriteApiAsync.cs`: 61/468 lines, 3 blocks
- `Client/InfluxDB.Client.Api/Service/WriteService.cs`: 27/1055 lines, 1 blocks
- `Client/Writes/PointData.cs`: 15/718 lines, 1 blocks
- `Client/InfluxDBClient.cs`: 8/1002 lines, 1 blocks
- `Client/IDomainObjectMapper.cs`: 3/22 lines, 1 blocks
- `Client/WriteApi.cs`: 3/676 lines, 1 blocks

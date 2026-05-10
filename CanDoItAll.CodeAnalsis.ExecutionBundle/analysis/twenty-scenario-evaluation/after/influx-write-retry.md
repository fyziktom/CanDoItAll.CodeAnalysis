# influx-write-retry

## Simulated Prompt

WriteApi retries are behaving oddly. Show retry scheduling and write options around Client/WriteApi.cs.

## Simulated Agent Approach

Start at WriteApi and ask for write/retry context with RetryAttempt and WriteOptions as relation hints.

## Query

- Repository: `influxdb-client-csharp`
- Category: `Specific`
- Query text: `WriteApi`
- Focus tags: `Write`
- Relation hints: `RetryAttempt`, `WriteOptions`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Mixed`
- Helpfulness score: 0,660
- Expected terms: 2/3
- Expected files: 1/3
- Useful files: 1
- Non-useful files: 0
- Noise term hits: 0
- Token budget ratio: 0,117

## Output Metrics

- Search results: 40
- Seed type: InfluxDB.Client.WriteApi
- Seed member: InfluxDB.Client.WriteApi.WritePoint(InfluxDB.Client.Writes.PointData, string, string)
- Files: 1
- Blocks: 1
- Selected lines: 12
- Estimated tokens: 327
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `InfluxDB.Client.IWriteApi.Flush()` (Member)
- `InfluxDB.Client.IWriteApi.WriteMeasurement<TM>(TM, InfluxDB.Client.Api.Domain.WritePrecision, string, string)` (Member)
- `InfluxDB.Client.IWriteApi.WriteMeasurements<TM>(List<TM>, InfluxDB.Client.Api.Domain.WritePrecision, string, string)` (Member)
- `InfluxDB.Client.IWriteApi.WriteMeasurements<TM>(TM[], InfluxDB.Client.Api.Domain.WritePrecision, string, string)` (Member)
- `InfluxDB.Client.IWriteApi.WritePoint(InfluxDB.Client.Writes.PointData, string, string)` (Member)
- `InfluxDB.Client.IWriteApi.WritePoints(InfluxDB.Client.Writes.PointData[], string, string)` (Member)
- `InfluxDB.Client.IWriteApi.WritePoints(List<InfluxDB.Client.Writes.PointData>, string, string)` (Member)
- `InfluxDB.Client.IWriteApi.WriteRecord(string, InfluxDB.Client.Api.Domain.WritePrecision, string, string)` (Member)

## Selected Files

- `Client/WriteApi.cs`: 12/676 lines, 1 blocks

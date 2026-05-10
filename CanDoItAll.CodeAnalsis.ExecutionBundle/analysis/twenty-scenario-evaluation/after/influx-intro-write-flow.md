# influx-intro-write-flow

## Simulated Prompt

I need to add safer write batching behavior. First show me the client write flow from InfluxDBClient to write APIs and options.

## Simulated Agent Approach

Search the client facade, then ask focused context with write tags and WriteApi relation hints.

## Query

- Repository: `influxdb-client-csharp`
- Category: `Introduction`
- Query text: `InfluxDBClient`
- Focus tags: `Client`, `Write`
- Relation hints: `WriteApi`, `WriteApiAsync`, `WriteOptions`
- Depth: 2
- Intent: `TroublePath`
- Precision: `Outline`

## Score

- Rating: `Good`
- Helpfulness score: 0,750
- Expected terms: 3/3
- Expected files: 3/3
- Useful files: 0
- Non-useful files: 0
- Noise term hits: 1
- Token budget ratio: 0,237

## Output Metrics

- Search results: 40
- Seed type: InfluxDB.Client.InfluxDBClient
- Seed member: InfluxDB.Client.InfluxDBClient.DisableGzip()
- Files: 0
- Blocks: 0
- Selected lines: 0
- Estimated tokens: 831
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `InfluxDB.Client.InfluxDBClientOptions` (Type)
- `InfluxDB.Client.IInfluxDBClient.CreateService<TS>(Type)` (Member)
- `InfluxDB.Client.IInfluxDBClient.DisableGzip()` (Member)
- `InfluxDB.Client.IInfluxDBClient.EnableGzip()` (Member)
- `InfluxDB.Client.IInfluxDBClient.GetAuthorizationsApi()` (Member)
- `InfluxDB.Client.IInfluxDBClient.GetBucketsApi()` (Member)
- `InfluxDB.Client.IInfluxDBClient.GetChecksApi()` (Member)
- `InfluxDB.Client.IInfluxDBClient.GetDeleteApi()` (Member)

## Selected Files


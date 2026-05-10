# influx-point-escaping

## Simulated Prompt

PointData line protocol escaping is wrong for tags. Show PointData and its builder before patching tests.

## Simulated Agent Approach

Start at PointData, relation-hint the builder partial and tests.

## Query

- Repository: `influxdb-client-csharp`
- Category: `Specific`
- Query text: `PointData`
- Focus tags: `Write`, `Model`
- Relation hints: `PointData.Builder`, `PointDataTest`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Mixed`
- Helpfulness score: 0,609
- Expected terms: 2/3
- Expected files: 2/2
- Useful files: 1
- Non-useful files: 7
- Noise term hits: 1
- Token budget ratio: 1,389

## Output Metrics

- Search results: 40
- Seed type: InfluxDB.Client.Writes.PointData
- Seed member: InfluxDB.Client.Writes.PointData.Timestamp(DateTime, InfluxDB.Client.Api.Domain.WritePrecision)
- Files: 8
- Blocks: 13
- Selected lines: 245
- Estimated tokens: 3334
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `InfluxDB.Client.Test.PointDataBuilderTest.BuilderValuesToPoint()` (Member)
- `InfluxDB.Client.Test.PointDataBuilderTest.DateTimeMustBeUtc()` (Member)
- `InfluxDB.Client.Test.PointDataBuilderTest.FieldNullValue()` (Member)
- `InfluxDB.Client.Test.PointDataBuilderTest.HasFields()` (Member)
- `InfluxDB.Client.Test.PointDataBuilderTest.MultipleFields()` (Member)
- `InfluxDB.Client.Test.PointDataBuilderTest.ReplaceFieldValue()` (Member)
- `InfluxDB.Client.Test.PointDataBuilderTest.ReplaceTagValue()` (Member)
- `InfluxDB.Client.Test.PointDataBuilderTest.ReplaceTagValueInNewPoint()` (Member)

## Selected Files

- `Client/Internal/MeasurementMapper.cs`: 133/178 lines, 1 blocks
- `Client.Test/MeasurementMapperTest.cs`: 33/172 lines, 2 blocks
- `Client/Writes/PointData.cs`: 32/718 lines, 3 blocks
- `Client/WriteApi.cs`: 26/676 lines, 3 blocks
- `Client.Core/Flux/Domain/FluxRecord.cs`: 6/122 lines, 1 blocks
- `Client.Core/Flux/Internal/FluxResultMapper.cs`: 6/269 lines, 1 blocks
- `Client/Internal/DefaultDomainObjectMapper.cs`: 6/32 lines, 1 blocks
- `Client/IDomainObjectMapper.cs`: 3/22 lines, 1 blocks

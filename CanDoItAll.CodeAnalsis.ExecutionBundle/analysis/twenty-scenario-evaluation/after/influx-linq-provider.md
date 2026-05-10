# influx-linq-provider

## Simulated Prompt

LINQ query generation needs a fix. Show InfluxDBQueryable and the provider/expression path around it.

## Simulated Agent Approach

Search the LINQ queryable and use relation hints for provider and expression terms.

## Query

- Repository: `influxdb-client-csharp`
- Category: `Specific`
- Query text: `InfluxDBQueryable`
- Focus tags: `Linq`, `Query`
- Relation hints: `Expression`, `Provider`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Mixed`
- Helpfulness score: 0,703
- Expected terms: 2/3
- Expected files: 2/2
- Useful files: 2
- Non-useful files: 3
- Noise term hits: 1
- Token budget ratio: 0,773

## Output Metrics

- Search results: 40
- Seed type: InfluxDB.Client.Linq.InfluxDBQueryable<T>
- Seed member: InfluxDB.Client.Linq.InfluxDBQueryable<T>.Queryable(string, string, InfluxDB.Client.QueryApi, InfluxDB.Client.Linq.IMemberNameResolver, InfluxDB.Client.Linq.QueryableOptimizerSettings)
- Files: 5
- Blocks: 9
- Selected lines: 105
- Estimated tokens: 2011
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `Client.Linq.Test.ItInfluxDBQueryableTest.ASyncQuery()` (Member)
- `Client.Linq.Test.ItInfluxDBQueryableTest.ASyncQueryConfiguration()` (Member)
- `Client.Linq.Test.ItInfluxDBQueryableTest.ASyncQueryFirst()` (Member)
- `Client.Linq.Test.ItInfluxDBQueryableTest.After()` (Member)
- `Client.Linq.Test.ItInfluxDBQueryableTest.AggregateFunction()` (Member)
- `Client.Linq.Test.ItInfluxDBQueryableTest.AggregateFunctionAsync()` (Member)
- `Client.Linq.Test.ItInfluxDBQueryableTest.QueryAggregateWindow()` (Member)
- `Client.Linq.Test.ItInfluxDBQueryableTest.QueryAll()` (Member)

## Selected Files

- `Client.Linq/InfluxDBQueryable.cs`: 41/306 lines, 5 blocks
- `Client/InfluxDB.Client.Api/Domain/Query.cs`: 28/229 lines, 1 blocks
- `Client/QueryApiSync.cs`: 23/219 lines, 1 blocks
- `Client/QueryApi.cs`: 10/808 lines, 1 blocks
- `Client.Linq/IMemberNameResolver.cs`: 3/90 lines, 1 blocks

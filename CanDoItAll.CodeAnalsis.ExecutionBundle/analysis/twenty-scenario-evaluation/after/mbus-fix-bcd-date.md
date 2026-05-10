# mbus-fix-bcd-date

## Simulated Prompt

Fix a date decoding bug near MbusParser/Drivers/BcdDateTimeParser.cs. I need its registry usage and related parser examples.

## Simulated Agent Approach

Search the named parser and ask for direct parser/registry context.

## Query

- Repository: `MBusParser`
- Category: `Specific`
- Query text: `BcdDateTimeParser`
- Focus tags: `Parser`
- Relation hints: `DateTimeParserRegistry`, `IDateTimeParser`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Good`
- Helpfulness score: 0,747
- Expected terms: 1/3
- Expected files: 1/1
- Useful files: 1
- Non-useful files: 0
- Noise term hits: 0
- Token budget ratio: 0,211

## Output Metrics

- Search results: 3
- Seed type: MBus.Drivers.BcdDateTimeParser
- Seed member: MBus.Drivers.BcdDateTimeParser.Parse(byte[])
- Files: 1
- Blocks: 2
- Selected lines: 24
- Estimated tokens: 379
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `MBus.Drivers.BcdDateTimeParser.Parse(byte[])` (Member)
- `MBus.Drivers.BcdDateTimeParser.ParseBcdByte(byte)` (Member)
- `MBus.Drivers.BcdDateTimeParser` (Type)

## Selected Files

- `MbusParser/Drivers/BcdDateTimeParser.cs`: 24/32 lines, 2 blocks

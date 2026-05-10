# mbus-intro-parser

## Simulated Prompt

I need to add support for a new M-Bus frame variant. Before editing, show me the main parsing path and where telegram records are created.

## Simulated Agent Approach

Build a snapshot, search for the public parser entry point, then ask focused context for a bounded protocol/parser overview.

## Query

- Repository: `MBusParser`
- Category: `Introduction`
- Query text: `MBusParser`
- Focus tags: `Protocol`, `Parser`
- Relation hints: `MBusTelegram`, `VariableDataRecord`
- Depth: 2
- Intent: `TroublePath`
- Precision: `Outline`

## Score

- Rating: `Good`
- Helpfulness score: 0,800
- Expected terms: 3/3
- Expected files: 3/3
- Useful files: 0
- Non-useful files: 0
- Noise term hits: 0
- Token budget ratio: 0,108

## Output Metrics

- Search results: 40
- Seed type: MBus.MBusParser
- Seed member: MBus.MBusParser.CalculateCrc(System.Collections.Generic.IEnumerable<byte>)
- Files: 0
- Blocks: 0
- Selected lines: 0
- Estimated tokens: 303
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `MBus.MBusParser.CalculateCrc(System.Collections.Generic.IEnumerable<byte>)` (Member)
- `MBus.MBusParser.ComputeCrc(System.Collections.Generic.IEnumerable<byte>, int, int, int)` (Member)
- `MBus.MBusParser.DecryptBytes(byte[], byte[], System.Collections.Generic.IEnumerable<byte>, MBus.Header.EncryptionScheme)` (Member)
- `MBus.MBusParser.DecryptPayload(MBus.Header.MBusHeader, System.Collections.Generic.List<byte>, byte[])` (Member)
- `MBus.MBusParser.DecryptionSuccessful(byte[], MBus.Header.EncryptionScheme)` (Member)
- `MBus.MBusParser.GenerateCounterModeIV(MBus.Header.MBusHeader, byte[]?, byte)` (Member)
- `MBus.MBusParser.GenerateGenericIV(MBus.Header.MBusHeader)` (Member)
- `MBus.MBusParser.GenerateIv(MBus.Header.MBusHeader)` (Member)

## Selected Files


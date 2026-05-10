# mbus-enum-utils-dif

## Simulated Prompt

EnumUtils seems to map raw bytes to enum values. Show me how it is used by DIF/VIF fields before I tighten parsing.

## Simulated Agent Approach

Start from EnumUtils and narrow by data information field relation hints.

## Query

- Repository: `MBusParser`
- Category: `Specific`
- Query text: `EnumUtils`
- Focus tags: `Protocol`
- Relation hints: `DataInformationField`, `PrimaryValueInformationField`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Poor`
- Helpfulness score: 0,323
- Expected terms: 1/3
- Expected files: 1/3
- Useful files: 1
- Non-useful files: 4
- Noise term hits: 1
- Token budget ratio: 0,483

## Output Metrics

- Search results: 3
- Seed type: MBus.Helpers.EnumUtils
- Seed member: MBus.Helpers.EnumUtils.GetEnumOrDefault<T>(object, T)
- Files: 5
- Blocks: 5
- Selected lines: 55
- Estimated tokens: 869
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `MBus.Helpers.EnumUtils.GetEnumOrDefault<T>(object, T)` (Member)
- `MBus.Helpers.EnumUtils.TryGetEnum<T>(object)` (Member)
- `MBus.Helpers.EnumUtils` (Type)

## Selected Files

- `MbusParser/MBusParser.cs`: 21/498 lines, 1 blocks
- `MbusParser/Header/Configuration.cs`: 17/51 lines, 1 blocks
- `MbusParser/Helpers/EnumUtils.cs`: 11/49 lines, 1 blocks
- `MbusParser/Header/MBusHeader.cs`: 3/224 lines, 1 blocks
- `MbusParser/MBusTelegram.cs`: 3/46 lines, 1 blocks

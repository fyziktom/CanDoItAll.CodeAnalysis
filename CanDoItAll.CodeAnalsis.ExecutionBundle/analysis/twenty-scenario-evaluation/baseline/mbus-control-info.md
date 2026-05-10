# mbus-control-info

## Simulated Prompt

Control information code lookup looks suspicious. Show ControlInformationLookup with the enum/type it maps to.

## Simulated Agent Approach

Use exact-ish focused context on the lookup helper with relation hints for the enum.

## Query

- Repository: `MBusParser`
- Category: `Specific`
- Query text: `ControlInformationLookup`
- Focus tags: `Protocol`
- Relation hints: `ControlInformation`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Good`
- Helpfulness score: 0,850
- Expected terms: 2/2
- Expected files: 1/1
- Useful files: 6
- Non-useful files: 2
- Noise term hits: 0
- Token budget ratio: 2,000

## Output Metrics

- Search results: 4
- Seed type: MBus.Header.ControlInformationLookup
- Seed member: MBus.Header.ControlInformationLookup.Find(byte)
- Files: 8
- Blocks: 13
- Selected lines: 381
- Estimated tokens: 5806
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `MBus.Header.ControlInformationLookup.Find(byte)` (Member)
- `MBus.Header.ControlInformationLookup.HeaderLength(byte)` (Member)
- `MBus.Header.ControlInformationLookup._lookup` (Member)
- `MBus.Header.ControlInformationLookup` (Type)

## Selected Files

- `MbusParser/MBusParser.cs`: 286/498 lines, 3 blocks
- `MbusParser/Header/ControlInformationLookup.cs`: 38/87 lines, 2 blocks
- `MbusParser/DataRecord/DataRecordHeader/ValueInformationBlock/PrimaryValueInformationField.cs`: 24/232 lines, 1 blocks
- `MbusParser/DataRecord/DataBlock.cs`: 12/431 lines, 1 blocks
- `MbusParser/Header/MBusHeader.cs`: 9/224 lines, 3 blocks
- `MbusParser/DataRecord/DataRecordHeader/ValueInformationBlock/Extension/ValueInformationExtensionField.cs`: 6/22 lines, 1 blocks
- `MbusParser/DataRecord/DataRecordHeader/DataInformationBlock/DataInformationExtensionField.cs`: 3/61 lines, 1 blocks
- `MbusParser/DataRecord/DataRecordHeader/DataInformationBlock/DataInformationField.cs`: 3/94 lines, 1 blocks

# mbus-vif-extension

## Simulated Prompt

I need to add a new VIF extension value. Show me ValueInformationExtensionField and existing FB/FD extension handling.

## Simulated Agent Approach

Search the extension field and ask for related extension classes.

## Query

- Repository: `MBusParser`
- Category: `Specific`
- Query text: `ValueInformationExtensionField`
- Focus tags: `Protocol`
- Relation hints: `FBValueInformationExtensionField`, `FDValueInformationExtensionField`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Mixed`
- Helpfulness score: 0,613
- Expected terms: 1/3
- Expected files: 2/2
- Useful files: 1
- Non-useful files: 2
- Noise term hits: 0
- Token budget ratio: 0,483

## Output Metrics

- Search results: 25
- Seed type: MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension.ValueInformationExtensionField
- Seed member: MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension.ValueInformationExtensionField.ValueInformationExtensionField(byte)
- Files: 3
- Blocks: 3
- Selected lines: 55
- Estimated tokens: 1063
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension.FBValueInformationExtensionField.DetermineTypeAndMultiplier()` (Member)
- `MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension.FBValueInformationExtensionField.FBValueInformationExtensionField(byte)` (Member)
- `MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension.FBValueInformationExtensionField.Parse()` (Member)
- `MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension.FBValueInformationExtensionField.SetType(byte)` (Member)
- `MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension.FBValueInformationExtensionField.Type` (Member)
- `MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension.FDValueInformationExtensionField.DetermineTypeAndMultiplier()` (Member)
- `MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension.FDValueInformationExtensionField.FDValueInformationExtensionField(byte)` (Member)
- `MBus.DataRecord.DataRecordHeader.ValueInformationBlock.Extension.FDValueInformationExtensionField.Parse()` (Member)

## Selected Files

- `MbusParser/DataRecord/DataBlock.cs`: 46/431 lines, 1 blocks
- `MbusParser/DataRecord/DataRecordHeader/ValueInformationBlock/Extension/ValueInformationExtensionField.cs`: 6/22 lines, 1 blocks
- `MbusParser/DataRecord/VariableDataRecord.cs`: 3/59 lines, 1 blocks

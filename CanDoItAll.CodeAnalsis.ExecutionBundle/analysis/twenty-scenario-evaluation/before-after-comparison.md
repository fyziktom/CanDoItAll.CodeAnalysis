# Scenario evaluation before/after comparison

## Aggregate

- Scenarios: 22
- Improved: 9
- Regressed: 0
- Unchanged: 13
- Average helpfulness delta: 0,280
- Estimated token delta: 16135
- Selected line delta: 946
- Non-useful file delta: 24

## Baseline Aggregate

- Scenarios: 22
- Introduction scenarios: 6
- Average helpfulness: 0,434
- Average term coverage: 0,394
- Average file coverage: 0,561
- Average non-useful file ratio: 0,712
- Average token budget ratio: 0,771
- Ratings: 9 good, 3 mixed, 1 poor, 9 failed
- Estimated tokens: 21877
- Selected lines: 1096
- Useful files: 23
- Non-useful files: 15

## After Aggregate

- Scenarios: 22
- Introduction scenarios: 6
- Average helpfulness: 0,714
- Average term coverage: 0,701
- Average file coverage: 0,902
- Average non-useful file ratio: 0,552
- Average token budget ratio: 0,643
- Ratings: 12 good, 9 mixed, 1 poor, 0 failed
- Estimated tokens: 38012
- Selected lines: 2042
- Useful files: 35
- Non-useful files: 39

## Scenario Deltas

| Scenario | Repo | Category | Score delta | Token delta | Line delta | Non-useful file delta | Rating |
| --- | --- | --- | ---: | ---: | ---: | ---: | --- |
| `influx-client-options` | influxdb-client-csharp | Specific | 0,840 | 370 | 9 | 0 | Failed -> Good |
| `influx-delete-predicate` | influxdb-client-csharp | Specific | 0,802 | 2525 | 190 | 4 | Failed -> Good |
| `influx-intro-write-flow` | influxdb-client-csharp | Introduction | 0,750 | 831 | 0 | 0 | Failed -> Good |
| `influx-linq-provider` | influxdb-client-csharp | Specific | 0,703 | 2011 | 105 | 3 | Failed -> Mixed |
| `influx-write-retry` | influxdb-client-csharp | Specific | 0,660 | 327 | 12 | 0 | Failed -> Mixed |
| `influx-intro-query-flow` | influxdb-client-csharp | Introduction | 0,655 | 560 | 0 | 0 | Failed -> Mixed |
| `influx-point-escaping` | influxdb-client-csharp | Specific | 0,609 | 3334 | 245 | 7 | Failed -> Mixed |
| `influx-query-cancel` | influxdb-client-csharp | Specific | 0,583 | 3251 | 205 | 5 | Failed -> Mixed |
| `influx-write-async` | influxdb-client-csharp | Specific | 0,554 | 2482 | 180 | 5 | Failed -> Mixed |
| `mbus-intro-parser` | MBusParser | Introduction | 0,000 | 459 | 0 | 0 | Good -> Good |
| `mbus-intro-decryption` | MBusParser | Introduction | 0,000 | -11 | 0 | 0 | Mixed -> Mixed |
| `mbus-intro-record-model` | MBusParser | Introduction | 0,000 | 0 | 0 | 0 | Good -> Good |
| `cando-intro-canvas` | CanDoItAll | Introduction | 0,000 | 0 | 0 | 0 | Mixed -> Mixed |
| `mbus-fix-bcd-date` | MBusParser | Specific | 0,000 | -4 | 0 | 0 | Good -> Good |
| `mbus-enum-utils-dif` | MBusParser | Specific | 0,000 | 0 | 0 | 0 | Poor -> Poor |
| `mbus-aes-ctr` | MBusParser | Specific | 0,000 | 0 | 0 | 0 | Good -> Good |
| `mbus-control-info` | MBusParser | Specific | 0,000 | 0 | 0 | 0 | Good -> Good |
| `mbus-vif-extension` | MBusParser | Specific | 0,000 | 0 | 0 | 0 | Mixed -> Mixed |
| `cando-db-save` | CanDoItAll | Specific | 0,000 | 0 | 0 | 0 | Good -> Good |
| `cando-clock-workbench` | CanDoItAll | Specific | 0,000 | 0 | 0 | 0 | Good -> Good |
| `cando-storage-registry` | CanDoItAll | Specific | 0,000 | 0 | 0 | 0 | Good -> Good |
| `cando-canvas-mark-applied` | CanDoItAll | Specific | 0,000 | 0 | 0 | 0 | Good -> Good |

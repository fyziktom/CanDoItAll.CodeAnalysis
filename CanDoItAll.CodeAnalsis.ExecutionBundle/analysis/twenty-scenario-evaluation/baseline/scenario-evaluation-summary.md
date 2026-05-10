# Scenario evaluation baseline

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

## Snapshots

- `cando-full`: 67 projects, 3731 types, 31045 members, 116093 ms
- `influx-full`: 0 projects, 0 types, 0 members, 7467 ms
- `mbus-full`: 3 projects, 60 types, 559 members, 5787 ms

## Scenarios

| Scenario | Repo | Category | Rating | Score | Terms | Files | Tokens | Non-useful files |
| --- | --- | --- | --- | ---: | ---: | ---: | ---: | ---: |
| `cando-canvas-mark-applied` | CanDoItAll | Specific | Good | 0,873 | 2/3 | 3 | 1184 | 0 |
| `cando-clock-workbench` | CanDoItAll | Specific | Good | 1,000 | 2/2 | 1 | 356 | 0 |
| `cando-db-save` | CanDoItAll | Specific | Good | 0,759 | 2/3 | 7 | 2905 | 4 |
| `cando-intro-canvas` | CanDoItAll | Introduction | Mixed | 0,547 | 1/3 | 0 | 433 | 0 |
| `cando-storage-registry` | CanDoItAll | Specific | Good | 0,825 | 2/2 | 8 | 6596 | 3 |
| `influx-client-options` | influxdb-client-csharp | Specific | Failed | 0,000 | 0/0 | 0 | 0 | 0 |
| `influx-delete-predicate` | influxdb-client-csharp | Specific | Failed | 0,000 | 0/0 | 0 | 0 | 0 |
| `influx-intro-query-flow` | influxdb-client-csharp | Introduction | Failed | 0,000 | 0/0 | 0 | 0 | 0 |
| `influx-intro-write-flow` | influxdb-client-csharp | Introduction | Failed | 0,000 | 0/0 | 0 | 0 | 0 |
| `influx-linq-provider` | influxdb-client-csharp | Specific | Failed | 0,000 | 0/0 | 0 | 0 | 0 |
| `influx-point-escaping` | influxdb-client-csharp | Specific | Failed | 0,000 | 0/0 | 0 | 0 | 0 |
| `influx-query-cancel` | influxdb-client-csharp | Specific | Failed | 0,000 | 0/0 | 0 | 0 | 0 |
| `influx-write-async` | influxdb-client-csharp | Specific | Failed | 0,000 | 0/0 | 0 | 0 | 0 |
| `influx-write-retry` | influxdb-client-csharp | Specific | Failed | 0,000 | 0/0 | 0 | 0 | 0 |
| `mbus-aes-ctr` | MBusParser | Specific | Good | 0,873 | 2/3 | 2 | 578 | 0 |
| `mbus-control-info` | MBusParser | Specific | Good | 0,850 | 2/2 | 8 | 5806 | 2 |
| `mbus-enum-utils-dif` | MBusParser | Specific | Poor | 0,323 | 1/3 | 5 | 869 | 4 |
| `mbus-fix-bcd-date` | MBusParser | Specific | Good | 0,747 | 1/3 | 1 | 383 | 0 |
| `mbus-intro-decryption` | MBusParser | Introduction | Mixed | 0,547 | 1/3 | 0 | 422 | 0 |
| `mbus-intro-parser` | MBusParser | Introduction | Good | 0,800 | 3/3 | 0 | 303 | 0 |
| `mbus-intro-record-model` | MBusParser | Introduction | Good | 0,800 | 4/4 | 0 | 979 | 0 |
| `mbus-vif-extension` | MBusParser | Specific | Mixed | 0,613 | 1/3 | 3 | 1063 | 2 |

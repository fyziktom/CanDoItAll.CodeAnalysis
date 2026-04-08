# Implementation prompt

Implement the frozen symbol-tool set with the smallest maintainable change:

- add explicit query and response contracts
- implement the service methods over the existing snapshot facts
- keep deterministic ordering and capped result counts
- reuse shared source-excerpt logic where it improves maintainability
- add one symbol explorer UI route
- add unit, web, and rerun harness coverage

Do not add a second analysis pipeline.

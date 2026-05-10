# Twenty-scenario real-world evaluation request

Captured on 2026-05-09 from the user request.

## Raw Request

Analyze the CodeAnalytics output on at least 20 different real-world-looking problems. For each scenario, simulate a prompt and simulate the standard agent approach for asking for details.

At least 5 scenarios should be first-step project introduction scenarios where an agent needs main information about larger sections of the code before doing work. The remaining scenarios should be more specific repair or implementation tasks where the prompt includes stronger instructions, such as named files or symbols.

Use read-only target repositories such as:

- `C:\repositories\MBusParser`
- `C:\repositories\influxdb-client-csharp`
- `C:\repositories\CanDoItAll`

Do not change those target repositories.

Analyze the outputs from CodeAnalytics and judge:

- how helpful the context was,
- how much context was not useful,
- what metrics indicate overload or under-selection,
- what concrete improvements should be made to CodeAnalytics.

Record the scenario set, baseline results, improvement rationale, implementation proof, retest results, and conclusions in this bundle using the CanDoItAll bundle workflow.

## Interpretation

- All generated evaluation artifacts must stay under `C:\repositories\CanDoItAll.CodeAnalsis`.
- External repositories are read-only inputs only.
- The benchmark must be repeatable so future tuning can compare the same simulated prompts.
- Improvements must be driven by measured output quality, not by anecdotal inspection alone.

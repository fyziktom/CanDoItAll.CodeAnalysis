# Forbidden patterns for SB-00A — Current CanDoItAll compatibility baseline

- Copy-pasting `McpToolEnvelope`, `ToolInvocationException`, or host settings types into the standalone libraries.
- Using incorrect or mixed driver prefixes such as `analytics_*`, `mcp_*`, or `codeanalysis_*` after the naming map is frozen.
- Ignoring current host-repo install/config patterns when defining future integration docs.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.

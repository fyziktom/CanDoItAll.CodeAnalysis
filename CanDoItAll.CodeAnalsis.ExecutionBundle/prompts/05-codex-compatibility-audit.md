# Compatibility audit prompt

Use this prompt when you want Codex to do a focused compatibility pass against the current CanDoItAll MCP ecosystem.

## Task

Audit the standalone `CanDoItAll.CodeAnalsis` repo against the current CanDoItAll host repo and answer:

- does the naming map still hold?
- is `.slnx` still canonical?
- did any host-only MCP concern leak into the standalone libraries?
- is the future `CanDoItAll.Mcp.CodeAnalytics` driver still thin?
- which exact host-repo files would be updated during transplantation?
- do the future settings/tool names still match current conventions?

## Expected output

- findings first,
- specific file references,
- blocking vs non-blocking issues,
- concrete remediation steps.

# Forbidden patterns for SB-11 — Blazor SSR UI shell and dashboard

- Packing application orchestration logic into Razor components.
- Introducing Radzen or a large component dependency without a strong reason.
- Creating giant page files with many unrelated concerns.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.

# Forbidden patterns for SB-01 — Canonical domain model and contracts

- Mutable DTO bags with ad-hoc dictionaries for core snapshot sections.
- Encoding diagnostics as free-form strings without severity/category/source.
- Reintroducing MCP-specific wrapper types into the libraries.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.

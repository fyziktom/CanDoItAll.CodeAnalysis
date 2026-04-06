# Review pass prompt

Run this after the refactor pass.

## Review checklist

1. Does the repo still respect the naming map?
2. Is `.slnx` still canonical?
3. Is the application layer still thin-driver-friendly?
4. Were facts and insights kept separate?
5. Can the future `CanDoItAll.Mcp.CodeAnalytics` driver be added with thin glue only?
6. Were current host-repo MCP patterns respected where useful?
7. Are build/test/format/validation commands green?
8. Are key exports and examples present?
9. Is the UI still SSR-first and lightweight?
10. Are comments English-only and sparse?

## Output

Produce a concise review report with:
- findings first,
- any blocking issues,
- non-blocking follow-ups,
- final closure recommendation.

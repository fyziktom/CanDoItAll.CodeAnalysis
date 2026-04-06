# SB-14 — Tests hardening, repo-local Codex assets, and final refactor

## Objective
Perform the final quality pass: strengthen tests, split oversized files, clean naming/folders, and leave behind a repo shape that is both maintainable and portable into CanDoItAll.

## Milestone / priority / actor
- Milestone: `M5`
- Priority: `P0`
- Primary actor: `QA and refactor maintainer`

## Depends on
- SB-00
- SB-00A
- SB-01
- SB-02
- SB-03
- SB-04
- SB-05
- SB-06
- SB-07
- SB-08
- SB-09
- SB-10
- SB-11
- SB-12
- SB-13

## Read first
- overview/08-quality-gates.md
- overview/09-testing-strategy.md
- overview/18-execution-order-and-closure-evidence.md
- prompts/02-codex-refactor-pass.md
- prompts/03-codex-review-pass.md

## Current CanDoItAll reference files to inspect
- codex/README.md
- .codex/agents/arch-mapper.toml
- .codex/agents/canonical-model-skeptic.toml
- .codex/agents/runtime-validator.toml

## In scope
- Run the mandatory refactor pass focused on file length, folder hygiene, naming clarity, and boundary cleanliness.
- Strengthen unit/integration/web/architecture/golden tests and fix any gaps.
- Optionally add minimal repo-local Codex assets or documentation placeholders that mirror the host repo style.
- Complete final closure evidence and handoff notes.

## Out of scope
- No large-scope redesigns unless a blocker is discovered.
- No premature MCP server implementation.

## Compatibility rules specific to this subbundle
- The finished repo should look easy to transplant into the current CanDoItAll source tree and its Codex workflow.
- If repo-local Codex assets are added, keep them thin and obviously portable.

## Expected deliverables
- Clean final project/file structure.
- Strengthened tests and final golden outputs.
- Refactor report, review report, and closure checklist.

# Prompt for SB-14 — Tests hardening, repo-local Codex assets, and final refactor

You are implementing **SB-14** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Perform the final quality pass: strengthen tests, split oversized files, clean naming/folders, and leave behind a repo shape that is both maintainable and portable into CanDoItAll.

## Read before coding
- overview/08-quality-gates.md
- overview/09-testing-strategy.md
- overview/18-execution-order-and-closure-evidence.md
- prompts/02-codex-refactor-pass.md
- prompts/03-codex-review-pass.md
- subbundles/SB-14-tests-hardening-final-refactor/01-scope.md
- subbundles/SB-14-tests-hardening-final-refactor/03-checklist.md
- subbundles/SB-14-tests-hardening-final-refactor/04-validation.md
- subbundles/SB-14-tests-hardening-final-refactor/05-forbidden-patterns.md
- subbundles/SB-14-tests-hardening-final-refactor/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- codex/README.md
- .codex/agents/arch-mapper.toml
- .codex/agents/canonical-model-skeptic.toml
- .codex/agents/runtime-validator.toml

## Required implementation steps
- Audit all production files for size and cohesion; split anything too large or multi-purpose.
- Run the complete validation matrix and resolve failures.
- Ensure comments remain rare and English-only; avoid gratuitous XML docs.
- Add or update repo-local `codex/README.md` and optionally `.codex/agents` placeholders if they help future use.
- Produce a final handoff summary that points directly to the future MCP integration seam.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not close the bundle if any required validation is red.
- Do not close the bundle if long-file or folder-hygiene debt was left unresolved without justification.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.

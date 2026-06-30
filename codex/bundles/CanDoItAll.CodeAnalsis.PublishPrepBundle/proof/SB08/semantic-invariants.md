# SB08 Semantic Invariants

## Invariant Contract

- Invariant ID: SB08-INV-001

| Field | Value |
| --- | --- |
| Invariant ID | `SB08-INV-001` |
| Source raw note | `IN-001`, `IN-002`, `IN-003`, `IN-004`, `IN-005`, `IN-006`, `IN-007`, `IN-008`, `IN-009` |
| Expected behavior | Final docs, workbook, packages, and proof close every raw input with validated evidence. |
| Disallowed shallow implementation | Do not mark closure complete with stale README/package docs, unverified workbook state, or packages that leak non-shipping content. |
| Failing-first test | Process/non-production exemption: SB08 is documentation and closure; red-team checks in `bundle://proof/SB08/verifier-red-team.md` define the negative cases. |
| Passing test | `bundle://proof/SB08/transcripts/build.txt`, `bundle://proof/SB08/transcripts/test-unit.txt`, and final package inspection transcripts. |
| Changed source files | `repo://README.md`, `repo://architecture/adrs`, `repo://reference`, `repo://codex/validation-matrix.md`, `bundle://outputs/publishing-prep-checklist.xlsx`. |
| Production assertions | Package README and nuspec metadata match shipped source and do not claim unsupported EF/runtime/UI behavior. |
| Red-team negative case | Package forbidden-content scan rejects Web/tests/fixtures/proof/local paths in final packages. |
| Downstream dependency check | Completed-stage validator and final execution report consume this closure state. |

- Documentation only claims behavior proven by source changes, tests, browser proof, or package inspection.
- The README packaged into NuGet artifacts distinguishes reusable libraries from the desktop Web sandbox.
- EF Core documentation remains static-analyzer scoped and does not claim runtime query tuning.
- Desktop sandbox documentation remains desktop-large scoped and does not claim small/medium responsive polish.
- The future MCP driver remains reference-only; no host runtime contracts were copied into the engine.
- The final package set contains exactly eight reusable library packages.
- Raw inputs have final closure statuses with proof citations in the execution report.

# Implementation Prompt

Use this prompt when executing any subbundle in this bundle.

```text
Implement this subbundle only.

Before editing:
- Reopen the bundle README, plan, this subbundle README, traceability matrix, and execution report.
- Confirm prerequisites and record the entry gate result in reviews/01-execution-report.md.
- Inspect current repo files referenced by repo:// paths; do not trust stale assumptions.
- Preserve unrelated user changes.

Execution rules:
- Make the smallest coherent change set that satisfies the subbundle objective.
- Keep public contracts, stable IDs, export paths, and behavior compatible unless this subbundle explicitly owns a contract change.
- Do not tune small or medium sandbox UI layouts; validate large desktop screens first.
- For performance work, measure or scenario-test before replacing readable code.
- For EF work, distinguish production analyzer behavior from runtime EF query behavior.

Proof:
- Capture command transcripts under proof/SBxx/transcripts/.
- Critical subbundles must create proof/SBxx/manifest.md and proof/SBxx/semantic-invariants.md or .json.
- Critical semantic proof must include shallow-pass trap, adversarial negative proof, semantic positive proof, anti-stub audit, raw-note literal closure, changed-file hashes, and downstream dependency check.
- UI subbundles must capture large-screen browser evidence and screenshot review answers.

Stop conditions:
- Stop and mark Blocked if the progression gate cannot honestly pass.
- Reopen prerequisite subbundles when later evidence invalidates an earlier foundation.
```

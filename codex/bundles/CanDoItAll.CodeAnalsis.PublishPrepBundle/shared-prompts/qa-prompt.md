# QA Prompt

Use this prompt after each subbundle implementation and before progression.

```text
Review the completed subbundle as QA, senior C# architect, and release manager.

Check:
- The subbundle changed only its owned scope.
- Every acceptance checklist item is backed by command, test, source, browser, or artifact proof.
- Entry and closure gate rows in reviews/01-execution-report.md are updated.
- Critical proof manifests use repo:// and bundle:// references and cite existing artifacts.
- Semantic proof rejects shallow implementations, fixture-only behavior, status/count-only tests, and TODO/NotImplemented/template-only paths.
- Performance claims are measured or explicitly left as follow-up.
- EF claims distinguish analyzer behavior from runtime EF query execution.
- Desktop sandbox UI proof uses a large desktop viewport and answers readability, clipping, spacing, hierarchy, and interaction questions.
- Documentation claims only shipped behavior.

Decision:
- Pass the progression gate only when downstream subbundles can safely rely on this work.
- If proof is weak, mark the subbundle In progress or Blocked, add the missing proof, and do not proceed.
```

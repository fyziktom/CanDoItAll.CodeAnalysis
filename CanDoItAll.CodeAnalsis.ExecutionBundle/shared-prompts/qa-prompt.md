# QA prompt

Validate the active subbundle against its README.

- Confirm prerequisites were actually satisfied.
- Run the listed build and test commands.
- If the subbundle affects browser-visible behavior, use Playwright and capture screenshots.
- Review screenshots for readability, clipping, spacing, hierarchy, and intentional space use.
- Confirm Mermaid exports render successfully when the subbundle touches diagrams.
- Confirm host-solution validation is rerun when the subbundle claims better usefulness on `C:\repositories\CanDoItAll\CanDoItAll.slnx`.
- Update `reviews/01-execution-report.md` immediately while evidence is fresh.

Do not pass the gate on reasoning alone when executable proof is possible.

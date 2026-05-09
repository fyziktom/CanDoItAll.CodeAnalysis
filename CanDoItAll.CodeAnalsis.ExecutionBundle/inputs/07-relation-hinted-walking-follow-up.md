# Relation-hinted walking follow-up

Captured on 2026-05-09 from the user request to review and improve the code-analysis tool used by `CanDoItAll.Mcp.CodeAnalytics`.

## Raw notes

- Large projects still overload agent context too easily.
- Whole-solution scans should be used only for deliberate cases, not as the default way to answer narrow questions.
- The replacement for `C:\repositories\SharpToolsMCP` must preserve SharpTools-style narrow symbol navigation and add smarter context preparation for agents.
- Agents need advanced walking through code from a specific function, class, or helper.
- The walk must support tags such as `db` or `EntityFramework` so usages can be biased toward persistence instead of unrelated UI callers.
- The walk must also support related functions, classes, or components. Example: ask about a helper plus a related Razor component so only the helper usages connected to that component are emphasized.
- Depth must control quality and relevance, not just quantity.
- The tool surface and the agent skill must work together as a bundle.
- Add or improve a basic UI so analysis can be driven without restarting MCP servers.
- Testing must be real and results must be quantified so tuning can be evaluated instead of guessed.

## Scope interpretation

- This follow-up reopens the existing execution bundle because the current code already contains focused context, tags, depth, a lab page, SharpTools-style symbol tools, and a comparison harness.
- The missing primitive is relation-hinted focused walking: a second, explicit focus axis that narrows helper usages to named related symbols or areas.
- The implementation must expose relation hints through application contracts, the lab UI, the host MCP input model, comparison harness metrics, and agent skill guidance.
- Source editing and SharpTools modification parity remain out of scope for this code-analysis pass.

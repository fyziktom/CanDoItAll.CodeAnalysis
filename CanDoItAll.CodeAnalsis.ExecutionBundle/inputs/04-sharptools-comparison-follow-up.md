# Raw follow-up request

- Analyze the focused-context feature against SharpTools MCP.
- Simulate a few searches in `C:\repositories\CanDoItAll\CanDoItAll.slnx`:
  - one related to database work,
  - one related to common helpers,
  - one related to UI work.
- When preparing the test cases, define how to judge whether the result was good or not.
- Compare efficiency of the focused-context flow versus SharpTools:
  - not only content amount,
  - also whether the content is actually helpful,
  - and how much noise it contains.
- Based on that analysis, propose improvements.
- Use the bundle workflow to prepare and execute those improvements.
- Include generic refactoring that improves code readability, structure, and standard best practice.

## Direct comparison evidence captured during reopen

- Database case:
  - Query: `AppDbContext`
  - Project scope: `CanDoItAll.Infrastructure`
  - Tags: `Db`
  - Focused-context result: correct seed, but `622` selected lines across `8` files and `15` blocks, with near-full-file excerpts in non-seed files.
- Common helper case:
  - Query: `IClock`
  - Project scope: whole solution
  - Tags: `Service`
  - Focused-context result: hard failure in the lab with duplicate key `../../Users/lucys/.nuget/packages/microsoft.net.test.sdk/17.14.1/build/net8.0/Microsoft.NET.Test.Sdk.Program.cs`.
- UI case:
  - Query: `CanvasSceneHost`
  - Project scope: `CanDoItAll.Components.CanvasLib`
  - Tags: `Ui`
  - Focused-context result: strong first-pass output with `98` selected lines across `3` files and `8` blocks.

## SharpTools baseline from the same reopen

- Database case required manual search plus follow-up inspection because the first search returned many call sites without choosing the useful neighborhood automatically.
- Common helper case exposed the contract and implementation precisely, but consumer understanding still required manual follow-up selection across many modules.
- UI case was efficient once the exact symbol was known, but still depended on the operator to choose the next definitions to inspect.

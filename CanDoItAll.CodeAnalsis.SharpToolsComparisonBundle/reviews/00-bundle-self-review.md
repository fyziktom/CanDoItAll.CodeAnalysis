# Bundle self review

- The bundle is intentionally analysis-only and does not mix in implementation scope.
- The chosen comparison dimensions are explicit: usefulness, noise, tokens, calls, and time.
- The biggest fairness risk is different setup models, so setup cost is separated from warm per-scenario comparison.
- The biggest operational risk was measurement contamination from UI hosting, so the focused-context side was executed through an in-process service harness instead of the tuning page.
- The biggest remaining interpretation risk is that both sides use normalized carried-forward artifacts rather than raw wire payloads. That is intentional because the goal is agent-usable context, not connector serialization size.

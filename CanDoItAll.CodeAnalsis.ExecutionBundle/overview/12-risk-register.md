# Risk register

## Top risks and mitigations

### R1 — Over-scoped first version
**Risk:** the first version tries to become a full architecture platform immediately.  
**Mitigation:** keep v1 focused on deterministic static analysis, export surfaces, and minimal UI.

### R2 — Roslyn leakage
**Risk:** Roslyn-specific concerns leak into reusable contracts.  
**Mitigation:** isolate Roslyn to `Workspace`/`Facts`; map immediately to domain-friendly records.

### R3 — Host-coupling too early
**Risk:** the standalone repo starts cloning `CanDoItAll.Mcp.Core` or host-only runtime patterns.  
**Mitigation:** document the future seam and stay transport-agnostic in the engine.

### R4 — Naming drift
**Risk:** `CodeAnalsis` and `CodeAnalytics` get mixed unpredictably.  
**Mitigation:** freeze the naming map early and protect it with architecture tests.

### R5 — Diagram-first design
**Risk:** Mermaid output becomes the de facto source of truth.  
**Mitigation:** keep canonical snapshot records primary and test them directly.

### R6 — Silent unsupported cases
**Risk:** DI or EF collectors quietly omit hard cases.  
**Mitigation:** emit diagnostics and open questions explicitly.

### R7 — Codex long-file drift
**Risk:** implementation converges into giant source files.  
**Mitigation:** file-length validation scripts plus mandatory refactor pass.

### R8 — UI drives the domain
**Risk:** dashboard needs start shaping core models.  
**Mitigation:** UI consumes application services only.

### R9 — Future MCP driver still expensive
**Risk:** despite intentions, future host-repo integration still needs redesign.  
**Mitigation:** SB-00A and SB-13 explicitly freeze the tool surface and prove the seam.

### R10 — Fixture too toy-like
**Risk:** tests pass only on unrealistic mini examples.  
**Mitigation:** keep the fixture solution representative and add ambiguous/unsupported cases too.

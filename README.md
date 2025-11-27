# Code Snippets, Architecture & Standards

This folder collects curated code snippets, architecture guidance, patterns, and standards used across the Vita Admin Service codebase. It's intended for client delivery and as a reference for new contributors.


Contents:
- `ARCHITECTURE.md` - high-level architecture and layer responsibilities
- `PATTERNS.md` - conventions and common patterns (CQRS, MediatR, Interceptors, Soft Delete, Multi-tenancy)
- `FLOW.md` - request flow with folder and physical file examples
- `GIT_DELIVERY.md` - instructions for pushing these docs and delivering to clients


Keep this folder small, focused, and versioned in Git. Add new snippets as small, reviewed PRs. When adding snippets, link them from the relevant feature PR for discoverability.

Quick usage:
- The sample is illustrative and uses in-memory EF for simplicity; adapt to Postgres for production.

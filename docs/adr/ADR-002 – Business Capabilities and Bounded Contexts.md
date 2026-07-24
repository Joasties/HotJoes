# ADR-002 – Business Capabilities and Bounded Contexts

| **Document ID** | ADR-002 |
|-----------------|---------|
| **Document Title** | Business Capabilities and Bounded Contexts |
| **Version** | 1.1 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 23 July 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 23 July 2026 | Initial Architectural Decision Record. |
| 1.1 | 23 July 2026 | Expanded to define bounded context ownership, authoritative data ownership and approved cross-domain data sharing patterns. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-001 | Domain-Driven Design as the Primary Architectural Style | Accepted |
| HJ-001 | Project Vision | Draft |
| HJ-002 | Architectural Principles | Draft |
| HJ-003 | Ubiquitous Language Guide | Draft |
| HJ-004 | Vendor Domain Models | Draft |

---

# 1. Context

The HotJoes platform comprises multiple business capabilities that evolve independently while collaborating to deliver a single customer experience.

As the solution grows, different business capabilities inevitably require access to information owned by other parts of the system. Without clear ownership boundaries, business logic becomes duplicated, multiple domains attempt to maintain the same information, and the architecture gradually devolves into a tightly coupled distributed monolith.

The architecture therefore requires a clear definition of:

- how business responsibilities are partitioned;
- which domain owns each business concept;
- which domain is authoritative for each piece of business data; and
- how information may legitimately be shared across bounded context boundaries.

---

# 2. Decision

HotJoes shall organise the solution around **Business Capabilities**, each implemented as an independent **Bounded Context**.

Each bounded context shall:

- own its business concepts, behaviour, rules and invariants;
- define and maintain its own ubiquitous language;
- own the lifecycle of the information for which it is authoritative;
- expose behaviour through well-defined public contracts;
- prevent external access to its internal implementation and persistence model.

Every business concept and every authoritative data item shall belong to **exactly one bounded context**.

Other bounded contexts may consume information owned by another domain, but doing so **does not transfer ownership**.

Where information is required outside its owning domain, one of the following patterns shall be used as appropriate:

- **Reference** – store only the identifier of the authoritative entity.
- **Replicated Reference Data** – maintain a read-only copy of relatively stable information published by the owning domain.
- **Composite Read Model** – combine information from multiple bounded contexts for reporting or query purposes.
- **Immutable Snapshot** – capture business information at a point in time where historical accuracy must be preserved.
- **Cache** – retain temporary copies solely for performance or resilience.

The choice of pattern shall be driven by business requirements, consistency requirements and operational considerations.

No consuming bounded context shall become authoritative for information owned by another bounded context.

---

# 3. Consequences

### Positive

- Every business concept has a single, unambiguous owner.
- Business rules remain concentrated within the domain that owns them.
- Cross-domain dependencies become explicit architectural decisions.
- Different information sharing patterns can be selected according to business need rather than applying a single solution everywhere.
- Historical business facts can be preserved without compromising ownership boundaries.
- The architecture remains scalable as new bounded contexts are introduced.

### Negative

- Architectural discipline is required to prevent accidental ownership leakage.
- Information duplication is intentional in some cases and must be clearly justified.
- Eventual consistency becomes a natural consequence of independent domain ownership.
- Developers must understand both ownership and consistency implications when introducing new integrations.

---

# 4. Alternatives Considered

## Shared Database Ownership

Rejected because allowing multiple domains to update the same data destroys clear ownership, tightly couples business capabilities and significantly increases the cost of future change.

## Enterprise-Wide Canonical Domain Model

Rejected because a single shared model creates unnecessary coupling between business capabilities and prevents domains from evolving independently.

## Synchronous Cross-Domain Queries

Rejected as the default integration mechanism because it introduces runtime coupling between domains, reduces resilience and unnecessarily constrains deployment independence. Synchronous collaboration may still be appropriate where business requirements justify it.

---

# 5. Related Decisions

This decision directly supports:

- ADR-003 — Event-Driven Collaboration
- ADR-005 — Registered Information vs Vendor Managed Information
- ADR-006 — Address Domain Ownership and Business Address Snapshots
- ADR-007 — Vendor Compliance as a Separate Bounded Context

---

# 6. References

- Eric Evans — *Domain-Driven Design: Tackling Complexity in the Heart of Software*
- Vaughn Vernon — *Implementing Domain-Driven Design*
- Vaughn Vernon — *Domain-Driven Design Distilled*
- Martin Fowler — *Bounded Context*
- HJ-002 — Architectural Principles
- HJ-003 — Ubiquitous Language Guide
- HJ-004 — Vendor Domain Models

# ADR-002 – Business Capabilities and Bounded Contexts

| **Document ID** | ADR-002 |
|-----------------|---------|
| **Document Title** | Business Capabilities and Bounded Contexts |
| **Version** | 1.2 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 21 September 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 23 July 2026 | Initial Architectural Decision Record. |
| 1.1 | 23 July 2026 | Expanded to define bounded context ownership, authoritative data ownership and approved cross-domain data sharing patterns. |
| 1.2 | 21 September 2026 | Applied CR-078. Added the Community bounded context, Community Participation and Contact Preference ownership, and the minimal Vendor verification relationship approved through CON-046. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-001 | Domain-Driven Design as the Primary Architectural Style | Accepted |
| CR-078 | Amend ADR-002 for the Community Bounded Context | Approved |
| HJ-001 | Project Vision | Approved |
| HJ-002 | Architectural Principles | Approved |
| HJ-003 | Ubiquitous Language Guide | Approved |
| HJ-004 | Vendor Domain Models | Approved |
| HJ-010 | Current Application Architectural Concerns | Approved |
| HJ-012 | Established Application Architecture Patterns | Approved |

---

# 1. Context

The HotJoes platform comprises multiple business capabilities that evolve independently while collaborating to deliver a single customer experience.

As the solution grows, different business capabilities inevitably require access to information owned by other parts of the system. Without clear ownership boundaries, business logic becomes duplicated, multiple domains attempt to maintain the same information, and the architecture gradually devolves into a tightly coupled distributed monolith.

The architecture therefore requires a clear definition of:

- how business responsibilities are partitioned;
- which domain owns each business concept;
- which domain is authoritative for each piece of business data; and
- how information may legitimately be shared across bounded context boundaries.

CON-046 introduces Community Participation and Contact Preference after successful Vendor Registration. These concepts require an explicit owner while preserving Vendor authority for successful registration and Primary Contact information and avoiding any implication of Communication Consent or delivery authority.

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

## 2.1 Community Bounded Context

Community Participation is the explicit affirmative recorded choice of a successfully registered Vendor to participate in the HotJoes community. Contact Preference is the Vendor's selected preferred channel—Email, SMS or WhatsApp—for possible future community-related contact with its Primary Contact.

Community Participation and Contact Preference are owned by the dedicated **Community bounded context** and its Application capability. They are not attributes of the Vendor aggregate, Vendor Registration, Registration Session or Vendor-owned Primary Contact.

The Community record relates to exactly one successfully registered Vendor by reference to `VendorId`. Vendor remains authoritative for whether the Vendor was successfully registered and for its Primary Contact information. Before creating Community state, Community Application verifies the `VendorId` through a minimal Vendor-owned verification capability. That boundary returns only the information needed to establish successful registration; it does not expose the Vendor aggregate, Registered Vendor Details or Primary Contact information.

Community does not redefine the Primary Contact, copy Primary Contact details into Community-owned identity, or modify Vendor state. Physical co-location within an API host or PostgreSQL runtime does not transfer logical ownership between Vendor and Community.

Contact Preference expresses channel preference only. It is not Communication Consent, a subscription, a lawful basis for processing, an instruction to send a message or evidence that any communication was delivered. Communication Consent, recipient resolution and communication delivery remain separate future capabilities.

---

# 3. Consequences

### Positive

- Every business concept has a single, unambiguous owner.
- Business rules remain concentrated within the domain that owns them.
- Cross-domain dependencies become explicit architectural decisions.
- Different information sharing patterns can be selected according to business need rather than applying a single solution everywhere.
- Historical business facts can be preserved without compromising ownership boundaries.
- The architecture remains scalable as new bounded contexts are introduced.
- Community can evolve independently without extending the Vendor aggregate or copying Primary Contact information.
- Contact Preference remains clearly separated from consent and delivery authority.

### Negative

- Architectural discipline is required to prevent accidental ownership leakage.
- Information duplication is intentional in some cases and must be clearly justified.
- Eventual consistency becomes a natural consequence of independent domain ownership.
- Developers must understand both ownership and consistency implications when introducing new integrations.
- Community creation depends on a minimal synchronous Vendor verification boundary.

---

# 4. Alternatives Considered

## Shared Database Ownership

Rejected because allowing multiple domains to update the same data destroys clear ownership, tightly couples business capabilities and significantly increases the cost of future change.

## Enterprise-Wide Canonical Domain Model

Rejected because a single shared model creates unnecessary coupling between business capabilities and prevents domains from evolving independently.

## Synchronous Cross-Domain Queries

Rejected as the default integration mechanism because it introduces runtime coupling between domains, reduces resilience and unnecessarily constrains deployment independence. Synchronous collaboration may still be appropriate where business requirements justify it, including the minimal successful-Vendor verification required before Community Participation creation.

## Vendor-Owned Community Preference

Rejected because Community Participation is a separate post-registration lifecycle and Contact Preference is not registered or managed Vendor information. Placing either in Vendor would transfer Community ownership into the Vendor aggregate and conflate channel preference with Primary Contact data.

---

# 5. Related Decisions

This decision directly supports:

- ADR-003 — Event-Driven Collaboration
- ADR-005 — Registered Information vs Vendor Managed Information
- ADR-006 — Address Domain Ownership and Business Address Snapshots
- ADR-007 — Vendor Compliance as a Separate Bounded Context
- ADR-008 — Idempotent Operations and Reliable Event Publication
- ADR-013 — Epic 1 Runtime and Deployment Composition

---

# 6. References

- Eric Evans — *Domain-Driven Design: Tackling Complexity in the Heart of Software*
- Vaughn Vernon — *Implementing Domain-Driven Design*
- Vaughn Vernon — *Domain-Driven Design Distilled*
- Martin Fowler — *Bounded Context*
- CR-078 — Amend ADR-002 for the Community Bounded Context
- HJ-002 — Architectural Principles
- HJ-003 — Ubiquitous Language Guide
- HJ-004 — Vendor Domain Models
- HJ-010 — Current Application Architectural Concerns
- HJ-012 — Established Application Architecture Patterns

# ADR-001 – Domain-Driven Design as the Primary Architectural Style

| **Document ID** | ADR-001 |
|-----------------|---------|
| **Document Title** | Domain-Driven Design as the Primary Architectural Style |
| **Version** | 1.0 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 23 July 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 23 July 2026 | Initial Architectural Decision Record. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| HJ-001 | Project Vision | Draft |
| HJ-002 | Architectural Principles | Draft |
| HJ-003 | Ubiquitous Language Guide | Draft |
| HJ-004 | Vendor Domain Models | Draft |

---

# 1. Context

The HotJoes platform is intended to evolve into a cloud-native marketplace supporting multiple business capabilities, including vendor management, customer ordering, delivery operations, payments, and compliance.

Traditional layered architectures centred around database entities or technical services tend to produce tightly coupled systems in which business rules become distributed across application layers. As the solution grows, this makes the software increasingly difficult to understand, evolve and maintain.

The project requires an architectural approach that:

- models the business domain explicitly;
- promotes a shared business vocabulary;
- enables independent evolution of business capabilities;
- supports incremental delivery through vertical slices;
- remains maintainable as additional domains are introduced.

Several architectural styles were considered, including traditional layered architecture, clean architecture centred on technical layers, and Domain-Driven Design.

---

# 2. Decision

HotJoes shall adopt **Domain-Driven Design (DDD)** as its primary architectural style.

Business capabilities are modelled as independent bounded contexts with clearly defined ownership, responsibilities and ubiquitous language.

Business concepts take precedence over technical implementation. Domain models express business behaviour, rules and invariants directly rather than acting as simple data containers.

Supporting architectural patterns—including Clean Architecture, CQRS and Event-Driven Architecture—may be employed where appropriate, but they exist to support the domain model rather than define it.

Architectural decisions should reinforce domain boundaries and business language throughout the solution.

---

# 3. Consequences

### Positive

- The software structure reflects the business rather than technical implementation.
- Business terminology remains consistent across documentation, code and discussions.
- Domains can evolve independently with well-defined responsibilities.
- Business rules become easier to discover, understand and maintain.
- Supports incremental delivery through independently deployable vertical slices.
- Aligns naturally with event-driven collaboration between bounded contexts.

### Negative

- Domain modelling requires greater upfront analysis than data-centric approaches.
- Developers unfamiliar with DDD may require additional learning.
- Some solutions may initially appear more complex than traditional layered architectures.
- Maintaining clear bounded context boundaries requires ongoing architectural discipline.

---

# 4. Alternatives Considered

## Traditional Layered Architecture

Rejected because business behaviour typically becomes fragmented across controllers, services and repositories, resulting in weaker domain models and tighter coupling between business logic and infrastructure.

## Database-Centric Design

Rejected because persistence concerns become the primary organising principle rather than business capabilities, reducing long-term maintainability.

## Service-Oriented Architecture (SOA)

Not adopted as the primary architectural style because SOA describes how business capabilities may be exposed and integrated rather than how the business domain should be modelled internally. The architectural principles of HotJoes remain compatible with SOA, and individual bounded contexts may expose service interfaces where appropriate.

---

# 5. Related Decisions

This ADR establishes the foundation for several subsequent architectural decisions, including:

- ADR-002 — Business Capabilities and Bounded Contexts
- ADR-003 — Event-Driven Collaboration
- ADR-005 — Registered Information vs Vendor Managed Information
- ADR-006 — Address Domain Ownership and Business Address Snapshots

---

# 6. Notes

This decision defines the architectural style of the solution rather than prescribing specific implementation technologies.

Individual domains may adopt different implementation techniques where appropriate, provided they remain consistent with the principles established by this decision.

---

# 7. References

- Eric Evans — *Domain-Driven Design: Tackling Complexity in the Heart of Software*
- Vaughn Vernon — *Implementing Domain-Driven Design*
- Vaughn Vernon — *Domain-Driven Design Distilled*
- HJ-001 — Project Vision
- HJ-002 — Architectural Principles

# ADR-003 – Event-Driven Collaboration

| **Document ID** | ADR-003 |
|-----------------|---------|
| **Document Title** | Event-Driven Collaboration |
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
| 1.1 | 23 July 2026 | Clarified synchronous user interactions, business events as facts, and appropriate use of asynchronous collaboration. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-001 | Domain-Driven Design as the Primary Architectural Style | Accepted |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted |
| HJ-001 | Project Vision | Draft |
| HJ-002 | Architectural Principles | Draft |
| HJ-004 | Vendor Domain Models | Draft |

---

# 1. Context

HotJoes is composed of multiple independent bounded contexts, each owning its own business capabilities, domain model and authoritative data.

These bounded contexts must collaborate to deliver complete business processes. For example, successful Vendor registration may initiate Compliance activities, enable Ordering, update Search, notify Customer services and support operational reporting.

The architecture therefore requires a collaboration mechanism that preserves bounded context autonomy while avoiding unnecessary coupling between domains.

At the same time, the platform must provide responsive user interactions. Users should receive immediate confirmation of the outcome of their actions and should never be left uncertain whether a business operation has succeeded while asynchronous processing continues elsewhere.

---

# 2. Decision

HotJoes shall adopt **Event-Driven Collaboration** as the primary mechanism for communication between bounded contexts.

A bounded context shall publish business events describing **facts that have already occurred** within its own domain.

Business events communicate completed business outcomes. They do not request another bounded context to perform work, nor do they expose the publisher's internal implementation.

Other bounded contexts may subscribe to published events and react according to their own business responsibilities.

Published events shall:

- represent completed business facts;
- use terminology from the publishing domain's ubiquitous language;
- avoid exposing internal implementation details;
- be immutable once published;
- be versioned where incompatible changes are introduced.

Bounded contexts remain autonomous and shall not assume knowledge of which consumers exist or how published events are subsequently used.

Where a user interaction requires an immediate authoritative response, the initiating bounded context shall complete the required synchronous processing before returning a response to the caller.

Cross-domain collaboration that follows this response may proceed asynchronously through published events.

The user experience shall always reflect the latest confirmed business state rather than assuming downstream processing has already completed.

---

# 3. Consequences

### Positive

- Bounded contexts remain independently deployable and independently evolvable.
- Business capabilities collaborate without direct implementation dependencies.
- New bounded contexts can subscribe to existing events without modifying the publishing domain.
- Business workflows naturally span multiple domains while preserving ownership boundaries.
- Users receive immediate confirmation of completed business operations while longer-running processes continue asynchronously.
- The architecture supports eventual consistency without compromising user confidence.

### Negative

- Business processes become eventually consistent across bounded contexts.
- Distributed workflows require monitoring, tracing and operational visibility.
- Event versioning and schema evolution require governance.
- Failure handling, retries and duplicate message processing introduce additional implementation complexity.
- Architects must carefully determine which interactions require synchronous completion and which are suitable for asynchronous collaboration.

---

# 4. Alternatives Considered

## Direct Service-to-Service Integration

Rejected as the primary collaboration mechanism because synchronous dependencies increase runtime coupling, reduce resilience and constrain independent evolution.

Synchronous interaction remains appropriate where immediate business outcomes must be confirmed to users.

## Shared Database Integration

Rejected because allowing multiple bounded contexts to access the same persistence model violates ownership boundaries and tightly couples business capabilities through shared implementation.

## Centralised Business Orchestration

Rejected as the default collaboration model because placing business decision-making within a central orchestration layer risks concentrating business knowledge outside the domains that own it.

Process orchestration may still be appropriate where coordination itself represents a distinct business responsibility.

---

# 5. Related Decisions

This decision builds upon:

- ADR-001 — Domain-Driven Design as the Primary Architectural Style
- ADR-002 — Business Capabilities and Bounded Contexts

This decision is supported by:

- ADR-008 — Reliable Messaging and Idempotent Processing

---

# 6. References

- Eric Evans — *Domain-Driven Design: Tackling Complexity in the Heart of Software*
- Vaughn Vernon — *Implementing Domain-Driven Design*
- Martin Fowler — *Event-Driven Architecture*
- HJ-002 — Architectural Principles
- HJ-004 — Vendor Domain Models

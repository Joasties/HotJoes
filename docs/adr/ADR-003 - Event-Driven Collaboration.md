# ADR-003 – Event-Driven Collaboration

| **Document ID** | ADR-003 |
|-----------------|---------|
| **Document Title** | Event-Driven Collaboration |
| **Version** | 1.4 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 21 September 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 23 July 2026 | Initial Architectural Decision Record. |
| 1.1 | 23 July 2026 | Clarified synchronous user interactions, business events as facts, and appropriate use of asynchronous collaboration. |
| 1.2 | 22 August 2026 | Applied CR-061. Established pre-outbox Application mapping for VendorRegistered and prohibited relay-time reconstruction from current Vendor state under CON-019 and CON-020. |
| 1.3 | 28 August 2026 | Added the approved RabbitMQ at-least-once collaboration and durable idempotent-consumer semantics for CON-021 and CON-022. |
| 1.4 | 21 September 2026 | Applied CR-079. Added the CommunityParticipationRecorded v1 publication and independently executable Community consumer semantics approved through CON-046. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-001 | Domain-Driven Design as the Primary Architectural Style | Accepted |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted |
| ADR-008 | Idempotent Operations and Reliable Event Publication | Accepted |
| CR-079 | Amend ADR-003 for Community Event-Driven Collaboration | Approved |
| HJ-001 | Project Vision | Approved |
| HJ-002 | Architectural Principles | Approved |
| HJ-004 | Vendor Domain Models | Approved |
| HJ-010 | Current Application Architectural Concerns | Approved |
| HJ-012 | Established Application Architecture Patterns | Approved |

---

# 1. Context

HotJoes is composed of multiple independent bounded contexts, each owning its own business capabilities, domain model and authoritative data.

These bounded contexts must collaborate to deliver complete business processes. For example, successful Vendor registration may initiate Compliance activities, enable Ordering, update Search, notify Customer services and support operational reporting.

The architecture therefore requires a collaboration mechanism that preserves bounded context autonomy while avoiding unnecessary coupling between domains.

At the same time, the platform must provide responsive user interactions. Users should receive immediate confirmation of the outcome of their actions and should never be left uncertain whether a business operation has succeeded while asynchronous processing continues elsewhere.

CON-046 introduces a separate authoritative post-registration Community operation followed by asynchronous Community processing. Its event must remain distinct from Vendor and Compliance contracts and must not imply communication consent or delivery.

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

For `VendorRegistered`, the Vendor Application translates the completed internal business fact into the approved versioned Integration Event before outbox persistence. Vendor Infrastructure serializes and stores that event unchanged within the registration transaction. The Vendor Domain contains no Integration Event, outbox, serialization or broker representation. A relay publishes the stored event and shall not reconstruct it from current Vendor state.

Bounded contexts remain autonomous and shall not assume knowledge of which consumers exist or how published events are subsequently used.

Where a user interaction requires an immediate authoritative response, the initiating bounded context shall complete the required synchronous processing before returning a response to the caller.

Cross-domain collaboration that follows this response may proceed asynchronously through published events.

The user experience shall always reflect the latest confirmed business state rather than assuming downstream processing has already completed.

## 2.1 Community Participation Collaboration

A committed first `JoinCommunity` operation creates exactly one Community-owned `CommunityParticipationRecorded` Integration Event version 1. Equivalent replay, validation failure, Vendor Not Found, conflict and unavailable outcomes create no additional event.

The event uses the established immutable versioned envelope containing `eventId`, `eventType`, `eventVersion`, `occurredAt` and `payload`. Its version 1 payload contains exactly `communityParticipationId`, `vendorId`, `joinedAt` and `contactPreference`.

The event contains no Primary Contact name, telephone number or email address; no Vendor Registration information; no Communication Consent; no recipient resolution; and no delivery instruction or provider representation. Affirmative participation is inherent in the event type and is not repeated as a Boolean.

UUID values use lowercase canonical D format. Occurred At and Joined At are UTC invariant round-trip timestamps. Contact Preference uses `email`, `sms` or `whatsApp`. Community serializes the complete event once before outbox persistence, and every delivery attempt publishes that immutable representation unchanged.

An independently executable Community consumer receives the event through a Community-owned messaging adapter and routes it to an Epic 1 deterministic Community stub processor. The consumer validates the supported envelope and required payload, records durable receipt and processing evidence, and performs no communication-delivery work.

Event ID is the consumer idempotency key. First successful processing records one receipt associated with the Community Participation ID and Vendor ID. Redelivery of the same immutable event is acknowledged as already processed and creates no additional effect. Reuse of an Event ID with different immutable content is an integrity failure rather than a duplicate.

Unsupported event types or versions, malformed required content and integrity failures follow the established bounded retry and dead-letter treatment. Transient storage or infrastructure failure remains retryable. Unknown compatible JSON members are tolerated.

The Community stub's durable receipt is processing evidence only. It is not a second authoritative Community Participation record, does not modify the original record and does not imply that a message was sent or that Communication Consent exists.

## 2.2 Epic 1 Reliable Publication Profile

Epic 1 uses relay workers that poll their owned PostgreSQL outbox records in bounded batches and claim eligible records using leased `FOR UPDATE SKIP LOCKED` semantics. They publish stored immutable event bytes through durable RabbitMQ topology with publisher confirms, then mark the record published. Expired claims recover automatically. Failed attempts use validated bounded exponential backoff; exhausted work becomes durable `Stalled` work requiring explicit administrative requeue. Existing Vendor outbox records retain their approved no-deletion treatment. Eligible successfully published Community outbox records may follow the separately approved bounded cleanup policy without removing or altering authoritative Community Participation.

Delivery is at least once. Event ID is the stable message identity; exactly-once and ordering guarantees are not claimed. Consumers acknowledge only after durable idempotent receipt, use bounded retry, and route exhausted or non-retryable messages to a durable dead-letter queue without changing Event ID, Event Version or payload.

Trace context and safe diagnostic metadata remain outside immutable event payloads. Logs exclude serialized payloads, contact details, credentials and unsafe exception content.

---

# 3. Consequences

### Positive

- Bounded contexts remain independently deployable and independently evolvable.
- Business capabilities collaborate without direct implementation dependencies.
- New bounded contexts can subscribe to existing events without modifying the publishing domain.
- Business workflows naturally span multiple domains while preserving ownership boundaries.
- Users receive immediate confirmation of completed business operations while longer-running processes continue asynchronously.
- The architecture supports eventual consistency without compromising user confidence.
- Community publication and consumption reuse established reliability patterns without sharing Vendor or Compliance contracts.
- Community processing evidence remains distinguishable from authoritative participation and communication delivery.

### Negative

- Business processes become eventually consistent across bounded contexts.
- Distributed workflows require monitoring, tracing and operational visibility.
- Event versioning and schema evolution require governance.
- Failure handling, retries and duplicate message processing introduce additional implementation complexity.
- Architects must carefully determine which interactions require synchronous completion and which are suitable for asynchronous collaboration.
- Community introduces an additional event contract, routing boundary, consumer and durable receipt lifecycle.

---

# 4. Alternatives Considered

## Direct Service-to-Service Integration

Rejected as the primary collaboration mechanism because synchronous dependencies increase runtime coupling, reduce resilience and constrain independent evolution.

Synchronous interaction remains appropriate where immediate business outcomes must be confirmed to users, including authoritative Community creation and its minimal Vendor verification.

## Shared Database Integration

Rejected because allowing multiple bounded contexts to access the same persistence model violates ownership boundaries and tightly couples business capabilities through shared implementation.

## Centralised Business Orchestration

Rejected as the default collaboration model because placing business decision-making within a central orchestration layer risks concentrating business knowledge outside the domains that own it.

Process orchestration may still be appropriate where coordination itself represents a distinct business responsibility.

## Reusing VendorRegistered or Compliance Contracts

Rejected because Community Participation is a separately completed Community fact with different meaning, data minimisation and lifecycle. Reuse would couple Community to unrelated contracts and risk copying Vendor or Compliance information.

---

# 5. Related Decisions

This decision builds upon:

- ADR-001 — Domain-Driven Design as the Primary Architectural Style
- ADR-002 — Business Capabilities and Bounded Contexts

This decision is supported by:

- ADR-008 — Idempotent Operations and Reliable Event Publication
- ADR-013 — Epic 1 Runtime and Deployment Composition

---

# 6. References

- Eric Evans — *Domain-Driven Design: Tackling Complexity in the Heart of Software*
- Vaughn Vernon — *Implementing Domain-Driven Design*
- Martin Fowler — *Event-Driven Architecture*
- CR-079 — Amend ADR-003 for Community Event-Driven Collaboration
- HJ-002 — Architectural Principles
- HJ-004 — Vendor Domain Models
- HJ-010 — Current Application Architectural Concerns
- HJ-012 — Established Application Architecture Patterns

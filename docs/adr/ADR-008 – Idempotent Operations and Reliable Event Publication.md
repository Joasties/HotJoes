# ADR-008 – Idempotent Operations and Reliable Event Publication

| **Document ID** | ADR-008 |
|-----------------|---------|
| **Document Title** | Idempotent Operations and Reliable Event Publication |
| **Version** | 1.0 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 24 July 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 24 July 2026 | Initial Architectural Decision Record. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-003 | Event-Driven Collaboration | Accepted |
| HJ-002 | Architectural Principles | Draft |
| HJ-004 | Vendor Domain Models | Draft |
| HJ-006 | Testing Strategy and Standards | Draft |
| HJ-007 | Enforcement Strategy | Draft |

---

# 1. Context

HotJoes uses event-driven collaboration between bounded contexts.

Distributed systems cannot assume that commands or events will only ever be delivered once. Network failures, retries and transient infrastructure issues may cause the same operation to be processed multiple times.

The architecture should therefore favour business operations that naturally converge on the required business state rather than operations that perform repeated actions.

Where a business state change results in one or more domain events, those events must be published reliably without risking loss should publication temporarily fail.

---

# 2. Decision

## 2.1 Prefer Naturally Idempotent Operations

Business operations shall, wherever practical, be expressed as establishing a required business state rather than performing an action.

Examples include:

- Set Operational Availability
- Update Vendor Contact Details
- Set Trading Model
- Schedule Vendor Suspension

Executing the same operation multiple times shall leave the aggregate in the same business state as executing it once.

Operations that cannot naturally be expressed in this way shall implement additional safeguards appropriate to the business scenario.

---

## 2.2 Events Represent Business State Changes

Domain events shall only be raised when a genuine business state change occurs.

If an idempotent operation results in no change to the aggregate, no new domain event shall be produced.

The decision that an event has occurred belongs to the domain model rather than the persistence infrastructure.

---

## 2.3 Reliable Event Publication

Once a domain event has been raised, the persistence of the aggregate and the recording of its events shall occur atomically.

The implementation shall use a Transactional Outbox or an equivalent mechanism to ensure that committed business events are not lost if publication fails.

Publication may be retried without repeating the original business operation.

---

# 3. Consequences

## Positive

- Most business operations become naturally idempotent.
- Duplicate command processing normally has no adverse effect.
- Domain events accurately represent genuine business state changes.
- Business events cannot be silently lost after a successful transaction.
- The domain remains responsible for business behaviour while infrastructure remains responsible for reliable delivery.

## Negative

- A small number of business operations may require explicit duplicate detection.
- Reliable publication requires additional persistence infrastructure.
- Monitoring of failed event publication remains necessary.

---

# 4. Alternatives Considered

## Action-Based Commands

Commands such as *Activate*, *Suspend* or *Take Offline* were considered as the primary interaction style.

Rejected because state-setting operations are generally simpler to reason about, naturally idempotent and better reflect the desired business outcome.

---

## Publish Events Directly

Publishing events immediately after committing the aggregate was rejected because infrastructure failures may result in committed business changes without corresponding published events.

---

## Universal Duplicate Detection

Applying explicit idempotency keys and duplicate tracking to every command was rejected because the majority of HotJoes business operations naturally converge on the required state.

Additional duplicate detection should be reserved for those business operations where repeated execution could legitimately produce different outcomes.

---

# 5. Testing and Enforcement

Automated tests shall verify that:

- repeated execution of naturally idempotent operations leaves the aggregate in the same business state;
- domain events are only raised when business state changes;
- aggregate persistence and event recording occur atomically;
- publication retries do not repeat the original business operation.

Architecture and code review shall favour naturally idempotent business operations wherever practical.

---

# 6. Related Decisions

This decision builds upon:

- ADR-003 – Event-Driven Collaboration

This decision supports reliable communication between all HotJoes bounded contexts.

---

# 7. References

- HJ-002 – Architectural Principles
- HJ-004 – Vendor Domain Models
- HJ-006 – Testing Strategy and Standards
- HJ-007 – Enforcement Strategy

# ADR-008 – Idempotent Operations and Reliable Event Publication

| **Document ID** | ADR-008 |
|-----------------|---------|
| **Document Title** | Idempotent Operations and Reliable Event Publication |
| **Version** | 1.1 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 28 July 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 24 July 2026 | Initial Architectural Decision Record. |
| 1.1 | 28 July 2026 | Applied CR-024 to define the Register Vendor idempotency boundary and duplicate-submission behaviour, exclude Registration Session state, clarify the relationship between Domain Events and Integration Events, and strengthen reliable-publication testing and enforcement. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-003 | Event-Driven Collaboration | Accepted |
| HJ-002 | Architectural Principles | Approved |
| HJ-004 | Vendor Domain Models | Approved |
| HJ-006 | Testing Strategy and Standards | Approved |
| HJ-007 | Enforcement Strategy | Approved |
| HJ-105 | Vendor Registration Sequence Diagram | Approved |

---

# 1. Context

HotJoes uses event-driven collaboration between bounded contexts.

Distributed systems cannot assume that commands or events will only ever be delivered once. Network failures, retries, concurrent requests and transient infrastructure issues may cause the same operation to be processed multiple times.

The architecture should therefore favour business operations that naturally converge on the required business state rather than operations that perform repeated actions.

Some operations, including **Register Vendor**, create new business state and cannot be made naturally idempotent by setting a state on an already identified aggregate. These operations require an explicit idempotency safeguard.

Where a business state change results in a published Integration Event, the corresponding publication work must be recorded reliably without risking loss should publication temporarily fail. Internal Domain Events and published Integration Events represent different architectural concerns and are not required to have identical representations or payloads.

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

## 2.2 Register Vendor Idempotency

**Register Vendor is not naturally idempotent** because successful execution creates new business state rather than merely setting existing state.

Register Vendor shall therefore be protected by an explicit idempotency safeguard consisting of:

- an explicit idempotency identity; or
- an equivalent uniqueness constraint.

Where a request carries the same idempotency identity and is semantically identical to a previously successful `RegisterVendor` request—or satisfies the equivalent approved uniqueness constraint—processing shall:

- return the original successful outcome;
- create no additional Vendor;
- record no additional `VendorRegistered` Domain Event or completed business fact;
- create no additional publication or outbox record;
- publish no additional `VendorRegistered` Integration Event; and
- initiate no additional Pending Activation Process.

This business behaviour is mandatory regardless of implementation.

The concrete format of the idempotency identity, the uniqueness mechanism, retention duration, payload-equivalence rules and storage strategy are implementation conventions.

---

## 2.3 Idempotency Conflict

If a Register Vendor request is received that carries the same idempotency identity (or satisfies the equivalent approved uniqueness constraint) but is **not** semantically identical to the previously successful request associated with that identity, the request shall not create an additional Vendor or modify the existing Vendor.

Such requests represent an idempotency conflict rather than a retry of the original business operation.

The operation shall therefore:

- return a controlled conflict outcome;
- create no additional Vendor;
- record no additional completed business fact;
- record no additional VendorRegistered Domain Event;
- create no additional publication work;
- publish no additional VendorRegistered Integration Event; and
- initiate no additional Pending Activation Process.

The concrete response format, HTTP status code and payload are implementation conventions and are outside the scope of this Architectural Decision.

## 2.4 Register Vendor Idempotency Boundary

The idempotency boundary is the complete `RegisterVendor` request submitted to the Vendor Registration capability.

Registration Session state is explicitly outside that boundary. Registration Session ownership is an interaction concern and has no influence on server-side idempotency behaviour.

The Vendor Registration capability shall be independent of how the complete registration request was assembled. It shall not create, retrieve, inspect or depend on Registration Session state when determining an idempotent outcome.

---

## 2.5 Domain Events and Integration Events

Domain Events represent internal business facts used within a Domain. The decision that a Domain Event has occurred belongs to the domain model rather than the persistence or publication infrastructure.

Integration Events are explicitly published contracts intended for consumption by other business capabilities.

Domain Events and Integration Events address different architectural concerns. A published Integration Event may be derived from a completed business fact, but the internal Domain Event and published Integration Event are not required to have identical representations or payloads.

Domain Events shall only be raised when a genuine business state change occurs. If an idempotent operation results in no business state change, no new Domain Event, completed business fact or corresponding Integration Event shall be produced.

Reliable publication does not alter Domain behaviour. Publication concerns shall remain separate from business decision-making.

---

## 2.6 Reliable Event Publication

Where a completed business state change requires publication of an Integration Event, aggregate persistence and durable recording of the corresponding publication work shall occur atomically.

The durable publication record may contain an Integration Event derived from the completed business fact. The Domain Event itself is not required to be the persisted publication record or outbox message.

The implementation shall use a Transactional Outbox or an equivalent mechanism to ensure that committed Integration Events are not lost if publication fails.

Publication may be retried without repeating the original business operation. Publication retry shall not create a new Domain Event, completed business fact, publication record or Integration Event.

The architecture does not prescribe a particular persistence mechanism, outbox technology, transport, serialization format, messaging infrastructure or implementation framework, provided the required atomicity and observable business behaviour are preserved.

---

# 3. Consequences

## Positive

- Most business operations remain naturally idempotent.
- Operations that create new business state have an explicit idempotency safeguard.
- Duplicate `RegisterVendor` processing cannot create duplicate Vendors, business facts, publication records, Integration Events or Pending Activation Processes.
- Register Vendor idempotency is independent of Registration Session state and request-assembly behaviour.
- Domain Events accurately represent genuine business state changes.
- Domain Events and published Integration Events are treated as distinct architectural concepts.
- Required Integration Events cannot be silently lost after a successful transaction.
- Publication retries do not repeat the original business operation.
- The Domain remains responsible for business behaviour while infrastructure remains responsible for reliable delivery.

## Negative

- Creation operations such as Register Vendor require explicit duplicate detection or an equivalent uniqueness constraint.
- Idempotency identities or uniqueness constraints require governance and implementation support.
- Reliable publication requires additional persistence infrastructure.
- Monitoring of failed event publication remains necessary.
- Payload-equivalence and idempotency-retention conventions must be defined by the implementation.

---

# 4. Alternatives Considered

## Action-Based Commands

Commands such as *Activate*, *Suspend* or *Take Offline* were considered as the primary interaction style.

Rejected because state-setting operations are generally simpler to reason about, naturally idempotent and better reflect the desired business outcome.

---

## Publish Events Directly

Publishing Integration Events immediately after committing the aggregate was rejected because infrastructure failures may result in committed business changes without corresponding published events.

---

## Universal Duplicate Detection

Applying explicit idempotency identities and duplicate tracking to every command was rejected because the majority of HotJoes business operations naturally converge on the required state.

Additional duplicate detection shall be reserved for business operations where repeated execution could legitimately produce different outcomes. Register Vendor is one such operation because each successful execution would otherwise create new business state.

---

## Registration Session as the Idempotency Boundary

Using Registration Session state as the Register Vendor idempotency boundary was rejected because Registration Sessions are interaction concerns outside the Vendor Registration service boundary.

The Vendor Registration capability must remain independent of how a complete registration request was assembled.

---

# 5. Testing and Enforcement

Automated tests shall verify that:

- repeated execution of naturally idempotent operations leaves the aggregate in the same business state;
- Domain Events are only raised when business state changes;
- aggregate persistence and durable publication recording occur atomically;
- publication retries do not repeat the original business operation;
- publication retries do not create duplicate Domain Events, completed business facts, publication records or Integration Events.

Register Vendor verification shall cover:

- client retries;
- network retries;
- concurrent duplicate requests;
- repeated successful submissions; and
- duplicate message delivery scenarios, where applicable.

For a request carrying the same idempotency identity and semantically identical to a previously successful request—or satisfying the equivalent approved uniqueness constraint—tests shall verify that processing:

- returns the original successful outcome;
- creates no additional Vendor;
- records no additional `VendorRegistered` Domain Event or completed business fact;
- creates no additional publication or outbox record;
- publishes no additional `VendorRegistered` Integration Event; and
- initiates no additional Pending Activation Process.

For a request carrying the same idempotency identity as a previously successful request but with semantically different registration information, tests shall verify that processing:

- returns the controlled idempotency-conflict outcome;
- creates no additional Vendor;
- records no additional `VendorRegistered` Domain Event or completed business fact;
- creates no additional publication or outbox record;
- publishes no additional `VendorRegistered` Integration Event; and
- initiates no additional Pending Activation Process.

Architecture and code review shall verify that:

- naturally idempotent business operations are favoured wherever practical;
- Register Vendor retains its explicit idempotency safeguard following future changes;
- Registration Session state is not introduced into the Register Vendor idempotency boundary;
- Domain Events are not treated as necessarily identical to published Integration Events; and
- implementation choices preserve the mandatory architectural behaviour without coupling the decision to a particular technology.

---

# 6. Scope and Application

ADR-008 is the authoritative architectural source for idempotency and reliable-publication principles.

HJ-105 applies these principles to the Vendor Registration workflow and shall remain consistent with this decision. Future regeneration or revision of HJ-105 shall use ADR-008 as the authoritative source for these concerns.

ADR-008 does not prescribe:

- idempotency-key or uniqueness-token format;
- storage technology or persistence mechanism;
- retention duration;
- payload-comparison implementation or hashing strategy;
- outbox technology;
- event transport technology or messaging infrastructure;
- serialization format; or
- implementation framework.

These remain implementation decisions provided the mandatory architectural behaviour defined by this ADR is preserved.

---

# 7. Related Decisions

This decision builds upon:

- ADR-003 – Event-Driven Collaboration

This decision supports reliable communication between all HotJoes bounded contexts.

---

# 8. References

- CR-024 – Register Vendor Idempotency Boundary and Reliable Publication Clarification
- HJ-002 – Architectural Principles
- HJ-004 – Vendor Domain Models
- HJ-006 – Testing Strategy and Standards
- HJ-007 – Enforcement Strategy
- HJ-105 – Vendor Registration Sequence Diagram

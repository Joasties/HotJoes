# ADR-008 – Idempotent Operations and Reliable Event Publication

| **Document ID** | ADR-008 |
|-----------------|---------|
| **Document Title** | Idempotent Operations and Reliable Event Publication |
| **Version** | 1.6 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 21 September 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 24 July 2026 | Initial Architectural Decision Record. |
| 1.1 | 28 July 2026 | Applied CR-024 to define the Register Vendor idempotency boundary and duplicate-submission behaviour, exclude Registration Session state, clarify the relationship between Domain Events and Integration Events, and strengthen reliable-publication testing and enforcement. |
| 1.2 | 13 August 2026 | Applied CR-034 to remove delivery-slice Pending Activation assumptions from Register Vendor idempotency and testing semantics. |
| 1.3 | 21 August 2026 | Defined the approved Epic 1 PostgreSQL concurrency authority, permanent replay-outcome persistence, atomic registration transaction and explicit EF Core mapping mechanics for CON-014–CON-016 and CON-028. |
| 1.4 | 22 August 2026 | Applied CR-062. Defined pre-outbox VendorRegistered translation, immutable serialized-event staging and the approved versioned publication contract under CON-019 and CON-020. |
| 1.5 | 28 August 2026 | Defined the Epic 1 PostgreSQL relay, RabbitMQ delivery, migration, trace-context and recovery mechanics for CON-018, CON-021, CON-029 and CON-035. |
| 1.6 | 21 September 2026 | Applied CR-080. Added Join Community idempotency, authoritative replay, concurrency, Community transaction and retention semantics approved through CON-046. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted |
| ADR-003 | Event-Driven Collaboration | Accepted |
| ADR-013 | Epic 1 Runtime and Deployment Composition | Accepted |
| CR-034 | Remove Delivery-Slice Scope from Enduring Vendor Architecture Artefacts | Approved |
| CR-080 | Amend ADR-008 for Join Community Idempotency and Reliable Publication | Approved |
| HJ-002 | Architectural Principles | Approved |
| HJ-004 | Vendor Domain Models | Approved |
| HJ-006 | Testing Strategy and Standards | Approved |
| HJ-007 | Enforcement Strategy | Approved |
| HJ-010 | Current Application Architectural Concerns | Approved |
| HJ-105 | Vendor Registration Sequence Diagram | Approved |

---

# 1. Context

HotJoes uses event-driven collaboration between bounded contexts.

Distributed systems cannot assume that commands or events will only ever be delivered once. Network failures, retries, concurrent requests and transient infrastructure issues may cause the same operation to be processed multiple times.

The architecture should therefore favour business operations that naturally converge on the required business state rather than operations that perform repeated actions.

Some operations, including **Register Vendor** and **Join Community**, create new business state and cannot be made naturally idempotent by setting a state on an already identified aggregate. These operations require explicit, operation-specific idempotency safeguards.

Where a business state change results in a published Integration Event, the corresponding publication work must be recorded reliably without risking loss should publication temporarily fail. Internal Domain Events, completed business facts and published Integration Events represent different architectural concerns and are not required to have identical representations or payloads.

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
- publish no additional `VendorRegistered` Integration Event.

This business behaviour is mandatory regardless of implementation.

For Epic 1, the approved composite identity and semantic-equivalence rules are defined by CON-013. PostgreSQL is the concurrency authority: a database-enforced unique constraint over the persisted normalized composite identity permits only one Vendor registration to commit. A competing request that loses the uniqueness race commits no business effect and resolves the committed registration record to either the original successful result or the controlled idempotency-conflict outcome. No process-local lock, distributed lock or separate request-coordination service is used.

Each successful registration permanently persists the original Application result and a SHA-256 fingerprint of a versioned deterministic UTF-8 canonical representation of the materially relevant registration information. The exclusions defined by CON-013 remain authoritative. The persisted outcome does not expire, is retained for at least as long as the Vendor registration exists, and is not reconstructed from current Vendor lifecycle state. Epic 1 provides no expiry or deletion operation for it.

## 2.3 Register Vendor Idempotency Conflict

If a Register Vendor request is received that carries the same idempotency identity, or satisfies the equivalent approved uniqueness constraint, but is not semantically identical to the previously successful request associated with that identity, the request shall not create an additional Vendor or modify the existing Vendor.

The operation shall:

- return a controlled conflict outcome;
- create no additional Vendor;
- record no additional completed business fact;
- record no additional VendorRegistered Domain Event;
- create no additional publication work;
- publish no additional VendorRegistered Integration Event; and
- leave previously committed Vendor state unchanged.

The concrete response format, HTTP status code and payload are implementation conventions and are outside this Register Vendor decision.

## 2.4 Register Vendor Idempotency Boundary

The idempotency boundary is the complete `RegisterVendor` request submitted to the Vendor Registration capability.

Registration Session state is explicitly outside that boundary. Registration Session ownership is an interaction concern and has no influence on server-side idempotency behaviour.

The Vendor Registration capability shall be independent of how the complete registration request was assembled. It shall not create, retrieve, inspect or depend on Registration Session state when determining an idempotent outcome.

## 2.5 Join Community Idempotency Boundary

**Join Community is not naturally idempotent** because its first successful execution creates a new immutable Community Participation record.

The request contains exactly the successfully registered `VendorId` and one Contact Preference from Email, SMS or WhatsApp. Affirmative Community Participation is implied by invoking the operation. The request contains no client-controlled participation Boolean, Primary Contact details, Communication Consent or delivery instruction.

`VendorId` is unique within the Community Participation store and is the natural replay and conflict boundary. Before creating state, Community Application verifies successful Vendor registration through the minimal Vendor-owned verification capability. Validation failure, Vendor Not Found and Vendor Verification Unavailable create no Community Participation record, outbox work or Integration Event.

A successful first request creates one immutable Community Participation record containing a server-generated `CommunityParticipationId`, the registered `VendorId`, selected Contact Preference, original `JoinedAt` and any technical concurrency value required by persistence.

When a Community Participation record already exists for the Vendor:

- the same Contact Preference returns the original committed result as `CommunityParticipationAlreadyRecorded`;
- a different Contact Preference returns `CommunityParticipationConflict`; and
- neither outcome creates another record, outbox item or Integration Event.

Concurrent first requests are resolved atomically by the Community persistence boundary. Exactly one request can establish the record. Equivalent contenders observe the original success; conflicting contenders receive the controlled conflict. At most one Community outbox message and one logical `CommunityParticipationRecorded` Integration Event are created.

The authoritative success contains the original `CommunityParticipationId`, `VendorId`, Contact Preference and `JoinedAt`. Equivalent replay returns these committed values rather than constructing a new result. The Web client may explicitly retry after a retryable or uncertain failure without causing duplicate Community state.

## 2.6 Domain Events and Integration Events

Domain Events represent internal business facts used within a Domain. The decision that a Domain Event has occurred belongs to the domain model rather than the persistence or publication infrastructure.

Integration Events are explicitly published contracts intended for consumption by other business capabilities.

Domain Events and Integration Events address different architectural concerns. A published Integration Event may be derived from a completed business fact, but the internal Domain Event and published Integration Event are not required to have identical representations or payloads.

For Epic 1, an explicit Vendor Application mapper translates the completed `VendorRegistered` business fact and its registration-time information into the approved `VendorRegistered` Integration Event v1 before outbox persistence. The mapping is outside the Vendor Domain. Vendor Infrastructure serializes the resulting contract once and persists that immutable serialized event in the outbox as part of the registration transaction.

For Community, the authoritative first `JoinCommunity` operation constructs the approved Community-owned `CommunityParticipationRecorded` Integration Event v1. Community Infrastructure serializes the complete contract once and persists its immutable representation as part of the Community transaction. Equivalent replay and all controlled failure outcomes produce no additional completed fact or Integration Event.

Domain Events or completed facts shall only be recorded when a genuine business state change occurs. If an idempotent operation results in no business state change, no new Domain Event, completed business fact or corresponding Integration Event shall be produced.

Reliable publication does not alter Domain behaviour. Publication concerns shall remain separate from business decision-making.

## 2.7 Reliable Event Publication

Where a completed business state change requires publication of an Integration Event, authoritative persistence and durable recording of corresponding publication work shall occur atomically.

The durable publication record contains the serialized Integration Event derived from the completed business fact. A Domain Event is not itself the persisted publication record or outbox message.

The implementation shall use a Transactional Outbox or an equivalent mechanism to ensure that committed Integration Events are not lost if publication fails.

Publication may be retried without repeating the original business operation. Publication retry shall not create a new Domain Event, completed business fact, publication record or Integration Event.

Publication retry preserves the original Event ID, Event Version, timestamps and serialized event. The relay publishes the stored event unchanged and shall not reconstruct its payload from current state.

Except for the explicit Epic 1 persistence treatments in §§2.8 and 2.9, the architecture does not prescribe a particular persistence mechanism, outbox technology, transport, serialization format, messaging infrastructure or implementation framework, provided the required atomicity and observable business behaviour are preserved.

## 2.8 Epic 1 Vendor Registration Transaction and PostgreSQL Mapping

For Epic 1, one explicit PostgreSQL transaction atomically commits:

- the Vendor Aggregate and Registered Information;
- the persisted composite identity, semantic fingerprint and original successful `RegisterVendor` Application result; and
- exactly one durable outbox item for the genuine `VendorRegistered` occurrence.

Any failure before commit leaves none of these records committed. Address resolution and pre-transaction validation occur before the transaction begins. Outbox dispatch occurs after commit and outside the registration transaction. The Vendor Application coordinates this boundary; EF Core and PostgreSQL mechanics remain in Vendor Infrastructure.

Vendor Infrastructure uses explicit EF Core fluent mappings. PostgreSQL mappings define keys, lengths, nullability, conversions, enum representations, indexes and restrictive deletion behaviour. Persisted normalized Trading Name and Legal Operator Name together with `CanonicalAddressId` form the database-enforced unique constraint. A one-to-one registration-outcome record retains the semantic fingerprint and original result. Registration Declarations and the opaque Address Resolution reference are not Vendor state. Registration outcomes and outbox records do not cascade-delete.

Schema migration, outbox relay and broker delivery use the approved Epic 1 profile in §2.10. The `VendorRegistered` Integration Event v1 contract and pre-outbox translation are governed by CON-019 and CON-020.

## 2.9 Epic 1 Community Transaction, Persistence and Retention

For Epic 1, one Community-owned PostgreSQL transaction atomically commits:

- one immutable Community Participation record for the first successful `JoinCommunity` request; and
- exactly one durable Community outbox item containing the serialize-once `CommunityParticipationRecorded` v1 event.

Any failure before commit leaves neither durable. Vendor verification occurs before Community creation, and outbox dispatch occurs after commit. Community persistence uses a Community-owned schema, repository, mappings and transaction boundary even when it physically shares the Epic 1 PostgreSQL runtime. Vendor persistence and the Vendor aggregate are not modified.

The Community Participation record and its original successful result are retained for at least as long as the associated Vendor exists. Epic 1 provides no expiry, deletion, amendment or withdrawal operation. Absence of those operations is an Epic 1 scope constraint and is not a permanent legal or product retention policy.

Because the record is create-only and immutable, its audit evidence consists of Community Participation ID, Vendor ID, Contact Preference, original Joined At, associated outbox Event ID and creation metadata. Epic 1 introduces no separate mutable audit-history model.

Eligible published Community outbox records may follow the established bounded cleanup policy. Cleanup does not remove or alter authoritative Community Participation or affect equivalent replay. Community consumer receipts are retained durably for the Epic 1 lifetime of the corresponding participation so delayed redelivery remains idempotent. Epic 1 provides no receipt deletion operation; future cleanup must preserve duplicate detection.

Contact Preference is personal-data-related metadata even though it contains no address or telephone number. Persistence, events, logs, diagnostics, health output and test evidence shall not copy or expose Primary Contact details. Application and infrastructure logs record only safe technical identifiers and outcome kinds and shall not record request bodies, serialized events, credentials, connection strings or unsafe exception content.

Vendor ID, Community Participation ID and Event ID are correlation identifiers, not authentication credentials, consent evidence or proof that the caller controls the Vendor. They shall not be treated as bearer secrets or authorisation mechanisms.

## 2.10 Epic 1 Relay, Broker and Migration Profile

Epic 1 relay workers poll their owned PostgreSQL outbox records in bounded batches and claim eligible records using leased `FOR UPDATE SKIP LOCKED` semantics. They publish stored immutable event bytes through durable RabbitMQ topology with publisher confirms, then mark records published. Expired claims recover automatically. Failed attempts use validated bounded exponential backoff; exhausted work becomes durable `Stalled` work requiring explicit administrative requeue. Existing Vendor outbox records retain their approved no-deletion treatment. Only eligible successfully published Community outbox rows are subject to the bounded cleanup policy approved through CON-046.

Delivery is at least once. Event ID is the stable message identity; exactly-once and ordering guarantees are not claimed. Consumers acknowledge only after durable idempotent receipt, use bounded retry, and route exhausted or non-retryable messages to a durable dead-letter queue without changing Event ID, Event Version or payload.

W3C traceparent and optional tracestate are persisted as outbox metadata and forwarded in RabbitMQ headers; they do not alter immutable Integration Event JSON. PostgreSQL schema evolution uses reviewed versioned EF Core migrations applied by an explicit pre-readiness deployment step. Clean creation and upgrade from the previous supported baseline are verified against real PostgreSQL. Ordinary API, relay and consumer startup does not migrate production databases.

---

# 3. Consequences

## Positive

- Most business operations remain naturally idempotent.
- Operations that create new business state have explicit, operation-specific idempotency safeguards.
- Duplicate `RegisterVendor` processing cannot create duplicate Vendors, business facts, publication records or Integration Events.
- Register Vendor idempotency is independent of Registration Session state and request-assembly behaviour.
- Equivalent Join Community retries return the original result without duplicate Community state or events.
- Different-preference retries produce a controlled conflict, and concurrent requests establish at most one record and event.
- Domain Events accurately represent genuine business state changes.
- Domain Events and published Integration Events are treated as distinct architectural concepts.
- Required Integration Events cannot be silently lost after a successful transaction.
- Publication retries do not repeat original business operations.
- Domain and Application responsibilities remain separate from reliable-delivery infrastructure.

## Negative

- Creation operations such as Register Vendor and Join Community require explicit duplicate detection or equivalent uniqueness constraints.
- Idempotency identities or uniqueness constraints require governance and implementation support.
- Reliable publication requires additional persistence infrastructure.
- Monitoring of failed event publication remains necessary.
- Permanent replay-outcome storage duplicates limited successful-result representations and requires governed semantics.
- Community introduces durable participation, outbox and consumer-receipt retention responsibilities.

---

# 4. Alternatives Considered

## Action-Based Commands

Commands such as *Activate*, *Suspend* or *Take Offline* were considered as the primary interaction style.

Rejected because state-setting operations are generally simpler to reason about, naturally idempotent and better reflect the desired business outcome.

## Publish Events Directly

Publishing Integration Events immediately after committing authoritative state was rejected because infrastructure failures may result in committed business changes without corresponding published events.

## Universal Duplicate Detection

Applying explicit idempotency identities and duplicate tracking to every command was rejected because the majority of HotJoes business operations naturally converge on the required state.

Additional duplicate detection shall be reserved for business operations where repeated execution could legitimately produce different outcomes. Register Vendor and Join Community are such operations because each first successful execution otherwise creates new business state.

## Registration Session as the Idempotency Boundary

Using Registration Session state as the Register Vendor or Join Community idempotency boundary was rejected because Registration Sessions are transient interaction concerns outside both authoritative service boundaries.

## Client-Generated Community Idempotency Key

Rejected for Epic 1 because the unique registered Vendor ID already provides the approved natural replay and conflict boundary. A separate client token would add another identity without improving the required semantics.

---

# 5. Testing and Enforcement

Automated tests shall verify that:

- repeated execution of naturally idempotent operations leaves the aggregate in the same business state;
- Domain Events or completed facts are only recorded when business state changes;
- authoritative persistence and durable publication recording occur atomically;
- publication retries do not repeat the original business operation; and
- publication retries do not create duplicate completed facts, publication records or Integration Events.

Register Vendor verification shall cover client retries, network retries, concurrent duplicate requests, repeated successful submissions and duplicate message delivery scenarios where applicable.

For a Register Vendor request carrying the same idempotency identity and semantically identical information—or satisfying the equivalent approved uniqueness constraint—tests shall verify the original successful outcome and no additional Vendor, completed fact, Domain Event, outbox record or Integration Event.

For semantically different registration information at the same identity, tests shall verify controlled conflict, no additional state or publication work, and unchanged committed Vendor state.

Join Community verification shall cover:

- request and Contact Preference validation;
- authoritative Vendor verification and unavailable outcomes;
- first successful record and outbox atomicity;
- equivalent replay returning original Community Participation ID, Vendor ID, Contact Preference and Joined At;
- different-preference conflict with no new state or publication work;
- concurrent equivalent and conflicting requests resulting in exactly one record and at most one logical event;
- uncertain client retry;
- immutable publication retry; and
- consumer duplicate, integrity-failure, retry and dead-letter treatment.

Architecture and code review shall verify that:

- naturally idempotent operations are favoured wherever practical;
- Register Vendor and Join Community retain their distinct safeguards;
- Registration Session state is introduced into neither authoritative idempotency boundary;
- Domain Events are not treated as necessarily identical to published Integration Events;
- Vendor and Community persistence ownership remains separate; and
- implementation choices preserve mandatory behaviour without coupling the decision to a particular technology.

---

# 6. Scope and Application

ADR-008 is the authoritative architectural source for idempotency and reliable-publication principles.

HJ-105 applies these principles to the Vendor Registration and post-registration Community workflows and shall remain consistent with this decision. Future regeneration or revision of HJ-105 shall use ADR-008 as the authoritative source for these concerns.

Outside the approved Epic 1 treatments in §§2.2, 2.5, 2.8 and 2.9, ADR-008 does not prescribe:

- idempotency-key or uniqueness-token format;
- storage technology or persistence mechanism;
- retention duration for other operations;
- payload-comparison implementation or hashing strategy for other operations;
- outbox technology;
- event transport technology or messaging infrastructure;
- serialization format; or
- implementation framework.

These remain implementation decisions provided the mandatory architectural behaviour defined by this ADR is preserved.

---

# 7. Related Decisions

This decision builds upon:

- ADR-002 – Business Capabilities and Bounded Contexts
- ADR-003 – Event-Driven Collaboration

This decision supports reliable communication between all HotJoes bounded contexts and the runtime composition defined by ADR-013.

---

# 8. References

- CR-024 – Register Vendor Idempotency Boundary and Reliable Publication Clarification
- CR-080 – Amend ADR-008 for Join Community Idempotency and Reliable Publication
- HJ-002 – Architectural Principles
- HJ-004 – Vendor Domain Models
- HJ-006 – Testing Strategy and Standards
- HJ-007 – Enforcement Strategy
- HJ-010 – Current Application Architectural Concerns
- HJ-105 – Vendor Registration Sequence Diagram

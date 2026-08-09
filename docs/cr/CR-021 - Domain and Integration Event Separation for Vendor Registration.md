# CR-021 – Domain and Integration Event Separation for Vendor Registration

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-021 |
| **Title** | Domain and Integration Event Separation for Vendor Registration |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Important |
| **Affected Documents** | HJ-004 – Vendor Domain Models; HJ-105 – Vendor Registration Sequence Diagram |

---

# 1. Background

Separate the treatment of the internal VendorRegistered domain event from the published VendorRegistered integration event, which defines the collaboration contract with downstream bounded contexts.

Without explicitly distinguishing these concepts, the architecture leaves it unclear whether:

- VendorRegistered is an internal domain event;
- VendorRegistered is an integration event;
- one is derived from the other; or
- the same event is intended to fulfil both responsibilities.

This distinction is now particularly important because **CR-016** explicitly prohibits Registration Declarations from appearing in either domain events or integration events.

This change clarifies the architectural separation between internal domain events and published integration events without prescribing transport technology, serialization format or implementation patterns.

---

# 2. Objectives

The revised artefacts shall:

- distinguish clearly between Vendor domain events and Vendor integration events;
- define **VendorRegistered domain event** as an internal domain event representing successful completion of Vendor registration;
- define the **VendorRegistered integration event** as the published collaboration contract used by downstream bounded contexts;
- establish the minimum contractual content of the published integration event;
- clarify that reliable publication derives from the completed business fact rather than requiring identical internal and external event representations.

---

# 3. Required Changes

## 3.1 HJ-004 – Vendor Domain Models

### Section 7 – Domain and Integration Events

Revise the existing **Section 7 – Domain Events** by either:

- renaming it to **Domain and Integration Events**; or
- retaining the existing heading and introducing clear subsections for **Domain Events** and **Integration Events**.

The document shall explicitly distinguish the two event concepts.

---

### Section 7.1 – Domain Events

Define **VendorRegistered domain event** as the internal domain event representing successful completion of Vendor registration.

The domain event:

- records the completed business fact within the Vendor Domain;
- represents completion of Vendor creation; and
- is an internal business event rather than an external collaboration contract.

No minimum payload requirements are prescribed for the internal domain event.

---

### Section 7.2 – Integration Events

Introduce a separate subsection defining the **VendorRegistered integration event**.

The integration event represents the published collaboration contract used to initiate downstream business processes.

Reliable publication of the integration event shall derive from the successful completion of Vendor registration.

The architecture shall explicitly state that:

- the internal domain event and published integration event represent different architectural concerns;
- they need not be identical representations of the same information; and
- no transport protocol, serialization format or messaging technology is implied by this distinction.

---

### VendorRegistered Integration Event Contract

Define the minimum contractual content of the published **VendorRegistered integration event**.

The integration event shall contain, at a minimum:

- VendorId;
- RegisteredAt;
- resulting Vendor State;
- Trading Preference;
- Legal Operator Type;
- Trading Characteristics;
- the approved Business Address information required by the Compliance contract;
- Food Registration Authority; and
- Primary Trading Authority where applicable.

The published integration event shall contain sufficient information for the **Pending Activation Process** to begin processing without requiring an immediate synchronous query back into the Vendor Domain.

A published event that requires downstream bounded contexts to retrieve these values directly from the Vendor Domain is not permitted.

---

## 3.2 HJ-105 – Vendor Registration Sequence Diagram

Within **Section 5 – Happy Path**, immediately following successful completion of Vendor registration, strengthen the description of event publication.

State that:

- successful registration records the completed business fact within the Vendor Domain;
- reliable publication subsequently produces the **VendorRegistered integration event**;
- the integration event represents the collaboration contract for downstream bounded contexts;
- the internal domain event and published integration event are separate architectural concepts; and
- their representations are not required to be identical.

The minimum integration-event payload shall match the contract defined in **HJ-004 Section 7**.

---

# 4. Editorial Principles

- No new business capability shall be introduced.
- No new business events shall be introduced.
- This change clarifies architectural responsibilities rather than changing behaviour.
- Domain events and integration events shall be treated as distinct architectural concepts.
- The architecture shall remain independent of transport technology, serialization format and messaging infrastructure.
- Reliable publication shall continue to follow the project's Transactional Outbox and reliability principles.
- CR-016's prohibition on Registration Declarations appearing in either domain events or integration events shall remain fully consistent with this separation.

---

# 5. Expected Outcome

Following this change:

- HJ-004 clearly distinguishes between internal domain events and published integration events.
- **VendorRegistered** is explicitly recognised as both:
  - an internal domain event recording successful Vendor registration; and
  - a published integration event used as the collaboration contract with downstream bounded contexts.
- The published integration event has a clearly defined minimum contractual payload while remaining independent of transport or serialization technology.
- Downstream bounded contexts receive sufficient information to begin processing without synchronously querying the Vendor Domain.
- Future Service Contracts, event schemas and implementations derive a consistent collaboration model directly from the approved architectural artefacts.

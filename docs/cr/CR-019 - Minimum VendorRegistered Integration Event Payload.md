# CR-019 – Minimum VendorRegistered Integration Event Payload

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-019 |
| **Title** | Minimum VendorRegistered Integration Event Payload |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Important |
| **Affected Documents** | HJ-004 – Vendor Domain Models; HJ-105 – Vendor Registration Sequence Diagram |

---

# 1. Background

The independent architectural review of **HJ-106 – Vendor Registration Service Contract** identified that the minimum content of the **VendorRegistered** integration event is currently implied rather than explicitly defined.

The existing architecture already establishes that successful Vendor Registration initiates the Pending Activation Process, which subsequently requests applicable Compliance Requirements.

However, the current documents do not explicitly define the minimum information that must be contained within the published **VendorRegistered** integration event to enable downstream business capabilities to perform their responsibilities independently.

Without an explicit minimum payload, implementations could legitimately publish an event containing only a Vendor identifier, forcing the Pending Activation Process (or other downstream consumers) to synchronously query the Vendor Domain for registration information immediately after receiving the event.

This would introduce unnecessary runtime coupling between business capabilities and violate the architectural principles of explicit published contracts, capability ownership and event-driven collaboration.

This change defines the minimum business information that every **VendorRegistered** integration event shall contain while deliberately leaving serialization format and implementation details to the implementation.

---

# 2. Objectives

The revised architecture shall:

- define the minimum business content required within every **VendorRegistered** integration event;
- ensure downstream consumers receive sufficient information to begin the Pending Activation Process without querying the Vendor Domain;
- reinforce the role of published events as complete business facts rather than notification triggers;
- align HJ-105 with the Vendor Domain event definitions contained within HJ-004; and
- prevent runtime coupling between the Pending Activation Process and the Vendor Domain.

---

# 3. Required Changes

## 3.1 HJ-004 – Vendor Domain Models

### Section 7 – Domain Events

Revise the description of the published **VendorRegistered** integration event by introducing the following minimum payload rule.

> **Minimum VendorRegistered Integration Event Payload**
>
> The **VendorRegistered** integration event published following successful Vendor Registration shall contain, at a minimum:
>
> - VendorId;
> - RegisteredAt;
> - resulting Vendor State;
> - Trading Preference;
> - Legal Operator Type;
> - Trading Characteristics;
> - the approved Business Address information required by the Compliance contract;
> - Food Registration Authority; and
> - Primary Trading Authority, where applicable.
>
> The event shall represent a complete business fact sufficient for downstream business capabilities to begin processing without requiring synchronous retrieval of registration information from the Vendor Domain.

Clarify that implementation-specific metadata may be added provided the minimum business content is preserved.

---

## 3.2 HJ-105 – Vendor Registration Sequence Diagram

### Section 5 – Vendor Registration Happy Path

Immediately following the final note associated with publication of **VendorRegistered**, add the following authoritative rule.

> **Minimum VendorRegistered Integration Event Payload**
>
> The **VendorRegistered** integration event published after successful Vendor Registration shall contain, at a minimum:
>
> - VendorId;
> - RegisteredAt;
> - resulting Vendor State;
> - Trading Preference;
> - Legal Operator Type;
> - Trading Characteristics;
> - the approved Business Address information required by the Compliance contract;
> - Food Registration Authority; and
> - Primary Trading Authority, where applicable.
>
> A payload that requires the Pending Activation Process, Compliance Requirement Provider or any other downstream business capability to synchronously query the Vendor Domain for these values is not permitted.

Retain the existing registration sequence and event publication flow.

No additional interactions shall be introduced between the Pending Activation Process and the Vendor Domain following publication of **VendorRegistered**.

---

### Pending Activation Guidance

Immediately following the new minimum payload rule, add the following clarification.

> The Pending Activation Process shall obtain the business information required to initiate compliance processing directly from the published **VendorRegistered** integration event.
>
> The event is the authoritative integration contract between the Vendor Domain and downstream business capabilities.

---

# 4. Editorial Principles

- No business capability shall change.
- No Vendor lifecycle shall change.
- No event sequencing shall change.
- No additional integration events shall be introduced.
- The **VendorRegistered** integration event shall remain the authoritative business fact representing successful Vendor Registration.
- Published events shall contain sufficient business information to support downstream processing without synchronous callbacks into the Vendor Domain.
- HJ-004 remains the authoritative definition of the Vendor Domain event model.
- HJ-105 shall apply those event definitions consistently within the registration workflow.
- Implementation-specific serialization formats, transport metadata and messaging technology remain outside the scope of these documents.

---

# 5. Expected Outcome

Following this change:

- The minimum business payload of the **VendorRegistered** integration event is explicitly defined.
- HJ-004 and HJ-105 consistently define the same minimum integration-event contract.
- Every successful Vendor Registration publishes sufficient business information to initiate the Pending Activation Process.
- The Pending Activation Process does not require synchronous callbacks into the Vendor Domain to obtain registration information already known at registration time.
- The published event represents a complete business fact rather than merely notifying downstream consumers that additional data should be retrieved.
- Runtime coupling between the Vendor Domain and downstream business capabilities is reduced.
- The event-driven architecture remains aligned with the Architectural Principles governing explicit published contracts, capability ownership and business autonomy.
- The ambiguity identified as **Important Finding #7** in the independent review of HJ-106 is eliminated.

# CR-033 – Align HJ-105 with Epic 1 Implementation Scope

## 1. Change Summary

Update **HJ-105 – Vendor Registration Sequence Diagram** to align its Epic 1 runtime interactions and implementation boundary with the approved **HJ-011 – Epic 1 Vendor Registration Implementation Scope**.

The change shall:

- retain real `VendorRegistered` Integration Event publication through the Event Bus;
- replace Epic 1 Pending Activation and Compliance Requirement processing with the approved **Compliance Event Consumer Stub**;
- remove the Compliance Requirement Provider abstraction and stub from the Epic 1 implementation boundary;
- make clear that Pending Activation processing and real Compliance processing are outside Epic 1; and
- align the Epic 1 client sequence with the approved **Web-client-owned Registration Session**, with no BFF implementation.

No Vendor Registration business rules, Vendor aggregate behaviour, Address ownership, Integration Event contract, persistence, idempotency or reliable-publication decisions are changed.

## 2. Reason for Change

HJ-011 establishes the authoritative Epic 1 implementation boundary.

HJ-105 v3.1 currently extends the Epic 1 asynchronous path beyond that boundary by requiring:

- initiation of the Pending Activation Process from `VendorRegistered`;
- a Compliance Requirement Provider abstraction; and
- an Epic 1 Compliance Requirement Provider stub.

HJ-011 instead requires the Epic 1 asynchronous path to terminate at a passive **Compliance Event Consumer Stub** that receives, deserializes, validates and records the published `VendorRegistered` Integration Event.

HJ-105 also describes the Epic 1 client implementation repeatedly as `Registration UI / BFF`, whereas HJ-011 explicitly selects the Web client as the Registration Session owner for Epic 1 and places BFF implementation out of scope.

HJ-105 must therefore be aligned with the authoritative implementation scope while preserving the wider architectural model for later Epics.

## 3. Required Changes to HJ-105

### 3.1 Document Control

Update HJ-105 as follows:

- increment Version from **3.1** to **3.2**;
- set **Last Updated** to **12 August 2026**;
- add a Revision History entry describing the HJ-011 scope alignment; and
- add **HJ-011 – Epic 1 Vendor Registration Implementation Scope** to Related Documents as the authoritative Epic 1 implementation-scope source.

### 3.2 Section 1 – Purpose and Authority

Add HJ-011 to the list of authoritative sources for implementation-scope concerns.

The authority statement shall make clear that:

- HJ-105 defines runtime interaction behaviour; and
- HJ-011 governs whether a capability is implemented, excluded or stubbed for Epic 1.

### 3.3 Section 2 – Scope and Boundary

Replace the Epic 1 scope item:

> initiation of the Pending Activation Process;

with:

> end-to-end delivery of the published `VendorRegistered` Integration Event to the Compliance Event Consumer Stub for receipt, deserialization, contract validation and observable recording.

Explicitly add to the excluded/deferred scope:

- Pending Activation Process initiation and processing;
- determination of Compliance Requirements;
- Compliance Requirement Provider implementation; and
- all real Compliance Domain processing.

Retain Vendor creation in `PendingActivation` as the resulting Vendor lifecycle state. This change concerns **processing of the Pending Activation Process**, not the Vendor's initial lifecycle state.

### 3.4 Section 3 – Architectural Rules Applied

Replace the current **Capability autonomy** rule that states that the Pending Activation Process consumes the Integration Event.

The revised rule shall state that, for Epic 1:

- the published `VendorRegistered` Integration Event is delivered to the Compliance Event Consumer Stub;
- the stub verifies the published contract without synchronously querying the Vendor Domain; and
- real Pending Activation and Compliance processing are deferred.

The architectural principle that future downstream capabilities consume the published contract without synchronously querying Vendor remains unchanged.

### 3.5 Section 4 – Participants and Responsibilities

Remove the following Epic 1 participants:

- **Pending Activation Process**
- **Compliance Requirement Provider**

Add:

| Participant | Responsibility |
| --- | --- |
| **Compliance Event Consumer Stub** | Epic 1 downstream integration stub that subscribes to `VendorRegistered`, receives and deserializes the Integration Event, validates the agreed contract and records receipt for integration verification and observability. It performs no Compliance domain behaviour. |

Update the client participant from:

> Registration UI / BFF

to the Epic 1 implementation:

> Vendor Web Client

The description shall state that the Web client owns temporary interaction state and the Registration Session for Epic 1.

### 3.6 Section 6 – Client Assembly and Address Selection

Align the Epic 1 sequence with HJ-011 by replacing `Registration UI / BFF` references with **Vendor Web Client**.

The sequence shall continue to show:

- client-owned transient interaction state;
- Address search and selection outside the Vendor Registration service boundary; and
- assembly of one complete `RegisterVendor` request before invocation.

Do not introduce a BFF participant.

### 3.7 Section 7 – Successful Vendor Registration

Revise the asynchronous portion of the sequence.

Remove:

```text
Bus -> Pending Activation Process: Deliver VendorRegistered Integration Event
Pending Activation Process -> Compliance Requirement Provider: Get applicable Compliance Requirements
Compliance Requirement Provider -> Pending Activation Process: Applicable Compliance Requirements
Pending Activation Process -> Pending Activation Process: Begin Pending Activation coordination
```

Replace with the equivalent sequence:

```text
Publisher -> Event Bus: Publish VendorRegistered Integration Event
Event Bus -> Compliance Event Consumer Stub: Deliver VendorRegistered Integration Event
Compliance Event Consumer Stub -> Compliance Event Consumer Stub: Deserialize and validate Integration Event contract
Compliance Event Consumer Stub -> Compliance Event Consumer Stub: Record receipt for verification and observability
```

Replace the accompanying explanatory text so that it states:

- the synchronous response confirms committed Vendor state;
- reliable event publication proceeds asynchronously;
- Epic 1 verifies successful downstream receipt through the Compliance Event Consumer Stub; and
- Pending Activation and real Compliance processing are outside the Epic 1 implementation boundary.

Do not imply that registration itself completes Compliance processing.

### 3.8 Section 9 – Idempotent Replay and Concurrency

Remove the **Pending Activation Process** participant from the sequence diagram.

Replace any replay/concurrency note that prohibits an additional Pending Activation Process with wording that prohibits:

- duplicate Vendor creation;
- duplicate Domain Events;
- duplicate outbox/publication records; and
- duplicate Integration Event publication resulting from replay of the same successful registration.

No Compliance Requirement creation or Pending Activation execution is part of the Epic 1 idempotency boundary.

### 3.9 Section 10.1 – Minimum VendorRegistered Integration Event Contract

Retain the minimum `VendorRegistered` Integration Event contract.

The Integration Event remains the published contract intended to support eventual downstream Compliance and Pending Activation processing.

Revise wording that currently states that the payload must be sufficient for the Pending Activation Process and Compliance capability **to begin processing within Epic 1**.

The section shall distinguish:

- **contract purpose** – the event carries the approved registration-time information required by eventual downstream Compliance processing without a synchronous Vendor callback; from
- **Epic 1 implementation scope** – the Compliance Event Consumer Stub verifies that the contract can be received and deserialized but performs no Compliance or Pending Activation business processing.

The concrete Business Address payload deferral remains unchanged.

### 3.10 Section 11 – Client-Owned Registration Session Lifecycle

Align Epic 1 participant terminology with HJ-011.

Where the sequence represents the actual Epic 1 implementation, replace `Registration UI / BFF` with **Vendor Web Client**.

Preserve the architectural principle that the Registration Session is outside the Vendor Registration service boundary.

Do not introduce server-side Registration Session persistence.

### 3.11 Section 15.1 – Epic 1 Implementation Boundary / Required

Remove:

- `Pending Activation initiation without a synchronous Vendor Domain callback`;
- `Compliance Requirement Provider abstraction and Epic 1 stub implementation`.

Add:

- real Event Bus / Message Broker delivery of the published `VendorRegistered` Integration Event;
- Compliance Event Consumer Stub;
- successful receipt and deserialization of `VendorRegistered`;
- validation of the agreed Integration Event contract; and
- observable recording of event receipt.

Retain:

- Vendor creation in `PendingActivation`;
- atomic Vendor persistence and durable publication recording;
- distinct Domain and Integration Event representations; and
- reliable `VendorRegistered` Integration Event publication.

### 3.12 Section 15.2 – Outside the Vendor Registration Service

Where this section describes Epic 1 client implementation, align references with the **Web-client-owned Registration Session**.

The wider architectural rule that Registration Session state is outside the Vendor Registration service remains unchanged.

### 3.13 Section 15.3 – Deferred

Ensure the deferred list explicitly includes:

- Pending Activation Process initiation and execution;
- determination of Compliance Requirements;
- Compliance Requirement Provider implementation; and
- real Compliance Domain processing.

Retain the existing deferred items including evidence workflows, Vendor Activation and full Compliance Domain implementation.

### 3.14 Section 16 – Design Decisions

Replace **Decision 10: Downstream capabilities consume the published contract** with wording that preserves the architectural decision while aligning its Epic 1 implementation.

Use:

#### Decision 10: Epic 1 verifies the published downstream contract without implementing downstream business processing

`VendorRegistered` is reliably published as the external collaboration contract. For Epic 1, the Compliance Event Consumer Stub receives, deserializes, validates and records the Integration Event. Pending Activation and real Compliance processing are deferred. Future downstream capabilities shall consume the published Integration Event without synchronously querying the Vendor Domain for registration information already present in the contract.

This change does not reverse the event-driven architecture. It narrows only the Epic 1 implementation boundary.

### 3.15 Section 17 – Implementation Checklist

Remove the checklist item:

> the Pending Activation Process uses the Integration Event without a synchronous Vendor Domain callback

Replace it with checks confirming that:

- the `VendorRegistered` Integration Event is delivered through the real Event Bus / Message Broker;
- the Compliance Event Consumer Stub receives the event;
- the stub successfully deserializes and validates the agreed Integration Event contract;
- receipt is recorded for automated verification and observability;
- the stub performs no Compliance business behaviour; and
- Epic 1 does not initiate the Pending Activation Process or determine Compliance Requirements.

Where checklist items refer to the Epic 1 client implementation, align them with the Web-client-owned Registration Session and absence of a BFF.

### 3.16 Section 18 – Traceability

Add:

| Behaviour | Authoritative source |
| --- | --- |
| Epic 1 implementation boundary | HJ-011 |
| Compliance Event Consumer Stub | HJ-011 |
| Web-client-owned Registration Session for Epic 1 | HJ-011 |

Retain the existing traceability to ADR-003, ADR-007 and HJ-004 for the **target architectural relationship** between `VendorRegistered`, Pending Activation and Compliance.

Those sources continue to describe the wider architecture; HJ-011 determines that their real downstream implementation is deferred beyond Epic 1.

## 4. Explicit Non-Changes

This Change Request does not change:

- Vendor creation in `PendingActivation`;
- Trading Preference `Offline`;
- the separation of registration from activation;
- the existence or purpose of the Pending Activation Process in the wider architecture;
- Compliance as a separate bounded context;
- Address Domain ownership or the Epic 1 Address stub;
- the `VendorRegistered` Domain Event;
- the `VendorRegistered` Integration Event;
- the minimum Integration Event business payload;
- atomic Vendor persistence and publication recording;
- reliable event publication;
- Register Vendor idempotency;
- Retrieve Registered Vendor behaviour; or
- the principle that future downstream capabilities consume published contracts rather than synchronously querying the Vendor Domain.

## 5. Acceptance Criteria

CR-033 is complete when HJ-105:

- contains no requirement to implement or initiate Pending Activation processing in Epic 1;
- contains no Epic 1 Compliance Requirement Provider abstraction or stub;
- represents the Compliance Event Consumer Stub as the sole downstream Epic 1 consumer of `VendorRegistered`;
- shows successful receipt, deserialization, contract validation and observable recording of that event;
- contains no Compliance business processing in the Epic 1 successful sequence;
- identifies real Pending Activation and Compliance processing as deferred;
- uses the Vendor Web Client, rather than a BFF, for the implemented Epic 1 Registration Session;
- preserves the target event-driven architecture for later downstream capabilities; and
- is consistent with HJ-011 as the authoritative Epic 1 implementation-scope artefact.
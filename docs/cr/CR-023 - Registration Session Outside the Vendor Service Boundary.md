# CR-023 – Registration Session Outside the Vendor Service Boundary

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-023 |
| **Title** | Registration Session Outside the Vendor Service Boundary |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Critical |
| **Affected Documents** | HJ-003 – Ubiquitous Language Guide; HJ-104 – Vendor Registration Fields Matrix; HJ-105 – Vendor Registration Sequence Diagram; HJ-106 – Vendor Registration Service Contract; ADR-004 – Business Capability Ownership; ADR-008 – Idempotent Operations and Reliable Event Publication |

---

# 1. Background

The approved architecture now establishes that a **Registration Session** is an interaction concern owned by the Registration UI or a Backend-for-Frontend (BFF).

It exists solely to assist a prospective Vendor in assembling a complete Vendor Registration request before invoking the Vendor Registration service.

A Registration Session:

- is not owned by the Vendor Domain;
- is not owned by the Vendor Registration service;
- is not a Domain Entity;
- produces no domain events;
- is transient interaction state only; and
- has no business significance once the registration request has been submitted.

The Vendor Registration service therefore has no knowledge of Registration Sessions and is concerned only with processing a complete **RegisterVendor** request.

This change aligns the architectural artefacts with that approved boundary and removes any remaining implication that the Vendor Registration service creates, updates, stores, expires or discards Registration Sessions.

---

# 2. Objectives

The revised artefacts shall:

- establish that Registration Session ownership belongs exclusively to the Registration UI or Backend-for-Frontend;
- remove Registration Session from the Vendor Registration service boundary;
- ensure the Vendor Registration service receives only a complete Register Vendor request;
- update HJ-105 so that all sequence diagrams reflect the approved architectural boundary;
- preserve all existing business behaviour; and
- ensure all affected documents consistently distinguish interaction behaviour from business processing.

---

# 3. Required Changes

## 3.1 ADR-004 – Business Capability Ownership

Update the capability ownership guidance to state that:

- Registration Session ownership belongs exclusively to the Registration UI or Backend-for-Frontend;
- the Vendor Registration capability neither owns nor manages Registration Sessions;
- Registration Session lifecycle management is outside the Vendor service boundary; and
- Vendor Registration begins only when a complete Register Vendor request is received.

No changes shall be made to existing business capability boundaries.

---

## 3.2 ADR-008 – Idempotent Operations and Reliable Event Publication

Where Register Vendor processing is described, clarify that:

- idempotency applies to the complete Register Vendor request;
- Registration Session state is outside the scope of idempotency;
- the Vendor Registration service is independent of how the registration payload was assembled.

No Registration Session behaviour shall be introduced into ADR-008.

---

## 3.3 HJ-003 – Ubiquitous Language Guide

Update the definition of **Registration Session** to state that:

- it is a client-side or Backend-for-Frontend interaction concept;
- it exists only while assembling a Vendor Registration request;
- it is not part of the Vendor Domain;
- it is not processed by the Vendor Registration service;
- it is discarded by its owner once no longer required.

The definition shall continue to distinguish Registration Session from Vendor Registration.

---

## 3.4 HJ-104 – Vendor Registration Fields Matrix

Revise **Section 6 – Assumptions and Outstanding Decisions**.

Replace the assumptions relating to Registration Session processing with assumptions stating that:

- Vendor Registration processes a complete Register Vendor request;
- Registration Session ownership belongs exclusively to the Registration UI or Backend-for-Frontend;
- Vendor Registration neither creates, stores, retrieves nor discards Registration Sessions;
- temporary interaction state is outside the Vendor service boundary.

Retain the purpose of Section 6.

Do not convert the section into workflow documentation.

Update any traceability references so they refer to:

- Registration Session ownership; and
- Register Vendor request processing.

---

## 3.5 HJ-105 – Vendor Registration Sequence Diagram

### Participant List

Revise the participant list so that the Registration Session is no longer represented as a participant within the Vendor Registration sequence.

Remove:

- **Registration Session**

Retain:

- Prospective Vendor;
- Registration UI (or Backend-for-Frontend);
- Vendor Registration Application;
- Address Service;
- Vendor Aggregate;
- Vendor Repository;
- Outbox / Event Publisher;
- Pending Activation Process; and
- Compliance Requirement Provider.

Where appropriate, clarify that the Registration UI (or Backend-for-Frontend) owns any temporary interaction state required while the user is completing registration.

---

### Happy Path Registration Sequence

Rewrite the Happy Path sequence diagram so that:

- the Registration Session participant disappears completely;
- no Registration Session is created, updated or discarded by the Vendor Registration Application;
- the Registration UI (or Backend-for-Frontend) is responsible for collecting registration information;
- the Registration UI (or Backend-for-Frontend) performs address search and selection;
- the Registration UI (or Backend-for-Frontend) assembles the complete registration payload before submission.

Immediately before registration, include a note stating:

> The Registration UI (or Backend-for-Frontend) is responsible for assembling the complete registration payload before invoking Vendor Registration.

The only registration interaction with the Vendor Registration service shall be:

```text
RegisterVendor(complete registration payload)
```

Following submission, the Vendor Registration Application shall perform only:

- server-side validation;
- Vendor creation;
- persistence;
- reliable event publication; and
- initiation of the Pending Activation Process.

No Registration Session interaction shall appear anywhere within the server-side sequence.

---

### Registration Validation Failure

Rewrite the Validation Failure sequence so that:

- Registration Session is removed as a participant;
- no Registration Session is retained by the Vendor Registration service.

Validation errors shall be returned directly to the Registration UI (or Backend-for-Frontend).

The Registration UI (or Backend-for-Frontend) remains responsible for retaining any temporary interaction state required for user correction.

No server-side interaction state shall be represented.

---

### Registration Session Expiry

Revise this section so that Registration Session expiry is described solely as a Registration UI or Backend-for-Frontend concern.

The Vendor Registration service shall not participate in:

- Registration Session creation;
- Registration Session persistence;
- Registration Session expiry; or
- Registration Session disposal.

If a sequence diagram is retained, it shall involve only:

- the Prospective Vendor;
- the Registration UI (or Backend-for-Frontend); and
- any local interaction-state mechanism.

The Vendor Registration Application shall not appear within the Registration Session expiry sequence.

---

### Address Resolution

Retain the existing Address Resolution behaviour.

Where interaction state is implied, ensure it is represented as owned by the Registration UI (or Backend-for-Frontend).

Address Resolution shall continue to occur before submission of the completed Register Vendor request.

No Registration Session participant shall appear.

---

### Sequence Diagram Consistency

Review every sequence diagram within HJ-105 and ensure that:

- Registration Session never appears as a server-side participant;
- Registration Session is never represented as a Domain concept;
- temporary interaction state belongs exclusively to the Registration UI (or Backend-for-Frontend);
- Vendor Registration begins only when a complete registration payload is submitted.

Across all diagrams, the only registration operation exposed by the Vendor Registration service shall be:

```text
RegisterVendor(complete registration payload)
```

No alternative server-side registration interaction shall be introduced.

---

### Narrative Updates

Update surrounding narrative wherever necessary to reinforce that:

- Registration Sessions are interaction concerns;
- Registration Sessions exist entirely outside the Vendor service boundary;
- the Registration UI (or Backend-for-Frontend) assembles the complete registration payload;
- the Vendor Registration service has no knowledge of how the payload was assembled;
- Vendor Registration always begins with a complete Register Vendor request.

---

## 3.6 HJ-106 – Vendor Registration Service Contract

Update the service contract so that:

- Register Vendor accepts a complete registration request;
- Registration Session is not referenced within the service contract;
- the contract makes no assumptions regarding client interaction state;
- Register Vendor remains the single business operation exposed by the Vendor Registration service.

---

# 4. Editorial Principles

- No business capability shall change.
- No Vendor lifecycle shall change.
- No validation rules shall change.
- No event publication behaviour shall change.
- No Address Domain responsibilities shall change.
- Registration Session shall remain outside the Vendor service boundary.
- Temporary interaction state belongs exclusively to the Registration UI or Backend-for-Frontend.
- The Vendor Registration service shall expose one business operation only:

```text
RegisterVendor(complete registration payload)
```

- All documents shall consistently distinguish interaction behaviour from business processing.
- Existing business behaviour shall be preserved.

---

# 5. Expected Outcome

Following this change:

- Registration Session ownership is consistently defined as belonging to the Registration UI or Backend-for-Frontend.
- Registration Session is removed from the Vendor Registration service boundary.
- The Registration Session participant disappears from every server-side sequence diagram within HJ-105.
- The Registration UI (or Backend-for-Frontend) is explicitly responsible for assembling the complete registration payload.
- Vendor Registration begins only when a complete Register Vendor request is submitted.
- The only server interaction for registration is:

```text
RegisterVendor(complete registration payload)
```

- The Vendor Registration service no longer appears to create, update, retain, expire or discard Registration Sessions.
- HJ-003, HJ-104, HJ-105, HJ-106, ADR-004 and ADR-008 consistently describe the approved Registration Session ownership model.
- The approved separation between interaction concerns and server-side business processing is fully reflected throughout the architecture.

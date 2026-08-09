# CR-025 – Registration Session Ownership and Vendor Service Boundary Clarification

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-025 |
| **Title** | Registration Session Ownership and Vendor Service Boundary Clarification |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Critical |
| **Affected Documents** | ADR-004 – Vendor Lifecycle Begins After Successful Registration |

---

# 1. Background

The approved architecture has evolved to establish a clear separation between client interaction concerns and server-side business processing.

ADR-004 currently states:

> Vendor registration shall be performed using a transient Registration Session.

While this accurately reflected the original architectural direction, it is now inconsistent with the approved architecture documented in HJ-003, HJ-004, HJ-104 and ADR-008.

The approved architecture establishes that a Registration Session is not part of the Vendor Domain and is not owned by the Vendor Registration capability. Instead, it is an interaction concern owned by the Registration UI or a Backend-for-Frontend (BFF) and exists solely to assist a prospective Vendor in assembling a complete registration request before invoking the Vendor Registration service.

ADR-004 also contains outdated Related Document statuses, continuing to identify HJ-003 and HJ-004 as Draft despite their approved architectural revisions.

Although ADR-002 remains the authoritative architectural decision governing Business Capability Ownership and bounded contexts, ADR-004 should become the authoritative architectural source describing:

- Registration Session ownership in the context of Vendor Registration;
- the Vendor Registration service boundary; and
- the point at which the Vendor lifecycle begins.

Once this Change Request has been applied, HJ-105 shall be able to consume ADR-004 directly without requiring additional clarification from CR-023.

---

# 2. Objectives

The revised ADR shall:

- align the Vendor Registration decision with the approved service boundary;
- establish that Registration Session ownership belongs to the Registration UI or Backend-for-Frontend;
- remove any implication that the Vendor Registration capability owns Registration Sessions;
- define the point at which Vendor Registration begins;
- define the point at which the Vendor lifecycle begins;
- clarify that incomplete client interaction state has no business significance;
- reinforce the separation between interaction concerns and business processing; and
- update ADR metadata, revision history and related-document statuses.

---

# 3. Required Changes

## 3.1 Document Metadata

Update the document metadata as appropriate.

Update:

- Version;
- Last Updated;
- Revision History; and
- Related Documents.

Ensure Related Document statuses accurately reflect the current architectural baseline, including HJ-003 and HJ-004.

---

## 3.2 Registration Session Ownership

Replace the existing architectural decision stating that:

> Vendor registration shall be performed using a transient Registration Session.

with guidance consistent with the approved architecture.

The revised decision shall state, in substance, that:

- a Registration UI or Backend-for-Frontend may use a transient Registration Session to assist a prospective Vendor while assembling a registration request;
- the Registration Session is owned entirely by the Registration UI or Backend-for-Frontend;
- the Registration Session exists outside the Vendor Domain and outside the Vendor Registration service boundary;
- Registration Session management is an interaction concern rather than a Vendor Domain responsibility.

The wording shall establish ownership without prescribing implementation technology.

---

## 3.3 Vendor Registration Service Boundary

Introduce an explicit description of the Vendor Registration service boundary.

State that:

- Vendor Registration begins only when a complete **RegisterVendor** request is received by the Vendor Registration capability;
- the Vendor Registration service has no knowledge of how the registration request was assembled;
- the Vendor Registration capability does not create, retrieve, inspect, update, persist, expire or dispose of Registration Sessions;
- Registration Session lifecycle management is entirely outside the Vendor Registration service boundary.

This definition shall become the authoritative description of the Vendor Registration service boundary within ADR-004.

---

## 3.4 Vendor Lifecycle Boundary

Clarify the relationship between client interaction state and business state.

State that:

- the Vendor lifecycle begins only after successful processing of a complete **RegisterVendor** request;
- incomplete client interaction state does not create a Vendor;
- incomplete client interaction state records no business fact;
- incomplete client interaction state records no Domain Event; and
- incomplete client interaction state publishes no Integration Event.

This distinction shall reinforce that client interaction state has no business significance until successful registration processing has completed.

---

## 3.5 Capability Boundary Clarification

Update the architectural guidance to reinforce that:

- Registration Session ownership belongs exclusively to the Registration UI or Backend-for-Frontend;
- the Vendor Registration capability owns only business processing performed after receipt of a complete RegisterVendor request;
- interaction-state management and business-state management are separate architectural responsibilities; and
- the Vendor Registration capability shall not assume ownership of client interaction concerns.

No business capability boundaries shall otherwise change.

---

## 3.6 Cross-Document Consistency

Update ADR-004 so that it is fully consistent with:

- HJ-003 – Ubiquitous Language Guide;
- HJ-004 – Vendor Domain Models;
- HJ-104 – Vendor Registration Fields Matrix; and
- ADR-008 – Idempotent Operations and Reliable Event Publication.

ADR-004 shall become the authoritative architectural source describing:

- Registration Session ownership in the context of Vendor Registration;
- the Vendor Registration service boundary; and
- the beginning of the Vendor lifecycle.

---

# 4. Explicit Non-Requirements

This Change Request intentionally does **not** prescribe:

- user-interface implementation;
- Backend-for-Frontend implementation;
- client storage mechanisms;
- browser storage technology;
- session persistence technology;
- session timeout duration;
- session expiration policy;
- request transport mechanisms; or
- client workflow implementation.

These remain implementation decisions provided the architectural ownership and service boundary defined by ADR-004 are preserved.

---

# 5. Relationship to Existing Change Requests

This Change Request formalises within ADR-004 the architectural direction established by:

- **CR-023 – Registration Session Outside the Vendor Service Boundary**; and
- **CR-024 – Register Vendor Idempotency Boundary and Reliable Publication Clarification**.

It does not replace those Change Requests or alter their scope.

Following completion of this Change Request:

- ADR-004 shall become the authoritative architectural source describing Registration Session ownership in the context of Vendor Registration, the Vendor Registration service boundary and the beginning of the Vendor lifecycle.
- HJ-105 and future workflow documentation shall reference ADR-004 directly for these architectural decisions rather than relying upon CR-023.

---

# 6. Editorial Principles

- Preserve all existing architectural decisions unless explicitly changed.
- No business capability shall change.
- No Vendor lifecycle behaviour shall change.
- No Domain boundaries shall change.
- Strengthen architectural clarity without prescribing implementation.
- Clearly distinguish interaction concerns from server-side business processing.
- Maintain consistency with ADR-002, HJ-003, HJ-004, HJ-104 and ADR-008.
- Ensure all guidance remains technology-neutral.
- Preserve the architectural authority hierarchy by leaving general Business Capability Ownership decisions within ADR-002.

---

# 7. Expected Outcome

Following this change:

- ADR-004 consistently reflects the approved Vendor Registration service boundary.
- Registration Session is explicitly defined as a client or Backend-for-Frontend interaction concern.
- Registration Session is explicitly outside the Vendor Domain and Vendor Registration service boundary.
- Vendor Registration is defined as beginning only with receipt of a complete **RegisterVendor** request.
- The Vendor Registration capability is explicitly prohibited from creating, retrieving, inspecting, updating, persisting, expiring or disposing of Registration Sessions.
- The Vendor lifecycle begins only after successful processing of a complete registration request.
- Incomplete client interaction state creates no Vendor, records no business fact, records no Domain Event and publishes no Integration Event.
- ADR metadata, revision history and related-document statuses are updated.
- ADR-004 becomes the authoritative architectural source describing Registration Session ownership in the context of Vendor Registration, the Vendor Registration service boundary and the beginning of the Vendor lifecycle, while ADR-002 remains the authoritative architectural decision governing Business Capability Ownership and bounded contexts.

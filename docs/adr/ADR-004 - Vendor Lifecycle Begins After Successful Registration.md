# ADR-004 – Vendor Lifecycle Begins After Successful Registration

| **Document ID** | ADR-004 |
|-----------------|---------|
| **Document Title** | Vendor Lifecycle Begins After Successful Registration |
| **Version** | 1.2 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 28 July 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 23 July 2026 | Initial Architectural Decision Record. |
| 1.1 | 23 July 2026 | Reframed the decision around the business significance of Vendor Registration and the intentional use of transient Registration Sessions. |
| 1.2 | 28 July 2026 | Applied CR-025 to establish client/BFF ownership of Registration Sessions, define the Vendor Registration service boundary, clarify when Vendor Registration and the Vendor lifecycle begin, and update related-document statuses. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted |
| ADR-008 | Idempotent Operations and Reliable Event Publication | Accepted |
| HJ-003 | Ubiquitous Language Guide | Approved |
| HJ-004 | Vendor Domain Models | Approved |
| HJ-104 | Vendor Registration Fields Matrix | Approved |

---

# 1. Context

Vendor Registration is the business process through which a prospective Vendor expresses a deliberate intention to join the HotJoes platform.

The purpose of registration is to validate the complete information required to establish a new Vendor. Until that process has completed successfully, no business relationship exists between the prospective Vendor and the platform.

A Registration UI or Backend-for-Frontend (BFF) may use a transient Registration Session to assist the prospective Vendor while assembling a registration request. The Registration Session is client interaction state, not Vendor business state.

Incomplete, abandoned or expired client interaction state has no business significance. The business does not recognise partially completed registrations as entities requiring management, tracking or lifecycle states.

The architecture therefore requires a clear separation between:

- client-owned interaction state;
- the Vendor Registration service boundary; and
- the lifecycle of a successfully registered Vendor.

---

# 2. Decision

## 2.1 Registration Session Ownership

A Registration UI or BFF may use a transient Registration Session to assist a prospective Vendor while assembling a complete registration request.

The Registration Session:

- is owned entirely by the Registration UI or BFF;
- exists outside the Vendor Domain;
- exists outside the Vendor Registration service boundary;
- is an interaction concern rather than a Vendor Domain responsibility;
- is not a Domain Entity and forms no part of the Vendor aggregate or Vendor lifecycle;
- produces no Domain Events or Integration Events; and
- cannot be resumed after it has been abandoned or expired.

Registration Session management—including creation, storage, update, expiry and disposal—is entirely outside the Vendor Registration service boundary.

This decision establishes architectural ownership without prescribing user-interface, BFF, client-storage or session-management technology.

---

## 2.2 Vendor Registration Service Boundary

Vendor Registration begins only when the Vendor Registration capability receives a complete `RegisterVendor` request.

The Vendor Registration service:

- has no knowledge of how the registration request was assembled;
- does not create, retrieve, inspect, reconcile, update, persist, expire or dispose of Registration Sessions; and
- owns only the business processing performed after receipt of the complete request.

The complete request contains the client-authored registration information and approved references required to validate and create the Vendor. Information owned by another Domain is obtained from that authoritative Domain in accordance with the applicable architectural decision and published contract.

Interaction-state management and business-state management are separate architectural responsibilities. The Vendor Registration capability shall not assume ownership of client interaction concerns.

---

## 2.3 Vendor Lifecycle Boundary

Successful processing of a complete and valid `RegisterVendor` request represents a clear business commitment to join the platform and results in the creation of a new Vendor aggregate.

The Vendor lifecycle begins only when that Vendor is successfully created in the **Pending Activation** state.

Incomplete, abandoned or expired client interaction state:

- does not create a Vendor or any other persistent business entity;
- records no completed business fact;
- records no Domain Event;
- publishes no Integration Event; and
- is not recoverable as Vendor business state.

This decision intentionally avoids introducing a draft registration lifecycle. Only successful Vendor Registration creates durable business state.

---

# 3. Consequences

## Positive

- Registration Session ownership is explicit and remains outside the Vendor service boundary.
- The Vendor Registration capability has one clear starting point: receipt of a complete `RegisterVendor` request.
- The Vendor aggregate always represents a valid business entity.
- The Vendor lifecycle contains only meaningful business states.
- Client interaction state remains separate from Vendor lifecycle management.
- Abandoned registrations require no business or operational management within the Vendor capability.
- Successful registration is a clear business milestone that can be communicated to other bounded contexts.
- Future changes to the registration experience can be made without affecting the Vendor Domain or service boundary.
- Workflow artefacts can reference ADR-004 directly for Registration Session ownership, the Vendor Registration service boundary and the beginning of the Vendor lifecycle.

## Negative

- Users cannot resume abandoned or expired Registration Sessions as Vendor business state.
- Registration interaction analytics and any permitted audit records must be captured independently of the Vendor aggregate.
- The UI or BFF is responsible for managing any temporary interaction state it requires.
- Future support for resumable registration would require a new architectural decision and an explicitly owned model outside the current Vendor lifecycle.

---

# 4. Alternatives Considered

## Vendor Registration Owns the Registration Session

Rejected because interaction-state management is not Vendor business processing. Making the Vendor Registration capability responsible for session creation, retrieval, update, expiry or disposal would blur the service boundary and couple Vendor Registration to a particular onboarding experience.

---

## Persist Draft Vendors

Rejected because partially completed registrations do not represent recognised business entities. Introducing draft Vendors would unnecessarily couple the Vendor lifecycle to the registration interaction and introduce lifecycle states that have no business meaning.

---

## Persist Registration Sessions in the Vendor Domain

Rejected because a Registration Session is client-owned interaction state rather than a Domain Entity. Persisting it within the Vendor Domain would introduce lifecycle, security and operational responsibilities without a corresponding Vendor business requirement.

---

## Begin Vendor Registration Before Complete Request Submission

Rejected because it would require the Vendor Registration capability to understand or manage partially assembled interaction state. Vendor Registration begins only when a complete `RegisterVendor` request is received.

---

# 5. Authority and Scope

ADR-002 remains the authoritative architectural decision governing general Business Capability Ownership and bounded contexts.

ADR-004 is the authoritative architectural source for:

- Registration Session ownership in the context of Vendor Registration;
- the Vendor Registration service boundary; and
- the point at which the Vendor lifecycle begins.

HJ-105 and future Vendor Registration workflow documentation shall apply these decisions directly.

This ADR does not prescribe:

- user-interface or BFF implementation;
- client or browser storage mechanisms;
- session persistence technology;
- session timeout duration or expiration policy;
- request transport mechanisms; or
- client workflow implementation.

These remain implementation decisions provided the ownership, service boundary and lifecycle boundary defined by this ADR are preserved.

---

# 6. Related Decisions

This decision builds upon:

- ADR-002 — Business Capabilities and Bounded Contexts

This decision supports:

- ADR-005 — Registered Information vs Vendor Managed Information
- ADR-007 — Vendor Compliance as a Separate Bounded Context
- ADR-008 — Idempotent Operations and Reliable Event Publication

---

# 7. References

- CR-025 — Registration Session Ownership and Vendor Service Boundary Clarification
- HJ-001 — Project Vision
- HJ-003 — Ubiquitous Language Guide
- HJ-004 — Vendor Domain Models
- HJ-104 — Vendor Registration Fields Matrix

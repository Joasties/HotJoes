# CR-017 – Register Vendor Idempotency Identity

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-017 |
| **Title** | Register Vendor Idempotency Identity |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Critical |
| **Affected Documents** | ADR-008 – Idempotent Operations and Reliable Event Publication; HJ-105 – Vendor Registration Sequence Diagram |

---

# 1. Background

HJ-105 currently states that duplicate submission must not create duplicate Vendors or duplicate business events. However, it does not define the business identity used to determine whether two submissions represent the same successful registration.

Similarly, ADR-008 establishes general principles for idempotent operations but does not explicitly identify **Register Vendor** as an operation that requires an explicit idempotency safeguard because successful execution creates new business state rather than updating existing state.

Without these clarifications, different implementations could legitimately choose different idempotency strategies, leading to inconsistent behaviour.

This change establishes the business requirement while deliberately leaving implementation details to the technical design.

---

# 2. Objectives

The revised artefacts shall:

- define the business meaning of **the same successful registration submission**;
- require Register Vendor to use an explicit idempotency identity or an equivalent uniqueness constraint;
- define the mandatory business behaviour when duplicate successful requests are received;
- distinguish mandatory business behaviour from implementation-specific technical conventions; and
- align HJ-105 with ADR-008 regarding operations that require explicit idempotency protection.

---

# 3. Required Changes

## 3.1 ADR-008 – Idempotent Operations and Reliable Event Publication

### Section 2.1 – Idempotent Operations

Add the following clarification immediately after the general description of naturally idempotent operations.

> **Register Vendor** is not naturally idempotent because successful execution creates new business state rather than simply setting an existing state.
>
> Consequently, Register Vendor shall be protected by an explicit idempotency safeguard, such as an idempotency identity or an equivalent uniqueness constraint, ensuring that repeated processing of the same successful business request cannot create duplicate Vendors or duplicate business events.

Retain all existing implementation guidance.

No implementation technology or storage mechanism shall be prescribed by this document.

---

## 3.2 HJ-105 – Vendor Registration Sequence Diagram

### Section 11 – Idempotency and Concurrency

Immediately following the introductory description of idempotency, add the following authoritative definition.

> **Register Vendor Idempotency Identity**
>
> For the Register Vendor use case, **the same successful submission** is defined by an explicit idempotency identity supplied with the registration request (or an equivalent uniqueness constraint derived from the complete validated registration payload).
>
> Reprocessing a request that carries the same idempotency identity and is semantically identical to a previously successful registration shall:
>
> - return the original successful outcome;
> - create no additional Vendor;
> - record no additional `VendorRegistered` business event; and
> - initiate no additional Pending Activation Process.
>
> The concrete format of the idempotency identity, its retention period, payload-equivalence rules and implementation mechanism are technical conventions.
>
> The business behaviour defined above is mandatory regardless of implementation.

Retain all existing guidance regarding duplicate submission caused by retries, concurrent processing or network failures.

No changes shall be made to the sequence diagrams.

---

# 4. Editorial Principles

- No business capability shall change.
- No Vendor lifecycle shall change.
- No event semantics shall change.
- No aggregate boundaries shall change.
- This change defines the business meaning of duplicate Register Vendor submissions without prescribing implementation.
- ADR-008 shall remain the authoritative document for idempotency principles.
- HJ-105 shall apply those principles specifically to the Register Vendor use case.
- Implementation-specific mechanisms, storage strategies and payload comparison algorithms remain outside the scope of these documents.

---

# 5. Expected Outcome

Following this change:

- Register Vendor has a single, unambiguous business definition of **the same successful submission**.
- All implementations are required to protect Register Vendor using an explicit idempotency safeguard or an equivalent uniqueness constraint.
- Duplicate processing of the same successful registration consistently:
  - returns the original outcome;
  - creates no additional Vendor;
  - records no additional `VendorRegistered` business event; and
  - initiates no additional Pending Activation Process.
- ADR-008 explicitly identifies Register Vendor as an operation requiring an explicit idempotency safeguard because it cannot be made naturally idempotent solely through state-setting.
- Technical implementation details remain implementation-specific while mandatory business behaviour is defined consistently across the architecture.

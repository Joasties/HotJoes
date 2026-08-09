# CR-015 – Canonical Address Identity and Address Snapshot Ownership

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-015 |
| **Title** | Canonical Address Identity and Address Snapshot Ownership |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Critical |
| **Affected Documents** | ADR-006 – Address Domain Ownership and Business Address Snapshots; HJ-004 – Vendor Domain Models; HJ-104 – Vendor Registration Fields Matrix; HJ-105 – Vendor Registration Sequence Diagram |

---

# 1. Background

ADR-006 establishes that the Address Domain is the authoritative owner of canonical Address information and that a Vendor stores:

- the Canonical Address Identifier; and
- an immutable Business Address Snapshot representing the approved address at the time of registration.

However, the current HJ-004 Vendor Domain Model only represents the Business Address Snapshot. The Canonical Address Identifier is not currently reflected consistently throughout the aggregate model, Registered Information definitions, supporting value objects, domain model diagram or implementation scope.

Furthermore, HJ-104 currently contains no representation of the Canonical Address Identifier, despite HJ-004 treating it as part of the Vendor's Registered Information. Unless HJ-104 is updated accordingly, the authoritative registration model would become inconsistent across the architectural artefacts.

This change aligns HJ-004 and HJ-104 with the architectural ownership model already established by ADR-006 and strengthens the trust boundary between the Vendor and Address Domains.

---

# 2. Objectives

The revised artefacts shall:

- explicitly model both the Canonical Address Identifier and the immutable Business Address Snapshot within the Vendor aggregate;
- establish that the Vendor maintains its relationship to the canonical Address through the Canonical Address Identifier;
- reinforce that all Address Snapshot and regulatory-authority information originates exclusively from the Address Domain;
- prohibit callers and the Vendor Domain from supplying, modifying or deriving Address-owned information;
- ensure HJ-004 and HJ-104 consistently represent the Canonical Address Identifier as Registered Information;
- ensure the aggregate model, Registered Information definitions, diagrams, field matrix and implementation scope consistently represent the approved architecture; and
- ensure ADR-006, HJ-004 and HJ-105 express an identical Address Snapshot Ownership Invariant so that no downstream artefact can interpret Business Address Snapshot or regulatory-authority information as client supplied.

---

# 3. Required Changes

## 3.1 ADR-006 – Address Domain Ownership and Business Address Snapshots

Within **Section 2 – Address Ownership Principles**, introduce the following invariant.

> **Address Snapshot Ownership Invariant**
>
> When a Vendor is created, the Vendor aggregate shall store only:
>
> - the Canonical Address Identifier returned by the Address Service; and
> - the immutable Business Address Snapshot exactly as returned by the Address Service for that identifier.
>
> Neither the Vendor Domain nor any caller shall supply, override, normalise, derive or invent Business Address Snapshot content or regulatory-authority values.
>
> Where a registration request presents an approved Address Resolution identifier or equivalent reference, the application shall obtain the Canonical Address Identifier, Business Address Snapshot and all derived regulatory-authority information directly from the Address Service.
>
> Any client-supplied Business Address Snapshot or regulatory-authority values shall be ignored or rejected.

---

## 3.2 HJ-004 – Vendor Domain Models

The Vendor Domain Model shall be updated so that the aggregate representation fully reflects the Address ownership model established by ADR-006.

### Section 1.3 – Registered Information

Add the **Canonical Address Identifier** as part of the Vendor's Registered Information.

The Registered Information shall therefore include:

- Canonical Address Identifier; and
- Business Address Snapshot.

---

### Section 1.4 – Core Domain Responsibility

Extend the existing description to state that:

> The Vendor maintains its relationship to the canonical Address exclusively through the stored Canonical Address Identifier while retaining an immutable Business Address Snapshot representing the approved address at the time of registration.

---

### Section 1.5 – Domain Boundaries – Address Domain

Strengthen the existing Address ownership description by introducing the following invariant.

> **Address Snapshot Ownership Invariant**
>
> When a Vendor is created, the Vendor aggregate shall store only:
>
> - the Canonical Address Identifier returned by the Address Domain; and
> - the immutable Business Address Snapshot exactly as returned by the Address Domain for that identifier.
>
> Neither the Vendor Domain nor any caller shall supply, override, normalise, derive or invent Business Address Snapshot content or regulatory-authority values.
>
> Where a registration request presents an approved Address Resolution identifier or equivalent reference, the application shall obtain the Canonical Address Identifier, Business Address Snapshot and all derived regulatory-authority information directly from the Address Domain.
>
> Any client-supplied Business Address Snapshot or regulatory-authority values shall be ignored or rejected.

---

### Section 1.6 – Registered Information

Update the Registered Information definition so that it is consistent with Section 1.3.

The Registered Information definition shall explicitly include:

- Canonical Address Identifier; and
- Business Address Snapshot.

---

### Section 2.2 – Vendor Properties

Add a new Vendor property:

- **CanonicalAddressId**

alongside the existing Business Address Snapshot.

---

### Section 2.3 – Supporting Value Objects

Introduce a new supporting value object:

- **CanonicalAddressId**

representing the immutable identifier assigned by the Address Domain to the canonical Address.

This value object represents the Vendor aggregate's durable relationship with the Address Domain and does not duplicate Address information owned by that bounded context.

---

### Section 8 – Aggregate Invariants

Add the following invariant.

> The Vendor aggregate shall store only the Canonical Address Identifier and the immutable Business Address Snapshot supplied by the Address Domain.
>
> Neither the Vendor Domain nor any caller shall construct, modify or replace Address-owned information.

---

### Section 9 – Vendor Domain Model Diagram

Update the domain model diagram to include:

- the **CanonicalAddressId** property on the Vendor aggregate;
- the association between the Vendor aggregate and the CanonicalAddressId value object; and
- an explicit association, dependency or equivalent diagram annotation showing that **CanonicalAddressId** is supplied by the Address Service (or originates from the Address Domain).

The diagram shall continue to show BusinessAddressSnapshot as a separate value object owned by the Vendor aggregate.

---

### Section 17 – Initial Implementation Scope

Update the implementation scope to include persistence of:

- the Canonical Address Identifier; and
- the immutable Business Address Snapshot.

Both shall form part of the initial implementation.

---

## 3.3 HJ-104 – Vendor Registration Fields Matrix

Update HJ-104 so that its authoritative registration model is consistent with HJ-004.

### Registration Field Matrix

Add a new Registration Field Matrix row for the **Canonical Address Identifier**.

The row shall be equivalent to:

| Field | Type | Required | Validation / Rules | Classification |
|---|---|---|---|---|
| **Canonical Address Identifier** | Derived | Yes | Supplied by the Address Domain from the approved Address Resolution. Not client-editable. | Registered Information |

The **Notes** for this field shall state that:

- the Canonical Address Identifier is supplied exclusively by the Address Domain;
- it is derived from the approved Address Resolution;
- it is not entered, edited or overridden by the Vendor;
- it is persisted together with the Business Address Snapshot as part of the Vendor's Registered Information; and
- it is governed by:
  - **Section 5.4**;
  - **Section 5.5**;
  - **ADR-006 – Address Domain Ownership and Business Address Snapshots**; and
  - **HJ-004 – Vendor Domain Models**.

---

### Registered Information

Include the **Canonical Address Identifier** within the Registered Information definition alongside the Business Address Snapshot.

---

### Derived Information

Where the document identifies information obtained from the Address Domain rather than entered directly by the Vendor, include the **Canonical Address Identifier** as a derived value obtained from the approved Address Resolution.

Clarify that the Canonical Address Identifier:

- is supplied by the Address Domain;
- is not entered or edited by the Vendor;
- forms part of the Vendor's Registered Information following successful registration; and
- is persisted together with the Business Address Snapshot.

---

### Address Information Notes

Strengthen the notes associated with the Business Address fields to state that:

- the Vendor supplies address search or selection inputs only;
- the Address Domain supplies the Canonical Address Identifier, Business Address Snapshot and regulatory-authority information;
- these values are derived from the approved Address Resolution and become Registered Information upon successful Vendor registration.

---

### Cross-References

Add appropriate cross-references to:

- ADR-006;
- HJ-004 Address Domain ownership; and
- the Address Resolution workflow defined in HJ-105.

HJ-104 shall not redefine Address ownership but shall consistently reference the authoritative architectural model.

---

## 3.4 HJ-105 – Vendor Registration Sequence Diagram

Within **Section 8 – Address Resolution**, strengthen the Address Resolution description by introducing the same invariant established in ADR-006 and HJ-004.

Add the following statement immediately following the description of the Address Service response.

> **Address Snapshot Ownership Invariant**
>
> When a Vendor is created, the Vendor aggregate shall store only:
>
> - the Canonical Address Identifier returned by the Address Service; and
> - the immutable Business Address Snapshot exactly as returned by the Address Service for that identifier.
>
> Neither the Vendor Domain nor any caller shall supply, override, normalise, derive or invent Business Address Snapshot content or regulatory-authority values.
>
> Where a registration request presents an approved Address Resolution identifier or equivalent reference, the Vendor Registration Application shall obtain the Canonical Address Identifier, Business Address Snapshot and all derived regulatory-authority information directly from the Address Service.
>
> Any client-supplied Business Address Snapshot or regulatory-authority values shall be ignored or rejected.

The accompanying sequence description shall also make it explicit that:

- the approved Address Resolution result is the sole authoritative source of:
  - the Canonical Address Identifier;
  - the immutable Business Address Snapshot;
  - Food Registration Authority; and
  - Primary Trading Authority, where applicable;
- Vendor Registration shall never trust client-supplied values for any of these items; and
- the Vendor aggregate shall persist only the values returned by the Address Domain.

---

# 4. Editorial Principles

- No new business capability shall be introduced.
- No additional Address lifecycle shall be introduced.
- No new bounded context shall be introduced.
- This change propagates the Address ownership model already established by ADR-006 throughout HJ-004, HJ-104 and HJ-105.
- HJ-004 shall consistently represent both the Canonical Address Identifier and the immutable Business Address Snapshot across textual descriptions, Registered Information definitions, aggregate properties, value objects, diagrams and implementation scope.
- HJ-104 shall consistently represent the Canonical Address Identifier as derived Registered Information originating from the Address Domain, including within its authoritative Registration Field Matrix.
- ADR-006, HJ-004 and HJ-105 shall contain the same Address Snapshot Ownership Invariant, expressed consistently so that no document can be interpreted as permitting client-supplied Business Address Snapshot or regulatory-authority information.
- HJ-104 shall reference, not redefine, Address ownership.
- All terminology shall remain consistent with HJ-003.

---

# 5. Expected Outcome

Following this change:

- HJ-004 consistently models the Vendor's relationship to the Address Domain through both the Canonical Address Identifier and the immutable Business Address Snapshot.
- HJ-104 becomes fully aligned with HJ-004 by recognising the Canonical Address Identifier as part of the Vendor's Registered Information.
- The authoritative Registration Field Matrix explicitly models the Canonical Address Identifier as derived information supplied by the Address Domain.
- The Canonical Address Identifier is consistently identified as derived from the Address Domain rather than supplied by the Vendor.
- ADR-006, HJ-004 and HJ-105 contain an identical Address Snapshot Ownership Invariant, ensuring that no downstream artefact, including HJ-106, can legitimately interpret the Business Address Snapshot or regulatory-authority values as client-provided.
- The Vendor Domain Model, Registration Field Matrix, Address Resolution workflow and implementation scope become internally consistent.
- Address ownership boundaries become explicit and enforceable throughout the architecture.
- Vendor Registration cannot inadvertently trust or persist client-authored Business Address Snapshot or regulatory-authority information.
- Future Service Contracts, APIs and implementations derive a complete and internally consistent Address collaboration model directly from the approved architectural artefacts.

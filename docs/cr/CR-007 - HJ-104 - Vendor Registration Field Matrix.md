# CR-007 – HJ-104 – Vendor Registration Fields Matrix Changes to Approved

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-007 |
| **Affected Document** | HJ-104 – Vendor Registration Fields Matrix |
| **Current Version** | 0.2 |
| **Target Version** | 1.0 |
| **Status** | Approved |
| **Owner** | Project Architecture |
| **Date** | 22 July 2026 |

---

# 1. Objective

Update HJ-104 to reflect all approved Vendor Registration decisions arising from Epic 1 domain modelling.

This change request promotes HJ-104 from Draft to the approved baseline for Vendor Registration.

---

# 2. Required Changes

## 2.1 Registration Field Matrix

### Company Registration Number

Replace the validation rule with:

> Mandatory where required by Legal Operator Type.
> Must match UK Companies House registration number format:
>
> `^(?:[A-Za-z]{2})?\d{6,8}$`
>
> Stored in canonical uppercase format.
> Validation confirms format only.

---

### Trading Location

Replace the existing controlled values with:

| Value | Description |
|---|---|
| Restaurant | Restaurant, Café, Takeaway or Food Market Hall. A customer-facing permanent premises. |
| Stall | Mobile Food Unit or Market Stall. |
| Kitchen | Dark Kitchen, Ghost Kitchen or Home Kitchen. A non-customer-facing food preparation venue that trades exclusively online. |

---

### Opening Hours

Replace "Representation to be confirmed" with:

> Opening Hours are represented by Start Time and End Time.

Add the validation rule:

> Validation shall not require Start Time to be earlier than End Time, allowing legitimate overnight trading periods (for example 23:00–05:00).

---

### Service Includes Hot Food

Replace the existing note with:

> Indicates whether the Vendor supplies food or drink heated above ambient room temperature.

Add:

> Used together with Opening Hours to determine applicable Compliance Requirements.

---

### Alcohol Service

Replace the existing note with:

> Indicates whether the Vendor supplies alcohol.

Add:

> Used to determine applicable Compliance Requirements.

---

## 2.2 Regulatory Authority Fields

Add the following Registered Information fields immediately after Business Address.

| Field | Type | Required | Validation / Rules | Classification | Notes |
|---|---|---|---|---|---|
| Food Registration Authority | Derived | Yes | Derived from the approved Business Address or mobile unit base address. | Registered Information | Provided by the Address Service. Not manually editable by the Vendor. |
| Primary Trading Authority | Derived / Conditional | Conditional | Required for Trading Location = Stall. | Registered Information | Represents the primary authority responsible for the Vendor's declared trading area. |

Renumber subsequent fields accordingly.

---

## 2.3 Website

Confirm Website remains within Epic 1.

No further change required.

---

## 2.4 Business Description

Confirm Business Description remains within Epic 1.

No further change required.

---

# 3. Assumptions

Replace the existing assumptions with the following additions.

Add:

- Registration Sessions are transient and cannot be resumed.
- Registration Sessions expire automatically and all collected information is discarded.
- A Vendor is created only after successful submission of a Registration Session.
- Business Address search, retrieval and validation are provided by a stubbed Address Service during Epic 1.
- The Vendor Domain stores an approved snapshot of the Business Address.
- Food Registration Authority is derived from the approved Business Address.
- Trading Characteristics determine Compliance Requirements.
- Compliance Requirements are obtained through a stubbed Compliance Provider during Epic 1.
- A Vendor represents a single trading location during Epic 1.
- Support for multiple premises is deferred to a future epic.

Retain all existing assumptions that remain valid.

---

# 4. Outstanding Decisions

Update Section 6.

Remove the following outstanding decisions:

- Confirm Trading Location controlled values.
- Confirm Opening Hours representation.
- Confirm Hot Food definition.
- Confirm Alcohol Service definition.
- UK company number validation rules.
- Address storage strategy.
- Future support for multiple premises.
- Whether Website should remain in Epic 1.
- Whether Business Description should remain in Epic 1.

Retain:

- Final Legal Operator Type values.

If the Legal Operator Types have already been approved elsewhere before this change is applied, remove this remaining outstanding decision and delete the Outstanding Decisions section if it becomes empty.

---

# 5. Preservation Requirements

- Preserve all existing document structure.
- Preserve heading numbering.
- Preserve markdown formatting.
- Preserve all unchanged content.
- Renumber the Registration Field Matrix where required.
- Update the document version to **1.0**.
- Update the document status to **Approved**.
- Update the Revision History to record this change.

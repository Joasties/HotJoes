# CR-009 – HJ-004 – Vendor Domain Models Changes to Approved

| Property | Value |
|----------|-------|
| **Change Request ID** | CR-009 |
| **Affected Document** | HJ-004 – Vendor Domain Models |
| **Current Version** | 1.2 |
| **Target Version** | 1.3 |
| **Status** | Approved |
| **Classification** | Model |
| **Owner** | Project Architecture |
| **Date** | 22 July 2026 |

---

# 1. Objective

Update the Vendor Domain Models to incorporate the final Epic 1 Vendor Registration decisions agreed following completion of the domain modelling review.

These changes refine the model without changing its aggregate boundaries or overall architecture.

---

# 2. Required Changes

## 2.1 Registration Session

Replace all remaining descriptions of Registration Session with the following model.

Registration Session is a transient application construct that exists only while a prospective Vendor is completing Vendor Registration.

Clarify that:

- Registration Session is **not** a Domain Entity.
- Registration Session is **not** part of the Vendor Aggregate.
- Registration Session is **not** part of the Vendor lifecycle.
- Registration Session exists before a Vendor exists.
- Registration Session may use transient persistence.
- Registration Session expires automatically after inactivity.
- Expired Registration Sessions are discarded.
- Registration Sessions cannot be resumed.
- Registration Sessions do not generate Vendor domain events.

Add the following architectural rationale:

> Vendor Registration is intentionally completed within a single Registration Session. Successful submission represents a deliberate expression of intent to become a Vendor. Incomplete registrations have no business significance and are discarded.

---

## 2.2 Vendor Creation Boundary

Clarify throughout the document that:

```
Prospective Vendor
        ↓
Registration Session
        ↓
Successful Registration
        ↓
Vendor Created
        ↓
PendingActivation
```

A Vendor never exists prior to successful registration.

---

## 2.3 Trading Location

Replace the current Trading Location values with the approved controlled vocabulary.

| Value | Description |
|-------|-------------|
| Restaurant | Restaurant, Café, Takeaway or Food Market Hall. A customer-facing permanent premises. |
| Stall | Mobile Food Unit or Market Stall. |
| Kitchen | Dark Kitchen, Ghost Kitchen or Home Kitchen. A non-customer-facing food preparation venue that trades exclusively online. |

Update all diagrams, value objects and enumerations accordingly.

---

## 2.4 Opening Hours

Clarify that Opening Hours are represented by:

- Start Time
- End Time

Add:

Opening Hours may legitimately span midnight.

Validation must not require Start Time to be earlier than End Time.

---

## 2.5 Service Includes Hot Food

Update the description.

Defines whether the Vendor supplies food or drink heated above ambient room temperature.

Clarify that this contributes to Compliance Requirement generation.

---

## 2.6 Alcohol Service

Update the description.

Defines whether the Vendor supplies alcohol.

Clarify that this contributes to Compliance Requirement generation.

---

## 2.7 Business Address

Replace references to storing only an AddressId.

Clarify the approved architecture.

The Address Domain owns:

- address search
- address retrieval
- address validation
- third-party address integration

The Vendor Domain stores:

- an approved snapshot of the Business Address

The Vendor Domain does not own address validation.

The Address Domain remains stubbed during Epic 1.

---

## 2.8 Regulatory Authorities

Introduce the following Registered Information.

### Food Registration Authority

Represents the competent authority responsible for Food Business Registration.

Characteristics:

- derived from the approved Business Address
- supplied by the Address Domain
- not manually editable by the Vendor

---

### Primary Trading Authority

Represents the authority responsible for the Vendor's declared primary trading area.

Characteristics:

- applicable only to Trading Location = Stall
- derived by the Address Domain
- forms part of Registered Information

---

## 2.9 Registered Information

Extend Registered Information to include:

- Food Registration Authority
- Primary Trading Authority (where applicable)

Retain the existing rule:

Registered Information becomes read-only once the Vendor enters PendingActivation.

Only authorised platform operators may amend Registered Information.

---

## 2.10 Vendor Managed Information

No functional change.

Retain:

- Website
- Business Description

as Vendor Managed Information.

---

## 2.11 Company Registration Number

Update the CompanyRegistrationNumber value object.

Validation shall:

- require a value only when Legal Operator Type requires one
- validate format using

```
^(?:[A-Za-z]{2})?\d{6,8}$
```

- normalise alphabetic prefixes to uppercase
- validate format only

Existence or Companies House verification remains outside Epic 1.

---

## 2.12 Compliance Requirement Generation

Clarify that Compliance Requirements are derived from:

- Trading Characteristics
- Legal Operator Type
- Approved Business Address
- Food Registration Authority
- Primary Trading Authority (where applicable)

The Vendor Domain requests Compliance Requirements through the existing abstraction.

The Vendor Domain does not contain regulatory decision logic.

---

## 2.13 Aggregate Model

Update the aggregate diagram and supporting model to reflect:

- Business Address Snapshot
- Food Registration Authority
- Primary Trading Authority

Remove any implication that the Vendor Aggregate owns only an AddressId.

---

## 2.14 Domain Diagrams

Update every affected diagram to reflect:

- revised Trading Location values
- Registration Session existing outside the Vendor Aggregate
- Business Address snapshot
- Food Registration Authority
- Primary Trading Authority

Do not redesign unaffected diagrams.

---

## 2.15 Initial Implementation Scope

Clarify Epic 1 implementation assumptions.

Epic 1 SHALL include:

- transient Registration Session
- single-session Vendor Registration
- no resumable registrations
- Business Address snapshot
- stubbed Address Domain
- stubbed Compliance Domain
- one Vendor represents one trading location

Epic 1 SHALL NOT include:

- registration drafts
- multiple premises
- Branch aggregate
- Registration Amendment workflow

---

## 2.16 Future Evolution

Retain explicit recognition that future phases will introduce:

- Branches
- Multiple premises
- Registration Amendment capability
- Full Address Domain
- Full Compliance Domain

No implementation is required during Epic 1.

---

# 3. Revision History

Add:

| Version | Date | Description |
|---------|------|-------------|
| 1.3 | 22 July 2026 | Finalised Vendor Registration domain model following Epic 1 review. Clarified Registration Session, Business Address ownership, Regulatory Authorities, Trading Location, Company Registration validation and Business Address snapshot strategy. |

---

# 4. Preservation Requirements

Codex shall:

- preserve all document structure
- preserve all heading numbering
- preserve markdown formatting
- preserve all unchanged content
- preserve aggregate boundaries
- preserve lifecycle semantics
- preserve process boundaries
- preserve all diagrams except where changes above require updates
- maintain consistency with HJ-003 Version 2.0 and HJ-104 Version 1.0

---

# 5. Expected Outcome

After applying this Change Request:

- HJ-001 (Project Vision)
- HJ-002 (Architectural Principles)
- HJ-003 (Ubiquitous Language Guide)
- HJ-004 (Vendor Domain Models)
- HJ-104 (Vendor Registration Fields Matrix)

shall contain a consistent and internally aligned representation of the Epic 1 Vendor Registration domain suitable for implementation.

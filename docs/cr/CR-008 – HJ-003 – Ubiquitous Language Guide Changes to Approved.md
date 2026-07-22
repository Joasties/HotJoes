# CR-008 – HJ-003 – Ubiquitous Language Guide Changes to Approved

| Property | Value |
|----------|-------|
| **Change Request ID** | CR-008 |
| **Affected Document** | HJ-003 – Ubiquitous Language Guide |
| **Current Version** | 1.2 |
| **Target Version** | 2.0 |
| **Status** | Approved |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Date** | 22 July 2026 |

---

# 1. Objective

Update the Ubiquitous Language Guide to incorporate the final agreed Vendor Registration terminology and business rules arising from completion of Epic 1 domain modelling.

This change promotes HJ-003 from Draft to the approved baseline for all Vendor Registration terminology.

---

# 2. Required Changes

## 2.1 Vendor Registration

Replace the definition of **Vendor Registration** with:

> The process through which a prospective Vendor supplies sufficient Registered Information to create a Vendor within the platform.

Clarify that:

- A Vendor does not exist before successful submission.
- Successful submission creates both the Vendor and its initial lifecycle state of **Pending Activation**.
- Vendor Registration does not authorise trading.
- Compliance and Activation remain separate business processes.

---

## 2.2 Registration Session

Replace the existing Registration Session definition with the following.

### Registration Session

A transient application concept used to collect registration information before a Vendor exists.

A Registration Session:

- exists only while registration is being completed
- is not a business entity
- is not part of the Vendor lifecycle
- may be implemented using any suitable transient persistence mechanism
- expires automatically after inactivity
- cannot be resumed
- is discarded completely if abandoned
- produces no business events

Add the following note:

> Completion of Vendor Registration represents a deliberate expression of intent to become a Vendor on the platform. Registration Sessions are intentionally transient and cannot be resumed.

---

## 2.3 Trading Location

Replace the current Trading Location values with:

| Value | Description |
|-------|-------------|
| Restaurant | Restaurant, Café, Takeaway or Food Market Hall. A customer-facing permanent premises. |
| Stall | Mobile Food Unit or Market Stall. |
| Kitchen | Dark Kitchen, Ghost Kitchen or Home Kitchen. A non-customer-facing food preparation venue that trades exclusively online. |

Clarify that Trading Location is a controlled business classification.

---

## 2.4 Opening Hours

Extend the Trading Characteristics definition.

Opening Hours are represented by:

- Start Time
- End Time

Add:

> Opening Hours may legitimately span midnight. Validation must not require Start Time to be earlier than End Time.

---

## 2.5 Service Includes Hot Food

Replace the definition with:

> Indicates whether the Vendor supplies food or drink heated above ambient room temperature.

Add:

> This information is used together with Opening Hours to determine applicable Compliance Requirements.

---

## 2.6 Alcohol Service

Replace the definition with:

> Indicates whether the Vendor supplies alcohol.

Add:

> This information is used to determine applicable Compliance Requirements.

---

## 2.7 Food Registration Authority

Introduce a new business term.

### Food Registration Authority

The competent local authority responsible for Food Business Registration for the Vendor's trading premises.

Clarify:

- derived from the approved Business Address
- not manually selected by the Vendor
- supplied by the Address Domain
- forms part of Registered Information

---

## 2.8 Primary Trading Authority

Introduce a new business term.

### Primary Trading Authority

The local authority responsible for the Vendor's declared primary trading area.

Clarify:

- required only for Trading Location = Stall
- derived through the Address Domain
- used to determine applicable Compliance Requirements

---

## 2.9 Address

Replace the existing Address definition with:

> The Address Domain owns address search, retrieval and validation.

>The Vendor Domain stores an approved snapshot of the Business Address as Registered Information.

---

## 2.10 Registered Information

Extend the Registered Information definition.

Registered Information now includes:

- Trading Name
- Legal Operator Name
- Legal Operator Type
- Company Registration Number
- Contact Name
- Contact Email
- Contact Telephone
- Business Address
- Food Registration Authority
- Primary Trading Authority (where applicable)
- Trading Characteristics

Retain the existing rule that Registered Information becomes read-only once the Vendor enters Pending Activation.

---

## 2.11 Vendor Managed Information

No change to definition.

Retain Website and Business Description as the initial examples.

---

## 2.12 Vendor Status

Remove **Draft** from the example lifecycle states.

Vendor lifecycle begins when the Vendor is created.

Example lifecycle states become:

- Pending Activation
- Active
- Suspended
- Deactivated

---

## 2.13 Events

Remove the following examples:

- Registration Session Started
- Registration Session Abandoned

Registration Session is not a business concept and therefore does not generate domain events.

Retain:

- Vendor Registered
- Vendor Activated
- Vendor Suspended
- Vendor Deactivated
- etc.

---

## 2.14 Commands

Remove:

- Start Registration Session

Replace:

- Complete Registration

with

- Register Vendor

Registration Session is an application concern rather than a business command.

---

## 2.15 Glossary

Add glossary entries for:

- Food Registration Authority
- Primary Trading Authority

Update the following glossary entries:

- Registration Session
- Vendor Registration
- Trading Location
- Address
- Registered Information

to reflect the changes introduced by this Change Request.

---

# 3. Revision History

Update the Revision History.

Add:

| Version | Date | Description |
|---------|------|-------------|
| 2.0 | 22 July 2026 | Approved Vendor Registration terminology. Clarified Registration Session, Trading Characteristics, Regulatory Authorities, Vendor lifecycle, Registered Information and Address ownership. |

---

# 4. Document Status

Update:

- Version → **2.0**
- Status → **Approved**
- Last Updated → **22 July 2026**

---

# 5. Preservation Requirements

Codex shall:

- preserve all existing document structure
- preserve all heading numbering
- preserve markdown formatting
- preserve all unchanged terminology
- update only the sections identified above
- ensure all terminology remains consistent with HJ-104
- ensure no new terminology is introduced unless explicitly defined within the Ubiquitous Language Guide

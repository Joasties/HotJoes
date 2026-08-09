# CR-020 – Legal Operator Terminology Clarification and Standardisation

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-020 |
| **Title** | Legal Operator Terminology Alignment |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Important |
| **Affected Documents** | HJ-003 – Ubiquitous Language Guide; HJ-104 – Vendor Registration Fields Matrix |

---

# 1. Background

As the Vendor Registration model has matured, the terminology describing the legal entity responsible for operating a Vendor has become more precise.

Earlier iterations of the documentation used **Company Name** as a generic business term. However, this is not correct for many legal operator types, including Sole Traders, Partnerships, Charitable Organisations and other non-company legal structures.

The approved domain model now distinguishes two separate concepts:

- **Legal Operator Type** — the classification of the legal entity responsible for operating the Vendor; and
- **Legal Operator Name** — the registered legal name of that legal entity or individual.

This terminology is now used consistently throughout the Vendor Domain Model, Registration Fields Matrix and Service Contract.

HJ-003 currently requires this terminology restructuring, while HJ-104 continues to expose the registration field named **Legal Operator Name**. To preserve consistency across the architectural artefacts, HJ-104 should explicitly reference the authoritative terminology defined in HJ-003.

This change standardises terminology without changing business behaviour, data capture or validation rules.

---

# 2. Objectives

The revised artefacts shall:

- establish **Legal Operator Type** as the authoritative business classification;
- establish **Legal Operator Name** as the authoritative business term for the registered legal name of the operating legal entity or individual;
- retire **Company Name** as an active business term, retaining it only as a historical synonym where appropriate;
- align HJ-104 terminology with the authoritative definitions contained in HJ-003; and
- ensure consistent terminology across all Vendor Registration artefacts.

---

# 3. Required Changes

## 3.1 HJ-003 – Ubiquitous Language Guide

### Section 3.10 – Legal Operator Type

Rename the existing **Legal Operator** section to **Legal Operator Type**.

Define the term as the authoritative classification describing the legal form of the organisation or individual operating the Vendor.

Examples include:

- Sole Trader;
- General Partnership;
- Limited Company (Ltd);
- Limited Liability Partnership (LLP);
- Charitable Incorporated Organisation (CIO); and
- other supported legal operator types.

---

### Section 3.11 – Legal Operator Name

Introduce a new section immediately following **Legal Operator Type**.

Define:

> **Legal Operator Name** is the authoritative business term for the registered legal name of the individual or legal entity responsible for operating the Vendor business.

Clarify that this terminology applies uniformly across all supported Legal Operator Types and is not restricted to incorporated companies.

---

### Section 3.12 – Company Name

Replace the previous **Company Name** definition with a historical terminology note.

State that:

> **Company Name** is retained only as a historical synonym for documentation compatibility.
>
> The authoritative business term is **Legal Operator Name**.

---

### Subsequent Sections

Renumber subsequent sections accordingly.

---

### Glossary

Update the glossary so that it contains separate entries for:

- Legal Operator Type;
- Legal Operator Name; and
- Company Name (historical synonym only).

The glossary shall identify **Legal Operator Name** as the authoritative business term.

---

## 3.2 HJ-104 – Vendor Registration Fields Matrix

### Legal Operator Name Field

Retain the registration field named **Legal Operator Name**.

No changes are required to:

- field behaviour;
- validation rules;
- mandatory rules;
- editability;
- registration workflow; or
- business semantics.

---

### Terminology Note

Add a terminology note adjacent to the **Legal Operator Name** field definition (or within the field notes section) stating:

> **Terminology**
>
> **Legal Operator Name** is the authoritative business term defined in **HJ-003 – Ubiquitous Language Guide**.
>
> It represents the registered legal name of the individual or legal entity responsible for operating the Vendor, regardless of Legal Operator Type.
>
> The historical term **Company Name** is retained only for compatibility with earlier documentation and is not the preferred architectural terminology.

This note provides a direct cross-reference to the authoritative terminology while avoiding duplication of the definition.

---

# 4. Editorial Principles

- No business capability shall change.
- No registration workflow shall change.
- No validation rules shall change.
- No field names presented to users are required to change unless separately requested.
- HJ-003 shall remain the authoritative source of Vendor terminology.
- HJ-104 shall reference the authoritative terminology defined in HJ-003 rather than redefining it.
- The term **Company Name** shall no longer be used as the preferred architectural term within the Vendor Registration documentation.

---

# 5. Expected Outcome

Following this change:

- **Legal Operator Type** and **Legal Operator Name** become the authoritative business terminology for the Vendor Domain.
- **Company Name** is retained only as a historical synonym for compatibility with earlier documentation.
- HJ-003 provides the single authoritative definition of the terminology.
- HJ-104 explicitly cross-references the authoritative terminology through a dedicated terminology note while preserving the existing registration field behaviour.
- All Vendor Registration artefacts use consistent legal-entity terminology without changing any business capability or implementation behaviour.

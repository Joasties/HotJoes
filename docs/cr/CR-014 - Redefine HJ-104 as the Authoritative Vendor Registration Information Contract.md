# CR-014 – Redefine HJ-104 as the Authoritative Vendor Registration Information Contract

## Document Information

| Field | Value |
|-------|-------|
| Change Request | CR-014 |
| Title | Redefine HJ-104 as the Authoritative Vendor Registration Information Contract |
| Status | Proposed |
| Affected Document | HJ-104 – Vendor Registration Fields Matrix |
| Priority | High |

---

# 1. Background

HJ-104 has evolved incrementally alongside the Vendor Domain Model, Vendor Registration Sequence Diagram and Registration Service Contract. During this evolution, many important business rules governing Vendor Registration information have been introduced into other artefacts, leaving HJ-104 functioning primarily as a field catalogue rather than the authoritative source of business information rules.

As a result, downstream documents such as HJ-106 are required to duplicate, infer or restate business rules that should instead be owned by HJ-104.

This change redefines the architectural responsibility of HJ-104 so that it becomes the single authoritative source for the business semantics governing Vendor Registration information.

---

# 2. Objectives

The revised HJ-104 shall:

- Clearly define its responsibility as the authoritative Vendor Registration Information Contract.
- Continue to provide a concise executive summary of all registration information.
- Record business rules exactly once.
- Avoid duplication of business rules between overview tables and detailed rule sections.
- Provide traceability to related architectural artefacts where appropriate.

---

# 3. Required Changes

## 3.1 Update Purpose

Replace the existing Purpose section with one that establishes HJ-104 as the authoritative source for Vendor Registration information and its associated business rules.

The Purpose shall explicitly state that the document governs:

- Information classification.
- Required, optional and conditional information.
- Validation rules.
- Conditional business rules.
- Derived information.
- Information ownership.
- Editability.
- Information lifecycle.
- Traceability to the Vendor Domain Model.

The Purpose shall also explicitly state that workflow, aggregate behaviour, API contracts and implementation details are documented elsewhere.

---

## 3.2 Preserve Registration Field Matrix

Retain the existing Registration Field Matrix as the executive summary of Vendor Registration information.

The existing structure and content shall remain unchanged except for the **Notes** column.

---

## 3.3 Enhance Notes Column

Append to the **Notes** column a concise references to any additional business rules recorded elsewhere in the document.

The Notes column shall:

- preserve the existing note;
- append one or more references to the relevant section numbers where additional rules are defined.

Example:

> Validation depends on Legal Operator Type. See §5.1, §5.2, §5.3.

---

## 3.4 Preserve Existing Sections of the document

Retain the existing sections without structural changes:

- Registration Field Matrix
- Identity Fields
- Deferred Fields

except where cross-references are required.

---

## 3.5 Introduce Business Rules

Insert a new **Business Rules** section immediately before the existing Assumptions section.

Business Rule subsections shall only be created where actual business rules exist.

Initially introduce the following subsections.

### 5.1 Conditional Business Rules

Capture conditional business rules governing Vendor Registration information.

Examples include:

- Legal Operator Type dependencies.
- Company Registration Number applicability.
- Trading Location dependencies.
- Alcohol Service dependencies.
- Service Includes Hot Food dependencies.

---

### 5.2 Validation Rules

Capture business validation rules.

Where appropriate this section shall include:

- Validation policies.
- Format requirements.
- Regular expressions.
- Maximum lengths.
- Permitted value constraints.

Validation rules represent business constraints and are not implementation details.

Implementation-specific validation remains within HJ-005.

---

### 5.3 Canonicalisation Rules

Capture business rules governing canonicalisation before persistence.

Examples include:

- Company Registration Number converted to uppercase.
- Future approved canonicalisation rules.

---

### 5.4 Derived Information

Capture information derived by other domains or services.

Examples include:

- Food Registration Authority.
- Primary Trading Authority.
- Business Address Snapshot.

---

### 5.5 Information Lifecycle

Capture lifecycle rules where applicable.

Examples include:

- Registered Information becomes read-only following successful registration.
- Vendor Managed Information may be updated through future Vendor Management services.
- Registration Declarations exist only during registration.
- Business Address Snapshot is immutable after registration.

---

### 5.6 Traceability to Domain Model

Introduce a traceability table identifying the architectural source of significant business rules.

Typical references include:

- HJ-004 Vendor Domain Models.
- HJ-105 Vendor Registration Sequence Diagram.
- Relevant Architectural Decision Records.

---

## 3.6 Rename Assumptions

Rename the existing **Assumptions** section to:

> **Assumptions and Outstanding Decisions**

Review every existing assumption.

- Remove assumptions that have become approved business rules.
- Retain only genuine unresolved decisions.

---

# 4. Editorial Principles

The revised document shall adhere to the following principles.

1. HJ-104 is the authoritative source of Vendor Registration information business rules.

2. Business rules shall be defined exactly once.

3. The Registration Field Matrix remains an executive summary and shall not become a detailed business rules catalogue.

4. The **Notes** column shall provide concise references to relevant Business Rules sections and shall not duplicate those rules.

5. Business Rule subsections shall only be introduced where genuine business rules exist.

6. Workflow, aggregate behaviour, API contracts and implementation details remain the responsibility of their respective documents.

---

# 5. Expected Outcome

Following implementation:

- HJ-104 becomes the authoritative Vendor Registration Information Contract.
- HJ-105 continues to define Vendor Registration workflow.
- HJ-106 references HJ-104 for business information rules rather than duplicating or inferring them.
- Business rules are maintained in a single authoritative location.
- The documentation set exhibits improved separation of concerns, reduced duplication and clearer ownership of business semantics.

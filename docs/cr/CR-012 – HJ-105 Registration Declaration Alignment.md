# CR-012 – HJ-105 Registration Declaration Alignment

| Property | Value |
|----------|-------|
| **Change Request ID** | CR-012 |
| **Title** | HJ-105 Registration Declaration Alignment |
| **Status** | Proposed |
| **Priority** | Medium |
| **Author** | Project Architecture |
| **Date** | 24 July 2026 |
| **Target Document** | HJ-105 – Vendor Registration Sequence Diagram v0.1 |

---

# 1. Summary

Amend **HJ-105 – Vendor Registration Sequence Diagram** to explicitly incorporate the mandatory **Registration Declarations** introduced into **HJ-104 – Vendor Registration Fields Matrix v2.0**.

This change aligns the behavioural model with the approved Vendor Registration business contract by making the Registration Declarations explicit within the registration workflow and server-side validation.

No new business behaviour is introduced.

---

# 2. Background

HJ-104 Version 2.0 introduced the **Registration Declaration** classification and defined three mandatory declarations that form part of every Vendor Registration request:

- Authorised to Register Business
- Information Accurate
- Accept HotJoes Platform Terms

HJ-105 currently refers to validation of "registration data" and "mandatory registration fields" but does not explicitly include these declarations within the RegisterVendor request, validation sequence or validation checklist.

Although the behaviour can reasonably be inferred, the sequence diagram should explicitly model the complete Vendor Registration business contract to maintain consistency across the approved architectural artefacts.

---

# 3. Change Required

## 3.1 RegisterVendor Request

Update the Vendor Registration sequence to make it explicit that the RegisterVendor request contains:

- Registered Information;
- Vendor Managed Information;
- Registration Declarations.

Where appropriate, replace generic references such as:

```text
RegisterVendor(registration data)
```

with wording that better represents the complete business contract, for example:

```text
RegisterVendor(registration request)
```

or equivalent terminology that clearly encompasses all mandatory registration information and declarations.

---

## 3.2 Server-Side Validation

Extend the server-side validation sequence to explicitly validate the Registration Declarations before Vendor creation.

Validation shall confirm that:

- the applicant confirms they are authorised to register the business;
- the applicant confirms the submitted information is accurate;
- the applicant accepts the applicable HotJoes platform terms.

Vendor creation shall not proceed unless all mandatory Registration Declarations have been successfully validated.

---

## 3.3 Validation Checklist

Update the **Registration Validation Failure** section to include the Registration Declarations within the authoritative server-side validation checklist.

The checklist shall explicitly validate:

- Authorised to Register Business declaration;
- Information Accurate declaration;
- Accept HotJoes Platform Terms declaration.

These validations shall be treated as mandatory business validation rules.

---

## 3.4 Sequence Diagram Notes

Where appropriate, amend explanatory notes within the happy-path sequence diagram to clarify that successful Vendor Registration requires both:

- mandatory registration information; and
- mandatory Registration Declarations.

The intent is to make the complete Vendor Registration contract explicit to readers of the behavioural model.

---

## 3.5 Cross-Document Consistency

Ensure HJ-105 aligns with:

- HJ-003 – Ubiquitous Language Guide;
- HJ-004 – Vendor Domain Models;
- HJ-104 – Vendor Registration Fields Matrix.

Registration Declarations shall be represented consistently across all four artefacts.

---

# 4. Out of Scope

This change request shall **not** modify:

- Vendor lifecycle behaviour;
- Registration Session behaviour;
- Address Domain interactions;
- Pending Activation Process;
- Compliance Requirement generation;
- Commands;
- Events;
- Transaction boundaries;
- Reliability patterns;
- Aggregate design.

This change introduces no additional business capability.

---

# 5. Architectural Impact

None.

This change:

- aligns the behavioural model with the approved Vendor Registration requirements;
- improves traceability between HJ-104 and HJ-105;
- makes the Vendor Registration business contract explicit;
- improves implementation guidance for API design and server-side validation.

No architectural decisions are changed.

---

# 6. Rationale

HJ-104 is the authoritative definition of the Vendor Registration business contract.

HJ-105 is the authoritative behavioural model describing how that contract is executed.

To maintain architectural consistency, all mandatory inputs defined by HJ-104 should be explicitly represented within the HJ-105 registration flow.

Making the Registration Declarations visible within the sequence diagram improves clarity for developers, testers, API designers and independent reviewers without altering the underlying business behaviour.

---

# 7. Acceptance Criteria

The change shall be considered complete when:

1. The RegisterVendor request explicitly includes the Registration Declarations as part of the Vendor Registration contract.
2. Server-side validation explicitly validates all three Registration Declarations.
3. The Registration Validation checklist includes all three Registration Declarations.
4. Sequence diagram notes clearly indicate that successful Vendor Registration requires both mandatory registration information and mandatory Registration Declarations.
5. HJ-105 is fully aligned with HJ-104 regarding the Vendor Registration contract.
6. No business behaviour or architectural decisions have changed.

---

# 8. Expected Outcome

Following implementation of this change request, HJ-105 will explicitly model the complete Vendor Registration business contract defined by HJ-104.

The resulting behavioural model will provide complete traceability from registration requirements through to application behaviour, improving consistency across the HotJoes architectural documentation while preserving all existing business rules and architectural decisions.

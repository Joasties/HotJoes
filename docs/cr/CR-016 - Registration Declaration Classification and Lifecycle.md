# CR-016 – Registration Declaration Classification and Lifecycle

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-016 |
| **Title** | Registration Declaration Classification and Lifecycle |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Important |
| **Affected Documents** | HJ-104 – Vendor Registration Fields Matrix; HJ-004 – Vendor Domain Models; HJ-105 – Vendor Registration Sequence Diagram |

---

# 1. Background

HJ-104 already establishes that Registration Declarations are distinct from both **Registered Information** and **Vendor Managed Information**, but several important architectural rules remain implicit or incomplete.

Specifically, Registration Declarations should be recognised as transient business inputs used solely to determine whether Vendor registration may proceed. They are not part of the Vendor's enduring business state and therefore should not participate in persistence, domain events, integration events or the Vendor lifecycle.

Subsequent review also identified that HJ-104 repeats declaration lifecycle information within **Section 5.5 – Information Lifecycle**. Updating only the Registration Declaration Classification subsection would leave Section 5.5 inconsistent with the revised architectural model. Likewise, the Registration Field Matrix currently directs readers only to Section 5.5, rather than to the authoritative classification.

Finally, the revised structure makes the existing **Purpose** statement technically inaccurate because Registration Declaration classification is now defined authoritatively in **Section 2** rather than exclusively within the Business Rules section.

This change establishes a single authoritative definition of Registration Declarations while ensuring that all lifecycle guidance, document structure and matrix references remain internally consistent.

---

# 2. Objectives

The revised artefacts shall:

- establish Registration Declarations as transient inputs to the **Register Vendor** command;
- distinguish Registration Declarations from Registered Information and Vendor Managed Information;
- confirm that Registration Declarations never become part of Vendor business state;
- prohibit persistence of Registration Declarations on the Vendor aggregate;
- prohibit Registration Declarations from appearing in domain events or integration events;
- establish that Registration Declarations have no lifecycle after completion of the registration transaction;
- clarify that any audit retention occurs only outside the Vendor Domain; and
- ensure HJ-104 contains a single authoritative classification with consistent lifecycle guidance, document structure and field-matrix references.

---

# 3. Required Changes

## 3.1 HJ-104 – Vendor Registration Fields Matrix

### Section 1 – Purpose

Revise the **Purpose** statement to reflect the revised document structure.

Replace the existing statement:

> Detailed business rules are recorded exactly once in the Business Rules section and are referenced from the matrix.

with wording equivalent to:

> Business rules and classifications are defined once in their designated authoritative sections and are cross-referenced from the matrix.

This wording shall become the general structural principle for HJ-104 and shall accommodate both the Business Rules section and the authoritative Registration Declaration Classification without introducing special-case wording.

---

### Section 2 – Registration Declaration Classification

Revise the existing **Registration Declaration Classification** subsection to establish it as the authoritative definition of Registration Declarations.

The subsection shall state that Registration Declarations:

- exist solely as transient business inputs to the **Register Vendor** command;
- are neither Registered Information nor Vendor Managed Information;
- are not persisted on the Vendor aggregate;
- do not become part of Vendor business state;
- never appear in Vendor domain events;
- never appear in Vendor integration events;
- have no lifecycle after completion of the registration transaction; and
- may be retained for audit purposes only outside the Vendor Domain.

This subsection shall become the authoritative architectural definition of Registration Declarations within HJ-104.

---

### Section 5.5 – Information Lifecycle

Revise **Section 5.5 – Information Lifecycle** so that it provides a concise lifecycle summary consistent with the authoritative classification defined in Section 2.

Replace or revise the Registration Declarations lifecycle description with wording equivalent to:

> Registration Declarations are transient inputs to **Register Vendor** and have no lifecycle after the registration transaction completes. They are not persisted on the Vendor aggregate or exposed through domain or integration events. Any audit retention occurs outside the Vendor Domain. The authoritative classification is defined in **Section 2 – Registration Declaration Classification**.

Section 5.5 shall summarise the lifecycle behaviour and cross-reference the authoritative classification. It shall not become a second authoritative definition of Registration Declarations.

---

### Registration Field Matrix

Update the **Notes** for Registration Declaration rows (currently **Rows 17–19**) so that they reference both:

- **Section 2 – Registration Declaration Classification**; and
- **Section 5.5 – Information Lifecycle**.

The Notes shall identify:

- **Section 2** as the authoritative classification; and
- **Section 5.5** as the corresponding lifecycle guidance.

---

## 3.2 HJ-004 – Vendor Domain Models

Strengthen the Registration Declaration description to state that Registration Declarations:

- are transient command inputs;
- are not persisted within the Vendor aggregate;
- are not Registered Information;
- are not Vendor Managed Information;
- are not represented within aggregate state;
- are excluded from Vendor domain events; and
- are excluded from Vendor integration events.

The Vendor aggregate shall contain no representation of Registration Declarations after successful completion of Vendor registration.

---

## 3.3 HJ-105 – Vendor Registration Sequence Diagram

Strengthen the registration workflow by stating that Registration Declarations:

- are evaluated during execution of the **RegisterVendor** command;
- influence the registration decision only;
- are not persisted following successful Vendor creation;
- do not appear in the **VendorRegistered domain event**;
- do not appear in the published **VendorRegistered integration event**; and
- are discarded upon completion of the registration transaction.

Any audit retention shall occur outside the Vendor Domain.

---

# 4. Editorial Principles

- No new business capability shall be introduced.
- No Vendor lifecycle shall change.
- No aggregate boundaries shall change.
- Registration Declarations shall remain transient command inputs.
- Registration Declarations shall never become Vendor business state.
- Registration Declarations shall never be exposed through domain events or integration events.
- Audit retention shall remain outside the Vendor Domain.
- Within HJ-104, business rules and information classifications shall each have a single designated authoritative location. Other sections shall summarise or cross-reference those definitions rather than duplicate them.
- All terminology shall remain consistent with HJ-003, HJ-004 and HJ-105.

---

# 5. Expected Outcome

Following this change:

- Registration Declarations are consistently recognised as transient inputs to the **RegisterVendor** command.
- Registration Declarations are never persisted within the Vendor aggregate.
- Registration Declarations never become Vendor business state.
- Registration Declarations never appear in Vendor domain events or published integration events.
- Any audit retention occurs outside the Vendor Domain.
- HJ-104 contains a single authoritative definition of Registration Declarations in **Section 2**, with **Section 5.5** providing a concise lifecycle summary through cross-reference rather than duplication.
- The document **Purpose** accurately reflects the revised information architecture by recognising that both business rules and information classifications have designated authoritative sections.
- The Registration Field Matrix consistently directs readers to the authoritative Registration Declaration classification while retaining the appropriate lifecycle guidance.
- HJ-004, HJ-105 and HJ-104 remain fully aligned regarding the classification, lifecycle and architectural treatment of Registration Declarations.

# CR-011 – Activated Terminology Alignment

| Property | Value |
|----------|-------|
| **Change Request ID** | CR-011 |
| **Title** | Activated Terminology Alignment |
| **Status** | Proposed |
| **Priority** | Medium |
| **Author** | Project Architecture |
| **Date** | 24 July 2026 |
| **Target Documents** | HJ-003 – Ubiquitous Language Guide<br>HJ-004 – Vendor Domain Models<br>HJ-105 – Vendor Registration Sequence Diagram |

---

# 1. Summary

Amend the Vendor Registration architectural documentation to ensure consistent use of the approved lifecycle term **Activated**.

This change resolves residual terminology drift remaining after the approval of **HJ-003 v2.0**, where **Activated** became the authoritative lifecycle state.

The change is editorial only and introduces no new business behaviour or architectural decisions.

---

# 2. Background

HJ-003 establishes the official HotJoes Ubiquitous Language.

The approved Vendor lifecycle states are:

- Pending Activation
- Activated
- Suspended
- Deactivated

During iterative development, most references across HJ-004 and HJ-105 were updated to use **Activated**.

However, a small number of diagrams, notes, tables and descriptive text still refer to the previous lifecycle state **Active**.

These references create unnecessary inconsistency across the approved architectural artefacts.

---

# 3. Change Required

## 3.1 HJ-004 – Vendor Domain Models

Review the complete document and replace any remaining lifecycle references to **Active** with **Activated**, including:

- descriptive text;
- lifecycle explanations;
- tables;
- diagrams;
- notes;
- examples.

Do not modify:

- `VendorActivated`
- `ActivateVendor`
- `ActivatedAt`
- any event names
- any command names
- historical revision history.

Only lifecycle state terminology is affected.

---

## 3.2 HJ-105 – Vendor Registration Sequence Diagram

Review the complete document and replace any remaining lifecycle references to **Active** with **Activated**, including:

- sequence diagram notes;
- explanatory text;
- state descriptions;
- implementation guidance;
- examples.

Do not modify:

- `VendorActivated`
- `ActivateVendor`
- event names
- command names
- revision history.

---

## 3.3 HJ-003 – Ubiquitous Language Guide

Verify that no remaining lifecycle references use **Active** where the approved term is **Activated**.

No changes should be made unless an inconsistency is found.

---

# 4. Out of Scope

This change request shall **not** modify:

- business behaviour;
- Vendor lifecycle rules;
- state transitions;
- commands;
- events;
- API contracts;
- aggregate design;
- bounded context ownership;
- implementation guidance.

This is a terminology alignment exercise only.

---

# 5. Architectural Impact

None.

This change:

- improves consistency across architectural documentation;
- aligns all artefacts with the approved Ubiquitous Language;
- removes ambiguity during implementation and review;
- preserves all existing architectural decisions.

No functional behaviour changes.

---

# 6. Rationale

The HotJoes Architectural Principles require that business terminology is explicit, consistent and originates from the approved Ubiquitous Language.

Once **Activated** became the approved lifecycle state, all documentation should consistently use that terminology.

Maintaining a single authoritative lifecycle vocabulary reduces ambiguity for:

- architects;
- developers;
- testers;
- API designers;
- AI reviewers;
- future documentation.

---

# 7. Acceptance Criteria

The change shall be considered complete when:

1. All remaining lifecycle references to **Active** have been replaced with **Activated** where appropriate.
2. All diagrams, notes and explanatory text use consistent lifecycle terminology.
3. Event names remain unchanged (for example `VendorActivated`).
4. Command names remain unchanged (for example `ActivateVendor`).
5. Property names remain unchanged (for example `ActivatedAt`).
6. No business behaviour or architectural decisions have changed.
7. HJ-003, HJ-004 and HJ-105 use a single consistent lifecycle vocabulary.

---

# 8. Expected Outcome

Following implementation of this change request, all Vendor Registration architectural artefacts will consistently use the approved lifecycle terminology defined by the HotJoes Ubiquitous Language Guide.

The change is editorial in nature and is expected to require only terminology updates with no impact to design, implementation or business behaviour.

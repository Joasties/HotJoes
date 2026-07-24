# ADR-005 – Registered Information vs Vendor Managed Information

| **Document ID** | ADR-005 |
|-----------------|---------|
| **Document Title** | Registered Information vs Vendor Managed Information |
| **Version** | 1.1 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 23 July 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 23 July 2026 | Initial Architectural Decision Record. |
| 1.1 | 23 July 2026 | Clarified the distinction between architectural responsibilities and field-level business rules. Added reference to HJ-104 as the authoritative source for attribute behaviour. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted |
| ADR-004 | Vendor Lifecycle Begins After Successful Registration | Accepted |
| HJ-003 | Ubiquitous Language Guide | Draft |
| HJ-004 | Vendor Domain Models | Draft |
| HJ-104 | Vendor Registration Fields Matrix | Draft |

---

# 1. Context

Vendor Registration establishes the initial information required to create a new Vendor and begin its lifecycle.

Once a Vendor has been successfully registered, the relationship between the Vendor and the platform continues to evolve. Business information may be updated over time as trading arrangements, contact information, operational preferences and other business characteristics change.

The lifecycle behaviour of individual Vendor attributes varies according to business policy. Some attributes are immutable after registration, some become Vendor-managed, and others may only be updated by authorised platform administration.

These distinctions are business decisions rather than architectural ones.

The architecture therefore requires a clear distinction between the information required to establish a Vendor and the information subsequently managed throughout the Vendor lifecycle, while leaving the behaviour of individual attributes to the business rules that govern them.

---

# 2. Decision

The Vendor domain shall distinguish between **Registered Information** and **Vendor Managed Information**.

**Registered Information** represents the information required to establish a new Vendor and successfully complete the onboarding process.

Successful registration creates the initial state of the Vendor aggregate.

Following registration, responsibility for maintaining the Vendor transfers to the normal Vendor management lifecycle.

**Vendor Managed Information** represents business information that may be maintained, amended or extended throughout the lifetime of the Vendor in accordance with applicable business rules.

This distinction represents a change in **business responsibility**, not a difference in where the information is stored.

Information captured during registration becomes part of the Vendor aggregate after successful registration and may subsequently be modified where permitted by business policy.

The business rules governing the ownership, editability, validation and lifecycle behaviour of individual attributes are defined within the **Vendor Registration Fields Matrix (HJ-104)** and related domain artefacts.

Whether an individual attribute is editable is therefore determined by business rules rather than by the stage at which the information was originally captured.

---

# 3. Consequences

### Positive

- Registration remains a clearly defined business milestone.
- Vendor lifecycle management remains independent of the registration process.
- Business rules determine whether information may be changed rather than implementation history.
- Field-level behaviour can evolve without requiring architectural changes.
- The Vendor Registration Fields Matrix becomes the single authoritative source for attribute behaviour.
- The domain model clearly separates onboarding responsibilities from ongoing Vendor administration.

### Negative

- Developers must distinguish between registration behaviour and ongoing Vendor management behaviour.
- Business rules governing individual attributes must be maintained consistently within HJ-104.
- Registration validation and ongoing maintenance validation may legitimately differ for the same information.

---

# 4. Alternatives Considered

## Registration as a Permanent State

Rejected because registration is a completed business event rather than an ongoing operational state.

## Registration as Merely the First Vendor Edit

Rejected because it fails to recognise successful onboarding as a significant business milestone and unnecessarily couples registration workflow to Vendor lifecycle management.

## Immutable Registration Data

Rejected because many business attributes legitimately change throughout the lifetime of a Vendor and should be governed by business policy rather than by the stage at which they were originally captured.

---

# 5. Related Decisions

This decision builds upon:

- ADR-002 — Business Capabilities and Bounded Contexts
- ADR-004 — Vendor Lifecycle Begins After Successful Registration

This decision is supported by:

- ADR-006 — Address Domain Ownership and Business Address Snapshots

---

# 6. References

- HJ-002 — Architectural Principles
- HJ-003 — Ubiquitous Language Guide
- HJ-004 — Vendor Domain Models
- HJ-104 — Vendor Registration Fields Matrix

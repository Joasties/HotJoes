# ADR-000 – Architectural Decision Register

| **Document ID** | ADR-000 |
|-----------------|--------|
| **Document Title** | Architectural Decision Register |
| **Version** | 1.0 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 23 July 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 23 July 2026 | Initial Architectural Decision Register created. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| HJ-001 | Project Vision | Draft |
| HJ-002 | Architectural Principles | Draft |
| HJ-003 | Ubiquitous Language Guide | Draft |
| HJ-004 | Vendor Domain Models | Draft |
| HJ-008 | AI Roles and Responsibilities | Draft |
| HJ-009 | AI Operating Guide | Draft |

---

# 1. Purpose

This Architectural Decision Register (ADR Register) provides a single authoritative index of the significant architectural decisions made throughout the HotJoes project.

Each Architectural Decision Record (ADR) documents a decision that has a long-term impact on the architecture of the system. The register enables architects and developers to understand why important decisions were made, what alternatives were considered, and which documents are affected by those decisions.

This document acts solely as an index. Individual decisions are documented in their own ADR documents.

---

# 2. Scope

This register records only significant architectural decisions that:

- Influence the long-term structure of the solution.
- Affect multiple bounded contexts, services, or architectural layers.
- Introduce constraints that future development must respect.
- Capture important trade-offs where reasonable alternatives existed.

Implementation details, temporary project constraints, coding standards, and operational procedures are intentionally excluded unless they have lasting architectural significance.

---

# 3. Architectural Decision Lifecycle

Architectural decisions evolve over time.

Each ADR progresses through the following lifecycle:

| Status | Meaning |
|---------|---------|
| Proposed | Decision is under discussion and has not yet been accepted. |
| Accepted | Decision has been approved and forms part of the architecture baseline. |
| Superseded | Decision has been replaced by a newer ADR. |
| Deprecated | Decision is no longer recommended but remains documented for historical context. |

ADRs are never deleted. When a decision changes, a new ADR supersedes the previous one, preserving the project's architectural history.

---

# 4. Architectural Decision Records

| ADR | Title | Status |
|-----|-------|--------|
| ADR-001 | Domain-Driven Design as the Primary Architectural Style | Proposed |
| ADR-002 | Business Capabilities and Bounded Contexts | Proposed |
| ADR-003 | Event-Driven Collaboration | Proposed |
| ADR-004 | Vendor Lifecycle Begins After Successful Registration | Proposed |
| ADR-005 | Registered Information vs Vendor Managed Information | Proposed |
| ADR-006 | Address Domain Ownership and Business Address Snapshots | Proposed |
| ADR-007 | Vendor Compliance as a Separate Bounded Context | Proposed |
| ADR-008 | Reliable Messaging and Idempotent Processing | Proposed |
| ADR-009 | AI-Assisted Development with Human Accountability | Proposed |

---

# 5. Numbering Convention

Architectural Decision Records use the following numbering convention:

- ADR-000 — Architectural Decision Register
- ADR-001 onwards — Individual Architectural Decision Records

ADR numbers are allocated sequentially and are never reused.

---

# 6. Ownership

The Project Architecture function owns the Architectural Decision Register.

Changes to this register are managed through the standard HotJoes Change Request process.

New Architectural Decision Records should only be added when a decision has lasting architectural significance and cannot be adequately documented within existing architectural artefacts.

---

# 7. References

- HJ-001 — Project Vision
- HJ-002 — Architectural Principles
- HJ-003 — Ubiquitous Language Guide
- HJ-004 — Vendor Domain Models

# ADR-004 – Vendor Lifecycle Begins After Successful Registration

| **Document ID** | ADR-004 |
|-----------------|---------|
| **Document Title** | Vendor Lifecycle Begins After Successful Registration |
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
| 1.1 | 23 July 2026 | Reframed the decision around the business significance of Vendor Registration and the intentional use of transient Registration Sessions. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted |
| HJ-003 | Ubiquitous Language Guide | Draft |
| HJ-004 | Vendor Domain Models | Draft |

---

# 1. Context

Vendor Registration is the business process through which a prospective Vendor expresses a deliberate intention to join the HotJoes platform.

The purpose of registration is to collect and validate the information required to establish a new Vendor. Until that process has been completed successfully, no business relationship exists between the prospective Vendor and the platform.

Registration Sessions are intentionally transient and cannot be resumed.

An incomplete or expired registration has no business significance and is discarded. The business does not recognise partially completed registrations as entities requiring management, tracking or lifecycle states.

The architecture therefore requires a clear separation between the temporary registration process and the lifecycle of a registered Vendor.

---

# 2. Decision

Vendor registration shall be performed using a **transient Registration Session**.

The Registration Session exists solely to support the collection and validation of registration information during a single onboarding interaction and is not part of the persisted Vendor domain model.

Successful completion of registration represents a clear business commitment to join the platform and results in the creation of a new Vendor aggregate.

The Vendor lifecycle begins only when a Vendor is created in the **Pending Activation** state.

Incomplete, abandoned or expired Registration Sessions shall not result in the creation of any persistent business entity and shall not be recoverable.

This decision intentionally avoids introducing a draft registration lifecycle while ensuring that successful registration represents a clear and unambiguous business event.

---

# 3. Consequences

### Positive

- The Vendor aggregate always represents a valid business entity.
- The Vendor lifecycle contains only meaningful business states.
- Registration workflow remains separate from Vendor lifecycle management.
- Abandoned registrations require no business or operational management.
- Successful registration becomes a clear business milestone that can be communicated to other bounded contexts.
- Future changes to the registration experience can be made without affecting the Vendor domain model.

### Negative

- Users cannot pause and resume registration at a later time.
- Registration analytics must be captured independently of the Vendor aggregate if required.
- Future support for resumable registration would require a new architectural decision and associated domain model.

---

# 4. Alternatives Considered

## Persist Draft Vendors

Rejected because partially completed registrations do not represent recognised business entities. Introducing draft Vendors would unnecessarily couple the Vendor lifecycle to the registration process and introduce lifecycle states that have no business meaning.

## Persist Registration Sessions

Rejected because resumable registration is not a business requirement for HotJoes. Persisting Registration Sessions would introduce additional lifecycle management, security and operational complexity without providing corresponding business value.

This decision may be revisited if future business requirements demonstrate a clear need for resumable onboarding.

---

# 5. Related Decisions

This decision builds upon:

- ADR-002 — Business Capabilities and Bounded Contexts

This decision supports:

- ADR-005 — Registered Information vs Vendor Managed Information
- ADR-007 — Vendor Compliance as a Separate Bounded Context

---

# 6. References

- HJ-001 — Project Vision
- HJ-003 — Ubiquitous Language Guide
- HJ-004 — Vendor Domain Models

# ADR-007 – Vendor Compliance as a Separate Bounded Context

| **Document ID** | ADR-007 |
|-----------------|---------|
| **Document Title** | Vendor Compliance as a Separate Bounded Context |
| **Version** | 1.1 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 28 August 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 23 July 2026 | Initial Architectural Decision Record. |
| 1.1 | 28 August 2026 | Defined the Epic 1 Compliance consumer stub as a thin RabbitMQ adapter with durable EventId receipt and byte-hash deduplication under CON-022. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-003 | Event-Driven Collaboration | Accepted |
| ADR-004 | Vendor Registration Establishes Business Existence | Accepted |
| HJ-001 | Project Vision | Approved |
| HJ-002 | Architectural Principles | Approved |
| HJ-003 | Ubiquitous Language Guide | Approved |
| HJ-004 | Vendor Domain Models | Approved |

# 1. Context

Successful Vendor Registration establishes the existence of a Vendor within the platform.

However, the ability for a Vendor to trade is governed by a variety of regulatory and operational requirements, including Food Business Registration, Street Trading Licences and other statutory obligations.

These requirements vary according to Trading Characteristics, Legal Operator Type and jurisdiction, and are expected to evolve independently of the Vendor lifecycle.

Attempting to embed compliance directly within the Vendor aggregate would tightly couple business onboarding with regulatory policy, increasing complexity and reducing the ability of each concern to evolve independently.

The architecture therefore separates Vendor creation from Vendor compliance while allowing compliance outcomes to influence the Vendor lifecycle.

---

# 2. Decision

Vendor Compliance shall be implemented as a separate bounded context.

Successful Vendor Registration creates a Vendor in the **Pending Activation** state.

The Vendor domain shall not determine regulatory compliance.

Instead, the Compliance domain shall:

- determine applicable compliance requirements;
- evaluate submitted evidence;
- monitor continuing compliance;
- publish compliance outcomes.

The Vendor domain consumes those outcomes to determine whether a Vendor may transition from **Pending Activation** to **Activated**.

Vendor activation therefore depends upon successful compliance rather than successful registration.

The Vendor aggregate remains responsible only for its own business lifecycle and does not implement regulatory decision making.

## 2.1 Epic 1 Compliance Consumer Stub

The Epic 1 stub consumes only VendorRegistered v1. It durably records EventId, EventType, EventVersion, receipt time and a hash of the serialized bytes before acknowledgement. An equivalent duplicate is acknowledged without another receipt; the same EventId with different bytes is dead-lettered as a contract-integrity failure. The stub performs no Vendor lookup and introduces no Compliance Domain behaviour or Pending Activation processing.

---

# 3. Consequences

### Positive

- Vendor onboarding remains simple and business focused.
- Regulatory policy evolves independently.
- New compliance rules can be introduced without redesigning the Vendor aggregate.
- Vendor state model remains stable with a single Pending Activation state.
- Compliance becomes reusable by other domains if required.

### Negative

- Vendor activation becomes an inter-domain collaboration.
- Event-driven communication is required between Vendor and Compliance.
- Temporary inconsistencies must be tolerated while compliance processing completes.

---

# 4. Alternatives Considered

### Compliance Embedded Within Vendor

Rejected because it couples regulatory policy to the Vendor lifecycle and creates unnecessary complexity.

### Multiple Pending States

Rejected because they expose internal regulatory workflow within the Vendor lifecycle and unnecessarily complicate state management.

### Compliance Before Vendor Creation

Rejected because an organisation may legitimately begin onboarding before satisfying every trading requirement.

# 5. References

- ADR-000 — Architectural Decision Register
- ADR-003 — Event-Driven Collaboration
- ADR-004 — Vendor Registration Establishes Business Existence
- HJ-001 — Project Vision
- HJ-002 — Architectural Principles
- HJ-003 — Ubiquitous Language Guide
- HJ-004 — Vendor Domain Models

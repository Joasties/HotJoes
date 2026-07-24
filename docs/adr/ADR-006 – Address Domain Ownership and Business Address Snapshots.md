# ADR-006 – Address Domain Ownership and Business Address Snapshots

| **Document ID** | ADR-006 |
|-----------------|---------|
| **Document Title** | Address Domain Ownership and Business Address Snapshots |
| **Version** | 1.0 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 23 July 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 23 July 2026 | Initial Architectural Decision Record. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted |
| ADR-005 | Registered Information vs Vendor Managed Information | Accepted |
| HJ-003 | Ubiquitous Language Guide | Draft |
| HJ-004 | Vendor Domain Models | Draft |
| HJ-104 | Vendor Registration Fields Matrix | Draft |

---

# 1. Context

The HotJoes platform is organised around bounded contexts, each of which owns its own business concepts and authoritative data.

During Vendor Registration, a business address must be captured. While the address forms part of the Vendor's business identity, the responsibility for searching, validating, normalising and maintaining addresses belongs to the Address bounded context.

The Vendor domain therefore requires access to information owned elsewhere while remaining capable of operating independently and preserving its own historical business records.

The architecture requires a clear distinction between **ownership of address information** and **ownership of the Vendor's use of that address**.

---

# 2. Decision

The **Address** bounded context shall be the sole authoritative owner of address information.

The Address domain is responsible for:

- address search;
- address validation;
- address normalisation;
- canonical address identifiers;
- management of authoritative address data.

The Vendor bounded context owns the relationship between a Vendor and its business address.

When a Vendor is registered, the Vendor aggregate shall retain:

- the canonical Address identifier; and
- an immutable snapshot of the business address captured during registration.

The snapshot forms part of the Vendor's business history and exists independently of subsequent changes made within the Address domain.

The Vendor domain shall not validate, normalise or otherwise become authoritative for address information owned by the Address domain.

This pattern represents an application of the broader architectural principle established in ADR-002: consuming information does not transfer ownership.

---

# 3. Consequences

### Positive

- Clear ownership of address data is maintained.
- Historical Vendor information remains stable even if authoritative address data changes.
- Vendor behaviour does not depend upon synchronous access to the Address domain.
- Future changes to address validation or normalisation occur entirely within the Address domain.
- The pattern provides a reusable approach for other cross-domain information ownership scenarios.

### Negative

- Business address information intentionally exists in more than one bounded context.
- Synchronisation between the canonical address and historical snapshots is not automatic or expected.
- Developers must understand the distinction between authoritative data and historical business records.

---

# 4. Alternatives Considered

## Vendor Owns Address Information

Rejected because it duplicates address validation and maintenance responsibilities across bounded contexts, weakening ownership and increasing inconsistency.

## Vendor Stores Only an Address Identifier

Rejected because historical business records should remain understandable even if the authoritative address changes or becomes unavailable.

## Vendor Maintains a Live Copy of Address Data

Rejected because it creates ambiguity regarding which bounded context is authoritative and introduces unnecessary synchronisation responsibilities.

---

# 5. Related Decisions

This decision builds upon:

- ADR-002 — Business Capabilities and Bounded Contexts
- ADR-005 — Registered Information vs Vendor Managed Information

This decision demonstrates one application of the information ownership principles established by ADR-002.

---

# 6. References

- HJ-002 — Architectural Principles
- HJ-003 — Ubiquitous Language Guide
- HJ-004 — Vendor Domain Models
- HJ-104 — Vendor Registration Fields Matrix

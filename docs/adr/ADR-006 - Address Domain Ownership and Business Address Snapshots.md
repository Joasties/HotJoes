# ADR-006 – Address Domain Ownership and Business Address Snapshots

| **Document ID** | ADR-006 |
|-----------------|---------|
| **Document Title** | Address Domain Ownership and Business Address Snapshots |
| **Version** | 1.3 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 17 August 2026 |

---

# Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 23 July 2026 | Initial Architectural Decision Record. |
| 1.1 | 27 July 2026 | Applied CR-015 to enforce Address Snapshot ownership and the trust boundary between Vendor Registration and the Address Domain. |
| 1.2 | 17 August 2026 | Applied CR-047. Defined the Address application boundary, immutable Address Resolution reference, contextual resolution, translation and failure semantics approved by CON-006–CON-010. |
| 1.3 | 17 August 2026 | Applied CR-056. Clarified the positional translation from Address source Lines 1–4 to `RecipientOrOrganisationName` and `BusinessAddressSnapshot.AddressLine1–3`. |

---

# Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted |
| ADR-005 | Registered Information vs Vendor Managed Information | Accepted |
| HJ-003 | Ubiquitous Language Guide | Approved |
| HJ-004 | Vendor Domain Models | Approved |
| HJ-104 | Vendor Registration Fields Matrix | Approved |

---

# 1. Context

The HotJoes platform is organised around bounded contexts, each of which owns its own business concepts and authoritative data.

During Vendor Registration, a business address must be captured. While the address forms part of the Vendor's business identity, the responsibility for searching, validating, normalising and maintaining addresses belongs to the Address bounded context.

The Vendor domain therefore requires access to information owned elsewhere while remaining capable of operating independently and preserving its own historical business records.

The architecture requires a clear distinction between **ownership of address information** and **ownership of the Vendor's use of that address**.

---

# 2. Address Ownership Principles

The **Address** bounded context shall be the sole authoritative owner of address information.

The Address domain is responsible for:

- address search;
- address validation;
- address normalisation;
- canonical address identifiers;
- management of authoritative address data.

It is also responsible for deciding whether search criteria are sufficient, returning a single result or a reasonably small selection list, and requesting refinement where necessary. A successful selection always represents a complete authoritative Address result; an incomplete result is not a successful Address outcome.

The Vendor bounded context owns the relationship between a Vendor and its business address.

When a Vendor is registered, the Vendor aggregate shall retain:

- the canonical Address identifier; and
- an immutable snapshot of the business address captured during registration.

The snapshot forms part of the Vendor's business history and exists independently of subsequent changes made within the Address domain.

The Vendor domain shall not validate, normalise or otherwise become authoritative for address information owned by the Address domain.

## Address Snapshot Ownership Invariant

When a Vendor is created, the Vendor aggregate shall store only:

- the canonical Address identifier returned by the Address Service; and
- the immutable Business Address Snapshot exactly as returned by the Address Service for that identifier.

Neither the Vendor Domain nor any caller shall supply, override, normalise, derive or invent Business Address Snapshot content or regulatory-authority values.

Where a registration request presents an approved Address Resolution identifier or equivalent reference, the application shall obtain the canonical Address identifier, Business Address Snapshot and all derived regulatory-authority information directly from the Address Service.

Any client-supplied snapshot content or regulatory-authority values shall be ignored or rejected.

This pattern represents an application of the broader architectural principle established in ADR-002: consuming information does not transfer ownership.

## Address Resolution and Consumption Boundary

The client shall not progress beyond Address selection or submit `RegisterVendor` until the Address Domain has returned a complete valid Address result and its opaque Address Resolution reference.

The reference:

- is issued only for a complete authoritative result containing a `CanonicalAddressId`;
- is permanent, non-expiring, non-revocable, reusable and non-consuming;
- binds the original immutable Address result and the Trading Location declared when that result is selected;
- may be resolved only with that same declared Trading Location; and
- returns the original result on every successful repeated resolution rather than recalculating current Address data.

An unknown or fabricated reference returns `InvalidReference`. A reference used with a different Trading Location, or a known reference whose immutable result cannot satisfy its bound context, returns `InvalidAddressResult`.

Vendor Application code depends on an application port. A typed adapter invokes the Address capability synchronously and translates the Address-owned contract into Vendor-owned values. Neither the Vendor Domain Model nor transport code depends on the Address implementation, its stub, or its foreign contract types.

The translated `BusinessAddressSnapshot` contains required `AddressLine1`, optional `AddressLine2`, optional `AddressLine3`, required `PostTown`, required `Postcode`, optional `County`, and optional `RecipientOrOrganisationName`.

Translation is positional. Address source Line 1 maps to `RecipientOrOrganisationName`; source Lines 2, 3 and 4 map respectively to `AddressLine1`, `AddressLine2` and `AddressLine3`. Post Town, Postcode and optional County map directly to their corresponding snapshot fields. Blank or absent optional source fields remain absent in the corresponding optional snapshot fields. No source-line concatenation, compression, shifting or reordering occurs.

The Vendor Application adapter performs this positional type translation without interpreting, normalising, deriving or inventing Address-owned content. `RecipientOrOrganisationName` is retained when supplied, is not compared with Legal Operator Name or Trading Name, and does not affect registration validity, Vendor identity or Address authority.

Food Registration Authority is returned for every valid Address result. Primary Trading Authority is returned when the supplied Trading Location is `Stall` and absent for other Trading Locations. Epic 1 stubs provide explicit valid and invalid authority outcomes without implementing production authority-matching algorithms.

Semantic failures (`InvalidReference` and `InvalidAddressResult`) fail fast. Technical timeout, unavailability or transient failure produces a controlled retryable application failure. The caller may retry `RegisterVendor` using the same permanent reference. Epic 1 performs no in-process automatic retry and includes no circuit breaker.

---

# 3. Consequences

### Positive

- Clear ownership of address data is maintained.
- Historical Vendor information remains stable even if authoritative address data changes.
- Vendor aggregate behaviour does not depend upon Address invocation mechanics; RegisterVendor application orchestration resolves Address information synchronously before aggregate creation.
- Future changes to address validation or normalisation occur entirely within the Address domain.
- The trust boundary prevents client-authored Address Snapshot or regulatory-authority information from becoming Vendor business state.
- The pattern provides a reusable approach for other cross-domain information ownership scenarios.

### Negative

- Business address information intentionally exists in more than one bounded context.
- Synchronisation between the canonical address and historical snapshots is not automatic or expected.
- Developers must understand the distinction between authoritative data and historical business records.
- Vendor Registration must obtain Address Snapshot and regulatory-authority information from the Address Service before Vendor creation.

---

# 4. Alternatives Considered

## Vendor Owns Address Information

Rejected because it duplicates address validation and maintenance responsibilities across bounded contexts, weakening ownership and increasing inconsistency.

## Vendor Stores Only an Address Identifier

Rejected because historical business records should remain understandable even if the authoritative address changes or becomes unavailable.

## Vendor Maintains a Live Copy of Address Data

Rejected because it creates ambiguity regarding which bounded context is authoritative and introduces unnecessary synchronisation responsibilities.

## Vendor Accepts Caller-Supplied Address Snapshot Information

Rejected because caller-supplied snapshot or regulatory-authority values would bypass the Address Domain trust boundary and could produce inconsistent historical business records.

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

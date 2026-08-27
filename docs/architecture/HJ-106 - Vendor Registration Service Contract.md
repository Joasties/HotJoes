# HJ-106 - Vendor Registration Service Contract

| Property | Value |
|---|---|
| **Document ID** | HJ-106 |
| **Document Title** | Vendor Registration Service Contract |
| **Version** | 1.8 |
| **Status** | Approved |
| **Classification** | Service Contract |
| **Owner** | Project Architecture |
| **Last Updated** | 26 August 2026 |

## Revision History

| Version | Date | Description |
|---|---|---|
| 0.1 | 8 August 2026 | Initial service contract regenerated from the updated Vendor Registration artefacts using PR-002, including Register Vendor, Retrieve Registered Vendor, controlled idempotency conflict behaviour and explicit Register Vendor completion semantics. |
| 1.0 | 8 August 2026 | Regenerated from the latest approved Vendor artefacts using PR-002 and promoted as the Approved service-contract baseline. Reconciled HJ-105 v3.2 and removed the resolved idempotency alignment records without changing the approved business behaviour. |
| 1.1 | 14 August 2026 | Applied CR-040. Assessed the contract against the first HJ-012 Approved architecture baseline; found no change to normative business behaviour; added architectural traceability and Current Concern dependencies; and aligned durable-publication terminology. |
| 1.2 | 17 August 2026 | Applied CR-053 using PR-002. Defined the consumed Address Resolution business contract and RegisterVendor failure outcomes derived from HJ-003, HJ-004, HJ-104, HJ-105, HJ-011 and ADR-006. |
| 1.3 | 17 August 2026 | Regenerated using PR-002 from ADR-006 v1.3, HJ-004 v2.5 and HJ-104 v3.4. Defined the positional Address source-line translation without changing other service behaviour. |
| 1.4 | 19 August 2026 | Regenerated using PR-002 from the approved CON-013 baseline, HJ-104 v3.5 and HJ-105 v3.6. Defined the composite Vendor uniqueness identity, semantic registration equivalence and post-Address-resolution replay sequencing; removed the obsolete caller-supplied idempotency identity convention. |
| 1.5 | 22 August 2026 | Regenerated using PR-002 from the approved CON-019 and CON-020 baseline. Defined the Vendor Application-owned pre-outbox translation boundary and the VendorRegistered Integration Event v1 envelope, payload, independent BusinessAddress representation, serialization and compatibility rules; removed the resolved event-contract deferrals. |
| 1.6 | 23 August 2026 | Regenerated using PR-002 from HJ-004 v2.7, HJ-105 v3.8 and the synchronized HJ-010/HJ-012 v1.8 Approved baseline. Defined the exact VendorRegistered v1 JSON member structure and deterministic contract-owned identifier, timestamp, time-only and enum representations; removed the resolved wire-format ambiguity. |
| 1.7 | 25 August 2026 | Regenerated using PR-002 from HJ-004 v2.8, HJ-104 v3.6, HJ-105 v3.9 and the synchronized HJ-010/HJ-012 v1.9 Approved baseline. Defined the approved CON-023–CON-026 HTTP/JSON contract, controlled error mapping, validation allocation and Contact Email and Primary Contact Telephone profiles; removed superseded proposed and unresolved API wording. |

| 1.8 | 26 August 2026 | Regenerated using PR-002 from HJ-104 v3.6, HJ-105 v4.0 and the synchronized HJ-010/HJ-012 v2.0 Approved baseline. Consolidated every Application validation failure into one `RequestValidationFailure` and one `registrationValidationFailed` API outcome; removed the superseded separate declaration and conditional-rule failure outcomes and codes. |

## Related Documents

| Document ID | Title | Status | Relevance |
|---|---|---|---|
| PR-002 | Infer Service Contract from Approved Domain Artefacts | Governing prompt | Governs the inference method and document structure |
| HJ-002 | Architectural Principles | Approved | Capability ownership, contracts, events and architectural simplicity |
| HJ-003 | Ubiquitous Language Guide | Approved | Authoritative Vendor terminology and query language |
| HJ-004 v2.8 | Vendor Domain Models | Approved | Vendor aggregate, lifecycle, invariants, events, retrieval model and defensive Contact Email and Telephone Value Object rules |
| HJ-005 | Coding Standards | Approved | API boundary, validation, error and HTTP conventions |
| HJ-010 v2.0 | Current Application Architectural Concerns | Approved | Records the approved CON-023–CON-026 API and validation cohort and the amended unified validation-failure treatment for CON-025, CON-026 and CON-040, while retaining relay and broker-delivery mechanics under CON-018 and CON-021 |
| HJ-011 v2.0 | Epic 1 Vendor Registration Implementation Scope | Approved | Places the thin Minimal API, technical contract, unified controlled validation-failure outcome and validation boundary within Epic 1 delivery scope |
| HJ-012 v2.0 | Established Application Architecture Patterns | Approved | Defines the approved HTTP adaptation, technical API contract, unified validation-failure mapping and validation allocation |
| HJ-104 v3.6 | Vendor Registration Fields Matrix | Approved | Authoritative registration information, business validation, canonicalisation and Contact Email and Telephone profiles |
| HJ-105 v4.0 | Vendor Registration Sequence Diagram | Approved | Authoritative interaction order, API adaptation, validation ordering, outcomes and failure behaviour |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted | Capability and persistence ownership |
| ADR-003 v1.2 | Event-Driven Collaboration | Accepted | Pre-outbox Application mapping and prohibition of relay-time reconstruction |
| ADR-004 | Vendor Lifecycle Begins After Successful Registration | Accepted | Registration Session, service and Vendor-existence boundaries |
| ADR-005 | Registered Information vs Vendor Managed Information | Accepted | Information classification and editability |
| ADR-006 | Address Domain Ownership and Business Address Snapshots | Accepted | Address trust boundary and snapshot invariant |
| ADR-007 | Vendor Compliance as a Separate Bounded Context | Accepted | Compliance and activation separation |
| ADR-008 v1.4 | Idempotent Operations and Reliable Event Publication | Accepted | Register Vendor idempotency, atomic publication staging and immutable serialized-event handling |
| CR-040 | Align HJ-106 with the Approved Architecture Baseline | Applied | Records the architecture-impact assessment and authorises this traceability-only revision |

# 1. Purpose

This document defines the business service contract for the Epic 1 Vendor Registration vertical slice. It translates the approved domain model into reviewable operations without redesigning the Vendor Domain.

The contract defines:

- registration of one Vendor from a complete, self-contained request;
- retrieval of one registered Vendor by VendorId;
- request and result information;
- validation and aggregate invariants;
- lifecycle effects and side-effect constraints;
- Address Domain collaboration;
- Domain and Integration Events;
- idempotency and reliable-publication behaviour;
- controlled business failures; and
- the approved Epic 1 HTTP representation.

Part A, Sections 1–5, is the inferred business contract. Part B, Section 6, defines the approved Epic 1 HTTP representation derived from CON-023–CON-026. The API representation adapts and does not redefine the transport-independent Application contract.

# 2. Source Artefacts

| Artefact | Why Used | Authority Provided |
|---|---|---|
| HJ-002 | Constrains interpretation of capability boundaries and published contracts | Vendor owns Vendor behaviour and persistence; contracts are explicit; events represent completed facts; prefer simplicity |
| HJ-003 v2.3 | Supplies authoritative language | Vendor State, Vendor Registration, Registered Information, Vendor Managed Information, Vendor Administrator, Retrieve Registered Vendor and Registered Vendor Details |
| HJ-004 v2.8 | Defines the business model exposed by this contract | Aggregate properties, creation invariants, Contact Email and Telephone Value Objects, lifecycle, commands, events, the exact VendorRegistered v1 published contract, persisted retrieval source, result content and query side-effect invariant |
| HJ-005 | Separates business contract from implementation conventions | Separate API models, controlled errors, layered validation, status-code guidance and safe response rules |
| HJ-104 v3.6 | Is the authoritative registration information contract | Required, optional, conditional, derived and transient fields; validation ownership; exact contact profiles; canonicalisation; composite Vendor uniqueness identity; semantic registration equivalence; ownership; lifecycle |
| HJ-105 v4.0 | Defines observable interaction behaviour | Thin HTTP adaptation, complete request boundary, unified validation-failure outcome and mapping, validation ordering, Address collaboration, idempotency, pre-outbox event mapping, reliable publication and registered Vendor retrieval |
| ADR-002 | Prevents ownership leakage | Vendor capability owns Vendor registration and persistence; other capabilities retain their own behaviour and data |
| ADR-003 v1.2 | Governs asynchronous collaboration | Completed business facts cross boundaries through explicit Integration Events mapped before outbox persistence and never reconstructed by the relay |
| ADR-004 | Defines when a Vendor exists | No Vendor exists before successful registration; Registration Session remains outside the Vendor service boundary |
| ADR-005 | Governs information classification | Registered Information and Vendor Managed Information have distinct post-registration lifecycles |
| ADR-006 v1.3 | Defines Address authority | Address Domain supplies canonical identity, immutable snapshot and applicable regulatory authorities |
| ADR-007 | Preserves compliance separation | Registration creates Pending Activation; Compliance and activation decisions remain outside registration |
| ADR-008 v1.4 | Defines mandatory reliability behaviour | Explicit Register Vendor idempotency safeguard; atomic persistence; pre-outbox VendorRegistered mapping; serialization and immutable outbox storage; retry without reconstruction or repeated business effects |

No contradictory business rule was found among the supplied source artefacts. HJ-104 v3.6 specializes the approved registration-information and validation rules. HJ-004 v2.8 and HJ-105 v4.0 preserve the approved concrete VendorRegistered v1 contract, defensive contact invariants and HTTP adaptation sequence while consolidating Application validation failures into one outcome. HJ-005 supplies compatible engineering constraints; CON-023–CON-026 provide the approved Epic 1 technical specialization.

HJ-010 v2.0 and HJ-012 v2.0 are the controlled architectural governance baseline for this contract. CON-006–CON-011 establish the Address collaboration boundary, consumed result and failure semantics. CON-013–CON-017 and CON-028 establish idempotency, concurrency, replay persistence, transaction, reliable staging and database enforcement. CON-019/CON-020 establish the published-event boundary. CON-023–CON-026 and amended CON-040 establish the thin HTTP adaptation, exact API representation, unified controlled validation failure and validation allocation used below. Relay processing and broker delivery remain governed separately by unresolved CON-018 and CON-021.

# 3. Business Operation Summary

| Operation | Business Purpose | Owning Bounded Context | Preconditions | Successful Outcome |
|---|---|---|---|---|
| `RegisterVendor` | Create a Vendor from complete and valid registration information | Vendor | One complete request; mandatory declarations accepted; approved Address Resolution reference resolved; all field rules and aggregate invariants satisfied; composite Vendor identity and replay outcome established | One Vendor is committed in `PendingActivation` and `Offline`, or an equivalent replay returns its original committed result; `VendorRegistered` publication work exists exactly once |
| `RetrieveRegisteredVendor` | Return the persisted details established through successful Vendor Registration | Vendor | VendorId supplied; an existing Vendor is identified | Registered Vendor Details is returned from persisted Vendor state without mutation, events or cross-domain collaboration |

# 4. Register Vendor Business Contract

## 4.1 Business Intent

`RegisterVendor` requests creation of one Vendor representing one trading location. Vendor Registration begins only when the Vendor Registration capability receives a complete, self-contained request.

The operation does not create or manage a Registration Session, authenticate an applicant, decide compliance, activate a Vendor or authorise trading.

## 4.2 Preconditions

- The caller submits the complete request in one invocation.
- All mandatory client-authored information is present.
- Conditional information is present or absent according to the approved rules.
- All three Registration Declarations are explicitly accepted.
- The request carries an approved Address Resolution reference.
- Address-owned values can be obtained authoritatively from the Address Domain.
- The Address Resolution reference resolves successfully so the Address Domain supplies the authoritative Canonical Address Identifier.
- The application derives the composite Vendor uniqueness identity from normalized Trading Name, normalized Legal Operator Name and Canonical Address Identifier before aggregate creation.
- The application evaluates semantic registration equivalence for an existing identity before aggregate creation.
- The same composite identity with materially different registration information is treated as `IdempotencyConflict` and never as a new registration or update.

No Vendor or server-side Registration Session needs to exist before invocation.

## 4.3 Request Information

| Field | Business Meaning | Data Owner | Required Status | Source Artefact | Validation | Persisted or Transient | Registered or Vendor Managed | Notes |
|---|---|---|---|---|---|---|---|---|
| Trading Name | Public name under which the Vendor trades | Vendor | Required | HJ-104 §§2, 5.2, 5.3, 5.6 | 1–160 characters; trimmed, case-insensitive comparison contributes to the composite Vendor identity | Persisted | Registered Information | The registered display value is retained and is not replaced by its comparison form |
| Legal Operator Name | Registered legal name of the operator | Vendor | Required | HJ-003 §3.11; HJ-104 §§2, 5.2, 5.3, 5.6 | 1–160 characters; trimmed, case-insensitive comparison contributes to the composite Vendor identity | Persisted | Registered Information | “Company Name” is historical terminology only; the registered display value is retained |
| Legal Operator Type | Classification of the responsible legal person or organisation | Vendor | Required | HJ-104 §§2, 5.1, 5.2 | Controlled value: Sole Trader, General Partnership, Limited Company, Limited Liability Partnership, Charitable Community Group or Charitable Incorporated Organisation | Persisted | Registered Information | Drives Company Registration Number applicability |
| Company Registration Number | Government-issued registration identifier where applicable | Vendor | Conditional | HJ-104 §§2, 5.1–5.3 | Required only for Limited Company, Limited Liability Partnership or Charitable Incorporated Organisation; otherwise absent; pattern `^(?:[A-Za-z]{2})?\d{6,8}$`; alphabetic prefix canonicalised to uppercase; format only | Persisted when applicable | Registered Information | No Companies House existence verification in Epic 1 |
| Trading Location | Controlled operating classification | Vendor | Required | HJ-104 §§2, 5.1, 5.2 | Restaurant, Stall or Kitchen | Persisted | Registered Information | Part of Trading Characteristics |
| Opening Hours – Start Time | Beginning of the registered operating interval | Vendor | Required | HJ-104 §§2, 5.2 | Valid time; must not be constrained to occur before End Time | Persisted | Registered Information | Overnight periods are valid |
| Opening Hours – End Time | End of the registered operating interval | Vendor | Required | HJ-104 §§2, 5.2 | Valid time; may be earlier than Start Time for overnight operation | Persisted | Registered Information | Part of Trading Characteristics |
| Service Includes Hot Food | Whether food or drink is supplied heated above ambient temperature | Vendor | Required | HJ-104 §§2, 5.1 | Boolean | Persisted | Registered Information | Part of Trading Characteristics |
| Alcohol Service | Whether the Vendor supplies alcohol | Vendor | Required | HJ-104 §§2, 5.1 | Boolean | Persisted | Registered Information | Part of Trading Characteristics |
| Contact Name | Primary business contact name | Vendor | Required | HJ-104 §§2, 5.2 | 1–100 characters | Persisted | Registered Information | Stored in Primary Contact |
| Contact Email | Business correspondence address | Vendor | Required | HJ-104 §§2, 5.2–5.3 | Trim surrounding whitespace; exactly one `@`; local part 1–64 characters; total length at most 254; approved ASCII dot-atom local part without leading, trailing or consecutive dot; domain with at least two valid 1–63-character labels; prohibit display names, comments, quoted local parts, domain literals and Unicode addresses | Persisted in canonical form | Registered Information | Preserve local-part case and lowercase the domain; structural validation only |
| Contact Telephone | Primary contact number | Vendor | Required | HJ-104 §§2, 5.2–5.3 | Trim surrounding whitespace; raw input permits digits, spaces, hyphens, parentheses and one leading `+`; remove presentation characters; reject bare `44`; normalized value matches the approved pragmatic UK telephone expression; exclude unsupported specialist, short-code and legacy ranges | Persisted in canonical `+44` form | Registered Information | Structural validation only; allocation, activity, reachability and ownership verification are outside Epic 1 |
| Address Resolution Reference | Reference to the approved Address selection | Address Domain | Required | HJ-104 §§2, 5.4, 6; HJ-105 §5 | Must resolve to an approved authoritative Address result | Transient request input | Neither | Sole request-side link to Address-owned values |
| Website | Vendor website | Vendor | Optional | HJ-104 §§2, 5.2 | Valid HTTPS URL | Persisted when supplied | Vendor Managed Information | May be changed through future Vendor Management |
| Business Description | Vendor-provided business description | Vendor | Optional | HJ-104 §§2, 5.2 | Maximum 2,000 characters | Persisted when supplied | Vendor Managed Information | May be changed through future Vendor Management |
| Authorised to Register Business | Applicant confirms authority to register | Applicant | Required | HJ-104 §2 and Registration Declaration Classification | Must be explicitly accepted (`true`) | Transient | Registration Declaration | Never Vendor state or event content |
| Information Accurate | Applicant confirms submitted information is accurate | Applicant | Required | HJ-104 §2 and Registration Declaration Classification | Must be explicitly accepted (`true`) | Transient | Registration Declaration | Never Vendor state or event content |
| Accept HotJoes Platform Terms | Applicant accepts applicable platform terms | Applicant | Required | HJ-104 §2 and Registration Declaration Classification | Must be explicitly accepted (`true`) | Transient | Registration Declaration | Never Vendor state or event content |
The request must not authoritatively supply Canonical Address Identifier, Business Address Snapshot, Food Registration Authority or Primary Trading Authority. If present, client-authored Address-owned values are rejected or ignored.

## 4.4 Derived Information

| Information | Derivation and Ownership | Persistence and Use |
|---|---|---|
| VendorId | Created by the Vendor Domain after successful validation | Persisted as aggregate identity and returned in the successful outcome |
| RegisteredAt | Recorded by the Vendor Domain on successful creation | Persisted and available in registered Vendor details and event contracts where specified |
| Canonical Address Identifier | Supplied exclusively by the Address Domain from the approved Address Resolution | Persisted with the snapshot as Registered Information |
| Composite Vendor Uniqueness Identity | Derived by the Vendor Application from trimmed, case-insensitive Trading Name, trimmed, case-insensitive Legal Operator Name and authoritative Canonical Address Identifier | Used to identify an existing Epic 1 Vendor registration; its storage and database enforcement are governed separately by CON-015 and CON-028 |
| Semantic Registration Fingerprint | Derived deterministically from all materially relevant registration information after approved canonicalisation | Versioned deterministic UTF-8 canonical representation hashed using SHA-256; excludes transient declarations, the opaque Address Resolution reference, server-generated values and technical metadata |
| Business Address Snapshot | Supplied exclusively by the Address Domain from the approved Address Resolution | Persisted immutably using the complete seven-field schema defined below |
| Food Registration Authority | Derived and supplied by the Address Domain from the approved Business Address or mobile-unit base address | Persisted as Registered Information |
| Primary Trading Authority | Derived and supplied by the Address Domain | Persisted only when Trading Location is `Stall`; absent otherwise |
| Initial Vendor State | Established by successful Vendor creation | `PendingActivation` |
| Initial Trading Preference | Established by successful Vendor creation | `Offline` |
| Compliance Requirements | Determined later by the Compliance capability from published information | Not returned, stored or decided by Register Vendor |

## 4.5 Business Validation

The Vendor Application authoritatively validates the complete raw `RegisterVendor` intent regardless of client-side or API convenience validation. It validates all HJ-104 field, Registration Declaration, conditional and cross-field rules before Address resolution, uniqueness identity or fingerprint determination, Aggregate creation or persistence. Any Application validation failure returns one immutable `RequestValidationFailure` containing every independently detectable validation error. Registration Declaration and conditional-rule errors are entries in that result rather than separate top-level outcomes.

Successful validation supplies canonical values to every downstream stage. These include uppercase Company Registration Number, unmodified registered name display values alongside trimmed case-insensitive comparison forms, Contact Email with surrounding whitespace removed and domain lowercased while preserving local-part case, and Primary Contact Telephone stored in canonical `+44` form. No downstream stage may return to uncanonicalized client input. The Vendor Domain remains the final defensive owner of Aggregate and Value Object invariants.

The Contact Email profile permits ASCII letters, digits and ``. ! # $ % & ' * + - / = ? ^ _ ` { | } ~`` in the local part. A dot is prohibited first, last or consecutively. The domain contains at least two dot-separated labels; each label contains 1–63 ASCII letters, digits or hyphens and does not begin or end with a hyphen. Display names, comments, quoted local parts, domain literals and internationalized Unicode addresses are prohibited. Validation establishes plausible structure only; allocation, deliverability and ownership verification are outside Epic 1.

For Primary Contact Telephone, presentation spaces, hyphens and parentheses are removed while a single permitted leading `+` is retained. Bare `44` is rejected. The normalized value shall match `^(?:(?:\+44|0)7\d{9}|(?:\+44|0)(?:1|2|3|5|8|9)\d{8,9})$`. A domestic value is converted by removing its leading `0` and prepending `+44`; an accepted `+44` value is retained. Specialist, short-code and legacy ranges outside this profile are rejected. Validation does not establish allocation, activity, reachability or ownership.

Creation invariants are:

- all mandatory registration information is complete and valid;
- Company Registration Number is present if and only if Legal Operator Type is Limited Company, Limited Liability Partnership or Charitable Incorporated Organisation;
- Primary Trading Authority is present if and only if Trading Location is `Stall`;
- Canonical Address Identifier and immutable Business Address Snapshot originate together from the Address Domain;
- caller-authored Address snapshots or authority values cannot replace Address-owned information;
- every Registration Declaration is explicitly accepted;
- one Vendor represents one trading location;
- final Vendor identity evaluation occurs only after authoritative Address resolution supplies the Canonical Address Identifier;
- Trading Name and Legal Operator Name are trimmed and compared without regard to case for identity purposes without changing their registered display values;
- equivalent registration information for an existing composite identity returns the original committed successful result without repeating any business effect;
- materially different registration information for the same composite identity returns `IdempotencyConflict` and never updates the Vendor;
- a new Vendor starts in `PendingActivation` and `Offline`; and
- incomplete or invalid registration creates no Vendor, completed business fact, event or publication work.

Trading Characteristics comprise Trading Location, Opening Hours, Service Includes Hot Food and Alcohol Service. They describe the operation; they do not replace Legal Operator Type, which describes legal identity and registration obligations.

## 4.6 Address Domain Collaboration

`RegisterVendor` accepts one permanent opaque Address Resolution reference obtained by the client from a complete valid Address selection. Selection binds the reference to the original immutable Address result and the Trading Location declared at that time. The reference is non-expiring, non-revocable, reusable and non-consuming.

The Vendor Registration Application resolves it synchronously through an application-facing Address port, supplying the request's declared Trading Location. A typed adapter invokes the Address capability and translates the foreign result into Vendor-owned values. Invocation, transport and Address contract types do not enter the Vendor Domain Model.

A successful resolution returns the original immutable contextual result and requires:

- `CanonicalAddressId`;
- `BusinessAddressSnapshot` containing `AddressLine1`, optional `AddressLine2`, optional `AddressLine3`, `PostTown`, `Postcode`, optional `County` and optional `RecipientOrOrganisationName`;
- `FoodRegistrationAuthority`; and
- `PrimaryTradingAuthority` when Trading Location is `Stall`, otherwise no Primary Trading Authority.

Address translation is positional. Address source Line 1 maps to optional `RecipientOrOrganisationName`; source Lines 2, 3 and 4 map respectively to required `BusinessAddressSnapshot.AddressLine1`, optional `BusinessAddressSnapshot.AddressLine2` and optional `BusinessAddressSnapshot.AddressLine3`. Post Town, Postcode and optional County map directly to their corresponding snapshot fields. Blank or absent optional source fields remain absent in the corresponding optional snapshot fields. No source-line concatenation, compression, shifting or reordering occurs. `RecipientOrOrganisationName` is not compared with Legal Operator Name or Trading Name and does not affect registration validity.

An unknown or fabricated reference returns `InvalidReference`. A reference used with a different Trading Location, or a known reference whose immutable result cannot satisfy its bound context, returns `InvalidAddressResult`. These semantic failures fail fast. Timeout, unavailability or transient Address failure returns `AddressServiceTemporarilyUnavailable`, a controlled retryable application failure. The caller may retry RegisterVendor using the same permanent reference. RegisterVendor performs no in-process automatic retry and Epic 1 includes no circuit breaker.

1. The request supplies the approved Address Resolution reference, not authoritative Address content.
2. The Vendor Registration Application requests the approved result from the Address Service.
3. The Address Domain validates the reference and returns the Canonical Address Identifier, immutable Business Address Snapshot, Food Registration Authority and Primary Trading Authority where applicable.
4. The application rejects or ignores caller-authored Address-owned values.
5. The application derives the composite Vendor uniqueness identity and semantic registration fingerprint.
6. The application determines whether processing is a first registration, equivalent replay or `IdempotencyConflict`.
7. Only first registration proceeds to aggregate creation; the Vendor aggregate persists the canonical identifier and snapshot together, with applicable authorities.

`InvalidReference` or `InvalidAddressResult` produces the corresponding controlled semantic failure. A technical Address failure produces `AddressServiceTemporarilyUnavailable`. No such outcome creates a Vendor, Domain Event, Integration Event or publication work.

## 4.7 Successful Outcome

A first successful invocation:

- commits exactly one Vendor aggregate;
- assigns VendorId and RegisteredAt;
- persists Registered Information and supplied Vendor Managed Information;
- persists the Address-owned canonical identifier, immutable snapshot and applicable authorities;
- establishes Vendor State `PendingActivation` and Trading Preference `Offline`;
- records the internal `VendorRegistered` Domain Event;
- maps the completed business fact and registration-time information to the approved `VendorRegistered` Integration Event v1 before outbox persistence;
- serializes that contract once as UTF-8 camel-case JSON;
- atomically records the immutable serialized event as durable publication work; and
- synchronously returns a committed successful outcome containing, at minimum, VendorId and Vendor State `PendingActivation`.

Pending Activation and Integration Event dispatch continue asynchronously and do not alter the synchronous successful outcome.

RegisteredAt, Trading Preference and other persisted Vendor properties are committed state but are deliberately excluded from the approved minimum `RegisterVendor` HTTP response. Callers obtain complete committed state through `RetrieveRegisteredVendor`.

## 4.8 Vendor Lifecycle Transition

Before successful registration, no Vendor exists. Successful `RegisterVendor` creates the Vendor directly in `PendingActivation`; it is not a transition from a draft or registration state. Trading Preference begins `Offline`, and the Vendor cannot trade.

Registration does not activate the Vendor, establish activation eligibility or make it operationally available.

## 4.9 Idempotency

`RegisterVendor` is not naturally idempotent. The application derives the Epic 1 Vendor uniqueness identity only after the Address Domain has resolved the permanent reference and supplied the authoritative Canonical Address Identifier.

The composite identity consists of:

- Trading Name compared after trimming and without regard to case;
- Legal Operator Name compared after trimming and without regard to case; and
- Canonical Address Identifier.

The registered Trading Name and Legal Operator Name remain unchanged display values. The opaque Address Resolution reference does not form part of the identity.

Semantic registration equivalence compares all materially relevant registration information after its approved canonicalisation. It excludes transient Registration Declarations, the opaque Address Resolution reference, server-generated values and technical metadata.

For the same composite identity and semantically equivalent previously successful registration, the operation shall:

- return the original successful outcome;
- create no additional Vendor;
- record no additional Domain Event or completed business fact;
- create no additional durable publication record;
- publish no additional Integration Event; and
- initiate no additional Pending Activation Process.

Concurrent equivalent requests must converge on one processing owner and one successful outcome. Registration Session state is never consulted. The approved PostgreSQL concurrency authority, permanent replay persistence, atomic transaction, deterministic fingerprint and explicit database enforcement are governed by CON-014, CON-015, CON-016 and CON-028.

If the same composite identity is associated with materially different registration information from the previously successful registration, the operation shall return `IdempotencyConflict`. It shall not create or modify a Vendor, record a completed business fact or Domain Event, create publication work, publish an Integration Event or initiate Pending Activation. Vendor updates require a separate future administration operation. The approved HTTP mapping is `409 Conflict` with code `idempotencyConflict`.

## 4.10 Domain and Integration Events

| Event | Business Meaning | Producing Bounded Context | Minimum Payload | Information Intentionally Excluded |
|---|---|---|---|---|
| Internal `VendorRegistered` Domain Event | Successful Vendor creation is a completed fact inside the Vendor Domain | Vendor | No minimum payload is prescribed by HJ-004 | Registration Declarations; no requirement to mirror the Integration Event; infrastructure metadata |
| Published `VendorRegistered` Integration Event v1 | Announces successful registration so downstream Pending Activation and Compliance processing can begin without a synchronous Vendor callback | Vendor | Stable camel-case envelope: `eventId`, `eventType` `VendorRegistered`, `eventVersion` `1`, `occurredAt` and immutable `payload`. Payload: `vendorId`, `registeredAt`, `vendorState`, `tradingPreference`, `legalOperatorType`, nested `tradingCharacteristics`, Integration Event-owned `businessAddress`, `foodRegistrationAuthority` and conditional `primaryTradingAuthority` | Registration Declarations; full aggregate; Vendor Domain `BusinessAddressSnapshot`; Vendor Domain Aggregate, Value Object and enum types; internal persistence and broker representations; information not required to initiate downstream processing |

The Integration Event-owned `BusinessAddress` contains `CanonicalAddressId`, optional `RecipientOrOrganisationName`, required `AddressLine1`, optional `AddressLine2`, optional `AddressLine3`, required `PostTown`, required `Postcode` and optional `County`. It originates exclusively from the authoritative Address result used at registration and does not expose or reuse the Vendor Domain `BusinessAddressSnapshot` type.

An explicit Vendor Application mapper translates the completed internal business fact and registration-time information to the v1 contract before outbox persistence. Vendor Infrastructure serializes the contract once as UTF-8 camel-case JSON and persists that immutable serialized event unchanged within the registration transaction. The Vendor Domain owns neither the Integration Event nor outbox, serialization or broker representations.

The concrete v1 JSON representation is:

```json
{
  "eventId": "00000000-0000-0000-0000-000000000000",
  "eventType": "VendorRegistered",
  "eventVersion": 1,
  "occurredAt": "2026-08-23T10:15:30.0000000Z",
  "payload": {
    "vendorId": "00000000-0000-0000-0000-000000000000",
    "registeredAt": "2026-08-23T10:15:30.0000000Z",
    "vendorState": "pendingActivation",
    "tradingPreference": "offline",
    "legalOperatorType": "limitedCompany",
    "tradingCharacteristics": {
      "tradingLocation": "stall",
      "openingHours": {
        "startTime": "09:00:00",
        "endTime": "17:00:00"
      },
      "serviceIncludesHotFood": true,
      "alcoholService": false
    },
    "businessAddress": {
      "canonicalAddressId": "canonical-address-001",
      "recipientOrOrganisationName": null,
      "addressLine1": "2 High Street",
      "addressLine2": null,
      "addressLine3": null,
      "postTown": "GREENWICH",
      "postcode": "SE10 8AA",
      "county": null
    },
    "foodRegistrationAuthority": "Greenwich Borough Council",
    "primaryTradingAuthority": "Greenwich Borough Council"
  }
}
```

The example fixes the v1 member names and nesting. Its identifier and timestamp values are illustrative; their formats are normative. The representation rules are:

- `eventId` and `payload.vendorId` use lowercase canonical UUID `D` format;
- `occurredAt` and `payload.registeredAt` are converted to UTC and use invariant round-trip `O` format;
- time-only values use invariant `HH:mm:ss` without an offset;
- enum values use lower-camel-case strings matching the approved ubiquitous terms;
- `tradingCharacteristics` and `openingHours` use the nested structures shown;
- the Integration Event contract owns every published representation and exposes no Vendor Domain Aggregate, Value Object or enum type; and
- every optional member remains present with an explicit JSON `null` when absent.

Optional values are represented explicitly as `null`. Compatible optional fields may be added within v1 and consumers must tolerate unknown fields. Removal, renaming, type changes or meaning changes require a new event version. Dispatch publishes the stored event unchanged; retries preserve the original EventId, EventVersion and serialized event. Relay-time reconstruction from current Vendor state is prohibited.

Vendor persistence and durable recording of the serialized Integration Event must commit atomically. Dispatch failure leaves the registered Vendor unchanged and the durable publication record available for retry. A publication retry does not repeat registration or create another business fact or event record.

## 4.11 Business Failures

| Failure | Trigger | Business Meaning | Retryable | Expected Caller Behaviour |
|---|---|---|---|---|
| RequestValidationFailure | One or more request-field, Registration Declaration, conditional or cross-field rules fail | Registration cannot be attempted with the supplied information; the result contains every independently detectable validation error | Yes, after correction | Correct all identified errors, including any required declaration acceptance or Address-context correction, and resubmit |
| InvalidReference | Address Resolution reference is unknown or fabricated | No authoritative Address selection can be established | Yes, after Address search and selection | Select a complete valid Address result and submit its issued reference |
| InvalidAddressResult | A known reference cannot provide a complete immutable result for the request's declared Trading Location | Required Address-owned information cannot form valid Vendor Registered Information | Yes, after correction or reselection | Correct Trading Location or select another complete Address result |
| AddressServiceTemporarilyUnavailable | Address resolution times out or encounters unavailable/transient infrastructure | Registration has not been attempted because authoritative Address information is temporarily unavailable | Yes, as a new caller-controlled attempt using the same permanent reference | Retry later; the service performs no in-process automatic retry |
| Aggregate invariant failure | Valid-looking inputs still violate Vendor creation rules | The Vendor cannot be created in a valid state | Yes, if correctable | Correct the reported business information; do not interpret technical exceptions as business messages |
| Persistence or atomic-recording failure | Vendor and publication work cannot commit atomically | Registration has not succeeded | Yes through the idempotency safeguard | Retry safely; do not treat the Vendor as registered without a successful response |
| Duplicate or concurrent equivalent submission | Same composite Vendor identity and semantically equivalent successful registration information | Registration has already completed | Not a failure; convergent replay | Accept the original committed successful result; do not start another registration |
| Integration Event dispatch failure | Durable publication exists but external dispatch fails | Registration remains successful; downstream notification is delayed | Caller retry not required | Do not register again; infrastructure retries publication |
| IdempotencyConflict | The same composite Vendor identity is associated with materially different registration information from the prior successful registration | The submission is neither an equivalent replay nor an update and must not create or change business state | No as submitted; an update requires a separate future administration operation | Do not treat the response as registration success; use the future authorized administration operation if an existing Vendor must change |

Every pre-commit validation, Address or invariant failure persists no Vendor and records no Domain Event, Integration Event or publication work. Controlled errors must not expose stack traces, database details, internal class names or framework diagnostics.

## 4.12 Retrieve Registered Vendor Business Contract

### 4.12.1 Business Intent and Preconditions

`RetrieveRegisteredVendor(VendorId)` allows a trusted Epic 1 Vendor Administrator to retrieve one existing Vendor directly by VendorId. Authentication and authorisation are outside Epic 1 and introduce no Identity Domain collaboration.

VendorId is the sole lookup criterion. No Registration Session, search, filtering, paging, ownership check or cross-domain callback participates.

### 4.12.2 Authoritative Source and Result Boundary

The persisted Vendor aggregate is the authoritative Epic 1 read source. The Vendor Application loads it from the Vendor Repository and maps its persisted state into Registered Vendor Details. The aggregate itself is never exposed as the service response.

Registered Vendor Details contains:

- VendorId and RegisteredAt;
- Vendor State and Trading Preference;
- Legal Operator Type, Legal Operator Name and applicable Company Registration Number;
- Trading Name;
- Trading Characteristics: Trading Location, Opening Hours, Service Includes Hot Food and Alcohol Service;
- Contact Name, Contact Email and Contact Telephone;
- Canonical Address Identifier and immutable Business Address Snapshot;
- Food Registration Authority and applicable Primary Trading Authority; and
- Website and Business Description where supplied.

It excludes Registration Declarations, Compliance Requirements, Compliance evidence, Compliance state or decisions, Activation decisions, Domain and Integration Event representations, outbox/publication metadata, Identity information and internal persistence metadata.

### 4.12.3 Outcomes and Side Effects

| Outcome | Trigger | Result |
|---|---|---|
| Vendor Found | VendorId identifies an existing persisted Vendor | Return Registered Vendor Details mapped from persisted state |
| Vendor Not Found | VendorId identifies no Vendor | Return a controlled Vendor Not Found outcome |

Retrieval is read-only. It does not change Vendor State, Trading Preference, Registered Information or Vendor Managed Information; record or publish an event; create publication work; initiate Pending Activation; re-resolve an Address; retrieve Compliance information; or require dedicated read-model infrastructure.

# 5. Information Outside Scope

The following are intentionally outside this service contract:

- Registration Session creation, retrieval, reconciliation, storage, expiry or disposal within the Vendor service;
- incomplete or resumable registration drafts;
- authentication, authorisation, Identity account creation and caller-to-Vendor ownership checks;
- Activation and activation decisions;
- Compliance decisions, evidence, state, ongoing monitoring and the completed Pending Activation Process;
- Operational Availability composition;
- post-registration amendment of Registered Information;
- future Vendor Management operations for Website, Business Description or Primary Contact;
- Vendor search, filtering, paging, multiple-Vendor retrieval or lookup by names;
- Address search UI and live Address re-resolution during registered Vendor retrieval;
- multiple premises, Branches, Menu, Ordering, Payment and Delivery behaviour;
- dedicated retrieval projections, eventual consistency and independently optimised read models;
- implementation code, persistence mappings, controllers, handlers and OpenAPI definitions; and
- concrete transport, storage, outbox, broker and serialization technologies.

# 6. Approved HTTP Representation

This section defines the approved Epic 1 HTTP adaptation under CON-023–CON-026. The API uses thin ASP.NET Core Minimal API endpoint adapters. They own HTTP binding, structural request validation, API-to-Application mapping, Application-result-to-HTTP mapping, cancellation-token forwarding, response headers and centralized unexpected-exception handling. They contain no Domain rules, Address resolution, persistence query, transaction, event, outbox or broker behaviour.

## 6.1 Endpoints and JSON Conventions

| Business Operation | Method | Route | Success |
|---|---|---|---|
| Register Vendor | `POST` | `/vendors` | `201 Created` with the original committed registration outcome and `Location` header |
| Retrieve Registered Vendor | `GET` | `/vendors/{vendorId}` | `200 OK` with Registered Vendor Details |

No collection, search, filtering, paging, update or API-versioned endpoint is included in Epic 1.

The wire contract uses:

- `application/json` where a body exists;
- lower-camel-case JSON member names and enum strings using approved ubiquitous terms;
- canonical UUID `D` strings, lowercase in responses;
- UTC invariant round-trip `O` timestamps;
- invariant `HH:mm:ss` time-only strings without an offset;
- explicit JSON `null` for every absent optional response member;
- omission or JSON `null` as equivalent legitimate absence for optional request members; and
- compatible ignoring of unknown request members.

Required request-member presence is distinguished from a supplied default value, including for required booleans.

## 6.2 Register Vendor Request

`addressResolutionReference` is the only request-side link to Address-owned information. The request excludes Canonical Address Identifier, Business Address Snapshot, regulatory authorities, Vendor state, server-generated identifiers, composite identity, semantic fingerprint, persistence/outbox information and Registration Session state.

```json
{
  "tradingName": "Hot Joe's Kitchen",
  "legalOperatorName": "Hot Joe's Foods Limited",
  "legalOperatorType": "limitedCompany",
  "companyRegistrationNumber": "AB123456",
  "tradingCharacteristics": {
    "tradingLocation": "kitchen",
    "openingHours": {
      "startTime": "17:00:00",
      "endTime": "02:00:00"
    },
    "serviceIncludesHotFood": true,
    "alcoholService": false
  },
  "primaryContact": {
    "contactName": "Jordan Smith",
    "contactEmail": "jordan@example.test",
    "contactTelephone": "+442079460123"
  },
  "addressResolutionReference": "addr-resolution-example",
  "website": "https://example.test",
  "businessDescription": "Evening food delivery kitchen.",
  "registrationDeclarations": {
    "authorisedToRegisterBusiness": true,
    "informationAccurate": true,
    "acceptHotJoesPlatformTerms": true
  }
}
```

## 6.3 Success Responses

### 6.3.1 Register Vendor

First successful processing and an equivalent replay both return the original `201 Created` response:

```json
{
  "vendorId": "00000000-0000-0000-0000-000000000000",
  "vendorState": "pendingActivation"
}
```

The response contains `Location: /vendors/{vendorId}`. `RegisteredAt`, Trading Preference and other committed details are deliberately excluded from this minimum response and are available through retrieval.

### 6.3.2 Retrieve Registered Vendor

```json
{
  "vendorId": "00000000-0000-0000-0000-000000000000",
  "registeredAt": "2026-08-25T10:30:00.0000000Z",
  "vendorState": "pendingActivation",
  "tradingPreference": "offline",
  "tradingName": "Hot Joe's Kitchen",
  "legalOperatorType": "limitedCompany",
  "legalOperatorName": "Hot Joe's Foods Limited",
  "companyRegistrationNumber": "AB123456",
  "tradingCharacteristics": {
    "tradingLocation": "kitchen",
    "openingHours": {
      "startTime": "17:00:00",
      "endTime": "02:00:00"
    },
    "serviceIncludesHotFood": true,
    "alcoholService": false
  },
  "primaryContact": {
    "contactName": "Jordan Smith",
    "contactEmail": "jordan@example.test",
    "contactTelephone": "+442079460123"
  },
  "canonicalAddressId": "address-example-id",
  "businessAddressSnapshot": {
    "addressLine1": "10 Example Street",
    "addressLine2": "Example Village",
    "addressLine3": null,
    "postTown": "LONDON",
    "postcode": "AB1 2CD",
    "county": null,
    "recipientOrOrganisationName": "Example Foods Ltd"
  },
  "foodRegistrationAuthority": "Example authority",
  "primaryTradingAuthority": null,
  "website": "https://example.test",
  "businessDescription": "Evening food delivery kitchen."
}
```

The response excludes the Aggregate, Domain and Integration Events, outbox state and persistence metadata.

## 6.4 Controlled Error Envelope

All expected failures use an API-owned client-safe envelope:

```json
{
  "code": "registrationValidationFailed",
  "message": "The Vendor could not be registered because supplied information is invalid.",
  "validationErrors": [
    {
      "field": "tradingName",
      "code": "lengthOutOfRange",
      "message": "Trading Name must contain between 1 and 160 characters."
    }
  ]
}
```

`validationErrors` is always present and is `null` when not applicable. Each validation entry contains the API JSON-path `field`, stable machine-readable `code` and client-safe `message`. The approved validation-entry code vocabulary is `required`, `invalidFormat`, `lengthOutOfRange`, `invalidValue`, `conditionallyRequired` and `prohibited`.

Example without field validation:

```json
{
  "code": "vendorNotFound",
  "message": "The requested Vendor was not found.",
  "validationErrors": null
}
```

Messages are explanatory text rather than identifiers and may be improved compatibly without changing the stable code. Every Application validation failure uses this one envelope and top-level code; `registrationDeclarationFailed` and `conditionalRuleFailed` are not contract outcomes.

## 6.5 HTTP Status and Error-Code Mapping

| Outcome | HTTP Status | Error Code |
|---|---:|---|
| Structurally malformed or unbindable request | `400 Bad Request` | `requestMalformed` |
| RequestValidationFailure, including request-field, Registration Declaration, conditional and cross-field errors | `400 Bad Request` | `registrationValidationFailed` |
| Invalid Address reference | `400 Bad Request` | `invalidAddressReference` |
| Invalid contextual Address result | `400 Bad Request` | `invalidAddressResult` |
| Aggregate invariant failure | `400 Bad Request` | `aggregateInvariantFailed` |
| Vendor not found | `404 Not Found` | `vendorNotFound` |
| Idempotency conflict | `409 Conflict` | `idempotencyConflict` |
| Address service temporarily unavailable | `503 Service Unavailable` | `addressServiceTemporarilyUnavailable` |
| Persistence or atomic recording failure | `503 Service Unavailable` | `persistenceOrAtomicRecordingFailed` |
| Unexpected unhandled failure | `500 Internal Server Error` | `unexpectedFailure` |

Epic 1 does not use `422 Unprocessable Content`. Unexpected exceptions are handled at one central API boundary, logged without duplication and returned without stack traces, database/provider details, internal class names or framework diagnostics.

Authentication and authorization are outside Epic 1; this contract therefore assigns no `401` or `403` outcome.

## 6.6 Headers

| Header | Direction | Requirement | Purpose |
|---|---|---|---|
| `Content-Type: application/json` | Register request and every response with a body | Required | JSON media type |
| `Accept: application/json` | Request | Standard negotiation when supplied | Requested response media type |
| `Location: /vendors/{vendorId}` | Successful Register response | Required | Identifies the registered Vendor retrieval resource |

No caller-supplied `Idempotency-Key` or equivalent header is accepted. No custom correlation header or error-body correlation member is normative in this contract; CON-035 retains that responsibility.


# 7. Traceability Matrix

| Contract Element | Source Artefact | Source Section / Model Element | Interpretation |
|---|---|---|---|
| Complete self-contained request | HJ-104; HJ-105; ADR-004 | HJ-104 §6; HJ-105 §5; ADR-004 §2.2 | Registration begins with one complete request and never depends on server-side Registration Session state |
| Registration fields and classifications | HJ-104 | §§2, 5.5 | Request table preserves required, optional, conditional, derived, Registered, Vendor Managed and transient classifications |
| Legal Operator rules | HJ-104; HJ-004 | HJ-104 §§5.1–5.3; HJ-004 §8 | Type controls Company Registration Number presence and canonicalisation |
| Trading Characteristics | HJ-003; HJ-004; HJ-104 | HJ-003 §3.4; HJ-004 §2.3; HJ-104 §§2, 5.1 | Four operating characteristics are persisted and remain distinct from legal identity |
| Registration Declarations | HJ-104; HJ-004 | HJ-104 §2 and §5.5; HJ-004 §§1.3, 8 | Mandatory transient inputs; never aggregate or event state |
| Address trust boundary | ADR-006; HJ-104; HJ-105 | ADR-006 §2; HJ-104 §§5.4–5.5; HJ-105 §§5–8 | Request carries only approved reference; Address Domain supplies authoritative values |
| Vendor creation | ADR-004; HJ-004; HJ-105 | ADR-004 §2.3; HJ-004 §§2, 3, 8; HJ-105 §§7, 13 | Successful registration creates one Vendor in PendingActivation and Offline |
| Register Vendor minimum outcome | HJ-004 | §1.7, Register Vendor Completion Semantics | Minimum synchronous business outcome identifies VendorId and Vendor State PendingActivation; further response fields are service-contract choices |
| Registration/Activation separation | ADR-004; ADR-007 | ADR-004 §2.3; ADR-007 §2 | Registration neither authorises trading nor decides Compliance or activation |
| Register Vendor identity, equivalence and conflict | HJ-010; HJ-012; HJ-104; HJ-105; ADR-008 | CON-013; HJ-104 §§5.3, 5.6; HJ-105 §9; ADR-008 §§2.2–2.4 | Identity is normalized Trading Name plus normalized Legal Operator Name plus Canonical Address Identifier; equivalent replay returns the original committed result; materially different information returns `IdempotencyConflict` without effects or update |
| Domain Event | HJ-004; ADR-008 | HJ-004 §7.1; ADR-008 §2.5 | Internal completed fact; no prescribed minimum payload |
| Integration Event translation | HJ-004; HJ-010; HJ-012; HJ-105; ADR-003; ADR-008 | HJ-004 §7.2; CON-019; HJ-105 §10; ADR-003 §2.2; ADR-008 §§2.5–2.6 | Vendor Application maps the completed business fact to the external contract before outbox persistence; Domain and Integration Event representations remain separate; relay reconstruction is prohibited |
| VendorRegistered Integration Event v1 | HJ-004; HJ-010; HJ-012; HJ-105; ADR-008 | HJ-004 §7.2; amended CON-020; HJ-105 §10.1; ADR-008 §§2.5–2.6 | The exact nested camel-case envelope and payload use contract-owned representations, deterministic UUID, UTC timestamp, time-only and enum formats, an independent BusinessAddress and explicit-null optional values; the UTF-8 JSON is serialized once and published unchanged |
| Failure atomicity | HJ-105; ADR-008 | HJ-105 §§8, 10, 12; ADR-008 §2.6 | Pre-commit failure creates no partial Vendor or publication work; dispatch failure does not undo registration |
| Retrieval actor and language | HJ-003 | §§3.20–3.22 | Vendor Administrator retrieves Registered Vendor Details by VendorId |
| Retrieval source and projection | HJ-004 | §1.7 | Persisted aggregate is authoritative; application maps it to a purpose-specific representation |
| Retrieval sequence and not-found outcome | HJ-105 | §14 | Repository lookup uses VendorId only and produces Found or controlled Not Found |
| Retrieval exclusions and side effects | HJ-004; HJ-105 | HJ-004 §1.7; HJ-105 §§14–15 | No search, mutation, event, Identity, Address, Compliance or read-model dependency |
| Approved Domain implementation architecture | HJ-012 | CON-001 to CON-005 | Aggregate, Value Object, Entity, Domain Event and Repository Approaches fulfil existing Domain and service guarantees without adding service behaviour |
| Reliable publication implementation architecture | HJ-012 | CON-017 | Transactional Outbox is the approved implementation Approach for the existing atomic durable-publication guarantee; relay and broker details remain unresolved elsewhere |
| Registered Vendor retrieval implementation architecture | HJ-012 | CON-027 | Query handler, Repository and response mapper fulfil the existing persisted-source, purpose-specific result and side-effect-free retrieval contract |
| Address application boundary and consumed result | HJ-012; ADR-006; HJ-104; HJ-105 | CON-006–CON-011; ADR-006 §2; HJ-104 §§2, 5.4; HJ-105 §§6, 12.2 | Application port and typed adapter resolve the permanent contextual reference, apply the approved positional source-line mapping, and distinguish semantic from retryable technical failure |
| HTTP adaptation and validation | HJ-010; HJ-012; HJ-011; HJ-104; HJ-105 | CON-023–CON-026 and CON-040; HJ-011 §2.3; HJ-104 §§5.2–5.3; HJ-105 §§4.1, 12.1 | Thin Minimal API adapters expose the two Application operations using the approved JSON, status and error-envelope rules; every Application validation failure uses one `RequestValidationFailure` mapped to `registrationValidationFailed` |
| Remaining architecture dependencies | HJ-010 | CON-018, CON-021 and CON-035 | Relay, broker-delivery and correlation remain separately governed and are not invented by this contract |
| Separate API models | HJ-005 | §§9.4, 16.1 | HTTP requests/results do not expose domain aggregate or persistence entities |
| Controlled errors and HTTP guidance | HJ-005 | §§12.4, 16.3 | Technical mapping uses safe error bodies and result-appropriate status codes |

# 8. Assumptions and Open Questions

| Classification | Item | Consequence / Required Decision |
|---|---|---|
| Confirmed | HJ-002 v2.0, HJ-003 v2.3, HJ-004 v2.8, HJ-104 v3.6 and HJ-105 v4.0 are Approved | They are authoritative inputs to this regenerated contract |
| Confirmed | Register Vendor creates one Vendor in PendingActivation and Offline | No draft Vendor or Registration lifecycle state is exposed |
| Confirmed | Register Vendor’s successful HTTP response contains only VendorId and Vendor State PendingActivation | First processing and equivalent replay return the original `201 Created` outcome; complete state is available through retrieval |
| Confirmed | Vendor uniqueness identity is trimmed, case-insensitive Trading Name plus trimmed, case-insensitive Legal Operator Name plus Canonical Address Identifier | Derive it after Address resolution; retain the original registered name display values |
| Confirmed | Equivalent registration information for an existing composite identity is a replay | Return the original committed successful result without repeating any business effect |
| Confirmed | Materially different registration information for an existing composite identity is `IdempotencyConflict` | Create or change no Vendor, event, publication work or Pending Activation Process; registration is not an update operation |
| Confirmed | Retrieve Registered Vendor is read-only and uses VendorId only | No search, cross-domain call or event belongs in the operation |
| Confirmed | `POST /vendors` and `GET /vendors/{vendorId}` are the two Epic 1 endpoints | CON-023/CON-024 approve thin Minimal API adaptation without search, collection, update or API versioning |
| Confirmed | Lower-camel JSON, deterministic identifiers/times, explicit-null response optionals and compatible unknown request members | CON-024 defines the complete wire conventions used in §6 |
| Confirmed | Successful equivalent replay returns the original `201` response and body | The transport preserves the permanent committed Application outcome |
| Confirmed | Successful registration returns `Location: /vendors/{vendorId}` | No caller idempotency or custom correlation header is introduced |
| Confirmed | Concurrent coordination, replay-outcome persistence and retention, transaction mechanics, database enforcement, and exact fingerprint encoding and storage representation | CON-014–CON-016 and CON-028 approve PostgreSQL concurrency authority, permanent outcome persistence, deterministic SHA-256 fingerprinting, one atomic transaction and explicit EF Core/PostgreSQL constraints |
| Confirmed | Business Address Snapshot schema and positional translation used by Vendor Registration and retrieval | Address source Line 1 maps to optional RecipientOrOrganisationName; source Lines 2–4 map respectively to AddressLine1–3; Post Town, Postcode and optional County map directly; no concatenation, compression, shifting or reordering occurs |
| Confirmed | Address Resolution reference semantics and failure taxonomy | Permanent, opaque, non-expiring, non-revocable, reusable and non-consuming; InvalidReference and InvalidAddressResult are semantic failures; temporary technical failure is caller-retryable with no in-process retry |
| Confirmed | `VendorRegistered` Integration Event v1 schema, translation, serialization and compatibility | CON-019 and amended CON-020 define the Application mapper, exact nested member structure, deterministic UUID, UTC timestamp, time-only and lower-camel enum formats, contract-owned representations, independent BusinessAddress, explicit-null optionals, UTF-8 camel-case serialization, compatible optional additions and new-version requirements for breaking changes |
| Missing Information | Outbox relay processing and broker delivery semantics | CON-018 and CON-021: resolve claim, retry, acknowledgement, duplicate delivery, ordering and poison-message treatment without reconstructing the stored event |
| Confirmed | Contact Email and Primary Contact Telephone validation and canonicalisation | CON-026 and HJ-104 v3.6 define the exact supported profiles, canonical stored values and structural-only boundary |
| Confirmed | Expected failure envelope, validation paths/codes and HTTP mappings | Amended CON-025, CON-026 and CON-040 define one `RequestValidationFailure` containing all independently detectable validation errors and one `registrationValidationFailed` API mapping in §6.4–§6.5; Epic 1 does not use `422` |
| Missing Information | Correlation propagation and ownership | CON-035 remains responsible; no custom header or error member is made normative here |

# 9. Review Checklist

- [x] Models the approved Vendor Registration workflow.
- [x] Includes every operation required by the current Epic 1 authoritative artefacts.
- [x] Preserves Vendor bounded-context ownership and persistence authority.
- [x] Uses Vendor State and other ubiquitous language consistently.
- [x] Preserves the Vendor lifecycle boundary and creates no draft Vendor.
- [x] Preserves Address Domain ownership and immutable Address snapshots.
- [x] Keeps Registration separate from Activation and trading authorisation.
- [x] Keeps Compliance decisions and evidence outside Vendor Registration.
- [x] Derives the approved composite Vendor uniqueness identity after authoritative Address resolution.
- [x] Preserves registered Trading Name and Legal Operator Name separately from their normalized comparison forms.
- [x] Returns the original committed result for semantically equivalent replay without repeating business effects.
- [x] Returns `IdempotencyConflict` for materially different information under the same composite identity without creating, updating or publishing additional business effects.
- [x] Produces events only for genuine successful Vendor creation.
- [x] Separates the internal Domain Event from the published Integration Event.
- [x] Requires atomic Vendor persistence and durable publication recording.
- [x] Defines Retrieve Registered Vendor as a side-effect-free query over persisted Vendor state.
- [x] Returns Registered Vendor Details rather than exposing the aggregate.
- [x] Introduces no unsupported search, Identity dependency, lifecycle state or read-model infrastructure.
- [x] Separates the transport-independent business contract from the approved Epic 1 HTTP adaptation.
- [x] Records missing information and ambiguities instead of silently resolving them.
- [x] Reconciles the HJ-010/HJ-012 v2.0 Approved baseline, including the amended unified validation-failure treatment for CON-025, CON-026 and CON-040, without deciding unresolved relay, broker-delivery or correlation mechanics.
- [x] Keeps approved implementation patterns from becoming unnecessary service-contract requirements.
- [x] Keeps the remaining unresolved architectural choices explicit and traceable to HJ-010 concerns.

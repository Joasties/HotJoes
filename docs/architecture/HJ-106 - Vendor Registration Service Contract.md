# HJ-106 - Vendor Registration Service Contract

| Property | Value |
|---|---|
| **Document ID** | HJ-106 |
| **Document Title** | Vendor Registration Service Contract |
| **Version** | 2.2 |
| **Status** | Approved |
| **Classification** | Service Contract |
| **Owner** | Project Architecture |
| **Last Updated** | 22 September 2026 |

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
| 1.9 | 28 August 2026 | Regenerated using PR-002 from HJ-105 v4.1, ADR-003 v1.3, ADR-007 v1.1, ADR-008 v1.5 and the synchronized HJ-010/HJ-012 v2.1 Approved baseline. Defined the approved PostgreSQL relay, at-least-once RabbitMQ delivery, durable idempotent Compliance receipt, retry, dead-letter and trace-context semantics; removed the resolved relay, broker-delivery and correlation deferrals. |
| 2.0 | 14 September 2026 | Regenerated from the approved CR-073 baseline. Replaced the single Opening Hours interval throughout the business, HTTP, retrieval and unreleased VendorRegistered v1 contracts with the complete seven-day Weekly Opening Hours representation and aligned semantic fingerprint and migration behaviour. |
| 2.1 | 19 September 2026 | Regenerated through PR-002 from the approved CON-047 authoritative cohort. Added the separate pre-registration Required Licence Types determination operation, complete immutable Compliance Determination contract, Address and Compliance collaborations, controlled failures and proposed HTTP representation; preserved RegisterVendor independence and corrected duplicated section headings and affected traceability references. |
| 2.2 | 22 September 2026 | Regenerated through PR-002 from the approved CON-046 authoritative cohort. Added the separate post-registration Join Community business and HTTP contracts, Vendor verification, immutable Community Participation, replay, conflict, atomic publication, event and consumer boundaries while preserving Vendor Registration independence. |

## Related Documents

| Document ID | Title | Status | Relevance |
|---|---|---|---|
| PR-002 | Infer Service Contract from Approved Domain Artefacts | Governing prompt | Governs the inference method and document structure |
| HJ-002 | Architectural Principles | Approved | Capability ownership, contracts, events and architectural simplicity |
| HJ-003 v2.6 | Ubiquitous Language Guide | Approved | Authoritative Vendor, Compliance and Community terminology |
| HJ-004 v3.1 | Vendor Domain Models | Approved | Vendor aggregate, lifecycle and separate Community bounded-context boundary |
| HJ-005 | Coding Standards | Approved | API boundary, validation, error and HTTP conventions |
| HJ-010 v2.19 | Current Application Architectural Concerns | Approved | Records approved CON-046 and CON-047 boundaries |
| HJ-011 v2.18 | Epic 1 Vendor Registration Implementation Scope | Approved | Defines the Community and existing Vendor/Compliance delivery boundaries |
| HJ-012 v2.14 | Established Application Architecture Patterns | Approved | Defines the approved CON-046 and CON-047 application and adapter patterns |
| HJ-104 v3.9 | Vendor Registration Fields Matrix | Approved | Authoritative registration and separate post-registration Community information contracts |
| HJ-105 v4.4 | Vendor Registration Sequence Diagram | Approved | Authoritative Vendor, Community, publication and consumer sequences |
| ADR-002 v1.2 | Business Capabilities and Bounded Contexts | Accepted | Community and Vendor ownership boundaries |
| ADR-003 v1.4 | Event-Driven Collaboration | Accepted | Community event publication and consumer semantics |
| ADR-004 | Vendor Lifecycle Begins After Successful Registration | Accepted | Registration Session, service and Vendor-existence boundaries |
| ADR-005 | Registered Information vs Vendor Managed Information | Accepted | Information classification and editability |
| ADR-006 | Address Domain Ownership and Business Address Snapshots | Accepted | Address trust boundary and snapshot invariant |
| ADR-007 v1.2 | Vendor Compliance as a Separate Bounded Context | Accepted | Compliance separation, synchronous pre-registration determination and asynchronous post-registration processing |
| ADR-008 v1.6 | Idempotent Operations and Reliable Event Publication | Accepted | Vendor and Community idempotency, atomic staging, delivery and retention |
| ADR-013 v1.1 | Epic 1 Runtime and Deployment Composition | Accepted | Trusted Community Edge route and runtime responsibilities |
| CR-040 | Align HJ-106 with the Approved Architecture Baseline | Applied | Records the architecture-impact assessment and authorises this traceability-only revision |
| CR-073 | Define Weekly Opening Hours in the Vendor Domain Model | Approved | Authorises the complete seven-day schedule and unreleased v1 contract correction |
| CR-076 | Amend ADR-007 for Pre-registration Compliance Determination | Approved | Records the approved amendment applied through accepted ADR-007 v1.2 |

# 1. Purpose

This document defines the business service contract for the Epic 1 Vendor Registration vertical slice. It translates the approved domain model into reviewable operations without redesigning the Vendor Domain.

The contract defines:

- registration of one Vendor from a complete, self-contained request;
- determination of Required Licence Types before registration submission;
- retrieval of one registered Vendor by VendorId;
- recording affirmative Community Participation after successful registration;
- request and result information;
- validation and aggregate invariants;
- lifecycle effects and side-effect constraints;
- Address Domain collaboration;
- Domain and Integration Events;
- idempotency and reliable-publication behaviour;
- controlled business failures; and
- the approved Epic 1 HTTP representation.

Part A, Sections 1–5, is the inferred business contract. Part B, Section 6, preserves the approved Register Vendor and retrieval HTTP representations derived from CON-023–CON-026 and defines the determination and Community Participation representations inferred from approved CON-047 and CON-046. API representations adapt and do not redefine the transport-independent Application contracts.

# 2. Source Artefacts

| Artefact | Why Used | Authority Provided |
|---|---|---|
| HJ-002 | Constrains interpretation of capability boundaries and published contracts | Vendor owns Vendor behaviour and persistence; contracts are explicit; events represent completed facts; prefer simplicity |
| HJ-003 v2.6 | Supplies authoritative language | Vendor, Compliance and Community terms, including Community Participation and Contact Preference |
| HJ-004 v3.1 | Defines the business model exposed by this contract | Vendor lifecycle plus the separate immutable Community Participation record and ownership boundary |
| HJ-005 | Separates business contract from implementation conventions | Separate API models, controlled errors, layered validation, status-code guidance and safe response rules |
| HJ-104 v3.9 | Is the authoritative information contract | Registration and determination information plus the separate post-registration Community request, result and exclusions |
| HJ-105 v4.4 | Defines observable interaction behaviour | Vendor, determination and Community orchestration, publication, durable receipt and retrieval |
| ADR-002 v1.2 | Prevents ownership leakage | Vendor, Compliance and Community retain their own behaviour and data |
| ADR-003 v1.4 | Governs asynchronous collaboration | Completed Vendor and Community facts cross boundaries through explicit immutable Integration Events using approved at-least-once delivery and durable idempotent receipt |
| ADR-004 | Defines when a Vendor exists | No Vendor exists before successful registration; Registration Session remains outside the Vendor service boundary |
| ADR-005 | Governs information classification | Registered Information and Vendor Managed Information have distinct post-registration lifecycles |
| ADR-006 v1.3 | Defines Address authority | Address Domain supplies canonical identity, immutable snapshot and applicable regulatory authorities |
| ADR-007 v1.2 | Preserves compliance separation | Compliance owns pre-registration applicability policy while post-registration Compliance Requirement processing remains separate and asynchronous |
| ADR-008 v1.6 | Defines mandatory reliability behaviour | Vendor and Community idempotency, atomic staging, immutable at-least-once publication, retry, recovery, dead-letter and retention behaviour |
| ADR-013 v1.1 | Defines runtime composition | Trusted Edge exposure and independently executable Community consumer responsibilities |

No contradictory business rule was found among the supplied source artefacts. HJ-104 v3.9 specializes the registration, determination and separate post-registration Community information rules. HJ-004 v3.1 and HJ-105 v4.4 preserve Vendor Registration while placing Community Participation, its persistence and its publication in the Community boundary. HJ-005 supplies compatible engineering constraints; CON-023–CON-026 provide the existing Epic 1 HTTP specialization.

HJ-010 v2.19 and HJ-012 v2.14 are the controlled architectural governance baseline for this contract. CON-046 establishes Community ownership, the post-registration Join Community operation, minimal Vendor verification, immutable persistence, replay/conflict semantics, atomic event staging, asynchronous stub processing and explicit communication exclusions. CON-047 establishes the dedicated Vendor Application determination operation and Compliance-owned policy. CON-006–CON-011 continue to govern Address resolution. CON-013–CON-020 and CON-028 govern registration and publication. CON-023–CON-026 and amended CON-040 govern thin HTTP adaptation and controlled validation mapping. CON-018, CON-021, CON-022, CON-029 and CON-035 govern relay, broker delivery, durable receipts, migration and trace context. CON-036, CON-037 and CON-039 govern operational health and enforcement.

# 3. Business Operation Summary

| Operation | Business Purpose | Owning Bounded Context | Preconditions | Successful Outcome |
|---|---|---|---|---|
| `DetermineRequiredLicenceTypes` | Indicate every Required Licence Type applicable to the current registration draft before submission | Compliance owns policy and result meaning; Vendor Application exposes and orchestrates the operation | Complete valid controlling inputs; approved Address Resolution reference resolves to authoritative Address and authority information; active rule set supports the coverage | Complete immutable Compliance Determination containing the active Rule Set Version and exactly five canonically ordered explicit items |
| `RegisterVendor` | Create a Vendor from complete and valid registration information | Vendor | One complete request; mandatory declarations accepted; approved Address Resolution reference resolved; all field rules and aggregate invariants satisfied; composite Vendor identity and replay outcome established | One Vendor is committed in `PendingActivation` and `Offline`, or an equivalent replay returns its original committed result; `VendorRegistered` publication work exists exactly once |
| `RetrieveRegisteredVendor` | Return the persisted details established through successful Vendor Registration | Vendor | VendorId supplied; an existing Vendor is identified | Registered Vendor Details is returned from persisted Vendor state without mutation, events or cross-domain collaboration |
| `JoinCommunity` | Record affirmative Community Participation and one Contact Preference after definitive Vendor Registration success | Community | Valid VendorId and Email, SMS or WhatsApp preference; Vendor verification confirms successful registration | First request records immutable participation and publication work; equivalent replay returns the original result |

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

`RegisterVendor` does not require, accept or verify a prior Compliance Determination, Rule Set Version, determination token or Required Licence Types result. The Web journey's fresh pre-submission determination is a caller-flow requirement, not a precondition of this transport-independent operation.

## 4.3 Request Information

| Field | Business Meaning | Data Owner | Required Status | Source Artefact | Validation | Persisted or Transient | Registered or Vendor Managed | Notes |
|---|---|---|---|---|---|---|---|---|
| Trading Name | Public name under which the Vendor trades | Vendor | Required | HJ-104 §§2, 5.2, 5.3, 5.6 | 1–160 characters; trimmed, case-insensitive comparison contributes to the composite Vendor identity | Persisted | Registered Information | The registered display value is retained and is not replaced by its comparison form |
| Legal Operator Name | Registered legal name of the operator | Vendor | Required | HJ-003 §3.11; HJ-104 §§2, 5.2, 5.3, 5.6 | 1–160 characters; trimmed, case-insensitive comparison contributes to the composite Vendor identity | Persisted | Registered Information | “Company Name” is historical terminology only; the registered display value is retained |
| Legal Operator Type | Classification of the responsible legal person or organisation | Vendor | Required | HJ-104 §§2, 5.1, 5.2 | Controlled value: Sole Trader, General Partnership, Limited Company, Limited Liability Partnership, Charitable Community Group or Charitable Incorporated Organisation | Persisted | Registered Information | Drives Company Registration Number applicability |
| Company Registration Number | Government-issued registration identifier where applicable | Vendor | Conditional | HJ-104 §§2, 5.1–5.3 | Required only for Limited Company, Limited Liability Partnership or Charitable Incorporated Organisation; otherwise absent; pattern `^(?:[A-Za-z]{2})?\d{6,8}$`; alphabetic prefix canonicalised to uppercase; format only | Persisted when applicable | Registered Information | No Companies House existence verification in Epic 1 |
| Trading Location | Controlled operating classification | Vendor | Required | HJ-104 §§2, 5.1, 5.2 | Restaurant, Stall or Kitchen | Persisted | Registered Information | Part of Trading Characteristics |
| Weekly Opening Hours | Complete registered operating schedule | Vendor | Required | HJ-003 §3.4.2; HJ-104 §§2, 5.2 | Exactly seven Daily Opening Hours entries, one per unique Trading Day Monday–Sunday | Persisted | Registered Information | Canonical order is Monday through Sunday |
| Daily Opening Hours state | Whether one Trading Day is Closed, Open All Day or uses a timed interval | Vendor | Required for each day | HJ-003 §3.4.2; HJ-104 §§2, 5.2 | Exactly one state; Closed and Open All Day are mutually exclusive | Persisted | Registered Information | “Everyday” is not a contract state |
| Daily Opening Hours – Start Time / End Time | Timed operating interval for one Trading Day | Vendor | Conditional | HJ-003 §3.4.2; HJ-104 §§2, 5.2 | Both required and unequal only for a timed interval; absent for Closed or Open All Day; End Time may be earlier for overnight operation | Persisted when applicable | Registered Information | Equal times must use Open All Day instead |
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
| Semantic Registration Fingerprint | Derived deterministically from all materially relevant registration information after approved canonicalisation | New registrations use representation version 2, including the canonical Monday-to-Sunday Weekly Opening Hours states and times, hashed using SHA-256. Migrated legacy intervals remain replay-compatible with seven equivalent timed days. Transient declarations, the opaque Address Resolution reference, server-generated values and technical metadata are excluded. |
| Business Address Snapshot | Supplied exclusively by the Address Domain from the approved Address Resolution | Persisted immutably using the complete seven-field schema defined below |
| Food Registration Authority | Derived and supplied by the Address Domain from the approved Business Address or mobile-unit base address | Persisted as Registered Information |
| Primary Trading Authority | Derived and supplied by the Address Domain | Persisted only when Trading Location is `Stall`; absent otherwise |
| Initial Vendor State | Established by successful Vendor creation | `PendingActivation` |
| Initial Trading Preference | Established by successful Vendor creation | `Offline` |
| Pre-registration Compliance Determination | Produced only by the separate `DetermineRequiredLicenceTypes` operation | Transient Registration Session review state; excluded from RegisterVendor, Vendor persistence, semantic fingerprint, replay comparison, retrieval and events |
| Compliance Requirements | Determined later by the Compliance capability from published information | Not returned, stored or decided by Register Vendor |

## 4.5 Business Validation

The Vendor Application authoritatively validates the complete raw `RegisterVendor` intent regardless of client-side or API convenience validation. It validates all HJ-104 field, Registration Declaration, conditional and cross-field rules before Address resolution, uniqueness identity or fingerprint determination, Aggregate creation or persistence. Any Application validation failure returns one immutable `RequestValidationFailure` containing every independently detectable validation error. Registration Declaration and conditional-rule errors are entries in that result rather than separate top-level outcomes.

Successful validation supplies canonical values to every downstream stage. These include Weekly Opening Hours ordered Monday through Sunday with exactly one valid state per day, uppercase Company Registration Number, unmodified registered name display values alongside trimmed case-insensitive comparison forms, Contact Email with surrounding whitespace removed and domain lowercased while preserving local-part case, and Primary Contact Telephone stored in canonical `+44` form. No downstream stage may return to uncanonicalized client input. The Vendor Domain remains the final defensive owner of Aggregate and Value Object invariants.

The Contact Email profile permits ASCII letters, digits and ``. ! # $ % & ' * + - / = ? ^ _ ` { | } ~`` in the local part. A dot is prohibited first, last or consecutively. The domain contains at least two dot-separated labels; each label contains 1–63 ASCII letters, digits or hyphens and does not begin or end with a hyphen. Display names, comments, quoted local parts, domain literals and internationalized Unicode addresses are prohibited. Validation establishes plausible structure only; allocation, deliverability and ownership verification are outside Epic 1.

For Primary Contact Telephone, presentation spaces, hyphens and parentheses are removed while a single permitted leading `+` is retained. Bare `44` is rejected. The normalized value shall match `^(?:(?:\+44|0)7\d{9}|(?:\+44|0)(?:1|2|3|5|8|9)\d{8,9})$`. A domestic value is converted by removing its leading `0` and prepending `+44`; an accepted `+44` value is retained. Specialist, short-code and legacy ranges outside this profile are rejected. Validation does not establish allocation, activity, reachability or ownership.

Creation invariants are:

- all mandatory registration information is complete and valid;
- Company Registration Number is present if and only if Legal Operator Type is Limited Company, Limited Liability Partnership or Charitable Incorporated Organisation;
- Primary Trading Authority is present if and only if Trading Location is `Stall`;
- Canonical Address Identifier and immutable Business Address Snapshot originate together from the Address Domain;
- caller-authored Address snapshots or authority values cannot replace Address-owned information;
- Weekly Opening Hours contain exactly one Daily Opening Hours entry for each unique Trading Day Monday–Sunday and every entry is exactly Closed, Open All Day or a valid timed interval;
- every Registration Declaration is explicitly accepted;
- one Vendor represents one trading location;
- final Vendor identity evaluation occurs only after authoritative Address resolution supplies the Canonical Address Identifier;
- Trading Name and Legal Operator Name are trimmed and compared without regard to case for identity purposes without changing their registered display values;
- equivalent registration information for an existing composite identity returns the original committed successful result without repeating any business effect;
- materially different registration information for the same composite identity returns `IdempotencyConflict` and never updates the Vendor;
- a new Vendor starts in `PendingActivation` and `Offline`; and
- incomplete or invalid registration creates no Vendor, completed business fact, event or publication work.

Trading Characteristics comprise Trading Location, Weekly Opening Hours, Service Includes Hot Food and Alcohol Service. They describe the operation; they do not replace Legal Operator Type, which describes legal identity and registration obligations.

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

Pending Activation and Integration Event dispatch continue asynchronously and do not alter the synchronous successful outcome. Successful registration requires durable outbox staging, not immediate RabbitMQ delivery or downstream receipt.

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
      "weeklyOpeningHours": {
      "days": [
        { "day": "monday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "tuesday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "wednesday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "thursday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "friday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "saturday", "isClosed": false, "isOpenAllDay": true, "startTime": null, "endTime": null },
        { "day": "sunday", "isClosed": true, "isOpenAllDay": false, "startTime": null, "endTime": null }
      ]
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
- `tradingCharacteristics.weeklyOpeningHours.days` uses the nested structure shown and contains exactly seven entries ordered Monday through Sunday;
- every daily entry contains `day`, `isClosed`, `isOpenAllDay`, `startTime` and `endTime`, with times explicitly `null` for Closed and Open All Day;
- the Integration Event contract owns every published representation and exposes no Vendor Domain Aggregate, Value Object or enum type; and
- every optional member remains present with an explicit JSON `null` when absent.

Optional values are represented explicitly as `null`. Because v1 has not been released or consumed externally, CR-073 deliberately revises its Weekly Opening Hours representation in place. Compatible optional fields may be added within the corrected v1 and consumers must tolerate unknown fields. After release, removal, renaming, type changes or meaning changes require a new event version. Dispatch publishes the stored event unchanged; retries preserve the original EventId, EventVersion and serialized event. Relay-time reconstruction from current Vendor state is prohibited.

Vendor persistence and durable recording of the serialized Integration Event must commit atomically. Dispatch failure leaves the registered Vendor unchanged and the durable publication record available for retry. A publication retry does not repeat registration or create another business fact or event record.

After commit, a dedicated Vendor relay polls PostgreSQL in bounded batches and claims eligible outbox records using leased `FOR UPDATE SKIP LOCKED` semantics. It publishes the stored immutable bytes through durable RabbitMQ topology with publisher confirms and marks a record published only after broker confirmation. Expired claims are recoverable. Failed attempts use bounded exponential backoff; exhausted work becomes durable `Stalled` work requiring explicit administrative requeue, and publication records are not deleted.

Delivery is at least once. `EventId` is the stable message identity; exactly-once delivery and ordering are not claimed. The Compliance consumer stub durably records EventId, EventType, EventVersion, receipt time and a hash of the serialized bytes before acknowledgement. An equivalent duplicate is acknowledged without another receipt. The same EventId with different bytes is a contract-integrity failure and is dead-lettered. Retry exhaustion or a non-retryable delivery also routes the unchanged message to a durable dead-letter queue.

W3C `traceparent` and optional `tracestate` are persisted as outbox metadata and propagated as RabbitMQ headers. They do not alter the immutable Integration Event JSON. Their absence does not invalidate the business event.

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
| Integration Event dispatch failure | Durable publication exists but relay claim, broker publication or publisher confirmation fails | Registration remains successful; downstream notification is delayed | Caller retry not required | Do not register again; the relay retries with bounded backoff and preserves the original event |
| Publication retry exhausted | The relay exhausts its configured bounded attempts | Registration remains successful but publication work requires intervention | No automatic retry while stalled | Administratively inspect and explicitly requeue the durable `Stalled` record |
| Consumer processing or receipt failure | The Compliance stub cannot durably record receipt before acknowledgement | Delivery has not been safely accepted by the consumer | Yes, through broker redelivery or bounded consumer retry | Allow redelivery; do not invoke RegisterVendor again |
| Poison or contract-integrity message | Delivery is non-retryable, retries are exhausted, or an existing EventId has different serialized bytes | The message cannot be safely processed as the original fact | No automatic processing after dead-lettering | Inspect the durable dead-letter record; preserve EventId, EventVersion and payload |
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
- Trading Characteristics: Trading Location, Weekly Opening Hours, Service Includes Hot Food and Alcohol Service;
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

## 4.13 Determine Required Licence Types Business Contract

### 4.13.1 Business Intent and Ownership

`DetermineRequiredLicenceTypes` supplies the Step 4 registration journey with a complete indication of which controlled Required Licence Types apply to the current draft. It is a separate, synchronous and side-effect-free operation. Compliance owns the applicability policy, determination meaning, canonical ordering and active Rule Set Version. Vendor Application exposes the Vendor-facing operation and orchestrates validation, Address resolution and invocation of its consumed Compliance determination port; it owns no applicability rule.

Epic 1 fulfils the port through one in-process, deterministic, versioned, non-persistent Compliance stub adapter. A future Compliance implementation may replace that outer adapter without changing this business contract or relocating policy into Vendor, Address or the Web client.

### 4.13.2 Request Information

| Field | Business Meaning | Required Status | Validation and Ownership | Persistence |
|---|---|---|---|---|
| Legal Operator Type | Legal classification of the draft operator | Required | One approved controlled value; supplied by the caller and validated by Vendor Application | Transient |
| Trading Location | Operating classification used by the active policy | Required | Restaurant, Stall or Kitchen; supplied by the caller and validated by Vendor Application | Transient |
| Weekly Opening Hours | Complete proposed Monday-to-Sunday operating schedule | Required | Exactly seven unique Daily Opening Hours entries satisfying the approved Closed, Open All Day or timed-interval invariants | Transient |
| Service Includes Hot Food | Whether the proposed service includes heated food or drink | Required | Required boolean with presence distinguished from its value | Transient |
| Alcohol Service | Whether the proposed service includes alcohol | Required | Required boolean with presence distinguished from its value | Transient |
| Address Resolution Reference | Opaque reference to the selected authoritative Address result | Required | Resolved by Vendor Application through the approved Address boundary; the caller cannot substitute Address or authority values | Transient |

Trading Name, Legal Operator Name, Company Registration Number, Primary Contact, Website, Business Description, Registration Declarations, VendorId, Vendor State and every determination retained by the Registration Session are excluded from this request.

### 4.13.3 Derived Authoritative Input

After request validation, Vendor Application resolves the Address Resolution Reference using the submitted Trading Location. Address supplies the approved Business Address, Food Registration Authority and Primary Trading Authority when Trading Location is Stall. Vendor Application passes those authoritative values, not the opaque reference, into the Compliance determination port together with the other validated controlling inputs.

The active Rule Set Version is selected by Compliance and is not caller-selectable. The same canonical input under the same Rule Set Version produces the same result.

### 4.13.4 Applicability Rules

The active Epic 1 policy evaluates every controlled Required Licence Type:

| Required Licence Type | `IsRequired` rule |
|---|---|
| Food Business Registration | Always `true` |
| Street Trading Licence | `true` exactly when Trading Location is Stall and the authoritative Address result supplies the required Primary Trading Authority context |
| Late Night Refreshment Licence | `true` exactly when Service Includes Hot Food is `true` and any Daily Opening Hours interval overlaps any portion of 23:00 inclusive to 05:00 exclusive; Closed does not overlap, Open All Day does overlap and timed intervals are evaluated across midnight |
| Premises Licence | Equal to Alcohol Service |
| Personal Licence Holder | Equal to Alcohol Service |

Unsupported policy or jurisdictional coverage fails closed rather than returning a partial or assumed determination.

### 4.13.5 Successful Outcome

Success returns one complete immutable Compliance Determination containing:

- the active Rule Set Version; and
- exactly one Compliance Determination Item for every controlled Required Licence Type, in this canonical order: Food Business Registration, Street Trading Licence, Late Night Refreshment Licence, Premises Licence and Personal Licence Holder.

Each item identifies its Required Licence Type and carries an explicit `IsRequired` boolean, including explicit `false` values. The result is complete and duplicate-free. It is an indication of future Compliance requirements, not Licence Details, evidence, a Compliance Requirement or proof that any licence exists, is authentic, current, valid or satisfied.

### 4.13.6 Result Lifecycle and Submission Relationship

The Web-client Registration Session may retain the Compliance Determination, Rule Set Version and a fingerprint of all controlling inputs solely for review. Any change to Legal Operator Type, Trading Location, Weekly Opening Hours, Service Includes Hot Food, Alcohol Service or selected Address Resolution Reference invalidates all three retained values.

Immediately before Web submission, the client invokes this operation again. An unchanged item set and Rule Set Version permits the separate `RegisterVendor` call. A changed item set or Rule Set Version requires renewed user review before registration submission. A controlled determination failure retains the registration draft and prevents only that Web submission attempt.

The result, Rule Set Version, items, client fingerprint and Required Licence Types are not inputs to `RegisterVendor`; are not recalculated by `RegisterVendor`; and are not Vendor state, persistence, semantic-fingerprint, replay, registered-Vendor retrieval, Domain Event or `VendorRegistered` content. Direct `RegisterVendor` invocation remains independent of Compliance availability and does not require a prior determination token.

### 4.13.7 Failures and Side Effects

| Failure | Trigger | Business Meaning | Retryable | Expected Caller Behaviour |
|---|---|---|---|---|
| DeterminationRequestValidationFailure | One or more controlling inputs are missing or invalid | No determination can be made from the supplied draft; every independently detectable validation error is returned | Yes, after correction | Correct all reported fields and retry |
| InvalidReference | Address Resolution Reference is unknown or fabricated | No authoritative Address selection can be established | Yes, after reselection | Select a complete Address result and retry with its issued reference |
| InvalidAddressResult | The known reference cannot satisfy the declared Trading Location or required authority context | The active policy lacks valid authoritative Address input | Yes, after correction or reselection | Correct Trading Location or select another valid Address result |
| UnsupportedDetermination | The active rule set does not support the authoritative jurisdiction or coverage | Compliance cannot safely state a complete result and therefore fails closed | No with unchanged inputs and rule set | Do not progress; retain the draft and obtain supported Address or policy coverage |
| ComplianceDeterminationTemporarilyUnavailable | The determination boundary cannot complete because of a temporary technical failure | No complete result is available for this attempt | Yes, as an explicit caller-controlled retry | Retain the draft and retry later; do not submit RegisterVendor from this Web flow |

Every success and failure is side-effect-free. The operation creates no Vendor, Compliance aggregate, Compliance Requirement, Licence Details, evidence, Domain Event, Integration Event, persistence record, outbox work or external regulatory call.

## 4.14 Join Community Business Contract

### 4.14.1 Intent and Ownership

`JoinCommunity` records the explicit affirmative choice of a successfully registered Vendor to participate in the HotJoes community. Community Application owns the operation, result, immutable participation, persistence and event. Vendor remains authoritative for successful registration and Primary Contact information.

### 4.14.2 Request

The request contains exactly:

- `VendorId` — required UUID identifying the successfully registered Vendor; and
- `ContactPreference` — required controlled value Email, SMS or WhatsApp.

Invocation implies affirmative participation. The request contains no participation Boolean, Primary Contact details, Communication Consent, recipient resolution or delivery instruction.

### 4.14.3 Verification and Success

Before creation, Community verifies VendorId through the minimal Vendor-owned verification capability. It returns only whether the Vendor was successfully registered and exposes no aggregate, Registered Vendor Details or Primary Contact information.

The first successful request atomically commits one immutable Community Participation record and one immutable outbox item. The record contains server-generated CommunityParticipationId, VendorId, Contact Preference, original JoinedAt and required technical concurrency metadata. The authoritative success returns CommunityParticipationId, VendorId, Contact Preference and JoinedAt.

VendorId is unique in Community persistence. The same preference returns `CommunityParticipationAlreadyRecorded` with the original result. A different preference returns `CommunityParticipationConflict`. Concurrent requests produce exactly one record and at most one logical event.

### 4.14.4 Outcomes

The complete transport-independent outcome set is:

| Outcome | Trigger | Business Meaning | Retryable | Expected Caller Behaviour |
|---|---|---|---|---|
| `CommunityParticipationRecorded` | First valid request commits | Participation, original success and publication work are authoritative | Not applicable | Present definitive success |
| `CommunityParticipationAlreadyRecorded` | Same VendorId and Contact Preference already committed | Original authoritative success is replayed without another effect | Not applicable | Treat as the same definitive success |
| `RequestValidationFailure` | VendorId or Contact Preference is invalid | No operation was attempted with valid information | Yes, after correction | Correct the request and resubmit |
| `VendorNotFound` | Vendor cannot confirm successful registration | Community cannot associate participation with a registered Vendor | No for the same unknown identifier | Use a successfully registered VendorId |
| `CommunityParticipationConflict` | The Vendor already joined with a different Contact Preference | Epic 1 cannot amend the immutable participation | No | Do not retry as an amendment |
| `VendorVerificationUnavailable` | Authoritative Vendor verification cannot presently complete | Eligibility is unknown for this attempt | Yes | Retain the selection and retry safely |
| `CommunityPersistenceUnavailable` | The Community transaction cannot presently complete | No definitive Community result is available for this attempt | Yes | Retain the selection and retry safely |

Every failure creates no Community Participation, outbox work or event. Failure before commit leaves neither durable; committed success remains authoritative when publication or consumer processing is delayed. Recorded and Already Recorded both carry the original CommunityParticipationId, VendorId, Contact Preference and JoinedAt.

### 4.14.5 Integration Event and Processing

First commit creates one `CommunityParticipationRecorded` Integration Event v1. Its lower-camel-case envelope contains `eventId`, `eventType` fixed as `CommunityParticipationRecorded`, `eventVersion` fixed as `1`, `occurredAt` and `payload`. The payload contains exactly `communityParticipationId`, `vendorId`, `joinedAt` and `contactPreference`. UUIDs use lowercase canonical `D` format, timestamps use UTC invariant round-trip format and Contact Preference is `email`, `sms` or `whatsApp`. It contains no Primary Contact, Vendor Registration, consent, recipient or delivery information.

The serialize-once event is published unchanged through the outbox and RabbitMQ pattern. The independently executable Community consumer uses EventId for durable idempotent receipt. Matching redelivery creates no further effect; EventId reuse with different immutable content is an integrity failure. The deterministic Community stub sends no communication.

### 4.14.6 Lifecycle and Exclusions

The record and original result are retained for at least as long as the associated Vendor exists. Epic 1 provides no expiry, deletion, amendment or withdrawal. Absence means not joined and is not a persisted refusal. Contact Preference is personal-data-related metadata but contains no contact destination and grants no consent or authority to communicate.

# 5. Information Outside Scope

The following are intentionally outside this service contract:

- Registration Session creation, retrieval, reconciliation, storage, expiry or disposal within the Vendor service;
- incomplete or resumable registration drafts;
- authentication, authorisation, Identity account creation and caller-to-Vendor ownership checks;
- Activation and activation decisions;
- Licence Details, regulatory evidence, licence validation, Compliance Requirement creation or lifecycle, ongoing monitoring, activation decisions and the completed Pending Activation Process;
- Operational Availability composition;
- post-registration amendment of Registered Information;
- future Vendor Management operations for Website, Business Description or Primary Contact;
- Community Participation amendment, withdrawal, deletion or expiry;
- Communication Consent, recipient resolution and Email, SMS or WhatsApp delivery;
- Vendor search, filtering, paging, multiple-Vendor retrieval or lookup by names;
- Address search UI and live Address re-resolution during registered Vendor retrieval;
- multiple premises, Branches, Menu, Ordering, Payment and Delivery behaviour;
- dedicated retrieval projections, eventual consistency and independently optimised read models;
- implementation code, persistence mappings, controllers, handlers and OpenAPI definitions; and
- concrete transport, storage, outbox, broker and serialization technologies.

# 6. Approved HTTP Representation

This section preserves the approved Register Vendor and retrieval HTTP adaptation under CON-023–CON-026 and defines the minimal technical representations for `DetermineRequiredLicenceTypes` and `JoinCommunity` inferred from approved CON-047 and CON-046. These routes, JSON members and HTTP mappings are service-contract conventions; they do not change Compliance, Community or Vendor ownership. API adapters own HTTP binding, structural validation, API-to-Application mapping, Application-result-to-HTTP mapping, cancellation-token forwarding, response headers and centralized unexpected-exception handling. They contain no Domain, Compliance or Community rules, Address resolution, persistence query, transaction, event, outbox or broker behaviour.

## 6.1 Endpoints and JSON Conventions

| Business Operation | Method | Route | Success |
|---|---|---|---|
| Determine Required Licence Types | `POST` | `/vendor-registration/required-licence-types` | `200 OK` with the complete Compliance Determination |
| Register Vendor | `POST` | `/vendors` | `201 Created` with the original committed registration outcome and `Location` header |
| Retrieve Registered Vendor | `GET` | `/vendors/{vendorId}` | `200 OK` with Registered Vendor Details |
| Join Community | `POST` | `/community-participations` | `201 Created` with the original Community Participation and stable `Location` |

The determination uses `POST` because it evaluates a structured request body; it remains safe and side-effect-free and creates no resource. Join Community uses `POST` because first success creates Community state; equivalent replay preserves the original `201 Created` semantics. No Vendor collection, search, filtering, paging, update or API-versioned endpoint is included in Epic 1.

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

## 6.2 Request Representations

### 6.2.1 Register Vendor Request

`addressResolutionReference` is the only request-side link to Address-owned information. The request excludes Canonical Address Identifier, Business Address Snapshot, regulatory authorities, Vendor state, server-generated identifiers, composite identity, semantic fingerprint, persistence/outbox information and Registration Session state.

```json
{
  "tradingName": "Hot Joe's Kitchen",
  "legalOperatorName": "Hot Joe's Foods Limited",
  "legalOperatorType": "limitedCompany",
  "companyRegistrationNumber": "AB123456",
  "tradingCharacteristics": {
    "tradingLocation": "kitchen",
    "weeklyOpeningHours": {
      "days": [
        { "day": "monday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "tuesday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "wednesday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "thursday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "friday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "saturday", "isClosed": false, "isOpenAllDay": true, "startTime": null, "endTime": null },
        { "day": "sunday", "isClosed": true, "isOpenAllDay": false, "startTime": null, "endTime": null }
      ]
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

`tradingCharacteristics.weeklyOpeningHours.days` is required and contains exactly seven entries. Each entry requires `day`, `isClosed`, `isOpenAllDay`, `startTime` and `endTime`; the two time members accept either an invariant `HH:mm:ss` value or JSON `null` according to the daily-state rules. `day` uses the lower-camel-case Trading Day values and the submitted collection is canonicalised to Monday-through-Sunday order before downstream processing.

### 6.2.2 Determine Required Licence Types Request

The request contains only the six controlling inputs. It excludes caller-authored Address values and every retained or previous determination value.

```json
{
  "legalOperatorType": "limitedCompany",
  "tradingLocation": "kitchen",
  "weeklyOpeningHours": {
    "days": [
      { "day": "monday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
      { "day": "tuesday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
      { "day": "wednesday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
      { "day": "thursday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
      { "day": "friday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
      { "day": "saturday", "isClosed": false, "isOpenAllDay": true, "startTime": null, "endTime": null },
      { "day": "sunday", "isClosed": true, "isOpenAllDay": false, "startTime": null, "endTime": null }
    ]
  },
  "serviceIncludesHotFood": true,
  "alcoholService": false,
  "addressResolutionReference": "addr-resolution-example"
}
```

The same Weekly Opening Hours structural and time representations used by Register Vendor apply here. The API maps the flat determination representation into the transport-independent Application request; the Compliance port receives authoritative Address values rather than `addressResolutionReference`.

### 6.2.3 Join Community Request

```json
{
  "vendorId": "00000000-0000-0000-0000-000000000000",
  "contactPreference": "email"
}
```

`contactPreference` is exactly `email`, `sms` or `whatsApp`. Unknown members are ignored. Missing, null or invalid required members produce the controlled validation representation. The request contains no participation Boolean, Primary Contact information, Communication Consent or delivery instruction.

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
    "weeklyOpeningHours": {
      "days": [
        { "day": "monday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "tuesday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "wednesday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "thursday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "friday", "isClosed": false, "isOpenAllDay": false, "startTime": "17:00:00", "endTime": "02:00:00" },
        { "day": "saturday", "isClosed": false, "isOpenAllDay": true, "startTime": null, "endTime": null },
        { "day": "sunday", "isClosed": true, "isOpenAllDay": false, "startTime": null, "endTime": null }
      ]
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

Retrieval returns the authoritative persisted Weekly Opening Hours in canonical Monday-to-Sunday order using the same daily member names, state rules and explicit-null time representation as the registration request.

The response excludes the Aggregate, Domain and Integration Events, outbox state and persistence metadata.

### 6.3.3 Determine Required Licence Types

```json
{
  "ruleSetVersion": "epic1-v1",
  "items": [
    { "requiredLicenceType": "foodBusinessRegistration", "isRequired": true },
    { "requiredLicenceType": "streetTradingLicence", "isRequired": false },
    { "requiredLicenceType": "lateNightRefreshmentLicence", "isRequired": true },
    { "requiredLicenceType": "premisesLicence", "isRequired": false },
    { "requiredLicenceType": "personalLicenceHolder", "isRequired": false }
  ]
}
```

`ruleSetVersion` is a non-empty opaque Compliance-owned string; `epic1-v1` is illustrative rather than a mandated versioning scheme. `items` always contains exactly the five controlled values in canonical order with an explicit boolean for each. The response contains no Address, authority, Licence Details, evidence, Compliance Requirement, Vendor or Registration Session information.

### 6.3.4 Join Community

First successful processing and equivalent replay both return the original `201 Created` response:

```json
{
  "communityParticipationId": "00000000-0000-0000-0000-000000000000",
  "vendorId": "00000000-0000-0000-0000-000000000000",
  "contactPreference": "email",
  "joinedAt": "2026-09-22T10:30:00.0000000Z"
}
```

The response contains the stable `Location: /community-participations/{communityParticipationId}`. Equivalent replay returns the original identifiers, preference and timestamp rather than constructing a new success.

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

Weekly Opening Hours validation paths identify the affected collection or daily member, for example `tradingCharacteristics.weeklyOpeningHours.days` or `tradingCharacteristics.weeklyOpeningHours.days[0].startTime`; they do not use the visual “Everyday” shortcut as a contract path.

Determination validation paths use the determination request shape, for example `legalOperatorType`, `weeklyOpeningHours.days`, `weeklyOpeningHours.days[0].startTime` and `addressResolutionReference`. They use the same approved validation-entry code vocabulary and return every independently detectable controlling-input error together.

Join Community validation paths are `vendorId` and `contactPreference`. They use the same approved validation-entry code vocabulary.

Example without field validation:

```json
{
  "code": "vendorNotFound",
  "message": "The requested Vendor was not found.",
  "validationErrors": null
}
```

Messages are explanatory text rather than identifiers and may be improved compatibly without changing the stable code. Every expected Application failure uses this envelope. Register Vendor validation uses `registrationValidationFailed`; determination validation uses `determinationValidationFailed`; Join Community validation uses `communityParticipationValidationFailed`. `registrationDeclarationFailed` and `conditionalRuleFailed` are not contract outcomes.

## 6.5 HTTP Status and Error-Code Mapping

| Outcome | HTTP Status | Error Code |
|---|---:|---|
| Structurally malformed or unbindable request | `400 Bad Request` | `requestMalformed` |
| DeterminationRequestValidationFailure | `400 Bad Request` | `determinationValidationFailed` |
| RequestValidationFailure, including request-field, Registration Declaration, conditional and cross-field errors | `400 Bad Request` | `registrationValidationFailed` |
| Join Community RequestValidationFailure | `400 Bad Request` | `communityParticipationValidationFailed` |
| Invalid Address reference | `400 Bad Request` | `invalidAddressReference` |
| Invalid contextual Address result | `400 Bad Request` | `invalidAddressResult` |
| Aggregate invariant failure | `400 Bad Request` | `aggregateInvariantFailed` |
| Vendor not found | `404 Not Found` | `vendorNotFound` |
| Idempotency conflict | `409 Conflict` | `idempotencyConflict` |
| CommunityParticipationConflict | `409 Conflict` | `communityParticipationConflict` |
| Unsupported Compliance determination coverage | `409 Conflict` | `complianceDeterminationUnsupported` |
| Address service temporarily unavailable | `503 Service Unavailable` | `addressServiceTemporarilyUnavailable` |
| Compliance determination temporarily unavailable | `503 Service Unavailable` | `complianceDeterminationTemporarilyUnavailable` |
| VendorVerificationUnavailable | `503 Service Unavailable` | `vendorVerificationUnavailable` |
| CommunityPersistenceUnavailable | `503 Service Unavailable` | `communityPersistenceUnavailable` |
| Persistence or atomic recording failure | `503 Service Unavailable` | `persistenceOrAtomicRecordingFailed` |
| Unexpected unhandled failure | `500 Internal Server Error` | `unexpectedFailure` |

Epic 1 does not use `422 Unprocessable Content`. Unexpected exceptions are handled at one central API boundary, logged without duplication and returned without stack traces, database/provider details, internal class names or framework diagnostics.

Authentication and authorization are outside Epic 1; this contract therefore assigns no `401` or `403` outcome.

## 6.6 Headers

| Header | Direction | Requirement | Purpose |
|---|---|---|---|
| `Content-Type: application/json` | Determine, Register and Join Community requests and every response with a body | Required | JSON media type |
| `Accept: application/json` | Request | Standard negotiation when supplied | Requested response media type |
| `Location: /vendors/{vendorId}` | Successful Register response | Required | Identifies the registered Vendor retrieval resource |
| `Location: /community-participations/{communityParticipationId}` | Successful Join Community response, including equivalent replay | Required | Identifies the original Community Participation resource |

No caller-supplied `Idempotency-Key` or equivalent header is accepted. No custom correlation header or error-body correlation member is normative in this contract; CON-035 retains that responsibility.


# 7. Traceability Matrix

| Contract Element | Source Artefact | Source Section / Model Element | Interpretation |
|---|---|---|---|
| Complete self-contained request | HJ-104; HJ-105; ADR-004 | HJ-104 §6; HJ-105 §5; ADR-004 §2.2 | Registration begins with one complete request and never depends on server-side Registration Session state |
| Registration fields and classifications | HJ-104 | §§2, 5.5 | Request table preserves required, optional, conditional, derived, Registered, Vendor Managed and transient classifications |
| Legal Operator rules | HJ-104; HJ-004 | HJ-104 §§5.1–5.3; HJ-004 §8 | Type controls Company Registration Number presence and canonicalisation |
| Trading Characteristics | HJ-003; HJ-004; HJ-104 | HJ-003 §3.4; HJ-004 §2.3; HJ-104 §§2, 5.1 | Four operating characteristics, including the complete Weekly Opening Hours schedule, are persisted and remain distinct from legal identity |
| Registration Declarations | HJ-104; HJ-004 | HJ-104 §2 and §5.5; HJ-004 §§1.3, 8 | Mandatory transient inputs; never aggregate or event state |
| Address trust boundary | ADR-006; HJ-104; HJ-105 | ADR-006 §2; HJ-104 §§5.4–5.5; HJ-105 §§5–8 | Request carries only approved reference; Address Domain supplies authoritative values |
| Vendor creation | ADR-004; HJ-004; HJ-105 | ADR-004 §2.3; HJ-004 §§2, 3, 8; HJ-105 §§8, 14 | Successful registration creates one Vendor in PendingActivation and Offline |
| Register Vendor minimum outcome | HJ-004 | §1.7, Register Vendor Completion Semantics | Minimum synchronous business outcome identifies VendorId and Vendor State PendingActivation; further response fields are service-contract choices |
| Registration/Activation separation | ADR-004; ADR-007 | ADR-004 §2.3; ADR-007 §2 | Registration neither authorises trading nor decides Compliance or activation |
| Register Vendor identity, equivalence and conflict | HJ-010; HJ-012; HJ-104; HJ-105; ADR-008 | CON-013; HJ-104 §§5.3, 5.6; HJ-105 §10; ADR-008 §§2.2–2.4 | Identity is normalized Trading Name plus normalized Legal Operator Name plus Canonical Address Identifier; equivalent replay returns the original committed result; materially different information returns `IdempotencyConflict` without effects or update |
| Required Licence Types determination | HJ-003; HJ-010; HJ-012; HJ-104; HJ-105; ADR-007 | HJ-003 §§3.20–3.22; CON-047; HJ-104 §§4.1, 5.1; HJ-105 §7; ADR-007 §2.1 | A separate Vendor Application operation resolves authoritative Address information and invokes the Compliance-owned policy to return the complete five-item determination and active Rule Set Version without side effects |
| Determination lifecycle and RegisterVendor independence | HJ-010; HJ-012; HJ-104; HJ-105; ADR-007 | CON-047; HJ-104 §4.1; HJ-105 §7; ADR-007 §2.1 | The client invalidates and refreshes transient review state; changed results require renewed review; RegisterVendor contains, persists, fingerprints, retrieves and publishes none of it |
| Community terminology and ownership | HJ-003; HJ-004; HJ-010; HJ-012; ADR-002 | Community Participation and Contact Preference; CON-046; ADR-002 Community boundary | Community owns affirmative participation, preference, persistence and event; neither is Vendor Registration, Vendor aggregate state, Communication Consent or delivery authority |
| Join Community operation and Vendor verification | HJ-010; HJ-012; HJ-104; HJ-105 | CON-046; HJ-104 Community information; HJ-105 post-registration sequence | The exact VendorId and Contact Preference request is accepted only after minimal authoritative Vendor verification and exposes no Primary Contact information |
| Community replay, conflict and atomicity | HJ-010; HJ-012; HJ-105; ADR-008 | CON-046; Community sequence; ADR-008 Community idempotency | Unique VendorId creates one immutable record and outbox item; equivalent requests return original success, different preferences conflict and concurrency creates at most one event |
| Community event and stub consumer | HJ-010; HJ-012; HJ-105; ADR-003; ADR-008; ADR-013 | CON-046; Community publication and processing sequence | `CommunityParticipationRecorded` v1 is serialized once and delivered at least once to an independently executable Community-owned idempotent stub consumer that sends no communication |
| Domain Event | HJ-004; ADR-008 | HJ-004 §7.1; ADR-008 §2.5 | Internal completed fact; no prescribed minimum payload |
| Integration Event translation | HJ-004; HJ-010; HJ-012; HJ-105; ADR-003; ADR-008 | HJ-004 §7.2; CON-019; HJ-105 §11; ADR-003 §2.2; ADR-008 §§2.5–2.6 | Vendor Application maps the completed business fact to the external contract before outbox persistence; Domain and Integration Event representations remain separate; relay reconstruction is prohibited |
| VendorRegistered Integration Event v1 | HJ-004; HJ-010; HJ-012; HJ-105; ADR-008 | HJ-004 §7.2; amended CON-020; HJ-105 §11.1; ADR-008 §§2.5–2.6 | The deliberately corrected unreleased v1 uses the exact seven-day `weeklyOpeningHours.days` representation, contract-owned types, deterministic ordering and formats, explicit-null daily times and BusinessAddress optionals; the UTF-8 JSON is serialized once and published unchanged |
| Failure atomicity | HJ-105; ADR-008 | HJ-105 §§9, 11, 13; ADR-008 §2.6 | Pre-commit failure creates no partial Vendor or publication work; dispatch failure does not undo registration |
| Retrieval actor and language | HJ-003 | §§3.23–3.25 | Vendor Administrator retrieves Registered Vendor Details by VendorId |
| Retrieval source and projection | HJ-004 | §1.7 | Persisted aggregate is authoritative; application maps it to a purpose-specific representation |
| Retrieval sequence and not-found outcome | HJ-105 | §15 | Repository lookup uses VendorId only and produces Found or controlled Not Found |
| Retrieval exclusions and side effects | HJ-004; HJ-105 | HJ-004 §1.7; HJ-105 §§15–16 | No search, mutation, event, Identity, Address, Compliance or read-model dependency |
| Approved Domain implementation architecture | HJ-012 | CON-001 to CON-005 | Aggregate, Value Object, Entity, Domain Event and Repository Approaches fulfil existing Domain and service guarantees without adding service behaviour |
| Reliable publication and broker delivery | HJ-010; HJ-012; HJ-105; ADR-003; ADR-008 | CON-017, CON-018, CON-021 and CON-029; HJ-105 §11.2; ADR-003 §2; ADR-008 §§2.6–2.8 | Atomic outbox staging is followed by leased PostgreSQL relay claims, immutable at-least-once RabbitMQ publication with confirms, bounded retry, stalled-work recovery and durable dead-letter handling |
| Durable Compliance receipt | HJ-010; HJ-012; HJ-105; ADR-003; ADR-007 | CON-022; HJ-105 §11.2; ADR-007 §2.2 | The thin Compliance consumer adapter records EventId and a byte hash before acknowledgement, suppresses equivalent duplicates and dead-letters identity conflicts without introducing post-registration Compliance Domain behaviour |
| Publication trace context | HJ-010; HJ-012; ADR-008 | CON-035; ADR-008 §2.8 | W3C trace context is propagated as outbox metadata and RabbitMQ headers without changing the immutable Integration Event payload |
| Registered Vendor retrieval implementation architecture | HJ-012 | CON-027 | Query handler, Repository and response mapper fulfil the existing persisted-source, purpose-specific result and side-effect-free retrieval contract |
| Address application boundary and consumed result | HJ-012; ADR-006; HJ-104; HJ-105 | CON-006–CON-011; ADR-006 §2; HJ-104 §§2, 5.4; HJ-105 §§6, 13.2 | Application port and typed adapter resolve the permanent contextual reference, apply the approved positional source-line mapping, and distinguish semantic from retryable technical failure |
| HTTP adaptation and validation | HJ-010; HJ-012; HJ-011; HJ-104; HJ-105 | CON-023–CON-026, CON-040, CON-046 and CON-047; HJ-011 Community and Vendor API scope; HJ-104 information contracts; HJ-105 operation sequences | Thin API adapters expose four Application operations; Register Vendor retains its approved mapping while determination and Join Community use explicit representations without owning policy or behaviour |
| Operational and enforcement controls | HJ-010; HJ-012; HJ-011 | CON-029, CON-036, CON-037 and CON-039 | Reviewed deployment-applied migrations, dependency-sensitive health, architecture tests and CI gates support the contract without becoming request, response or event members |
| Separate API models | HJ-005 | §§9.4, 16.1 | HTTP requests/results do not expose domain aggregate or persistence entities |
| Controlled errors and HTTP guidance | HJ-005 | §§12.4, 16.3 | Technical mapping uses safe error bodies and result-appropriate status codes |

# 8. Assumptions and Open Questions

| Classification | Item | Consequence / Required Decision |
|---|---|---|
| Confirmed | HJ-002 v2.0, HJ-003 v2.6, HJ-004 v3.1, HJ-010 v2.19, HJ-011 v2.18, HJ-012 v2.14, HJ-104 v3.9 and HJ-105 v4.4 are Approved | They are authoritative inputs to this regenerated contract |
| Confirmed | `JoinCommunity` belongs to Community and accepts exactly VendorId plus Email, SMS or WhatsApp Contact Preference | Invocation is affirmative participation; the request contains no Boolean, Primary Contact, consent or delivery instruction |
| Confirmed | Join Community verifies successful registration through a minimal Vendor-owned capability | Unknown or unverifiable Vendor creates no Community state or publication work and no Vendor or Primary Contact representation crosses the boundary |
| Confirmed | Community Participation is immutable, unique by VendorId and atomically persisted with its outbox item | Equivalent replay returns original success, a different preference conflicts and concurrency creates exactly one record and at most one event |
| Confirmed | `CommunityParticipationRecorded` v1 and the Community consumer use the established outbox, RabbitMQ and durable receipt patterns | Event payload is minimal; the Community-owned deterministic stub sends no communication and receipt is processing evidence only |
| Confirmed | Amendment, withdrawal, Communication Consent, recipient resolution and message delivery are outside Epic 1 | Absence is not stored as refusal and the Epic 1 preference authorises no communication |
| Confirmed | `DetermineRequiredLicenceTypes` is separate, side-effect-free and Compliance-owned in policy meaning | Vendor Application validates and resolves Address information before invoking its consumed Compliance port; Vendor and the Web client contain no applicability rules |
| Confirmed | Every successful determination contains the active Rule Set Version and exactly five canonically ordered explicit items | Partial, duplicate or caller-selected results are prohibited |
| Confirmed | The retained determination is invalidated by every controlling-input change and refreshed immediately before Web submission | A changed set or Rule Set Version requires renewed review; a controlled failure retains the draft and prevents only that Web submission attempt |
| Confirmed | Direct RegisterVendor remains independent of Compliance availability | It accepts no determination, Required Licence Types, Rule Set Version, fingerprint or token and does not recalculate them |
| Confirmed | Register Vendor creates one Vendor in PendingActivation and Offline | No draft Vendor or Registration lifecycle state is exposed |
| Confirmed | Register Vendor’s successful HTTP response contains only VendorId and Vendor State PendingActivation | First processing and equivalent replay return the original `201 Created` outcome; complete state is available through retrieval |
| Confirmed | Vendor uniqueness identity is trimmed, case-insensitive Trading Name plus trimmed, case-insensitive Legal Operator Name plus Canonical Address Identifier | Derive it after Address resolution; retain the original registered name display values |
| Confirmed | Equivalent registration information for an existing composite identity is a replay | Return the original committed successful result without repeating any business effect |
| Confirmed | Materially different registration information for an existing composite identity is `IdempotencyConflict` | Create or change no Vendor, event, publication work or Pending Activation Process; registration is not an update operation |
| Confirmed | Retrieve Registered Vendor is read-only and uses VendorId only | No search, cross-domain call or event belongs in the operation |
| Confirmed | `POST /vendors` and `GET /vendors/{vendorId}` retain their approved representations | CON-023/CON-024 remain authoritative for registration and retrieval adaptation |
| Technical Convention | `POST /vendor-registration/required-licence-types` represents the side-effect-free determination operation | PR-002 requires a minimal HTTP proposal; the route, request flattening, `ruleSetVersion` string and item JSON are technical conventions inferred from CON-047 rather than new business rules |
| Confirmed | Lower-camel JSON, deterministic identifiers/times, explicit-null response optionals and compatible unknown request members | CON-024 defines the complete wire conventions used in §6 |
| Confirmed | Successful equivalent replay returns the original `201` response and body | The transport preserves the permanent committed Application outcome |
| Confirmed | Successful registration returns `Location: /vendors/{vendorId}` | No caller idempotency or custom correlation header is introduced |
| Confirmed | Concurrent coordination, replay-outcome persistence and retention, transaction mechanics, database enforcement, and exact fingerprint encoding and storage representation | CON-014–CON-016 and CON-028 approve PostgreSQL concurrency authority, permanent outcome persistence, deterministic SHA-256 fingerprinting, representation version 2 with canonical Weekly Opening Hours, migrated legacy replay compatibility, one atomic transaction and explicit EF Core/PostgreSQL constraints |
| Confirmed | Business Address Snapshot schema and positional translation used by Vendor Registration and retrieval | Address source Line 1 maps to optional RecipientOrOrganisationName; source Lines 2–4 map respectively to AddressLine1–3; Post Town, Postcode and optional County map directly; no concatenation, compression, shifting or reordering occurs |
| Confirmed | Address Resolution reference semantics and failure taxonomy | Permanent, opaque, non-expiring, non-revocable, reusable and non-consuming; InvalidReference and InvalidAddressResult are semantic failures; temporary technical failure is caller-retryable with no in-process retry |
| Confirmed | `VendorRegistered` Integration Event v1 schema, translation, serialization and compatibility | CON-019, amended CON-020 and CR-073 define the Application mapper, exact seven-day Weekly Opening Hours structure and Monday-to-Sunday order, deterministic UUID, UTC timestamp, time-only and lower-camel enum formats, contract-owned representations, independent BusinessAddress, explicit-null optionals, UTF-8 camel-case serialization, the controlled pre-release v1 correction and post-release versioning rules |
| Confirmed | Outbox relay processing and broker delivery semantics | CON-018, CON-021 and ADR-008 v1.5 define leased claims, publisher confirms, bounded retry, stalled work, at-least-once delivery, no ordering claim and dead-letter handling without event reconstruction |
| Confirmed | Compliance receipt and duplicate handling | CON-022 and ADR-007 v1.2 preserve durable post-registration receipt before acknowledgement, equivalent duplicate suppression and byte-conflict dead-lettering separately from pre-registration determination |
| Confirmed | Contact Email and Primary Contact Telephone validation and canonicalisation | CON-026 and HJ-104 v3.6 define the exact supported profiles, canonical stored values and structural-only boundary |
| Confirmed | Expected failure envelope, validation paths/codes and HTTP mappings | Amended CON-025, CON-026 and CON-040 define one `RequestValidationFailure` containing all independently detectable validation errors and one `registrationValidationFailed` API mapping in §6.4–§6.5; Epic 1 does not use `422` |
| Confirmed | Correlation propagation and ownership | CON-035 uses W3C trace context as outbox metadata and RabbitMQ headers; it does not change the event JSON or introduce a custom API error member |

# 9. Review Checklist

- [x] Models the approved Vendor Registration workflow.
- [x] Includes every operation required by the current Epic 1 authoritative artefacts.
- [x] Preserves Vendor bounded-context ownership and persistence authority.
- [x] Uses Vendor State and other ubiquitous language consistently.
- [x] Preserves the Vendor lifecycle boundary and creates no draft Vendor.
- [x] Preserves Address Domain ownership and immutable Address snapshots.
- [x] Keeps Registration separate from Activation and trading authorisation.
- [x] Keeps Compliance policy, Licence Details, evidence and lifecycle state outside Vendor while exposing the approved pre-registration determination operation.
- [x] Returns the complete five-item Compliance Determination and active Rule Set Version with no side effect.
- [x] Preserves transient invalidation, fresh pre-submission determination and renewed review when the result changes.
- [x] Keeps RegisterVendor independent of the determination result and Compliance availability.
- [x] Keeps Community Participation and Contact Preference outside Vendor Registration and the Vendor aggregate.
- [x] Defines Join Community as a separate post-registration Community operation with minimal Vendor verification.
- [x] Preserves immutable Community persistence, original-result replay, controlled conflict and atomic outbox staging.
- [x] Defines the exact minimal `CommunityParticipationRecorded` v1 event and durable idempotent Community stub receipt.
- [x] Introduces no Communication Consent, recipient resolution, provider call or delivery claim.
- [x] Carries one canonical seven-day Weekly Opening Hours schedule through request, validation, persistence, retrieval, fingerprint and publication boundaries.
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
- [x] Reconciles the HJ-010 v2.19/HJ-012 v2.14 Approved baseline, including CON-046, CON-047 and the existing reliable-publication, broker-delivery, durable receipt, migration, trace-context and enforcement cohort.
- [x] Keeps approved implementation patterns from becoming unnecessary service-contract requirements.
- [x] Keeps the remaining unresolved architectural choices explicit and traceable to HJ-010 concerns.

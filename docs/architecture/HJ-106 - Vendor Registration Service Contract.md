# HJ-106 - Vendor Registration Service Contract

| Property | Value |
|---|---|
| **Document ID** | HJ-106 |
| **Document Title** | Vendor Registration Service Contract |
| **Version** | 1.6 |
| **Status** | Approved |
| **Classification** | Service Contract |
| **Owner** | Project Architecture |
| **Last Updated** | 23 August 2026 |

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

## Related Documents

| Document ID | Title | Status | Relevance |
|---|---|---|---|
| PR-002 | Infer Service Contract from Approved Domain Artefacts | Governing prompt | Governs the inference method and document structure |
| HJ-002 | Architectural Principles | Approved | Capability ownership, contracts, events and architectural simplicity |
| HJ-003 | Ubiquitous Language Guide | Approved | Authoritative Vendor terminology and query language |
| HJ-004 | Vendor Domain Models | Approved | Vendor aggregate, lifecycle, invariants, events and retrieval model |
| HJ-005 | Coding Standards | Approved | API boundary, validation, error and HTTP conventions |
| HJ-010 v1.8 | Current Application Architectural Concerns | Approved | Records the amended concrete CON-020 contract while retaining relay and broker-delivery mechanics under CON-018 and CON-021 |
| HJ-011 v1.8 | Epic 1 Vendor Registration Implementation Scope | Approved | Places the exact VendorRegistered Integration Event v1 representation within Epic 1 delivery scope |
| HJ-012 v1.8 | Established Application Architecture Patterns | Approved | Defines the approved CON-019 translation boundary and amended CON-020 published contract |
| HJ-104 | Vendor Registration Fields Matrix | Approved | Authoritative registration information and business rules |
| HJ-105 v3.8 | Vendor Registration Sequence Diagram | Approved | Authoritative interaction order, outcomes, failure behaviour and deterministic event-publication representation |
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
- a proposed minimal HTTP representation.

Part A, Sections 1–5, is the inferred business contract. Part B, Section 6, proposes a technical HTTP representation. Technical conventions do not amend the authoritative business artefacts.

# 2. Source Artefacts

| Artefact | Why Used | Authority Provided |
|---|---|---|
| HJ-002 | Constrains interpretation of capability boundaries and published contracts | Vendor owns Vendor behaviour and persistence; contracts are explicit; events represent completed facts; prefer simplicity |
| HJ-003 v2.3 | Supplies authoritative language | Vendor State, Vendor Registration, Registered Information, Vendor Managed Information, Vendor Administrator, Retrieve Registered Vendor and Registered Vendor Details |
| HJ-004 v2.7 | Defines the business model exposed by this contract | Aggregate properties, creation invariants, lifecycle, commands, events, the exact VendorRegistered v1 published contract, persisted retrieval source, result content and query side-effect invariant |
| HJ-005 | Separates business contract from implementation conventions | Separate API models, controlled errors, layered validation, status-code guidance and safe response rules |
| HJ-104 v3.5 | Is the authoritative registration information contract | Required, optional, conditional, derived and transient fields; validation; canonicalisation; composite Vendor uniqueness identity; semantic registration equivalence; ownership; lifecycle |
| HJ-105 v3.8 | Defines observable interaction behaviour | Complete request boundary, Address collaboration, idempotency, pre-outbox event mapping, deterministic event serialization, immutable serialized-event staging, reliable publication and registered Vendor retrieval |
| ADR-002 | Prevents ownership leakage | Vendor capability owns Vendor registration and persistence; other capabilities retain their own behaviour and data |
| ADR-003 v1.2 | Governs asynchronous collaboration | Completed business facts cross boundaries through explicit Integration Events mapped before outbox persistence and never reconstructed by the relay |
| ADR-004 | Defines when a Vendor exists | No Vendor exists before successful registration; Registration Session remains outside the Vendor service boundary |
| ADR-005 | Governs information classification | Registered Information and Vendor Managed Information have distinct post-registration lifecycles |
| ADR-006 v1.3 | Defines Address authority | Address Domain supplies canonical identity, immutable snapshot and applicable regulatory authorities |
| ADR-007 | Preserves compliance separation | Registration creates Pending Activation; Compliance and activation decisions remain outside registration |
| ADR-008 v1.4 | Defines mandatory reliability behaviour | Explicit Register Vendor idempotency safeguard; atomic persistence; pre-outbox VendorRegistered mapping; serialization and immutable outbox storage; retry without reconstruction or repeated business effects |

No contradictory business rule was found among the supplied source artefacts. HJ-104 v3.5 specializes the approved registration-information and idempotency rules. HJ-004 v2.7 and HJ-105 v3.8 define the approved concrete VendorRegistered v1 contract and observable publication sequence consistently with ADR-003 v1.2 and ADR-008 v1.4. The approved business artefacts govern business behaviour; HJ-005 informs only the proposed HTTP representation.

HJ-010 v1.8 and HJ-012 v1.8 are the controlled architectural governance baseline for this contract. CON-006–CON-011 establish the Address collaboration boundary, consumed result and failure semantics used below. CON-013–CON-017 and CON-028 establish idempotency, concurrency, replay persistence, transaction, reliable staging and database-enforcement mechanics. CON-019 establishes the pre-outbox translation boundary and amended CON-020 establishes the exact published VendorRegistered v1 representation. Relay processing and broker delivery remain governed separately by unresolved CON-018 and CON-021.

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
| Contact Email | Business correspondence address | Vendor | Required | HJ-104 §§2, 5.2 | Valid email format | Persisted | Registered Information | Stored in Primary Contact |
| Contact Telephone | Primary contact number | Vendor | Required | HJ-104 §§2, 5.2 | UK telephone validation; exact accepted formats unspecified | Persisted | Registered Information | Stored in Primary Contact |
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
| Semantic Registration Fingerprint | Derived deterministically from all materially relevant registration information after approved canonicalisation | Used to distinguish equivalent replay from `IdempotencyConflict`; excludes transient declarations, the opaque Address Resolution reference, server-generated values and technical metadata; exact encoding and storage remain deferred |
| Business Address Snapshot | Supplied exclusively by the Address Domain from the approved Address Resolution | Persisted immutably as Registered Information; concrete schema is unspecified |
| Food Registration Authority | Derived and supplied by the Address Domain from the approved Business Address or mobile-unit base address | Persisted as Registered Information |
| Primary Trading Authority | Derived and supplied by the Address Domain | Persisted only when Trading Location is `Stall`; absent otherwise |
| Initial Vendor State | Established by successful Vendor creation | `PendingActivation` |
| Initial Trading Preference | Established by successful Vendor creation | `Offline` |
| Compliance Requirements | Determined later by the Compliance capability from published information | Not returned, stored or decided by Register Vendor |

## 4.5 Business Validation

The service shall enforce the HJ-104 rules server-side regardless of client-side convenience validation.

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

RegisteredAt, Trading Preference and other persisted Vendor properties are committed state, but HJ-004 does not require them in the minimum `RegisterVendor` response. This contract proposes RegisteredAt and Trading Preference as additional response fields in Section 6; callers can obtain the complete committed Vendor state through `RetrieveRegisteredVendor`.

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

Concurrent equivalent requests must converge on one processing owner and one successful outcome. Registration Session state is never consulted. The concurrency technique, replay persistence and retention, transaction mechanics, database enforcement, and exact fingerprint encoding and storage representation are governed separately by CON-014, CON-015, CON-016 and CON-028.

If the same composite identity is associated with materially different registration information from the previously successful registration, the operation shall return `IdempotencyConflict`. It shall not create or modify a Vendor, record a completed business fact or Domain Event, create publication work, publish an Integration Event or initiate Pending Activation. Vendor updates require a separate future administration operation. The response payload and transport status are technical conventions.

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
| Request validation failure | Missing, malformed or out-of-range request information | Registration cannot be attempted with the supplied information | Yes, after correction | Correct the identified fields and resubmit |
| Registration Declaration failure | Any mandatory declaration is absent or not explicitly accepted | Applicant has not supplied the mandatory business confirmations | Yes, after explicit acceptance | Obtain confirmation and resubmit |
| Conditional rule failure | Company Registration Number or Primary Trading Authority presence conflicts with its controlling type/location | Complete registration invariants are not satisfied | Yes, after correction or authoritative Address re-resolution | Correct client information or select/resolve an appropriate Address |
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

# 6. Proposed HTTP Representation

Everything in this section is a proposed technical convention unless explicitly traced to a business artefact. Route shapes, JSON property names, enum serialization, identifiers, timestamps, headers and error codes require technical review.

## 6.1 Endpoint

| Business Operation | Proposed Method | Proposed Route | Success |
|---|---|---|---|
| Register Vendor | `POST` | `/vendors` | `201 Created` with committed registration outcome |
| Retrieve Registered Vendor | `GET` | `/vendors/{vendorId}` | `200 OK` with Registered Vendor Details |

No search or collection retrieval endpoint is proposed.

## 6.2 Request Example

`addressResolutionReference` represents only the approved permanent reference; Address-owned values and the application-derived composite Vendor identity are absent from the caller-authored request.

```json
{
  "tradingName": "Hot Joe's Kitchen",
  "legalOperatorName": "Hot Joe's Foods Limited",
  "legalOperatorType": "LimitedCompany",
  "companyRegistrationNumber": "AB123456",
  "tradingCharacteristics": {
    "tradingLocation": "Kitchen",
    "openingHours": {
      "startTime": "17:00",
      "endTime": "02:00"
    },
    "serviceIncludesHotFood": true,
    "alcoholService": false
  },
  "primaryContact": {
    "contactName": "Jordan Smith",
    "contactEmail": "jordan@example.test",
    "contactTelephone": "+44 20 7946 0123"
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

The nesting and enum spellings shown are technical conventions. The business fields and rules are authoritative; their JSON representation is not.

## 6.3 Success Response

### Register Vendor

```json
{
  "vendorId": "vendor-example-id",
  "registeredAt": "2026-08-08T10:30:00Z",
  "vendorState": "PendingActivation",
  "tradingPreference": "Offline"
}
```

VendorId and the committed `PendingActivation` outcome are business-required. Including RegisteredAt and Trading Preference in the synchronous HTTP body is a proposed convention consistent with committed Vendor state.

### Retrieve Registered Vendor

```json
{
  "vendorId": "vendor-example-id",
  "registeredAt": "2026-08-08T10:30:00Z",
  "vendorState": "PendingActivation",
  "tradingPreference": "Offline",
  "tradingName": "Hot Joe's Kitchen",
  "legalOperatorType": "LimitedCompany",
  "legalOperatorName": "Hot Joe's Foods Limited",
  "companyRegistrationNumber": "AB123456",
  "tradingCharacteristics": {
    "tradingLocation": "Kitchen",
    "openingHours": {
      "startTime": "17:00",
      "endTime": "02:00"
    },
    "serviceIncludesHotFood": true,
    "alcoholService": false
  },
  "primaryContact": {
    "contactName": "Jordan Smith",
    "contactEmail": "jordan@example.test",
    "contactTelephone": "+44 20 7946 0123"
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

The Business Address Snapshot field set is normative at the business-contract level. JSON null-versus-omission and other wire serialization choices remain unresolved under CON-024.

## 6.4 Error Responses

Proposed controlled error envelope:

```json
{
  "code": "vendor_registration_validation_failed",
  "message": "The Vendor could not be registered because supplied information is invalid.",
  "validationErrors": [
    {
      "field": "tradingName",
      "code": "length_out_of_range",
      "message": "Trading Name must contain between 1 and 160 characters."
    }
  ],
  "correlationId": "correlation-example-id"
}
```

Vendor Not Found example:

```json
{
  "code": "vendor_not_found",
  "message": "The requested Vendor was not found.",
  "correlationId": "correlation-example-id"
}
```

Idempotency conflict example:

```json
{
  "code": "idempotency_conflict",
  "message": "A Vendor with the same registration identity already exists with materially different registration information.",
  "correlationId": "correlation-example-id"
}
```

Error-code vocabulary, validation-path syntax and correlation identifier format are proposed conventions. Responses must remain client-safe and omit internal diagnostics.

## 6.5 HTTP Status Codes

| Status | Proposed Use |
|---|---|
| `200 OK` | Registered Vendor retrieved successfully |
| `201 Created` | Vendor committed successfully; proposed for both first processing and an equivalent successful replay so the original HTTP outcome is preserved |
| `400 Bad Request` | Structurally malformed request or business validation/declaration failure |
| `404 Not Found` | VendorId does not identify an existing Vendor |
| `409 Conflict` | Proposed technical mapping for the business-required controlled idempotency-conflict outcome |
| `422 Unprocessable Content` | Optional alternative convention for semantically invalid but structurally valid registration; not selected unless the project standard adopts it |
| `500 Internal Server Error` | Unexpected failure, with no internal details exposed |
| `503 Service Unavailable` | Proposed for temporary Address or persistence dependency unavailability where safe retry is appropriate |

Authentication and authorisation are outside Epic 1; therefore this contract does not assign `401 Unauthorized` or `403 Forbidden` to either operation.

## 6.6 Headers

| Header | Direction | Requirement | Purpose |
|---|---|---|---|
| `Content-Type: application/json` | Request and response | Proposed required convention where a body exists | Media type |
| `Accept: application/json` | Request | Proposed convention | Requested response media type |
| `Location` | Register response | Proposed | Identifies `/vendors/{vendorId}` after creation |
| `X-Correlation-Id` | Request and response | Proposed | Correlates logs and controlled errors; exact name and generation rules unspecified |

No caller-supplied idempotency header is proposed. The Vendor Application derives the approved composite identity after authoritative Address resolution.

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
| Remaining architecture dependencies | HJ-010 | CON-018, CON-021 and CON-024 to CON-026 | Downstream derivation must preserve unresolved relay, broker-delivery, transport, error-mapping and validation matters and must not invent their Approaches |
| Separate API models | HJ-005 | §§9.4, 16.1 | HTTP requests/results do not expose domain aggregate or persistence entities |
| Controlled errors and HTTP guidance | HJ-005 | §§12.4, 16.3 | Technical mapping uses safe error bodies and result-appropriate status codes |

# 8. Assumptions and Open Questions

| Classification | Item | Consequence / Required Decision |
|---|---|---|
| Confirmed | HJ-002 v2.0, HJ-003 v2.3, HJ-004 v2.7, HJ-104 v3.5 and HJ-105 v3.8 are Approved | They are authoritative inputs to this regenerated contract |
| Confirmed | Register Vendor creates one Vendor in PendingActivation and Offline | No draft Vendor or Registration lifecycle state is exposed |
| Confirmed | Register Vendor’s minimum business response identifies VendorId and Vendor State PendingActivation | RegisteredAt and Trading Preference are proposed additional response fields rather than mandatory minimum response content |
| Confirmed | Vendor uniqueness identity is trimmed, case-insensitive Trading Name plus trimmed, case-insensitive Legal Operator Name plus Canonical Address Identifier | Derive it after Address resolution; retain the original registered name display values |
| Confirmed | Equivalent registration information for an existing composite identity is a replay | Return the original committed successful result without repeating any business effect |
| Confirmed | Materially different registration information for an existing composite identity is `IdempotencyConflict` | Create or change no Vendor, event, publication work or Pending Activation Process; registration is not an update operation |
| Confirmed | Retrieve Registered Vendor is read-only and uses VendorId only | No search, cross-domain call or event belongs in the operation |
| Technical Convention | `POST /vendors` and `GET /vendors/{vendorId}` | CON-024: route review is required before implementation |
| Technical Convention | Camel-case JSON and enum spellings for the proposed HTTP request and response representations | CON-024: HTTP serialization conventions require approval; this does not affect the approved VendorRegistered v1 Integration Event representation |
| Technical Convention | Successful identical replay returns the same `201` response and body | CON-024 and CON-025: confirm the transport mapping while preserving the business outcome |
| Technical Convention | `Location` and `X-Correlation-Id` headers | CON-024: confirm header names and mandatory behaviour |
| Confirmed | Concurrent coordination, replay-outcome persistence and retention, transaction mechanics, database enforcement, and exact fingerprint encoding and storage representation | CON-014–CON-016 and CON-028 approve PostgreSQL concurrency authority, permanent outcome persistence, deterministic SHA-256 fingerprinting, one atomic transaction and explicit EF Core/PostgreSQL constraints |
| Confirmed | Business Address Snapshot schema and positional translation used by Vendor Registration and retrieval | Address source Line 1 maps to optional RecipientOrOrganisationName; source Lines 2–4 map respectively to AddressLine1–3; Post Town, Postcode and optional County map directly; no concatenation, compression, shifting or reordering occurs |
| Confirmed | Address Resolution reference semantics and failure taxonomy | Permanent, opaque, non-expiring, non-revocable, reusable and non-consuming; InvalidReference and InvalidAddressResult are semantic failures; temporary technical failure is caller-retryable with no in-process retry |
| Confirmed | `VendorRegistered` Integration Event v1 schema, translation, serialization and compatibility | CON-019 and amended CON-020 define the Application mapper, exact nested member structure, deterministic UUID, UTC timestamp, time-only and lower-camel enum formats, contract-owned representations, independent BusinessAddress, explicit-null optionals, UTF-8 camel-case serialization, compatible optional additions and new-version requirements for breaking changes |
| Missing Information | Outbox relay processing and broker delivery semantics | CON-018 and CON-021: resolve claim, retry, acknowledgement, duplicate delivery, ordering and poison-message treatment without reconstructing the stored event |
| Missing Information | Exact UK telephone validation rule | CON-026: specify accepted forms through an approved business or validation source before executable contract validation is produced |
| Missing Information | Identifier, timestamp and time-of-day formats for the proposed HTTP API | CON-024: adopt explicit HTTP serialization standards before OpenAPI generation; the VendorRegistered v1 Integration Event formats are approved and no longer missing |
| Missing Information | Null-versus-omission behaviour for optional and inapplicable result fields | CON-024: decide for Company Registration Number, Primary Trading Authority, Website and Business Description |
| Missing Information | Authoritative service error-code catalogue and field-path convention | CON-024 and CON-025: agree before client integration and test-catalogue finalisation |
| Technical Convention | HTTP mappings for Address failures | CON-024 and CON-025 still govern wire status/error-envelope mapping; the business failure distinction and retry semantics are approved |

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
- [x] Separates inferred business requirements from proposed HTTP conventions.
- [x] Records missing information and ambiguities instead of silently resolving them.
- [x] Reconciles the HJ-010/HJ-012 v1.8 Approved baseline, including amended CON-020, without deciding unresolved relay or broker-delivery mechanics.
- [x] Keeps approved implementation patterns from becoming unnecessary service-contract requirements.
- [x] Keeps unresolved architectural choices explicit and traceable to HJ-010 concerns.

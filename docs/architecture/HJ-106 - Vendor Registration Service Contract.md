# HJ-106 - Vendor Registration Service Contract

| Property | Value |
|---|---|
| **Document ID** | HJ-106 |
| **Document Title** | Vendor Registration Service Contract |
| **Version** | 1.0 |
| **Status** | Approved |
| **Classification** | Service Contract |
| **Owner** | Project Architecture |
| **Last Updated** | 8 August 2026 |

## Revision History

| Version | Date | Description |
|---|---|---|
| 0.1 | 8 August 2026 | Initial service contract regenerated from the updated Vendor Registration artefacts using PR-002, including Register Vendor, Retrieve Registered Vendor, controlled idempotency conflict behaviour and explicit Register Vendor completion semantics. |
| 1.0 | 8 August 2026 | Regenerated from the latest approved Vendor artefacts using PR-002 and promoted as the Approved service-contract baseline. Reconciled HJ-105 v3.2 and removed the resolved idempotency alignment records without changing the approved business behaviour. |

## Related Documents

| Document ID | Title | Status | Relevance |
|---|---|---|---|
| PR-002 | Infer Service Contract from Approved Domain Artefacts | Governing prompt | Governs the inference method and document structure |
| HJ-002 | Architectural Principles | Approved | Capability ownership, contracts, events and architectural simplicity |
| HJ-003 | Ubiquitous Language Guide | Approved | Authoritative Vendor terminology and query language |
| HJ-004 | Vendor Domain Models | Approved | Vendor aggregate, lifecycle, invariants, events and retrieval model |
| HJ-005 | Coding Standards | Approved | API boundary, validation, error and HTTP conventions |
| HJ-104 | Vendor Registration Fields Matrix | Approved | Authoritative registration information and business rules |
| HJ-105 | Vendor Registration Sequence Diagram | Approved | Authoritative interaction order, outcomes and failure behaviour |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted | Capability and persistence ownership |
| ADR-003 | Event-Driven Collaboration | Accepted | Cross-capability event collaboration |
| ADR-004 | Vendor Lifecycle Begins After Successful Registration | Accepted | Registration Session, service and Vendor-existence boundaries |
| ADR-005 | Registered Information vs Vendor Managed Information | Accepted | Information classification and editability |
| ADR-006 | Address Domain Ownership and Business Address Snapshots | Accepted | Address trust boundary and snapshot invariant |
| ADR-007 | Vendor Compliance as a Separate Bounded Context | Accepted | Compliance and activation separation |
| ADR-008 | Idempotent Operations and Reliable Event Publication | Accepted | Register Vendor idempotency and publication reliability |

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
| HJ-003 v2.2 | Supplies authoritative language | Vendor State, Vendor Registration, Registered Information, Vendor Managed Information, Vendor Administrator, Retrieve Registered Vendor and Registered Vendor Details |
| HJ-004 v2.2 | Defines the business model exposed by this contract | Aggregate properties, creation invariants, lifecycle, commands, events, persisted retrieval source, result content and query side-effect invariant |
| HJ-005 | Separates business contract from implementation conventions | Separate API models, controlled errors, layered validation, status-code guidance and safe response rules |
| HJ-104 v3.1 | Is the authoritative registration information contract | Required, optional, conditional, derived and transient fields; validation; canonicalisation; ownership; lifecycle |
| HJ-105 v3.2 | Defines observable interaction behaviour | Complete request boundary, Address collaboration, successful registration, controlled idempotency conflict, failures, reliable publication and registered Vendor retrieval |
| ADR-002 | Prevents ownership leakage | Vendor capability owns Vendor registration and persistence; other capabilities retain their own behaviour and data |
| ADR-003 | Governs asynchronous collaboration | Completed business facts cross capability boundaries through explicit Integration Events |
| ADR-004 | Defines when a Vendor exists | No Vendor exists before successful registration; Registration Session remains outside the Vendor service boundary |
| ADR-005 | Governs information classification | Registered Information and Vendor Managed Information have distinct post-registration lifecycles |
| ADR-006 | Defines Address authority | Address Domain supplies canonical identity, immutable snapshot and applicable regulatory authorities |
| ADR-007 | Preserves compliance separation | Registration creates Pending Activation; Compliance and activation decisions remain outside registration |
| ADR-008 v1.1 | Defines mandatory reliability behaviour | Explicit Register Vendor idempotency safeguard; equivalent replay; controlled idempotency conflict; atomic persistence/publication recording; retry without repeated business effects |

No contradictory business rule was found among the supplied source artefacts. HJ-105 v3.2 illustrates the controlled idempotency-conflict behaviour defined by ADR-008 and its traceability references align with the current ADR-008 structure. The approved business artefacts govern business behaviour; HJ-005 informs only the proposed technical representation.

# 3. Business Operation Summary

| Operation | Business Purpose | Owning Bounded Context | Preconditions | Successful Outcome |
|---|---|---|---|---|
| `RegisterVendor` | Create a Vendor from complete and valid registration information | Vendor | One complete request; mandatory declarations accepted; approved Address Resolution reference; all field rules and aggregate invariants satisfied; idempotency outcome established | One Vendor is committed in `PendingActivation` and `Offline`; original successful outcome is returned; `VendorRegistered` publication work is durably recorded |
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
- An explicit idempotency identity or approved equivalent uniqueness constraint protects the creation operation.
- Reuse of an idempotency identity with semantically different registration information is treated as a controlled idempotency conflict and never as a new registration.

No Vendor or server-side Registration Session needs to exist before invocation.

## 4.3 Request Information

| Field | Business Meaning | Data Owner | Required Status | Source Artefact | Validation | Persisted or Transient | Registered or Vendor Managed | Notes |
|---|---|---|---|---|---|---|---|---|
| Trading Name | Public name under which the Vendor trades | Vendor | Required | HJ-104 §§2, 5.2 | 1–160 characters | Persisted | Registered Information | Read-only to Vendor after registration |
| Legal Operator Name | Registered legal name of the operator | Vendor | Required | HJ-003 §3.11; HJ-104 §§2, 5.2 | 1–160 characters | Persisted | Registered Information | “Company Name” is historical terminology only |
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
| Idempotency Identity | Identifies retried or concurrent registration processing | Technical caller / Vendor application boundary | Required unless an approved equivalent uniqueness constraint exists | ADR-008 §§2.2–2.4; HJ-105 §§5, 9 | Concrete format, retention and equivalence rules unspecified | Transient request metadata with implementation-defined safeguard state | Neither | Registration Session is outside the idempotency boundary |

The request must not authoritatively supply Canonical Address Identifier, Business Address Snapshot, Food Registration Authority or Primary Trading Authority. If present, client-authored Address-owned values are rejected or ignored.

## 4.4 Derived Information

| Information | Derivation and Ownership | Persistence and Use |
|---|---|---|
| VendorId | Created by the Vendor Domain after successful validation | Persisted as aggregate identity and returned in the successful outcome |
| RegisteredAt | Recorded by the Vendor Domain on successful creation | Persisted and available in registered Vendor details and event contracts where specified |
| Canonical Address Identifier | Supplied exclusively by the Address Domain from the approved Address Resolution | Persisted with the snapshot as Registered Information |
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
- a new Vendor starts in `PendingActivation` and `Offline`; and
- incomplete or invalid registration creates no Vendor, completed business fact, event or publication work.

Trading Characteristics comprise Trading Location, Opening Hours, Service Includes Hot Food and Alcohol Service. They describe the operation; they do not replace Legal Operator Type, which describes legal identity and registration obligations.

## 4.6 Address Domain Collaboration

1. The request supplies the approved Address Resolution reference, not authoritative Address content.
2. The Vendor Registration Application requests the approved result from the Address Service.
3. The Address Domain validates the reference and returns the Canonical Address Identifier, immutable Business Address Snapshot, Food Registration Authority and Primary Trading Authority where applicable.
4. The application rejects or ignores caller-authored Address-owned values.
5. The Vendor aggregate persists the canonical identifier and snapshot together, with applicable authorities.

An invalid, expired or unresolved reference produces a controlled Address failure. No Vendor, Domain Event, Integration Event or publication work is created.

## 4.7 Successful Outcome

A first successful invocation:

- commits exactly one Vendor aggregate;
- assigns VendorId and RegisteredAt;
- persists Registered Information and supplied Vendor Managed Information;
- persists the Address-owned canonical identifier, immutable snapshot and applicable authorities;
- establishes Vendor State `PendingActivation` and Trading Preference `Offline`;
- records the internal `VendorRegistered` Domain Event;
- atomically records durable `VendorRegistered` Integration Event publication work; and
- synchronously returns a committed successful outcome containing, at minimum, VendorId and Vendor State `PendingActivation`.

Pending Activation and Integration Event dispatch continue asynchronously and do not alter the synchronous successful outcome.

RegisteredAt, Trading Preference and other persisted Vendor properties are committed state, but HJ-004 does not require them in the minimum `RegisterVendor` response. This contract proposes RegisteredAt and Trading Preference as additional response fields in Section 6; callers can obtain the complete committed Vendor state through `RetrieveRegisteredVendor`.

## 4.8 Vendor Lifecycle Transition

Before successful registration, no Vendor exists. Successful `RegisterVendor` creates the Vendor directly in `PendingActivation`; it is not a transition from a draft or registration state. Trading Preference begins `Offline`, and the Vendor cannot trade.

Registration does not activate the Vendor, establish activation eligibility or make it operationally available.

## 4.9 Idempotency

`RegisterVendor` is not naturally idempotent and requires an explicit idempotency identity or equivalent approved uniqueness constraint over the complete request.

For the same identity and a semantically identical previously successful request, or the equivalent approved uniqueness condition, the operation shall:

- return the original successful outcome;
- create no additional Vendor;
- record no additional Domain Event or completed business fact;
- create no additional outbox or publication record;
- publish no additional Integration Event; and
- initiate no additional Pending Activation Process.

Concurrent equivalent requests must converge on one processing owner and one successful outcome. Registration Session state is never consulted. Identity format, retention, payload equivalence, storage and concurrency mechanisms are technical conventions not defined by this contract.

If the same idempotency identity, or equivalent approved uniqueness condition, is associated with registration information that is not semantically identical to the previously successful request, the operation shall return a controlled idempotency-conflict outcome. It shall not create or modify a Vendor, record a completed business fact or Domain Event, create publication work, publish an Integration Event or initiate Pending Activation. The response payload and transport status are technical conventions.

## 4.10 Domain and Integration Events

| Event | Business Meaning | Producing Bounded Context | Minimum Payload | Information Intentionally Excluded |
|---|---|---|---|---|
| Internal `VendorRegistered` Domain Event | Successful Vendor creation is a completed fact inside the Vendor Domain | Vendor | No minimum payload is prescribed by HJ-004 | Registration Declarations; no requirement to mirror the Integration Event; infrastructure metadata |
| Published `VendorRegistered` Integration Event | Announces successful registration so downstream Pending Activation processing can begin without a synchronous Vendor callback | Vendor | VendorId; RegisteredAt; resulting Vendor State; Trading Preference; Legal Operator Type; Trading Characteristics; approved registration-time Business Address information required by Compliance; Food Registration Authority; Primary Trading Authority where applicable | Registration Declarations; full aggregate; internal persistence metadata; unspecified internal Domain Event representation |

Vendor persistence and durable recording of Integration Event publication work must commit atomically. Dispatch failure leaves the registered Vendor unchanged and the durable publication record available for retry. A publication retry does not repeat registration or create another business fact or event record.

The concrete Business Address payload schema within the Integration Event is deliberately deferred and must not be inferred from the aggregate or snapshot representation.

## 4.11 Business Failures

| Failure | Trigger | Business Meaning | Retryable | Expected Caller Behaviour |
|---|---|---|---|---|
| Request validation failure | Missing, malformed or out-of-range request information | Registration cannot be attempted with the supplied information | Yes, after correction | Correct the identified fields and resubmit using the approved idempotency approach |
| Registration Declaration failure | Any mandatory declaration is absent or not explicitly accepted | Applicant has not supplied the mandatory business confirmations | Yes, after explicit acceptance | Obtain confirmation and resubmit |
| Conditional rule failure | Company Registration Number or Primary Trading Authority presence conflicts with its controlling type/location | Complete registration invariants are not satisfied | Yes, after correction or authoritative Address re-resolution | Correct client information or select/resolve an appropriate Address |
| Address Resolution failure | Reference is invalid, expired, unresolved or does not return required authoritative values | The Vendor Domain cannot trust or persist the Business Address | Yes, after correction or reselection | Search/select/resolve the Address again and submit the approved reference |
| Aggregate invariant failure | Valid-looking inputs still violate Vendor creation rules | The Vendor cannot be created in a valid state | Yes, if correctable | Correct the reported business information; do not interpret technical exceptions as business messages |
| Persistence or atomic-recording failure | Vendor and publication work cannot commit atomically | Registration has not succeeded | Yes through the idempotency safeguard | Retry safely; do not treat the Vendor as registered without a successful response |
| Duplicate or concurrent equivalent submission | Same idempotency identity and semantically identical successful request | Registration has already completed | Not a failure; convergent replay | Accept the original successful outcome; do not start another registration |
| Integration Event dispatch failure | Durable publication exists but external dispatch fails | Registration remains successful; downstream notification is delayed | Caller retry not required | Do not register again; infrastructure retries publication |
| Idempotency conflict | Same identity, or equivalent approved uniqueness condition, is reused with registration information that is not semantically identical to the prior successful request | This is not a retry and must return a controlled conflict without creating or changing business state | No as submitted; a genuinely new registration requires a distinct valid identity | Do not treat the response as registration success; correct accidental key reuse or start a distinct request according to the approved client convention |

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

The `Idempotency-Key` header is the proposed representation of the required idempotency identity. `addressResolutionReference` represents only the approved reference; Address-owned values are absent.

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
    "addressLine1": "Example address line",
    "locality": "Example locality",
    "postalCode": "AB1 2CD"
  },
  "foodRegistrationAuthority": "Example authority",
  "primaryTradingAuthority": null,
  "website": "https://example.test",
  "businessDescription": "Evening food delivery kitchen."
}
```

The Business Address Snapshot object above is illustrative only. Its concrete field schema is missing from the approved source artefacts and is not established by this contract. Optional and inapplicable properties may instead be omitted; null-versus-omission is an unresolved serialization convention.

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
  "message": "The idempotency identity is already associated with different registration information.",
  "correlationId": "correlation-example-id"
}
```

Error-code vocabulary, validation-path syntax and correlation identifier format are proposed conventions. Responses must remain client-safe and omit internal diagnostics.

## 6.5 HTTP Status Codes

| Status | Proposed Use |
|---|---|
| `200 OK` | Registered Vendor retrieved successfully |
| `201 Created` | Vendor committed successfully; proposed for both first processing and an identical successful replay so the original HTTP outcome is preserved |
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
| `Idempotency-Key` | Register request | Proposed required representation unless an equivalent approved uniqueness mechanism is selected | Carries the idempotency identity |
| `Location` | Register response | Proposed | Identifies `/vendors/{vendorId}` after creation |
| `X-Correlation-Id` | Request and response | Proposed | Correlates logs and controlled errors; exact name and generation rules unspecified |

No idempotency header applies to the read-only retrieval query.

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
| Register Vendor idempotency and conflict | ADR-008; HJ-105 | ADR-008 §§2.2–2.4; HJ-105 §9 | Explicit identity or equivalent uniqueness; identical replay returns the original result; semantically different reuse returns a controlled conflict without effects |
| Domain Event | HJ-004; ADR-008 | HJ-004 §7.1; ADR-008 §2.5 | Internal completed fact; no prescribed minimum payload |
| Integration Event | HJ-004; HJ-105; ADR-008 | HJ-004 §7.2; HJ-105 §10.1; ADR-008 §§2.5–2.6 | Explicit downstream contract, durably recorded atomically and published asynchronously |
| Failure atomicity | HJ-105; ADR-008 | HJ-105 §§8, 10, 12; ADR-008 §2.6 | Pre-commit failure creates no partial Vendor or publication work; dispatch failure does not undo registration |
| Retrieval actor and language | HJ-003 | §§3.20–3.22 | Vendor Administrator retrieves Registered Vendor Details by VendorId |
| Retrieval source and projection | HJ-004 | §1.7 | Persisted aggregate is authoritative; application maps it to a purpose-specific representation |
| Retrieval sequence and not-found outcome | HJ-105 | §14 | Repository lookup uses VendorId only and produces Found or controlled Not Found |
| Retrieval exclusions and side effects | HJ-004; HJ-105 | HJ-004 §1.7; HJ-105 §§14–15 | No search, mutation, event, Identity, Address, Compliance or read-model dependency |
| Separate API models | HJ-005 | §§9.4, 16.1 | HTTP requests/results do not expose domain aggregate or persistence entities |
| Controlled errors and HTTP guidance | HJ-005 | §§12.4, 16.3 | Technical mapping uses safe error bodies and result-appropriate status codes |

# 8. Assumptions and Open Questions

| Classification | Item | Consequence / Required Decision |
|---|---|---|
| Confirmed | HJ-002 v2.0, HJ-003 v2.2, HJ-004 v2.2, HJ-104 v3.1 and HJ-105 v3.2 are Approved | They are authoritative inputs to this approved contract |
| Confirmed | Register Vendor creates one Vendor in PendingActivation and Offline | No draft Vendor or Registration lifecycle state is exposed |
| Confirmed | Register Vendor’s minimum business response identifies VendorId and Vendor State PendingActivation | RegisteredAt and Trading Preference are proposed additional response fields rather than mandatory minimum response content |
| Confirmed | Semantically different reuse of an existing idempotency identity is an idempotency conflict | Return a controlled conflict and create or change no Vendor, event, publication work or Pending Activation Process |
| Confirmed | Retrieve Registered Vendor is read-only and uses VendorId only | No search, cross-domain call or event belongs in the operation |
| Technical Convention | `POST /vendors` and `GET /vendors/{vendorId}` | Route review is required before implementation |
| Technical Convention | Camel-case JSON, flattened operation names and enum spellings | Serialization conventions require approval |
| Technical Convention | `Idempotency-Key` carries the explicit identity | An equivalent uniqueness constraint remains architecturally permitted |
| Technical Convention | Successful identical replay returns the same `201` response and body | Confirm whether replay should instead return `200` while preserving the business outcome |
| Technical Convention | `Location` and `X-Correlation-Id` headers | Confirm header names and mandatory behaviour |
| Missing Information | Idempotency identity format, retention duration, semantic-equivalence algorithm and storage | Define as application/infrastructure policy without using Registration Session state |
| Missing Information | Concrete Business Address Snapshot schema in Register, Retrieve and Integration Event contracts | Agree the Address-owned contract; do not infer it from persistence or aggregate internals |
| Missing Information | Exact approved Address Resolution reference representation and failure taxonomy | Define with the Address Service contract |
| Missing Information | Exact UK telephone validation rule | Specify accepted forms before executable contract validation is produced |
| Missing Information | Identifier, timestamp and time-of-day wire formats | Adopt explicit serialization standards before OpenAPI generation |
| Missing Information | Null-versus-omission behaviour for optional and inapplicable result fields | Decide for Company Registration Number, Primary Trading Authority, Website and Business Description |
| Missing Information | Authoritative service error-code catalogue and field-path convention | Agree before client integration and test-catalogue finalisation |
| Missing Information | Availability/error distinction for Address rejection versus temporary Address Service failure | Define retry and HTTP mapping while preserving no-partial-registration behaviour |

# 9. Review Checklist

- [x] Models the approved Vendor Registration workflow.
- [x] Includes every operation required by the current Epic 1 authoritative artefacts.
- [x] Preserves Vendor bounded-context ownership and persistence authority.
- [x] Uses Vendor State and other ubiquitous language consistently.
- [x] Preserves the Vendor lifecycle boundary and creates no draft Vendor.
- [x] Preserves Address Domain ownership and immutable Address snapshots.
- [x] Keeps Registration separate from Activation and trading authorisation.
- [x] Keeps Compliance decisions and evidence outside Vendor Registration.
- [x] Applies explicit idempotency to the non-naturally-idempotent Register Vendor operation.
- [x] Returns a controlled conflict for semantically different reuse of an idempotency identity without creating additional business effects.
- [x] Produces events only for genuine successful Vendor creation.
- [x] Separates the internal Domain Event from the published Integration Event.
- [x] Requires atomic Vendor persistence and durable publication recording.
- [x] Defines Retrieve Registered Vendor as a side-effect-free query over persisted Vendor state.
- [x] Returns Registered Vendor Details rather than exposing the aggregate.
- [x] Introduces no unsupported search, Identity dependency, lifecycle state or read-model infrastructure.
- [x] Separates inferred business requirements from proposed HTTP conventions.
- [x] Records missing information and ambiguities instead of silently resolving them.

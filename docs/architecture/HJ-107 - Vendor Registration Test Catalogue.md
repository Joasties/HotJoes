# HJ-107 - Vendor Registration Test Catalogue

| Property | Value |
|---|---|
| **Document ID** | HJ-107 |
| **Document Title** | Vendor Registration Test Catalogue |
| **Version** | 1.0 |
| **Status** | Approved |
| **Classification** | Test Catalogue |
| **Owner** | Project Architecture |
| **Last Updated** | 15 August 2026 |

## Revision History

| Version | Date | Description |
|---|---|---|
| 0.1 | 10 August 2026 | Initial catalogue generated from HJ-106 v1.0 and its approved or accepted source artefacts using PR-004. Covers RegisterVendor and RetrieveRegisteredVendor, separates normative business obligations from proposed API conventions, and records unresolved dependencies without inventing contracts. |
| 0.2 | 14 August 2026 | Regenerated from HJ-106 v1.1 using PR-004 as aligned by CR-041. Preserved all stable Test IDs; applied the authoritative HJ-006 classification/level mapping; added HJ-010 concern dependencies and HJ-012/HJ-013 authority boundaries; and reconciled behavioural coverage without changing the approved service behaviour. |
| 1.0 | 15 August 2026 | Approved the regenerated Vendor Registration Test Catalogue as the controlled behavioural-test baseline. Promotion changes document control only: all `VR-*` Test IDs, behavioural obligations, dependency classifications, proposed Part B tests and traceability are unchanged from v0.2. |

## Related Documents

| Document ID | Title | Version | Status | Relationship |
|---|---|---:|---|---|
| PR-004 | Generate Test Catalogue from Approved Service Contract | Current | Governing prompt | CR-041-aligned regeneration method, authority boundaries and required structure |
| HJ-002 | Architectural Principles | 2.0 | Approved | Ownership, explicit contracts and completed business facts |
| HJ-003 | Ubiquitous Language Guide | 2.2 | Approved | Authoritative terminology |
| HJ-004 | Vendor Domain Models | 2.3 | Approved | Aggregate, invariants, lifecycle, events and retrieval model; v2.3 removes delivery-slice scope without changing the enduring model |
| HJ-005 | Coding Standards | 2.0 | Approved | Technical API, validation and controlled-error guidance |
| HJ-006 | Testing Strategy and Standards | 2.0 | Approved | Test levels, classifications and quality rules |
| HJ-007 | Enforcement Strategy | 2.0 | Approved | Supporting enforcement boundary; not a source of Vendor Registration behaviour |
| HJ-010 | Current Application Architectural Concerns | 1.0 | Approved | Current concern dependencies that generation must preserve rather than resolve |
| HJ-012 | Established Application Architecture Patterns | 1.0 | Approved | Approved implementation architecture; not a source of additional service behaviour |
| HJ-104 | Vendor Registration Fields Matrix | 3.2 | Approved | Registration fields, classifications and lifecycle; v3.2 removes delivery-slice scope without changing the enduring contract |
| HJ-105 | Vendor Registration Sequence Diagram | 3.4 | Approved | Processing order, collaboration, outcomes and failures; later revisions align runtime scope and restore the enduring behavioural model |
| HJ-106 | Vendor Registration Service Contract | 1.1 | Approved | Primary normative behavioural test basis |
| HJ-013 | Architecture and Implementation Test Catalogue | Current | Downstream catalogue | Owns complementary architecture and implementation verification; not an HJ-107 generation or behavioural source |
| ADR-002 | Business Capabilities and Bounded Contexts | Current | Accepted | Capability and persistence ownership |
| ADR-003 | Event-Driven Collaboration | Current | Accepted | Asynchronous cross-capability collaboration |
| ADR-004 | Vendor Lifecycle Begins After Successful Registration | Current | Accepted | Registration and Vendor-existence boundaries |
| ADR-005 | Registered Information vs Vendor Managed Information | Current | Accepted | Information classification and editability |
| ADR-006 | Address Domain Ownership and Business Address Snapshots | Current | Accepted | Address authority and immutable snapshot |
| ADR-007 | Vendor Compliance as a Separate Bounded Context | Current | Accepted | Compliance and activation separation |
| ADR-008 | Idempotent Operations and Reliable Event Publication | 1.2 | Accepted | Idempotency, atomic recording and publication recovery |

# 1. Purpose

This document is the authoritative catalogue of behavioural test obligations derived from HJ-106 v1.1 Part A and its approved or accepted sources for the Epic 1 `RegisterVendor` and `RetrieveRegisteredVendor` business operations.

It defines what must be verified, why, at which boundary and with which prohibited outcomes. It contains no executable test code and does not prescribe frameworks, fixtures, mocks, brokers, databases or CI implementation.

# 2. Source Authority

| Artefact | Classification | Use and Authority |
|---|---|---|
| HJ-106 Part A, §§1–5 | Normative primary basis | Defines the approved operations, request and result information, validation, invariants, collaboration, events, idempotency, failures, retrieval and exclusions. |
| HJ-106 Part B, §6 | Non-normative proposal | Supplies only proposed API-contract obligations in Section 6 of this catalogue. |
| HJ-003 | Supporting authoritative language | Confirms Vendor, Vendor Registration, Vendor Administrator, Registered Information, Vendor Managed Information, Retrieve Registered Vendor and Registered Vendor Details terminology. |
| HJ-004 | Supporting domain authority | Confirms aggregate creation, invariants, initial state, Domain Event semantics and retrieval representation. |
| HJ-104 | Supporting information authority | Defines every registration field, controlled value, validation, canonicalisation, classification and lifecycle rule. |
| HJ-105 | Supporting behavioural authority | Confirms processing order, failure atomicity, replay, concurrency, publication and retrieval interactions. |
| ADR-004 | Supporting decision | Keeps Registration Session outside every Vendor service boundary and defines when a Vendor begins to exist. |
| ADR-006 | Supporting decision | Makes Address Domain values authoritative and the stored Business Address Snapshot immutable. |
| ADR-008 | Supporting decision | Requires safe replay, controlled conflict, atomic persistence/publication work and independent dispatch retry. |
| HJ-002, HJ-005, ADR-002, ADR-003, ADR-005, ADR-007 | Supporting constraints | Preserve ownership, information classification, event collaboration, safe technical boundaries and Compliance separation. |
| HJ-006 | Normative testing standard | Defines the approved Test Levels and Domain, Application, Persistence Integration, Integration Contract, API Contract and Contract Review classifications. |
| HJ-010 | Architectural governance | Identifies unresolved Current Architectural Concerns and dependencies. It does not add Vendor Registration behaviour. |
| HJ-012 | Approved architecture governance | Identifies approved implementation architecture that fulfils existing service guarantees. It does not add service behaviour absent from HJ-106 Part A. |
| Existing HJ-107 v0.1 | Regeneration baseline | Supplies stable Test IDs and retained catalogue obligations; it is not a normative source of behaviour. |

No conflict was found among the normative and supporting sources. HJ-106 v1.1 records no change to normative business behaviour from v1.0. HJ-004 v2.3, HJ-104 v3.2 and HJ-105 v3.4 preserve the enduring Domain, information and behavioural rules used by HJ-106; their later scope and runtime-alignment revisions introduce no additional HJ-107 behaviour. Change Requests and the previous HJ-107 were not used as normative behavioural inputs.

# 3. Catalogue Conventions

| Convention | Definition |
|---|---|
| Test ID | Stable `VR-<AREA>-<NNN>` identifier. |
| Test Classification / Level | Each executable obligation uses exactly one HJ-006 classification and its mapped level: Domain / Unit, Application / Unit, Persistence Integration / Integration, Integration Contract / Integration or API Contract / API Integration. End-to-End is used only where a complete workflow cannot be verified lower. Contract Review / Non-executable is a review obligation. |
| Priority | Critical protects ownership, aggregate validity, idempotency, atomicity, event correctness or duplicate-effect prohibitions; Important protects required business behaviour, lifecycle, collaboration or failure behaviour; Standard covers supporting rules and optional boundaries; Proposed is Part B only. Blocked is not a Priority. |
| Automation | Automate, Parameterised automation, Contract automation after dependency, Manual contract review, or Deferred. |
| Dependency Status | Ready, Partially Blocked, Blocked, or Proposed pending approval. Deferred automation is used only when sequencing or approval prevents implementation; it is not a Priority. |
| Prohibited outcome | A mandatory negative assertion. Its omission makes the obligation incomplete. |
| Normative test | Derived from HJ-106 Part A. |
| Proposed test | Derived only from HJ-106 Part B and never treated as approved business behaviour. |

Parameterized obligations may cover equivalent values or boundaries, but materially different outcomes have separate Test IDs.

HJ-107 owns the behavioural boundary established by HJ-106. HJ-013 owns complementary dependency enforcement, concrete mapping, transaction-mechanism, Transactional Outbox, relay-recovery and broker-mechanism verification. The same guarantee may have separate obligations at distinct boundaries, but identical obligations are not duplicated across the catalogues.

# 4. Coverage Summary

| Test Area | Obligations | Primary Classification / Level | Primary Source | Coverage | Open Dependencies |
|---|---:|---|---|---|---|
| Successful Registration | 5 | Application / Unit | HJ-106 §§4.7–4.8 | Covered/partly blocked | CON-016 and CON-028 affect durable persistence execution |
| Request Completeness | 6 | Application / Unit | HJ-106 §§4.2–4.5 | Covered | Telephone rule partly blocked |
| Legal Operator Rules | 7 | Domain / Unit | HJ-106 §§4.3–4.5 | Covered/partly blocked | CON-028 affects canonical persistence proof |
| Trading Characteristics | 5 | Domain / Unit | HJ-106 §§4.3–4.5 | Covered | None |
| Contact Information | 4 | Application / Unit | HJ-106 §4.3 | Covered | Exact telephone formats |
| Vendor Managed Information | 5 | Application / Unit | HJ-106 §4.3 | Covered | None |
| Registration Declarations | 5 | Application / Unit | HJ-106 §§4.3–4.5 | Covered/partly blocked | CON-028 affects persistence exclusion proof |
| Address Collaboration | 8 | Application / Unit | HJ-106 §§4.4–4.6 | Covered/partly blocked | Reference lifecycle and concrete snapshot schema |
| Aggregate Invariants | 5 | Domain / Unit | HJ-106 §4.5 | Covered | None |
| Lifecycle and Initial State | 4 | Domain / Unit | HJ-106 §§4.7–4.8 | Covered | None |
| Domain Event | 4 | Domain / Unit | HJ-106 §4.10 | Covered | Internal payload deliberately unconstrained |
| Integration Event | 5 | Integration Contract / Integration | HJ-106 §4.10 | Covered/partly blocked | CON-019 and CON-020 |
| Idempotency and Concurrency | 6 | Persistence Integration / Integration | HJ-106 §4.9 | Covered/partly blocked | CON-013–CON-016 and CON-028 |
| Persistence and Publication | 5 | Persistence Integration / Integration | HJ-106 §§4.7, 4.10 | Covered/partly blocked | CON-016, CON-018, CON-020, CON-021 and CON-028 |
| Registered Vendor Retrieval | 8 | Application / Unit | HJ-106 §4.12 | Covered/partly blocked | CON-024 affects HTTP mapping; CON-028 affects real persistence proof |
| Business Failures | 9 | Application / Unit | HJ-106 §4.11 | Covered/partly blocked | CON-009, CON-010, CON-016, CON-018, CON-020, CON-021 and CON-028 |
| Scope Exclusions | 5 | Contract Review / Non-executable | HJ-106 §§1, 5 | Covered | Identity contract deliberately excluded |
| Proposed API Contract | 12 | API Contract / API Integration | HJ-106 §6 | Proposed | CON-024–CON-026 |
| Blocked and Deferred | 9 | Contract Review / Non-executable | HJ-106 §§4, 6, 8; HJ-010 | Explicitly recorded | Current Concern and external-source decisions required |

# 5. Test Catalogue

All rows in Sections 5.1–5.17 are normative obligations unless their Dependency Status says Blocked.

## 5.1 Successful Registration

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-SUCCESS-001 | Complete valid registration | A complete valid request creates one Vendor. | HJ-106 | §§4.2–4.7 | Application / Unit | Critical | Address reference resolves; identity is unused. | Submit all required information, valid optional information and accepted declarations. | Exactly one committed Vendor and successful outcome. | No duplicate Vendor or partial success. | Automate | Ready | Primary happy path. |
| VR-SUCCESS-002 | Persist Registered Information | All approved Registered Information is committed. | HJ-106; HJ-104 | §§4.3–4.7; §§2, 5.5 | Persistence Integration / Integration | Critical | Successful first processing. | Register with representative valid values. | Persisted fields equal validated and canonicalised values. | No declaration or caller-authored Address authority value is persisted. | Parameterised automation | Partially Blocked | Covers all Registered Information rows. CON-028 blocks concrete PostgreSQL mapping verification. |
| VR-SUCCESS-003 | Persist supplied managed information | Website and Business Description persist when supplied. | HJ-106 | §§4.3, 4.7 | Persistence Integration / Integration | Important | Valid optional values supplied. | Register Vendor. | Both supplied values are committed. | Values are not reclassified as Registered Information. | Automate | Partially Blocked | Absence covered in §5.6. CON-028 blocks concrete PostgreSQL mapping verification. |
| VR-SUCCESS-004 | Return committed minimum outcome | Synchronous success contains VendorId and `PendingActivation`. | HJ-106 | §4.7 | Application / Unit | Important | Commit succeeds. | Complete first registration. | Returned VendorId identifies committed Vendor; state is `PendingActivation`. | No success before commit; no requirement for proposed Part B extras. | Automate | Ready | RegisteredAt and Trading Preference in HTTP body are proposed only. |
| VR-SUCCESS-005 | Enable asynchronous continuation | Durable publication work enables later `VendorRegistered` dispatch. | HJ-106 | §§4.7, 4.10 | Persistence Integration / Integration | Critical | Vendor commit succeeds. | Complete first registration. | One durable unpublished publication item exists for later dispatch. | No synchronous Compliance callback or second registration. | Automate after dependency | Partially Blocked | CON-016 and CON-028 block concrete transaction and PostgreSQL verification; the behavioural obligation is normative. |

## 5.2 Request Completeness and Required Information

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-REQ-001 | Complete self-contained request | Vendor processing depends only on the complete request and Address reference. | HJ-106; ADR-004 | §§4.1–4.2 | Application / Unit | Critical | No Vendor or Registration Session exists. | Submit a complete request. | Processing proceeds without Registration Session lookup. | No session retrieval, reconciliation or server-side session creation. | Automate | Ready | Complete request is authoritative. |
| VR-REQ-002 | Required scalar absence | Each required name, type, location, time or contact field is mandatory. | HJ-106; HJ-104 | §4.3; §2 | Application / Unit | Important | Otherwise valid request. | Omit each required scalar in turn. | Controlled validation failure identifies the missing information. | No Vendor, fact, event, publication work or Pending Activation. | Parameterised automation | Ready | Booleans and declarations have separate coverage. |
| VR-REQ-003 | Required boolean presence | Hot Food and Alcohol Service must each be supplied as booleans. | HJ-106; HJ-104 | §4.3; §2 | Application / Unit | Important | Otherwise valid request. | Omit or supply non-boolean value for each. | Controlled validation failure. | No default inferred and no business side effect. | Parameterised automation | Ready | False is a valid supplied value. |
| VR-REQ-004 | Address reference required | An approved Address Resolution reference is mandatory. | HJ-106 | §§4.2–4.3 | Application / Unit | Critical | Otherwise valid request. | Omit reference. | Controlled validation failure before aggregate creation. | No Address-owned value accepted from caller; no Vendor or event. | Automate | Ready | Format is blocked separately. |
| VR-REQ-005 | Idempotency safeguard required | RegisterVendor must have an identity or approved equivalent uniqueness constraint. | HJ-106; ADR-008 | §§4.2, 4.9 | Application / Unit | Critical | No equivalent safeguard configured. | Submit without identity. | Processing is rejected or cannot start until safeguard exists. | No unprotected Vendor creation. | Automate after dependency | Partially Blocked | CON-013 leaves the concrete identity or equivalent uniqueness contract unresolved. |
| VR-REQ-006 | Server-side enforcement | All HJ-104 rules are enforced independently of client validation. | HJ-106 | §4.5 | Application / Unit | Critical | Client permits an invalid request. | Submit directly to service boundary. | Same controlled business validation applies. | No reliance on UI-only validation. | Parameterised automation | Ready | Applies across field cases below. |

## 5.3 Legal Operator Rules

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-LEGAL-001 | Controlled Legal Operator Type | Only six approved types are valid. | HJ-106; HJ-104 | §4.3; §5.1 | Domain / Unit | Important | Complete aggregate input. | Use each approved value, then an unknown value. | Approved values accepted; unknown value rejected. | No invented type or fallback. | Parameterised automation | Ready | Six values from HJ-104. |
| VR-LEGAL-002 | Legal names lower boundary | Trading Name and Legal Operator Name require at least 1 character. | HJ-106 | §4.3 | Application / Unit | Important | Otherwise valid request. | Test 0 and 1 characters for each. | Zero rejected; one accepted. | No trimming rule invented by catalogue. | Parameterised automation | Ready | Whitespace policy is not specified. |
| VR-LEGAL-003 | Legal names upper boundary | Both names allow at most 160 characters. | HJ-106 | §4.3 | Application / Unit | Important | Otherwise valid request. | Test 160 and 161 characters. | 160 accepted; 161 rejected. | No persistence on failure. | Parameterised automation | Ready | Applies independently to each name. |
| VR-LEGAL-004 | Registration number required types | Ltd, LLP and CIO require Company Registration Number. | HJ-106; HJ-104 | §§4.3–4.5; §5.1 | Domain / Unit | Critical | Each controlling type selected. | Supply and omit number. | Presence accepted; absence violates invariant. | No Vendor/event/publication on absence. | Parameterised automation | Ready | If-and-only-if positive side. |
| VR-LEGAL-005 | Registration number prohibited types | Sole Trader, General Partnership and Charitable Community Group require absence. | HJ-106; HJ-104 | §§4.3–4.5; §5.1 | Domain / Unit | Critical | Each controlling type selected. | Omit and supply number. | Absence accepted; presence violates invariant. | No extraneous identifier persisted. | Parameterised automation | Ready | If-and-only-if negative side. |
| VR-LEGAL-006 | Registration number format | Applicable number matches `^(?:[A-Za-z]{2})?\d{6,8}$`. | HJ-106; HJ-104 | §4.3; §5.2 | Application / Unit | Important | Type requires number. | Exercise valid and invalid prefix/digit-length boundaries. | Valid forms accepted; malformed forms rejected. | No Companies House existence check. | Parameterised automation | Ready | Format only. |
| VR-LEGAL-007 | Registration number canonicalisation | Alphabetic prefix is stored uppercase. | HJ-106; HJ-104 | §4.3; §5.3 | Persistence Integration / Integration | Important | Lowercase valid prefix supplied. | Register successfully. | Persisted number has uppercase prefix. | No change to numeric portion or external lookup. | Automate | Partially Blocked | Canonicalisation before persistence. CON-028 blocks concrete PostgreSQL mapping verification. |

## 5.4 Trading Characteristics

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-TRADING-001 | Controlled Trading Location | Only Restaurant, Stall and Kitchen are valid. | HJ-106; HJ-104 | §4.3; §5.1 | Domain / Unit | Important | Complete input. | Exercise each approved and one unknown value. | Approved values accepted; unknown rejected. | No invented classification. | Parameterised automation | Ready | Exact controlled list. |
| VR-TRADING-002 | Opening Hours valid times | Start and End are valid time values. | HJ-106 | §4.3 | Application / Unit | Important | Otherwise valid request. | Supply valid and malformed time values. | Valid values accepted; malformed rejected. | No wire format invented beyond approved time validity. | Parameterised automation | Partially Blocked | CON-024 governs concrete time serialization; Domain-valid time behaviour remains testable. |
| VR-TRADING-003 | Overnight operation | End Time may be earlier than Start Time. | HJ-106; HJ-104 | §§4.3–4.5; §5.2 | Domain / Unit | Important | Valid times. | Supply 23:00–05:00. | Trading Characteristics accepted unchanged. | No `Start < End` invariant. | Automate | Ready | Explicit boundary rule. |
| VR-TRADING-004 | Boolean combinations | Hot Food and Alcohol Service preserve all four boolean combinations. | HJ-106 | §§4.3–4.5 | Domain / Unit | Standard | Complete input. | Register each combination. | Both values persist as supplied. | No value inferred from location or hours. | Parameterised automation | Ready | Used later by Compliance only. |
| VR-TRADING-005 | Characteristics remain distinct from legal identity | Trading Characteristics do not replace Legal Operator Type. | HJ-106 | §4.5 | Domain / Unit | Critical | Valid legal and trading inputs. | Create aggregate. | Both concepts are retained independently. | No legal obligation inferred solely from trading classification. | Automate | Ready | Ownership/meaning guard. |

## 5.5 Contact Information

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-CONTACT-001 | Contact Name boundaries | Contact Name is 1–100 characters. | HJ-106 | §4.3 | Application / Unit | Standard | Otherwise valid request. | Exercise 0, 1, 100 and 101 characters. | Only 1–100 accepted. | No Vendor on invalid boundary. | Parameterised automation | Ready | Exact rule. |
| VR-CONTACT-002 | Contact Email format | Contact Email must be a valid email. | HJ-106 | §4.3 | Application / Unit | Important | Otherwise valid request. | Supply representative valid and invalid formats. | Valid accepted; invalid rejected. | No specific unapproved email algorithm made normative. | Parameterised automation | Partially Blocked | CON-026 governs validation allocation; an approved validation profile is still needed for exhaustive cases. |
| VR-CONTACT-003 | Contact Telephone rule | Contact Telephone must satisfy UK telephone validation. | HJ-106 | §4.3 | Application / Unit | Important | Otherwise valid request. | Apply only cases established by an approved telephone contract. | Accepted/rejected according to that contract. | No invented accepted formats. | Deferred | Blocked | CON-026; see VR-BLOCKED-004. |
| VR-CONTACT-004 | Primary Contact persistence | Name, Email and Telephone persist together. | HJ-106 | §§4.3, 4.7 | Persistence Integration / Integration | Important | All three valid. | Register successfully. | Persisted Primary Contact equals approved request values. | No omitted or substituted contact component. | Automate | Partially Blocked | Classification is Registered Information. CON-028 blocks concrete PostgreSQL mapping verification. |

## 5.6 Vendor Managed Information

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-MANAGED-001 | Optional fields absent | Website and Business Description may both be absent. | HJ-106 | §4.3 | Application / Unit | Standard | Required information valid. | Omit both. | Registration succeeds; details represent absence. | No synthetic defaults. | Automate | Ready | Null/omission on wire is proposed only. |
| VR-MANAGED-002 | HTTPS Website | Supplied Website must be a valid HTTPS URL. | HJ-106 | §4.3 | Application / Unit | Standard | Otherwise valid request. | Supply valid HTTPS and non-HTTPS/invalid values. | HTTPS valid accepted; invalid rejected. | No HTTP URL accepted as equivalent. | Parameterised automation | Ready | Exact scheme requirement. |
| VR-MANAGED-003 | Description maximum | Business Description allows at most 2,000 characters. | HJ-106 | §4.3 | Application / Unit | Standard | Otherwise valid request. | Exercise 2,000 and 2,001 characters. | Boundary accepted/rejected respectively. | No truncation or Vendor creation on failure. | Parameterised automation | Ready | No minimum specified. |
| VR-MANAGED-004 | Independent optionality | Either optional field may be supplied without the other. | HJ-106 | §4.3 | Application / Unit | Standard | Required information valid. | Supply Website only, then Description only. | Each valid request succeeds and persists only supplied value. | No coupling invented. | Parameterised automation | Ready | Two cases. |
| VR-MANAGED-005 | Classification preserved | Optional values remain Vendor Managed Information. | HJ-106; ADR-005 | §§4.3, 4.7 | Persistence Integration / Integration | Important | Values supplied. | Register and retrieve. | Values persist and appear in Registered Vendor Details where supplied. | No reclassification as immutable Registered Information. | Automate | Partially Blocked | Future update service remains out of scope. CON-028 blocks concrete PostgreSQL mapping verification. |

## 5.7 Registration Declarations

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-DECL-001 | Explicit acceptance | Each of the three declarations must be `true`. | HJ-106; HJ-104 | §§4.2–4.5; §2 | Application / Unit | Critical | Otherwise valid request. | For each declaration test absent, false and true. | Only all-three-true permits registration. | No Vendor/fact/event/publication/Pending Activation on failure. | Parameterised automation | Ready | Materially equivalent negative cases grouped. |
| VR-DECL-002 | Declarations transient | Declarations are not persisted on Vendor aggregate. | HJ-106 | §§4.3, 4.5 | Persistence Integration / Integration | Critical | Successful registration. | Inspect persisted Vendor representation. | No declaration value is Vendor state. | No audit field silently added to Vendor. | Automate | Partially Blocked | External audit retention is outside Vendor Domain. CON-028 blocks concrete PostgreSQL mapping verification. |
| VR-DECL-003 | Excluded from Domain Event | Internal event contains no declaration. | HJ-106 | §4.10 | Domain / Unit | Critical | Successful creation. | Inspect recorded Domain Event. | All declarations absent. | No declaration leakage. | Automate | Ready | Internal payload otherwise unconstrained. |
| VR-DECL-004 | Excluded from Integration Event | Published contract contains no declaration. | HJ-106 | §4.10 | Integration Contract / Integration | Critical | Publication message derived. | Inspect contract content. | All declarations absent. | No declaration leakage across boundary. | Automate | Ready | Security/privacy guard. |
| VR-DECL-005 | No post-transaction lifecycle | Vendor service performs no later declaration management. | HJ-106; HJ-104 | §4.3; §5.5 | Contract Review / Non-executable | Important | Registration completes or fails. | Review service operations and stored model. | No declaration query, update or lifecycle exists. | No invented Vendor operation. | Manual contract review | Ready | Audit outside Vendor Domain permitted but unspecified. |

## 5.8 Address Collaboration and Derived Information

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-ADDRESS-001 | Reference-only request boundary | Caller supplies approved reference, not authoritative Address content. | HJ-106; ADR-006 | §§4.3, 4.6 | Application / Unit | Critical | Address Service available. | Submit reference with no caller snapshot/authority values. | Application resolves Address-owned values from Address Service. | No client value becomes authoritative. | Automate | Ready | Reference representation blocked separately. |
| VR-ADDRESS-002 | Caller snapshot rejected or ignored | Caller-authored snapshot cannot replace Address result. | HJ-106 | §§4.3–4.6 | Application / Unit | Critical | Reference resolves to value A. | Also submit conflicting caller snapshot B. | Stored snapshot equals Address result A. | Snapshot B is never persisted or emitted. | Automate | Ready | Contract permits reject or ignore; exact handling is technical. |
| VR-ADDRESS-003 | Caller authorities rejected or ignored | Caller-authored regulatory authorities are not authoritative. | HJ-106 | §§4.3–4.6 | Application / Unit | Critical | Reference resolves authoritative values. | Submit conflicting authority values. | Address-provided values control. | No caller authority persisted or published. | Automate | Ready | Exact reject/ignore outcome not prescribed. |
| VR-ADDRESS-004 | Canonical identity and snapshot stored together | Both values originate together and persist together. | HJ-106; ADR-006 | §§4.4–4.6 | Domain / Unit | Critical | Address result contains both. | Create Vendor. | Aggregate contains both or creation fails. | No identifier-only or snapshot-only Vendor. | Automate | Ready | Creation invariant. |
| VR-ADDRESS-005 | Snapshot stored exactly | Immutable snapshot is stored without Vendor normalisation. | HJ-106; ADR-006 | §§4.4, 4.6 | Persistence Integration / Integration | Critical | Opaque Address snapshot supplied by Address Domain. | Register and reload. | Stored value is exactly the returned snapshot. | No modification, derivation or replacement. | Contract automation after dependency | Partially Blocked | CON-009 and CON-028 block concrete Address contract and PostgreSQL mapping proof; opaque equality remains testable. |
| VR-ADDRESS-006 | Food authority mandatory | Address result and Vendor require Food Registration Authority. | HJ-106 | §§4.4–4.6 | Domain / Unit | Critical | Address result lacks/presents authority. | Attempt creation in each case. | Present accepted; absent fails. | No Vendor or events when absent. | Parameterised automation | Ready | Applies all locations. |
| VR-ADDRESS-007 | Stall primary authority | Primary Trading Authority is present iff location is Stall. | HJ-106 | §§4.4–4.6 | Domain / Unit | Critical | Address result/location combinations. | Exercise Stall with/without and non-Stall with/without value. | Only Stall+present and non-Stall+absent satisfy invariant. | No invalid aggregate. | Parameterised automation | Ready | Restaurant and Kitchen are non-Stall. |
| VR-ADDRESS-008 | Address failure atomicity | Invalid, expired, unresolved or incomplete result is controlled failure. | HJ-106 | §§4.6, 4.11 | Application / Unit | Critical | Address resolution cannot provide trusted complete result. | Submit affected reference. | Controlled Address failure with retry guidance. | No Vendor, fact, Domain Event, publication work, Integration Event or Pending Activation. | Parameterised after contract | Partially Blocked | CON-009 and CON-010 leave reference and failure taxonomy cases unresolved. |

## 5.9 Aggregate Creation Invariants

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-INV-001 | Complete valid aggregate | Aggregate can be created only with all mandatory valid inputs. | HJ-106; HJ-004 | §4.5; §8 | Domain / Unit | Critical | Valid Domain inputs assembled. | Create Vendor. | One valid aggregate and completed fact. | No partially valid aggregate. | Automate | Ready | Positive invariant synthesis. |
| VR-INV-002 | Invalid aggregate rejected | Valid-looking input that violates an invariant fails. | HJ-106 | §§4.5, 4.11 | Domain / Unit | Critical | At least one invariant violated. | Attempt creation. | Controlled Domain failure. | No Vendor or Domain Event. | Parameterised automation | Ready | Conditional invariants have dedicated tests. |
| VR-INV-003 | One trading location | One Vendor represents one trading location. | HJ-106 | §4.5 | Domain / Unit | Critical | Request attempts multiple locations. | Attempt creation. | Request is outside approved model/rejected. | No multi-location aggregate. | Automate if representable | Ready | Multiple premises explicitly deferred. |
| VR-INV-004 | Address origin invariant | Aggregate accepts canonical Address values only from trusted application input sourced from Address Domain. | HJ-106; ADR-006 | §§4.5–4.6 | Domain / Unit | Critical | Trusted and untrusted mappings available. | Attempt creation through each boundary. | Only trusted Address result reaches aggregate. | No caller-authored replacement. | Automate | Ready | Application trust boundary also covered. |
| VR-INV-005 | Failure produces no completed fact | Any creation failure has no completed business fact or event. | HJ-106 | §§4.5, 4.11 | Domain / Unit | Critical | Any invariant fails. | Attempt creation. | Failure only. | No `VendorRegistered` Domain Event. | Parameterised automation | Ready | Mandatory negative assertion. |

## 5.10 Vendor Lifecycle and Initial State

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-STATE-001 | Vendor begins at success | No Vendor exists before successful registration. | HJ-106; ADR-004 | §§4.1, 4.8 | Domain / Unit | Critical | Pre-registration interaction may exist outside service. | Observe before/after successful command. | Vendor identity begins only after success. | No Draft/Registration Vendor state. | Automate | Ready | Existence boundary. |
| VR-STATE-002 | Initial Vendor State | New Vendor starts `PendingActivation`. | HJ-106 | §§4.7–4.8 | Domain / Unit | Critical | Valid creation. | Create Vendor. | State equals `PendingActivation`. | No Activated or other invented state. | Automate | Ready | Direct creation, not transition from Draft. |
| VR-STATE-003 | Initial Trading Preference | New Vendor starts `Offline`. | HJ-106 | §§4.7–4.8 | Domain / Unit | Critical | Valid creation. | Create Vendor. | Preference equals `Offline`. | No Online Vendor. | Automate | Ready | Vendor cannot trade. |
| VR-STATE-004 | Registration does not activate | Registration neither decides Compliance nor activation eligibility. | HJ-106; ADR-007 | §§4.1, 4.8 | Application / Unit | Critical | Successful registration. | Complete operation. | Only Pending Activation continuation is enabled. | No activation, eligibility decision or trading authorisation. | Automate | Ready | Compliance separation. |

## 5.11 Domain Event Behaviour

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-DOMAIN-EVENT-001 | Successful creation records fact | First successful creation records one internal `VendorRegistered`. | HJ-106; HJ-004 | §§4.7, 4.10 | Domain / Unit | Critical | Valid first processing. | Create Vendor. | Exactly one Domain Event representing completed creation. | No event before successful creation. | Automate | Ready | Internal event. |
| VR-DOMAIN-EVENT-002 | No event on failure | Pre-commit failure records no Domain Event. | HJ-106 | §4.11 | Domain / Unit | Critical | Any validation/Address/invariant failure. | Attempt registration. | Zero Domain Events. | No completed business fact. | Parameterised automation | Ready | Covers all failure classes. |
| VR-DOMAIN-EVENT-003 | Domain payload not over-specified | No minimum internal payload is imposed beyond completed-fact semantics. | HJ-106 | §4.10 | Contract Review / Non-executable | Important | Domain event defined. | Review event contract. | It need not mirror Integration Event. | No invented minimum or infrastructure metadata requirement. | Manual contract review | Ready | Deliberate non-requirement. |
| VR-DOMAIN-EVENT-004 | No repeat event | Replay, conflict and retrieval record no additional Domain Event. | HJ-106 | §§4.9, 4.12 | Domain / Unit | Critical | Prior success exists or query invoked. | Replay, conflict, retrieve. | Event count unchanged. | No duplicate completed fact. | Parameterised automation | Ready | Three stimuli. |

## 5.12 Integration Event Behaviour

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-INTEGRATION-EVENT-001 | Minimum business content | Event includes VendorId, RegisteredAt, state, preference, Legal Operator Type, Trading Characteristics, required Business Address information, Food authority and applicable Primary authority. | HJ-106 | §4.10 | Integration Contract / Integration | Critical | First registration committed. | Derive published contract. | Every specified business element is present and matches committed state. | No omission of a defined minimum element. | Contract automation after dependency | Partially Blocked | CON-020 blocks the concrete schema; minimum business content remains normative. |
| VR-INTEGRATION-EVENT-002 | Deliberate exclusions | Event excludes declarations, full aggregate, persistence metadata and internal Domain Event representation. | HJ-106 | §4.10 | Integration Contract / Integration | Critical | Event produced. | Inspect content. | All excluded categories absent. | No sensitive/internal leakage. | Contract automation after dependency | Partially Blocked | CON-020 blocks concrete contract execution; exclusions can be reviewed now. |
| VR-INTEGRATION-EVENT-003 | Domain/Integration separation | Published event is a derived contract, not assumed identical to the internal event. | HJ-106; ADR-008 | §4.10 | Integration Contract / Integration | Critical | Both representations exist. | Compare their responsibilities and approved translation. | Separation is explicit and translation preserves approved facts. | No internal event serialized by assumption. | Automate/Review after dependency | Partially Blocked | CON-019 and CON-020 block translation placement and published representation. HJ-013 owns structural separation enforcement. |
| VR-INTEGRATION-EVENT-004 | Primary authority conditional payload | Primary Trading Authority appears only where applicable. | HJ-106 | §4.10 | Integration Contract / Integration | Important | Stall and non-Stall registrations. | Produce each event. | Stall includes value; non-Stall has no business value. | No missing Stall authority or invented non-Stall authority. | Parameterised automation after dependency | Partially Blocked | CON-020 blocks representation; null versus omission is also governed by CON-024 where transported through the API. |
| VR-INTEGRATION-EVENT-005 | No event on non-success | Validation, conflict and retrieval publish no Integration Event. | HJ-106 | §§4.9–4.12 | Integration Contract / Integration | Critical | Each non-success/query condition. | Execute each stimulus and publication cycle. | No new event. | No Pending Activation initiation. | Parameterised automation | Ready | Dispatch failure is different: durable item remains. |

## 5.13 Idempotency and Concurrency

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-IDEMP-001 | Identical successful replay | Same identity and semantically identical successful request returns original outcome. | HJ-106; ADR-008 | §4.9 | Application / Unit | Critical | Original success stored. | Replay identical request. | Outcome equals original committed result. | No new Vendor, fact, event, durable publication record or Pending Activation. | Automate after dependency | Partially Blocked | CON-013 and CON-015 leave identity/equivalence and replay storage unresolved; behaviour remains mandatory. |
| VR-IDEMP-002 | Replay after caller uncertainty | Retry after lost/uncertain response converges on original success. | HJ-106; ADR-008 | §§4.9, 4.11 | Application / Unit | Critical | Commit succeeded but caller did not observe response. | Retry safely. | Original outcome returned. | No second registration. | Automate after dependency | Partially Blocked | CON-013 and CON-015 block complete replay implementation; the outcome is normative. |
| VR-IDEMP-003 | Concurrent equivalent requests | Same identity/equivalent information has one processing owner and outcome. | HJ-106 | §4.9 | Persistence Integration / Integration | Critical | Requests begin concurrently. | Release both at the approved concurrency boundary. | Exactly one Vendor, outcome and durable publication record; both requests converge. | No duplicate side effects or split outcomes. | Automate after dependency | Partially Blocked | CON-013, CON-014, CON-016 and CON-028 block concrete concurrency and persistence verification. |
| VR-IDEMP-004 | Different payload conflict | Same identity with semantically different information returns controlled conflict. | HJ-106 | §§4.9, 4.11 | Application / Unit | Critical | Prior success exists. | Reuse identity with changed registration information. | Controlled idempotency-conflict outcome. | No Vendor create/modify, fact, event, durable publication record or Pending Activation. | Automate after dependency | Partially Blocked | CON-013 defines the missing equivalence contract; CON-024 and CON-025 govern proposed transport mapping. |
| VR-IDEMP-005 | Conflict preserves committed Vendor | Conflict leaves all prior committed state unchanged. | HJ-106 | §4.9 | Persistence Integration / Integration | Critical | Prior Vendor snapshot recorded. | Submit conflicting reuse. | Reloaded Vendor equals prior state. | No mutation or additional durable publication state. | Automate after dependency | Partially Blocked | CON-013, CON-015 and CON-028 block concrete equivalence, outcome-store and persistence verification. |
| VR-IDEMP-006 | Registration Session excluded | Idempotency never consults Registration Session state. | HJ-106; ADR-008 | §4.9 | Application / Unit | Critical | Client/BFF may hold session. | Execute first, replay and conflict. | Decisions use complete request identity/equivalence only. | No session lookup or coupling. | Automate | Ready | Service-boundary guard. |

## 5.14 Persistence and Reliable Publication

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-RELIABILITY-001 | Atomic Vendor and publication work | Vendor state and its durable publication obligation commit as one business outcome. | HJ-106; ADR-008 | §§4.7, 4.10 | Persistence Integration / Integration | Critical | Valid first processing. | Complete registration at the approved persistence boundary. | Vendor and exactly one durable publication record are committed together. | No Vendor without publication work or publication work without its Vendor. | Automate after dependency | Partially Blocked | CON-016 and CON-028 block concrete transaction/database proof. HJ-013 owns mechanism-level atomicity and failure injection. |
| VR-RELIABILITY-002 | No partial outcome on atomic-recording failure | Failure to commit the business outcome means registration did not succeed. | HJ-106 | §4.11 | Persistence Integration / Integration | Critical | An approved pre-commit failure can be induced. | Attempt registration. | Controlled retryable failure and no committed registration outcome. | No success response, Vendor, event record or partial publication work. | Automate after dependency | Partially Blocked | CON-016 and CON-028 block concrete rollback points. HJ-013 owns transaction-mechanism failure injection. |
| VR-RELIABILITY-003 | Dispatch failure preserves recoverable obligation | Failed dispatch leaves the Vendor committed and the publication obligation available for independent retry. | HJ-106 | §§4.10–4.11 | Integration Contract / Integration | Critical | Commit succeeded and dispatch subsequently fails. | Observe failure and later retry through the approved publication boundary. | Vendor remains unchanged and the same obligation remains recoverable until successful publication. | No rerun of RegisterVendor or caller instruction to register again. | Automate after dependency | Partially Blocked | CON-018, CON-020 and CON-021 block concrete relay, contract and broker execution. HJ-013 owns relay restart/recovery mechanics. |
| VR-RELIABILITY-004 | Publication retry creates no new business effect | Repeated publication attempts do not create new durable publication records or business facts. | HJ-106; ADR-008 | §4.10 | Integration Contract / Integration | Critical | One committed publication obligation exists. | Exercise retry through the approved publication boundary. | The original obligation is retried and the observable business result remains singular. | No duplicate registration, Domain Event, completed fact or durable publication record. | Automate after dependency | Partially Blocked | CON-018, CON-020 and CON-021 block concrete execution; HJ-013 owns broker and relay mechanics. |
| VR-RELIABILITY-005 | Success only after commit | Synchronous registration success is confirmed only after atomic commit. | HJ-106 | §§4.7, 4.11 | Application / Unit | Critical | Commit delayed/fails. | Observe response timing/outcome. | Success follows successful commit; failure otherwise. | No partial success or optimistic success. | Automate | Ready | Observable application behaviour. |

## 5.15 Registered Vendor Retrieval

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-RETRIEVE-001 | Retrieve by VendorId | VendorId is the sole lookup criterion. | HJ-106 | §§4.12.1–4.12.3 | Application / Unit | Important | Existing Vendor. | Invoke `RetrieveRegisteredVendor(VendorId)`. | Repository loads exactly that Vendor. | No search, filtering, paging or name lookup. | Automate | Ready | Trusted Administrator premise; Identity outside scope. |
| VR-RETRIEVE-002 | Persisted aggregate authoritative | Result maps current persisted Vendor state. | HJ-106; HJ-004 | §4.12.2 | Persistence Integration / Integration | Critical | Persisted Vendor with representative values. | Retrieve Vendor. | Details match persisted state. | No Address/Compliance/Identity call or stale separate projection. | Automate | Partially Blocked | Aggregate itself not exposed. CON-028 blocks concrete PostgreSQL mapping verification. |
| VR-RETRIEVE-003 | Complete Registered Information | Details contain every listed Registered Information element. | HJ-106 | §4.12.2 | Application / Unit | Important | Vendor has applicable and inapplicable conditional fields. | Retrieve representative Vendors. | All applicable persisted registered fields returned accurately. | No required field omitted or invented. | Parameterised automation | Partially Blocked | CON-009 leaves the Address-owned snapshot schema opaque; CON-024 governs wire representation. |
| VR-RETRIEVE-004 | Managed information representation | Website and Description appear where supplied and remain absent where not supplied. | HJ-106 | §4.12.2 | Application / Unit | Important | Vendors with each optional combination. | Retrieve each. | Result reflects persisted presence/absence. | No defaults or mutation. | Parameterised automation | Ready | Null/omission transport convention excluded. |
| VR-RETRIEVE-005 | Retrieval exclusions | Details exclude declarations, Compliance/Activation decisions, events, publication metadata, Identity and persistence metadata. | HJ-106 | §4.12.2 | Application / Unit | Critical | Existing Vendor and infrastructure records. | Retrieve. | All excluded categories absent. | No aggregate or internal representation leakage. | Automate | Ready | Security and boundary assertion. |
| VR-RETRIEVE-006 | Read-only invariants | Retrieval changes no Vendor or managed information and records/publishes nothing. | HJ-106 | §4.12.3 | Persistence Integration / Integration | Critical | Snapshot state and event counts. | Retrieve repeatedly. | State and counts unchanged. | No lifecycle transition, Domain Event, work item, Integration Event or Pending Activation. | Automate | Partially Blocked | Includes no re-resolution/cross-domain call. CON-028 blocks concrete PostgreSQL mapping verification. |
| VR-RETRIEVE-007 | Vendor Not Found | Unknown VendorId returns controlled Not Found outcome. | HJ-106 | §4.12.3 | Application / Unit | Important | No matching Vendor. | Retrieve unknown ID. | Controlled Vendor Not Found. | No create, mutation, event, publication or cross-domain call. | Automate | Ready | HTTP 404 is proposed only. |
| VR-RETRIEVE-008 | No dedicated read model required | Epic 1 retrieval works without projection infrastructure. | HJ-106 | §§4.12.2–4.12.3 | Contract Review / Non-executable | Standard | Architecture reviewed. | Inspect query design. | Repository/aggregate source is sufficient. | No invented eventual-consistency dependency. | Manual contract review | Ready | Future optimisation not prohibited. |

## 5.16 Business Failures

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-FAILURE-001 | Request validation failure | Missing/malformed/out-of-range input returns correctable failure. | HJ-106 | §4.11 | Application / Unit | Important | Invalid request. | Submit. | Controlled validation outcome and correction guidance. | No Vendor/fact/events/publication/Pending Activation. | Parameterised automation | Ready | Field cases above supply triggers. |
| VR-FAILURE-002 | Declaration failure | Missing/false declaration returns acceptance-required failure. | HJ-106 | §4.11 | Application / Unit | Critical | One declaration not true. | Submit. | Controlled declaration failure; retry after acceptance. | No business side effect. | Parameterised automation | Ready | No declaration persistence. |
| VR-FAILURE-003 | Conditional rule failure | Controlling type/location and conditional value conflict. | HJ-106 | §4.11 | Application / Unit | Critical | Invalid CRN or authority combination. | Submit. | Controlled conditional failure; correction/re-resolution guidance. | No business side effect. | Parameterised automation | Ready | Both invariants. |
| VR-FAILURE-004 | Address Resolution failure | Untrusted/unresolved Address prevents registration. | HJ-106 | §4.11 | Application / Unit | Critical | Address reference fails. | Submit. | Controlled Address failure; reselection/resolution guidance. | No business side effect. | Parameterised after contract | Partially Blocked | CON-009 and CON-010 block exhaustive reference and failure cases. |
| VR-FAILURE-005 | Aggregate invariant failure | Domain rejection is returned as controlled business failure. | HJ-106 | §4.11 | Application / Unit | Critical | Valid-looking input violates invariant. | Submit. | Correctable business outcome where applicable. | No technical exception exposed; no side effect. | Parameterised automation | Ready | Safe translation required. |
| VR-FAILURE-006 | Persistence/atomic failure | Failed atomic commit is retryable through the idempotency safeguard. | HJ-106 | §4.11 | Persistence Integration / Integration | Critical | An approved pre-commit failure can be induced. | Register. | Failure, safe retry guidance and no registered outcome. | No partial Vendor or durable publication record. | Automate after dependency | Partially Blocked | CON-016 and CON-028 block concrete failure injection; behavioural coverage complements HJ-013 mechanism verification. |
| VR-FAILURE-007 | Equivalent duplicate convergence | Equivalent duplicate is original success, not failure. | HJ-106 | §4.11 | Application / Unit | Critical | Prior identical success. | Replay. | Original outcome returned. | No duplicate business effect or error requiring new registration. | Automate after dependency | Partially Blocked | CON-013 and CON-015 leave identity/equivalence and stored replay outcome unresolved. |
| VR-FAILURE-008 | Dispatch failure after commit | Dispatch failure delays notification but registration remains successful. | HJ-106 | §4.11 | Integration Contract / Integration | Critical | Commit complete. | Fail external dispatch through the approved publication boundary. | Vendor remains registered and the publication obligation remains recoverable. | No caller instruction to register again, registration rerun or rollback. | Automate after dependency | Partially Blocked | CON-018, CON-020 and CON-021 block concrete execution; HJ-013 owns relay/broker recovery mechanics. |
| VR-FAILURE-009 | Idempotency conflict | Different information under same identity returns non-retry conflict. | HJ-106 | §4.11 | Application / Unit | Critical | Prior success. | Submit conflicting payload. | Controlled conflict; caller uses corrected/new approved identity convention. | No state change or event/publication/Pending Activation. | Automate after dependency | Partially Blocked | CON-013 governs equivalence; CON-024 and CON-025 govern the proposed HTTP mapping. |

All controlled errors must be client-safe and omit stack traces, database details, internal class names and framework diagnostics.

## 5.17 Scope Exclusions and Prohibited Behaviour

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-SCOPE-001 | No Registration Session service | Vendor service exposes no session creation, retrieval, expiry or disposal. | HJ-106 | §§4.1, 5 | Contract Review / Non-executable | Critical | Service surface reviewed. | Inspect operations/dependencies. | Only complete RegisterVendor request is processed. | No Vendor-owned session state. | Manual contract review | Ready | Client/BFF may own interaction state. |
| VR-SCOPE-002 | No Identity behaviour | Registration/retrieval introduces no authentication, authorisation or ownership-check contract. | HJ-106 | §§4.1, 4.12, 5 | Contract Review / Non-executable | Important | Epic 1 contract reviewed. | Inspect inputs, collaborators and outcomes. | Identity remains outside scope. | No invented account/caller association tests. | Manual contract review | Ready | Future Identity work is recorded as a scope exclusion in VR-BLOCKED-003 and does not block Epic 1. |
| VR-SCOPE-003 | No Compliance or activation decision | Registration publishes facts but does not determine Compliance or activate. | HJ-106 | §§4.4, 4.8, 5 | Application / Unit | Critical | Successful registration. | Observe collaborators and state. | Pending Activation only. | No evidence/state decision or synchronous callback. | Automate | Ready | ADR-007. |
| VR-SCOPE-004 | No unsupported queries | Only direct VendorId retrieval exists. | HJ-106 | §§4.12, 5 | Contract Review / Non-executable | Important | Service surface reviewed. | Search for contract operations. | Register and Retrieve only for this slice. | No search, list, filter, page or name lookup. | Manual contract review | Ready | No collection endpoint proposed. |
| VR-SCOPE-005 | Deferred domains remain absent | Multiple premises, Menu, Ordering, Payments, Delivery and post-registration management do not enter this contract. | HJ-106 | §5 | Contract Review / Non-executable | Standard | Catalogue and service reviewed. | Trace every obligation. | No tests prescribe deferred behaviour. | No scope expansion. | Manual contract review | Ready | Completeness guard. |

# 6. Proposed API Contract Tests

> **Non-normative technical conventions pending approval.** Every obligation in this section is derived only from HJ-106 Part B and must not be treated as an approved business requirement.

| Test ID | Title | Requirement | Source | Section | Classification / Level | Priority | Preconditions | Input / Stimulus | Expected Result | Prohibited Outcome | Automation | Dependency Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VR-API-001 | Register route | Proposed `POST /vendors` reaches RegisterVendor. | HJ-106 Part B | §6.1 | API Contract / API Integration | Proposed | Convention approved. | POST proposed request. | Request maps to business operation. | No extra operation invented. | Deferred | Proposed pending approval | Route not normative. CON-024 governs the unresolved technical API contract. |
| VR-API-002 | Retrieve route | Proposed `GET /vendors/{vendorId}` reaches retrieval. | HJ-106 Part B | §6.1 | API Contract / API Integration | Proposed | Convention approved. | GET known/unknown ID. | Maps VendorId and returns business outcome. | No collection/search route. | Deferred | Proposed pending approval | Route not normative. CON-024 governs the unresolved technical API contract. |
| VR-API-003 | Register request shape | Proposed JSON maps all client-authored fields and excludes Address-owned values. | HJ-106 Part B | §6.2 | API Contract / API Integration | Proposed | JSON schema approved. | Submit representative body. | Values map without weakening Part A. | No authoritative snapshot/authority input. | Deferred | Proposed pending approval | Nesting/enum spellings pending. CON-024 and CON-026 govern request representation and validation allocation. |
| VR-API-004 | Register success response | Proposed first success is `201` with VendorId, RegisteredAt, state and preference. | HJ-106 Part B | §§6.3, 6.5 | API Contract / API Integration | Proposed | Convention approved. | Successful POST. | Proposed body/status returned from committed outcome. | No response before commit. | Deferred | Proposed pending approval | Only VendorId/state are business-minimum. CON-024 governs the unresolved technical API contract. |
| VR-API-005 | Replay response | Proposed identical replay preserves original `201` response/body. | HJ-106 Part B | §§6.3, 6.5 | API Contract / API Integration | Proposed | Replay convention approved. | Repeat same key/body. | Same proposed outcome. | No second Vendor. | Deferred | Proposed pending approval | `200` alternative remains open. CON-024 governs the unresolved technical API contract. |
| VR-API-006 | Retrieve success response | Proposed `200` body represents Registered Vendor Details. | HJ-106 Part B | §§6.3, 6.5 | API Contract / API Integration | Proposed | Serialization approved. | GET existing Vendor. | Proposed fields map persisted details. | No aggregate/internal metadata exposure. | Deferred | Proposed pending approval | Snapshot schema and null rules blocked. CON-024 governs the unresolved technical API contract. |
| VR-API-007 | Controlled error envelope | Proposed errors use code, message, optional validation errors and correlation ID. | HJ-106 Part B | §6.4 | API Contract / API Integration | Proposed | Error vocabulary approved. | Trigger each business failure. | Client-safe proposed envelope. | No stack/database/class/framework diagnostics. | Deferred | Proposed pending approval | Vocabulary/path conventions open. CON-024 and CON-025 govern the unresolved error and transport mapping. |
| VR-API-008 | Not Found mapping | Proposed missing Vendor maps to `404` and safe error. | HJ-106 Part B | §§6.4–6.5 | API Contract / API Integration | Proposed | Convention approved. | GET unknown ID. | `404` proposed envelope. | No Vendor creation or internal leakage. | Deferred | Proposed pending approval | Business Not Found is normative; HTTP mapping is not. CON-024 governs the unresolved technical API contract. |
| VR-API-009 | Conflict mapping | Proposed idempotency conflict maps to `409`. | HJ-106 Part B | §§6.4–6.5 | API Contract / API Integration | Proposed | Convention approved. | Reuse key with different information. | `409` proposed envelope. | No success or side effect. | Deferred | Proposed pending approval | Business conflict is normative. CON-024 and CON-025 govern the unresolved error and transport mapping. |
| VR-API-010 | Validation/status alternatives | Proposed malformed/invalid cases use selected `400`/`422`; temporary dependency may use `503`. | HJ-106 Part B | §6.5 | API Contract / API Integration | Proposed | One mapping is approved. | Trigger mapped cases. | Approved status used consistently. | No catalogue selection of unresolved alternative. | Deferred | Proposed pending approval | `422` explicitly unselected. CON-024 governs the unresolved technical API contract. |
| VR-API-011 | Required/proposed headers | Content negotiation, Idempotency-Key, Location and correlation headers follow approved convention. | HJ-106 Part B | §6.6 | API Contract / API Integration | Proposed | Header contract approved. | Send/receive headers. | Direction and purpose match approved technical contract. | No idempotency header required for GET. | Deferred | Proposed pending approval | Names/generation rules open. CON-024 governs the unresolved technical API contract. |
| VR-API-012 | Optional/null serialization | Conditional and optional fields follow approved omission/null policy. | HJ-106 Part B | §§6.3, 6.6 | API Contract / API Integration | Proposed | Serialization decision exists. | Serialize applicable/inapplicable variants. | Exact approved representation used. | No inferred convention. | Deferred | Blocked | Applies CRN, authority, Website, Description, snapshot. CON-024 governs the unresolved technical API contract. |

# 7. Blocked and Deferred Tests

| Test Area | Affected Test IDs | Missing Contract or Decision | Current Concern ID(s) | Authoritative Owner / Source Required | What Can Already Be Tested | What Remains Blocked | Required Resolution | Implementation / Readiness Impact |
|---|---|---|---|---|---|---|---|---|
| VR-BLOCKED-001 Address Resolution reference | VR-REQ-004; VR-ADDRESS-001, VR-ADDRESS-008; VR-FAILURE-004 | Format, lifetime, reuse, revocation, expiry and failure taxonomy | CON-009; CON-010 | Address consumed contract / Address capability | Reference-only trust boundary and no-partial-registration guarantee | Exhaustive reference validation, expiry, retry and failure cases | Approve the consumed Address contract and failure taxonomy | Affected behavioural cases remain Partially Blocked. |
| VR-BLOCKED-002 Integration Event contract | VR-INTEGRATION-EVENT-001–004; VR-DECL-004 | Concrete Business Address representation, metadata/envelope and compatibility rules | CON-019; CON-020 | Vendor published contract with Address/Compliance requirements | Minimum business content, deliberate exclusions and Domain/Integration separation can be reviewed | Field-level schema, serialization, translation and compatibility execution | Approve translation placement and the versioned Integration Event contract | Integration Contract automation remains Partially Blocked. |
| VR-BLOCKED-003 Identity scope exclusion | VR-SCOPE-002 | Authentication, authorisation and caller-to-Vendor association are outside Epic 1 | None; explicitly excluded by HJ-106 | Future Identity/service-boundary contract | Verify that Epic 1 introduces no Identity behaviour | Future access-control behaviour cannot be derived in this catalogue | No Epic 1 resolution required; approve separately when Identity enters scope | Does not block RegisterVendor or RetrieveRegisteredVendor behavioural coverage. |
| VR-BLOCKED-004 Telephone validation | VR-CONTACT-003 | Exact accepted UK telephone formats | CON-026 | Approved business validation source and validation-allocation decision | Presence and requirement ownership | Exhaustive accepted/rejected formats and canonicalisation | Approve formats, boundaries and owning validation layer | VR-CONTACT-003 remains Blocked. |
| VR-BLOCKED-005 Idempotency and transaction mechanics | VR-REQ-005; VR-IDEMP-001–005; VR-RELIABILITY-001–002; VR-FAILURE-006 | Identity/equivalence, concurrency, outcome storage, retention and transaction boundary | CON-013–CON-016 | Vendor architecture and technical idempotency contract | Replay, conflict, non-duplication and no-partial-outcome guarantees are normative | Exhaustive equivalence, retention, concurrency and transaction execution | Approve the four Current Concern resolutions without Registration Session dependence | Behavioural catalogue is complete, but affected automation remains Partially Blocked. |
| VR-BLOCKED-006 Technical representation and serialization | VR-API-001–006, VR-API-008, VR-API-010–012; applicable schema cases | Routes, identifiers, timestamps, time, headers and null/omission representations | CON-024 | Approved technical service/API contract | Part A behaviour and Proposed examples remain reviewable | Normative API Contract automation and exact schemas | Approve the technical API/OpenAPI and serialization contract | Proposed API tests cannot become normative automation. |
| VR-BLOCKED-007 Error and HTTP conventions | VR-API-007–010; applicable failure tests | Error codes, field paths, correlation rules and status mappings | CON-024; CON-025 | Approved technical API and error-mapping contract | Business failures and prohibited side effects remain testable below transport | Normative error-envelope, header and HTTP mapping assertions | Approve error catalogue and transport mappings | Proposed API failure tests remain non-normative. |
| VR-BLOCKED-008 Publication relay and broker behaviour | VR-RELIABILITY-003–004; VR-FAILURE-008; applicable Integration Event tests | Relay, retry/restart policy, published representation and broker delivery semantics | CON-018–CON-021 | Vendor publication architecture and approved published contract | Registration success, durable obligation, non-duplication and caller behaviour | Concrete relay recovery, broker delivery and poison/retry execution | Resolve relay, translation, event-contract and broker concerns | HJ-107 remains behavioural; HJ-013 owns concrete relay/broker mechanism verification. |
| VR-BLOCKED-009 PostgreSQL mapping and constraints | VR-SUCCESS-002–003, VR-LEGAL-007, VR-CONTACT-004, VR-MANAGED-005, VR-DECL-002, VR-ADDRESS-005, VR-IDEMP-003/005, VR-RELIABILITY-001–002, VR-RETRIEVE-002/006, VR-FAILURE-006 | Concrete aggregate, idempotency and publication-work mappings, keys and constraints | CON-028 | Vendor persistence architecture | Required persisted outcomes and exclusions are fully catalogued | Real PostgreSQL mapping, constraint and rehydration execution | Approve PostgreSQL mapping and constraint treatment | Persistence Integration automation remains Partially Blocked; HJ-013 owns concrete mapping verification. |

# 8. Requirement-to-Test Traceability Matrix

| Requirement / Contract Element | HJ-106 Section | Upstream Source | Test IDs | Coverage | Notes |
|---|---|---|---|---|---|
| Complete self-contained request and no Registration Session | §§4.1–4.2 | ADR-004; HJ-104; HJ-105 | VR-REQ-001, VR-SCOPE-001, VR-IDEMP-006 | Covered | Session is outside every service boundary. |
| Preconditions and mandatory information | §§4.2–4.3 | HJ-104 §2 | VR-REQ-002–006 | Covered/partly blocked | Telephone detail blocked. |
| Trading and Legal Operator names | §4.3 | HJ-104 §§2, 5.2 | VR-LEGAL-002–003 | Covered | Boundary coverage. |
| Legal Operator Type and Company Registration Number | §§4.3–4.5 | HJ-104 §§5.1–5.3; HJ-004 §8 | VR-LEGAL-001, VR-LEGAL-004–007 | Covered | Includes iff and canonicalisation. |
| Trading Characteristics | §§4.3, 4.5 | HJ-003; HJ-104 §§2, 5.1–5.2 | VR-TRADING-001–005 | Covered/partly blocked | Wire time format not made normative. |
| Primary Contact | §4.3 | HJ-104 §§2, 5.2 | VR-CONTACT-001–004 | Covered/blocked | Telephone contract required. |
| Vendor Managed Information | §§4.3, 4.7 | ADR-005; HJ-104 §5.5 | VR-MANAGED-001–005 | Covered | Presence and absence included. |
| Registration Declarations | §§4.2–4.5, 4.10 | HJ-104 §2, §5.5; HJ-004 | VR-DECL-001–005 | Covered | Acceptance, transience and exclusions. |
| Address request trust boundary | §§4.3, 4.6 | ADR-006; HJ-104 §5.4; HJ-105 | VR-ADDRESS-001–003, VR-REQ-004 | Covered/partly blocked | Exact reference contract open. |
| Derived Address values and persistence | §§4.4–4.6 | ADR-006; HJ-004 | VR-ADDRESS-004–007 | Covered/partly blocked | Snapshot treated opaquely. |
| Vendor creation invariants | §4.5 | HJ-004 §8; HJ-104 §5.1 | VR-INV-001–005 plus VR-LEGAL-004–005 and VR-ADDRESS-007 | Covered | Positive and negative. |
| First successful outcome | §4.7 | HJ-004; HJ-105 | VR-SUCCESS-001–005 | Covered | Business-minimum response separated from Part B. |
| Vendor existence and initial state | §4.8 | ADR-004; ADR-007 | VR-STATE-001–004 | Covered | No activation. |
| Idempotent replay, concurrency and conflict | §4.9 | ADR-008; HJ-105 §9 | VR-IDEMP-001–006 | Covered/partly blocked | Behaviour is complete; CON-013–CON-016 and CON-028 block concrete mechanism execution. |
| Internal Domain Event | §4.10 | HJ-004; ADR-008 | VR-DOMAIN-EVENT-001–004 | Covered | No invented minimum payload. |
| Published Integration Event | §4.10 | HJ-004; HJ-105; ADR-008 | VR-INTEGRATION-EVENT-001–005 | Covered/partly blocked | CON-019 and CON-020 block translation and concrete published representation. |
| Atomic persistence and publication recovery | §§4.7, 4.10–4.11 | ADR-008; HJ-105 §10 | VR-RELIABILITY-001–005 | Covered/partly blocked | Behaviour is complete; CON-016, CON-018, CON-020, CON-021 and CON-028 block mechanism execution. |
| Request validation failure | §4.11 | HJ-104 | VR-FAILURE-001 | Covered | Field triggers mapped above. |
| Declaration failure | §4.11 | HJ-104 | VR-FAILURE-002 | Covered | Explicit acceptance. |
| Conditional failure | §4.11 | HJ-104; HJ-004 | VR-FAILURE-003 | Covered | CRN and authority. |
| Address failure | §4.11 | ADR-006; HJ-105 | VR-FAILURE-004, VR-ADDRESS-008 | Partly blocked | Taxonomy unresolved. |
| Aggregate failure | §4.11 | HJ-004 | VR-FAILURE-005 | Covered | Safe controlled outcome. |
| Persistence failure | §4.11 | ADR-008 | VR-FAILURE-006, VR-RELIABILITY-002 | Covered/partly blocked | Retry guarantee is normative; CON-016 and CON-028 block failure-injection execution. |
| Equivalent duplicate and dispatch failure | §4.11 | ADR-008 | VR-FAILURE-007–008 | Covered/partly blocked | CON-018, CON-020 and CON-021 block concrete dispatch execution. |
| Idempotency conflict | §4.11 | ADR-008 | VR-FAILURE-009, VR-IDEMP-004–005 | Covered/partly blocked | HTTP mapping proposed only. |
| Client-safe controlled errors | §4.11 | HJ-005 | VR-FAILURE-001–009, VR-API-007 | Covered/proposed | Internal diagnostics prohibited. |
| Retrieval intent and VendorId lookup | §4.12.1 | HJ-003; HJ-004; HJ-105 | VR-RETRIEVE-001 | Covered | Trusted actor premise; Identity excluded. |
| Retrieval source and Registered Vendor Details | §4.12.2 | HJ-004 | VR-RETRIEVE-002–005 | Covered/partly blocked | Snapshot wire schema open. |
| Retrieval outcomes and no side effects | §4.12.3 | HJ-004; HJ-105 | VR-RETRIEVE-006–008 | Covered | Includes Not Found. |
| Information and operations outside scope | §5 | ADR-002–ADR-007 | VR-SCOPE-001–005 | Covered | Negative contract review. |
| Proposed HTTP representation | §6 | HJ-005 | VR-API-001–012 | Proposed/blocked | Not normative until approved. |

# 9. Completeness Analysis

- Every normative HJ-106 Part A operation, precondition, field category, creation invariant, outcome, event rule, failure, retrieval rule and scope exclusion maps to at least one Test ID in Section 8.
- Requirements covered at multiple levels are intentionally split by observable boundary: for example, conditional creation rules at Domain level, persistence at Persistence Integration level and proposed transport mapping at API Contract level.
- Domain/Integration Event separation, no Registration Session dependence, and the absence of prohibited business effects are repeated where distinct stimuli could regress independently.
- The Business Address payload schema, Address reference lifecycle, telephone formats, idempotency/transaction mechanics, publication mechanics, API contract and PostgreSQL mappings are explicitly linked to their Current Concerns rather than inferred.
- Identity authentication, authorisation and caller association are scope exclusions, not blockers for the approved Epic 1 operations.
- The absence of a prescribed internal Domain Event payload and the absence of dedicated read-model infrastructure are deliberate non-requirements verified by Contract Review rather than executable schema tests.
- Part B obligations are wholly separated in Section 6 and carry Proposed or Blocked status.
- Potential overlap exists between failure rows and their triggering field/invariant rows; this is intentional because the former verifies orchestration and prohibited side effects while the latter verifies the lowest responsible rule boundary.
- HJ-107 owns behavioural outcomes. Complementary dependency enforcement, concrete mapping, Transactional Outbox, transaction failure-injection, relay recovery and broker-mechanism obligations remain in HJ-013.
- No source ambiguity changes approved business behaviour. Open technical choices and their Current Concern references are listed in Sections 7 and 10.

# 10. Assumptions and Open Questions

| Classification | Item | Treatment |
|---|---|---|
| Confirmed | HJ-106 v1.1 Part A is the primary normative behavioural basis. | All normative tests trace to it; its v1.1 alignment introduced no service-behaviour change. |
| Confirmed | HJ-006 v2.0 approves the detailed classifications used here. | Every executable classification maps to one approved Test Level. |
| Confirmed | `RegisterVendor` and `RetrieveRegisteredVendor` are the only Epic 1 operations in scope. | No additional query or command tests are invented. |
| Test Design Convention | Parameterized obligations represent equivalent values/boundaries without hiding distinct outcomes. | Implementations may split them while retaining stable traceability. |
| Test Design Convention | “Exactly one” assertions include persisted aggregate, completed fact and durable publication work where applicable. | Prevents partial or duplicate effects. |
| Technical Convention | Test framework and test-harness details remain implementation choices. | Catalogue states observable obligations only. Architectural mechanisms recorded as Current Concerns are not dismissed as arbitrary conventions. |
| Blocked Dependency | Address reference and failure taxonomy are incomplete. | CON-009 and CON-010; see VR-BLOCKED-001. |
| Blocked Dependency | Integration Event translation and concrete published contract are incomplete. | CON-019 and CON-020; see VR-BLOCKED-002. |
| Scope Exclusion | Identity behaviour is outside Epic 1. | See VR-SCOPE-002 and VR-BLOCKED-003; it does not block the approved operations. |
| Blocked Dependency | Idempotency, concurrency, outcome storage and transaction mechanics are unresolved. | CON-013–CON-016; see VR-BLOCKED-005. |
| Blocked Dependency | Publication relay and broker semantics are unresolved. | CON-018–CON-021; see VR-BLOCKED-008. |
| Blocked Dependency | PostgreSQL mappings and constraints are unresolved. | CON-028; see VR-BLOCKED-009. |
| Missing Information | Exact UK telephone rule is absent. | See VR-BLOCKED-004. |
| Missing Information | API serialization, errors, statuses and headers lack approval. | CON-024–CON-026; Proposed tests remain non-normative. |
| Ambiguity | Caller-authored Address values may be rejected or ignored. | Tests require that they never become authoritative; exact transport outcome awaits technical contract. |
| Ambiguity | Optional/inapplicable response fields may be omitted or null. | Business presence/absence is tested; wire representation remains blocked. |
| Artefact Conflict | None identified among the approved/accepted sources. | No conflict resolution was required. |

# 11. Review Checklist

- [x] Covers every normative statement in HJ-106 Part A through the traceability matrix.
- [x] Uses HJ-106 v1.1 Part A as the primary normative behavioural basis.
- [x] Uses the approved HJ-006 Test Classification and Test Level mapping.
- [x] Preserves stable Test IDs and records their regeneration reconciliation.
- [x] Keeps Priority distinct from Dependency Status.
- [x] Identifies applicable HJ-010 Current Concern dependencies.
- [x] Selects no unresolved architectural Approach.
- [x] Does not duplicate HJ-013 architecture or implementation obligations.
- [x] Treats HJ-012 as approved architecture rather than a source of new service behaviour.
- [x] Treats Identity as outside Epic 1 rather than as a blocker.
- [x] Preserves bounded-context ownership.
- [x] Uses approved ubiquitous language.
- [x] Covers every HJ-104 field rule, including boundaries and conditional rules.
- [x] Covers every HJ-004 creation invariant.
- [x] Covers RegisterVendor and RetrieveRegisteredVendor requirements.
- [x] Covers every HJ-106 business failure and its prohibited side effects.
- [x] Covers identical replay, conflicting reuse and concurrent submission.
- [x] Covers atomic Vendor persistence and durable publication work.
- [x] Keeps Domain and Integration Events distinct.
- [x] Does not invent unresolved Address, Compliance, Identity, idempotency or transport contracts.
- [x] Separates HJ-106 Part A business tests from Part B proposed API tests.
- [x] Keeps all Part B-derived tests Proposed and non-normative.
- [x] Identifies blocked dependencies explicitly.
- [x] Provides complete requirement-to-test traceability.
- [x] Contains no executable test code or implementation-specific test design.

# 12. Regeneration Reconciliation

All v0.1 behavioural Test IDs are retained. No existing obligation is removed, merged, split, superseded or assigned to a different business behaviour.

| Test ID | Previous Treatment | Regenerated Treatment | Reason |
|---|---|---|---|
| VR-SUCCESS-001–005 | Five behavioural obligations | Retained; HJ-006 Classification / Level made explicit; persistence and durable-publication dependencies added where applicable | HJ-106 v1.1 changes traceability, not success behaviour. |
| VR-REQ-001–006 | Six behavioural obligations | Retained; CON-013 added to the idempotency-safeguard dependency | Preserve complete-request coverage without selecting the safeguard. |
| VR-LEGAL-001–007 | Seven behavioural obligations | Retained; CON-028 added to canonical persistence verification | Preserve business rules while exposing the concrete mapping dependency. |
| VR-TRADING-001–005 | Five behavioural obligations | Retained; CON-024 added to concrete time representation | Separate Domain-valid time behaviour from proposed wire format. |
| VR-CONTACT-001–004 | Four behavioural obligations | Retained; CON-026 and CON-028 dependencies added | Preserve approved contact behaviour without inventing validation allocation or persistence mapping. |
| VR-MANAGED-001–005 | Five behavioural obligations | Retained; CON-028 added to persistence verification | Preserve information classification and expose mapping dependency. |
| VR-DECL-001–005 | Five behavioural/review obligations | Retained; CON-028 added to persisted-exclusion proof | Preserve declaration transience and distinguish concrete persistence verification. |
| VR-ADDRESS-001–008 | Eight behavioural obligations | Retained; CON-009, CON-010 and CON-028 dependencies made explicit | Preserve trust-boundary behaviour while deferring unresolved Address and mapping detail. |
| VR-INV-001–005 | Five Domain obligations | Retained unchanged apart from authoritative Classification / Level notation | Approved invariant behaviour is unchanged. |
| VR-STATE-001–004 | Four Domain/Application obligations | Retained unchanged apart from authoritative Classification / Level notation | Approved lifecycle behaviour is unchanged. |
| VR-DOMAIN-EVENT-001–004 | Four Domain/review obligations | Retained unchanged apart from authoritative Classification / Level notation | Internal completed-fact behaviour is unchanged. |
| VR-INTEGRATION-EVENT-001–005 | Five behavioural contract obligations | Retained; CON-019 and CON-020 dependencies added; structural enforcement left to HJ-013 | Preserve published-event behaviour without inventing translation or schema. |
| VR-IDEMP-001–006 | Six behavioural obligations | Retained; CON-013–CON-016 and CON-028 dependencies made explicit; durable-publication terminology aligned | Preserve replay, conflict and concurrency outcomes without selecting mechanisms. |
| VR-RELIABILITY-001–005 | Five behavioural obligations | Retained; narrowed to service-observable outcomes and linked to CON-016, CON-018, CON-020, CON-021 and CON-028; mechanism proof assigned to HJ-013 | Enforce the HJ-107/HJ-013 boundary without losing behavioural coverage. |
| VR-RETRIEVE-001–008 | Eight behavioural obligations; three v0.1 rows lacked an explicit Priority | Retained; CON-009, CON-024 and CON-028 dependencies made explicit; VR-RETRIEVE-004, VR-RETRIEVE-005 and VR-RETRIEVE-007 assigned Important, Critical and Important respectively | Preserve retrieval behaviour, repair required catalogue structure and expose contract/mapping dependencies. |
| VR-FAILURE-001–009 | Nine behavioural obligations | Retained; affected Address, idempotency, persistence, relay and transport dependencies added | Preserve every HJ-106 failure and prohibited outcome. |
| VR-SCOPE-001–005 | Five behavioural/review obligations | Retained; Identity clarified as an Epic 1 scope exclusion rather than a blocker | Align with HJ-106 v1.1 and PR-004 authority rules. |
| VR-API-001–012 | Twelve Proposed obligations | Retained; Classification / Level mapping made explicit and CON-024–CON-026 added | Preserve Part B proposals without promoting them to normative tests. |
| VR-BLOCKED-001–007 | Seven dependency records | Retained and expanded with affected Test IDs, Current Concern references, testable boundary and blocked boundary | Make unresolved authority and readiness impact explicit. |
| VR-BLOCKED-008 | No v0.1 entry | Added for publication relay and broker dependencies affecting existing reliability tests | HJ-010 now identifies CON-018–CON-021 explicitly. |
| VR-BLOCKED-009 | No v0.1 entry | Added for PostgreSQL mapping and constraint dependencies affecting existing Persistence Integration tests | HJ-010 now identifies CON-028 explicitly. |

Regeneration totals:

- 108 retained behavioural, proposed or review Test IDs;
- 7 retained dependency-record IDs;
- 2 new dependency-record IDs; and
- 0 removed, merged, split, superseded or reused IDs.

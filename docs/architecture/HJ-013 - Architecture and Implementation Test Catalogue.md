# HJ-013 - Architecture and Implementation Test Catalogue

| Field | Value |
|---|---|
| **Document ID** | HJ-013 |
| **Document Title** | Architecture and Implementation Test Catalogue |
| **Version** | 2.0 |
| **Status** | Approved |
| **Classification** | Test Catalogue |
| **Owner** | Project Architecture / Engineering |
| **Last Updated** | 26 August 2026 |

## Revision History

| Version | Date | Description |
|---|---|---|
| 0.1 | 14 August 2026 | Initial draft derived from the first HJ-012 approved architecture batch: CON-001 to CON-005, CON-017 and CON-027. |
| 0.2 | 14 August 2026 | Regenerated against HJ-004 v2.3, HJ-106 v1.1 and HJ-107 v0.2 following the Architecture Verification Gap Analysis. Preserves stable `AI-*` IDs, removes behavioural duplication, separates persistence responsibilities, identifies applicable Epic 1 Value Objects, adds CON-037 enforcement dependencies and narrows Entity verification to approved Epic 1 behaviour. |
| 0.3 | 15 August 2026 | Regenerated using PR-005 under CR-043. Adds explicit source authority, delivery scope, concern traceability, generation readiness and reconciliation; corrects AI-AGG-003 and AI-VO-002; preserves all 24 stable `AI-*` IDs and all seven Approved concerns; and changes no HJ-107 behavioural ID. |
| 1.0 | 15 August 2026 | Approved the regenerated Architecture and Implementation Test Catalogue as the controlled architecture-verification baseline. Promotion preserves all 24 stable `AI-*` IDs and all seven Approved concern mappings. HJ-107 is aligned to its v1.0 Approved publication baseline with no behavioural-reference change. |
| 1.1 | 17 August 2026 | Applied CR-055 using PR-005. Preserved all existing `AI-*` IDs, added AI-ADDR-001–007 and AI-APP-001 for CON-006–CON-011, and removed resolved Address dependencies. |
| 1.2 | 17 August 2026 | Regenerated using PR-005 from HJ-012 v1.2, HJ-011 v1.2, HJ-106 v1.3 and HJ-107 v1.2 Approved. Preserved all 32 existing `AI-*` IDs, aligned the positional Address mapping, corrected verification-form/classification defects and added AI-APP-002 to separate application-layer enforcement from orchestration behaviour. |
| 1.3 | 18 August 2026 | Regenerated using PR-005 from HJ-010 v1.3, HJ-011 v1.3 and HJ-012 v1.3 Approved, with HJ-106 v1.3 and HJ-107 v1.2 Approved. Preserved all 33 existing `AI-*` IDs and added AI-APP-003 for the approved CON-012 registration-intent representation. |
| 1.4 | 19 August 2026 | Regenerated using PR-005 from HJ-010 v1.4, HJ-011 v1.4 and HJ-012 v1.4 Approved. Preserved all 34 existing `AI-*` IDs and added AI-APP-004 for the approved CON-040 `RegisterVendorResult` representation. |
| 1.5 | 21 August 2026 | Regenerated using PR-005 from HJ-010, HJ-011 and HJ-012 v1.5 and HJ-107 v1.3 Approved. Preserved all 35 existing `AI-*` IDs, reconciled three Application obligations with CON-013, and added AI-IDEMP-001–002 for the approved composite identity, semantic fingerprint and dependent persistence-boundary evidence. |
| 1.6 | 21 August 2026 | Regenerated using PR-005 from HJ-010, HJ-011 and HJ-012 v1.6 and HJ-107 v1.4 Approved. Preserved all 37 existing `AI-*` IDs, made eight persistence/orchestration obligations Derivable, amended atomicity coverage and added AI-IDEMP-003–004 and AI-PG-001–002 for the approved CON-014–CON-016 and CON-028 cohort. |
| 1.7 | 22 August 2026 | Regenerated using PR-005 from HJ-010, HJ-011 and HJ-012 v1.7 and HJ-107 v1.5 Approved. Preserved all 41 existing `AI-*` IDs, resolved CON-019/CON-020 dependencies, added AI-EVT-001–003 for pre-outbox mapping, immutable serialized-event staging and relay non-reconstruction, and retained relay/broker execution dependencies under CON-018/CON-021. |
| 1.8 | 23 August 2026 | Regenerated using PR-005 from HJ-010, HJ-011 and HJ-012 v1.8 and HJ-107 v1.6 Approved. Preserved all 44 active `AI-*` IDs and materially strengthened AI-DE-003, AI-EVT-001 and AI-EVT-002 for the amended CON-020 contract-owned representations, exact mapping and deterministic serialization rules. |
| 1.9 | 26 August 2026 | Regenerated using PR-005 from HJ-010, HJ-011 and HJ-012 v1.9 and HJ-107 v1.7 Approved. Preserved all 44 existing `AI-*` IDs, added AI-API-001–004 and AI-VAL-001 for the approved CON-023–CON-026 API and validation architecture, and delegated the corresponding observable behaviour to the current HJ-107 catalogue. |

| 2.0 | 26 August 2026 | Regenerated using PR-005 from HJ-010, HJ-011 and HJ-012 v2.0 and HJ-107 v1.8 Approved. Preserved all 49 active `AI-*` IDs and reconciled AI-APP-004, AI-API-003 and AI-VAL-001 with the approved unified `RequestValidationFailure` and `registrationValidationFailed` mapping. |

## Related Documents

| Document ID | Title | Version | Status | Relationship |
|---|---|---:|---|---|
| PR-005 | Generate Architecture and Implementation Test Catalogue | Current | Governing prompt | Defines scope-parameterised HJ-013 derivation, catalogue ownership and stable-ID reconciliation. |
| HJ-004 | Vendor Domain Models | 2.8 | Approved | Authoritative Vendor Aggregate, Value Object, Entity, exact VendorRegistered v1 contract, Address snapshot and retrieval model. |
| HJ-005 | Coding Standards | 2.0 | Approved | Defines implementation boundaries, Domain modelling, persistence and enforcement expectations. |
| HJ-006 | Testing Strategy and Standards | 2.0 | Approved | Authoritative Test Levels, Test Classifications and catalogue rules. |
| HJ-007 | Enforcement Strategy | 2.0 | Approved | Defines build, static-analysis, dependency-validation and review mechanisms. |
| HJ-010 | Current Application Architectural Concerns | 2.0 | Approved | Complete concern register including approved CON-023–CON-026 and remaining unresolved dependencies. |
| HJ-011 | Epic 1 Vendor Registration Implementation Scope | 2.0 | Approved | Active delivery boundary including the approved API adaptation and validation allocation. |
| HJ-012 | Established Application Architecture Patterns | 2.0 | Approved | Authoritative approved architecture for CON-001–CON-017, CON-019, CON-020, CON-023–CON-028 and CON-040. |
| HJ-SM-001 | System Model | 1.0 | Approved | Confirms visible component relationships; visibility does not create delivery scope. |
| HJ-104 | Vendor Registration Fields Matrix | 3.6 | Approved | Defines registration information, exact contact validation and canonicalisation, positional Address translation, composite Vendor identity and semantic registration equivalence. |
| HJ-105 | Vendor Registration Sequence Diagram | 4.0 | Approved | Defines API adaptation, validation allocation, RegisterVendor collaboration order, replay and failure paths. |
| HJ-106 | Vendor Registration Service Contract | 1.8 | Approved | Approved transport-independent operations and normative Epic 1 HTTP/JSON contract. |
| HJ-107 | Vendor Registration Test Catalogue | 1.8 | Approved | Current behavioural catalogue and stable `VR-*` coverage source. |
| ADR-001 | Domain-Driven Design as the Primary Architectural Style | Current | Accepted | Governs the Domain Model and approved DDD patterns. |
| ADR-003 | Event-Driven Collaboration | 1.2 | Accepted | Governs event-driven collaboration, pre-outbox translation and relay non-reconstruction. |
| ADR-004 | Vendor Lifecycle Begins After Successful Registration | Current | Accepted | Governs Vendor identity and lifecycle commencement. |
| ADR-006 | Address Domain Ownership and Business Address Snapshots | 1.3 | Accepted | Governs Address ownership, the permanent contextual reference and positional snapshot translation. |
| ADR-008 | Idempotent Operations and Reliable Event Publication | 1.4 | Accepted | Governs atomic registration, pre-outbox translation, immutable serialized-event staging and publication guarantees. |

# 1. Purpose

HJ-013 catalogues executable tests and controlled verification obligations required to validate conformance with approved HotJoes application architecture and implementation standards.

This catalogue covers the twenty-six Approved concerns in HJ-012 v2.0:

```text
CON-001–CON-017, CON-019, CON-020, CON-023–CON-028 and CON-040
```

HJ-013 does not complete unresolved architecture. Where verification cannot be implemented without selecting an Approach for a Current Architectural Concern, the obligation remains **Dependent** and identifies that concern.

HJ-013 complements HJ-107. HJ-107 owns service and Domain behaviour derived from HJ-106 and its authoritative sources. HJ-013 owns distinct structural, dependency, concrete persistence, transaction-mechanism, outbox and runtime-mechanism evidence.

# 2. Source Authority

| Artefact | Authority Classification | Use in This Derivation |
|---|---|---|
| HJ-012 v2.0 Approved | Primary approved architecture | Supplies twenty-six Approved concerns, including CON-023–CON-026. |
| HJ-010 v2.0 Approved | Architectural governance | Supplies complete concern states and unresolved `CON-*` dependencies without authorising selection of candidate Approaches. |
| HJ-011 v2.0 Approved | Active delivery scope | Restricts instantiation to Epic 1 Vendor Registration and includes its approved API adaptation and validation boundaries. |
| HJ-SM-001 v1.0 | System context | Confirms visible relationships; components outside HJ-011 remain outside this derivation. |
| HJ-004 v2.8 | Domain authority | Defines the Vendor Aggregate, internal fact, exact published v1 contract, Vendor-owned Address snapshot and Domain concepts used by the in-scope operations. |
| HJ-104 v3.6 and HJ-105 v4.0 | Information and interaction authority | Define registration information, contact canonicalisation, validation allocation, API adaptation and orchestration order without independently approving an implementation Approach. |
| HJ-106 v1.8 | Service boundary | Defines the approved transport-independent operations and normative Epic 1 HTTP/JSON contract. |
| HJ-107 v1.8 | Behavioural catalogue | Owns behavioural obligations and supplies the current stable `VR-*` coverage references. |
| HJ-005 / HJ-006 / HJ-007 | Engineering and test standards | Govern implementation boundaries, classification and enforcement treatment. |
| ADR-001, ADR-003 v1.2, ADR-004, ADR-006 v1.3 and ADR-008 v1.4 | Accepted decisions | Supply decision authority for the applicable Approved patterns. |
| Controlled HJ-013 v1.9 | Reconciliation baseline only | Sole baseline for preserving and reconciling `AI-*` IDs; not independent architectural authority. |

No normative source conflict was identified for the twenty-six Approved concerns. CON-023–CON-026 and amended CON-040 define the thin HTTP adaptation, technical API contract, one unified Application validation outcome, centralized controlled failure mapping and layered validation allocation. CON-018 relay processing and CON-021 broker delivery remain unresolved without weakening the approved contract and staging boundaries.

# 3. Scope and Derivation Rules

## 3.1 Scope Boundary

| Boundary | Applied Baseline |
|---|---|
| Approved architecture | HJ-012 v2.0 Approved: CON-001–CON-017, CON-019, CON-020, CON-023–CON-028 and CON-040 |
| Active delivery scope | HJ-011 v2.0 Approved: Epic 1 Vendor Registration |
| Generation mode | Controlled Regeneration |
| Previous HJ-013 baseline | HJ-013 v1.9 Approved |
| Current behavioural catalogue | HJ-107 v1.8 Approved |
| Applicable operations | RegisterVendor and RetrieveRegisteredVendor |
| Applicable visible architecture | Vendor Domain, PostgreSQL persistence, reliable publication and the Address/Compliance boundaries only where exercised by HJ-011 |
| Visible but excluded architecture | Later Vendor lifecycle commands, Identity behaviour, future queries/read models and other capabilities excluded by HJ-011 |

Approved architecture is instantiated only within HJ-011 v2.0. The wider Domain Model and System Model do not independently expand this catalogue version.

## 3.2 Derivation Rule

Every obligation shall be traceable to:

- an Approved HJ-012 concern and its matching HJ-010 entry;
- an applicable ADR or approved engineering standard; and
- the current Domain, service and behavioural-test baseline where relevant.

An obligation shall not make an unresolved candidate Approach normative. A behavioural guarantee already owned by HJ-107 is recorded as **HJ-107 Coverage**, not duplicated as an HJ-013 executable test.

## 3.3 Verification Forms

| Verification Form | Treatment |
|---|---|
| **Executable Test** | Uses one HJ-006 Test Classification and its mapped Test Level. |
| **Automated Enforcement** | Uses a build, compiler, static-analysis, dependency-validation or architecture-fitness mechanism. It is not assigned an invented HJ-006 Test Classification. |
| **Contract Review** | Uses the approved HJ-006 non-executable Contract Review classification. |
| **HJ-107 Coverage** | Records a behavioural obligation owned by HJ-107. HJ-013 generates no duplicate executable test. |

## 3.4 HJ-006 Mapping

| Classification | Test Level |
|---|---|
| Domain | Unit |
| Application | Unit |
| Persistence Integration | Integration |
| Integration Contract | Integration |
| API Contract | API Integration |
| Contract Review | Non-executable review |

## 3.5 Derivation Status

| Status | Meaning |
|---|---|
| **Derivable** | The obligation is sufficiently governed to define now. |
| **Dependent** | The obligation is valid, but its executable specification or implementation depends on one or more unresolved HJ-010 concerns. |
| **HJ-107** | The behavioural obligation is already owned by HJ-107. |

A dependency is not evidence that an Approved concern is deficient. A concern becomes Challenged only if its approved Required Guarantee or Approach is insufficient, contradictory or incapable of fulfilment.

# 4. Coverage Summary

| Concern ID | Approved Approach | Required Guarantee | Delivery Applicability | HJ-013 Treatment | HJ-107 Boundary | Known Dependencies |
|---|---|---|---|---|---|---|
| CON-001 | Aggregate | Vendor creation and lifecycle invariants are protected within one consistency boundary. | Vendor creation, persistence and retrieval | Mutation-boundary enforcement and whole-Aggregate rehydration | Creation invariants and outcomes | CON-037 for automated enforcement |
| CON-002 | Value Object | Identity-free concepts remain explicit and avoid primitive obsession. | Epic 1 Vendor values and Address snapshot | Equality, immutability and persistence conversion | Accepted, rejected and canonical business values | None; CON-026 is approved behavioural validation authority |
| CON-003 | Entity | Vendor identity persists through valid lifecycle-bearing behaviour. | Registration and retrieval | Encapsulation and persisted identity | Identity commencement and retrieval | CON-037 for automated enforcement |
| CON-004 | Domain Event | Registration records an internal fact without infrastructure coupling. | VendorRegistered Domain Event | Domain isolation and representation separation | Event occurrence and non-occurrence | CON-037 for automated enforcement |
| CON-005 | Repository | Persistence and retrieval do not introduce persistence concerns into Domain. | Vendor save and VendorId retrieval | Contract, dependency direction and adapter integration | Persisted and retrieved service outcomes | CON-037 for dependency enforcement |
| CON-006 | Vendor application port + Address adapter; Anti-Corruption Layer | Address remains authoritative behind an application-facing boundary. | RegisterVendor Address collaboration and Epic 1 stub | Dependency direction and foreign-model isolation | Address business outcomes | CON-037 |
| CON-007 | Synchronous typed Address Resolution | Registration resolves authoritative Address information without Domain invocation mechanics. | One Address call per RegisterVendor attempt | Application invocation and Domain-isolation verification | Resolution outcomes | None |
| CON-008 | Explicit Address mapper/translator | Address concepts translate exactly into Vendor-owned immutable values without leakage or Vendor derivation. | Positional snapshot, canonical ID and authorities | Exact adapter mapping and Anti-Corruption Layer verification | Snapshot and authority outcomes, including VR-ADDRESS-012 | None |
| CON-009 | Permanent contextual Address Resolution reference | A reference binds one immutable result and Trading Location. | Epic 1 deterministic Address stub and consumed contract | Binding, repeat-resolution and semantic-failure contract tests | Client progression and service failures | None |
| CON-010 | Fail-fast semantics; caller-controlled retry without in-process retry or circuit breaker | Address failures are deterministic and cannot create partial registration. | Semantic and technical Address stub outcomes | Failure translation, invocation-count and circuit-breaker absence evidence | Controlled business failures and prohibited effects | CON-037 for automated absence enforcement |
| CON-011 | Vendor Application Service | Application coordinates registration while Domain rules and infrastructure mechanics remain in their layers. | RegisterVendor orchestration | Executable collaborator test plus separate automated layer enforcement | Success, invariant and failure outcomes | CON-037 for enforcement |
| CON-012 | Immutable transport-independent `RegisterVendorCommand` | Complete registration intent remains independent of HTTP, Registration Session state, created Vendor state and client-authoritative Address values. | RegisterVendor Application boundary | Command-retention and exclusion test plus structural enforcement | Complete request and no-session behaviour | CON-037 for automated dependency enforcement |
| CON-013 | Composite Vendor uniqueness identity + deterministic semantic registration fingerprint | Identity and equivalence are explicit Application/persistence-boundary concepts derived only after authoritative Address resolution. | RegisterVendor identity, equivalence and replay decision boundary | Explicit value-representation and persistence/orchestration evidence | VR-REQ-007; VR-IDEMP-001–010; VR-FAILURE-007/009 | CON-037 for automated enforcement |
| CON-014 | PostgreSQL composite-identity uniqueness constraint + transaction-coordinated duplicate resolution | Concurrent equivalent or conflicting submissions converge without duplicate effects. | RegisterVendor concurrency boundary | Real-PostgreSQL uniqueness-race and convergence evidence | VR-IDEMP-003 and applicable replay/conflict outcomes | None |
| CON-015 | Permanent persisted registration outcome + versioned deterministic semantic fingerprint | Qualifying replay returns the original persisted result without re-execution or expiry. | Registration outcome persistence and replay | Fingerprint, retention and original-result round-trip evidence | VR-IDEMP-001–002, VR-FAILURE-007 | None |
| CON-016 | One explicit PostgreSQL transaction coordinated by Vendor Application and implemented by Infrastructure | Vendor, replay outcome and outbox obligation commit or roll back together. | RegisterVendor atomic persistence boundary | Transaction failure-injection and three-part atomicity evidence | VR-RELIABILITY-001–002, VR-FAILURE-006 | None |
| CON-040 | Closed typed `RegisterVendorResult` owned by the Vendor Application | RegisterVendor distinguishes committed success from every expected controlled failure without exposing transport, Address-provider, persistence, publication, Registration Session or framework representations; every Application validation failure is one `RequestValidationFailure`. | RegisterVendor Application outcome boundary | Closed-result construction, unified-validation representation, state-validity, failure-preservation and prohibited-representation evidence | HJ-106 success and controlled failure outcomes; VR-REQ-008; VR-FAILURE-001–003 | CON-037 for automated enforcement only; CON-024–CON-026 are approved |
| CON-017 | Transactional Outbox | A committed Vendor cannot lose its publication obligation. | Registration persistence and messaging | Atomicity, immutable serialized-event staging, rollback, Domain isolation and relay recovery | Durable publication and non-duplication outcomes | CON-018 and CON-021 for relay/broker; CON-037 enforcement |
| CON-019 | Vendor Application-owned explicit mapper | Internal and external event representations remain separate; translation occurs before outbox persistence and never at relay time. | RegisterVendor completed-fact translation | Mapper execution, Domain isolation and no-reconstruction evidence | VR-INTEGRATION-EVENT-001–003, VR-INTEGRATION-EVENT-010 | CON-018 for concrete relay execution; CON-037 for automated enforcement |
| CON-020 | VendorRegistered Integration Event v1 contract serialized once as immutable UTF-8 camel-case JSON using contract-owned representations, explicit nulls and deterministic identifier, timestamp, time and enum formats | The exact nested member structure, contract-owned representations and deterministic formats are preserved unchanged through persistence and retry. | Event creation and durable publication staging | Exact mapping plus serialize-once/persist-unchanged integration evidence | VR-INTEGRATION-EVENT-001–010 | CON-018 and CON-021 only for relay/broker execution |
| CON-023 | Thin ASP.NET Core Minimal API endpoint adapters | HTTP exposes RegisterVendor and RetrieveRegisteredVendor without allowing transport semantics or collaborators to enter Application or Domain behaviour. | `POST /vendors` and `GET /vendors/{vendorId}` | Transport isolation enforcement plus delegated endpoint behaviour | VR-API-001–003, VR-API-011 | CON-037 for automated dependency enforcement only |
| CON-024 | Explicit Epic 1 HTTP/JSON contract with generated OpenAPI | Routes, schemas, headers, serialization and response representations are exact and consistently described. | Vendor API and generated OpenAPI | OpenAPI-to-runtime congruence evidence plus delegated wire behaviour | VR-API-003–006, VR-API-011–012 | None |
| CON-025 | API-owned error envelope and centralized typed outcome mapping | Expected and unexpected failures produce the approved safe HTTP representation without leaking implementation details; every `RequestValidationFailure` uses one `registrationValidationFailed` mapping. | Vendor API error boundary | Centralized-mapper ownership enforcement plus delegated unified mapping behaviour | VR-API-007–010 | CON-037 for automated centralization enforcement only |
| CON-026 | API structural validation, authoritative Application validation/canonicalisation and defensive Domain invariants | Each layer validates only its approved responsibility; every independently detectable Application validation error is returned in one `RequestValidationFailure`; canonical values flow downstream and pre-commit failure creates no business effect. | RegisterVendor API, Application and Domain boundaries | Behavioural delegation without duplicate architecture tests | VR-REQ-006, VR-REQ-008–010, VR-FAILURE-001–003, VR-CONTACT-002–004, VR-API-003, VR-API-007, VR-API-010 | None |
| CON-027 | Query handler + Repository + response mapper | Retrieval uses persisted Vendor, returns a purpose-specific result and remains side-effect-free. | RetrieveRegisteredVendor | Handler/repository/mapper composition and persistence integration | Found, Not Found, response content, side effects and HTTP mapping | None |
| CON-028 | Explicit EF Core fluent mapping + PostgreSQL keys, constraints and indexes | Aggregate, registration-outcome and outbox persistence has explicit faithful mappings and database enforcement. | Vendor Infrastructure persistence | Mapping inspection plus real-PostgreSQL constraint/index/delete evidence | Ready persistence and retrieval outcomes in HJ-107 | CON-029 for migration lifecycle only |

# 5. Architecture and Implementation Test Catalogue

## 5.1 CON-001 — Aggregate

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-AGG-001 | Aggregate creation preserves invariants | Vendor creation produces a complete valid Aggregate or no Vendor. | CON-001; HJ-004 §8; HJ-106 §§4.5, 4.7; HJ-107 VR-INV-001–005 | HJ-107 Coverage | Applicable HJ-107 classifications | P0 | The cited HJ-107 invariant and success/failure obligations pass; HJ-013 adds no duplicate behavioural test. | HJ-107 owns the behavioural outcome. | HJ-107 |
| AI-AGG-002 | Aggregate Root controls mutation | External code cannot mutate aggregate-owned state by bypassing the Vendor Aggregate Root. | CON-001; HJ-012; ADR-001; HJ-005 §§3.2, 8.2–8.3 | Automated Enforcement | Not applicable | P0 | Structural checks reject prohibited project references, unrestricted public setters and externally accessible mutation paths. | CON-037 dependency-composition and enforcement mechanism. | Dependent |
| AI-AGG-003 | Aggregate consistency boundary is rehydrated as a whole | Repository rehydration reconstructs one complete valid Vendor Aggregate without exposing or independently materialising aggregate-owned parts as separate consistency boundaries. | CON-001; CON-005; CON-028; HJ-004 §§2.1–2.3; HJ-012; HJ-005 §10 | Executable Test | Persistence Integration / Integration | P0 | A real-PostgreSQL fixture rehydrates the complete Aggregate with owned-state fidelity and valid invariants through the Aggregate Root. Repository save, lookup and absence semantics remain independently owned by AI-REP-003. | None. | Derivable |

## 5.2 CON-002 — Value Object

For the Epic 1 RegisterVendor and RetrieveRegisteredVendor operations, HJ-004 identifies these applicable Value Objects: `VendorId`, `TradingCharacteristics`, `OpeningHours`, `VendorName`, `CompanyRegistrationNumber`, `PrimaryContact`, `EmailAddress`, `TelephoneNumber`, `CanonicalAddressId` and `BusinessAddressSnapshot`. Only Value Objects actually present in the approved operation model are exercised; later lifecycle-only Value Objects are outside this catalogue version.

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-VO-001 | Epic 1 Value Objects use value equality | Each applicable Value Object is equal when all defining values are equal, unequal when a defining value differs, and has equality-consistent hash behaviour. | CON-002; HJ-004 §2.3 and §9; HJ-012; ADR-001; HJ-005 §§6.4, 8 | Executable Test | Domain / Unit | P1 | Parameterised tests cover every listed Epic 1 Value Object using representative equal and unequal instances, including all seven BusinessAddressSnapshot fields. | None; AI-VO-003 owns the independently derivable persistence proof. | Derivable |
| AI-VO-002 | Epic 1 Value Objects preserve invariants and immutability | Applicable Value Objects expose invariant-preserving construction and cannot be mutated after construction; accepted and rejected business values remain owned by HJ-107. | CON-002; HJ-004 §2.3; HJ-012; HJ-005 §§2.5, 6.4, 8.1–8.3; HJ-107 | Executable Test | Domain / Unit | P1 | Construction and structural tests demonstrate controlled creation and absence of mutating public state without duplicating HJ-107 validation profiles. | None; CON-026 continues to govern exhaustive validation allocation and profiles in HJ-107, not this structural obligation. | Derivable |
| AI-VO-003 | Value Object persistence conversion is faithful | Each persisted applicable Value Object round-trips without loss, primitive distortion, nullability distortion or weakened validity/equality. | CON-002; CON-005; CON-028; HJ-004 §2.3; HJ-012; HJ-005 §§10.1–10.4 | Executable Test | Persistence Integration / Integration | P1 | Parameterised real-PostgreSQL conversion tests diagnose the individual Value Object whose representation fails, including snapshot optionality. | None. | Derivable |

## 5.3 CON-003 — Entity

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-ENT-001 | Vendor identity is assigned and stable within approved Epic 1 behaviour | Successful first registration assigns Vendor identity; retrieval exposes the same persisted identity. No later lifecycle command is invented for this test. | CON-003; HJ-004 §§2.2, 2.6; HJ-106 §§4.7–4.8, 4.12; HJ-107 VR-STATE and VR-RETRIEVE | HJ-107 Coverage | Applicable HJ-107 classifications | P0 | HJ-107 creation and retrieval identity obligations pass; later lifecycle-transition identity tests remain outside this catalogue version. | HJ-107 owns current behavioural identity evidence. | HJ-107 |
| AI-ENT-002 | Entity state is encapsulated | Entity state cannot be replaced through unrestricted public setters or significant property-setter behaviour. | CON-003; HJ-012; HJ-005 §§8.2–8.3, 23 | Automated Enforcement | Not applicable | P0 | Structural analysis rejects prohibited public mutation and persistence-only mutation paths exposed to application callers. | CON-037 dependency-composition and enforcement mechanism. | Dependent |
| AI-ENT-003 | Entity identity survives concrete rehydration | The persistence adapter rehydrates Vendor with the same `VendorId` and approved initial lifecycle state stored at registration. | CON-003; CON-005; CON-028; HJ-004 §§2.2, 2.6; HJ-012; HJ-005 §10 | Executable Test | Persistence Integration / Integration | P0 | Focused real-PostgreSQL round trip asserts identity and lifecycle fields only; Aggregate completeness and repository-adapter behaviour are owned by AI-AGG-003 and AI-REP-003. | None. | Derivable |

## 5.4 CON-004 — Domain Event

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-DE-001 | Domain Event is infrastructure-free | The Domain Event and Domain project do not reference broker, serialization, transport or Integration Event implementation types. | CON-004; HJ-012; ADR-001; ADR-003; ADR-008 §2.5; HJ-005 §§3.2, 8.4, 17.1 | Automated Enforcement | Not applicable | P0 | Dependency and type checks reject infrastructure or published-contract dependencies in the Domain. | CON-037 dependency-composition and enforcement mechanism. | Dependent |
| AI-DE-002 | Domain Event occurrence follows business state change | Successful first creation records one internal business fact; failures, replay, conflict and retrieval record no additional fact. | CON-004; HJ-004; HJ-106 §§4.9–4.12; HJ-107 VR-DOMAIN-EVENT-001–004 | HJ-107 Coverage | Applicable HJ-107 classifications | P0 | The cited HJ-107 Domain Event obligations pass; HJ-013 adds no duplicate behavioural test. | HJ-107 owns event-occurrence behaviour. | HJ-107 |
| AI-DE-003 | Domain and Integration Event representations remain separate | Internal Domain Event and Vendor Domain Aggregate, Value Object and enum types are not reused as published Integration Event contract types; the VendorRegistered v1 contract owns every published representation. | CON-004; CON-019; amended CON-020; HJ-012; ADR-003; ADR-008 §2.5 | Automated Enforcement | Not applicable | P0 | Type/dependency checks prove distinct internal and published representations, contract-owned identifier, timestamp, enum, Trading Characteristics, Opening Hours and BusinessAddress types, and absence of Integration Event, outbox, serialization and broker types from the Vendor Domain. | CON-037 enforcement mechanism only. | Dependent |

## 5.5 CON-005 — Repository

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-REP-001 | Repository boundary has correct dependency direction | The repository abstraction is available to its approved inner boundary and its concrete persistence implementation remains in Infrastructure. | CON-005; HJ-012; ADR-001; HJ-005 §§3.2, 10.1, 10.4 | Automated Enforcement | Not applicable | P0 | Dependency validation rejects references to a concrete repository from Domain/Application and rejects infrastructure representations leaking through the abstraction. | CON-037 determines exact composition and enforcement; exact abstraction placement must follow the approved solution structure. | Dependent |
| AI-REP-002 | Repository contract exposes aggregate semantics | The repository persists and retrieves the Vendor Aggregate Root without exposing ORM entities, database rows or transport DTOs. | CON-005; HJ-012; HJ-005 §§10.1, 10.4 | Contract Review | Contract Review / Non-executable | P0 | Review confirms aggregate-oriented operations, absence of speculative generic methods and no persistence representation in the contract. | Concrete API remains implementation-local within approved standards. | Derivable |
| AI-REP-003 | Repository adapter fulfils its persistence contract | The concrete repository can save a new Vendor, retrieve it by `VendorId`, and return absence for an unknown identifier without leaking persistence representations or exceptions. | CON-005; CON-028; HJ-012; HJ-005 §10; HJ-006 §2.1.6 | Executable Test | Persistence Integration / Integration | P0 | Real-PostgreSQL adapter tests prove repository operation semantics; detailed Aggregate, Entity and Value Object fidelity are owned by AI-AGG-003, AI-ENT-003 and AI-VO-003. | None. | Derivable |

## 5.6 CON-017 — Transactional Outbox

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-OUT-001 | Vendor, replay outcome and publication work commit atomically | A successful registration commits Vendor state, its permanent original-result record and exactly one immutable serialized VendorRegistered v1 publication obligation in one atomic boundary. | CON-015–CON-017; CON-019; CON-020; CON-028; HJ-012; ADR-008 §§2.6–2.7; HJ-106 §§4.7, 4.10; HJ-006 §2.1.6 | Executable Test | Persistence Integration / Integration | P0 | Real-PostgreSQL failure injection demonstrates that committed Vendor state always has the corresponding replay outcome and exactly one immutable serialized event record. | None for atomic recording; relay and broker execution remain under CON-018 and CON-021. | Derivable |
| AI-OUT-002 | Atomic rollback leaves no partial registration state | Failure before transaction commit leaves no Vendor state, replay outcome or durable publication work. | CON-015–CON-017; CON-028; HJ-012; ADR-008 §§2.6–2.7; HJ-106 §§4.10–4.11 | Executable Test | Persistence Integration / Integration | P0 | Failure injection at approved transaction points proves full rollback and no orphan Vendor, registration-outcome or outbox record. | None. | Derivable |
| AI-OUT-003 | Outbox is outside Domain decision-making | Domain behaviour records the business fact without referencing outbox persistence, broker or publication infrastructure. | CON-004; CON-017; ADR-008 §§2.5–2.6; HJ-005 §§3.2, 8.4 | Automated Enforcement | Not applicable | P0 | Dependency checks show no outbox, broker or infrastructure types in Domain code. | CON-037 dependency-composition and enforcement mechanism. | Dependent |
| AI-OUT-004 | Publication retry does not repeat the business operation | Retry does not recreate Vendor state, Domain Events, completed facts, publication records or Integration Events. | CON-017; ADR-008 §§2.2, 2.6, 5; HJ-106 §§4.9–4.11; HJ-107 VR-RELIABILITY and VR-IDEMP | HJ-107 Coverage | Applicable HJ-107 classifications | P0 | The cited HJ-107 observable non-duplication obligations pass; AI-OUT-005 supplies distinct mechanism evidence after dependencies resolve. | HJ-107 owns the behavioural outcome. | HJ-107 |
| AI-OUT-005 | Relay recovery preserves one publication obligation | Following relay failure or restart, the stored immutable event remains recoverable and retry does not repeat the original business operation or reconstruct the event. | CON-017; CON-019; CON-020; ADR-008 §§2.6, 5; HJ-106 §4.10 | Executable Test | Integration Contract / Integration | P0 | Relay failure/restart tests demonstrate recovery of the exact stored EventId, version and serialized bytes with preservation of the no-repeat guarantees. | CON-018 relay processing and CON-021 broker semantics. | Dependent |

## 5.7 CON-027 — Query Handler, Repository and Response Mapper

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-QRY-001 | Retrieval uses the approved handler, repository and mapper roles | The query handler obtains persisted Vendor through the Repository and delegates production of a purpose-specific response without using an alternative state source or exposing the Aggregate. | CON-027; HJ-004 §2.6; HJ-106 §4.12; HJ-012; HJ-005 §§9.2–9.4, 10.4 | Executable Test | Application / Unit | P1 | Collaborator-focused tests show one repository lookup by `VendorId`, mapping from returned Vendor, and no alternative persistence/read-model collaborator. | Response content remains governed by HJ-004/HJ-106 and HJ-107. | Derivable |
| AI-QRY-002 | Retrieval is side-effect-free | Retrieval does not save Vendor state, record an event, create publication work or invoke a publisher. | CON-027; HJ-106 §4.12.3; HJ-107 VR-RETRIEVE-006 and related obligations | HJ-107 Coverage | Applicable HJ-107 classifications | P1 | The cited HJ-107 retrieval side-effect obligations pass; HJ-013 adds no duplicate application test. | HJ-107 owns the behavioural outcome. | HJ-107 |
| AI-QRY-003 | Retrieval returns controlled Not Found | Repository absence produces the approved application-level Not Found outcome without infrastructure leakage. | CON-027; HJ-106 §§4.12.2–4.12.3, 6.5; HJ-107 VR-RETRIEVE-007, VR-API-008 | HJ-107 Coverage | Applicable HJ-107 classifications | P1 | The cited HJ-107 obligations prove the Application Not Found outcome and its approved HTTP representation. | HJ-107 owns both observable boundaries; HJ-013 adds no duplicate test. | HJ-107 |
| AI-QRY-004 | Concrete retrieval adapter and mapper preserve their boundaries | A real persisted Vendor is rehydrated through the Repository and mapped into Registered Vendor Details without persistence representations entering the response. | CON-027; CON-005; CON-028; HJ-004 §2.6; HJ-106 §4.12; HJ-012; HJ-005 §§9.4, 10.4–10.5 | Executable Test | Persistence Integration / Integration | P1 | Real-PostgreSQL integration evidence focuses on repository-to-mapper boundary, purpose-specific response mapping, approved snapshot fields and absence of ORM/database types; HJ-107 owns response values and side effects. | None. | Derivable |

## 5.8 CON-006–CON-013 and CON-040 — Address Boundary and RegisterVendor Orchestration

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-ADDR-001 | Address dependency points outward through an application port | Vendor Domain and Application code reference neither the Address implementation, Epic 1 stub nor Address transport types. | CON-006; HJ-012; ADR-006 | Automated Enforcement | Not applicable | P0 | Dependency checks show a Vendor Application-owned port implemented by an outer adapter; Domain has no Address adapter or stub reference. | CON-037 governs the enforcement mechanism. | Dependent |
| AI-ADDR-002 | Typed adapter invokes Address synchronously | One RegisterVendor application attempt makes one typed resolution call with the opaque reference and declared Trading Location. | CON-007; CON-010; HJ-106 §4.6 | Executable Test | Application / Unit | P0 | Collaborator test proves one call, deterministic result handling and no invocation mechanics in Domain behaviour. | None. | Derivable |
| AI-ADDR-003 | Adapter performs exact positional translation | The adapter maps Address source Line 1 to optional RecipientOrOrganisationName; Lines 2, 3 and 4 respectively to required AddressLine1, optional AddressLine2 and optional AddressLine3; and Post Town, Postcode and optional County directly, while also mapping CanonicalAddressId and applicable authority values. | CON-008; HJ-012; ADR-006 §2; HJ-004 §2.3; HJ-104 §5.4; HJ-106 §4.6; HJ-107 VR-ADDRESS-012 | Executable Test | Application / Unit | P0 | Parameterised mapping tests prove exact field positions and optional absence, with no concatenation, compression, shifting, reordering, name matching or Vendor-side normalisation. Foreign-model dependency evidence remains owned by AI-ADDR-001. | None. | Derivable |
| AI-ADDR-004 | Permanent reference binds context and resolves immutably | Repeated resolution with the bound Trading Location returns the originally bound result despite simulated later Address-data change; another Trading Location returns InvalidAddressResult. | CON-009; ADR-006; HJ-011 §4.1 | Executable Test | Integration Contract / Integration | P0 | Contract tests prove Trading Location binding, no expiry, revocation, consumption or recalculation, and require CanonicalAddressId. | Epic 1 stub supplies deterministic evidence. | Derivable |
| AI-ADDR-005 | Semantic failure translation is exact | InvalidReference and InvalidAddressResult remain distinct controlled application outcomes and are not converted to technical failure. | CON-009; CON-010; HJ-106 §§4.6, 4.11 | Executable Test | Application / Unit | P0 | Adapter/application tests assert exact outcome and zero persistence/aggregate collaboration. | HJ-107 owns caller-observable outcome and business-effect assertions. | Derivable |
| AI-ADDR-006 | Technical failure has no automatic retry | Timeout, unavailability and transient failure create one retryable application failure from one Address invocation while preserving the reference for a later caller-controlled attempt. | CON-010; HJ-011 §4.1 | Executable Test | Application / Unit | P0 | Failure-injection tests count one invocation per attempt, show no internal retry or aggregate/persistence call on failure, and permit the same reference on a later attempt. | None. | Derivable |
| AI-ADDR-007 | Epic 1 has no Address circuit breaker | Runtime composition contains no circuit-breaker policy on the Address adapter. | CON-010; HJ-011 §4.1 | Automated Enforcement | Not applicable | P0 | Dependency and composition checks reject circuit-breaker state, policy or library configuration on the Epic 1 Address adapter. | CON-037 governs the approved enforcement mechanism. | Dependent |
| AI-APP-001 | RegisterVendor orchestration coordinates approved collaborators | The Application Service coordinates request validation, one Address resolution, first-processing aggregate creation and persistence/publication staging in the approved order while Domain creation rules remain exercised through the Aggregate. Identity/replay persistence coordination is independently owned by AI-IDEMP-002. | CON-011; HJ-004; HJ-105; HJ-106 | Executable Test | Application / Unit | P0 | Collaborator-focused tests prove first-processing and failure-path calls and prohibited calls without duplicating HJ-107 business assertions or the independently testable persistence boundary. | None; AI-IDEMP-002 owns the separate derivable identity/replay coordination. | Derivable |
| AI-APP-002 | RegisterVendor layer boundaries are enforced | Transport and adapters do not contain Vendor Domain rules, and Domain code does not perform Address, persistence, transport or publication orchestration. | CON-011; CON-006; HJ-012; HJ-005 | Automated Enforcement | Not applicable | P0 | Dependency and structural checks reject Domain-rule relocation and infrastructure orchestration in inner layers. | CON-037 governs the approved enforcement mechanism. | Dependent |
| AI-APP-003 | RegisterVendor command represents complete client-authored intent | The immutable Vendor Application-owned `RegisterVendorCommand` retains every client-authored registration field, the opaque Address Resolution reference and transient declarations while excluding HTTP types, Registration Session state, a Vendor Aggregate, Address-owned authoritative values, server-generated state, persistence/publication representations, the derived composite identity and the semantic registration fingerprint. | CON-012; CON-013; HJ-012; HJ-011 §2.1; HJ-104; HJ-106 §§4.1–4.3 | Executable Test | Application / Unit | P0 | Focused construction and reflection evidence proves complete field retention, immutability and prohibited-type/value exclusion without duplicating HJ-107 request-validity behaviour. | Automated cross-project dependency enforcement remains subject to CON-037; the executable command contract is independently derivable. | Derivable |
| AI-APP-004 | RegisterVendor result is a closed valid Application outcome | The immutable Vendor Application-owned `RegisterVendorResult` represents exactly one committed success or one expected controlled HJ-106 failure. Success contains the minimum committed `VendorId` and `PendingActivation` state; failure contains no success payload. Every request-field, Registration Declaration, conditional and cross-field validation failure uses one immutable `RequestValidationFailure` containing all independently detectable errors; `RegistrationDeclarationFailure` and `ConditionalRuleFailure` are absent from the closed outcome set. `InvalidReference`, `InvalidAddressResult` and `AddressServiceTemporarilyUnavailable` remain distinct. The result exposes no HTTP, Address-provider, persistence, publication, Registration Session or framework representation. | CON-040; CON-025; CON-026; HJ-012; HJ-011 §2.1; HJ-106 §§4.5, 4.7, 4.9, 4.11; HJ-107 VR-REQ-008, VR-FAILURE-001–003 | Executable Test | Application / Unit | P0 | Focused construction and reflection evidence proves a closed immutable outcome set, mutually exclusive success/failure state, minimum success data, one aggregated validation result, absence of the two superseded result kinds, exact Address failure preservation and prohibited-type exclusion. Tests do not prescribe HTTP mapping. | Automated cross-project dependency enforcement remains subject to CON-037; HJ-107 owns the validation-error content behaviour. | Derivable |
| AI-IDEMP-001 | Composite identity and semantic fingerprint are explicit Application values | Vendor uniqueness identity and semantic registration fingerprint are immutable, transport-independent Vendor Application values derived after Address resolution. Identity contains only the normalized Trading Name, normalized Legal Operator Name and `CanonicalAddressId`. Fingerprint excludes transient declarations, the opaque Address Resolution reference, server-generated values and technical metadata. Neither value is caller-authored or part of the Vendor Aggregate. | CON-013; HJ-012; HJ-011 §§2.1, 2.4; HJ-104 §§5.3, 5.6; HJ-106 §§4.4, 4.9; HJ-107 VR-REQ-007, VR-IDEMP-007–010 | Executable Test | Application / Unit | P0 | Focused construction and structural tests prove ownership, immutability, exact identity components, prohibited-content exclusion and deterministic value behaviour. HJ-107 remains authoritative for equivalent replay, conflict and registered display-value outcomes. | Automated cross-project enforcement remains subject to CON-037; exact fingerprint encoding and persistence representation remain outside this obligation. | Derivable |
| AI-IDEMP-002 | RegisterVendor consumes derived identity and fingerprint through the persistence boundary | After successful Address resolution, the Application derives the composite identity and fingerprint, obtains the first-processing, equivalent-replay or conflict determination before aggregate creation, returns the original committed result for replay and never treats conflict as an update. | CON-013–CON-016; CON-028; HJ-012; HJ-011 §§2.1, 2.4; HJ-105 §§7, 9; HJ-106 §§4.6, 4.9; HJ-107 VR-IDEMP-001–009, VR-FAILURE-007/009 | Executable Test | Persistence Integration / Integration | P0 | Collaborator and real-persistence tests prove post-Address derivation, one outcome owner, original-result replay, conflict preservation and absence of aggregate creation for replay/conflict without duplicating HJ-107 business assertions. | None. | Derivable |
| AI-IDEMP-003 | PostgreSQL uniqueness coordinates concurrent registration | The approved normalized composite identity is enforced by PostgreSQL so concurrent equivalent or conflicting submissions produce one committed owner and every losing transaction commits no business effect. | CON-014; CON-016; CON-028; HJ-012; ADR-008 §§2.2, 2.7; HJ-107 VR-IDEMP-003 | Executable Test | Persistence Integration / Integration | P0 | Real-PostgreSQL concurrency tests release competing transactions at the uniqueness boundary and prove one Vendor, one registration outcome and one outbox record; equivalent losers replay and conflicting losers return conflict after loading the winner. | No process-local/distributed lock, duplicate effect or split owner. | Derivable |
| AI-IDEMP-004 | Permanent replay record preserves the original result and versioned fingerprint | Each successful registration persists the original Application result and SHA-256 fingerprint of the versioned deterministic UTF-8 canonical material representation without expiry or reconstruction from current Vendor state. | CON-015; CON-028; HJ-012; ADR-008 §§2.2, 2.7; HJ-107 VR-IDEMP-001–002, VR-FAILURE-007 | Executable Test | Persistence Integration / Integration | P0 | Real-PostgreSQL round-trip and later-state tests prove fingerprint determinism, excluded-field neutrality, retained original result and permanent replay after Vendor lifecycle state changes. | No expiry, declaration/reference inclusion or current-state reconstruction. | Derivable |

## 5.9 CON-014–CON-016 and CON-028 — Registration Persistence Boundary

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-PG-001 | Explicit EF Core mappings preserve the approved persistence model | Vendor Infrastructure explicitly maps Aggregate state, Registered Information, the one-to-one registration outcome and outbox data with approved keys, lengths, nullability, conversions and enum representations. | CON-028; HJ-012; HJ-011 §2.4; HJ-005 §10; ADR-008 §2.7 | Executable Test | Persistence Integration / Integration | P0 | Configuration inspection and real-PostgreSQL round trips prove every approved field, optionality and conversion while declarations and the opaque Address Resolution reference remain absent from Vendor state. | None. | Derivable |
| AI-PG-002 | PostgreSQL constraints, indexes and delete behaviour enforce the approved guarantees | PostgreSQL enforces composite uniqueness, one-to-one outcome ownership, required foreign keys, restrictive deletion and indexes supporting identity, Vendor and unpublished-outbox lookup. | CON-014; CON-015; CON-028; HJ-012; ADR-008 §2.7 | Executable Test | Persistence Integration / Integration | P0 | Database metadata and attempted-violation tests prove normalized-name plus CanonicalAddressId uniqueness, required relational ownership, no cascade deletion of outcomes/outbox work and presence/use of required indexes. | CON-029 governs migration lifecycle, not the resulting schema guarantees. | Derivable |

## 5.10 CON-019–CON-020 — VendorRegistered Translation and Published Contract Staging

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-EVT-001 | Application mapper translates the completed fact into the exact contract before persistence | An explicit Vendor Application mapper creates VendorRegistered v1 from the completed internal fact and approved registration-time information using the exact contract-owned envelope, nested payload, Trading Characteristics, Opening Hours and BusinessAddress representations before outbox persistence. | CON-019; amended CON-020; HJ-012; HJ-004 §7.2; HJ-105 §10; ADR-003; ADR-008 §2.5 | Executable Test | Application / Unit | P0 | Mapper tests prove exact source-to-contract member translation, nesting and exclusions; contract properties match the authoritative registration-time values; no Vendor Domain Aggregate, Value Object, enum or `BusinessAddressSnapshot` type crosses the boundary. | Exact contract values and wire behaviour are owned by HJ-107 VR-INTEGRATION-EVENT-001–009; no unresolved architecture dependency. | Derivable |
| AI-EVT-002 | Infrastructure serializes deterministically once and persists the event unchanged | Vendor Infrastructure serializes the mapped VendorRegistered v1 once as UTF-8 camel-case JSON using the approved UUID `D`, UTC timestamp `O`, time-only `HH:mm:ss`, lower-camel enum, nested-object and explicit-null rules, then persists the exact immutable bytes with EventId and version inside the registration transaction. | CON-016; CON-017; CON-019; amended CON-020; CON-028; HJ-012; ADR-008 §§2.5–2.7 | Executable Test | Persistence Integration / Integration | P0 | Real-PostgreSQL integration evidence proves one deterministic serialization from representative contract variants and exact round-trip equality of stored EventId, EventVersion and bytes with the produced representation; no remapping, reserialization or format drift occurs at persistence. | None for creation and persistence; relay/broker execution is separate. | Derivable |
| AI-EVT-003 | Relay publishes the stored event without reconstruction | Publication reads and publishes the stored VendorRegistered v1 identity, version and serialized bytes without querying or reconstructing from current Vendor state. | CON-017; CON-019; CON-020; HJ-012; ADR-003; ADR-008 §§2.5–2.6 | Executable Test | Integration Contract / Integration | P0 | Relay-boundary tests prove exact-byte publication after Vendor state changes and across retry/restart, with no Vendor repository lookup or mapping during relay. | CON-018 must define relay claim, retry and recovery mechanics; CON-021 governs broker acknowledgement/delivery. Already derivable: the prohibited reconstruction rule and exact stored-event expectation. | Dependent |

## 5.11 CON-023–CON-026 — HTTP Adaptation and Validation Allocation

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-API-001 | Transport representations and collaborators remain outside inner layers | Vendor Application and Domain assemblies reference no ASP.NET Core, HTTP, OpenAPI or API DTO type, and endpoint adapters contain no Address, repository, transaction, event, outbox or broker implementation dependency. | CON-023; HJ-012; HJ-011 §2.3; HJ-005 | Automated Enforcement | Not applicable | P1 | Dependency and structural checks reject transport types in inner layers and prohibited implementation collaborators in endpoint composition. | CON-037 governs the approved enforcement mechanism. | Dependent |
| AI-API-002 | Generated OpenAPI describes the approved runtime contract | The generated OpenAPI description exposes only the two approved Epic 1 routes and accurately describes their request, response, media-type, header, nullability and controlled-error schemas. | CON-024; HJ-012; HJ-011 §2.3; HJ-106 Part B | Executable Test | API Contract / API Integration | P0 | Tests compare generated OpenAPI operations and schemas with the runtime API contract, including required members, explicit-null response optionals, success and controlled-error responses, and absence of excluded endpoints or headers. HJ-107 separately owns actual request/response behaviour. | None. | Derivable |
| AI-API-003 | Controlled failure translation is owned by one API mapping boundary | Typed Application outcomes are translated through one API-owned mapping boundary; every `RequestValidationFailure` follows the single `400 registrationValidationFailed` mapping; and unexpected exceptions are handled centrally rather than independently inside endpoints. | CON-025; CON-026; CON-040; HJ-012; HJ-011 §2.3; HJ-005; HJ-106 §§6.4–6.5 | Automated Enforcement | Not applicable | P0 | Composition and dependency checks show endpoints delegate controlled outcomes to one mapper, the mapper contains only one validation-outcome branch, the superseded declaration and conditional branches are absent, and unexpected exceptions reach the central handler; endpoint-specific mapping tables or exception payload construction are rejected. | CON-037 governs the approved enforcement mechanism; observable status, code and error aggregation remain delegated to HJ-107. | Dependent |
| AI-API-004 | HTTP endpoint and failure behaviour remains owned by HJ-107 | The approved routes, thin endpoint invocation, JSON representations, success responses, headers, status mappings and client-safe failure envelopes behave exactly as HJ-106 Part B specifies. | CON-023–CON-025; HJ-106 Part B; HJ-107 VR-API-001–012 | HJ-107 Coverage | Applicable HJ-107 classifications | P0 | All cited `VR-API-*` obligations pass; HJ-013 adds no duplicate executable API-behaviour test. | HJ-107 owns the observable API behaviour. | HJ-107 |
| AI-VAL-001 | Layered validation, unified failure and canonicalisation behaviour remains owned by HJ-107 | API structural validation, authoritative Application validation, aggregation of every independently detectable field, Registration Declaration, conditional and cross-field error into one `RequestValidationFailure`, pre-downstream ordering, canonical-value flow and defensive Domain invariants follow the approved allocation. | CON-025; CON-026; CON-040; HJ-012; HJ-011 §§2.1–2.3; HJ-104; HJ-106; HJ-107 VR-REQ-006, VR-REQ-008–010, VR-FAILURE-001–003, VR-CONTACT-002–004, VR-API-003, VR-API-007, VR-API-010 | HJ-107 Coverage | Applicable HJ-107 classifications | P0 | The cited behavioural obligations pass through direct Application, Domain and API boundaries, proving one aggregated validation result, exact Contact Email and Primary Contact Telephone rules, one API validation code and zero pre-commit business effect. | HJ-107 owns these observable validation responsibilities; HJ-013 adds no duplicate executable test. | HJ-107 |

# 6. Cross-Catalogue Responsibility

| Guarantee | HJ-107 Responsibility | HJ-013 Responsibility |
|---|---|---|
| Aggregate validity | Business-valid creation and failure outcomes | Aggregate Root mutation enforcement and consistency-boundary rehydration |
| Value Object semantics | Accepted/rejected and canonical business values | Equality, immutability and concrete persistence conversion |
| Entity identity | Identity commencement and retrieved identity-bearing state | Encapsulation and concrete rehydration identity |
| Domain Event | Occurrence and non-occurrence under business stimuli | Infrastructure isolation and representation separation |
| Repository | Persisted and retrieved service outcomes | Contract shape, dependency direction and concrete adapter integration |
| Reliable publication | Durable obligation, non-duplication and caller-observable outcomes | Transactional atomicity, failure injection and relay/broker mechanism evidence |
| Retrieval | Found/Not Found, response content and no side effects | Handler/repository/mapper structure and concrete persistence mapping |
| Address collaboration | Selection, resolution, snapshot, authority and failure outcomes, including VR-ADDRESS-012 positional behaviour | Port/adapter dependency direction, exact mapper execution, consumed-reference contract and resilience mechanism evidence |
| RegisterVendor orchestration | Business success, invariant and failure outcomes | Collaborator composition and application-layer dependency enforcement |
| Registration intent representation | Complete request and absence of Registration Session dependence | Command completeness, immutability, ownership and prohibited-content evidence |
| Vendor uniqueness and semantic equivalence | Identity matching, equivalent replay, conflict, display-value preservation and prohibited business effects | Application-owned identity/fingerprint representation, PostgreSQL concurrency and persistence-boundary orchestration evidence |
| Permanent replay outcome | Original-result replay and absence of repeated effects | Fingerprint encoding, permanent outcome storage and original-result round-trip evidence |
| Registration atomicity | No partial observable registration and safe retry | Real-PostgreSQL three-part commit/rollback failure injection across Vendor, replay outcome and outbox work |
| PostgreSQL mapping | Persisted and retrieved behavioural outcomes | Explicit EF Core mapping, relational constraint, index and restrictive-delete evidence |
| RegisterVendor outcome representation | Successful and controlled failure outcomes defined by HJ-106 | Result closure, immutability, state validity, exact Address-failure preservation and prohibited-representation evidence |
| VendorRegistered v1 published contract | Envelope, payload, independent BusinessAddress, exclusions, explicit nulls, compatibility and retry identity/content | Application mapper execution, Domain isolation, serialize-once persistence and relay non-reconstruction mechanisms |
| HTTP adaptation | Route behaviour, Application invocation, cancellation, headers and prohibited endpoint side effects | Automated transport-isolation enforcement; exact observable endpoint behaviour is delegated |
| Technical API contract | Exact runtime HTTP/JSON requests, responses, formats and headers | Generated OpenAPI congruence with the approved runtime contract |
| Controlled failure mapping | Exact statuses, codes, error envelope and information-exclusion behaviour | Centralized mapping and exception-handler ownership enforcement |
| Validation allocation | Structural, Application and Domain validation behaviour, one aggregated `RequestValidationFailure`, canonicalisation, ordering and absence of pre-commit effects | Closed-result structure under AI-APP-004; observable validation and API mapping delegated to HJ-107; no duplicate HJ-013 behavioural test |

The same Required Guarantee may have tests at distinct boundaries, but identical stimuli and evidence shall not be duplicated across the catalogues.

# 7. Concern-to-Test Traceability

| Approved Concern | Required Guarantee | AI Test IDs | HJ-107 Coverage | Coverage Status | Dependencies |
|---|---|---|---|---|---|
| CON-001 | Vendor invariants are protected within one consistency boundary. | AI-AGG-001–003 | VR-INV-001–005 and applicable success/failure obligations | Covered | CON-037 for mutation-boundary enforcement only |
| CON-002 | Identity-free concepts remain explicit and do not degrade into primitive obsession. | AI-VO-001–003 | Applicable field, validation and canonicalisation obligations | Covered | None; CON-026 is approved behavioural validation authority |
| CON-003 | Vendor identity persists through valid in-scope behaviour. | AI-ENT-001–003 | VR-STATE and VR-RETRIEVE obligations | Covered | CON-037 for encapsulation enforcement only |
| CON-004 | Registration records an internal fact without infrastructure coupling. | AI-DE-001–003 | VR-DOMAIN-EVENT-001–004 | Covered / dependent enforcement | CON-037 only |
| CON-005 | Vendor persistence and retrieval do not introduce persistence concerns into the Domain Model. | AI-REP-001–003 | Applicable persistence and retrieval outcomes | Covered / dependent enforcement | CON-037 only |
| CON-006 | Vendor depends on an application-facing Address boundary. | AI-ADDR-001 | Applicable Address outcomes | Covered / dependent enforcement | CON-037 |
| CON-007 | Address invocation remains outside Domain behaviour. | AI-ADDR-002 | VR-ADDRESS-001, VR-ADDRESS-014–015 | Covered | None |
| CON-008 | Address foreign models are translated into Vendor values. | AI-ADDR-003; AI-PG-001 | VR-ADDRESS-005, VR-ADDRESS-012–013 | Covered | None |
| CON-009 | Permanent contextual references resolve the original immutable result. | AI-ADDR-004–005 | VR-ADDRESS-008–013 | Covered | None |
| CON-010 | Semantic failures fail fast and technical failures do not cause in-process retry or an Epic 1 circuit breaker. | AI-ADDR-005–007 | VR-ADDRESS-008, VR-ADDRESS-014–015 | Covered / dependent enforcement | CON-037 for automated absence rule |
| CON-011 | Application coordinates registration without relocating Domain rules. | AI-APP-001–002 | VR-SUCCESS, VR-INV and VR-FAILURE obligations | Covered / dependent enforcement | CON-037 |
| CON-012 | Complete registration intent is represented independently of transport, Registration Session and authoritative/server-generated state. | AI-APP-003 | VR-REQ-001, VR-REQ-004 and VR-SCOPE-001 | Covered / dependent enforcement | CON-037 for automated dependency enforcement only |
| CON-013 | Vendor identity is normalized Trading Name plus normalized Legal Operator Name plus CanonicalAddressId, with deterministic semantic equivalence and no update through registration. | AI-IDEMP-001–004; AI-APP-003 | VR-REQ-007; VR-IDEMP-001–010; VR-FAILURE-007/009 | Covered | CON-037 for automated dependency enforcement |
| CON-014 | Concurrent equivalent or conflicting submissions cannot create duplicate effects. | AI-IDEMP-003; AI-PG-002 | VR-IDEMP-003 and applicable replay/conflict obligations | Covered | None |
| CON-015 | Qualifying replay returns the permanent original committed outcome. | AI-IDEMP-002; AI-IDEMP-004; AI-PG-001 | VR-IDEMP-001–002; VR-FAILURE-007 | Covered | None |
| CON-016 | Vendor, replay outcome and outbox obligation have one atomic PostgreSQL boundary. | AI-OUT-001–002; AI-IDEMP-002 | VR-RELIABILITY-001–002; VR-FAILURE-006 | Covered | None |
| CON-040 | RegisterVendor returns a closed transport-independent typed Application outcome distinguishing committed success from every expected controlled failure, with one aggregated validation result. | AI-APP-004 | VR-REQ-008, VR-FAILURE-001–003 and existing success, Address, invariant, reliability and idempotency outcome obligations | Covered / dependent enforcement | CON-037 only; CON-024–CON-026 are approved |
| CON-017 | A committed Vendor cannot lose its publication obligation. | AI-OUT-001–005; AI-EVT-002–003 | VR-RELIABILITY, VR-IDEMP and applicable failure obligations | Covered / dependent relay execution | CON-018; CON-021; CON-037 |
| CON-019 | Internal and external event representations are mapped before outbox persistence and never reconstructed by the relay. | AI-DE-003; AI-EVT-001–003 | VR-INTEGRATION-EVENT-001–003, VR-INTEGRATION-EVENT-010 | Covered / dependent relay execution | CON-018 for relay execution; CON-037 for automated enforcement |
| CON-020 | VendorRegistered v1 has an immutable versioned contract that is serialized once, persisted and published unchanged. | AI-EVT-001–003; AI-OUT-001; AI-OUT-005 | VR-INTEGRATION-EVENT-001–010 | Covered / dependent relay execution | CON-018 and CON-021 only |
| CON-023 | HTTP exposes the two operations without importing transport semantics or collaborators into inner layers. | AI-API-001; AI-API-004 | VR-API-001–003, VR-API-011 | Covered / dependent enforcement | CON-037 for automated transport-isolation enforcement only |
| CON-024 | The approved HTTP/JSON contract and generated OpenAPI remain exact and consistent. | AI-API-002; AI-API-004 | VR-API-003–006, VR-API-011–012 | Covered | None |
| CON-025 | Expected and unexpected failures use one safe controlled API boundary and exact approved mappings, including one validation mapping. | AI-API-003–004 | VR-API-007–010 | Covered / dependent enforcement | CON-037 for automated centralization enforcement only |
| CON-026 | Validation ownership, aggregation, canonicalisation, ordering and no-effect guarantees remain explicit across layers. | AI-VAL-001; AI-VO-002; AI-APP-001; AI-APP-004 | VR-REQ-006, VR-REQ-008–010, VR-FAILURE-001–003, VR-CONTACT-002–004, VR-API-003, VR-API-007, VR-API-010 | Covered | None |
| CON-027 | Retrieval uses persisted Vendor as authoritative source and remains side-effect-free. | AI-QRY-001–004 | VR-RETRIEVE-001–008, VR-API-002, VR-API-006, VR-API-008 | Covered | None |
| CON-028 | Aggregate, replay-outcome and outbox data are explicitly mapped and relationally enforced. | AI-PG-001–002; AI-AGG-003; AI-VO-003; AI-ENT-003; AI-REP-003; AI-QRY-004; AI-EVT-002 | Ready persistence, retrieval and event-staging obligations in HJ-107 v1.8 | Covered | CON-029 for migration lifecycle only |

# 8. Derivation Findings and Dependencies

No evidence requires any of the twenty-six Approved concerns to become Challenged.

| Current Concern | Effect on HJ-013 |
|---|---|
| CON-009, CON-010 | Resolved in the controlled architecture baseline; Address value, adapter and failure tests are derivable. |
| CON-012 | Explicitly approved and represented by AI-APP-003; the executable command-contract evidence is derivable while automated dependency enforcement remains subject to CON-037. |
| CON-013 | Explicitly approved and represented by AI-IDEMP-001–004. Application representation, persistence consumption, concurrency and replay evidence are derivable. |
| CON-014 | Resolved; AI-IDEMP-003 and AI-PG-002 own PostgreSQL uniqueness-race and constraint evidence. |
| CON-015 | Resolved; AI-IDEMP-004 and AI-PG-001 own permanent fingerprint/original-result persistence evidence. |
| CON-016 | Resolved; AI-OUT-001–002 own three-part atomic commit and rollback evidence. |
| CON-040 | Explicitly approved and represented by AI-APP-004; validation detail, HTTP mapping and automated cross-project enforcement remain separately governed. |
| CON-018 | Blocks concrete relay retry and restart/recovery tests. |
| CON-019 | Resolved; AI-EVT-001 owns Application mapping, AI-EVT-002 owns immutable persistence and AI-EVT-003 retains the approved no-reconstruction rule at the unresolved relay boundary. |
| CON-020 | Resolved; HJ-107 owns exact v1 contract behaviour while AI-EVT-001–003 own complementary mapping, persistence and relay-mechanism evidence. |
| CON-021 | Blocks broker-specific delivery and recovery assertions. |
| CON-023 | Resolved; AI-API-001 owns transport-isolation enforcement and AI-API-004 delegates observable endpoint behaviour to HJ-107. |
| CON-024 | Resolved; AI-API-002 owns generated OpenAPI congruence and AI-API-004 delegates runtime HTTP/JSON behaviour to HJ-107. |
| CON-025 | Resolved; AI-API-003 owns centralized mapping-boundary enforcement and AI-API-004 delegates observable status/error behaviour to HJ-107. |
| CON-026 | Resolved; AI-VAL-001 delegates exact layered validation and canonicalisation behaviour to HJ-107 without duplicating it. |
| CON-028 | Resolved; real-PostgreSQL mapping, rehydration, repository, constraint, index and restrictive-delete evidence is derivable. CON-029 remains responsible only for migration lifecycle. |
| CON-037 | Blocks selection and implementation of automated dependency, structure and architecture-fitness enforcement. |

The remaining dependencies constrain implementation or execution of tests; they do not invalidate the twenty-six Approved Approaches.

This PR-005 regeneration found no behavioural-catalogue omission, normative source conflict or evidence requiring an Approved concern to become Challenged. It preserves all 49 active obligations and materially amends three to reflect the unified validation-failure decision.

# 9. Generation Readiness

| Readiness | Obligations | Treatment |
|---|---|---|
| **Derivable** | AI-AGG-003; AI-VO-001–003; AI-ENT-003; AI-REP-002–003; AI-OUT-001–002; AI-QRY-001; AI-QRY-004; AI-ADDR-002–006; AI-APP-001; AI-APP-003–004; AI-IDEMP-001–004; AI-PG-001–002; AI-EVT-001–002; AI-API-002 | Generate focused Domain, Application, Persistence Integration, Integration Contract and API Contract test candidates and perform the repository contract review within each obligation's stated boundary. |
| **HJ-107 Coverage** | AI-AGG-001; AI-ENT-001; AI-DE-002; AI-OUT-004; AI-QRY-002; AI-QRY-003; AI-API-004; AI-VAL-001 | Generate no duplicate HJ-013 executable test; use the referenced HJ-107 coverage. |
| **Dependent** | AI-AGG-002; AI-ENT-002; AI-DE-001; AI-DE-003; AI-REP-001; AI-OUT-003; AI-OUT-005; AI-ADDR-001; AI-ADDR-007; AI-APP-002; AI-EVT-003; AI-API-001; AI-API-003 | Retain the obligations, but do not allow test or implementation generation to select unresolved relay, broker or enforcement Approaches. |

HJ-107 test-candidate readiness remains governed by HJ-107. This catalogue does not modify or reconcile any `VR-*` ID.

Readiness for a test candidate does not authorise unrestricted implementation generation. Implementation must stop at every unresolved Current Concern identified in §8.

# 10. Regeneration Reconciliation

## 10.1 AI Test ID Reconciliation

HJ-013 v1.9 is the sole `AI-*` stable-ID baseline.

| AI Test ID | Previous Treatment | Regenerated Treatment | Reason |
|---|---|---|---|
| AI-APP-004 | Closed result represented success and expected failures but did not require one validation result or prohibit the two separate validation result kinds. | Requires one immutable `RequestValidationFailure`, absence of `RegistrationDeclarationFailure` and `ConditionalRuleFailure`, and preserves all other result kinds and exclusions. | Amended CON-040 and HJ-106 v1.8 change the closed result representation. |
| AI-API-003 | One centralized mapper without a prescribed unified validation branch. | Requires exactly one validation-outcome branch and absence of the superseded declaration/conditional branches, while retaining central exception handling. | Amended CON-025, CON-026 and CON-040 define one validation mapping. |
| AI-VAL-001 | Delegated layered validation, aggregate error collection and canonicalisation behaviour. | Delegates the explicit single-result aggregation and unified API mapping to current HJ-107 obligations. | HJ-107 v1.8 preserves the trigger obligations and makes their shared outcome explicit. |
| Remaining 46 active AI-* IDs | Controlled HJ-013 v1.9 treatment. | Preserved without material obligation change. | Their approved architecture and delivery applicability are unchanged. |

Reconciliation totals:

- 49 active `AI-*` IDs retained;
- 46 preserved without material obligation change;
- 3 materially amended;
- 0 added;
- 0 retired, merged, split, superseded, reused or reassigned; and
- 0 unresolved stable-ID mappings.

## 10.2 Behavioural Reference Reconciliation

| AI Test ID | Previous Behavioural Reference | Current Behavioural Reference | Treatment |
|---|---|---|---|
| AI-APP-004 | General HJ-106 success and controlled failures | VR-REQ-008; VR-FAILURE-001–003 plus unchanged success and other failure obligations | Adds explicit unified-result trigger coverage without duplicating behaviour. |
| AI-API-003 | Observable mapping delegated generally to VR-API-007–010 | VR-API-007 and VR-API-010 unified validation mappings | Architecture enforcement remains distinct from observable API behaviour. |
| AI-VAL-001 | VR-REQ-006, VR-REQ-008–010, VR-CONTACT-002–004, VR-API-003, VR-API-007, VR-API-010 | Same references plus VR-FAILURE-001–003 | Preserves distinct trigger coverage while delegating their common result. |
| All other HJ-107-referencing AI-* IDs | Stable IDs in HJ-107 v1.7 | Corresponding current stable IDs in HJ-107 v1.8 | References retained; no behavioural responsibility moved into HJ-013. |

Behavioural reconciliation totals:

- current behavioural baseline: HJ-107 v1.8 Approved;
- 3 `AI-*` obligations materially reconciled to current behavioural coverage;
- 0 missing referenced `VR-*` IDs; and
- 0 `VR-*` IDs created, changed, retired or reconciled by PR-005.

# 11. Review Checklist

- [x] Uses the supplied execution context and HJ-011 v2.0 delivery boundary.
- [x] Derives active obligations only from Approved entries in HJ-012 v2.0.
- [x] Uses HJ-013 v1.9 as the sole `AI-*` stable-ID baseline.
- [x] Retains all 49 active `AI-*` IDs and reconciles the three material amendments.
- [x] Does not create, modify, retire or reconcile any `VR-*` ID.
- [x] References only behavioural IDs present in HJ-107 v1.8 Approved.
- [x] Duplicates no HJ-107 behavioural obligation as an HJ-013 executable test.
- [x] Keeps closed-result structure, mapping-boundary enforcement and observable validation behaviour independently diagnosable.
- [x] Uses only approved Verification Forms, HJ-006 classifications and Derivation Status values.
- [x] Selects no Approach for an unresolved concern.
- [x] Introduces no operation, schema, format, technology or lifecycle transition.
- [x] Records no source conflict or architectural challenge.
- [x] Contains no executable test or implementation code.

# 12. Next Steps

1. Review and apply HJ-013 v2.0 derived from HJ-012 v2.0 and HJ-107 v1.8 Approved.
2. Resume PR-006 for Slice 10C under the approved CON-023–CON-026 and CON-040 validation/API cohort.
3. Generate the next focused test proposal from the reconciled validation obligations; do not recreate separate declaration or conditional result types.
4. Retain relay and broker obligations as Dependent on CON-018 and CON-021.
5. Retain automated enforcement obligations as Dependent where CON-037 remains the governing unresolved enforcement authority.

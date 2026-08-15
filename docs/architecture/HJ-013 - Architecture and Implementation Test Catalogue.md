# HJ-013 - Architecture and Implementation Test Catalogue

| Field | Value |
|---|---|
| **Document ID** | HJ-013 |
| **Document Title** | Architecture and Implementation Test Catalogue |
| **Version** | 1.0 |
| **Status** | Approved |
| **Classification** | Test Catalogue |
| **Owner** | Project Architecture / Engineering |
| **Last Updated** | 15 August 2026 |

## Revision History

| Version | Date | Description |
|---|---|---|
| 0.1 | 14 August 2026 | Initial draft derived from the first HJ-012 approved architecture batch: CON-001 to CON-005, CON-017 and CON-027. |
| 0.2 | 14 August 2026 | Regenerated against HJ-004 v2.3, HJ-106 v1.1 and HJ-107 v0.2 following the Architecture Verification Gap Analysis. Preserves stable `AI-*` IDs, removes behavioural duplication, separates persistence responsibilities, identifies applicable Epic 1 Value Objects, adds CON-037 enforcement dependencies and narrows Entity verification to approved Epic 1 behaviour. |
| 0.3 | 15 August 2026 | Regenerated using PR-005 under CR-043. Adds explicit source authority, delivery scope, concern traceability, generation readiness and reconciliation; corrects AI-AGG-003 and AI-VO-002; preserves all 24 stable `AI-*` IDs and all seven Approved concerns; and changes no HJ-107 behavioural ID. |
| 1.0 | 15 August 2026 | Approved the regenerated Architecture and Implementation Test Catalogue as the controlled architecture-verification baseline. Promotion preserves all 24 stable `AI-*` IDs and all seven Approved concern mappings. HJ-107 is aligned to its v1.0 Approved publication baseline with no behavioural-reference change. |

## Related Documents

| Document ID | Title | Version | Status | Relationship |
|---|---|---:|---|---|
| PR-005 | Generate Architecture and Implementation Test Catalogue | Current | Governing prompt | Defines scope-parameterised HJ-013 derivation, catalogue ownership and stable-ID reconciliation. |
| HJ-004 | Vendor Domain Models | 2.3 | Approved | Authoritative Vendor Aggregate, Value Object, Entity, event and retrieval model. |
| HJ-005 | Coding Standards | 2.0 | Approved | Defines implementation boundaries, Domain modelling, persistence and enforcement expectations. |
| HJ-006 | Testing Strategy and Standards | 2.0 | Approved | Authoritative Test Levels, Test Classifications and catalogue rules. |
| HJ-007 | Enforcement Strategy | 2.0 | Approved | Defines build, static-analysis, dependency-validation and review mechanisms. |
| HJ-010 | Current Application Architectural Concerns | 1.0 | Approved | Complete concern register and source of unresolved dependencies. |
| HJ-011 | Epic 1 Vendor Registration Implementation Scope | 1.1 | Approved | Defines the active delivery boundary for this derivation. |
| HJ-012 | Established Application Architecture Patterns | 1.0 | Approved | Authoritative approved architecture from which this catalogue is derived. |
| HJ-SM-001 | System Model | 1.0 | Approved | Confirms visible component relationships; visibility does not create delivery scope. |
| HJ-106 | Vendor Registration Service Contract | 1.1 | Approved | Approved RegisterVendor and RetrieveRegisteredVendor behavioural contract and architecture boundary. |
| HJ-107 | Vendor Registration Test Catalogue | 1.0 | Approved | Owns behavioural verification; referenced where it already covers a Required Guarantee. |
| ADR-001 | Domain-Driven Design as the Primary Architectural Style | Current | Accepted | Governs the Domain Model and approved DDD patterns. |
| ADR-003 | Event-Driven Collaboration | Current | Accepted | Governs event-driven collaboration and bounded-context autonomy. |
| ADR-004 | Vendor Lifecycle Begins After Successful Registration | Current | Accepted | Governs Vendor identity and lifecycle commencement. |
| ADR-008 | Idempotent Operations and Reliable Event Publication | 1.2 | Accepted | Governs Domain Event separation, publication atomicity and retry guarantees. |

# 1. Purpose

HJ-013 catalogues executable tests and controlled verification obligations required to validate conformance with approved HotJoes application architecture and implementation standards.

This version is limited to the seven Approved concerns in the first HJ-012 baseline:

```text
CON-001, CON-002, CON-003, CON-004, CON-005, CON-017 and CON-027
```

HJ-013 does not complete unresolved architecture. Where verification cannot be implemented without selecting an Approach for a Current Architectural Concern, the obligation remains **Dependent** and identifies that concern.

HJ-013 complements HJ-107. HJ-107 owns service and Domain behaviour derived from HJ-106 and its authoritative sources. HJ-013 owns distinct structural, dependency, concrete persistence, transaction-mechanism, outbox and runtime-mechanism evidence.

# 2. Source Authority

| Artefact | Authority Classification | Use in This Derivation |
|---|---|---|
| HJ-012 v1.0 | Primary approved architecture | Supplies the seven Approved concerns, Required Guarantees, Approaches and verification treatments. |
| HJ-010 v1.0 | Architectural governance | Supplies complete concern states and unresolved `CON-*` dependencies without authorising selection of candidate Approaches. |
| HJ-011 v1.1 | Active delivery scope | Restricts instantiation to the Epic 1 Vendor Registration implementation boundary. |
| HJ-SM-001 v1.0 | System context | Confirms visible relationships; components outside HJ-011 remain outside this derivation. |
| HJ-004 v2.3 | Domain authority | Defines the Vendor Aggregate and the Domain concepts used by the in-scope operations. |
| HJ-106 v1.1 | Service boundary | Defines the approved RegisterVendor and RetrieveRegisteredVendor operations and exclusions. |
| HJ-107 v1.0 | Behavioural catalogue | Owns behavioural obligations and supplies stable `VR-*` coverage references. |
| HJ-005 / HJ-006 / HJ-007 | Engineering and test standards | Govern implementation boundaries, classification and enforcement treatment. |
| ADR-001, ADR-003, ADR-004 and ADR-008 | Accepted decisions | Supply decision authority for the applicable Approved patterns. |
| Controlled HJ-013 v0.2 | Reconciliation baseline only | Sole baseline for preserving and reconciling `AI-*` IDs; not independent architectural authority. |

No source conflict was identified for the seven Approved concerns. Missing detail is preserved through the Current Concern dependencies in §8.

# 3. Scope and Derivation Rules

## 3.1 Scope Boundary

| Boundary | Applied Baseline |
|---|---|
| Approved architecture | HJ-012 v1.0: CON-001 to CON-005, CON-017 and CON-027 |
| Active delivery scope | HJ-011 v1.1 Epic 1 Vendor Registration |
| Applicable operations | RegisterVendor and RetrieveRegisteredVendor |
| Applicable visible architecture | Vendor Domain, PostgreSQL persistence, reliable publication and the Address/Compliance boundaries only where exercised by HJ-011 |
| Visible but excluded architecture | Later Vendor lifecycle commands, Identity behaviour, future queries/read models and other capabilities excluded by HJ-011 |

Approved architecture is instantiated only within HJ-011. The wider Domain Model and System Model do not independently expand this catalogue version.

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

| Concern | Approved Approach | HJ-013 Treatment | Known Boundary |
|---|---|---|---|
| CON-001 | Aggregate | Mutation-boundary enforcement and Aggregate consistency-boundary persistence verification | Creation invariants remain in HJ-107; automated enforcement depends on CON-037; concrete persistence depends on CON-028. |
| CON-002 | Value Object | Equality, immutability/validity and persistence-conversion verification for Epic 1 Value Objects | Behavioural value rules remain in HJ-107; concrete persistence depends on CON-028. |
| CON-003 | Entity | Encapsulation and persisted identity verification | Identity commencement and retrieval behaviour remain in HJ-107; automated enforcement depends on CON-037; concrete persistence depends on CON-028. |
| CON-004 | Domain Event | Domain/infrastructure isolation and Domain/Integration representation separation | Event occurrence remains in HJ-107; automated enforcement depends on CON-037; representation separation also depends on CON-019 and CON-020. |
| CON-005 | Repository | Dependency enforcement, contract review and repository-adapter integration | Service-level persistence/retrieval remains in HJ-107; enforcement depends on CON-037; database behaviour depends on CON-028. |
| CON-017 | Transactional Outbox | Concrete atomicity, rollback, Domain isolation and relay recovery verification | Service-observable publication outcomes remain in HJ-107; execution depends on CON-016, CON-018, CON-020, CON-021, CON-028 and CON-037 as identified per obligation. |
| CON-027 | Query handler + Repository + response mapper | Internal handler/repository/mapper orchestration and concrete mapping integration | Found/Not Found and side-effect behaviour remain in HJ-107; concrete persistence depends on CON-028. |

# 5. Architecture and Implementation Test Catalogue

## 5.1 CON-001 — Aggregate

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-AGG-001 | Aggregate creation preserves invariants | Vendor creation produces a complete valid Aggregate or no Vendor. | CON-001; HJ-004 §8; HJ-106 §§4.5, 4.7; HJ-107 VR-INV-001–005 | HJ-107 Coverage | Applicable HJ-107 classifications | P0 | The cited HJ-107 invariant and success/failure obligations pass; HJ-013 adds no duplicate behavioural test. | HJ-107 owns the behavioural outcome. | HJ-107 |
| AI-AGG-002 | Aggregate Root controls mutation | External code cannot mutate aggregate-owned state by bypassing the Vendor Aggregate Root. | CON-001; HJ-012; ADR-001; HJ-005 §§3.2, 8.2–8.3 | Automated Enforcement | Not applicable | P0 | Structural checks reject prohibited project references, unrestricted public setters and externally accessible mutation paths. | CON-037 dependency-composition and enforcement mechanism. | Dependent |
| AI-AGG-003 | Aggregate consistency boundary is rehydrated as a whole | Repository rehydration reconstructs one complete valid Vendor Aggregate without exposing or independently materialising aggregate-owned parts as separate consistency boundaries. | CON-001; CON-005; HJ-004 §§2.1–2.3; HJ-012; HJ-005 §10 | Executable Test | Persistence Integration / Integration | P0 | A production-equivalent database fixture rehydrates the complete Aggregate with owned-state fidelity and valid invariants through the Aggregate Root. Repository save, lookup and absence semantics remain independently owned by AI-REP-003. | CON-028 PostgreSQL mapping and constraints. | Dependent |

## 5.2 CON-002 — Value Object

For the Epic 1 RegisterVendor and RetrieveRegisteredVendor operations, HJ-004 identifies these applicable Value Objects: `VendorId`, `TradingCharacteristics`, `OpeningHours`, `VendorName`, `CompanyRegistrationNumber`, `PrimaryContact`, `EmailAddress`, `TelephoneNumber`, `CanonicalAddressId` and `BusinessAddressSnapshot`. Only Value Objects actually present in the approved operation model are exercised; later lifecycle-only Value Objects are outside this catalogue version.

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-VO-001 | Epic 1 Value Objects use value equality | Each applicable Value Object is equal when all defining values are equal, unequal when a defining value differs, and has equality-consistent hash behaviour. | CON-002; HJ-004 §2.3 and §9; HJ-012; ADR-001; HJ-005 §§6.4, 8 | Executable Test | Domain / Unit | P1 | Parameterised tests cover every listed Epic 1 Value Object using representative equal and unequal instances. | `BusinessAddressSnapshot` defining fields remain dependent on CON-009 and CON-010; equality can be completed once its approved contract exists. | Dependent |
| AI-VO-002 | Epic 1 Value Objects preserve invariants and immutability | Applicable Value Objects expose invariant-preserving construction and cannot be mutated after construction; accepted and rejected business values remain owned by HJ-107. | CON-002; HJ-004 §2.3; HJ-012; HJ-005 §§2.5, 6.4, 8.1–8.3; HJ-107 | Executable Test | Domain / Unit | P1 | Construction and structural tests demonstrate controlled creation and absence of mutating public state without duplicating HJ-107 validation profiles. | None; CON-026 continues to govern exhaustive validation allocation and profiles in HJ-107, not this structural obligation. | Derivable |
| AI-VO-003 | Value Object persistence conversion is faithful | Each persisted applicable Value Object round-trips without loss, primitive distortion, nullability distortion or weakened validity/equality. | CON-002; CON-005; HJ-004 §2.3; HJ-012; HJ-005 §§10.1–10.4 | Executable Test | Persistence Integration / Integration | P1 | Parameterised persistence-conversion tests diagnose the individual Value Object whose representation fails. | CON-009 and CON-010 for the Business Address Snapshot contract; CON-028 for PostgreSQL mappings and constraints. | Dependent |

## 5.3 CON-003 — Entity

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-ENT-001 | Vendor identity is assigned and stable within approved Epic 1 behaviour | Successful first registration assigns Vendor identity; retrieval exposes the same persisted identity. No later lifecycle command is invented for this test. | CON-003; HJ-004 §§2.2, 2.6; HJ-106 §§4.7–4.8, 4.12; HJ-107 VR-STATE and VR-RETRIEVE | HJ-107 Coverage | Applicable HJ-107 classifications | P0 | HJ-107 creation and retrieval identity obligations pass; later lifecycle-transition identity tests remain outside this catalogue version. | HJ-107 owns current behavioural identity evidence. | HJ-107 |
| AI-ENT-002 | Entity state is encapsulated | Entity state cannot be replaced through unrestricted public setters or significant property-setter behaviour. | CON-003; HJ-012; HJ-005 §§8.2–8.3, 23 | Automated Enforcement | Not applicable | P0 | Structural analysis rejects prohibited public mutation and persistence-only mutation paths exposed to application callers. | CON-037 dependency-composition and enforcement mechanism. | Dependent |
| AI-ENT-003 | Entity identity survives concrete rehydration | The persistence adapter rehydrates Vendor with the same `VendorId` and approved initial lifecycle state stored at registration. | CON-003; CON-005; HJ-004 §§2.2, 2.6; HJ-012; HJ-005 §10 | Executable Test | Persistence Integration / Integration | P0 | Focused database round trip asserts identity and lifecycle fields only; Aggregate completeness and repository-adapter behaviour are owned by AI-AGG-003 and AI-REP-003. | CON-028 PostgreSQL mapping and constraints. | Dependent |

## 5.4 CON-004 — Domain Event

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-DE-001 | Domain Event is infrastructure-free | The Domain Event and Domain project do not reference broker, serialization, transport or Integration Event implementation types. | CON-004; HJ-012; ADR-001; ADR-003; ADR-008 §2.5; HJ-005 §§3.2, 8.4, 17.1 | Automated Enforcement | Not applicable | P0 | Dependency and type checks reject infrastructure or published-contract dependencies in the Domain. | CON-037 dependency-composition and enforcement mechanism. | Dependent |
| AI-DE-002 | Domain Event occurrence follows business state change | Successful first creation records one internal business fact; failures, replay, conflict and retrieval record no additional fact. | CON-004; HJ-004; HJ-106 §§4.9–4.12; HJ-107 VR-DOMAIN-EVENT-001–004 | HJ-107 Coverage | Applicable HJ-107 classifications | P0 | The cited HJ-107 Domain Event obligations pass; HJ-013 adds no duplicate behavioural test. | HJ-107 owns event-occurrence behaviour. | HJ-107 |
| AI-DE-003 | Domain and Integration Event representations remain separate | Internal Domain Event types are not reused as published Integration Event contract types. | CON-004; HJ-012; ADR-003; ADR-008 §2.5 | Automated Enforcement | Not applicable | P0 | Type/dependency checks show distinct internal and published representations with translation outside the Domain. | CON-019 translation placement; CON-020 Integration Event contract; CON-037 enforcement mechanism. | Dependent |

## 5.5 CON-005 — Repository

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-REP-001 | Repository boundary has correct dependency direction | The repository abstraction is available to its approved inner boundary and its concrete persistence implementation remains in Infrastructure. | CON-005; HJ-012; ADR-001; HJ-005 §§3.2, 10.1, 10.4 | Automated Enforcement | Not applicable | P0 | Dependency validation rejects references to a concrete repository from Domain/Application and rejects infrastructure representations leaking through the abstraction. | CON-037 determines exact composition and enforcement; exact abstraction placement must follow the approved solution structure. | Dependent |
| AI-REP-002 | Repository contract exposes aggregate semantics | The repository persists and retrieves the Vendor Aggregate Root without exposing ORM entities, database rows or transport DTOs. | CON-005; HJ-012; HJ-005 §§10.1, 10.4 | Contract Review | Contract Review / Non-executable | P0 | Review confirms aggregate-oriented operations, absence of speculative generic methods and no persistence representation in the contract. | Concrete API remains implementation-local within approved standards. | Derivable |
| AI-REP-003 | Repository adapter fulfils its persistence contract | The concrete repository can save a new Vendor, retrieve it by `VendorId`, and return absence for an unknown identifier without leaking persistence representations or exceptions. | CON-005; HJ-012; HJ-005 §10; HJ-006 §2.1.6 | Executable Test | Persistence Integration / Integration | P0 | Real PostgreSQL or production-equivalent adapter tests prove repository operation semantics; detailed Aggregate, Entity and Value Object fidelity are owned by AI-AGG-003, AI-ENT-003 and AI-VO-003. | CON-028 PostgreSQL mapping and constraints. | Dependent |

## 5.6 CON-017 — Transactional Outbox

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-OUT-001 | Vendor and publication work commit atomically | A successful Vendor commit and its durable publication obligation are committed in one atomic boundary. | CON-017; HJ-012; ADR-008 §2.6; HJ-106 §§4.7, 4.10; HJ-006 §2.1.6 | Executable Test | Persistence Integration / Integration | P0 | Failure injection demonstrates that committed Vendor state always has exactly one corresponding durable publication record. | CON-016 transaction boundary and CON-028 PostgreSQL mappings and constraints. | Dependent |
| AI-OUT-002 | Atomic rollback leaves no partial success | Failure before transaction commit leaves neither committed Vendor state nor durable publication work. | CON-017; HJ-012; ADR-008 §2.6; HJ-106 §§4.10–4.11 | Executable Test | Persistence Integration / Integration | P0 | Failure injection at approved transaction points proves full rollback and no orphan Vendor or outbox record. | CON-016 transaction boundary and CON-028 PostgreSQL mappings and constraints. | Dependent |
| AI-OUT-003 | Outbox is outside Domain decision-making | Domain behaviour records the business fact without referencing outbox persistence, broker or publication infrastructure. | CON-004; CON-017; ADR-008 §§2.5–2.6; HJ-005 §§3.2, 8.4 | Automated Enforcement | Not applicable | P0 | Dependency checks show no outbox, broker or infrastructure types in Domain code. | CON-037 dependency-composition and enforcement mechanism. | Dependent |
| AI-OUT-004 | Publication retry does not repeat the business operation | Retry does not recreate Vendor state, Domain Events, completed facts, publication records or Integration Events. | CON-017; ADR-008 §§2.2, 2.6, 5; HJ-106 §§4.9–4.11; HJ-107 VR-RELIABILITY and VR-IDEMP | HJ-107 Coverage | Applicable HJ-107 classifications | P0 | The cited HJ-107 observable non-duplication obligations pass; AI-OUT-005 supplies distinct mechanism evidence after dependencies resolve. | HJ-107 owns the behavioural outcome. | HJ-107 |
| AI-OUT-005 | Relay recovery preserves one publication obligation | Following relay failure or restart, durable work remains recoverable and retry does not repeat the original business operation. | CON-017; ADR-008 §§2.6, 5; HJ-106 §4.10 | Executable Test | Integration Contract / Integration | P0 | Relay failure/restart tests demonstrate recovery from the durable record and preservation of the no-repeat guarantees at the infrastructure boundary. | CON-018 relay; CON-019 translation; CON-020 event contract; CON-021 broker semantics. | Dependent |

## 5.7 CON-027 — Query Handler, Repository and Response Mapper

| Test ID | Title | Requirement | Source | Verification Form | Classification / Level | Priority | Expected Evidence | Dependency | Status |
|---|---|---|---|---|---|---|---|---|---|
| AI-QRY-001 | Retrieval uses the approved handler, repository and mapper roles | The query handler obtains persisted Vendor through the Repository and delegates production of a purpose-specific response without using an alternative state source or exposing the Aggregate. | CON-027; HJ-004 §2.6; HJ-106 §4.12; HJ-012; HJ-005 §§9.2–9.4, 10.4 | Executable Test | Application / Unit | P1 | Collaborator-focused tests show one repository lookup by `VendorId`, mapping from returned Vendor, and no alternative persistence/read-model collaborator. | Response content remains governed by HJ-004/HJ-106 and HJ-107. | Derivable |
| AI-QRY-002 | Retrieval is side-effect-free | Retrieval does not save Vendor state, record an event, create publication work or invoke a publisher. | CON-027; HJ-106 §4.12.3; HJ-107 VR-RETRIEVE-006 and related obligations | HJ-107 Coverage | Applicable HJ-107 classifications | P1 | The cited HJ-107 retrieval side-effect obligations pass; HJ-013 adds no duplicate application test. | HJ-107 owns the behavioural outcome. | HJ-107 |
| AI-QRY-003 | Retrieval returns controlled Not Found | Repository absence produces the approved application-level Not Found outcome without infrastructure leakage. | CON-027; HJ-106 §§4.12.2–4.12.3; HJ-107 VR-RETRIEVE-007 | HJ-107 Coverage | Applicable HJ-107 classifications | P1 | The cited HJ-107 Not Found obligation passes; HTTP mapping remains non-normative pending CON-024. | HJ-107 owns business Not Found; CON-024 governs HTTP representation. | HJ-107 |
| AI-QRY-004 | Concrete retrieval adapter and mapper preserve their boundaries | A real persisted Vendor is rehydrated through the Repository and mapped into Registered Vendor Details without persistence representations entering the response. | CON-027; CON-005; HJ-004 §2.6; HJ-106 §4.12; HJ-012; HJ-005 §§9.4, 10.4–10.5 | Executable Test | Persistence Integration / Integration | P1 | Integration test focuses on repository-to-mapper boundary, purpose-specific response mapping and absence of ORM/database types; HJ-107 owns response values and side-effect behaviour. | CON-009 and CON-010 for concrete Business Address Snapshot fields; CON-028 for PostgreSQL mapping. | Dependent |

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

The same Required Guarantee may have tests at distinct boundaries, but identical stimuli and evidence shall not be duplicated across the catalogues.

# 7. Concern-to-Test Traceability

| Approved Concern | Required Guarantee | AI Test IDs | HJ-107 Coverage | Coverage Status | Dependencies |
|---|---|---|---|---|---|
| CON-001 | Vendor invariants are protected within one consistency boundary. | AI-AGG-001–003 | VR-INV-001–005 and applicable success/failure obligations | Covered / dependent execution | CON-028; CON-037 |
| CON-002 | Identity-free concepts remain explicit and do not degrade into primitive obsession. | AI-VO-001–003 | Applicable field, validation and canonicalisation obligations | Covered / dependent execution | CON-009; CON-010; CON-028; CON-026 remains behavioural validation authority only |
| CON-003 | Vendor identity persists through valid in-scope behaviour. | AI-ENT-001–003 | VR-STATE and VR-RETRIEVE obligations | Covered / dependent execution | CON-028; CON-037 |
| CON-004 | Registration records an internal fact without infrastructure coupling. | AI-DE-001–003 | VR-DOMAIN-EVENT-001–004 | Covered / dependent execution | CON-019; CON-020; CON-037 |
| CON-005 | Vendor persistence and retrieval do not introduce persistence concerns into the Domain Model. | AI-REP-001–003 | Applicable persistence and retrieval outcomes | Covered / dependent execution | CON-028; CON-037 |
| CON-017 | A committed Vendor cannot lose its publication obligation. | AI-OUT-001–005 | VR-RELIABILITY, VR-IDEMP and applicable failure obligations | Covered / dependent execution | CON-016; CON-018–CON-021; CON-028; CON-037 |
| CON-027 | Retrieval uses persisted Vendor as authoritative source and remains side-effect-free. | AI-QRY-001–004 | VR-RETRIEVE-001–008 | Covered / dependent execution | CON-009; CON-010; CON-024; CON-028 |

# 8. Derivation Findings and Dependencies

No evidence requires CON-001 to CON-005, CON-017 or CON-027 to become Challenged.

| Current Concern | Effect on HJ-013 |
|---|---|
| CON-009, CON-010 | Block complete Business Address Snapshot Value Object and retrieval-mapping verification. |
| CON-016 | Blocks executable proof of the atomic registration transaction boundary. |
| CON-018 | Blocks concrete relay retry and restart/recovery tests. |
| CON-019 | Blocks complete enforcement and execution of Domain Event to Integration Event translation. |
| CON-020 | Blocks concrete Integration Event representation and relay contract tests. |
| CON-021 | Blocks broker-specific delivery and recovery assertions. |
| CON-024 | Keeps HTTP Not Found and other technical API representations outside normative HJ-013 verification. |
| CON-026 | Blocks exhaustive EmailAddress/TelephoneNumber validation-profile and allocation evidence; tests must not invent accepted or rejected formats. |
| CON-028 | Blocks real PostgreSQL mapping, rehydration, constraint and repository/outbox persistence tests. |
| CON-037 | Blocks selection and implementation of automated dependency, structure and architecture-fitness enforcement. |

These dependencies constrain implementation or execution of tests; they do not invalidate the seven Approved Approaches.

The PR-005 regeneration found no behavioural-catalogue omission, source conflict or evidence requiring an Approved concern to become Challenged. It corrected two HJ-013 derivation defects: circular persistence-responsibility wording and avoidable blocking of Value Object immutability evidence.

# 9. Generation Readiness

| Readiness | Obligations | Treatment |
|---|---|---|
| **Derivable** | AI-VO-002; AI-REP-002; AI-QRY-001 | Generate the focused Domain test, perform the repository contract review and generate the query-orchestration Application test. |
| **HJ-107 Coverage** | AI-AGG-001; AI-ENT-001; AI-DE-002; AI-OUT-004; AI-QRY-002; AI-QRY-003 | Generate no duplicate HJ-013 executable test; use the referenced HJ-107 coverage. |
| **Dependent** | AI-AGG-002–003; AI-VO-001; AI-VO-003; AI-ENT-002–003; AI-DE-001; AI-DE-003; AI-REP-001; AI-REP-003; AI-OUT-001–003; AI-OUT-005; AI-QRY-004 | Retain the obligations, but do not allow test or implementation generation to select unresolved Approaches, mappings, contracts or enforcement mechanisms. |

HJ-107 test-candidate readiness remains governed by HJ-107. This catalogue does not modify or reconcile any `VR-*` ID.

Readiness for a test candidate does not authorise unrestricted implementation generation. Implementation must stop at every unresolved Current Concern identified in §8.

# 10. Regeneration Reconciliation

## 10.1 AI Test ID Reconciliation

| AI Test ID | Previous Treatment | Regenerated Treatment | Reason |
|---|---|---|---|
| AI-AGG-003 | Complete Aggregate rehydration, but delegated “aggregate equivalence” and adapter CRUD to AI-REP-003. | Owns complete Aggregate owned-state fidelity and invariant-valid rehydration; delegates only repository operation semantics to AI-REP-003. | Removes circular responsibility while preserving the essential Aggregate obligation. |
| AI-VO-002 | Combined immutability with unresolved concrete validation profiles and was Dependent on CON-026. | Narrows to invariant-preserving construction architecture and immutability; becomes Derivable. | Separates derivable structural evidence from HJ-107-owned validation profiles. |
| Remaining 22 `AI-*` IDs | Existing v0.2 treatment. | Preserved without material obligation change. | Current authority and delivery applicability are unchanged. |

Reconciliation totals:

- 24 preserved `AI-*` IDs;
- 2 materially amended under their existing IDs;
- 22 otherwise unchanged IDs;
- 0 added IDs;
- 0 retired, merged, split, superseded or reused IDs; and
- 0 unresolved stable-ID mappings.

## 10.2 Behavioural Reference Reconciliation

| Treatment | Result |
|---|---|
| Current behavioural baseline | HJ-107 v1.0 |
| Changed `VR-*` references | 0 |
| Missing referenced `VR-*` IDs | 0 |
| `VR-*` IDs created, changed, retired or reconciled by PR-005 | 0 |

# 11. Review Checklist

- [x] Uses HJ-012 v1.0 as the Approved architecture baseline.
- [x] Uses HJ-011 v1.1 rather than System Model visibility to define the active delivery boundary.
- [x] Instantiates only the seven applicable Approved concerns.
- [x] Preserves all 24 `AI-*` IDs from controlled HJ-013 v0.2.
- [x] Creates, changes and reconciles no `VR-*` ID.
- [x] Delegates existing behavioural obligations to HJ-107 without duplication.
- [x] Assigns distinct Aggregate, Entity, Value Object, Repository and query-mapping responsibilities.
- [x] Removes circular evidence delegation between AI-AGG-003 and AI-REP-003.
- [x] Separates AI-VO-002 immutability evidence from unresolved validation profiles.
- [x] Uses one HJ-006 Classification / Level for every executable obligation.
- [x] Invents no Architecture Test classification.
- [x] Uses only Derivable, Dependent and HJ-107 statuses.
- [x] Identifies an owning Current Concern for unresolved dependencies.
- [x] Selects no unresolved Approach, schema, mapping, technology or later lifecycle operation.
- [x] Finds no evidence requiring an Approved concern to become Challenged.
- [x] Contains no executable test or implementation code.

# 12. Next Steps

1. Resolve the Current Concerns that block the intended first implementation slice.
2. Generate test and implementation candidates only within the readiness constraints in §9.

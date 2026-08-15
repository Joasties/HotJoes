# HJ-012 - Established Application Architecture Patterns

| Field | Value |
|---|---|
| **Document ID** | HJ-012 |
| **Document Title** | Established Application Architecture Patterns |
| **Version** | 1.1 |
| **Status** | Approved |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 15 August 2026 |

## Revision History

| Version | Date | Description |
|---|---|---|
| 0.1 | 14 August 2026 | Initial draft of the established application architecture catalogue. |
| 1.0 | 14 August 2026 | Applied CR-039. Aligned the table with HJ-010, removed Pattern IDs, added CON-027, published the first seven Approved concern resolutions and established challenge retention and synchronization rules. |
| 1.1 | 15 August 2026 | Applied CR-044. Aligned HJ-012 with HJ-107 v1.0 Approved and HJ-013 v1.0 Approved, replaced initial-generation wording with independent regeneration rules, and changed no Approved concern, Approach, Required Guarantee or verification responsibility. |

## Related Documents

| Document ID | Title | Status | Relationship |
|---|---|---|---|
| HJ-SM-001 v1.0 | System Model | Approved | Defines the approved system boundary within which the patterns apply. |
| HJ-002 | Architectural Principles | Approved | Provides architectural constraints and evaluation criteria. |
| HJ-004 | Vendor Domain Models | Approved | Defines the first application of the approved Domain Model patterns. |
| HJ-005 | Coding Standards | Approved | Governs implementation conventions supporting the established patterns. |
| HJ-006 | Testing Strategy and Standards | Approved | Governs verification of architectural guarantees. |
| HJ-007 | Enforcement Strategy | Approved | Defines applicable architecture and engineering enforcement. |
| HJ-010 v1.0 | Current Application Architectural Concerns | Approved | Complete concern register and authority for each concern's current Resolution State. |
| HJ-011 v1.1 | Epic 1 Vendor Registration Implementation Scope | Approved | Defines the delivery scope in which the first approved architecture applies. |
| HJ-106 | Vendor Registration Service Contract | Approved | Downstream contract assessed where approved architecture affects observable service behaviour. |
| HJ-107 v1.0 | Vendor Registration Test Catalogue | Approved | Approved behavioural test catalogue for applicable externally observable guarantees. |
| HJ-013 v1.0 | Architecture and Implementation Test Catalogue | Approved | Approved test catalogue for application architecture and implementation guarantees. |
| ADR-001 | Domain-Driven Design as the Primary Architectural Style | Accepted | Governs Aggregate, Entity, Value Object and Repository use. |
| ADR-003 | Event-Driven Collaboration | Accepted | Governs Domain Event and integration collaboration boundaries. |
| ADR-004 | Vendor Lifecycle Begins After Successful Registration | Accepted | Governs Vendor identity and lifecycle application. |
| ADR-008 | Idempotent Operations and Reliable Event Publication | Accepted | Governs Domain Event separation and Transactional Outbox guarantees. |
| CR-039 | Establish Complete Concern Tracking and Publish First Approved Architecture Batch | Applied | Authorises this first approved architecture baseline and synchronization with HJ-010. |

## 1. Purpose

HJ-012 is the authoritative catalogue of application architecture that has received the applicable decision approval and entered the HotJoes application architecture baseline.

It gives developers one place to identify:

- the architectural concern and Required Guarantee;
- the approved Approach;
- its scope and decision authority;
- its Resolution State and Priority; and
- its verification treatment.

**Established** means selected and approved architecture. It does not mean that implementation has already been generated or that all applicable tests have passed. Implementation and test execution validate conformance with the approved architecture.

Every concern that enters HJ-012 remains represented here. A Challenged entry is retained for traceability but is not active implementation authority until its resolution is approved again.

HJ-012 does not reproduce ADR rationale, detailed implementation design or test specifications.

## 2. Relationship with HJ-010

HJ-010 is the complete concern register and is authoritative for concern identity and current Resolution State. HJ-012 is authoritative for approved architectural resolutions.

The two concern tables use the same nine-column structure and column order:

```text
ID | Architectural Concern | Required Guarantee | Scope / Source | Approach | Resolution State | Priority | Decision Treatment / Source | Verification Treatment
```

Their responsibilities differ through row population and state treatment:

| HJ-010 Resolution State | HJ-012 Treatment |
|---|---|
| Exploring | No entry for a concern that has never been Approved. |
| Selected | No entry for a concern that has never been Approved. |
| Blocked | No entry for a concern that has never been Approved. |
| Approved | Exactly one matching entry with Resolution State Approved. |
| Challenged | If previously Approved, exactly one matching entry with Resolution State Challenged. |

A previously Approved concern uses Challenged while architectural reconsideration is active. HJ-010 may then record the last approved Approach and alternatives under consideration; HJ-012 preserves the previously approved resolution until a replacement is approved.

## 3. Entry and Challenge Rules

An Approach enters HJ-012 when:

1. exactly one Approach has been selected for the architectural concern;
2. its Required Guarantee is sufficiently explicit to guide implementation;
3. the applicable ADR, principle, standard, convention or other decision authority has approved the resolution; and
4. its verification treatment is identifiable.

The concern remains in HJ-010 and both entries have Resolution State **Approved**.

If downstream derivation or evidence exposes a potential architectural deficiency:

- the same stable `CON-xxx` changes to Challenged in HJ-010;
- its HJ-012 entry remains present and changes to Challenged;
- the HJ-012 entry ceases to be active implementation authority;
- reconsideration details and candidate Approaches are managed in HJ-010; and
- once the reconsidered resolution receives approval, both entries return to Approved and HJ-012 is updated with the approved replacement.

A normal implementation defect does not challenge approved architecture. The implementation must be corrected to conform to HJ-012.

## 4. Established Application Architecture Patterns

| ID | Architectural Concern | Required Guarantee | Scope / Source | Approach | Resolution State | Priority | Decision Treatment / Source | Verification Treatment |
|---|---|---|---|---|---|---|---|---|
| CON-001 | Vendor consistency boundary | Vendor creation and lifecycle invariants are protected within one consistency boundary. | HJ-004; ADR-001 | Aggregate | Approved | P0 | Existing ADR: ADR-001; HJ-004 | HJ-107 domain/application tests; HJ-013 aggregate-boundary tests; architecture review. |
| CON-002 | Identity-free business concepts | Immutable or identity-free domain concepts are explicit and do not degrade into primitive obsession. | HJ-003; HJ-004; HJ-005 | Value Object | Approved | P1 | Existing ADR: ADR-001; Engineering Standard: HJ-005 | HJ-107/domain unit tests; HJ-013 value-object implementation tests; architecture/code review. |
| CON-003 | Vendor identity and lifecycle state | Vendor identity persists while lifecycle-bearing state changes through valid domain behaviour. | HJ-004; ADR-004 | Entity | Approved | P0 | Existing ADRs: ADR-001, ADR-004 | HJ-107 lifecycle/creation tests; HJ-013 entity identity and boundary tests. |
| CON-004 | Registration business fact | Successful registration raises an internal business fact without coupling the Domain Model to external messaging. | HJ-004; HJ-105; ADR-008 | Domain Event | Approved | P0 | Existing ADRs: ADR-003, ADR-008 | HJ-107 event/no-event behavioural tests; HJ-013 Domain Event isolation tests; architecture review. |
| CON-005 | Aggregate persistence abstraction | Vendor persistence and retrieval do not introduce persistence concerns into the Domain Model. | HJ-004; HJ-005 | Repository | Approved | P0 | Existing ADR: ADR-001; Engineering Standard: HJ-005 | HJ-013 repository contract, persistence integration and dependency-direction tests. |
| CON-017 | Reliable publication staging | A committed Vendor cannot lose the obligation to publish VendorRegistered. | HJ-011 §2.5; ADR-008 | Transactional Outbox | Approved | P0 | Existing ADR: ADR-008 | HJ-107 publication outcomes; HJ-013 atomicity, failure-injection and recovery tests. |
| CON-027 | Registered Vendor retrieval | Retrieval loads the persisted aggregate as authoritative source, maps a purpose-specific response, is side-effect-free and returns controlled Not Found. | HJ-004; HJ-105; HJ-106 §4.12; HJ-011 | Query handler + Repository + response mapper | Approved | P1 | Existing ADR: ADR-001; approved HJ-004/HJ-106 contract; implementation-local handler style | HJ-107 VR-RETRIEVE tests; HJ-013 repository and mapping integration tests. |

## 5. Implementation and Verification Derivation

The Approved entries form the current application architecture baseline for downstream generation:

```text
HJ-012 Approved architecture
    -> assess and regenerate HJ-106 where service or integration behaviour is affected
    -> independently regenerate HJ-107 behavioural tests where its authoritative inputs materially change
    -> independently regenerate HJ-013 architecture and implementation tests where its authoritative inputs materially change
    -> generate implementation
    -> execute HJ-107 and HJ-013 tests
```

HJ-107 verifies externally observable Vendor Registration behaviour derived from HJ-106 and its authoritative sources. HJ-013 verifies application architecture, dependency, persistence, transaction, reliable-publication and other implementation guarantees derived from applicable approved architecture, ADRs and engineering standards.

The test catalogues must not invent missing architectural semantics. If correct downstream artefacts cannot be derived, or the approved Approach cannot fulfil its Required Guarantee, the affected concern becomes Challenged. Incorrectly derived tests are corrected downstream; normal implementation defects are corrected in the implementation.

## 6. Initial Batch Reconciliation

| Field | Result |
|---|---|
| **Approved concerns** | CON-001, CON-002, CON-003, CON-004, CON-005, CON-017 and CON-027 |
| **Approved Approaches** | Aggregate; Value Object; Entity; Domain Event; Repository; Transactional Outbox; Query handler + Repository + response mapper |
| **HJ-010 effect** | All seven concerns remain in HJ-010 with Resolution State Approved. |
| **HJ-012 effect** | All seven matching resolutions form the first Approved application architecture baseline. |
| **Downstream generation** | Assess HJ-106 impact, then independently regenerate HJ-107 and HJ-013 where their authoritative inputs materially change. |
| **Publication date** | 14 August 2026 |

## 7. Current Approved Baseline

Developers implementing the affected HotJoes application architecture shall use the seven Approved entries in §4 and preserve their Required Guarantees.

Later approved resolutions are added without removing their concern from HJ-010. Challenged resolutions remain visible here but cease to be active implementation authority until re-approved.

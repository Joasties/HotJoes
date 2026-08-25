# HJ-012 - Established Application Architecture Patterns

| Field | Value |
|---|---|
| **Document ID** | HJ-012 |
| **Document Title** | Established Application Architecture Patterns |
| **Version** | 1.8 |
| **Status** | Approved |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 23 August 2026 |

## Revision History

| Version | Date | Description |
|---|---|---|
| 0.1 | 14 August 2026 | Initial draft of the established application architecture catalogue. |
| 1.0 | 14 August 2026 | Applied CR-039. Aligned the table with HJ-010, removed Pattern IDs, added CON-027, published the first seven Approved concern resolutions and established challenge retention and synchronization rules. |
| 1.1 | 15 August 2026 | Applied CR-044. Aligned HJ-012 with HJ-107 v1.0 Approved and HJ-013 v1.0 Approved, replaced initial-generation wording with independent regeneration rules, and changed no Approved concern, Approach, Required Guarantee or verification responsibility. |
| 1.2 | 17 August 2026 | Applied CR-046. Added the six approved Address collaboration and RegisterVendor orchestration resolutions CON-006–CON-011, synchronized transactionally with HJ-010 v1.2. |
| 1.3 | 18 August 2026 | Added the approved CON-012 transport-independent RegisterVendorCommand resolution, synchronized transactionally with HJ-010 v1.3. |
| 1.4 | 19 August 2026 | Reconciled the approved CON-040 transport-independent `RegisterVendorResult` decision as part of the synchronized HJ-010/HJ-012 v1.4 architecture baseline. |
| 1.5 | 19 August 2026 | Reconciled the approved CON-013 composite Vendor uniqueness identity and semantic registration-equivalence decision as part of the synchronized HJ-010/HJ-012 v1.5 architecture baseline. |
| 1.6 | 21 August 2026 | Reconciled the approved CON-014–CON-016 and CON-028 PostgreSQL-backed concurrency, permanent replay outcome, atomic registration transaction and explicit EF Core mapping cohort as part of the synchronized HJ-010/HJ-012 v1.6 architecture baseline. |
| 1.7 | 22 August 2026 | Applied CR-060. Published the approved CON-019 pre-outbox Integration Event translation and CON-020 VendorRegistered v1 contract resolutions, synchronized transactionally with HJ-010 v1.7. |
| 1.8 | 23 August 2026 | Applied CR-TBD-HJ012. Published the approved amended CON-020 concrete VendorRegistered v1 JSON representation, synchronized transactionally with HJ-010 v1.8. |

## Related Documents

| Document ID | Title | Status | Relationship |
|---|---|---|---|
| HJ-SM-001 v1.0 | System Model | Approved | Defines the approved system boundary within which the patterns apply. |
| HJ-002 | Architectural Principles | Approved | Provides architectural constraints and evaluation criteria. |
| HJ-004 | Vendor Domain Models | Approved | Defines the first application of the approved Domain Model patterns. |
| HJ-005 | Coding Standards | Approved | Governs implementation conventions supporting the established patterns. |
| HJ-006 | Testing Strategy and Standards | Approved | Governs verification of architectural guarantees. |
| HJ-007 | Enforcement Strategy | Approved | Defines applicable architecture and engineering enforcement. |
| HJ-010 v1.8 | Current Application Architectural Concerns | Approved | Synchronized concern register and authority for each concern's current Resolution State. |
| HJ-011 v1.8 | Epic 1 Vendor Registration Implementation Scope | Approved | Delivery scope aligned to CON-013–CON-017, CON-019, amended CON-020 and CON-028. |
| HJ-106 | Vendor Registration Service Contract | Approved | Downstream contract assessed where approved architecture affects observable service behaviour. |
| HJ-107 v1.5 | Vendor Registration Test Catalogue | Approved | Current approved behavioural test catalogue; regeneration is required after the amended CON-020 contract reaches HJ-106. |
| HJ-013 v1.7 | Architecture and Implementation Test Catalogue | Approved | Current approved architecture and implementation test catalogue; regeneration is required after HJ-107. |
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
| CON-006 | Vendor-to-Address dependency direction | Address remains authoritative and Vendor depends only on an application-facing boundary, not an Address implementation or stub. | HJ-SM-001; HJ-011 §4.1; ADR-006 | Vendor application port + Address adapter; Anti-Corruption Layer | Approved | P0 | Approved under ADR-006. The Vendor Application depends on an application-facing Address Resolution port. Concrete Address Domain integration and the Epic 1 Address stub are implemented as adapters outside the Vendor Domain. Address-owned representations are translated through an Anti-Corruption Layer. | HJ-013 dependency-direction and architecture tests; Address port contract tests; adapter integration tests; architecture review confirming that the Vendor Domain has no dependency on an Address implementation or stub. |
| CON-007 | Address capability invocation | Registration can resolve authoritative Address information synchronously without invocation mechanics entering domain behaviour. | HJ-105; HJ-106 Part A; HJ-011 §4.1 | Synchronous typed Address Resolution through the Vendor application port and Address adapter | Approved | P0 | Approved application-boundary decision under ADR-006. The Vendor Application resolves the submitted opaque Address Resolution reference synchronously through its typed Address port. Invocation mechanics remain in the concrete adapter and do not enter Vendor Domain behaviour. | Address port contract tests; adapter integration tests; Vendor Application orchestration tests; HJ-107 observable Address-resolution outcomes after upstream propagation; architecture tests confirming that invocation mechanics do not enter the Vendor Domain. |
| CON-008 | Address contract translation | Address-owned concepts are translated by the Address adapter into the Vendor’s immutable BusinessAddressSnapshot, CanonicalAddressId and regulatory-authority values without foreign-model leakage or Vendor-side derivation. | HJ-004; HJ-104; HJ-106; ADR-006 | Explicit Address adapter mapper/translator forming the Vendor Anti-Corruption Layer | Approved | P0 | Approved under ADR-006. The Address adapter translates the complete authoritative Address result into Vendor Domain values without exposing the Address Domain representation. It creates the immutable BusinessAddressSnapshot containing optional RecipientOrOrganisationName, required AddressLine1, optional AddressLine2 and AddressLine3, required PostTown and Postcode, and optional County. It also translates CanonicalAddressId, FoodRegistrationAuthority and conditional PrimaryTradingAuthority. The Vendor Domain does not parse, derive, normalise or replace Address-owned information. | HJ-013 Address adapter mapping and Anti-Corruption Layer tests; Address contract tests; BusinessAddressSnapshot immutability and exact-value tests; HJ-107 observable snapshot and authority outcomes after upstream propagation. |
| CON-009 | Address Resolution reference contract | Vendor Registration accepts only a permanent opaque Address Resolution reference issued for a complete authoritative Address result bound to the declared Trading Location. Repeated resolution deterministically returns the original immutable result, and a mismatched Trading Location context is rejected. | HJ-004; HJ-104; HJ-105; HJ-106; ADR-006 | Permanent opaque Address Resolution reference bound to an immutable Address result and declared Trading Location | Approved | P0 | Approved consumed Address contract under ADR-006. A reference is issued only after the Address Domain has produced a complete valid result for the declared Trading Location. It is permanent, opaque, reusable, non-consuming, has no expiry or revocation, and binds the original immutable CanonicalAddressId, BusinessAddressSnapshot and regulatory-authority values. Unknown or fabricated references return InvalidReference. A reference used with a different Trading Location, or a known selection unable to satisfy the required context, returns InvalidAddressResult. | Address consumed-contract tests; client Address-selection and progression tests; Vendor Application reference-resolution tests; Trading Location binding tests; immutable repeated-resolution tests; HJ-107 behavioural tests and HJ-013 adapter/contract tests after upstream propagation. |
| CON-010 | Address failure and resilience | Vendor Registration deterministically handles invalid Address references, invalid contextual Address results and technical Address unavailability without partial Vendor creation, unsafe retry or leakage of Address implementation details. | HJ-105; HJ-106; HJ-011 §4.1 | Fail fast for semantic Address failures; no in-process automatic retry for technical failures; controlled retryable application failure | Approved | P0 | Approved following CON-009. InvalidReference and InvalidAddressResult fail fast and prevent Vendor creation. Address timeout, unavailability or transient invocation failure produces a controlled retryable Vendor Application failure with no in-process automatic retry. The caller may retry RegisterVendor using the same permanent Address Resolution reference. No circuit breaker is introduced for Epic 1. | Vendor Application tests for InvalidReference and InvalidAddressResult; timeout and unavailability failure-injection tests; assertions that Vendor creation, Domain Events, persistence and publication work do not occur; safe caller-retry tests; HJ-107 observable failure outcomes and HJ-013 adapter resilience tests after upstream propagation. |
| CON-011 | RegisterVendor orchestration | The application layer coordinates registration without moving domain rules into transport or infrastructure code. | HJ-004; HJ-105; HJ-106 | Vendor Application Service orchestrating the RegisterVendor use case | Approved | P0 | Approved application-layer decision governed by HJ-005, HJ-004, HJ-105 and HJ-106. The Vendor Application Service coordinates request validation, synchronous Address Resolution through the application port, Vendor aggregate creation, persistence and the approved reliable-publication boundary. Domain rules remain in the Vendor Domain; transport and infrastructure mechanics remain outside the application use case. | Vendor Application orchestration tests; collaborator and prohibited-call assertions; architecture tests confirming dependency direction and absence of Domain-rule duplication; HJ-107 RegisterVendor behavioural tests after upstream propagation. |
| CON-012 | Registration intent representation | A complete Vendor Registration intent is represented independently of HTTP request models and client/BFF Registration Session state, without containing an already-created Vendor or client-authoritative Address-owned values. | HJ-104; HJ-105; HJ-106 §§4.1–4.3; ADR-004; ADR-006; ADR-008 | Immutable transport-independent `RegisterVendorCommand` owned by the Vendor Application | Approved | P0 | Approved application-boundary decision. The command carries all client-authored registration fields, the opaque Address Resolution reference and transient declarations. Address-owned, Domain-generated, persistence and publication values are excluded. Idempotency mechanics remain under CON-013–CON-016. | Application unit tests for complete command retention, transport independence, Address-owned-field exclusion and RegisterVendor orchestration; architecture enforcement remains subject to CON-037. |
| CON-013 | Idempotency identity and request equivalence | Epic 1 identifies an existing Vendor registration by trimmed, case-insensitive Trading Name plus trimmed, case-insensitive Legal Operator Name plus CanonicalAddressId. An equivalent repeated submission returns the original committed result; the same identity with materially different registration information returns IdempotencyConflict and never updates the Vendor. | HJ-004; HJ-104; HJ-106 §§4.8–4.10; ADR-008; HJ-107 VR-BLOCKED-005 | Composite Vendor uniqueness identity + deterministic semantic registration fingerprint | Approved | P0 | Approved Application and persistence-boundary decision. Vendor uniqueness identity comprises trimmed, case-insensitive TradingName, trimmed, case-insensitive LegalOperatorName and CanonicalAddressId. Semantic equivalence compares the complete materially relevant registration information after its approved canonicalisation, excluding transient Registration Declarations, the opaque Address Resolution reference, server-generated values and technical metadata. Equivalent replay returns the original committed successful result without repeating any business effect. The same identity with different material information returns IdempotencyConflict and does not update the Vendor. Vendor updates require a separate future administration operation. CON-014–CON-016 and CON-028 govern concurrency, replay persistence, transaction and database mechanics. | Application tests for trimmed and case-insensitive identity matching, equivalent replay, materially different conflict and absence of update or repeated business effects. CON-014, CON-015, CON-016 and CON-028 provide separate concurrency, persisted replay, atomicity and database-constraint evidence. |
| CON-014 | Concurrent duplicate coordination | Concurrent equivalent or conflicting submissions cannot create duplicate Vendors, outcomes, facts or publication work. | ADR-008; HJ-105; HJ-106 | PostgreSQL composite-identity uniqueness constraint + transaction-coordinated duplicate resolution | Approved | P0 | Approved persistence-boundary decision. PostgreSQL is the concurrency authority for RegisterVendor. A database-enforced unique constraint over the approved normalized composite Vendor identity permits only one Vendor registration to commit. A competing submission that loses the uniqueness race commits no business effect and, after the winning transaction completes, loads the committed registration record. Semantically equivalent information returns the original committed successful result; materially different information returns IdempotencyConflict. No process-local lock, distributed lock or separate request-coordination service is used. ADR-008 amendment required. | Real-PostgreSQL concurrent-submission tests prove that equivalent and conflicting races converge on one Vendor, one persisted successful outcome and one outbox item. Losing transactions create no Vendor, Domain Event, publication work or other business effect. |
| CON-015 | Idempotency outcome persistence and replay | A qualifying retry returns the original committed outcome without re-executing the business operation; retention is explicit. | ADR-008; HJ-106; HJ-107 VR-BLOCKED-005 | Permanent persisted registration outcome + versioned deterministic semantic fingerprint | Approved | P0 | Approved Application and persistence-boundary decision. Each successful registration persists the original RegisterVendor Application result and a SHA-256 fingerprint calculated from a versioned deterministic UTF-8 canonical representation of the materially relevant registration information defined by CON-013. The persisted representation excludes transient Registration Declarations, the opaque Address Resolution reference, server-generated values and technical metadata. The outcome does not expire and is retained for at least as long as the Vendor registration exists; Epic 1 provides no deletion or expiry operation. Equivalent replay returns the persisted original result and does not reconstruct it from the Vendor’s current lifecycle state. ADR-008 amendment required. | Persistence integration tests prove deterministic fingerprinting, permanent retention, faithful original-result replay after Vendor lifecycle state changes, equivalent replay without repeated effects and conflict for materially different information. |
| CON-016 | Registration transaction boundary | Vendor creation, idempotency outcome and durable publication obligation have an explicit atomic boundary. | HJ-011 §§2.4-2.5; HJ-105; ADR-008 | One explicit PostgreSQL transaction coordinated by the Vendor Application and implemented by Infrastructure | Approved | P0 | Approved transaction-boundary decision. One PostgreSQL transaction atomically commits the Vendor Aggregate and Registered Information, the persisted idempotency identity, semantic fingerprint and original successful result, and exactly one durable outbox item for the genuine VendorRegistered occurrence. Any failure before commit leaves none of these committed. Address resolution and pre-transaction validation occur before the transaction begins. Outbox dispatch occurs after commit and outside the registration transaction. The Vendor Application owns orchestration of the boundary; EF Core and PostgreSQL mechanics remain in Infrastructure. ADR-008 amendment required. | Real-PostgreSQL failure-injection and transaction tests prove atomic commit and rollback across Vendor, registration outcome and outbox persistence. Tests prove that dispatch failure does not roll back registration and that pre-commit failure leaves no partial state. |
| CON-017 | Reliable publication staging | A committed Vendor cannot lose the obligation to publish VendorRegistered. | HJ-011 §2.5; ADR-008 | Transactional Outbox | Approved | P0 | Existing ADR: ADR-008 | HJ-107 publication outcomes; HJ-013 atomicity, failure-injection and recovery tests. |
| CON-019 | Domain Event to Integration Event translation | Internal VendorRegistered business facts remain separate from the external versioned contract, are translated before outbox persistence and are never reconstructed by the relay from current Vendor state. | HJ-004; HJ-106; ADR-003; ADR-008; CON-020 | Vendor Application-owned explicit mapper translating VendorRegistered before outbox persistence | Approved | P0 | Approved Decision Mode resolution, 22 August 2026; ADR-003 and ADR-008. An explicit Vendor Application-owned mapper translates VendorRegistered into the approved versioned Integration Event before outbox persistence. Vendor Infrastructure serializes and persists the resulting event unchanged within the registration transaction. Domain code contains no Integration Event, outbox, serialization or broker representation. Relay-time reconstruction from current Vendor state is prohibited. | Application mapper tests prove exact Domain-to-Integration translation and exclusions. Architecture tests prove that Integration Event, outbox, serialization and broker types remain outside the Vendor Domain. Persistence integration tests prove that the translated serialized event is stored unchanged and is not reconstructed from later Vendor state. |
| CON-020 | Integration Event schema and compatibility | VendorRegistered has an approved Vendor-owned versioned payload, independent Business Address representation, stable envelope, exact JSON member structure and deterministic wire-format rules sufficient to initiate Pending Activation and Compliance processing without synchronous Vendor retrieval. | HJ-004; HJ-106; CR-019; HJ-107 VR-BLOCKED-002; CON-019 | Vendor-owned transport-independent VendorRegistered Integration Event v1 contract serialized once as immutable UTF-8 camel-case JSON using contract-owned representations, explicit nulls and deterministic identifier, timestamp, time and enum formats | Approved | P0 | Approved amended Decision Mode resolution, 23 August 2026. Version 1 uses the concrete camel-case JSON envelope and nested payload representation approved through the Slice 8C.3 finding. EventId and VendorId use lowercase canonical UUID D format. OccurredAt and RegisteredAt are converted to UTC and serialized using invariant round-trip O format. Time-only values use invariant HH:mm:ss without an offset. Enum values use lower-camel-case strings matching the approved ubiquitous terms. TradingCharacteristics and OpeningHours use the approved nested object structure. The Integration Event contract owns all published representations and does not expose Vendor Domain Aggregate, Value Object or enum types. Optional members are always present and represented as explicit null when absent. The event is serialized once before outbox persistence and published unchanged. Compatible optional additions are permitted within v1; removal, renaming, type or meaning changes require a new version. Retries preserve EventId, version and the original serialized event. Consumers tolerate unknown fields. | Integration contract and serialization tests prove the exact v1 envelope, nested payload, member names and ordering-independent JSON structure; lowercase canonical UUID D identifiers; UTC invariant round-trip O timestamps; invariant HH:mm:ss time-only values; lower-camel-case enum strings; contract-owned representations; independent BusinessAddress schema; conditional authority; explicit-null optional members; and prohibited-field exclusions. Compatibility tests permit unknown and newly added optional fields while rejecting breaking v1 changes. Persistence and retry tests prove that EventId, version and serialized event remain unchanged and that downstream processing requires no synchronous Vendor lookup. |
| CON-027 | Registered Vendor retrieval | Retrieval loads the persisted aggregate as authoritative source, maps a purpose-specific response, is side-effect-free and returns controlled Not Found. | HJ-004; HJ-105; HJ-106 §4.12; HJ-011 | Query handler + Repository + response mapper | Approved | P1 | Existing ADR: ADR-001; approved HJ-004/HJ-106 contract; implementation-local handler style | HJ-107 VR-RETRIEVE tests; HJ-013 repository and mapping integration tests. |
| CON-028 | PostgreSQL mapping and constraints | Aggregate state, registered information, idempotency and outbox data are explicitly mapped with correct keys, constraints, indexes and delete behaviour. | HJ-005; HJ-011 §2.4; ADR-008 | Explicit EF Core fluent mapping + PostgreSQL keys, constraints and indexes | Approved | P0 | Approved persistence implementation decision. Vendor Infrastructure owns explicit EF Core fluent mappings for the Vendor Aggregate, Registered Information, persisted registration outcome and outbox data. Mappings define keys, lengths, nullability, conversions, enum representations, indexes and restrictive delete behaviour. Persisted normalized Trading Name and Legal Operator Name together with CanonicalAddressId form a database-enforced unique constraint. A one-to-one registration-outcome record retains the semantic fingerprint and original RegisterVendor result. Registration Declarations and the opaque Address Resolution reference are not persisted as Vendor state. No cascade deletion of registration outcomes or outbox records is permitted. CON-029 separately governs schema migrations; CON-018–CON-021 govern outbox contract, relay and delivery details. | Real-PostgreSQL tests prove complete Aggregate round-trip and rehydration, value conversion and nullability fidelity, composite uniqueness enforcement, registration-outcome persistence, restrictive delete behaviour, outbox persistence, required indexes and transaction atomicity. Migration lifecycle evidence remains under CON-029. |
| CON-040 | RegisterVendor application outcome representation | RegisterVendor returns a closed, transport-independent typed outcome that distinguishes committed success from every expected controlled failure without exposing HTTP, framework, Address implementation or persistence details. | HJ-005 §§7.4, 12.1; HJ-106 §§4.7, 4.9, 4.11; CON-011 | Closed typed `RegisterVendorResult` owned by the Vendor Application | Approved | P0 | Approved Application-boundary decision. Success carries the minimum committed Vendor identity/state. Expected failures use stable Application-owned outcome kinds. HTTP representation and status mapping remain under CON-024/CON-025. | Application unit tests for closed result construction, success/failure state validity, exact Address failure preservation, absence of transport/infrastructure types and RegisterVendor orchestration outcome mapping. |

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
| **Approved concerns** | CON-001–CON-017, CON-027, CON-028 and CON-040 |
| **Approved Approaches** | Aggregate; Value Object; Entity; Domain Event; Repository; Address application port and adapter; typed synchronous invocation; mapper/translator; permanent contextual Address Resolution reference; fail-fast semantic failures and controlled retryable technical failure without in-process retry or Epic 1 circuit breaker; Application Service; immutable transport-independent `RegisterVendorCommand`; composite Vendor uniqueness identity and semantic registration fingerprint; closed typed `RegisterVendorResult`; Transactional Outbox; Query handler + Repository + response mapper |
| **HJ-010 effect** | All twenty-two Approved concerns appear in HJ-010 v1.8 with Resolution State Approved. |
| **HJ-012 effect** | All twenty-two matching resolutions form the synchronized application architecture baseline. |
| **Downstream generation** | Assess HJ-106 impact, then independently regenerate HJ-107 and HJ-013 where their authoritative inputs materially change. |
| **Publication date** | 21 August 2026 |

## 7. Current Approved Baseline

Developers implementing the affected HotJoes application architecture shall use the twenty-two Approved entries in §4 and preserve their Required Guarantees.

Later approved resolutions are added without removing their concern from HJ-010. Challenged resolutions remain visible here but cease to be active implementation authority until re-approved.

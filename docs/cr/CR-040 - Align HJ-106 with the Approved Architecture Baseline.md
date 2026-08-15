# CR-040 - Align HJ-106 with the Approved Architecture Baseline

## 1. Change Summary

Amend **HJ-106 - Vendor Registration Service Contract** to record the impact assessment of the first approved application architecture baseline published in HJ-010 and HJ-012.

The assessment confirms that:

- CON-001, CON-002, CON-003, CON-004, CON-005, CON-017 and CON-027 require no change to HJ-106's normative business behaviour;
- HJ-106 already expresses the service-level guarantees required by those concerns;
- implementation patterns remain governed by HJ-012 and shall not be exposed unnecessarily as service-contract requirements; and
- unresolved technical matters recorded in HJ-106 shall be linked to their corresponding Current Architectural Concerns in HJ-010.

Apply a controlled traceability and terminology revision to HJ-106 without changing its operations, request or result information, business rules, outcomes, failures or proposed HTTP representation.

Publish the revised HJ-106 as **version 1.1, Approved**.

## 2. Reason for Change

HJ-106 v1.0 predates the approved HJ-010/HJ-012 architecture baseline. An impact assessment against the seven Approved concerns found that the existing service contract is already behaviourally aligned:

| Concern | Approved Approach | Existing HJ-106 Treatment |
|---|---|---|
| CON-001 | Aggregate | Commits exactly one valid Vendor aggregate and preserves its creation invariants. |
| CON-002 | Value Object | Defines business concepts and validation without prescribing their implementation types. |
| CON-003 | Entity | Defines Vendor identity, lifecycle commencement and retrieval of persisted Vendor state. |
| CON-004 | Domain Event | Records the internal completed fact only for successful creation and separates it from the Integration Event. |
| CON-005 | Repository | Uses the persisted aggregate as authoritative source and prevents infrastructure representations from becoming service results. |
| CON-017 | Transactional Outbox | Requires atomic Vendor persistence and durable publication recording, recoverability after dispatch failure and retry without repeating registration. |
| CON-027 | Query handler + Repository + response mapper | Defines side-effect-free retrieval from the persisted aggregate with a purpose-specific result and controlled Not Found outcome. |

HJ-106 therefore does not require behavioural regeneration. It does require a small alignment revision so future derivation can distinguish:

- normative service behaviour;
- approved implementation architecture;
- proposed technical representation; and
- unresolved architectural concerns that must not be silently resolved by downstream generators.

## 3. Required Changes to HJ-106

### 3.1 Document Control

Update HJ-106 as follows:

| Field | Required Value |
|---|---|
| **Version** | 1.1 |
| **Status** | Approved |
| **Last Updated** | Date CR-040 is applied |

Add a revision-history entry stating that CR-040:

- assessed HJ-106 against the first HJ-012 approved architecture baseline;
- found no change to normative business behaviour;
- added architectural traceability and explicit Current Concern dependencies; and
- aligned durable-publication terminology with the separation between service guarantees and implementation architecture.

### 3.2 Related Documents

Add the following entries to Related Documents:

| Document ID | Title | Status | Relevance |
|---|---|---|---|
| HJ-010 v1.0 | Current Application Architectural Concerns | Approved | Identifies unresolved architectural concerns and their current Resolution States; does not replace the authoritative business sources of this contract. |
| HJ-012 v1.0 | Established Application Architecture Patterns | Approved | Defines the approved implementation architecture that fulfils applicable HJ-106 guarantees without becoming part of the external service contract. |
| CR-040 | Align HJ-106 with the Approved Architecture Baseline | Applied | Records the architecture-impact assessment and authorises this traceability-only revision. |

Do not add HJ-010 or HJ-012 as authorities for new business behaviour. The existing approved business and domain artefacts remain authoritative for the normative service contract.

### 3.3 Source-Artefact Interpretation

Retain the existing Source Artefacts table and its authority assignments.

After that table, add an architectural-alignment statement equivalent to:

> HJ-010 v1.0 and HJ-012 v1.0 were assessed as downstream architectural governance for this service contract. The seven Approved concern resolutions introduce no new operation, business rule, result, failure or externally observable guarantee. HJ-012 governs implementation architecture where it fulfils this contract; HJ-010 identifies unresolved architectural dependencies that downstream derivation must not decide implicitly.

The statement shall not imply that all Current Architectural Concerns have been resolved or that the complete Epic 1 implementation can be generated without further architectural decisions.

### 3.4 Durable Publication Terminology

In §4.9 Idempotency, replace:

> create no additional outbox or publication record;

with:

> create no additional durable publication record;

This change preserves the business guarantee while avoiding two conflicting interpretations:

- HJ-106 remains implementation-neutral at the service-contract boundary; and
- HJ-012 remains authoritative that the approved implementation Approach for CON-017 is Transactional Outbox.

Retain the existing use of **durable publication work** and **durable publication record** elsewhere in HJ-106. Do not expose outbox storage structure, relay mechanism, persistence mapping, event schema, broker policy or implementation framework as part of the service contract.

### 3.5 Assumptions and Open Questions

Amend §8 so that unresolved technical matters identify their governing Current Architectural Concerns without resolving them.

The following mappings are required:

| Existing Open Matter | Current Concern Reference | Required Treatment |
|---|---|---|
| Idempotency identity format, equivalence, storage, retention and concurrent duplicate coordination | CON-013, CON-014, CON-015 and CON-016 | Record the concern references and retain the matters as unresolved. Do not select an identity, hashing, persistence, concurrency or transaction mechanism. |
| Concrete Business Address Snapshot schema and Address Resolution reference/failure treatment | CON-009 and CON-010 | Record the concern references and retain the Address contract and failure taxonomy as unresolved. Do not infer Address-owned schema or semantics. |
| Concrete VendorRegistered Integration Event schema | CON-020 | Add or amend an entry identifying the unresolved event contract, including the Business Address representation, metadata/envelope and compatibility rules. Do not infer a wire schema. |
| Routes, serialization, identifiers, timestamps, headers, null/omission rules, status mappings and controlled error representation | CON-024 and CON-025 | Record the concern references and retain Part B as proposed technical representation. Do not promote proposed conventions to normative requirements. |
| Validation allocation and concrete validation profiles where not established by an approved business source | CON-026 | Record the concern reference without weakening authoritative Domain or service validation rules. |

Where one existing row spans more than one concern, the concern references may be included in the Item or Consequence / Required Decision field. Do not add a separate concern-reference column unless required for readability across the complete table.

### 3.6 Traceability Matrix

Add the following architectural alignment rows to §7:

| Contract Element | Source Artefact | Source Section / Model Element | Interpretation |
|---|---|---|---|
| Approved Domain implementation architecture | HJ-012 | CON-001 to CON-005 | Aggregate, Value Object, Entity, Domain Event and Repository Approaches fulfil existing Domain and service guarantees without adding service behaviour. |
| Reliable publication implementation architecture | HJ-012 | CON-017 | Transactional Outbox is the approved implementation Approach for the existing atomic durable-publication guarantee; relay and broker details remain unresolved elsewhere. |
| Registered Vendor retrieval implementation architecture | HJ-012 | CON-027 | Query handler, Repository and response mapper fulfil the existing persisted-source, purpose-specific result and side-effect-free retrieval contract. |
| Unresolved architecture dependencies | HJ-010 | CON-009, CON-010, CON-013 to CON-016, CON-020, CON-024 to CON-026 and CON-028 | Downstream derivation must preserve the unresolved status of these matters and must not invent their Approaches. |

These rows provide architectural traceability. They do not make HJ-012 implementation types part of the public or business service contract.

### 3.7 Review Checklist

Add checklist confirmations equivalent to:

- `[x]` The first HJ-012 Approved architecture batch has been assessed and introduces no change to normative HJ-106 business behaviour.
- `[x]` Approved implementation patterns are not exposed unnecessarily as service-contract requirements.
- `[x]` Unresolved architectural choices remain explicit and traceable to HJ-010 concerns.

## 4. Required Preservation of Existing Contract

The following HJ-106 content shall remain normatively unchanged:

- the `RegisterVendor` and `RetrieveRegisteredVendor` operation set;
- complete-request and Registration Session boundaries;
- request and result information;
- Vendor creation invariants and lifecycle effects;
- Address ownership and collaboration behaviour;
- idempotent replay and idempotency-conflict outcomes;
- Domain Event and Integration Event business semantics;
- atomic persistence and durable-publication guarantees;
- business failures and side-effect constraints;
- Registered Vendor Details content and exclusions;
- controlled Vendor Not Found behaviour;
- information explicitly outside scope; and
- the proposed, non-normative status of the HTTP representation in Part B.

CR-040 shall not silently resolve any item currently classified as Technical Convention or Missing Information.

## 5. Explicit Non-Changes

CR-040 does not:

- add, remove or rename a service operation;
- change a precondition, business rule, invariant, outcome or failure;
- change the minimum `RegisterVendor` response;
- change Registered Vendor Details;
- require an outbox representation in a public request, response or Integration Event contract;
- select an outbox relay mechanism;
- define the Integration Event schema;
- define Address-owned contracts;
- select idempotency identity, equivalence, retention, storage, concurrency or transaction mechanics;
- approve the proposed HTTP representation;
- regenerate HJ-107;
- amend HJ-010, HJ-012 or HJ-013;
- create or amend an ADR; or
- generate tests or implementation code.

## 6. Impacted Artefacts

| Artefact | Impact |
|---|---|
| **HJ-106 - Vendor Registration Service Contract** | Traceability and terminology alignment; publish v1.1 Approved with no normative behavioural change. |
| **HJ-010 - Current Application Architectural Concerns** | No change. Referenced for unresolved architectural dependencies. |
| **HJ-012 - Established Application Architecture Patterns** | No change. Referenced for approved implementation architecture. |
| **HJ-107 - Vendor Registration Test Catalogue** | No change under this CR. Regeneration follows the aligned HJ-106 baseline. |
| **HJ-013 - Architecture and Implementation Test Catalogue** | No change under this CR. Reconcile after HJ-107 regeneration. |

## 7. Acceptance Criteria

CR-040 is satisfied when:

1. HJ-106 is published as version 1.1, Approved.
2. The revision history records a no-behaviour-change architecture-impact assessment.
3. HJ-010, HJ-012 and CR-040 appear in Related Documents with the correct authority boundaries.
4. The Source Artefacts section states that the seven Approved concerns introduce no new operation, rule, result, failure or observable guarantee.
5. §4.9 uses **durable publication record** rather than **outbox or publication record**.
6. The unresolved matters in §8 identify their applicable HJ-010 concern references.
7. §7 traces the existing HJ-106 guarantees to CON-001 to CON-005, CON-017 and CON-027 without exposing implementation types as service-contract requirements.
8. The Review Checklist confirms both architectural alignment and preservation of unresolved choices.
9. All normative service behaviour listed in Section 4 of this CR remains unchanged.
10. No unresolved Approach has been selected or inferred.

## 8. Follow-up Work

After CR-040 is applied:

1. regenerate HJ-107 from HJ-106 v1.1 and its other authoritative sources;
2. reconcile HJ-107 ownership against the initial HJ-013 draft;
3. inspect the remaining HJ-013 dependencies for genuine architectural gaps;
4. resolve the applicable Current Architectural Concerns through the established HJ-010/HJ-012 process; and
5. generate constrained implementation only where the governing architecture and test obligations are sufficiently explicit.

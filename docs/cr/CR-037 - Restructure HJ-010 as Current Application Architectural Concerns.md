# CR-037 – Restructure HJ-010 as the Current Application Architectural Concerns Register

## 1. Change Summary

Rename **HJ-010 - Application Architecture and Implementation Pattern Map** to **HJ-010 - Current Application Architectural Concerns** and establish it as the authoritative register of Current Architectural Concerns for the active implementation scope.

HJ-010 shall no longer operate as a cumulative Epic-specific pattern catalogue containing a mixture of unresolved concerns, settled patterns and historical exploration.

For the current baseline, HJ-010 shall be reconciled against **Epic 1 – Vendor Registration**, with **HJ-011 v1.1 – Epic 1 Vendor Registration Implementation Scope** as the authoritative implementation-scope source.

HJ-010 shall:

- identify the architectural concerns that must be resolved, implemented or verified for the active implementation scope;
- state the architectural guarantee or problem that each concern must address;
- record candidate approaches while a concern is under exploration and the selected approach once resolved;
- track the concern through a controlled resolution lifecycle;
- identify the authoritative decision, standard, principle, convention or implementation-local authority governing the resolution;
- identify the verification treatment for executable architectural guarantees;
- remain limited to the current implementation boundary rather than accumulating permanent Epic-specific sections;
- promote reusable, implemented and verified architecture into **HJ-012 – Established Application Architecture Patterns** when that artefact is introduced; and
- keep known, materially significant concerns outside the active implementation scope separate from HJ-010 as **Deferred Architectural Concerns**.

This Change Request restructures HJ-010 and reconciles its Epic 1 concern coverage. It does not itself create HJ-012, create the deferred-concern artefact, amend HJ-106 or HJ-107, or select unresolved implementation technologies.

## 2. Reason for Change

HJ-010 v0.1 is a strong concern-centric starting point, but its present structure mixes:

- unresolved architectural concerns;
- candidate implementation patterns;
- already accepted architectural decisions;
- established DDD patterns;
- verification concerns; and
- Epic 1-specific scope material.

If this model is extended cumulatively across future Epics, HJ-010 will become an architectural warehouse rather than a usable working register.

The architecture documentation therefore requires a controlled lifecycle in which:

```text
Deferred Architectural Concern
        ↓
Current Architectural Concern in HJ-010
        ↓
Resolved, implemented and verified reusable architecture
        ↓
Established Application Architecture Pattern in HJ-012
```

with the additional permitted transitions:

```text
Current → Deferred
Current → Superseded / Removed
Established Pattern → Current Concern when challenged by new scope
```

Version control and ADR history remain the audit trail for past exploration. HJ-010 should represent the current architectural state and active architectural work, not preserve every historical concern indefinitely.

HJ-010 v0.1 also currently ties ADR necessity too closely to whether a candidate is classified as a DDD pattern. DDD classification and architectural significance are separate questions. An architectural concern may be resolved by an existing ADR, a new ADR, an architectural principle, an engineering standard, a framework or platform convention, or an implementation-local decision.

Finally, the approved Epic 1 implementation boundary has expanded beyond the Vendor Service’s internal implementation path. HJ-011 v1.1 and the current System Model include the Web client, API Gateway, centralized configuration, security, observability, real messaging infrastructure, persistence, Address Stub and Compliance Event Consumer Stub. HJ-010 must be reconciled against that complete executable vertical slice.

## 3. Required Changes to HJ-010

### 3.1 Document Purpose

Replace the current purpose with wording that establishes HJ-010 as the controlled register of architectural concerns for the active implementation scope.

The revised purpose shall state that HJ-010:

- records **Current Architectural Concerns** relevant to the active implementation scope;
- describes the **Required Guarantee** or architectural problem that must be addressed;
- records the **Approach** under consideration and, once resolved, the selected approach;
- tracks each concern through resolution, implementation and verification;
- provides navigation to the authoritative decision or governing source;
- identifies the verification treatment for executable architectural guarantees;
- does not act as the permanent catalogue of established reusable patterns;
- does not attempt to anticipate detailed architecture for every future Epic;
- does not duplicate ADR rationale;
- does not duplicate engineering-standard content;
- does not duplicate test specifications; and
- does not define C# project, namespace, folder or class structure before the relevant architectural concerns are sufficiently resolved.

HJ-010 shall remain an architecture-navigation and working artefact, but its authority shall be limited to the current concern set for the explicit implementation baseline recorded in the document.

### 3.2 Current Implementation Baseline

Add a dedicated baseline section identifying the exact scope against which HJ-010 has been reconciled.

The section shall contain at least:

| Field | Required Value / Treatment |
| --- | --- |
| **Active Implementation Scope** | Epic 1 – Vendor Registration |
| **Authoritative Scope** | HJ-011 v1.1 |
| **System Model Baseline** | Stable System Model identifier and version where governed; otherwise the approved interim baseline reference defined in §3.3 |
| **Last Concern Reconciliation** | Date of the most recent complete concern-reconciliation review |
| **Applicable Sources** | Approved domain, behavioural, service, architecture and engineering artefacts relevant to the current scope |

HJ-010 shall claim concern completeness only for this named baseline.

The document shall not imply completeness for future HotJoes capabilities or future Epics.

### 3.3 System Model Identification and Versioning

The governed System Model baseline is:

| Field | Value |
| --- | --- |
| **Artefact ID** | HJ-SM-001 |
| **Title** | System Model |
| **Version** | 1.0 |
| **Status** | Approved |
| **Last Updated** | 13 August 2026 |

The authoritative artefact name is:

> **HJ-SM-001 - System Model**

The artefact identifier and title are stable. Version, status and date are separate metadata and shall not be incorporated into the title.

HJ-010 shall reference the approved baseline as:

> **HJ-SM-001 v1.0**

The System Model image should carry an unobtrusive identifying footer when exported independently:

```text
HJ-SM-001 | Version 1.0 | Approved | 13 August 2026
```

The authoritative System Model metadata should reside in, or ultimately be accompanied by, a governed source artefact or wrapper containing:

- metadata;
- revision history;
- the diagram;
- scope and legend; and
- source/change references.

CR-037 does not itself require creation of a separate System Model wrapper artefact unless separately approved. The absence of that wrapper does not alter the governed identity or baseline reference established above.

### 3.4 Related Documents and Sources

Add **HJ-011 v1.1 – Epic 1 Vendor Registration Implementation Scope** to the HJ-010 related-document set and identify it as the authoritative current implementation boundary.

Retain the relevant approved sources already used by HJ-010, including as applicable:

- HJ-001 – Project Vision;
- HJ-002 – Architectural Principles;
- HJ-003 – Ubiquitous Language Guide;
- HJ-004 – Vendor Domain Models;
- HJ-005 – Coding Standards;
- HJ-006 – Testing Strategy and Standards;
- HJ-007 – Enforcement Strategy;
- HJ-104 – Vendor Registration Information Contract;
- HJ-105 – Vendor Registration Sequence Diagram;
- HJ-106 – Vendor Registration Service Contract;
- HJ-107 – Vendor Registration Test Catalogue; and
- accepted ADRs relevant to the current concern set.

Where HJ-012 or the deferred-concern artefact is introduced later, add those artefacts as related documents in the normal controlled manner.

### 3.5 Replace the Existing Concern Catalogue

Rename the existing catalogue heading:

> **Epic 1 Architectural Concern and Pattern Catalogue**

to:

> **Current Architectural Concerns**

The working table shall contain only concerns genuinely required to implement or verify the active implementation baseline.

Replace the current catalogue schema with the following compact structure:

| Column | Purpose |
| --- | --- |
| **ID** | Stable concern identifier |
| **Architectural Concern** | The architectural issue requiring treatment |
| **Required Guarantee** | What the architecture must preserve or achieve |
| **Scope / Source** | Requirement, model element, boundary or artefact creating the concern |
| **Approach** | Candidate patterns, standards or conventions while exploring; the single selected approach once resolved |
| **Resolution State** | Controlled lifecycle state |
| **Priority** | Resolution priority |
| **Decision Treatment / Source** | Authority for the selected resolution, or unresolved treatment |
| **Verification Treatment** | Catalogue, test suite, enforcement mechanism, validation or review proving the guarantee |

The table shall not contain extensive pattern comparison or rationale.

Where an unresolved concern requires architectural exploration, candidate comparison may be recorded in a concise concern-specific subsection or, where the decision is architecturally significant, in the applicable ADR.

### 3.6 Define the Approach Column

Define **Approach** as follows:

> The Approach column records candidate patterns, standards, conventions, policies or implementation mechanisms while a concern is under exploration, and the single selected approach once the concern is resolved.

The column is intentionally broader than “Pattern” because an architectural concern may be resolved through:

- a DDD pattern;
- an application or integration pattern;
- an architectural principle;
- an engineering standard;
- a framework or platform convention;
- a policy;
- an implementation-local mechanism; or
- an explicit decision not to introduce an additional mechanism.

During exploration, multiple candidate approaches may be listed concisely.

After selection, the column shall contain only the selected approach.

Example progression:

| Approach | Resolution State | Decision Treatment / Source |
| --- | --- | --- |
| Transactional Outbox; CDC; equivalent durable relay | Exploring | ADR assessment required |
| Transactional Outbox | Selected | ADR-008 |
| Transactional Outbox | Implemented | ADR-008 |
| Transactional Outbox | Verified | ADR-008; applicable architecture verification IDs |

### 3.7 Define Controlled Resolution States

Replace the current informal `TBD` / DDD-classification-driven lifecycle with the following controlled **Resolution State** values:

- **Identified**
- **Exploring**
- **Selected**
- **Blocked**
- **Implemented**
- **Verified**

These values describe the lifecycle state of the concern and are independent of the Approach itself.

The following rules shall apply:

- **Identified** means the concern is known but substantive exploration has not started.
- **Exploring** means one or more candidate approaches are being evaluated.
- **Selected** means an approach has been chosen with the required decision authority, but implementation or verification remains incomplete.
- **Blocked** means further resolution, implementation or verification is prevented by an explicit dependency.
- **Implemented** means the selected resolution has been implemented but has not yet met the required verification threshold.
- **Verified** means the required architectural guarantee has been demonstrated through the identified verification treatment.

“Decision required” shall not be a Resolution State. Decision need and authority belong in **Decision Treatment / Source**.

A selected approach does not become an Established Application Architecture Pattern merely because a decision or ADR exists.

A concern remains current until the required implementation and verification threshold is satisfied and the resulting architecture is suitable for promotion to HJ-012, or until the concern is explicitly deferred, superseded or removed.

### 3.8 Define Concern Lifecycle and Epic Reconciliation

Add an explicit lifecycle and reconciliation rule.

The normal progression shall be:

```text
Deferred Architectural Concern
        ↓
Current Architectural Concern in HJ-010
        ↓
Selected
        ↓
Implemented
        ↓
Verified
        ↓
Established Application Architecture Pattern in HJ-012
```

The lifecycle shall also permit:

```text
Current → Deferred
Current → Superseded / Removed
Established Pattern → Current Concern
```

An established pattern shall return to the current-concern set where a new Epic, changed non-functional requirement, new deployment boundary or other material architectural force challenges the established approach.

At the beginning of each Epic or material implementation-scope change, HJ-010 shall be reconciled by:

1. checking the active scope against the current HJ-010 concern set;
2. reassessing relevant established patterns against new architectural forces;
3. promoting applicable Deferred Architectural Concerns into HJ-010;
4. adding genuinely new concerns introduced or exposed by the new scope;
5. retaining current concerns that remain unresolved, unimplemented or unverified;
6. promoting qualified reusable, implemented and verified architecture to HJ-012;
7. deferring concerns that are no longer required by the active scope but remain materially significant;
8. superseding or removing obsolete concerns; and
9. updating the implementation-baseline metadata and reconciliation date.

HJ-010 shall not accumulate separate permanent sections for Epic 1, Epic 2, Epic 3 or later Epics.

Version control and ADRs shall provide historical traceability for removed or superseded material.

### 3.9 Separate Concern Identification from ADR Necessity

Remove the current rule that effectively determines ADR necessity from whether a candidate is or is not a DDD pattern.

Replace it with a decision-treatment model in which each concern identifies one of the following as applicable:

- **Existing ADR**
- **New ADR required**
- **Architectural Principle**
- **Engineering Standard**
- **Established Framework / Platform Convention**
- **Implementation-local Decision**
- **Unresolved**

A new ADR is required only where the selected decision is architecturally significant and warrants durable decision/rationale capture.

HJ-010 shall make the applicable ADR reference explicit where an ADR exists, but shall not create ADRs solely to complete the concern table.

DDD classification may be retained as secondary navigation information only if it continues to provide useful architectural value. It shall not determine ADR necessity. It may be removed if the revised concern and decision-treatment model makes it redundant.

### 3.10 Move Established Reusable Patterns out of HJ-010

Review every existing HJ-010 v0.1 row and classify it for one of the following treatments:

- retain as a Current Architectural Concern;
- promote or designate for promotion to HJ-012;
- defer;
- supersede; or
- remove.

Established reusable architecture shall not remain indefinitely in HJ-010 merely because it originated as an Epic 1 concern.

Potential HJ-012 candidates include, subject to implementation and verification evidence:

- Aggregate;
- Entity;
- Value Object;
- Repository;
- other reusable application-architecture patterns proven through implementation.

Accepted architectural decisions shall not automatically be promoted merely because an ADR exists.

For example:

- the mandatory idempotency and reliable-publication guarantees shall reference **ADR-008** where applicable;
- unresolved idempotency concurrency, persistence and replay mechanics shall remain current;
- unresolved outbox relay or broker-delivery mechanics shall remain current;
- a selected but unimplemented or unverified pattern shall remain in HJ-010.

HJ-010 shall not duplicate detailed ADR rationale in order to retain historical context. ADRs and version control already provide that audit trail.

### 3.11 Reconcile the Complete Epic 1 Concern Set

Perform a complete concern-reconciliation review against:

- HJ-011 v1.1;
- the approved System Model baseline;
- HJ-003;
- HJ-004;
- HJ-104;
- HJ-105;
- HJ-106;
- HJ-107;
- applicable accepted ADRs; and
- applicable engineering and enforcement standards.

The review shall retain, amend, split, merge or remove existing HJ-010 concerns as necessary and shall assess the need for Current Architectural Concerns covering at least the following areas:

#### Technical API contract and serialization

Assess separate concerns where required for:

- OpenAPI/schema ownership;
- transport-contract representation;
- serialization conventions;
- null versus omission behaviour;
- enumeration representation;
- date/time representation;
- error-envelope conventions;
- correlation/header conventions; and
- selected HTTP mappings where not already fully governed upstream.

Do not collapse independent contract concerns into one oversized row merely for table compactness.

#### Address Stub contract and failure taxonomy

Assess concerns for:

- the Address application-facing port/contract;
- Address Resolution reference format and lifecycle assumptions;
- successful authoritative resolution;
- failure taxonomy;
- timeout and retry behaviour; and
- protection against stub-specific coupling.

The Epic 1 Address Stub shall satisfy the same Vendor-facing application contract intended for the eventual production Address capability. Stub-specific semantics shall not leak into Vendor application or domain logic.

#### Idempotency concurrency and persistence mechanics

Assess unresolved implementation concerns for:

- concurrent duplicate coordination;
- request equivalence/fingerprinting;
- same identity + same payload behaviour;
- same identity + different payload behaviour;
- persistence of idempotency outcomes;
- retention;
- transactional relationship to Vendor creation;
- replay of the original outcome; and
- database constraints.

Do not reopen guarantees already established by ADR-008. HJ-010 shall distinguish already decided architectural guarantees from unresolved implementation mechanics.

#### Integration-event contract

Assess concerns for:

- Integration Event schema;
- event metadata/envelope;
- message identity;
- compatibility/versioning;
- Business Address representation; and
- separation between the Domain Event and Integration Event contracts.

#### Event broker, relay and delivery behaviour

Assess concerns for:

- relay ownership;
- durable publication;
- retry/backoff;
- duplicate delivery;
- ordering assumptions;
- poison-message handling;
- recovery;
- broker delivery; and
- observable consumer receipt.

#### Centralized configuration

Assess concerns for:

- configuration-provider/access pattern;
- bootstrap behaviour;
- validation;
- refresh behaviour where applicable;
- failure behaviour;
- environment separation;
- consumer responsibility; and
- separation of centralized configuration from secret management.

Feature-management behaviour remains out of scope for Epic 1 and shall not be introduced through HJ-010.

#### Security and registration-data protection

Assess concerns for:

- HTTPS termination and trust boundaries;
- secure handling of secrets and credentials;
- sensitive registration-data logging and redaction;
- protection of registration information;
- secure defaults; and
- any other security obligations explicitly required by the approved Epic 1 scope.

#### Observability, correlation and health

Assess concerns for:

- correlation propagation;
- structured logging;
- traces and metrics where required;
- outbox/publication diagnostics;
- observable Compliance Stub receipt;
- health checks; and
- readiness checks.

#### Vendor Web client and Registration Session

Assess concerns for:

- the Web-client-owned Registration Session;
- client state model;
- session disposal;
- validation allocation;
- submission and retry behaviour;
- registration confirmation; and
- retrieval/display flow.

HJ-010 shall not retain the previous client-or-BFF ambiguity for Epic 1. The Web client owns the Registration Session and BFF implementation is out of scope.

#### API Gateway

Assess concerns for:

- routing ownership;
- API/version routing;
- boundary responsibilities;
- validation allocation where applicable;
- error propagation; and
- correlation forwarding.

#### Registered Vendor retrieval

Assess concerns for:

- query handling;
- repository loading;
- purpose-specific response mapping;
- side-effect-free retrieval; and
- Vendor Not Found behaviour.

#### PostgreSQL schema and migration lifecycle

Assess concerns for:

- aggregate mapping;
- constraints and indexes;
- schema migration;
- deployment ordering; and
- migration verification.

#### Compliance Event Consumer Stub

Assess concerns for:

- subscription/topology;
- Integration Event deserialization;
- contract validation;
- receipt recording;
- duplicate handling; and
- strict prevention of real Compliance business behaviour.

#### Runtime and deployment composition

Assess concerns for:

- process/deployable boundaries;
- startup ordering;
- readiness dependencies;
- local/integration environment composition; and
- the executable completion boundary.

#### Architecture enforcement and delivery controls

Assess concerns for:

- architecture fitness tests;
- dependency enforcement;
- static analysis;
- build/CI quality gates;
- dependency/version governance where applicable; and
- other delivery controls required by HJ-005, HJ-006 or HJ-007.

Only concerns genuinely required by the current Epic 1 implementation boundary shall enter HJ-010.

This list is a completeness-review prompt, not an instruction to manufacture a concern for every bullet where an existing concern or upstream rule already provides sufficient treatment.

### 3.12 Correct Current Scope and Reference Drift

Update HJ-010 so that it reflects the approved current implementation boundary.

The revised HJ-010 shall state or reflect that:

- **HJ-011 v1.1** is the authoritative Epic 1 implementation boundary;
- the Centralized Configuration Service is in scope as an Epic 1 supporting runtime/delivery capability;
- feature-management behaviour is out of scope for Epic 1;
- Epic 1 uses a **Web-client-owned Registration Session**;
- BFF implementation is out of scope;
- the API Gateway is in scope;
- security is in scope to the degree required by HJ-011;
- sufficient observability is in scope;
- a real Event Bus / Message Broker is part of the Epic 1 implementation boundary;
- the Compliance Event Consumer Stub is part of the Epic 1 completion boundary;
- Registered Vendor retrieval is part of the Epic 1 executable slice;
- HJ-010 remains **Draft** until the restructured document has completed architectural review.

Remove or replace the current simplified Epic 1 concern-extraction diagram where it misrepresents the approved System Model boundary.

Any retained simplified diagram shall be explicitly derived from and consistent with the current approved System Model baseline and shall not reintroduce client/BFF ambiguity or omit current Epic 1 supporting/runtime components where those omissions would misstate the concern-extraction boundary.

Correct any related-document or scope statement that incorrectly describes HJ-010 as Approved while the document remains Draft.

### 3.13 Verification Traceability

Add the following governing rule:

> **Every Current Architectural Concern shall identify its authoritative source, resolution provenance and verification treatment. Every executable architectural guarantee shall identify at least one verification destination.**

Verification destinations may include:

- HJ-107 behavioural tests;
- the planned Application Architecture Verification Catalogue;
- architecture fitness tests;
- infrastructure or integration test suites;
- static analysis;
- build enforcement;
- deployment/configuration validation;
- runtime health or readiness checks;
- code review; or
- architecture review.

HJ-010 shall reference verification destinations and identifiers where available but shall not copy the underlying test cases into the concern table.

Not every architectural concern requires an executable test. Where review or documentary enforcement is the appropriate verification treatment, that shall be stated explicitly.

### 3.14 Preserve HJ-107’s Derivation Boundary

HJ-010 shall explicitly preserve the role of **HJ-107 – Vendor Registration Test Catalogue**.

The revised wording shall state that:

- HJ-107 remains the Vendor Registration behavioural test catalogue derived from HJ-106 and approved business/domain sources;
- HJ-010 may reference relevant HJ-107 obligations where an architectural concern has externally observable Vendor Registration behaviour;
- architecture, integration, infrastructure and runtime obligations that are not derivable from HJ-106 shall be placed in the separate planned **Application Architecture Verification Catalogue** or another explicitly governed verification destination;
- overlapping guarantees may be verified at different architectural layers without duplicating the same test obligation; and
- HJ-010 does not become an alternative behavioural test specification.

Replace the current linear implication:

```text
HJ-010
    -> HJ-107 behavioural tests + pattern-implied tests
```

with a model equivalent to:

```text
HJ-011 scope
    ↓
HJ-010 Current Architectural Concerns
    ↓
ADRs / Architectural Principles / Engineering Standards /
Platform Conventions / Implementation-local Decisions
    ↓
Implementation
    ↓
Verification Destinations
    ├─ HJ-107 behavioural tests
    ├─ Application Architecture Verification Catalogue
    ├─ architecture / integration / infrastructure tests
    ├─ static analysis / build enforcement
    └─ deployment, runtime or review evidence
```

HJ-107 shall continue to be generated or maintained from HJ-106 and its approved behavioural/domain sources independently of HJ-010’s architecture-verification obligations.

### 3.15 Concern Identifiers and Terminology

Replace the current `AC-xxx` concern identifier scheme to avoid ambiguity with **Acceptance Criteria**.

Use:

> **CON-001, CON-002, ...**

for Current Architectural Concern identifiers.

Existing concern rows shall be mapped to new `CON-xxx` identifiers during the v0.2 reconciliation. Where useful for review, the migration may temporarily record the former `AC-xxx` identifier, but the old identifier shall not remain the primary identifier in the revised document.

Use the following terms consistently:

- **HJ-010 – Current Application Architectural Concerns** for the artefact;
- **Current Architectural Concern**
- **Established Application Architecture Pattern**
- **Deferred Architectural Concern**

Do not use **Established Concern**. It is the architectural resolution or reusable pattern that becomes established, not the concern itself.

### 3.16 Deferred Architectural Concerns

HJ-010 shall define the relationship to Deferred Architectural Concerns but shall not become their permanent register.

A Deferred Architectural Concern is a concern that is:

- already known;
- materially significant;
- explicitly outside the active implementation scope; and
- worth preserving for later reconciliation.

The deferred register shall not be populated speculatively from every future capability visible in the System Model.

No candidate technology or implementation pattern shall be selected merely because a concern has been deferred.

Known examples that may qualify, subject to confirmation in the separate deferred-concern artefact, include:

- authentication and caller-to-Vendor association;
- real Compliance processing;
- production Address integration;
- production-scale event topology;
- service discovery; and
- feature management.

Do not add speculative future concerns for payments, driver tracking, search, menus or other capabilities unless an approved roadmap, scope artefact or architectural decision has identified a material constraint worth preserving.

The identifier and structure of the Deferred Architectural Concerns artefact shall be established separately.

### 3.17 Relationship to HJ-012

HJ-010 shall define **HJ-012 – Established Application Architecture Patterns** as the intended destination for reusable architecture that has completed the required current-concern lifecycle.

Promotion to HJ-012 requires that the resolution is:

- selected with appropriate decision authority;
- implemented;
- verified to the required architectural threshold; and
- sufficiently reusable or enduring to form part of the product-level application architecture baseline.

An ADR alone does not satisfy the promotion threshold.

Epic-specific implementation details that are verified but are not reusable architecture shall not be promoted merely to empty the HJ-010 table.

Creation, detailed structure and initial population of HJ-012 are outside CR-037 unless separately approved.

### 3.18 Reconciliation Record

Add a concise reconciliation/revision record that identifies, for each material scope reconciliation:

- the active implementation scope;
- the authoritative scope version;
- the System Model baseline;
- deferred concerns promoted into current scope;
- newly discovered concerns added;
- established patterns challenged or retained;
- current concerns promoted out of HJ-010;
- concerns deferred, superseded or removed; and
- the reconciliation date.

This shall remain concise.

No separate historical Epic tables shall be retained.

## 4. Initial Classification of Existing HJ-010 v0.1 Material

As part of applying CR-037, each existing HJ-010 concern shall be reviewed rather than mechanically copied into the new table.

At minimum:

- **Vendor consistency boundary / Aggregate** – assess for HJ-012 candidacy, subject to implementation and verification evidence.
- **Business concepts without independent identity / Value Object** – assess for HJ-012 candidacy.
- **Vendor identity and lifecycle-bearing state / Entity** – assess for HJ-012 candidacy.
- **Domain-significant registration occurrence / Domain Event** – assess whether the established Domain Event pattern is sufficiently implemented and verified for HJ-012 or whether any current translation concern remains in HJ-010.
- **Aggregate persistence abstraction / Repository** – assess for HJ-012 candidacy while retaining any unresolved concrete persistence concerns in HJ-010.
- **Vendor → Address ownership and dependency direction** – retain or split where unresolved implementation architecture remains.
- **Vendor → Address capability invocation** – retain where unresolved.
- **Vendor → Address contract translation** – retain where unresolved.
- **Vendor → Address failure handling** – retain and reconcile with the Epic 1 Address Stub and unresolved Address contract dependencies.
- **Registration use-case orchestration** – retain where unresolved.
- **Registration command representation** – retain only if the current scope still requires an unresolved architectural decision.
- **Registration idempotency boundary** – amend to reference ADR-008 for mandatory guarantees while retaining unresolved mechanics.
- **Same identity with different payload** – reference ADR-008 and retain only to the extent implementation/verification remains current.
- **Transactional consistency of Vendor persistence and Integration Event staging** – amend to reflect ADR-008’s reliable-publication requirement; retain unresolved mechanism details.
- **Integration-event publication** – retain unresolved relay/broker mechanics and reference applicable accepted ADRs.
- **Domain Event to Integration Event translation** – reference accepted event-separation decisions and retain only unresolved implementation concerns.
- **Registration transaction boundary** – retain where unresolved.
- **HTTP transport adaptation** – extend/reconcile across both RegisterVendor and Retrieve Registered Vendor endpoints.
- **Input validation allocation** – retain only unresolved architectural allocation questions.
- **Business failure representation** – reconcile with approved service behaviour and remaining technical error-contract concerns.
- **Dependency composition** – determine whether this requires current architectural treatment, an engineering standard or implementation-local resolution.
- **Architecture dependency enforcement** – retain where enforcement remains required for Epic 1.
- **Address collaboration verification** – convert into Verification Treatment of the relevant Address concerns where possible rather than preserving verification-only concerns unnecessarily.
- **Persistence mechanism verification** – convert into Verification Treatment of the relevant persistence concerns where possible.
- **Reliable publication mechanism verification** – convert into Verification Treatment of the relevant publication concerns where possible.

Verification-only rows should not automatically remain as separate architectural concerns where they are more naturally represented as the Verification Treatment of the underlying concern.

## 5. Decision and ADR Treatment

HJ-010 shall no longer use the reasoning rule:

```text
DDD pattern?
    Yes -> no ADR
    No  -> ADR
```

The revised reasoning model shall be equivalent to:

```text
Current Architectural Concern
        ↓
Required Guarantee / Architectural Forces
        ↓
Candidate Approach(es)
        ↓
Selected Approach
        ↓
What authority is appropriate?
        ├─ Existing ADR
        ├─ New ADR required
        ├─ Architectural Principle
        ├─ Engineering Standard
        ├─ Established Framework / Platform Convention
        └─ Implementation-local Decision
        ↓
Implementation
        ↓
Verification
        ↓
Promotion to HJ-012 where reusable and established
```

The need for a new ADR shall be determined by architectural significance, not by whether the selected approach is a DDD pattern.

HJ-010 shall remain a navigation point to ADRs where they exist and shall make those references explicit.

## 6. Verification Architecture

HJ-010 shall distinguish **behavioural-contract verification** from **implementation-architecture verification**.

### 6.1 HJ-107

HJ-107 remains responsible for Vendor Registration behavioural tests derived from HJ-106 and the approved Vendor Registration business/domain sources.

HJ-107 shall not be expanded merely to provide a test destination for every HJ-010 concern.

### 6.2 Application Architecture Verification

Architectural guarantees introduced by HJ-010, HJ-012, ADRs, engineering standards or infrastructure choices may require a separate **Application Architecture Verification Catalogue**.

That catalogue may cover, for example:

- architecture dependency rules;
- configuration bootstrap and failure behaviour;
- persistence/migration verification;
- outbox recovery;
- broker delivery/retry;
- Address adapter contract verification;
- runtime composition;
- health/readiness;
- security enforcement;
- observability;
- deployment validation; and
- other architecture/infrastructure obligations.

The artefact identifier, detailed structure and derivation rules for this catalogue shall be defined separately.

CR-037 shall not silently create or populate it.

### 6.3 Traceability Rule

The intended traceability model is:

```text
Authoritative scope / requirement
        ↓
HJ-010 Current Architectural Concern
        ↓
Resolution provenance
(ADR / principle / standard / convention / local decision)
        ↓
Implementation
        ↓
One or more verification destinations
        ↓
Catalogued test / enforcement / review evidence
```

Where an architectural guarantee is externally observable Vendor Registration behaviour, the verification destination may include HJ-107.

Where the guarantee is implementation-architecture-specific, it shall use the appropriate architecture/integration/runtime verification destination.

## 7. Explicit Non-Changes

CR-037 does not:

- change Vendor Registration business behaviour;
- change the Vendor Domain model;
- change Address Domain ownership;
- change Compliance Domain ownership;
- change HJ-011’s approved Epic 1 scope;
- introduce feature-management behaviour into Epic 1;
- reintroduce BFF ownership of the Epic 1 Registration Session;
- select concrete implementation technologies for unresolved concerns;
- require an ADR for every architectural concern;
- make DDD classification determine ADR necessity;
- turn HJ-010 into a test catalogue;
- turn HJ-107 into an architecture/infrastructure test catalogue;
- amend HJ-106;
- amend HJ-107;
- create HJ-012;
- create the Deferred Architectural Concerns artefact;
- create the Application Architecture Verification Catalogue; or
- formally create the System Model wrapper/governance artefact unless that work is separately approved.

Potential impacts on HJ-106, HJ-107, HJ-012, the deferred-concern register, the planned architecture-verification catalogue and System Model governance shall be recorded and handled through subsequent controlled work where required.

## 8. Impacted Artefacts

| Artefact | Required Change / Impact |
| --- | --- |
| **HJ-010 – Current Application Architectural Concerns** | Primary change. Rename and restructure the former Application Architecture and Implementation Pattern Map as the Current Application Architectural Concerns register; reconcile the complete Epic 1 concern set; introduce baseline, Approach, Resolution State, decision provenance, verification treatment and concern lifecycle. |
| **HJ-011 v1.1 – Epic 1 Vendor Registration Implementation Scope** | No content change required by CR-037. Becomes the explicit authoritative active-scope baseline referenced by HJ-010. |
| **HJ-SM-001 - System Model** | No architectural-content change required by CR-037. HJ-010 must reference the approved v1.0 baseline. A separate governed source/wrapper artefact may be introduced later without changing the established identity. |
| **HJ-012 – Established Application Architecture Patterns** | Planned downstream destination for qualified reusable patterns. Creation/population is outside CR-037 unless separately approved. |
| **Deferred Architectural Concerns artefact** | Planned separate register for known, material, explicitly deferred concerns. Identifier and structure to be defined separately. |
| **HJ-106 – Vendor Registration Service Contract** | No silent change. Record any impact discovered during HJ-010 reconciliation for separate assessment. |
| **HJ-107 – Vendor Registration Test Catalogue** | No silent change. Preserve its behavioural derivation boundary; record any legitimate behavioural-test impact for separate assessment. |
| **Application Architecture Verification Catalogue** | Planned verification destination for architecture/integration/infrastructure/runtime obligations not belonging in HJ-107. Identifier and structure to be defined separately. |
| **Applicable ADRs / Engineering Standards** | Existing sources shall be referenced where they already govern concerns. New ADRs are created only for architecturally significant unresolved decisions. |

## 9. Document Control

When CR-037 is applied to HJ-010:

- increment HJ-010 from **v0.1** to **v0.2**;
- change the document title to **HJ-010 – Current Application Architectural Concerns**;
- retain **Status: Draft** until the revised HJ-010 completes architectural review;
- update **Last Updated** to the date CR-037 is applied;
- add a Revision History entry identifying CR-037 and the restructuring of HJ-010 as the Current Application Architectural Concerns register;
- add HJ-011 v1.1 to Related Documents as the authoritative active implementation scope;
- add the specific System Model baseline reference;
- update related-document references where required by the revised concern lifecycle; and
- do not mark HJ-012, the deferred-concern register or the Application Architecture Verification Catalogue as existing approved artefacts until they are formally created.

The former title **Application Architecture and Implementation Pattern Map** shall be retained only in revision history and change traceability where useful. It shall not remain the operative HJ-010 document title after CR-037 is applied.

## 10. Acceptance Criteria

CR-037 is complete when:

1. HJ-010’s purpose is explicitly limited to architectural concerns for the active implementation scope.
2. HJ-010 identifies **Epic 1 – Vendor Registration** as the active implementation scope and **HJ-011 v1.1** as its authoritative scope source.
3. HJ-010 identifies **HJ-SM-001 v1.0** as its specific approved System Model baseline.
4. The authoritative artefact name is **HJ-SM-001 - System Model**, and its version, status and date are treated as metadata rather than being incorporated into the title.
5. HJ-010 is explicitly renamed **HJ-010 – Current Application Architectural Concerns**, with its former title retained only in revision history or change traceability where useful.
6. The existing **Epic 1 Architectural Concern and Pattern Catalogue** is replaced by **Current Architectural Concerns**.
7. The revised concern table uses the controlled columns: **ID, Architectural Concern, Required Guarantee, Scope / Source, Approach, Resolution State, Priority, Decision Treatment / Source, Verification Treatment**.
8. The **Approach** column supports multiple candidate approaches during exploration and a single selected approach after resolution.
9. The controlled Resolution States are **Identified, Exploring, Selected, Blocked, Implemented, Verified**.
10. “Decision required” is not used as a lifecycle state and decision authority is represented through **Decision Treatment / Source**.
11. The concern lifecycle and Epic/scope reconciliation process are documented.
12. HJ-010 does not accumulate permanent Epic-specific concern tables.
13. ADR necessity is separated from DDD classification.
14. Concern decision provenance supports **Existing ADR, New ADR required, Architectural Principle, Engineering Standard, Established Framework / Platform Convention, Implementation-local Decision, Unresolved**.
15. A new ADR is required only for architecturally significant decisions rather than for every non-DDD choice.
16. Current `AC-xxx` concern identifiers are migrated to `CON-xxx` identifiers to avoid ambiguity with Acceptance Criteria.
17. Existing HJ-010 rows are individually classified for retention, promotion/designation for HJ-012, deferral, supersession or removal.
18. Verification-only rows are folded into the Verification Treatment of the underlying architectural concern where that produces a cleaner concern model.
19. The complete HJ-011 v1.1 and current System Model implementation boundary has been assessed for architectural concerns.
20. The Epic 1 reconciliation explicitly assesses technical API/serialization, Address Stub, idempotency mechanics, integration-event contract, broker/relay delivery, centralized configuration, security, observability, Web client/Registration Session, API Gateway, retrieval, PostgreSQL lifecycle, Compliance Stub, runtime composition and architecture enforcement.
21. Only concerns genuinely required by the active Epic 1 scope are retained as Current Architectural Concerns.
22. HJ-010 reflects the Web-client-owned Registration Session and does not retain client-or-BFF ambiguity for Epic 1.
23. HJ-010 reflects Centralized Configuration Service, API Gateway, security, sufficient observability, Event Bus / Message Broker, Compliance Event Consumer Stub and Registered Vendor retrieval as part of the current Epic 1 concern-extraction boundary where applicable.
24. Feature-management behaviour remains out of scope for Epic 1.
25. Every retained Current Architectural Concern has a source, Required Guarantee, Approach treatment, Resolution State, Priority, decision provenance and Verification Treatment.
26. Every executable architectural guarantee identifies at least one verification destination.
27. HJ-107 remains confined to Vendor Registration behavioural tests derived from HJ-106 and approved business/domain sources.
28. HJ-010 may reference HJ-107 but does not make HJ-107 responsible for all architecture, integration, infrastructure or runtime verification.
29. Architecture/integration/infrastructure/runtime verification not derivable from HJ-106 is directed to the planned Application Architecture Verification Catalogue or another explicitly governed verification mechanism.
30. Qualified reusable architecture is designated for promotion to HJ-012 only after appropriate selection, implementation and verification; an ADR alone does not qualify a concern for promotion.
31. Deferred concerns are defined as known, materially significant and explicitly outside the current scope, and HJ-010 does not attempt to populate a speculative future architecture inventory.
32. HJ-106 and HJ-107 are not silently amended by CR-037; any discovered impact is recorded for subsequent controlled impact assessment.
33. HJ-010 is versioned to **v0.2** and remains **Draft** pending review.
34. No unrelated domain, business or Epic 1 scope decision is changed.

## 11. Follow-up Work

The following work may be required after CR-037 but is deliberately not performed by this Change Request:

1. create and approve **HJ-012 – Established Application Architecture Patterns**;
2. create and approve a separate **Deferred Architectural Concerns** artefact;
3. define and create the **Application Architecture Verification Catalogue**;
4. create a governed source/wrapper artefact for **HJ-SM-001 - System Model** if required, without changing its established identity or approved v1.0 baseline;
5. assess any HJ-106 or HJ-107 impacts discovered during the HJ-010 v0.2 reconciliation;
6. create new ADRs only for architecturally significant unresolved choices identified by the revised concern register; and
7. begin pattern-selection work for the P0/current blocking concerns only after the v0.2 completeness and traceability reconciliation is accepted.

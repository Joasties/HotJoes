# HotJoes AI Prompt
## PR-004 - Generate Test Catalogue from Approved Service Contract

### Objective

Act as a **Senior Test Architect and AI Test Writer** with extensive experience in:

- Domain-Driven Design (DDD)
- Business Contract Testing
- Test Analysis and Test Design
- Event-Driven Architecture
- Distributed Systems Reliability
- API and Integration Testing
- Enterprise Solution Architecture
- Regulated Marketplace Platforms

Use the attached HotJoes architectural artefacts as the **authoritative baseline** to derive the **Vendor Registration Test Catalogue**.

The purpose of this exercise is to translate the approved Vendor Service Contract into a complete, traceable catalogue of tests for the approved Epic 1 business operations.

---

# Attached Artefacts

The attached artefacts may include:

- HJ-002 – Architectural Principles
- HJ-003 – Ubiquitous Language Guide
- HJ-004 – Vendor Domain Models
- HJ-005 – Coding Standards
- HJ-006 – Testing Strategy and Standards
- HJ-007 – Enforcement Strategy
- HJ-010 – Current Application Architectural Concerns
- HJ-012 – Established Application Architecture Patterns
- HJ-104 – Vendor Registration Fields Matrix
- HJ-105 – Vendor Registration Sequence Diagram
- HJ-106 – Vendor Registration Service Contract
- the existing HJ-107 – Vendor Registration Test Catalogue when regenerating it
- ADR-002 through ADR-008

HJ-013 is a downstream complementary catalogue. It is not required or used as an HJ-107 behavioural-derivation, regeneration or stable-ID input.

Use Part A – Business Service Contract of the supplied current Approved HJ-106 baseline as the primary normative behavioural test basis. Record the exact HJ-106 version used in the generated catalogue.

Use upstream approved artefacts to:

- confirm source authority;
- understand the origin of requirements;
- resolve traceability references already identified by HJ-106; and
- verify that the Test Catalogue does not weaken or reinterpret the approved contract.

Where artefacts disagree, identify the conflict instead of attempting to resolve it.

Change Requests are not normative inputs unless explicitly identified as approved source material.

Use each artefact only within its authority:

| Artefact | Authority in this prompt |
|---|---|
| **HJ-106 Part A** | Primary normative basis for HJ-107 behavioural test derivation. |
| **Authoritative business/domain sources cited by HJ-106** | Confirm terminology, business rules, invariants, ownership and traceability already represented in HJ-106; they shall not silently expand the service contract. |
| **HJ-106 Part B** | Non-normative proposed technical representation; may produce Proposed tests only. |
| **HJ-010** | Identifies unresolved architectural concerns and dependencies that generation must preserve rather than resolve. |
| **HJ-012** | Identifies approved implementation architecture; it may constrain interpretation but shall not introduce service behaviour absent from HJ-106 Part A. |
| **HJ-006** | Authoritative Test Classification, Test Level and test-design standard. |
| **HJ-005 and HJ-007** | Supporting implementation and enforcement standards; not sources of new Vendor Registration behaviour. |
| **Existing HJ-107** | Sole regeneration baseline for preservation and reconciliation of the HJ-107-owned `VR-*` Test ID namespace, retained catalogue content and HJ-107 change history. |
| **HJ-013** | Downstream owner of complementary architecture and implementation verification. PR-004 may reference the stable Document ID `HJ-013` when assigning responsibility boundaries, but shall not use an HJ-013 version, its contents or `AI-*` identifiers as HJ-107 behavioural or stable-ID inputs. |

Where authoritative artefacts disagree, identify the conflict instead of attempting to resolve it. If a required authoritative source is unavailable, record the missing authority rather than inferring its contents.

---

# Objective

Derive the complete Test Catalogue required to verify the Vendor Registration vertical slice.

The objective is **not** to redesign the service contract.

The objective is **not** to generate executable test code.

The objective **is** to convert every approved, testable business requirement into explicit test obligations that can later be implemented as automated or manual tests.

The Test Catalogue must make omissions visible. Every normative rule in HJ-106 Part A must either:

- have one or more corresponding test cases;
- be identified as non-testable at the current level; or
- be recorded as blocked by an unresolved external dependency.

---

# Scope

Derive tests for the following approved Vendor business operations:

- `RegisterVendor`
- `RetrieveRegisteredVendor`

Treat RegisterVendor as the primary business operation and derive RetrieveRegisteredVendor tests only where explicitly defined by the approved service contract.

At a minimum cover:

1. Successful Vendor Registration.
2. Request completeness.
3. Required fields.
4. Optional fields.
5. Conditional fields.
6. Controlled values.
7. Validation rules.
8. Canonicalisation rules.
9. Registration Declaration acceptance.
10. Registration Declaration transience.
11. Address Domain collaboration.
12. Address trust-boundary enforcement.
13. Derived Address information.
14. Vendor aggregate creation invariants.
15. Initial Vendor lifecycle state.
16. Initial Trading Preference.
17. Registered Information persistence.
18. Vendor Managed Information persistence.
19. Domain Event behaviour.
20. Integration Event behaviour.
21. Minimum Integration Event business content.
22. Atomic Vendor and publication-work recording.
23. Idempotent successful replay.
24. Concurrent duplicate submission.
25. Business failure behaviour.
26. Prohibited outcomes.
27. Scope exclusions.
28. Registered Vendor retrieval.
29. Retrieval side-effect invariants.
30. Registered Vendor Details representation.
31. Retrieval scope exclusions.
32. Traceability completeness.

Do not infer tests for operations that HJ-106 explicitly excludes.

Derive tests only for RegisterVendor and RetrieveRegisteredVendor.
Do not invent additional Vendor queries or search capabilities.

---

# Test Basis and Authority

Apply the following authority order.

## Primary Test Basis

Use **HJ-106 Part A – Business Service Contract** as the normative source for test derivation.

Every normative statement in Part A must be assessed for test coverage.

## Supporting Sources

Use:

- HJ-003 for approved terminology;
- HJ-004 for aggregate invariants, lifecycle and event definitions;
- HJ-104 for field, validation, classification, canonicalisation and lifecycle rules;
- HJ-105 for processing order, outcomes, failures and collaboration behaviour;
- ADR-004 for Registration Session ownership and the Vendor lifecycle boundary;
- ADR-006 for Address ownership and snapshot authority;
- ADR-008 for idempotency and reliable-publication principles;
- HJ-006 for approved test levels, naming, automation and quality standards.

Supporting sources may clarify the origin of a requirement but must not introduce behaviour absent from the approved service contract.

## Non-Normative Technical Proposal

HJ-106 Part B may be used only to derive a separate set of **proposed API contract tests**.

Tests derived from Part B must be clearly labelled:

> Non-normative technical convention pending approval.

Do not mix proposed HTTP behaviour with normative business tests.

## Architectural Governance Inputs

Use HJ-010 to identify Current Architectural Concerns whose unresolved decisions constrain test derivation or implementation. Carry the applicable `CON-xxx` references into dependency records and do not select their Approaches.

Use HJ-012 to confirm approved implementation architecture. HJ-012 does not introduce Vendor Registration service behaviour absent from HJ-106 Part A.

## HJ-107 and HJ-013 Responsibility Boundary

HJ-107 owns behavioural test obligations for `RegisterVendor` and `RetrieveRegisteredVendor` derived from HJ-106 Part A and its authoritative sources.

HJ-013 owns complementary architecture, dependency, persistence-mechanism, transaction-mechanism, reliable-publication-mechanism and runtime verification. HJ-107 shall not become an omnibus architecture and implementation test register.

Apply these rules:

- HJ-107 may verify observable persistence, atomicity, idempotency and publication outcomes required by HJ-106.
- HJ-013 verifies implementation mechanisms that preserve those outcomes, including dependency enforcement, concrete mappings, transaction failure injection, outbox persistence, relay recovery and broker mechanics.
- One guarantee may require distinct obligations in both catalogues at different boundaries.
- Do not duplicate an identical obligation merely to create coverage in both catalogues.
- State the HJ-107 behavioural boundary where potential overlap exists and leave complementary architecture or implementation verification to HJ-013.
- Where an obligation belongs to complementary architecture or implementation verification, identify `HJ-013` as the owning catalogue without selecting or referencing an `AI-*` identifier.
- Perform cross-catalogue reconciliation downstream when HJ-013 is generated from the current HJ-107.

## Catalogue ID Ownership

| Catalogue | Owned Namespace | Stable-ID Baseline | Permitted External References |
|---|---|---|---|
| **HJ-107** | `VR-*` | Previous HJ-107 | Authoritative source identifiers and `CON-*` dependencies; stable Document ID `HJ-013` for responsibility assignment only |
| **HJ-013** | `AI-*` | Previous HJ-013, governed outside PR-004 | Current HJ-107 `VR-*` identifiers where behavioural coverage is delegated |

Apply these rules:

1. HJ-107 shall create, preserve, amend, retire and reconcile only `VR-*` Test IDs.
2. The previous HJ-107 is the only stable-ID baseline for `VR-*` identifiers.
3. HJ-107 shall not preserve, create, validate or reconcile `AI-*` identifiers.
4. HJ-107 shall not require knowledge of the current HJ-013 version.
5. HJ-107 shall not contain a version-specific HJ-013 dependency.
6. HJ-107 shall not identify individual HJ-013 obligations as required inputs or outputs.
7. HJ-013 may be named by stable Document ID when a complementary responsibility is assigned to that catalogue.

## Regeneration Direction and Trigger Rule

Apply the controlled generation order:

```text
authoritative behavioural sources + previous HJ-107
    -> regenerate HJ-107 and reconcile VR-* IDs

authoritative architecture sources + current HJ-107 + previous HJ-013
    -> regenerate HJ-013 and reconcile AI-* IDs

current HJ-107 + current HJ-013
    -> perform cross-catalogue completion check
```

PR-004 governs the first stage only. The HJ-013 generation method governs the second stage and its `AI-*` reconciliation separately.

An HJ-013 version change alone does not trigger HJ-107 regeneration. HJ-107 regeneration is triggered only by a controlled change to its behavioural sources, architectural dependency inputs, governing generation prompt or HJ-107 itself. HJ-107 does not become stale merely because HJ-013 is regenerated.

---

# Architectural Constraints

Preserve the architectural decisions already established.

Do **not**:

- redesign the Vendor Domain;
- invent additional lifecycle states;
- invent fields, validation rules or business failures;
- invent Vendor query behaviour beyond the approved RetrieveRegisteredVendor contract;
- invent Identity or authorisation behaviour;
- invent Address Resolution reference lifetime or reuse rules;
- invent the concrete Compliance-facing Business Address event schema;
- turn technical conventions into business requirements;
- generate implementation code;
- prescribe test frameworks unsupported by the artefacts;
- weaken prohibited outcomes into optional assertions.

Preserve:

- Registration Session outside every Vendor service boundary;
- the complete `RegisterVendor` request as the authoritative input;
- Registered Information;
- Vendor Managed Information;
- transient Registration Declarations;
- Address Domain ownership;
- immutable Business Address snapshots;
- Canonical Address Identifier;
- Trading Characteristics;
- Legal Operator Type rules;
- Vendor creation invariants;
- `PendingActivation`;
- initial Trading Preference `Offline`;
- internal Domain Event and published Integration Event separation;
- mandatory idempotent replay behaviour;
- atomic persistence and publication-work recording;
- event-driven Pending Activation collaboration;
- RetrieveRegisteredVendor as a side-effect-free query;
- the persisted Vendor aggregate as the authoritative retrieval source;
- the Registered Vendor Details representation;
- absence of cross-domain collaboration during retrieval.

---

# Test Catalogue vs Test Implementation

The output is a **Test Catalogue**, not executable tests.

## Catalogue Responsibilities

The catalogue shall define:

- what must be verified;
- why it must be verified;
- the authoritative source;
- the required Test Classification and mapped Test Level;
- preconditions;
- input or stimulus;
- expected observable outcome;
- prohibited outcome;
- priority;
- dependency status; and
- automation recommendation.

## Implementation Responsibilities

Do not define:

- concrete test class names;
- test framework attributes;
- mocking-library syntax;
- database fixture implementation;
- message-bus emulator implementation;
- HTTP client code;
- assertion-library syntax;
- CI pipeline configuration.

Those belong to later test design and implementation activities.

---

# Test Levels

Assign each test to the lowest appropriate level without losing confidence in the behaviour being verified.

Use the authoritative HJ-006 mapping:

| Test Classification | Test Level |
|---|---|
| **Domain** | Unit |
| **Application** | Unit |
| **Persistence Integration** | Integration |
| **Integration Contract** | Integration |
| **API Contract** | API Integration |
| **Contract Review** | Non-executable review |

Every executable obligation shall identify exactly one approved Test Classification and its mapped Test Level.

End-to-End remains an executable Test Level and shall be used only where a complete workflow cannot be verified adequately at a lower boundary. Do not introduce an Architecture Test classification or any additional HJ-006 category.

Do not default every test to End-to-End.

Prefer focused tests at the lowest responsible architectural boundary.

---

# Test Design Rules

For every test case:

1. Use approved ubiquitous language.
2. Describe one primary behaviour.
3. State the precise observable result.
4. State any business state or event that must **not** occur.
5. Identify the exact source section.
6. Separate business expectations from technical conventions.
7. Avoid implementation-specific setup unless required by the source.
8. Identify unresolved dependencies rather than inventing behaviour.
9. Include positive, negative and boundary coverage where the source supports it.
10. Include concurrency and retry coverage where mandated by ADR-008 and HJ-106.

A test is incomplete if it verifies only an error response but fails to verify prohibited side effects such as:

- no Vendor created;
- no completed business fact recorded;
- no Domain Event recorded;
- no durable publication record created;
- no Integration Event published;
- no Pending Activation Process initiated.

---

# Required Coverage

## Successful Registration

Verify that a complete and valid request:

- creates exactly one Vendor;
- persists the approved Registered Information;
- persists supplied Vendor Managed Information;
- stores Address-owned values exactly as returned by the Address Service;
- begins in `PendingActivation`;
- begins with Trading Preference `Offline`;
- records the internal `VendorRegistered` Domain Event;
- records durable Integration Event publication work atomically with the Vendor;
- returns the authoritative committed outcome;
- enables later publication of the `VendorRegistered` Integration Event.

## Registered Vendor Retrieval

Verify that RetrieveRegisteredVendor:

- loads a Vendor using VendorId;
- uses the persisted Vendor aggregate as the authoritative source;
- returns Registered Vendor Details;
- returns all Registered Information;
- returns all Vendor Managed Information;
- excludes Registration Declarations;
- excludes Compliance state;
- excludes Domain Events;
- excludes Integration Events;
- excludes persistence metadata;
- performs no lifecycle transition;
- records no Domain Event;
- creates no publication work;
- publishes no Integration Event;
- initiates no Pending Activation Process;
- returns the controlled Vendor Not Found outcome when VendorId does not exist.

## Registration Information

Derive coverage for every field in HJ-106 and HJ-104, including:

- required;
- optional;
- conditional;
- derived;
- controlled-value;
- length;
- format;
- canonicalisation;
- classification;
- persistence;
- absence rules.

Use parameterised catalogue entries where this improves clarity without hiding materially different business outcomes.

## Registration Declarations

Verify that all declarations:

- must be explicitly accepted;
- affect whether registration may proceed;
- are not persisted on the Vendor aggregate;
- do not become Vendor business state;
- do not appear in the Domain Event;
- do not appear in the Integration Event;
- have no lifecycle after request processing.

## Address Collaboration

Verify that:

- only an approved Address Resolution reference is accepted from the caller;
- the application obtains Address-owned values from the Address Service;
- client-supplied snapshot or authority values cannot become authoritative;
- the Canonical Address Identifier and snapshot are stored together;
- the snapshot is stored exactly as returned;
- Food Registration Authority is present;
- Primary Trading Authority is present only for `Stall`;
- Address failure creates no Vendor or business event.

## Creation Invariants

Verify every invariant listed in HJ-106 and HJ-004, including both presence and absence requirements.

## Idempotency

Verify that an identical successful replay using the same approved identity:

- returns the original outcome;
- creates no additional Vendor;
- records no additional completed business fact;
- records no additional Domain Event;
- creates no additional durable publication record;
- publishes no additional Integration Event;
- initiates no additional Pending Activation Process.

Verify concurrent requests with the same identity converge on one owner and one outcome.

Verify that reuse of the same idempotency identity with semantically different registration information produces the approved controlled idempotency-conflict outcome.

Verify that such processing:

- returns the approved conflict outcome;
- creates no additional Vendor;
- records no additional completed business fact;
- records no additional VendorRegistered Domain Event;
- creates no additional durable publication record;
- publishes no additional VendorRegistered Integration Event; and
- initiates no additional Pending Activation Process.

## Reliable Publication

Verify that:

- Vendor persistence and durable publication work commit atomically;
- partial success is not reported;
- publication retry does not rerun `RegisterVendor`;
- the required observable publication outcome remains recoverable after commit without repeating registration;
- the internal Domain Event is not assumed to be identical to the published Integration Event.

Keep the verification boundary explicit:

- HJ-107 owns the behavioural guarantees that successful registration creates the durable publication obligation, failure creates no partial business outcome, retry does not repeat registration and the required observable publication outcome is preserved.
- HJ-013 owns concrete Transactional Outbox implementation, database-boundary atomicity, failure injection, outbox persistence, relay restart/recovery and broker-delivery mechanics.

CON-017 and HJ-012 may be referenced to show that Transactional Outbox is approved architecture. Do not make outbox representation part of a public request, response or Integration Event contract.

## Business Failures

HJ-106 Part A, Section 4.11.

For each failure verify:

- the trigger;
- the returned business outcome;
- retry guidance where defined;
- prohibited persistence;
- prohibited Domain Event recording;
- prohibited publication work;
- prohibited Integration Event publication;
- prohibited Pending Activation initiation.

---

# Open and Blocked Test Areas

Do not invent behaviour for unresolved dependencies.

Derive the current unresolved in-scope dependency set from the supplied current HJ-010 baseline. Do not treat a concern as unresolved merely because an older prompt example or previous catalogue listed it as blocked.

Create explicit blocked or partially blocked catalogue entries for every currently unresolved in-scope dependency. Common dependency areas include:

| Concern Area | Treatment |
|---|---|
| Address collaboration and failures | Include only concerns that are not Approved in the current HJ-010 baseline. Preserve already approved Address behaviour as Ready where its test boundary is complete. |
| Idempotency, concurrency, replay persistence and transaction | Separate approved identity/equivalence behaviour from any still-unresolved concurrency, retention, persistence and transaction mechanisms. |
| Publication relay, event translation, event schema and broker delivery | Carry the exact current `CON-*` dependencies without selecting their unresolved Approaches. |
| Technical API contract, failure transport and validation allocation | Keep Part B tests proposed or blocked according to the current concern states. |
| Database mapping and constraints | Preserve behavioural guarantees while assigning concrete mapping and constraint verification to the applicable current concern and HJ-013 boundary. |

The concern identifiers and states recorded in the generated catalogue shall come from current HJ-010, not from this illustrative area list.

For every affected obligation:

- derive the behavioural guarantee that is already normative;
- identify the applicable `CON-xxx` dependency;
- state what can already be tested;
- state what remains blocked; and
- do not select an Approach, schema, format, mapping, persistence mechanism, relay, broker policy or implementation framework.

Identity authentication, authorisation and caller-to-Vendor association remain outside Epic 1. Record them as scope exclusions unless an approved source makes them dependencies of an in-scope obligation.

Each blocked entry shall identify:

- the missing contract;
- the affected Test IDs;
- the Current Concern ID or IDs;
- why testing cannot be completed;
- the owning capability or decision required;
- what can already be tested;
- what remains blocked; and
- the effect on implementation readiness.

---

# Required Output Structure

Produce a complete Markdown document using the following structure and apply the appropriate document-control mode.

For initial generation, create HJ-107 using the applicable project document-control rules.

For regeneration:

- use the existing HJ-107 as the document baseline;
- preserve Document ID, title and standard filename;
- increment the internal document version according to the controlled change being applied;
- set Status according to the applicable document-review decision rather than resetting it automatically;
- update Last Updated and Revision History;
- retain unaffected catalogue content; and
- record the exact source baselines used, including the version and Approved status of the supplied current HJ-106 baseline.

Do not append a version or status suffix to the filename.

## Revision History

## Related Documents

Identify HJ-013 as **HJ-013 - Architecture and Implementation Test Catalogue**, using `Current` or omitting a version value. Describe it as the downstream complementary catalogue. Do not describe an HJ-013 version as a regeneration or source-authority input, and do not include `AI-*` identifiers.

This rule applies when HJ-107 is next regenerated for a substantive reason. An existing HJ-013 version reference does not by itself require HJ-107 regeneration.

## 1. Purpose

State that the document is the authoritative catalogue of test obligations derived from HJ-106 and its approved sources.

Clarify that it does not contain executable test code.

## 2. Source Authority

For each attached artefact identify:

- why it was used;
- what authority it provides;
- whether it is normative, supporting or non-normative.

Do not list a particular HJ-013 version as a supplied source-authority or cross-catalogue review input. HJ-013 may be identified by stable Document ID only when explaining the complementary responsibility boundary.

## 3. Catalogue Conventions

Define:

- Test ID format;
- Test Classifications and their authoritative HJ-006 Test Level mapping;
- priority levels;
- automation status values;
- dependency status values;
- meaning of prohibited outcomes;
- distinction between normative and proposed tests.

## 4. Coverage Summary

Provide a table containing:

- Test Area;
- Number of Test Obligations;
- Primary Test Classification / Level;
- Source;
- Coverage Status;
- Open Dependencies.

## 5. Test Catalogue

Organise the catalogue into the following subsections.

### 5.1 Successful Registration

### 5.2 Request Completeness and Required Information

### 5.3 Legal Operator Rules

### 5.4 Trading Characteristics

### 5.5 Contact Information

### 5.6 Vendor Managed Information

### 5.7 Registration Declarations

### 5.8 Address Collaboration and Derived Information

### 5.9 Aggregate Creation Invariants

### 5.10 Vendor Lifecycle and Initial State

### 5.11 Domain Event Behaviour

### 5.12 Integration Event Behaviour

### 5.13 Idempotency and Concurrency

### 5.14 Persistence and Reliable Publication

### 5.15 Registered Vendor Retrieval

### 5.16 Business Failures

### 5.17 Scope Exclusions and Prohibited Behaviour

For every test obligation provide a table row or structured entry containing:

- Test ID;
- Title;
- Requirement;
- Source Artefact;
- Source Section;
- Test Classification / Level;
- Priority;
- Preconditions;
- Input / Stimulus;
- Expected Result;
- Prohibited Outcome;
- Automation Recommendation;
- Dependency Status;
- Notes.

Priority and Dependency Status are separate fields. Do not encode blocked state as Priority.

## 6. Proposed API Contract Tests

Derive tests from HJ-106 Part B only.

Clearly mark the entire section:

> Non-normative technical conventions pending approval.

Separate:

- request-shape tests;
- success-response tests;
- error-contract tests;
- HTTP status tests;
- header tests;
- replay-response tests.

Do not treat them as approved business requirements.

## 7. Blocked and Deferred Tests

Provide a table containing:

- Test Area;
- affected Test IDs;
- Missing Contract or Decision;
- Current Concern ID or IDs;
- authoritative owner or source required;
- what can already be tested;
- what remains blocked;
- required resolution; and
- implementation/readiness impact.

## 8. Requirement-to-Test Traceability Matrix

Provide a table containing:

- Requirement ID or Contract Element;
- HJ-106 Section;
- Upstream Source;
- Test IDs;
- Coverage Status;
- Notes.

Every normative HJ-106 Part A requirement must appear.

## 9. Completeness Analysis

Identify:

- normative requirements with test coverage;
- requirements covered at multiple levels;
- requirements intentionally covered once;
- blocked requirements;
- non-testable statements;
- potential duplicate tests;
- behavioural obligations owned by HJ-107;
- complementary architecture or implementation obligations owned by HJ-013;
- any source ambiguity or conflict.

Do not claim that HJ-107 completeness depends on a particular HJ-013 version or on the existence of particular `AI-*` obligations.

Do **not** silently resolve gaps.

## 10. Assumptions and Open Questions

Classify each item as:

- Confirmed;
- Test Design Convention;
- Technical Convention;
- Blocked Dependency;
- Ambiguity;
- Missing Information;
- Artefact Conflict.

## 11. Review Checklist

Confirm that the Test Catalogue:

- uses Part A of the supplied current Approved HJ-106 baseline as its primary normative behavioural basis and records the exact version used;
- uses the approved HJ-006 Test Classification and Test Level mapping;
- preserves stable Test IDs or records their controlled reconciliation;
- preserves and reconciles only HJ-107-owned `VR-*` identifiers;
- uses the previous HJ-107 as the sole `VR-*` stable-ID baseline;
- does not use HJ-013 as a behavioural, regeneration or stable-ID input;
- contains no version-specific HJ-013 dependency;
- contains no `AI-*` identifiers;
- uses stable Document ID `HJ-013` only to assign complementary verification responsibility;
- does not treat an HJ-013-only change as an HJ-107 regeneration trigger;
- reports preserved, materially changed, added and retired or restructured `VR-*` identifiers;
- keeps Priority distinct from Dependency Status;
- identifies applicable HJ-010 Current Concern dependencies;
- does not select an unresolved architectural Approach;
- does not duplicate HJ-013 architecture or implementation obligations;
- treats HJ-012 as approved architecture rather than a source of new service behaviour;
- treats Identity as outside Epic 1 unless an approved source establishes an in-scope dependency;
- covers every normative statement in HJ-106 Part A;
- preserves bounded-context ownership;
- uses ubiquitous language consistently;
- covers every HJ-104 field rule;
- covers every HJ-004 creation invariant;
- covers every HJ-106 RegisterVendor requirement;
- covers every HJ-106 RetrieveRegisteredVendor requirement;
- covers every HJ-106 business failure;
- verifies prohibited side effects;
- covers idempotent replay and concurrency;
- covers atomic persistence and publication work;
- keeps Domain and Integration Events distinct;
- does not invent unresolved external contracts;
- separates Part A business tests from Part B proposed API tests;
- keeps all Part B-derived tests explicitly Proposed and non-normative;
- identifies blocked dependencies explicitly;
- contains complete requirement-to-test traceability;
- contains no executable implementation code.

## 12. Regeneration Reconciliation

When regenerating an existing HJ-107, provide a table containing:

- Test ID;
- Previous Treatment;
- Regenerated Treatment; and
- Reason.

Record preserved, materially changed, added, retired, merged, split and superseded `VR-*` obligations explicitly enough to make omissions and Test ID changes visible.

Report completion totals for:

- preserved `VR-*` IDs;
- materially changed `VR-*` IDs;
- newly added `VR-*` IDs;
- retired, merged, split or superseded `VR-*` IDs; and
- unresolved source or cross-catalogue responsibility references.

No `AI-*` identifier shall appear in this reconciliation.

---

# Test ID Convention

Use stable identifiers in the following form:

```text
VR-<AREA>-<NNN>
```

Suggested area codes:

- `SUCCESS`
- `REQ`
- `LEGAL`
- `TRADING`
- `CONTACT`
- `MANAGED`
- `DECL`
- `ADDRESS`
- `INV`
- `STATE`
- `RETRIEVE`
- `DOMAIN-EVENT`
- `INTEGRATION-EVENT`
- `IDEMP`
- `RELIABILITY`
- `FAILURE`
- `SCOPE`
- `API`
- `BLOCKED`

Do not encode implementation layer, class name or test framework into the identifier.

## Stable Test ID Reconciliation

When regenerating HJ-107:

1. Preserve an existing Test ID when its behavioural obligation remains semantically unchanged.
2. Amend the existing entry under the same Test ID when traceability, classification, dependency or wording changes without changing the essential obligation.
3. Create a new Test ID only for a genuinely new and independently testable obligation.
4. Do not reuse a retired Test ID for another obligation.
5. Record removed, merged, split, superseded and newly added Test IDs explicitly.
6. Produce the Regeneration Reconciliation required by the output structure.

These rules apply exclusively to the HJ-107-owned `VR-*` namespace. Do not preserve, create, validate, reconcile or report `AI-*` identifiers. The previous HJ-107 is the sole stable-ID baseline.

Do not renumber retained obligations merely to make identifiers contiguous.

---

# Priority Classification

Classify test obligations as:

- **Critical** – protects ownership, aggregate validity, idempotency, atomicity, event correctness or prohibition of duplicate business effects.
- **Important** – protects required business behaviour, collaboration contracts, lifecycle outcomes or controlled failure behaviour.
- **Standard** – validates supporting field rules, optional behaviour or non-critical boundaries.
- **Proposed** – derived only from non-normative Part B technical conventions.

Do not assign every test the same priority.

Blocked is a Dependency Status, not a Priority.

---

# Dependency Status Classification

Use these controlled values:

- **Ready** – the obligation is sufficiently defined for test implementation.
- **Partially Blocked** – part of the obligation is testable, but identified assertions or cases depend on unresolved authority.
- **Blocked** – the obligation cannot be implemented responsibly until the identified contract or decision is approved.

If **Deferred** is required, use it only where an approved scope or sequencing decision deliberately defers the obligation. Do not use Deferred merely because required information is missing.

---

# Shared Baseline, Output and Verification Control

This prompt is governed by **PR-008 - Global Output and Verification Rules** for baseline validation, filenames, output packaging, source-write boundaries, document control, common preflight verification, direct links and the human-review handoff.

Use **HJ-000 - Current Approved Baseline Manifest** as a compact index when available. Validate HJ-106, the previous HJ-107 and required supporting artefacts against their actual controlled metadata; HJ-000 never replaces source authority, the previous HJ-107 stable-ID baseline or approval evidence.

For HJ-107 generation or regeneration:

- use `HJ-107 - Vendor Registration Test Catalogue.md` as the output filename;
- never append version or status to that filename;
- include a concise regeneration and stable-ID reconciliation summary;
- include an HJ-000 candidate only when approval of the generated HJ-107 changes its indexed version or status; and
- do not recreate HJ-107 after approval if the human has already applied the reviewed file unchanged.

Complete the PR-008 common preflight and catalogue-integrity verification before presenting the result. Any conflict between PR-008 and this prompt shall be reported rather than silently resolved.

# Output Format

Produce a **complete Markdown document** suitable for direct inclusion in the HotJoes documentation repository.

Requirements:

- Use standard GitHub Markdown.
- Use numbered headings.
- Use Markdown tables.
- Do not use HTML.
- Do not use Mermaid diagrams.
- Do not generate executable test code.
- Do not omit sections because information is unavailable.
- Record unavailable information as blocked, ambiguous or missing.
- The document should require no further formatting before being committed to Git.

---

# Review Standard

The completed document should be sufficiently detailed to become the baseline for:
Both RegisterVendor and RetrieveRegisteredVendor shall be treated as approved business operations for Epic 1. The resulting Test Catalogue shall derive complete test obligations for both operations while preserving the approved service boundaries and ownership rules.

- Test Architecture Review;
- Domain Test Design;
- Application Test Design;
- Persistence Integration Test Design;
- API Contract Test Design;
- Integration Event Contract Test Design;
- Test Automation Planning;
- Test Implementation;
- CI quality-gate definition.

Do **not**:

- generate unit-test code;
- generate integration-test code;
- generate API-test scripts;
- generate mocks or stubs;
- generate test data builders;
- generate CI pipeline configuration;
- approve unresolved HTTP conventions;
- resolve missing Address, Identity, Compliance or query contracts.

The output should describe **what must be tested and why**, not how the tests are implemented.

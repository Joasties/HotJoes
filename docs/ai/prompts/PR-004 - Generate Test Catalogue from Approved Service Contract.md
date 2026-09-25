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

Use the supplied current Approved HJ-106 baseline as the primary normative test basis. Part A governs transport-independent business behaviour. Part B governs API behaviour where its technical contract is supported by Approved architectural concerns. Record the exact HJ-106 version used.

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
| **HJ-106 Part A** | Primary normative basis for transport-independent behavioural test derivation. |
| **HJ-106 Part B** | Normative basis for API Contract tests when the applicable technical concerns are Approved in the current HJ-010/HJ-012 baseline; otherwise it may produce Proposed tests only. |
| **HJ-010** | Identifies unresolved architectural concerns and dependencies that generation must preserve rather than resolve. |
| **HJ-012** | Identifies approved implementation architecture; it may constrain interpretation but does not introduce Vendor Registration behaviour or technical contract requirements absent from the current Approved HJ-106 contract. |
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

Derive tests for the following approved Epic 1 business operations represented by the service contract:

- `DetermineRequiredLicenceTypes`
- `RegisterVendor`
- `RetrieveRegisteredVendor`
- `JoinCommunity`

Treat RegisterVendor as the primary Vendor creation operation. Treat DetermineRequiredLicenceTypes as the separate synchronous, side-effect-free pre-registration operation defined by the approved service contract. Derive RetrieveRegisteredVendor tests only where explicitly defined by the approved service contract. Treat JoinCommunity as a separate post-registration Community Application operation; its inclusion does not make Community Participation or Contact Preference part of Vendor Registration or Vendor state.

At a minimum cover:

1. Required Licence Types Determination.
2. Join Community.
3. Successful Vendor Registration.
4. Request completeness.
5. Required fields.
6. Optional fields.
7. Conditional fields.
8. Controlled values.
9. Validation rules.
10. Canonicalisation rules.
11. Registration Declaration acceptance.
12. Registration Declaration transience.
13. Address Domain collaboration.
14. Address trust-boundary enforcement.
15. Derived Address information.
16. Vendor aggregate creation invariants.
17. Initial Vendor lifecycle state.
18. Initial Trading Preference.
19. Registered Information persistence.
20. Vendor Managed Information persistence.
21. Domain Event behaviour.
22. Integration Event behaviour.
23. Minimum Integration Event business content.
24. Atomic Vendor and publication-work recording.
25. Idempotent successful replay.
26. Concurrent duplicate submission.
27. Business failure behaviour.
28. Prohibited outcomes.
29. Scope exclusions.
30. Registered Vendor retrieval.
31. Retrieval side-effect invariants.
32. Registered Vendor Details representation.
33. Retrieval scope exclusions.
34. Traceability completeness.

Do not infer tests for operations that HJ-106 explicitly excludes.

Derive tests only for DetermineRequiredLicenceTypes, RegisterVendor, RetrieveRegisteredVendor and JoinCommunity.
Do not invent additional Vendor queries, Community amendment or withdrawal operations, communication consent or message-delivery capabilities.

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
- ADR-007 for Compliance ownership and the separation of pre-registration determination from post-registration Compliance Requirement processing;
- ADR-008 for idempotency and reliable-publication principles;
- HJ-006 for approved test levels, naming, automation and quality standards.

Supporting sources may clarify the origin of a requirement but must not introduce behaviour absent from the approved service contract.

## Technical API Contract Authority

Derive API Contract tests from HJ-106 Part B.

When the applicable technical API concerns are Approved in the current HJ-010/HJ-012 baseline, treat the resulting API obligations as normative and assign their appropriate normal priority and dependency status.

When those concerns are unresolved, label the affected tests:

> Non-normative technical convention pending approval.

Do not infer approval from a Draft or proposed HJ-106 representation alone.

## Architectural Governance Inputs

Use HJ-010 to identify Current Architectural Concerns whose unresolved decisions constrain test derivation or implementation. Carry the applicable `CON-xxx` references into dependency records and do not select their Approaches.

**HJ-012** does not introduce Vendor Registration behaviour or technical contract requirements absent from the current Approved HJ-106 contract.

## HJ-107 and HJ-013 Responsibility Boundary

HJ-107 owns transport-independent behavioural test obligations derived from HJ-106 Part A and API Contract test obligations derived from HJ-106 Part B where the applicable technical concerns are Approved.

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
- treat Community Participation or Contact Preference as Vendor aggregate or Vendor Registration information;
- expose Primary Contact information to Community; or
- invent Community amendment, withdrawal, deletion, expiry, communication consent, recipient resolution or message delivery.

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

Also preserve for `DetermineRequiredLicenceTypes`:

- Compliance ownership of applicability policy, result meaning, canonical ordering and the active Rule Set Version;
- Vendor Application ownership of validation, authoritative Address resolution and orchestration only;
- the complete five-item Compliance Determination;
- transient Registration Session retention, controlling-input invalidation and fresh pre-submission refresh;
- renewed review when the item set or Rule Set Version changes;
- direct RegisterVendor independence from determination state and Compliance availability; and
- absence of Vendor, Compliance Requirement, Licence Details, persistence, event, outbox and external-regulatory-call side effects.

Also preserve for `JoinCommunity`:

- Community ownership of the operation, immutable record, persistence and Integration Event;
- the exact VendorId and Contact Preference request, with affirmative participation implied by invocation;
- Email, SMS and WhatsApp as the complete Epic 1 Contact Preference set;
- minimal Vendor-owned verification of successful registration without exposing Vendor or Primary Contact information;
- one Community Participation and at most one logical event per Vendor;
- original-result replay for the same preference and controlled conflict for a different preference;
- atomic Community persistence and immutable outbox recording;
- durable idempotent Community stub receipt using EventId;
- no communication-delivery effect; and
- absence of amendment, withdrawal, deletion, expiry, Communication Consent, recipient resolution and message delivery.

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

## Required Licence Types Determination

Verify that `DetermineRequiredLicenceTypes`:

- accepts the complete controlling input set: Legal Operator Type, Trading Location, Weekly Opening Hours, Service Includes Hot Food, Alcohol Service and the selected Address Resolution Reference;
- validates every independently detectable controlling-input error before collaboration;
- resolves authoritative Address information through the Address boundary;
- prevents caller-authored Address, jurisdiction or competent-authority values from becoming authoritative Compliance input;
- permits Vendor Application to orchestrate validation, Address resolution and Compliance collaboration without owning or reproducing licence-applicability rules;
- obtains the complete Compliance Determination from the Compliance-owned versioned deterministic policy;
- returns the active Rule Set Version;
- returns exactly one ordered item for each of Food Business Registration, Street Trading Licence, Late Night Refreshment Licence, Premises Licence and Personal Licence Holder;
- returns explicit `IsRequired` values, canonical ordering and no duplicate licence types;
- always marks Food Business Registration as required;
- marks Street Trading Licence as required for `Stall`, using authoritative competent-authority context;
- marks Late Night Refreshment Licence as required when hot-food service overlaps any portion of 23:00 inclusive to 05:00 exclusive;
- covers the 23:00 and 05:00 boundaries, Closed and Open All Day days, same-day intervals and overnight intervals;
- marks Premises Licence and Personal Licence Holder as required when Alcohol Service is selected;
- fails closed when the determination cannot be supported authoritatively;
- is synchronous, deterministic, side-effect-free and non-persistent;
- creates no Vendor, Compliance Requirement, Licence Details, Domain Event, Integration Event, durable publication work or external regulatory call.

Verify the Registration Session lifecycle associated with the result:

- retains the determination, applied Rule Set Version and controlling-input fingerprint only as transient derived state;
- invalidates the retained determination whenever any controlling input changes;
- obtains a fresh determination immediately before Web submission;
- requires renewed user review when the determined item set or Rule Set Version changes;
- prevents that Web submission attempt and retains the draft when refresh fails;
- does not include the determination in `RegisterVendor`;
- does not require direct `RegisterVendor` callers to obtain or submit a determination;
- does not recalculate the determination inside Vendor Application during registration;
- does not persist, fingerprint, retrieve or publish the determination as Vendor lifecycle state.

Derive explicit controlled-failure coverage for:

- `DeterminationRequestValidationFailure`;
- `InvalidReference`;
- `InvalidAddressResult`;
- `UnsupportedDetermination`; and
- `ComplianceDeterminationTemporarilyUnavailable`.

For every determination failure, verify the controlled outcome, retry guidance where defined, retained Registration Session draft where applicable and every prohibited side effect.

## Join Community

Verify that `JoinCommunity`:

- accepts exactly one successfully registered VendorId and one Contact Preference;
- accepts Email, SMS and WhatsApp as the complete Epic 1 Contact Preference set;
- implies affirmative Community Participation through invocation and accepts no client-controlled participation Boolean;
- contains no Primary Contact information, Communication Consent, recipient resolution or delivery instruction;
- is owned by Community Application, including its result, immutable record, persistence and event;
- verifies successful registration through the minimal Vendor-owned verification boundary;
- receives no Vendor aggregate, Registered Vendor Details or Primary Contact information from verification;
- creates one immutable Community Participation containing CommunityParticipationId, VendorId, Contact Preference and original JoinedAt;
- atomically records the participation and one immutable outbox message in the Community-owned transaction;
- enforces unique VendorId as the replay and conflict boundary;
- returns `CommunityParticipationAlreadyRecorded` with the original result for an equivalent request without another effect;
- returns `CommunityParticipationConflict` for an existing Vendor with a different preference without amendment or another effect;
- resolves concurrent first requests to exactly one participation record and at most one logical Integration Event;
- returns the complete closed outcome set defined by HJ-106;
- creates one `CommunityParticipationRecorded` Integration Event v1 only for first successful commit, with envelope members `eventId`, fixed `eventType`, fixed `eventVersion`, `occurredAt` and `payload`;
- contains exactly `communityParticipationId`, `vendorId`, `joinedAt` and `contactPreference` in the event payload, using the approved UUID, UTC timestamp and lower-camel-case Contact Preference representations;
- preserves the exact immutable serialized event across publication retries;
- processes the event through the independently executable Community-owned deterministic stub;
- records durable consumer processing evidence using EventId as the idempotency key;
- suppresses equivalent redelivery and treats EventId reuse with different immutable content as an integrity failure;
- sends no Email, SMS, WhatsApp or other communication; and
- retains the authoritative record and original result for at least as long as the associated Vendor exists without introducing amendment, withdrawal, deletion or expiry.

Derive explicit controlled-failure coverage for:

- `RequestValidationFailure` for invalid VendorId or Contact Preference;
- `VendorNotFound`;
- `CommunityParticipationConflict`;
- `VendorVerificationUnavailable`; and
- `CommunityPersistenceUnavailable`.

For every Join Community failure, verify the controlled outcome, retry guidance where defined and the absence of Community Participation, outbox work and Integration Events. Verify that retry after an uncertain response converges through the approved equivalent-replay guarantee.

Verify the post-registration client boundary represented by the service contract:

- no Community request is made when the Vendor chooses not to join;
- absence is not stored as a negative participation record; and
- no later navigation is interpreted as Community amendment or withdrawal.

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

Use the controlled failures defined for all four approved operations in HJ-106 Part A, including RegisterVendor failures in Section 4.11, DetermineRequiredLicenceTypes failures in Section 4.13.7 and JoinCommunity outcomes in Section 4.14.4.

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
| Technical API contract, failure transport and validation allocation | Derive API tests according to the current concern states. Approved concerns produce normative Ready obligations; unresolved concerns produce Proposed, Partially Blocked or Blocked obligations as applicable. |
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

Include `DetermineRequiredLicenceTypes`, `JoinCommunity`, `RegisterVendor` and `RetrieveRegisteredVendor` explicitly in the coverage accounting.

## 5. Test Catalogue

Organise the catalogue into the following subsections.

### 5.1 Required Licence Types Determination

### 5.2 Join Community

### 5.3 Successful Registration

### 5.4 Request Completeness and Required Information

### 5.5 Legal Operator Rules

### 5.6 Trading Characteristics

### 5.7 Contact Information

### 5.8 Vendor Managed Information

### 5.9 Registration Declarations

### 5.10 Address Collaboration and Derived Information

### 5.11 Aggregate Creation Invariants

### 5.12 Vendor Lifecycle and Initial State

### 5.13 Domain Event Behaviour

### 5.14 Integration Event Behaviour

### 5.15 Idempotency and Concurrency

### 5.16 Persistence and Reliable Publication

### 5.17 Registered Vendor Retrieval

### 5.18 Business Failures

### 5.19 Scope Exclusions and Prohibited Behaviour

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

## 6. API Contract Tests

Derive tests from HJ-106 Part B.

State whether the section is normative or proposed according to the current HJ-010/HJ-012 concern states. Apply the “Non-normative technical conventions pending approval” label only where the governing technical concerns remain unresolved.

Separate:

- request-shape tests;
- success-response tests;
- error-contract tests;
- HTTP status tests;
- header tests;
- replay-response tests.

For `POST /vendor-registration/required-licence-types`, derive normative API Contract coverage for:

- the complete request shape and controlled values;
- prohibition of extra caller-authored Address or authority fields;
- the `200` response envelope containing the active Rule Set Version and the complete, canonically ordered five-item determination with explicit `IsRequired` values;
- the standard error envelope and approved HTTP status mapping for `DeterminationRequestValidationFailure`, `InvalidReference`, `InvalidAddressResult`, `UnsupportedDetermination` and `ComplianceDeterminationTemporarilyUnavailable`;
- the operation's side-effect-free POST semantics; and
- the absence of Vendor creation, persistence, events and publication work.

Treat these obligations as normative because the applicable HJ-106 Part B contract and CON-047 concern are Approved. Do not relabel them as pending technical conventions.

For `POST /community-participations`, derive normative API Contract coverage for:

- the exact lower-camel-case `vendorId` and `contactPreference` request;
- controlled wire values `email`, `sms` and `whatsApp`;
- compatible unknown-member handling and controlled missing, null or invalid required-member validation;
- exclusion of a participation Boolean and contact destination;
- the original `201 Created` representation for both first success and equivalent replay;
- stable `Location: /community-participations/{communityParticipationId}` behaviour;
- the standard safe error envelope and approved `400`, `404`, `409` and `503` mappings;
- absence of automatic API retry; and
- absence of HTTP-owned Community behaviour, Vendor verification, persistence, event construction or broker interaction.

Treat these obligations as normative because HJ-106 v2.2 Part B, CON-046 and the applicable HJ-010/HJ-012 technical baseline are Approved. Do not reinterpret the HTTP representation as Community Domain behaviour.

Do not treat API technical obligations as transport-independent business requirements. Where their governing concerns are Approved, treat them as normative API Contract requirements.

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

Trace all four approved operations explicitly, including every normative `DetermineRequiredLicenceTypes` and `JoinCommunity` requirement.

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

Report completeness separately for `DetermineRequiredLicenceTypes`, `JoinCommunity`, `RegisterVendor` and `RetrieveRegisteredVendor`.

## 10. Assumptions and Open Questions

Classify each item as:

- Confirmed;
- Test Design Convention;
- Technical Convention;
- Blocked Dependency;
- Ambiguity;
- Missing Information;
- Artefact Conflict.

Include any determination-specific dependency, ambiguity or missing information without weakening the approved fail-closed behaviour.

Include any Community-specific dependency, ambiguity or missing information without introducing amendment, withdrawal, consent or delivery behaviour.

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
- accounts explicitly for all four approved operations in the Coverage Summary, traceability, completeness analysis and reconciliation;
- preserves bounded-context ownership;
- uses ubiquitous language consistently;
- covers every HJ-104 field rule;
- covers every HJ-004 creation invariant;
- covers every HJ-106 DetermineRequiredLicenceTypes requirement;
- covers every HJ-106 JoinCommunity requirement;
- covers every HJ-106 RegisterVendor requirement;
- covers every HJ-106 RetrieveRegisteredVendor requirement;
- covers every HJ-106 business failure;
- verifies prohibited side effects;
- covers idempotent replay and concurrency;
- covers atomic persistence and publication work;
- keeps Domain and Integration Events distinct;
- separates Part A transport-independent business tests from Part B API Contract tests;
- assigns Part B-derived tests normative or Proposed treatment according to the current Approved concern baseline;
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

Reconcile all genuinely new `VR-DETERMINATION-*` and `VR-COMMUNITY-*` obligations while preserving every semantically unchanged existing `VR-*` identifier.

---

# Test ID Convention

Use stable identifiers in the following form:

```text
VR-<AREA>-<NNN>
```

Suggested area codes:

- `DETERMINATION`
- `COMMUNITY`
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
- **Proposed** – derived from a technical convention whose governing authority remains unresolved; do not use for API behaviour already approved through HJ-010/HJ-012 and HJ-106 Part B.

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
DetermineRequiredLicenceTypes, JoinCommunity, RegisterVendor and RetrieveRegisteredVendor shall be treated as approved business operations for Epic 1. The resulting Test Catalogue shall derive complete test obligations for all four operations while preserving the approved service boundaries and ownership rules.

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
- treat unresolved HTTP conventions as approved;
- resolve missing Address, Identity, Compliance or query contracts.

The output should describe **what must be tested and why**, not how the tests are implemented.

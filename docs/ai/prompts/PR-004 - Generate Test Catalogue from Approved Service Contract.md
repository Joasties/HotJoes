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
- HJ-104 – Vendor Registration Fields Matrix
- HJ-105 – Vendor Registration Sequence Diagram
- HJ-106 – Vendor Registration Service Contract
- ADR-002 through ADR-008

Treat the attached approved or accepted versions as the **single source of truth**.

Use **HJ-106 Part A – Business Service Contract** as the primary test basis.

Use upstream approved artefacts to:

- confirm source authority;
- understand the origin of requirements;
- resolve traceability references already identified by HJ-106; and
- verify that the Test Catalogue does not weaken or reinterpret the approved contract.

Where artefacts disagree, identify the conflict instead of attempting to resolve it.

Change Requests are not normative inputs unless explicitly identified as approved source material.

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
- the required test level;
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

Use only the levels supported by HJ-006. Where HJ-006 does not define a required level, use the following classifications provisionally and flag them for review:

- **Domain** – aggregate invariants, value objects, lifecycle state and Domain Event behaviour.
- **Application** – orchestration, validation, Address collaboration, idempotency coordination and failure handling.
- **Persistence Integration** – aggregate persistence, atomic publication-work recording and retry state.
- **Integration Contract** – Address and Compliance-facing published-contract obligations.
- **API Contract** – proposed Part B request, response, headers and HTTP mappings.
- **End-to-End** – only where behaviour cannot be proven reliably at a lower level.

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
- no publication or outbox record created;
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
- creates no additional publication or outbox record;
- publishes no additional Integration Event;
- initiates no additional Pending Activation Process.

Verify concurrent requests with the same identity converge on one owner and one outcome.

Verify that reuse of the same idempotency identity with semantically different registration information produces the approved controlled idempotency-conflict outcome.

Verify that such processing:

- returns the approved conflict outcome;
- creates no additional Vendor;
- records no additional completed business fact;
- records no additional VendorRegistered Domain Event;
- creates no additional publication or outbox record;
- publishes no additional VendorRegistered Integration Event; and
- initiates no additional Pending Activation Process.

## Reliable Publication

Verify that:

- Vendor persistence and durable publication work commit atomically;
- partial success is not reported;
- publication retry does not rerun `RegisterVendor`;
- Integration Event dispatch can recover independently after commit;
- the internal Domain Event is not assumed to be identical to the published Integration Event.

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

Create explicit blocked or deferred catalogue entries for:

- Address Resolution reference format, lifetime, reuse, revocation and expiry;
- concrete Compliance-facing Business Address event schema;
- Identity authentication, authorisation and caller-to-Vendor association;
- idempotency retention duration;
- payload-equivalence implementation;
- any unapproved Part B HTTP convention.

Each blocked entry shall identify:

- the missing contract;
- why testing cannot be completed;
- the owning capability or decision required;
- the effect on implementation readiness.

---

# Required Output Structure

Produce a complete Markdown document using the following structure.

```text
Document ID: HJ-107
Document Title: Vendor Registration Test Catalogue
Version: 0.1
Status: Draft
Classification: Test Catalogue
Owner: Project Architecture
```

## Revision History

## Related Documents

## 1. Purpose

State that the document is the authoritative catalogue of test obligations derived from HJ-106 and its approved sources.

Clarify that it does not contain executable test code.

## 2. Source Authority

For each attached artefact identify:

- why it was used;
- what authority it provides;
- whether it is normative, supporting or non-normative.

## 3. Catalogue Conventions

Define:

- Test ID format;
- test levels;
- priority levels;
- automation status values;
- dependency status values;
- meaning of prohibited outcomes;
- distinction between normative and proposed tests.

## 4. Coverage Summary

Provide a table containing:

- Test Area;
- Number of Test Obligations;
- Primary Test Level;
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
- Test Level;
- Priority;
- Preconditions;
- Input / Stimulus;
- Expected Result;
- Prohibited Outcome;
- Automation Recommendation;
- Dependency Status;
- Notes.

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
- Missing Contract or Decision;
- Owning Artefact or Capability;
- Why Blocked;
- Required Resolution;
- Impact.

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
- any source ambiguity or conflict.

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
- identifies blocked dependencies explicitly;
- contains complete requirement-to-test traceability;
- contains no executable implementation code.

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

---

# Priority Classification

Classify test obligations as:

- **Critical** – protects ownership, aggregate validity, idempotency, atomicity, event correctness or prohibition of duplicate business effects.
- **Important** – protects required business behaviour, collaboration contracts, lifecycle outcomes or controlled failure behaviour.
- **Standard** – validates supporting field rules, optional behaviour or non-critical boundaries.
- **Proposed** – derived only from non-normative Part B technical conventions.
- **Blocked** – cannot be completed until an external contract or decision exists.

Do not assign every test the same priority.

---

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

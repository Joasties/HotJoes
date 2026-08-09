# HotJoes AI Prompt

## PR-003 - Independent Review of Service Contract related Artefacts

| Metadata | Value |
|---|---|
| **Prompt ID** | PR-003 |
| **Title** | Independent Review of Service Contract related Artefacts |
| **Status** | Draft |
| **Owner** | Project Architecture |
| **Purpose** | Perform an independent consistency, completeness, traceability and architectural-quality review of the HotJoes artefact library through HJ-106 |

---

# 1. Objective

Perform an **independent external review** of the supplied HotJoes architectural artefacts up to and including **HJ-106 – Vendor Registration Service Contract**.

The purpose of the review is to determine whether the current artefact library forms a coherent, internally consistent and implementation-ready architectural baseline for the Vendor Registration vertical slice.

Review the artefacts as a connected architectural system rather than as isolated documents.

Identify:

- contradictions;
- inconsistent terminology;
- inconsistent ownership;
- conflicting lifecycle rules;
- mismatched service boundaries;
- traceability breaks;
- missing invariants;
- incomplete contracts;
- unresolved ambiguities;
- obsolete references;
- stale document-status or version references;
- gaps between upstream architecture and downstream derived artefacts;
- places where a downstream artefact invents behaviour not supported upstream;
- places where an upstream requirement is missing from a downstream artefact; and
- any condition that would weaken confidence in implementation or subsequent Vendor Registration Test Catalogue generation.

Do not redesign the architecture unless a genuine inconsistency or gap requires a recommended correction.

---

# 2. Artefacts in Scope

Review all supplied maintained architectural artefacts relevant to the baseline through HJ-106.

The expected artefact set includes, where supplied:

## Core Architecture and Governance

- HJ-001 – Project Vision
- HJ-002 – Architectural Principles
- HJ-003 – Ubiquitous Language Guide
- HJ-004 – Vendor Domain Models
- HJ-005 – Coding Standards
- HJ-006 – Testing Strategy and Standards
- HJ-007 – Enforcement Strategy
- HJ-008 – AI Roles and Responsibilities
- HJ-009 – AI Operating Guide

## Vendor Registration Artefacts

- HJ-104 – Vendor Registration Fields Matrix
- HJ-105 – Vendor Registration Sequence Diagram
- HJ-106 – Vendor Registration Service Contract

## Architectural Decision Records

- ADR-000 – Architectural Decision Register
- ADR-001 – Domain-Driven Design as the Primary Architectural Style
- ADR-002 – Business Capabilities and Bounded Contexts
- ADR-003 – Event-Driven Collaboration
- ADR-004 – Vendor Lifecycle Begins After Successful Registration
- ADR-005 – Registered Information vs Vendor Managed Information
- ADR-006 – Address Domain Ownership and Business Address Snapshots
- ADR-007 – Vendor Compliance as a Separate Bounded Context
- ADR-008 – Idempotent Operations and Reliable Event Publication
- any additional accepted ADR supplied with the review set and relevant to Vendor Registration.

---

# 3. Artefacts Explicitly Out of Scope

Do **not** review or treat the following as authoritative sources:

- Change Requests;
- superseded drafts;
- informal review notes;
- chat history;
- implementation code;
- generated source code;
- tests;
- backlog items;
- speculative design notes.

Change Requests may explain how an artefact evolved, but they are deliberately excluded from the review baseline and must not be used to override the current maintained artefacts.

Judge the **current maintained artefacts as they stand**.

HJ-106 is **in scope** and must be reviewed as the generated downstream Vendor Registration Service Contract.

---

# 4. Concern-Based Authority Model

Do not assume that one document is globally authoritative for every concern.

Use each artefact according to its defined responsibility.

## HJ-001 – Project Vision

Authoritative for:

- project purpose;
- strategic business direction;
- business objectives;
- scope boundaries;
- long-term intent.

## HJ-002 – Architectural Principles

Authoritative for:

- architectural principles;
- ownership principles;
- integration principles;
- design principles;
- reliability and governance philosophy.

## HJ-003 – Ubiquitous Language Guide

Authoritative for:

- business terminology;
- business concept names;
- commands;
- queries;
- events;
- lifecycle language.

## HJ-004 – Vendor Domain Models

Authoritative for:

- Vendor aggregate;
- Vendor lifecycle;
- Vendor properties;
- business invariants;
- Domain behaviour;
- Domain Events;
- ownership within the Vendor Domain;
- registered Vendor retrieval model.

## HJ-005 – Coding Standards

Authoritative for:

- coding and implementation standards;
- API model separation;
- error-handling conventions;
- implementation constraints;
- engineering-quality expectations.

HJ-005 must not override business behaviour established by the business and domain artefacts.

## HJ-006 – Testing Strategy and Standards

Authoritative for:

- testing philosophy;
- test levels;
- approved test classifications;
- testing boundaries;
- testing-quality expectations.

## HJ-007 – Enforcement Strategy

Authoritative for:

- enforcement mechanisms;
- automated enforcement;
- review enforcement;
- CI and tooling enforcement;
- enforcement responsibilities.

## HJ-008 – AI Roles and Responsibilities

Authoritative for:

- roles AI may perform;
- AI responsibilities;
- AI authority boundaries;
- human accountability.

## HJ-009 – AI Operating Guide

Authoritative for:

- how approved AI roles operate;
- human review gates;
- role hand-off behaviour;
- AI self-review;
- escalation;
- tool usage;
- workflow progression.

## HJ-104 – Vendor Registration Fields Matrix

Authoritative for:

- Vendor Registration information;
- field classification;
- required, optional and conditional information;
- field validation;
- Registration Declarations;
- ownership of registration information;
- information lifecycle.

## HJ-105 – Vendor Registration Sequence Diagram

Authoritative for:

- interaction order;
- runtime collaboration;
- successful and failure flows;
- idempotent replay;
- controlled idempotency conflict;
- persistence/publication sequence;
- registered Vendor retrieval sequence.

## HJ-106 – Vendor Registration Service Contract

Treat HJ-106 as the **derived service contract under review**.

It must faithfully reflect the approved upstream architecture.

It must not become the source of authority for business behaviour that does not exist upstream.

## ADRs

Treat each ADR as authoritative for the architectural decision it explicitly defines.

Where an ADR and a non-ADR artefact concern the same subject, assess whether they are aligned rather than assuming one automatically overrides the other.

---

# 5. Review Method

Perform the review in the following order.

## Stage 1 – Internal Document Review

Review each artefact for internal consistency.

Check:

- terminology;
- version and status;
- Related Documents;
- internal cross-references;
- contradictory statements within the same document;
- obsolete section references;
- stale status references;
- unresolved placeholders;
- inconsistent naming;
- mismatches between narrative, tables and diagrams.

Do not report cosmetic Markdown differences unless they create ambiguity, materially damage readability or indicate inconsistent document governance.

## Stage 2 – Cross-Artefact Consistency

Compare the artefacts across the library.

Check whether:

- Project Vision is respected;
- Architectural Principles are followed;
- Ubiquitous Language is used consistently;
- Vendor Domain behaviour matches terminology;
- HJ-104 fields match HJ-004 properties and invariants;
- HJ-105 sequences implement HJ-004 and HJ-104 correctly;
- ADR decisions are reflected in HJ-003, HJ-004, HJ-104 and HJ-105;
- HJ-106 faithfully derives its business contract from the approved upstream artefacts;
- HJ-005 technical standards do not redefine business rules;
- HJ-006 testing taxonomy is sufficient and internally coherent;
- AI governance documents remain consistent with the human-governed delivery model.

## Stage 3 – Traceability Review

Trace important business and architectural rules from their authoritative source to downstream representation.

At minimum trace:

- Vendor existence boundary;
- Registration Session ownership;
- complete `RegisterVendor` request boundary;
- Registered Information;
- Vendor Managed Information;
- Registration Declarations;
- Legal Operator Type;
- Legal Operator Name;
- Company Registration Number rule;
- Trading Characteristics;
- Address ownership;
- Canonical Address Identifier;
- immutable Business Address Snapshot;
- Food Registration Authority;
- Primary Trading Authority;
- Vendor initial state;
- Trading Preference;
- Pending Activation;
- internal `VendorRegistered` Domain Event;
- published `VendorRegistered` Integration Event;
- Integration Event minimum business payload;
- persistence/publication atomicity;
- idempotency safeguard;
- replay semantics;
- controlled idempotency conflict;
- `RetrieveRegisteredVendor`;
- Vendor Not Found;
- retrieval side-effect invariants.

For each important rule, determine whether it is:

1. defined by an authoritative artefact;
2. represented consistently in downstream artefacts;
3. contradicted anywhere; or
4. omitted where it should appear.

---

# 6. Specific Review Areas

## 6.1 Terminology

Identify:

- deprecated terms;
- historical synonyms presented as authoritative terminology;
- inconsistent state names;
- inconsistent command or query names;
- inconsistent event names;
- inconsistent Address terminology;
- inconsistent Vendor Registration terminology.

Distinguish harmless explanatory synonyms from genuine ubiquitous-language defects.

## 6.2 Ownership and Bounded Contexts

Verify that:

- each authoritative concept has one owner;
- Vendor does not become authoritative for Address-owned information;
- Compliance remains outside the Vendor aggregate;
- Registration Session remains outside the Vendor Domain and Vendor Registration service boundary;
- bounded contexts do not silently acquire behaviour owned elsewhere;
- downstream artefacts do not reconstruct information owned by another bounded context.

## 6.3 Vendor Lifecycle

Verify alignment on:

- no Vendor before successful registration;
- initial Vendor State;
- initial Trading Preference;
- separation of registration from activation;
- Pending Activation;
- Deactivation where relevant;
- Operational Availability where relevant;
- absence of invented registration lifecycle states.

## 6.4 Registration Information

Verify alignment between HJ-104, HJ-004, HJ-105 and HJ-106.

Check:

- mandatory inputs;
- optional inputs;
- conditional inputs;
- derived information;
- transient declarations;
- field ownership;
- persistence;
- editability;
- post-registration lifecycle.

## 6.5 Address Trust Boundary

Verify that the artefact chain consistently establishes:

- Address Domain authority;
- approved Address Resolution reference;
- authoritative re-fetch during registration;
- Canonical Address Identifier;
- immutable Business Address Snapshot;
- regulatory-authority derivation;
- prohibition on caller-authored Address-owned values.

## 6.6 Idempotency

Verify that all relevant artefacts agree that:

- `RegisterVendor` is not naturally idempotent;
- explicit idempotency identity or equivalent uniqueness safeguard is required;
- Registration Session is outside the idempotency boundary;
- semantically identical replay returns the original successful outcome;
- replay creates no duplicate business effects;
- same identity with semantically different registration information produces a controlled idempotency conflict;
- conflict processing creates or changes no Vendor state and produces no downstream effects;
- concrete transport representation may remain a technical convention.

## 6.7 Domain and Integration Events

Verify that:

- Domain Event and Integration Event are distinct;
- the Domain Event records the internal completed business fact;
- the Integration Event is the published collaboration contract;
- Registration Declarations appear in neither event;
- the Integration Event contains the approved minimum business information;
- downstream Pending Activation can begin without a synchronous Vendor callback.

## 6.8 Reliable Publication

Verify that:

- Vendor persistence and durable publication recording are atomic;
- successful registration is not reported before the business state is safely committed;
- dispatch failure does not undo successful registration;
- publication retry does not rerun `RegisterVendor`;
- replay and idempotency conflict create no duplicate publication work.

## 6.9 Registered Vendor Retrieval

Verify that:

- `RetrieveRegisteredVendor` is a query;
- VendorId is the sole lookup identity;
- persisted Vendor state is the authoritative read source;
- the aggregate is not exposed directly;
- Registered Vendor Details contains the approved result information;
- Vendor Not Found is controlled;
- retrieval is side-effect free;
- Address, Compliance and Identity are not called during retrieval;
- prohibited information is not exposed.

## 6.10 Service Contract Derivation

Review HJ-106 particularly carefully.

Determine whether HJ-106:

- faithfully reflects upstream business behaviour;
- introduces unsupported request fields;
- omits required request fields;
- introduces unsupported response semantics;
- omits required result semantics;
- invents technical behaviour and presents it as business behaviour;
- clearly separates normative business contract from proposed HTTP conventions;
- correctly labels unresolved technical conventions;
- correctly traces every important rule to authoritative sources;
- contains stale references to earlier artefact versions or statuses;
- records resolved gaps as though they remain unresolved.

---

# 7. Severity Classification

Classify findings as follows.

## Critical

A contradiction or defect that:

- changes business meaning;
- changes ownership;
- permits invalid business state;
- breaks an architectural invariant;
- could produce data corruption;
- creates conflicting authoritative behaviour;
- makes implementation unsafe or fundamentally ambiguous;
- invalidates HJ-106 as an implementation baseline.

A Critical finding must be resolved before implementation proceeds.

## Important

A meaningful inconsistency, omission or ambiguity that:

- may lead to differing implementations;
- weakens traceability;
- leaves an important business rule insufficiently specified;
- leaves downstream artefacts out of alignment;
- creates a significant testing or contract gap.

An Important finding should be resolved before the Test Catalogue is treated as final.

## Minor

A defect that:

- does not materially change business behaviour;
- is unlikely to produce divergent implementation;
- concerns editorial consistency, low-risk traceability or documentation hygiene.

## Observation

A useful note that:

- is not currently a defect;
- records a deliberate deferral;
- identifies a future dependency;
- highlights a design consequence worth retaining.

Do not inflate stylistic preferences into Critical or Important findings.

---

# 8. Review Discipline

The review must be independent.

Do not assume that because an artefact is Approved it is correct.

Do not assume that because several artefacts agree they are correct if they all reproduce the same mistake.

Do not preserve a prior decision merely because it appears intentional if the maintained artefacts are inconsistent.

At the same time:

- do not redesign working architecture without cause;
- do not introduce alternative patterns merely because they are common industry practice;
- do not demand implementation detail where the architecture deliberately leaves a technical convention open;
- do not treat future-scope features as missing Epic 1 requirements;
- do not report an explicitly deferred decision as a defect unless the deferral makes the current implementation contract incomplete.

---

# 9. Evidence Requirements

Every finding shall identify:

- finding severity;
- affected artefact or artefacts;
- exact section or model element where practical;
- conflicting or missing rule;
- why it matters;
- authoritative source establishing expected behaviour;
- recommended correction.

Avoid unsupported statements such as:

> This is inconsistent with the architecture.

State exactly what is inconsistent and where.

---

# 10. Root-Cause Classification

For each finding, classify the root cause as one of:

## Source Artefact Defect

The authoritative artefact itself is incomplete, contradictory or ambiguous.

## Derived Artefact Defect

The authoritative source is clear but a downstream artefact has misrepresented, omitted or contradicted it.

## Cross-Artefact Conflict

Two or more artefacts claim authority over the same concern and conflict.

## Traceability Defect

Behaviour may be correct but references point to obsolete, wrong or insufficient source material.

## Documentation Hygiene

The issue concerns status, version, stale narrative, malformed references or presentation rather than business or architectural meaning.

This classification is mandatory.

---

# 11. Change Requests Are Not Review Authority

Change Requests are deliberately excluded from the review baseline.

If current artefacts disagree, report the disagreement.

Do not resolve it by reconstructing which Change Request was intended to win.

The maintained artefacts must stand on their own.

---

# 12. Status and Version Review

Check maintained artefacts for:

- stale Draft references to documents now Approved;
- stale Approved references to documents no longer Approved;
- obsolete version references where a current-authority version is intended;
- historical revision entries that should remain historically accurate.

Distinguish:

- **current-state references**, which must be accurate now; and
- **historical references**, which may correctly describe an earlier state.

---

# 13. Required Output Structure

Produce the review in the following structure.

# Independent Review of HotJoes Architecture Artefact Library

## 1. Executive Summary

Provide:

- overall assessment;
- whether the artefact library is coherent;
- whether HJ-106 is a safe downstream representation of the upstream architecture;
- whether the baseline is ready for Vendor Registration Test Catalogue generation;
- counts of Critical, Important, Minor and Observation findings.

Use one of these ratings:

- Excellent
- Strong
- Generally Sound
- Material Issues
- Not Ready

Do not award a strong rating if unresolved Critical findings exist.

## 2. Critical Findings

For each finding:

### [Finding Title]

**Affected Artefacts:**
...

**Classification:**
Source Artefact Defect / Derived Artefact Defect / Cross-Artefact Conflict / Traceability Defect / Documentation Hygiene

**Issue:**
...

**Why It Matters:**
...

**Expected / Authoritative Behaviour:**
...

**Recommended Correction:**
...

If none:

> None identified.

## 3. Important Findings

Use the same structure.

If none:

> None identified.

## 4. Minor Findings

Use the same structure.

If none:

> None identified.

## 5. Observations and Deliberate Deferrals

Record non-defect items such as:

- deferred wire schemas;
- future Identity integration;
- full Compliance implementation;
- implementation-specific storage or serialization decisions;
- future read-model optimisation;
- other clearly deliberate deferrals.

State whether each deferral is safe for the current Epic 1 baseline.

## 6. Cross-Artefact Consistency Assessment

Assess:

### 6.1 Vision and Principles

### 6.2 Ubiquitous Language and Domain Model

### 6.3 Registration Information and Domain Model

### 6.4 Sequence Behaviour and Domain Rules

### 6.5 ADR Alignment

### 6.6 Service Contract Derivation

### 6.7 Engineering and Testing Standards

### 6.8 AI Governance

## 7. Traceability Assessment

Provide:

| Concern | Authoritative Source | Downstream Artefacts | Status | Notes |
|---|---|---|---|---|

Use statuses:

- Aligned
- Partially Aligned
- Missing
- Conflicting
- Deferred

Include the material concerns listed in this prompt.

## 8. HJ-106 Readiness Assessment

Explicitly answer:

1. Is HJ-106 consistent with HJ-003, HJ-004, HJ-104, HJ-105 and the relevant ADRs?
2. Does it invent any unsupported business behaviour?
3. Does it omit any mandatory upstream business behaviour?
4. Are proposed HTTP conventions clearly separated from the normative business contract?
5. Are all material open questions explicitly identified?
6. Is HJ-106 suitable as the authoritative input to Test Catalogue generation?

Give one outcome:

- **Ready**
- **Ready with Minor Corrections**
- **Not Ready**

## 9. Test Catalogue Readiness

Assess whether the artefact library is ready for generation of the Vendor Registration Test Catalogue.

Explicitly state:

- whether any Critical or Important issue must be resolved first;
- whether HJ-006 provides a sufficient testing taxonomy;
- whether HJ-106 contains enough normative business detail;
- whether unresolved technical conventions should remain blocked or deferred rather than being invented by the Test Catalogue.

## 10. Recommended Actions

Provide only actions justified by findings.

Prioritise:

1. Critical corrections;
2. Important corrections;
3. Minor cleanup;
4. optional future improvements.

Do not add speculative redesign work.

## 11. Final Assessment

Conclude by answering:

> Does the current HotJoes artefact library form a coherent, traceable and implementation-ready architectural baseline for Epic 1 Vendor Registration, and is it ready to proceed to Test Catalogue generation?

---

# 14. Review Standard

A high-quality review shall:

- challenge the artefacts rather than merely summarise them;
- respect the defined authority boundaries;
- distinguish business requirements from technical conventions;
- distinguish current-state defects from deliberate deferrals;
- distinguish source defects from downstream derivation defects;
- verify actual cross-document consistency;
- identify stale references;
- avoid speculative redesign;
- avoid using Change Requests to repair contradictions mentally;
- treat HJ-106 as derived and accountable to its upstream sources;
- provide enough evidence that a human reviewer can reproduce each finding.

The goal is not to prove that the artefacts are good.

The goal is to determine whether they are actually coherent enough to trust.

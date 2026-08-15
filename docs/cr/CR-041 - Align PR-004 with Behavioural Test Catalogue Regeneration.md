# CR-041 - Align PR-004 with Behavioural Test Catalogue Regeneration

## 1. Change Summary

Amend **PR-004 - Generate Test Catalogue from Approved Service Contract** before it is used to regenerate **HJ-107 - Vendor Registration Test Catalogue**.

Retain HJ-106 Part A as the primary normative behavioural test basis while updating PR-004 to:

- recognise HJ-010 and HJ-012 as architectural governance inputs with distinct authority boundaries;
- use the authoritative HJ-006 Test Classification and Test Level mapping;
- preserve the division of responsibility between HJ-107 and HJ-013;
- carry unresolved HJ-010 concern dependencies into HJ-107 without selecting their Approaches;
- regenerate the existing HJ-107 catalogue without resetting its version or Test IDs;
- reconcile stable Test IDs explicitly;
- separate Priority from Dependency Status; and
- use service-contract terminology that does not expose implementation mechanisms unnecessarily.

CR-041 amends PR-004 only. It does not regenerate HJ-107 or amend HJ-010, HJ-012, HJ-013, HJ-106, HJ-006 or any ADR.

## 2. Reason for Change

PR-004 currently contains a strong behavioural derivation method, but it predates:

- HJ-010 v1.0 as the complete architectural concern register;
- HJ-012 v1.0 as the approved application architecture baseline;
- HJ-106 v1.1 and its explicit Current Concern dependencies;
- the initial HJ-013 architecture and implementation test derivation; and
- the authoritative HJ-006 Test Classification mapping.

If used unchanged, PR-004 could:

- treat every attached artefact as having equal normative authority;
- classify HJ-006 test categories as provisional even though they are approved;
- allow HJ-107 to absorb architecture and implementation tests owned by HJ-013;
- convert unresolved architectural choices into generated test requirements;
- reset an existing HJ-107 catalogue to version 0.1;
- regenerate stable Test IDs without reconciliation;
- use Blocked as both a Priority and Dependency Status; and
- expose the Transactional Outbox implementation choice through service-level terminology where only the durable-publication guarantee is relevant.

PR-004 must therefore be aligned before HJ-107 is regenerated.

## 3. Required Changes to PR-004

### 3.1 Attached Artefacts

Add the following to the list of applicable inputs:

- **HJ-010 - Current Application Architectural Concerns**;
- **HJ-012 - Established Application Architecture Patterns**; and
- the existing **HJ-107 - Vendor Registration Test Catalogue** when PR-004 is being used for regeneration.

HJ-013 may be supplied for subsequent cross-catalogue reconciliation, but it is not a normative source of Vendor Registration behaviour.

Do not require every possible artefact to be attached where the authoritative HJ-106 traceability already identifies the required source and that source is available. Missing required authority shall be reported rather than inferred.

### 3.2 Authority Hierarchy

Replace the statement that attached approved or accepted artefacts collectively form a single undifferentiated source of truth.

Define the authority hierarchy as follows:

| Artefact | Authority in PR-004 |
|---|---|
| **HJ-106 Part A** | Primary normative basis for HJ-107 behavioural test derivation. |
| **Authoritative business/domain sources cited by HJ-106** | Confirm terminology, business rules, invariants, ownership and traceability already represented in HJ-106. They shall not silently expand the service contract. |
| **HJ-106 Part B** | Non-normative proposed technical representation; may produce proposed tests only. |
| **HJ-010** | Identifies current unresolved architectural concerns and dependencies that generation must preserve rather than resolve. |
| **HJ-012** | Identifies approved implementation architecture. It may constrain interpretation but shall not introduce service behaviour absent from HJ-106 Part A. |
| **HJ-006** | Authoritative testing taxonomy and test-design standard. |
| **HJ-005 and HJ-007** | Supporting implementation and enforcement standards where relevant to classification or feasibility; not sources of new Vendor Registration behaviour. |
| **Existing HJ-107** | Regeneration baseline for Test ID preservation, change reconciliation and retained catalogue content. |
| **HJ-013** | Separate architecture and implementation test catalogue used for overlap review after derivation; not a normative source for HJ-107 behaviour. |

Where authoritative artefacts disagree, record the conflict and do not choose a resolution.

### 3.3 HJ-107 and HJ-013 Responsibility Boundary

Add a rule equivalent to:

> HJ-107 owns behavioural test obligations for RegisterVendor and RetrieveRegisteredVendor derived from HJ-106 Part A and its authoritative sources. HJ-013 owns complementary architecture, dependency, persistence-mechanism, transaction-mechanism, reliable-publication-mechanism and runtime verification. HJ-107 shall not become an omnibus architecture and implementation test register.

Clarify that:

- HJ-107 may verify observable persistence, atomicity, idempotency and publication outcomes required by HJ-106;
- HJ-013 verifies the implementation mechanisms that preserve those outcomes, including dependency enforcement, real mapping detail, transaction failure injection, outbox persistence, relay recovery and broker mechanics;
- one higher-level guarantee may require distinct obligations in both catalogues at different boundaries; and
- an identical obligation shall not be duplicated merely to create coverage in both catalogues.

When potential overlap is identified, PR-004 shall mark the HJ-107 obligation's behavioural boundary and leave the complementary architecture/implementation obligation to HJ-013.

### 3.4 HJ-006 Classification Mapping

Remove wording that treats Domain, Application, Persistence Integration, Integration Contract and API Contract classifications as provisional.

Use the approved HJ-006 mapping:

| Test Classification | Test Level |
|---|---|
| **Domain** | Unit |
| **Application** | Unit |
| **Persistence Integration** | Integration |
| **Integration Contract** | Integration |
| **API Contract** | API Integration |
| **Contract Review** | Non-executable review |

End-to-End remains an executable Test Level and shall be used only where a complete workflow cannot be verified adequately at a lower boundary.

Require every executable obligation to identify exactly one approved Test Classification and its mapped Test Level. Do not introduce an Architecture Test classification or any additional HJ-006 category.

### 3.5 Unresolved Architectural Concerns

Require PR-004 to use the Current Concern references in HJ-106 v1.1 and HJ-010.

At minimum, generation shall preserve the unresolved status of:

| Concern(s) | Unresolved Area |
|---|---|
| CON-009 and CON-010 | Address Resolution reference contract and Address failure taxonomy/resilience treatment. |
| CON-013 to CON-016 | Idempotency identity/equivalence, concurrency, outcome persistence and transaction boundary. |
| CON-018 to CON-021 | Outbox relay, Domain-to-Integration Event translation, Integration Event schema and broker delivery semantics. |
| CON-024 to CON-026 | Technical API contract, business-failure transport mapping and validation allocation. |
| CON-028 | PostgreSQL mapping and constraints. |

For every affected HJ-107 obligation:

- derive the behavioural guarantee that is already normative;
- identify the applicable `CON-xxx` dependency;
- mark the obligation Ready, Partially Blocked, Blocked or otherwise using the controlled Dependency Status values defined by PR-004;
- state what can and cannot be tested before resolution; and
- do not select an Approach, schema, format, mapping, persistence mechanism, relay, broker policy or implementation framework.

Identity authentication, authorisation and caller-to-Vendor association remain outside Epic 1. They shall be recorded as scope exclusions unless an approved source makes them a dependency of an in-scope obligation.

### 3.6 Reliable Publication Boundary and Terminology

Replace **publication or outbox record** with **durable publication record** where PR-004 describes HJ-106 service behaviour.

Use the following boundary:

- HJ-107 verifies that successful registration creates the required durable publication obligation, failure does not create partial business outcomes, retry does not repeat registration and the required externally observable publication outcome is preserved.
- HJ-013 verifies Transactional Outbox implementation, persistence atomicity at the concrete database boundary, failure injection, relay restart/recovery and broker-delivery mechanics.

PR-004 may reference CON-017 and HJ-012 to explain that Transactional Outbox is approved architecture, but it shall not make outbox representation part of a public request, response or Integration Event contract.

### 3.7 Regeneration Mode

Replace the hard-coded instruction to generate:

```text
Version: 0.1
Status: Draft
```

with instructions that distinguish initial generation from regeneration.

For regeneration:

- use the existing HJ-107 as the document baseline;
- preserve Document ID, title and standard filename;
- increment the internal document version according to the controlled change being applied;
- set Status according to the applicable document-review decision rather than resetting it automatically;
- update Last Updated and Revision History;
- retain unaffected catalogue content; and
- record the source baselines used, including HJ-106 v1.1.

PR-004 shall not append a version or status suffix to the filename.

### 3.8 Stable Test ID Reconciliation

Add mandatory Test ID reconciliation rules:

1. Preserve an existing Test ID when its behavioural obligation remains semantically unchanged.
2. Amend the existing entry under the same Test ID when its traceability, classification, dependency or wording changes without changing its essential obligation.
3. Create a new Test ID only for a genuinely new and independently testable obligation.
4. Do not reuse a retired Test ID for another obligation.
5. Record removed, merged, split, superseded or newly added Test IDs explicitly.
6. Produce a regeneration reconciliation table containing:

| Test ID | Previous Treatment | Regenerated Treatment | Reason |
|---|---|---|---|

The reconciliation shall make omissions visible and prevent Test ID churn caused solely by regeneration.

### 3.9 Priority and Dependency Status

Keep Priority and Dependency Status as separate catalogue fields.

Use Priority values:

- **Critical**;
- **Important**;
- **Standard**; and
- **Proposed** for tests derived solely from HJ-106 Part B.

Do not use **Blocked** as a Priority.

Define controlled Dependency Status values consistently with the existing HJ-107 baseline. At minimum support:

- **Ready**;
- **Partially Blocked**; and
- **Blocked**.

If **Deferred** is retained, define it distinctly from Blocked and use it only where an approved scope or sequencing decision defers the obligation rather than where required information is missing.

### 3.10 Blocked and Deferred Test Areas

Update the required Blocked and Deferred Tests table so that each row includes:

- Test Area;
- affected Test IDs;
- missing contract or decision;
- Current Concern ID(s);
- authoritative owner or source required;
- what can already be tested;
- what remains blocked; and
- implementation/readiness impact.

Do not describe an in-scope architectural decision merely as an unspecified implementation convention where HJ-010 records it as a Current Architectural Concern.

### 3.11 Required Output Structure

Retain the existing HJ-107 catalogue structure, subject to these changes:

- document-control values shall be derived from regeneration mode rather than hard-coded;
- Catalogue Conventions shall contain the authoritative HJ-006 mapping;
- the Catalogue shall retain separate Priority and Dependency Status columns;
- Blocked and Deferred Tests shall include Current Concern references;
- Requirement-to-Test Traceability shall continue to cover every normative HJ-106 Part A element;
- Completeness Analysis shall distinguish HJ-107 behavioural coverage from HJ-013 architecture/implementation coverage; and
- add a **Regeneration Reconciliation** section containing the stable Test ID reconciliation required by §3.8 of this CR.

### 3.12 Review Checklist

Add checklist confirmations that the regenerated HJ-107:

- uses HJ-106 v1.1 Part A as its primary normative behavioural basis;
- uses the approved HJ-006 classification mapping;
- preserves stable Test IDs or records their controlled reconciliation;
- keeps Priority distinct from Dependency Status;
- identifies applicable HJ-010 Current Concern dependencies;
- does not select an unresolved architectural Approach;
- does not duplicate HJ-013 architecture or implementation obligations;
- treats HJ-012 as approved architecture rather than a source of new service behaviour;
- treats Identity as outside Epic 1 unless an approved source establishes an in-scope dependency;
- keeps HJ-106 Part B tests explicitly Proposed and non-normative; and
- contains no executable test or implementation code.

## 4. Required Preservation of PR-004

Retain the existing rules that require PR-004 to:

- derive complete behavioural coverage for RegisterVendor and RetrieveRegisteredVendor;
- use HJ-106 Part A as the primary test basis;
- cover every normative HJ-106 Part A requirement or classify why coverage is not currently possible;
- preserve bounded-context ownership and ubiquitous language;
- cover HJ-104 field and validation rules represented by HJ-106;
- cover aggregate invariants, lifecycle outcomes and prohibited effects;
- cover Address collaboration without inventing the missing Address contract;
- cover Domain Event and Integration Event behavioural separation;
- cover idempotent replay, conflict and concurrency outcomes;
- cover atomic durable-publication guarantees at the behavioural boundary;
- cover Registered Vendor retrieval and its side-effect exclusions;
- keep HJ-106 Part B proposed tests separate and non-normative;
- record ambiguity, conflict, missing information and blocked dependencies;
- assign tests to the lowest responsible boundary;
- produce Markdown rather than executable test code; and
- provide complete requirement-to-test traceability and completeness analysis.

## 5. Explicit Non-Changes

CR-041 does not:

- change HJ-106's service behaviour;
- change HJ-006's testing taxonomy;
- make HJ-010 or HJ-012 a source of new Vendor Registration business behaviour;
- make HJ-013 normative for HJ-107 derivation;
- move all persistence or publication behaviour out of HJ-107;
- prohibit complementary obligations across HJ-107 and HJ-013 where they verify different boundaries;
- approve HJ-106 Part B;
- resolve any Current Architectural Concern;
- define executable test implementation;
- select a testing framework or infrastructure technology;
- regenerate HJ-107; or
- amend any artefact other than PR-004.

## 6. Impacted Artefacts

| Artefact | Impact |
|---|---|
| **PR-004 - Generate Test Catalogue from Approved Service Contract** | Amend authority, classification, regeneration, Test ID, dependency and cross-catalogue rules. |
| **HJ-107 - Vendor Registration Test Catalogue** | No change under this CR; becomes the downstream target of the revised prompt. |
| **HJ-010 - Current Application Architectural Concerns** | No change; referenced for unresolved architectural dependencies. |
| **HJ-012 - Established Application Architecture Patterns** | No change; referenced for approved architecture and responsibility boundaries. |
| **HJ-013 - Architecture and Implementation Test Catalogue** | No change; remains separate and is reconciled after HJ-107 regeneration. |
| **HJ-106 - Vendor Registration Service Contract** | No change; v1.1 Part A remains the primary normative test basis. |

## 7. Acceptance Criteria

CR-041 is satisfied when PR-004:

1. identifies HJ-106 Part A as the primary normative behavioural test basis.
2. defines distinct authority roles for HJ-010, HJ-012, HJ-013 and the existing HJ-107.
3. uses the approved HJ-006 Test Classification and Test Level mapping without provisional classifications.
4. defines the HJ-107/HJ-013 ownership boundary and permits only complementary, non-duplicate obligations.
5. carries applicable `CON-xxx` dependencies into blocked or partially blocked HJ-107 entries.
6. prevents generators from selecting unresolved Approaches or technical contracts.
7. uses **durable publication record** for the service-level guarantee.
8. supports regeneration without resetting HJ-107 to version 0.1 or renaming its file.
9. preserves stable Test IDs and requires an explicit regeneration reconciliation table.
10. keeps Priority and Dependency Status separate and does not use Blocked as a Priority.
11. treats Identity as outside Epic 1 unless an approved source establishes an in-scope dependency.
12. keeps HJ-106 Part B tests Proposed and non-normative.
13. retains complete coverage and traceability requirements for RegisterVendor and RetrieveRegisteredVendor.
14. contains no instruction to generate executable tests, implementation code or CI configuration.

## 8. Follow-up Work

After CR-041 is applied:

1. run the revised PR-004 using HJ-106 v1.1 and its authoritative sources;
2. regenerate HJ-107 while preserving and reconciling stable Test IDs;
3. review the regenerated catalogue for source fidelity, omissions and unresolved dependencies;
4. reconcile HJ-107 with the initial HJ-013 draft to remove duplication and identify complementary coverage;
5. classify remaining gaps as behavioural-source gaps, Current Concern dependencies, HJ-013 obligations or potential challenges to Approved architecture; and
6. proceed to constrained test and implementation generation only where the governing sources are sufficiently explicit.

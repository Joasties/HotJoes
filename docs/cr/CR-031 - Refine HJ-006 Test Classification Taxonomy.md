# CR-031 – Refine HJ-006 Test Classification Taxonomy

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-031 |
| **Title** | Refine HJ-006 Test Classification Taxonomy |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Medium |
| **Affected Document** | HJ-006 – Testing Strategy and Standards |
| **Downstream Relevance** | HJ-107 – Vendor Registration Test Catalogue; future test catalogues derived from approved service contracts |

---

# 1. Background

HJ-006 – *Testing Strategy and Standards* currently defines the project’s testing model primarily in terms of broad execution levels:

- Unit Tests;
- Integration Tests;
- API Integration Tests; and
- End-to-End Tests.

This remains a valid and useful high-level testing model.

However, the Vendor Registration Test Catalogue work has introduced more precise test classifications to describe where a test obligation belongs and what boundary it verifies:

- Domain;
- Application;
- Persistence Integration;
- Integration Contract;
- API Contract; and
- Contract Review.

These classifications are useful because they allow a test catalogue to distinguish substantially different obligations that would otherwise all be described only as Unit, Integration or API Integration tests.

HJ-006 does not currently define how these refined classifications relate to its existing testing levels.

If HJ-006 were promoted to Approved without resolving this distinction, the project would formally approve a testing standard whose taxonomy is less precise than the classifications already required by the Vendor Registration Test Catalogue.

This Change Request resolves that ambiguity before HJ-006 is promoted to its first Human Reviewed and Approved baseline.

The intent is not to replace the existing Testing Pyramid or broad test levels. It is to define a consistent two-level taxonomy in which:

1. **Test Level** describes the execution boundary and infrastructure scope of an executable test; and
2. **Test Classification** describes the architectural or contract concern being verified.

---

# 2. Objectives

This Change Request shall:

- preserve the existing HJ-006 Testing Pyramid;
- preserve Unit, Integration, API Integration and End-to-End as the project’s broad executable test levels;
- introduce a formal test-classification taxonomy for use by detailed Test Catalogues;
- define how Domain, Application, Persistence Integration, Integration Contract and API Contract map to the existing test levels;
- define Contract Review as a non-executable verification activity rather than a test level;
- remove ambiguity between test execution level and test purpose;
- provide an authoritative taxonomy that HJ-107 and future Test Catalogues can use;
- update HJ-006 revision history and version while retaining Draft status pending CR-030 approval; and
- avoid changes to unrelated testing standards.

---

# 3. Scope

This Change Request applies only to:

- HJ-006 – Testing Strategy and Standards.

It introduces clarification of:

- executable Test Levels;
- detailed Test Classifications;
- the relationship between those concepts; and
- Contract Review.

It does not require direct amendment of HJ-107 as part of this Change Request.

HJ-107 shall subsequently be validated or regenerated against the revised HJ-006 taxonomy through the normal Test Catalogue workflow.

---

# 4. Required Version Change

HJ-006 is currently Version 1.0 Draft.

Apply this Change Request as:

```text
Version: 1.0 → 1.1
Status: Draft → Draft
Last Updated: <CR-031 application date>
```

HJ-006 shall remain **Draft** after CR-031.

CR-030 shall subsequently perform the separate Human Review and Approval promotion.

---

# 5. Revision History

Append the following revision-history entry:

```text
1.1 | <CR-031 application date> | Applied CR-031 to refine the testing taxonomy. Preserved the existing Unit, Integration, API Integration and End-to-End test levels; introduced detailed Domain, Application, Persistence Integration, Integration Contract and API Contract classifications; and defined Contract Review as a non-executable verification activity.
```

Do not alter existing revision-history entries.

---

# 6. Preserve the Existing Testing Pyramid

The current Testing Pyramid shall remain authoritative.

HJ-006 shall continue to define the broad executable levels as:

1. Unit Tests;
2. Integration Tests;
3. API Integration Tests; and
4. End-to-End Tests.

The existing philosophy that the test suite should contain many fast tests and comparatively few slow tests shall remain unchanged.

The refined classifications introduced by this Change Request do **not** replace the Testing Pyramid.

They provide additional architectural precision within it.

---

# 7. Introduce Test Level and Test Classification

Add a new subsection immediately after the existing **Testing Pyramid** section and before **Unit Testing Standards**.

Recommended heading:

```markdown
## 2.1 Test Levels and Test Classifications
```

The new section shall establish the following distinction.

## 7.1 Test Level

A **Test Level** describes the execution boundary of an executable test, including how much of the system participates and whether real infrastructure or public interfaces are involved.

The approved executable Test Levels are:

- Unit;
- Integration;
- API Integration; and
- End-to-End.

## 7.2 Test Classification

A **Test Classification** describes the architectural responsibility, contract or system boundary that a test obligation verifies.

Detailed Test Catalogues may therefore use more precise classifications while still mapping every executable test to one of the approved Test Levels.

The approved detailed classifications are:

- Domain;
- Application;
- Persistence Integration;
- Integration Contract;
- API Contract.

**Contract Review** is also an approved catalogue classification, but it represents a non-executable verification activity rather than an executable Test Level.

---

# 8. Required Classification Mapping

HJ-006 shall define the following authoritative mapping.

| Test Classification | Test Level | Purpose |
|---|---|---|
| **Domain** | Unit | Verifies Domain behaviour, invariants, state transitions, Value Objects, Entities, Aggregates and Domain-level business rules in isolation from infrastructure. |
| **Application** | Unit | Verifies application orchestration and application-level behaviour with external dependencies replaced by appropriate test doubles. |
| **Persistence Integration** | Integration | Verifies persistence behaviour against real or production-equivalent persistence infrastructure, including mappings, constraints, transactions, concurrency and atomic persistence behaviour. |
| **Integration Contract** | Integration | Verifies integration-boundary contracts and collaboration behaviour between components or bounded contexts without requiring a complete End-to-End business workflow. |
| **API Contract** | API Integration | Verifies the externally observable API contract, including request/response behaviour, validation, controlled failures, routing and transport mappings where those mappings are part of the tested technical contract. |
| **Contract Review** | Non-executable review | Verifies that an inferred or proposed contract is complete, internally consistent, traceable and does not invent behaviour unsupported by its authoritative source artefacts. |

---

# 9. Domain Classification

Define **Domain** tests as executable Unit Tests focused on Domain behaviour.

Domain tests should cover, where applicable:

- Aggregate creation;
- business invariants;
- Entity behaviour;
- Value Object behaviour;
- valid state transitions;
- prohibited state transitions;
- Domain validation;
- Domain Events as completed business facts;
- absence of prohibited Domain side effects.

Domain tests shall not require:

- databases;
- HTTP;
- message brokers;
- external APIs;
- file systems;
- cloud infrastructure.

The existing HJ-006 Unit Testing rules continue to apply.

---

# 10. Application Classification

Define **Application** tests as executable Unit Tests focused on application orchestration.

Application tests may verify:

- command or query orchestration;
- coordination of Domain objects;
- invocation of required abstractions;
- trust-boundary handling;
- mapping between application inputs and Domain inputs;
- controlled application outcomes;
- prevention of prohibited calls or side effects.

External infrastructure shall be replaced with appropriate test doubles when testing at this classification.

Application tests shall test observable application behaviour rather than incidental implementation details.

---

# 11. Persistence Integration Classification

Define **Persistence Integration** tests as Integration Tests using real or production-equivalent persistence infrastructure wherever practical.

They may verify:

- persistence mappings;
- database constraints;
- unique constraints;
- transaction behaviour;
- atomic persistence;
- concurrency behaviour;
- repository behaviour;
- rollback behaviour;
- durable publication recording where persistence and publication work form one atomic boundary.

Persistence Integration tests shall not be reduced to mocks of the persistence framework where doing so would fail to test the real persistence behaviour.

This classification refines the existing HJ-006 Database Testing guidance rather than replacing it.

---

# 12. Integration Contract Classification

Define **Integration Contract** tests as Integration Tests focused on a published or consumed integration boundary.

They may verify:

- Integration Event contracts;
- required event information;
- deliberately excluded information;
- event serialization where a concrete serialization contract is approved;
- producer/consumer compatibility;
- downstream initiation from a published contract;
- absence of prohibited synchronous callbacks;
- reliable collaboration behaviour where multiple technical components participate.

An Integration Contract test does not require a complete End-to-End user workflow.

It verifies the integration contract and collaboration boundary.

Where a concrete wire schema has deliberately been deferred by the architecture, executable schema assertions shall also remain deferred rather than being invented by the test.

---

# 13. API Contract Classification

Define **API Contract** tests as API Integration Tests.

They shall exercise the public application/API boundary using the real API pipeline or the nearest production-equivalent test host appropriate to the solution.

They may verify:

- endpoint availability;
- request mapping;
- response mapping;
- validation behaviour;
- controlled business failures;
- HTTP status mapping;
- headers;
- serialization;
- route behaviour;
- client-safe error responses.

Only transport behaviour established by an approved technical contract shall be treated as normative.

A Test Catalogue shall not convert a proposed or unresolved transport convention into an approved requirement.

---

# 14. Contract Review Classification

Define **Contract Review** as a non-executable verification activity.

Contract Review is not:

- a Unit Test;
- an Integration Test;
- an API Integration Test;
- an End-to-End Test; or
- an additional level in the Testing Pyramid.

Its purpose is to verify source-derived contracts and Test Catalogues before implementation.

Contract Review may verify that:

- every normative rule is traceable to an authoritative artefact;
- no business behaviour has been invented;
- required positive, negative and boundary behaviour has been represented;
- known exclusions remain excluded;
- unresolved technical conventions are not accidentally promoted into business requirements;
- Domain and Integration Event semantics remain distinct;
- security- or privacy-sensitive information is not introduced into contracts without authority;
- Test Catalogue classifications correctly map to the HJ-006 taxonomy.

Contract Review obligations may appear in a Test Catalogue because they are verification obligations, but they shall be clearly distinguished from executable automated tests.

---

# 15. End-to-End Classification

End-to-End remains an executable Test Level rather than one of the refined architectural classifications above.

End-to-End tests verify complete business workflows across the required participating components.

They shall remain intentionally few because they are slower, more expensive and more difficult to diagnose than lower-level tests.

A detailed Test Catalogue may classify a requirement directly as **End-to-End** where the behaviour can only be meaningfully proven across the complete workflow.

No new separate refined label is required for End-to-End behaviour.

---

# 16. Classification Selection Rule

Add the following governing rule:

> Every executable test obligation in a detailed HotJoes Test Catalogue shall identify the most specific applicable Test Classification and shall map to exactly one approved executable Test Level.

Where one business rule requires verification at more than one boundary, the Test Catalogue shall define separate test obligations rather than assigning one test simultaneously to multiple levels.

Examples:

- a Vendor Aggregate invariant may require a Domain test;
- persistence of that invariant may separately require a Persistence Integration test;
- exposure of its controlled failure through HTTP may separately require an API Contract test.

This preserves one-behaviour-per-test while allowing the same business rule to be verified at the appropriate architectural boundaries.

---

# 17. Relationship to Existing HJ-006 Sections

The existing HJ-006 sections shall remain valid.

The refined taxonomy shall be interpreted as follows:

| Existing HJ-006 Area | Refined Interpretation |
|---|---|
| Unit Testing Standards | Governs Domain and Application classifications |
| Integration Testing | Governs Persistence Integration and Integration Contract classifications |
| Database Testing | Detailed guidance within Persistence Integration |
| API Integration Tests | Governs API Contract classification |
| End-to-End Tests | Remains the End-to-End Test Level |
| What Should Be Tested? | Applies across all executable classifications |
| Mocking Strategy | Applies according to Test Level and boundary |
| Test Data | Applies across executable classifications |
| CI / automation guidance | Applies according to execution cost and purpose |

No existing section shall be removed merely because the refined taxonomy has been introduced.

---

# 18. Relationship to HJ-107

The refined labels currently used by the Vendor Registration Test Catalogue shall be evaluated against the approved mapping introduced by this Change Request.

The intended mapping is:

| HJ-107 Classification | HJ-006 Meaning |
|---|---|
| Domain | Unit-level Domain test |
| Application | Unit-level Application test |
| Persistence Integration | Integration test |
| Integration Contract | Integration test |
| API Contract | API Integration test |
| Contract Review | Non-executable verification |

HJ-107 shall not become the authority for this taxonomy.

HJ-006 is the authoritative testing standard.

HJ-107 and future Test Catalogues shall conform to HJ-006.

If an existing HJ-107 obligation cannot be mapped cleanly using this taxonomy, the Test Catalogue shall be corrected or regenerated rather than expanding HJ-006 ad hoc.

---

# 19. Architectural Constraints

CR-031 shall not:

- replace the Testing Pyramid;
- redefine Unit Testing as integration testing;
- make Domain and Application tests separate Testing Pyramid levels;
- make Persistence Integration a new top-level Testing Pyramid layer;
- make Integration Contract a new top-level Testing Pyramid layer;
- make API Contract independent of API Integration testing;
- classify Contract Review as an executable automated test;
- mandate specific testing frameworks;
- mandate specific mocking libraries;
- mandate Testcontainers for every integration test;
- prescribe implementation project structure;
- change business behaviour;
- introduce Vendor-specific business rules into HJ-006;
- change HJ-005, HJ-007, HJ-008 or HJ-009.

---

# 20. Codex Execution Instructions

Codex shall update the existing repository copy of HJ-006 directly.

It shall:

1. confirm the current document is HJ-006 Version 1.0 Draft;
2. change the version to 1.1;
3. retain Status = Draft;
4. update Last Updated;
5. append the CR-031 revision-history entry;
6. add the Test Levels and Test Classifications section after Testing Pyramid;
7. add or integrate the definitions required by Sections 8–17 of this Change Request;
8. preserve all existing HJ-006 testing guidance unless wording must change to remove a direct taxonomy contradiction;
9. avoid unrelated style or content changes;
10. verify that all existing broad test levels remain present;
11. verify that every refined executable classification maps to one existing Test Level;
12. verify that Contract Review is explicitly non-executable;
13. verify that no Vendor-specific business rule has been introduced into HJ-006.

Codex shall update the document in the repository rather than outputting a newly generated replacement artefact.

---

# 21. Acceptance Criteria

CR-031 is complete when all of the following are true:

- [ ] HJ-006 is Version 1.1.
- [ ] HJ-006 remains Draft.
- [ ] Last Updated reflects the CR-031 application date.
- [ ] Revision History records CR-031.
- [ ] Unit remains an approved Test Level.
- [ ] Integration remains an approved Test Level.
- [ ] API Integration remains an approved Test Level.
- [ ] End-to-End remains an approved Test Level.
- [ ] Domain is formally defined and mapped to Unit.
- [ ] Application is formally defined and mapped to Unit.
- [ ] Persistence Integration is formally defined and mapped to Integration.
- [ ] Integration Contract is formally defined and mapped to Integration.
- [ ] API Contract is formally defined and mapped to API Integration.
- [ ] Contract Review is formally defined as non-executable.
- [ ] Contract Review is explicitly not a Testing Pyramid level.
- [ ] End-to-End remains intentionally limited and is not replaced by a refined classification.
- [ ] Detailed Test Catalogues are required to use the most specific applicable classification.
- [ ] Executable Test Catalogue entries map to exactly one executable Test Level.
- [ ] Multiple architectural-boundary checks for one business rule are represented as separate test obligations where required.
- [ ] Existing testing philosophy remains unchanged.
- [ ] Existing mocking, test-data and infrastructure guidance remains unchanged except where necessary to clarify taxonomy.
- [ ] HJ-107 is downstream of HJ-006 and does not become the taxonomy authority.
- [ ] No unrelated architectural or business behaviour has changed.

---

# 22. Expected Outcome

Following CR-031:

- HJ-006 retains its simple Testing Pyramid and broad execution levels;
- detailed Test Catalogues gain a precise and approved architectural classification vocabulary;
- the Domain and Application distinction is available without inventing new Testing Pyramid levels;
- persistence and integration-contract tests are clearly distinguishable while remaining Integration Tests;
- API Contract tests are clearly identifiable within API Integration testing;
- Contract Review is recognised as a legitimate verification obligation without being misrepresented as executable testing;
- HJ-107’s refined labels can be assessed against an authoritative testing standard rather than remaining provisional conventions; and
- HJ-006 is ready for subsequent Human Review and Approval under CR-030.

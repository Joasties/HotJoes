# HJ006 - Testing Strategy and Standards

| Field | Value |
|-------|-------|
| **Document ID** | HJ-006 |
| **Document Title** | Testing Strategy and Standards |
| **Version** | 2.0 |
| **Status** | Approved |
| **Classification** | Testing |
| **Owner** | Project Architecture |
| **Last Updated** | 9 August 2026 |

## Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 21 July 2026 | Applied the standard HotJoes document metadata, revision history, related documents and numbered heading structure. Testing Strategy and Standards first draft. |
| 1.1 | 9 August 2026 | Applied CR-031 to refine the testing taxonomy. Preserved the existing Unit, Integration, API Integration and End-to-End test levels; introduced detailed Domain, Application, Persistence Integration, Integration Contract and API Contract classifications; and defined Contract Review as a non-executable verification activity. |
| 2.0 | 9 August 2026 | Completed human review and promoted Testing Strategy and Standards from Draft to Approved. Existing testing strategy and standards were retained unchanged. Related-document status references were reconciled with the approved repository baseline. |

## Related Documents

| Document ID | Title | Status |
|------------|-------|--------|
| HJ-001 | Project Vision | Approved |
| HJ-002 | Architectural Principles | Approved |
| HJ-003 | Ubiquitous Language Guide | Approved |
| HJ-004 | Vendor Domain Models | Approved |
| HJ-005 | Coding Standards | Approved |
| HJ-007 | Enforcement Strategy | Approved |
| HJ-008 | AI Roles and Responsibilities | Approved |

1. Philosophy
Testing exists to give engineers confidence to change software safely.
The objective of the test suite is not to prove software is correct.
It is to quickly detect when software is no longer behaving as expected.
Good tests should therefore be:
Fast
Deterministic
Independent
Easy to understand
Cheap to maintain
Focused on observable behaviour rather than implementation

2. Testing Pyramid
HotJoes follows a traditional testing pyramid.

             End-to-End
          ----------------
         Integration Tests
      ------------------------
            Unit Tests

Approximate distribution:

The exact ratio is less important than the principle:
Thousands of fast tests.
Very few slow tests.

## 2.1 Test Levels and Test Classifications

The Testing Pyramid remains authoritative. The refined classifications in this section do not replace its broad executable Test Levels. They provide additional architectural precision for detailed Test Catalogues.

### 2.1.1 Test Level

A **Test Level** describes the execution boundary of an executable test, including how much of the system participates and whether real infrastructure or public interfaces are involved.

The approved executable Test Levels are:

- Unit;
- Integration;
- API Integration; and
- End-to-End.

### 2.1.2 Test Classification

A **Test Classification** describes the architectural responsibility, contract or system boundary that a test obligation verifies.

Detailed Test Catalogues may use precise classifications while mapping every executable test to one approved Test Level.

The approved detailed classifications are:

- Domain;
- Application;
- Persistence Integration;
- Integration Contract; and
- API Contract.

**Contract Review** is also an approved catalogue classification, but it represents a non-executable verification activity rather than an executable Test Level.

### 2.1.3 Classification Mapping

| Test Classification | Test Level | Purpose |
|---|---|---|
| **Domain** | Unit | Verifies Domain behaviour, invariants, state transitions, Value Objects, Entities, Aggregates and Domain-level business rules in isolation from infrastructure. |
| **Application** | Unit | Verifies application orchestration and application-level behaviour with external dependencies replaced by appropriate test doubles. |
| **Persistence Integration** | Integration | Verifies persistence behaviour against real or production-equivalent persistence infrastructure, including mappings, constraints, transactions, concurrency and atomic persistence behaviour. |
| **Integration Contract** | Integration | Verifies integration-boundary contracts and collaboration behaviour between components or bounded contexts without requiring a complete End-to-End business workflow. |
| **API Contract** | API Integration | Verifies the externally observable API contract, including request/response behaviour, validation, controlled failures, routing and transport mappings where those mappings are part of the tested technical contract. |
| **Contract Review** | Non-executable review | Verifies that an inferred or proposed contract is complete, internally consistent, traceable and does not invent behaviour unsupported by its authoritative source artefacts. |

### 2.1.4 Domain Classification

Domain tests are executable Unit Tests focused on Domain behaviour.

Domain tests should cover, where applicable:

- Aggregate creation;
- business invariants;
- Entity behaviour;
- Value Object behaviour;
- valid state transitions;
- prohibited state transitions;
- Domain validation;
- Domain Events as completed business facts; and
- absence of prohibited Domain side effects.

Domain tests shall not require:

- databases;
- HTTP;
- message brokers;
- external APIs;
- file systems; or
- cloud infrastructure.

The existing Unit Testing rules continue to apply.

### 2.1.5 Application Classification

Application tests are executable Unit Tests focused on application orchestration.

Application tests may verify:

- command or query orchestration;
- coordination of Domain objects;
- invocation of required abstractions;
- trust-boundary handling;
- mapping between application inputs and Domain inputs;
- controlled application outcomes; and
- prevention of prohibited calls or side effects.

External infrastructure shall be replaced with appropriate test doubles when testing at this classification.

Application tests shall test observable application behaviour rather than incidental implementation details.

### 2.1.6 Persistence Integration Classification

Persistence Integration tests are Integration Tests using real or production-equivalent persistence infrastructure wherever practical.

They may verify:

- persistence mappings;
- database constraints;
- unique constraints;
- transaction behaviour;
- atomic persistence;
- concurrency behaviour;
- repository behaviour;
- rollback behaviour; and
- durable publication recording where persistence and publication work form one atomic boundary.

Persistence Integration tests shall not be reduced to mocks of the persistence framework where doing so would fail to test the real persistence behaviour.

This classification refines the existing Database Testing guidance rather than replacing it.

### 2.1.7 Integration Contract Classification

Integration Contract tests are Integration Tests focused on a published or consumed integration boundary.

They may verify:

- Integration Event contracts;
- required event information;
- deliberately excluded information;
- event serialization where a concrete serialization contract is approved;
- producer/consumer compatibility;
- downstream initiation from a published contract;
- absence of prohibited synchronous callbacks; and
- reliable collaboration behaviour where multiple technical components participate.

An Integration Contract test does not require a complete End-to-End user workflow. It verifies the integration contract and collaboration boundary.

Where a concrete wire schema has deliberately been deferred by the architecture, executable schema assertions shall also remain deferred rather than being invented by the test.

### 2.1.8 API Contract Classification

API Contract tests are API Integration Tests.

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
- route behaviour; and
- client-safe error responses.

Only transport behaviour established by an approved technical contract shall be treated as normative.

A Test Catalogue shall not convert a proposed or unresolved transport convention into an approved requirement.

### 2.1.9 Contract Review Classification

Contract Review is a non-executable verification activity.

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
- security- or privacy-sensitive information is not introduced into contracts without authority; and
- Test Catalogue classifications correctly map to the HJ-006 taxonomy.

Contract Review obligations may appear in a Test Catalogue because they are verification obligations, but they shall be clearly distinguished from executable automated tests.

### 2.1.10 End-to-End Classification

End-to-End remains an executable Test Level rather than one of the refined architectural classifications above.

End-to-End tests verify complete business workflows across the required participating components.

They shall remain intentionally few because they are slower, more expensive and more difficult to diagnose than lower-level tests.

A detailed Test Catalogue may classify a requirement directly as **End-to-End** where the behaviour can only be meaningfully proven across the complete workflow.

No new separate refined label is required for End-to-End behaviour.

### 2.1.11 Classification Selection Rule

> Every executable test obligation in a detailed HotJoes Test Catalogue shall identify the most specific applicable Test Classification and shall map to exactly one approved executable Test Level.

Where one business rule requires verification at more than one boundary, the Test Catalogue shall define separate test obligations rather than assigning one test simultaneously to multiple levels.

Examples:

- a Vendor Aggregate invariant may require a Domain test;
- persistence of that invariant may separately require a Persistence Integration test; and
- exposure of its controlled failure through HTTP may separately require an API Contract test.

This preserves one-behaviour-per-test while allowing the same business rule to be verified at the appropriate architectural boundaries.

### 2.1.12 Relationship to Existing Sections

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

No existing section is removed merely because the refined taxonomy has been introduced.

### 2.1.13 Relationship to HJ-107

The refined labels currently used by the Vendor Registration Test Catalogue shall be evaluated against this authoritative mapping.

| HJ-107 Classification | HJ-006 Meaning |
|---|---|
| Domain | Unit-level Domain test |
| Application | Unit-level Application test |
| Persistence Integration | Integration test |
| Integration Contract | Integration test |
| API Contract | API Integration test |
| Contract Review | Non-executable verification |

HJ-107 does not define this taxonomy. HJ-006 is the authoritative testing standard, and HJ-107 and future Test Catalogues shall conform to it.

If an existing HJ-107 obligation cannot be mapped cleanly using this taxonomy, the Test Catalogue shall be corrected or regenerated rather than expanding HJ-006 ad hoc.

3. Unit Testing Standards
Unit tests verify behaviour of a single unit of code in complete isolation.
A unit test:
does not access a database
does not call HTTP APIs
does not access Azure
does not use the file system
does not depend on current time
does not depend on randomness
Dependencies should be replaced with test doubles where appropriate.
Unit tests should execute in milliseconds.

3.1 Naming
Test names should describe behaviour.
Preferred format

MethodName_State_ExpectedBehaviour

Example

CreateVendor_WithDuplicateEmail_ReturnsFailure()

or

SubmittingInvalidOrderReturnsValidationError()

The test should read almost like English.

3.2 Arrange – Act – Assert
Every unit test follows:

Arrange

Act

Assert

Avoid mixing these sections.

3.3 One Behaviour Per Test
Each test should verify one thing.
Avoid tests that verify multiple unrelated behaviours.
Bad

Creates Vendor
Creates Menu
Creates Address
Sends Email
Publishes Event

Good

Creates Vendor

Another test verifies

Publishes VendorCreated event


3.4 No Test Logic
Tests should not contain complicated algorithms.
If the test contains loops, branching or calculations, question whether it is still readable.

3.5 Deterministic Tests
A test must either:
Always pass
or
Always fail
Given identical code.
Tests must not rely on:
current clock
time zones
random numbers
network latency
external services

4. Integration Testing
Integration tests verify that multiple components work together correctly.
Examples:
Application → Database
Application → Message Broker
Application → External API
Application → Authentication
Integration tests should use real infrastructure wherever practical.
Prefer Docker containers over mocks.
Examples:
SQL Server container
Azurite
Testcontainers
RabbitMQ
SQLite is acceptable where production behaviour remains equivalent.

4.1 Database Testing
Repositories should be tested against a real database.
Avoid mocking EF Core.
Mocking EF often proves only that the mock behaves as configured.
The database should verify:
constraints
indexes
transactions
relationships
migrations

4.2 API Integration Tests
Every public endpoint should have integration tests verifying:
success
validation failure
authentication
authorization
error handling

5. End-to-End Tests
End-to-end tests verify complete business workflows.
Examples
Vendor Registration
↓
Vendor Approval
↓
Menu Creation
↓
Customer Order
↓
Payment
↓
Order Accepted
↓
Order Completed
These tests are intentionally few.
They are slower and more expensive.

6. What Should Be Tested?
Test behaviour.
Not implementation.
Good

Customer receives validation error.

Bad

Validator called Validate() exactly once.

The latter makes refactoring difficult while providing little business value.

7. Mocking Strategy
Mock only true external dependencies.
Examples
✔ Payment gateway
✔ Email provider
✔ SMS provider
✔ Third-party APIs
Avoid mocking:
Domain objects
Value objects
Repositories (where integration tests exist)
EF Core
Business rules

8. Test Data
Test data should clearly communicate intent.
Prefer

Vendor "Joe's Burgers"

Menu "Breakfast"

Instead of

Vendor1

Menu2

Avoid meaningless GUIDs unless testing GUID-specific behaviour.
Builders or factory methods should create common test objects.

9. Assertions
Assert what matters.
Avoid asserting every property unless required.
Bad

Assert.Equal(...)
Assert.Equal(...)
Assert.Equal(...)
Assert.Equal(...)
Assert.Equal(...)
Assert.Equal(...)

Good
Assert only the behaviour being verified.

10. Code Coverage
Code coverage is an indicator.
It is not a goal.
We optimise for confidence rather than percentages.
High-value business logic should have comprehensive coverage.
Simple DTOs, configuration classes and generated code require little or no testing.

11. Performance of Test Suite
The entire unit test suite should execute in seconds.
Integration tests should execute in minutes.
Long-running tests discourage frequent execution and reduce developer feedback.

12. Continuous Integration
Every Pull Request should execute:
Build
Static Analysis
Unit Tests
Integration Tests
Coverage Report
Security Scanning
A Pull Request must not be merged if any required check fails.

13. Regression Tests
Every production bug should result in:
A failing automated test.
A code fix.
The new test remaining permanently in the suite.
This prevents the same defect from reappearing.

14. Anti-Patterns
Avoid:
Sleeping (Thread.Sleep)
Time-dependent assertions
Shared mutable test state
Order-dependent tests
Magic values
Testing private methods directly
Large "kitchen sink" tests
Copy-pasted test setup
Overuse of mocks
Fragile assertions based on implementation details

15. Definition of Done
A feature is complete only when:
Business behaviour is implemented.
Unit tests pass.
Integration tests pass.
Existing tests remain green.
New behaviour is covered by automated tests.
No unnecessary test duplication has been introduced.
CI pipeline passes successfully.

# HJ006 - Testing Strategy and Standards

| Field | Value |
|-------|-------|
| **Document ID** | HJ-006 |
| **Document Title** | Testing Strategy and Standards |
| **Version** | 1.0 |
| **Status** | Draft |
| **Classification** | Testing |
| **Owner** | Project Architecture |
| **Last Updated** | 21 July 2026 |

## Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 21 July 2026 | Applied the standard HotJoes document metadata, revision history, related documents and numbered heading structure. Testing Strategy and Standards first draft. |

## Related Documents

| Document ID | Title | Status |
|------------|-------|--------|
| HJ-001 | Project Vision | Draft |
| HJ-002 | Architectural Principles | Draft |
| HJ-003 | Ubiquitous Language Guide | Draft |
| HJ-004 | Vendor Domain Models | Draft |
| HJ-005 | Coding Standards | Draft |
| HJ-007 | Enforcement Strategy | Draft |
| HJ-008 | AI Roles and Responsibilities | Draft |

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

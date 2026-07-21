# HJ-007 - Enforcement Strategy

| Field | Value |
|-------|-------|
| **Document ID** | HJ-007 |
| **Document Title** | Enforcement Strategy |
| **Version** | 1.0 |
| **Status** | Draft |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 21 July 2026 |

## Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 21 July 2026 | Applied the standard HotJoes document metadata, revision history, related documents and numbered heading structure. Enforcement Strategy first draft. |

## Related Documents

| Document ID | Title | Status |
|------------|-------|--------|
| HJ-001 | Project Vision | Draft |
| HJ-002 | Architectural Principles | Draft |
| HJ-003 | Ubiquitous Language Guide | Draft |
| HJ-004 | Vendor Domain Models | Draft |
| HJ-005 | Coding Standards | Draft |
| HJ-006 | Testing Strategy and Standards | Draft |
| HJ-008 | AI Roles and Responsibilities | Draft |
| HJ-009 | AI Operating Guide | Draft |

1. Purpose
This document defines how engineering standards are enforced throughout the HotJoes project.
The goal is to ensure that:
coding standards are consistently followed
architecture remains clean over time
defects are detected early
contributors receive immediate feedback
quality does not depend upon memory or discipline alone
Where possible, enforcement should be automatic.
Human review should focus on design and business logic rather than formatting or mechanical issues.

2. Enforcement Philosophy
Enforcement should happen at multiple stages.
The earlier an issue is detected, the cheaper it is to fix.


3. IDE Enforcement
What
Developers should receive immediate feedback while writing code.
Problems should ideally be corrected before the project even builds.
How
Visual Studio Code uses:
C# Dev Kit
Roslyn compiler diagnostics
.editorconfig
nullable reference analysis
analyzers
These highlight:
style violations
compiler warnings
nullability issues
unreachable code
unused variables
missing documentation (if enabled)
incorrect formatting
The goal is to minimise surprises during compilation.

4. .editorconfig
What
Defines formatting rules shared by every developer.
Every IDE reads the same configuration.
How
The .editorconfig file lives in the repository root.
Examples include:
indentation
whitespace
line endings
UTF-8 encoding
newline at end of file
naming conventions
language preferences
Developers should not manually format code differently.
Formatting is performed automatically.

5. dotnet format
What
Automatically fixes formatting and style violations.
How
Developers run

dotnet format

before committing.
The tool:
reformats code
fixes many style issues
applies .editorconfig rules
It does not change business logic.

6. Compiler Warnings
What
Warnings indicate potential defects.
Warnings should not accumulate over time.
How
The project should aim for
Zero compiler warnings.
Where practical, warnings should be treated as build failures.
Examples include:
nullable violations
unreachable code
obsolete APIs
unused variables
hidden members

7. Nullable Reference Types
What
Prevent many NullReferenceExceptions before the application runs.
How
Nullable reference types remain enabled.
The compiler analyses code flow.
Developers must explicitly decide whether a value:
can be null
or
cannot be null
rather than relying upon assumptions.

8. Unit Test Enforcement
What
Business behaviour should be verified automatically.
How
Every domain rule should have unit tests.
Unit tests should execute as part of:

dotnet test

before code is committed.

9. Integration Test Enforcement
What
Verify that application components work together correctly.
How
Integration tests validate interactions between:
API
Application Layer
Infrastructure
Database
These execute during CI.

10. Build Verification
What
Code must compile successfully before merging.
How
Continuous Integration performs:

dotnet restore
dotnet build
dotnet test

If any step fails:
the merge fails.
Broken builds are never merged.

11. Pull Request Reviews
What
Automated tools cannot judge architecture.
Human review remains essential.
Review Focus
Reviewers should evaluate:
architecture
naming
readability
business correctness
maintainability
security
performance where appropriate
Reviewers should not spend time correcting formatting already enforced automatically.

12. Architecture Enforcement
What
Prevent incorrect project dependencies.
How
Project references define allowed dependencies.
Current example:

API
↓

Application
↓

Domain

Infrastructure

The Domain project should never reference:
API
Infrastructure
UI
The compiler prevents invalid references.
Future architecture tests may verify these rules automatically.

13. Static Analysis
What
Automatically detect common coding problems.
How
Roslyn analyzers inspect code for:
code smells
performance issues
API misuse
dead code
maintainability concerns
Warnings appear during development and builds.
Additional analyzers may be introduced over time where they provide clear value without excessive noise.

14. GitHub Actions
What
Every Pull Request must pass automated verification.
How
GitHub Actions executes:

Restore

↓

Build

↓

Unit Tests

↓

Integration Tests

↓

Success

Only successful pipelines are eligible for merging.

15. Future Enforcement
The following may be introduced as the project grows.

These should only be adopted when they solve a real problem and provide a positive return on the additional maintenance they introduce.

16. Developer Workflow

Write Code
      │
      ▼
IDE Diagnostics
      │
      ▼
dotnet format
      │
      ▼
dotnet build
      │
      ▼
dotnet test
      │
      ▼
Commit
      │
      ▼
Push
      │
      ▼
GitHub Actions
      │
      ▼
Pull Request Review
      │
      ▼
Merge


17. Guiding Principles
Automate wherever practical.
Fail as early as possible.
Keep enforcement visible and understandable.
Minimise manual, repetitive checks.
Reserve human reviews for decisions requiring engineering judgement.
Introduce new enforcement only when its benefits outweigh its maintenance cost.
Prefer explicit, deterministic tooling over hidden or implicit behaviour.

# HotJoes AI Prompt
## PR-002 - Infer API Contract from Approved Domain Artefacts

### Objective

Act as a **Senior Business Analyst** with extensive experience in:

- Domain-Driven Design (DDD)
- API Contract Definition
- Event-Driven Architecture
- Enterprise Solution Architecture
- Regulated Marketplace Platforms

Use the attached HotJoes architectural artefacts as the **authoritative baseline** to infer the **Vendor Registration API Contract**.

The purpose of this exercise is to translate the approved business model into a complete API contract suitable for review prior to implementation.

---

# Attached Artefacts

The attached artefacts may include:

- HJ-002 – Architectural Principles
- HJ-003 – Ubiquitous Language Guide
- HJ-004 – Vendor Domain Models
- HJ-005 – Coding Standards
- HJ-104 – Vendor Registration Fields Matrix
- HJ-105 – Vendor Registration Sequence Diagram
- ADR-002 through ADR-008

Treat the attached versions as the **single source of truth**.

Where artefacts disagree, identify the conflict instead of attempting to resolve it.

---

# Objective

Infer the complete business contract required to support the Vendor Registration vertical slice.

The objective is **not** to redesign the domain.

The objective **is** to expose the approved business model as a complete API contract that can later be implemented.

---

# Scope

Define only the API operations required to support Vendor Registration.

At a minimum infer:

1. Business operations.
2. Business purpose of each operation.
3. Request information.
4. Required, optional, conditional and derived fields.
5. Validation rules.
6. Business invariants.
7. Successful outcomes.
8. Vendor lifecycle transitions.
9. Address Domain collaboration.
10. Trading Characteristics behaviour.
11. Legal Operator Type behaviour.
12. Idempotency expectations.
13. Domain Events.
14. Integration Events.
15. Business failures.
16. Information intentionally excluded from the API.

---

# Architectural Constraints

Preserve the architectural decisions already established.

Do **not**:

- redesign the Vendor Domain
- invent additional lifecycle states
- merge Registration and Activation
- move Compliance into Vendor Registration
- violate bounded context ownership
- introduce implementation assumptions unsupported by the artefacts

Preserve:

- Pending Activation
- Registration Session
- Registered Information
- Vendor Managed Information
- Address Domain ownership
- immutable Address snapshots
- Trading Characteristics
- Legal Operator Type
- naturally idempotent business operations
- event-driven collaboration
- events representing genuine business state changes

---

# Business Contract vs Technical API Design

Separate the document into two distinct parts.

## Part A — Business Contract

Infer only business behaviour from the attached artefacts.

Include:

- business operations
- requests
- outcomes
- validation
- lifecycle
- ownership
- invariants
- events
- failures
- idempotency

Everything in this section should be directly traceable to the attached artefacts.

---

## Part B — Proposed HTTP Representation

After inferring the business contract, propose a minimal HTTP representation including:

- HTTP methods
- routes
- request examples
- response examples
- status codes
- headers
- error structure

Clearly identify these as **technical conventions** unless explicitly defined by the attached artefacts.

---

# Required Output Structure

Produce a complete Markdown document using the following structure.

```
Document ID: HJ-106
Document Title: Vendor Registration API Contract
Version: 0.1
Status: Draft
Owner: Project Architecture
```

## Revision History

## Related Documents

## 1. Purpose

## 2. Source Artefacts

For each attached artefact briefly describe:

- why it was used
- what authority it provides

## 3. Business Operation Summary

Provide a table containing:

- Operation
- Business Purpose
- Owning Bounded Context
- Preconditions
- Successful Outcome

## 4. Register Vendor Business Contract

### 4.1 Business Intent

### 4.2 Preconditions

### 4.3 Request Information

Provide a table containing:

- Field
- Business Meaning
- Data Owner
- Required Status
- Source Artefact
- Validation
- Persisted or Transient
- Registered or Vendor Managed
- Notes

### 4.4 Derived Information

### 4.5 Business Validation

### 4.6 Address Domain Collaboration

### 4.7 Successful Outcome

### 4.8 Vendor Lifecycle Transition

### 4.9 Idempotency

### 4.10 Domain Events

For each event identify:

- Event
- Business Meaning
- Producing Bounded Context
- Minimum Payload
- Information intentionally excluded

### 4.11 Business Failures

Provide a table containing:

- Failure
- Trigger
- Business Meaning
- Retryable
- Expected Caller Behaviour

## 5. Information Outside Scope

Identify information intentionally excluded from the Vendor Registration API including:

- Activation
- Compliance decisions
- Operational Availability
- Vendor Management

## 6. Proposed HTTP Representation

### 6.1 Endpoint

### 6.2 Request Example

### 6.3 Success Response

### 6.4 Error Responses

### 6.5 HTTP Status Codes

### 6.6 Headers

## 7. Traceability Matrix

Provide a table containing:

- Contract Element
- Source Artefact
- Source Section / Model Element
- Interpretation

Every significant business rule should be traceable to one or more source artefacts.

## 8. Assumptions and Open Questions

Classify each item as:

- Confirmed
- Technical Convention
- Ambiguity
- Missing Information
- Artefact Conflict

Do **not** silently resolve ambiguities.

## 9. Review Checklist

Confirm that the proposed contract:

- models the approved business workflow
- preserves bounded context ownership
- uses ubiquitous language consistently
- preserves Vendor lifecycle decisions
- preserves Address Domain ownership
- keeps Registration separate from Activation
- keeps Compliance outside Vendor Registration
- supports naturally idempotent behaviour
- emits events only for genuine business state changes
- introduces no unsupported business concepts

---

# Output Format

Produce a **complete Markdown document** suitable for direct inclusion in the HotJoes documentation repository.

Requirements:

- Use standard GitHub Markdown.
- Use numbered headings.
- Use Markdown tables.
- Use fenced code blocks for request and response examples.
- Do not use HTML.
- Do not use Mermaid diagrams.
- Do not omit sections because information is unavailable. Instead explain why.
- The document should require no further formatting before being committed to Git.

---

# Review Standard

The completed document should be sufficiently detailed to become the baseline for:

- API Design Review
- OpenAPI Specification
- Application Command Design
- Integration Test Design
- Vendor Registration Test Catalogue

Do **not**:

- generate implementation code
- generate controllers
- generate handlers
- generate repositories
- generate database mappings
- generate an OpenAPI specification

The output should describe **the API contract**, not its implementation.

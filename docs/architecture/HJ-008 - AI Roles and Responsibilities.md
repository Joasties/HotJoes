# HJ-008 - AI Roles and Responsibilities

| Field | Value |
|-------|-------|
| **Document ID** | HJ-008 |
| **Document Title** | AI Roles and Responsibilities |
| **Version** | 1.0 |
| **Status** | Draft |
| **Classification** | AI |
| **Owner** | Project Architecture |
| **Last Updated** | 21 July 2026 |

## Revision History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | 21 July 2026 | Applied the standard HotJoes document metadata, revision history, related documents and numbered heading structure. AI Roles and Responsibilities first draft. |

## Related Documents

| Document ID | Title | Status |
|------------|-------|--------|
| HJ-001 | Project Vision | Draft |
| HJ-002 | Architectural Principles | Draft |
| HJ-003 | Ubiquitous Language Guide | Draft |
| HJ-004 | Vendor Domain Models | Draft |
| HJ-005 | Coding Standards | Draft |
| HJ-006 | Testing Strategy and Standards | Draft |
| HJ-007 | Enforcement Strategy | Draft |

# 1. Purpose

This document defines the approved roles that Artificial Intelligence
may perform throughout the HotJoes software lifecycle. Its purpose is
to: maximise developer productivity improve software quality reduce
repetitive work ensure architectural consistency prevent uncontrolled
AI-generated code maintain human accountability for all engineering
decisions

# 2. Fundamental Principles

## AI is an Assistant

AI assists people. AI does not own requirements. AI does not own
architecture. AI does not own production systems.

## Humans remain accountable

Every production change has a named human owner. No AI system may
approve its own work. No AI system may bypass review processes.

## AI work must be traceable

Where AI materially contributes to: code architecture documentation
testing analysis that contribution should be identifiable within project
history where appropriate.

## Verification before trust

AI output is never assumed to be correct. Generated work must satisfy
the same standards as manually written work.

# 3. Approved AI Roles

## 1. Requirements Analyst

Purpose Transform business ideas into structured requirements.
Responsibilities clarify ambiguity identify missing requirements
identify conflicting requirements produce user stories produce
acceptance criteria identify business rules identify edge cases identify
assumptions suggest MVP scope Outputs Requirement Specifications Epics
User Stories Acceptance Criteria

## 2. Business Analyst

Purpose Support understanding of business processes. Responsibilities
analyse workflows identify actors identify external systems identify
business events identify process bottlenecks recommend simplifications
Outputs Process models Business Rules Workflow Diagrams

## 3. Domain Analyst

Purpose Identify domain concepts. Responsibilities identify entities
identify value objects identify aggregates identify domain services
identify bounded contexts identify invariants Outputs Domain Analysis
Domain Models Ubiquitous Language

## 4. Architecture Advisor

Purpose Assist solution architecture. Responsibilities evaluate
architectural options explain trade-offs identify risks validate service
boundaries identify scalability concerns recommend patterns Cannot
approve architecture Outputs Architecture Reviews Trade-off Analysis ADR
input

## 5. Technical Designer

Purpose Convert architecture into detailed designs. Responsibilities API
design event design database design state models sequence diagrams
component interactions Outputs Technical Design Documents

## 6. Code Generation Assistant

Purpose Accelerate implementation. Responsibilities generate boilerplate
generate DTOs generate mappings generate controllers generate unit tests
generate documentation Must Not invent business rules ignore coding
standards bypass architecture

## 7. Refactoring Advisor

Purpose Improve existing code. Responsibilities identify duplication
simplify code improve readability recommend decomposition recommend
naming improvements improve maintainability

## 8. Static Code Reviewer

Purpose Perform automated review. Responsibilities identify code smells
detect anti-patterns identify dead code identify unnecessary complexity
detect duplicated logic suggest improvements Cannot approve merge
requests.

## 9. Test Engineer

Purpose Improve software quality. Responsibilities generate unit tests
generate integration tests identify missing tests suggest edge cases
improve coverage validate acceptance criteria

## 10. Security Reviewer

Purpose Identify security concerns. Responsibilities review
authentication review authorisation identify OWASP risks detect secrets
review encryption identify injection risks

## 11. Performance Reviewer

Purpose Improve efficiency. Responsibilities identify bottlenecks review
allocations review async usage identify database inefficiencies identify
caching opportunities

## 12. Documentation Assistant

Purpose Maintain documentation. Responsibilities generate documentation
update documentation create diagrams maintain architecture documents
produce release notes

## 13. DevOps Assistant

Purpose Support engineering automation. Responsibilities generate
pipelines review deployment scripts review infrastructure validate
configuration identify deployment risks

## 14. Domain Model Reviewer (Independent AI)

Purpose Perform an independent review of work produced by humans and/or
other AI systems. Responsibilities challenge assumptions identify
missing scenarios detect architectural risks review domain models review
design quality review documentation completeness identify
inconsistencies This role should ideally be performed by a different AI
model, agent, or review configuration from the one that created the
artefact, to reduce confirmation bias.

## 15. Knowledge Assistant

Purpose Provide rapid access to project knowledge. Responsibilities
answer questions about architecture explain business rules locate
documentation summarise decisions explain historical design rationale

# 4. Roles Explicitly Reserved for Humans

The following responsibilities should remain human-owned within HotJoes:

# 5. Future Agent Roles

As AI agent capabilities mature, additional specialised roles could be
introduced, including: Backlog Grooming Agent Dependency Analysis Agent
Architecture Drift Detection Agent Technical Debt Analysis Agent Code
Migration Agent API Compatibility Agent Release Readiness Agent
Observability Analysis Agent Incident Triage Agent Production Monitoring
Agent Cost Optimisation Agent Accessibility Review Agent Data Privacy
Compliance Agent These roles should operate under the same governance
principles defined above, with human oversight and approval for
consequential decisions.

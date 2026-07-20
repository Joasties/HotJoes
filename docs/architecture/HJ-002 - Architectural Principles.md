---
title: HJ-002 - Architectural Principles
---

  ----------------------------------- -----------------------------------
  **Document ID**                     HJ-002

  **Document Title**                  Architectural Principles

  **Version**                         1.2

  **Status**                          Draft

  **Classification**                  Principle

  **Owner**                           Project Architecture

  **Last Updated**                    17 July 2026
  ----------------------------------- -----------------------------------

Revision History

  ----------------------- ----------------------- -----------------------
  **Version**             **Date**                **Description**

  1.1                     16 July 2026            Previous draft.

  1.2                     17 July 2026            Applied the standard
                                                  HotJoes document
                                                  metadata, revision
                                                  history, related
                                                  documents and numbered
                                                  heading structure.
                                                  Architectural
                                                  principles and decision
                                                  checklist retained
                                                  unchanged.
  ----------------------- ----------------------- -----------------------

Related Documents

  ----------------------- ----------------------- -----------------------
  **Document ID**         **Title**               **Status**

  HJ-001                  HotJoes Project Vision  Draft

  HJ-003                  Ubiquitous Language     Draft
                          Guide                   

  HJ-004                  Coding Standards        Draft

  HJ-005                  Testing Strategy and    Draft
                          Standards               

  HJ-006                  Enforcement Strategy    Draft

  HJ-007                  AI Roles and            Draft
                          Responsibilities        
  ----------------------- ----------------------- -----------------------

1\. Purpose

This document defines the architectural principles that govern the
design, evolution and governance of the HotJoes platform.

Architectural principles are long-lived decision-making guidelines
rather than implementation rules. They establish the beliefs that shape
the architecture and provide a consistent basis for evaluating
architectural decisions.

Every significant architectural decision shall be traceable to one or
more principles defined in this document.

2\. How to Use this Document

These principles apply to every service, business capability,
integration, data store and shared platform component within the HotJoes
platform.

They guide architectural decisions, while detailed implementation
guidance belongs in the Coding Standards, Testing Strategy and
Standards, Enforcement Strategy and other engineering standards.

The principles in this document are mandatory unless an approved
Architectural Decision Record (ADR) explicitly documents and justifies
an exception.

3\. Business Principles

## AP-001 - Business First

**Statement\
**Technology shall serve the business.

**Rationale\
**Architectural decisions must be driven by business outcomes rather
than technology preferences.

**Implications**

Business value takes precedence over technical elegance.

Avoid adopting technology solely because it is fashionable.

Architecture should improve agility, maintainability or operational
effectiveness.

## AP-002 - Domain Models Express Business Language

**Statement\
**Domain models shall represent business concepts using ubiquitous
business language.

**Rationale\
**A shared business language reduces ambiguity and keeps the model
aligned with the problem domain.

**Implications**

Business terminology takes precedence over technical terminology.

Models should be understandable by both technical and business
stakeholders.

Persistence concerns shall not leak into the domain model.

4\. Ownership Principles

## AP-003 - Business Capabilities Own Their Behaviour

**Statement\
**Business behaviour shall belong exclusively to the capability that
owns it.

**Rationale\
**Clear ownership prevents inconsistent behaviour and enables
independent evolution.

**Implications**

Avoid duplicating business behaviour unless justified by autonomy,
performance or user experience.

Infrastructure shall contain no business behaviour.

Consumers shall invoke behaviour rather than reimplement it.

## AP-004 - Business Capabilities Own Their Persistence

**Statement\
**Each business capability shall be the logical owner of its persistence
model.

**Rationale\
**Logical ownership preserves autonomy regardless of physical
deployment.

**Implications**

Cross-capability database access is prohibited.

Expose behaviour rather than tables.

Physical deployment may change without affecting logical ownership.

## AP-005 - Single Authoritative Owner

**Statement\
**Every current business fact shall have one authoritative owner.

**Rationale\
**Clear ownership establishes a single source of truth while allowing
independent read models and projections.

**Implications**

Read models, caches and search indexes do not constitute ownership.

Historical snapshots belong to the consuming capability.

Updates shall occur only through published contracts.

5\. Integration Principles

## AP-006 - Explicit Published Contracts

**Statement\
**Business capabilities shall collaborate only through explicit
published contracts.

**Rationale\
**Stable contracts minimise coupling and enable independent evolution.

**Implications**

Internal implementation details remain private.

Collaboration occurs through approved APIs, events or other published
contracts.

Breaking changes require architectural governance.

Contracts shall be versioned where necessary.

## AP-007 - Events Represent Completed Business Facts

**Statement\
**Events shall describe completed business facts.

**Rationale\
**Events communicate outcomes rather than intentions.

**Implications**

Event names should use the past tense.

Events never request work.

Commands and events are distinct concepts.

6\. Design Principles

## AP-008 - Explicit Modelling

**Statement\
**Business intent shall be modelled explicitly.

**Rationale\
**Explicit systems are easier to understand, review, maintain and
evolve.

**Implications**

Prefer clarity over cleverness.

Minimise hidden conventions.

Framework behaviour shall never obscure business intent.

## AP-009 - Architectural Simplicity

**Statement\
**The architecture shall prefer the simplest solution that satisfies
current business requirements.

**Rationale\
**Complexity accumulates throughout the lifetime of a system. Every
unnecessary abstraction, framework or component increases maintenance
cost.

**Implications**

Avoid speculative abstraction.

Avoid premature optimisation.

Prefer understandable solutions.

Introduce complexity only when justified by measurable benefit.

## AP-010 - Long-Term Maintainability

**Statement\
**The architecture shall optimise for long-term change rather than
short-term delivery speed.

**Rationale\
**Most software cost is incurred after the initial release.

**Implications**

Technical debt shall be deliberate and documented.

Prefer simple, maintainable solutions.

Local optimisation shall not damage the overall architecture.

7\. Quality Principles

## AP-011 - Quality Attributes by Design

**Statement\
**Quality attributes shall be considered architectural responsibilities.

**Rationale\
**Security, resilience, scalability, performance, observability and
testability emerge from architecture rather than implementation.

**Implications**

Protect data integrity.

Design for failure.

Design for diagnosis.

Design for growth.

Ensure systems remain independently testable.

## AP-012 - Technology Independence

**Statement\
**Business behaviour shall not depend upon infrastructure.

**Rationale\
**Technology should support the business rather than define it.

**Implications**

Frameworks are implementation details.

Minimise unnecessary vendor lock-in.

Infrastructure decisions shall not leak into the business model.

8\. Governance Principles

## AP-013 - Human Accountability

**Statement\
**AI may assist architectural work, but humans remain accountable for
architectural decisions.

**Rationale\
**Architecture requires business judgement, governance and professional
accountability.

**Implications**

AI recommendations shall be reviewed before adoption.

Significant architectural decisions should be recorded using
Architectural Decision Records (ADRs).

Human approval is required for architectural changes.

9\. Architectural Decision Checklist

Every significant architectural decision should be evaluated against the
following questions.

  ----------------------------------- -----------------------------------
  **Question**                        **Relevant Principle**

  Does this solve a genuine business  AP-001
  problem?                            

  Does the model use business         AP-002
  language?                           

  Is ownership of behaviour and data  AP-003, AP-004, AP-005
  clear?                              

  Is business behaviour being         AP-003
  unnecessarily duplicated?           

  Does this introduce unnecessary     AP-006
  coupling?                           

  Are interactions based on explicit  AP-006
  contracts?                          

  Does the design model business      AP-008
  intent explicitly?                  

  Is this the simplest solution that  AP-009
  meets today\'s requirements?        

  Will this still be understandable   AP-010
  and maintainable in five years?     

  Does it satisfy the required        AP-011
  quality attributes?                 

  Is business logic independent of    AP-012
  infrastructure?                     

  Is there clear human accountability AP-013
  and, where appropriate, an ADR?     
  ----------------------------------- -----------------------------------

---
title: HJ-001 - Project Vision
---

  ----------------------------------- -----------------------------------
  **Document ID**                     HJ-001

  **Document Title**                  Project Vision

  **Version**                         1.0

  **Status**                          Draft

  **Classification**                  Vision

  **Owner**                           Project Architecture

  **Last Updated**                    21 July 2026
  ----------------------------------- -----------------------------------

Revision History

  ----------------------- ----------------------- -----------------------
  **Version**             **Date**                **Description**

  0.1                     16 July 2026            Previous draft.

  1.0                     21 July 2026            Applied the standard
                                                  HotJoes document
                                                  metadata, revision
                                                  history, related
                                                  documents and numbered
                                                  heading structure.
                                                  Project Vision unchanged.
  ----------------------- ----------------------- -----------------------

Related Documents

  ----------------------- ------------------------- -----------------------
  **Document ID**         **Title**                 **Status**

  HJ-002                  Architectural Principles  Draft

  HJ-003                  Ubiquitous Language       Draft
                          Guide

  HJ-004                  Vendor Domain Models      Draft

  HJ-005                  Coding Standards          Draft

  HJ-006                  Testing Strategy and      Draft
                          Standards

  HJ-007                  Enforcement Strategy      Draft

  HJ-008                  AI Roles and              Draft
                          Responsibilities
  ----------------------- ------------------------- -----------------------


1.  Purpose

    This document defines the business vision, objectives and
    strategic direction of the HotJoes platform.

    It establishes why the platform exists, who it serves and what business
    capabilities it intends to deliver.

    This document deliberately avoids detailed architectural design,
    business rules and implementation decisions. Those responsibilities
    belong to the Architectural Principles, Domain Analyses and supporting
    engineering documentation.

2.  Executive Summary

    HotJoes is a cloud-native online food ordering marketplace that connects
    independent food vendors, independent delivery drivers and customers
    through a modern, scalable digital platform.

    The project has two complementary objectives.

    The first is to build a commercially viable marketplace capable of
    supporting real-world food ordering and delivery operations.

    The second is to create a reference implementation demonstrating
    modern enterprise software engineering practices, including
    Domain-Driven Design (DDD), event-driven architecture, cloud-native
    technologies and disciplined engineering governance.

    The platform will be delivered incrementally, beginning with Vendor
    onboarding before expanding into customer ordering, payments and
    delivery capabilities.

3.  Vision Statement

    HotJoes is a cloud-native marketplace that enables
    independent food vendors to register and manage their businesses,
    independent delivery drivers to participate in the platform and
    customers to discover, order and receive food through a scalable,
    event-driven marketplace.

    The platform aims to provide an experience comparable to established
    online food ordering marketplaces whilst differentiating itself through
    fairness, transparency and sustainable commercial relationships for
    vendors, delivery drivers, customers and the platform itself.

    Beyond delivering commercial value, HotJoes also serves as a reference
    implementation of modern enterprise architecture and software engineering,
    demonstrating how clear business boundaries, Domain-Driven Design,
    event-driven collaboration and disciplined engineering governance can be
    combined to build complex business systems that remain understandable,
    maintainable and adaptable throughout their lifetime.

4.  Business Vision

    The vision of HotJoes is to create a trusted digital marketplace where
    independent food businesses and independent delivery drivers can thrive
    alongside customers through a platform that is fair, transparent and
    scalable.

    Rather than optimising solely for platform growth, HotJoes seeks to
    create sustainable long-term value for every participant by balancing
    commercial success with equitable and transparent business relationships.

5.  Business Objectives

    The objectives of the project are to:

    Deliver a production-quality online food ordering platform.

    Enable independent Vendors to register and manage their businesses.

    Enable independent Delivery Drivers to participate in the platform.

    Provide Customers with a simple, reliable and trustworthy ordering experience.

    Build a scalable marketplace capable of supporting future business growth.

    Demonstrate modern enterprise architecture and software engineering practices.

    Maintain clear ownership of business capabilities and business information.

    Create a platform that can evolve through the addition of new capabilities
    without significant architectural redesign.

6.  Business Capability Roadmap

    The platform will be delivered incrementally through independently evolving
    business capabilities.

    Phase 1 -- Vendor Onboarding

    Vendor Registration

    Vendor Profile Management

    Vendor Activation

    Vendor Compliance

    Menu Management

    The initial implementation focuses on enabling a prospective Vendor to
    register with the platform, maintain its business information and
    progress through the onboarding process.

    Registration creates a Vendor in a Pending Activation lifecycle state.

    Activation is a separate business decision that permits a Vendor to
    participate in the marketplace once all mandatory activation prerequisites
    have been satisfied.

    The specific prerequisites are determined by the Vendor Activation Policy
    and may evolve as additional business capabilities are introduced.

    Phase 2 -- Customer Ordering

    Customer Registration

    Address Management

    Menu Browsing

    Basket Management

    Order Placement

    Phase 3 -- Payments

    Payment Processing

    Refund Management

    Payment Reconciliation

    Phase 4 -- Delivery

    Delivery Driver Registration

    Delivery Assignment

    Delivery Tracking

    Delivery Status Updates

    Additional business capabilities will be introduced as the platform
    evolves.

7.  Business Model

    HotJoes operates as a digital marketplace connecting independent
    Vendors with Customers and independent Delivery Drivers.

    The platform coordinates Vendor onboarding, menu publication,
    customer ordering, payment processing and delivery while supporting
    fair and sustainable commercial relationships between all
    participants.

    Revenue is expected to be generated primarily through marketplace
    commissions together with value-added platform services.

8.  Stakeholders

    The primary stakeholders are:

    Customers

    Vendors

    Delivery Drivers

    Platform Administrators

    Customer Support

    Platform Operations

    Future Business Partners

9.  Core Business Capabilities

    The platform will be organised around independently evolving business
    capabilities including:

    Vendor Management

    Vendor Compliance

    Menu Management

    Customer Management

    Ordering

    Payments

    Delivery

    Address Management

    Notifications

    Platform Administration

    Each business capability owns its own business responsibilities and
    collaborates with other capabilities through explicit published
    contracts.

    Business capabilities remain autonomous and evolve independently
    wherever practical.

10. Business Principles

    The following principles guide business decision-making throughout
    the project.

    BP-001 -- Fair Marketplace
    Create a fair marketplace for Vendors, Delivery Drivers and
    Customers.

    BP-002 -- Transparency
    Promote transparency in business processes and commercial relationships
    wherever practical.

    BP-003 -- Support Independent Businesses
    Enable independent Vendors and Delivery Drivers to succeed through
    a sustainable marketplace.

    BP-004 -- Customer Trust
    Provide Customers with a reliable, trustworthy and consistent
    ordering experience.

    BP-005 -- Long-Term Sustainability
    Optimise for sustainable long-term business success rather than
    short-term growth.

    BP-006 -- Continuous Evolution
    Design the business so that new capabilities can be introduced
    incrementally without fundamental redesign.

11. Success Criteria

    The project will be considered successful when it:

    Supports real Vendors operating through the platform.

    Supports real Customer orders.

    Supports independent Delivery Drivers.

    Demonstrates clear ownership of business capabilities.

    Maintains clean business and architectural boundaries.

    Evolves through the addition of new business capabilities
    without major redesign.

    Serves as a high-quality reference implementation of modern
    enterprise software engineering practices.

12. Scope Boundaries

    The Project Vision intentionally defines strategic business
    direction rather than implementation detail.

    The following topics are defined elsewhere within the
    architecture repository:

    Business terminology --- HJ-003 - Ubiquitous Language Guide
    Architectural governance --- HJ-002 - Architectural Principles
    Domain boundaries and responsibilities --- HJ004 - Vendor Domain Model
    Lifecycle models and business rules --- HJ004 - Vendor Domain Model
    Engineering standards and implementation guidance ---
    HJ005 Coding Standards, HJ006 Testing Strategy and Standards,
    HJ007 Enforcement Strategy, HJ008 - AI Roles and Responsibilities

    This separation of responsibilities ensures that the Project Vision
    remains stable while detailed business analysis and implementation
    evolve over time.
13. References
    HJ-002 Architectural Principles
    HJ-003 Ubiquitous Language Guide
    HJ-004 Vendor Domain Models
    HJ-005 Coding Standards
    HJ-006 Testing Strategy and Standards
    HJ-007 Enforcement Strategy
    HJ-008 AI Roles and Responsibilities

Together these documents establish the strategic vision, architectural
principles, business vocabulary and domain understanding, coding
standards, testing strategies, implementation and formatting guidelines
and roles performed by AI within the development of the system that
underpin the HotJoes platform.

# HJ-003 – Ubiquitous Language Guide

| Property | Value |
|----------|-------|
| **Document ID** | HJ-003 |
| **Document Title** | Ubiquitous Language Guide |
| **Version** | 2.0 |
| **Status** | Approved |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 22 July 2026 |

## Revision History

| Version | Date | Description |
|---------|------|-------------|
| 0.1 | 16 July 2026 | Previous draft. |
| 0.2 | 16 July 2026 | Updated Vendor Domain Models. |
| 1.0 | 17 July 2026 | Applied the standard HotJoes document metadata, revision history, related documents and numbered heading structure. Architectural principles and decision checklist retained unchanged. |
| 1.1 | 20 July 2026 | Introduced Vendor operating classification concepts and aligned the language with the Vendor Registration lifecycle. |
| 1.2 | 22 July 2026 | Introduced Trading Characteristics, Trading Location, Registered Information, Vendor Managed Information and registered-information controls. |
| 2.0 | 22 July 2026 | Approved Vendor Registration terminology. Clarified Registration Session, Trading Characteristics, Regulatory Authorities, Vendor lifecycle, Registered Information and Address ownership. |

## Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| HJ-001 | HotJoes Project Vision | Draft |
| HJ-002 | Architectural Principles | Draft |
| HJ-004 | Vendor Domain Models | Draft |

# 1. Purpose

This document defines the official business language used throughout the HotJoes platform.

Its purpose is to ensure that business stakeholders, product owners, architects and software developers all use exactly the same terminology when discussing, designing and implementing the system.

Whenever a business concept is introduced, this document becomes the authoritative definition.

If terminology conflicts with this document, this document takes precedence.

# 2. Principles

The ubiquitous language shall be:

- Business focused
- Technology independent
- Unambiguous
- Consistent across documentation and source code
- Shared by both technical and non-technical stakeholders

Names used within:

- Domain Models
- Events
- Commands
- Queries
- APIs
- Databases
- User Stories
- Acceptance Criteria
- Documentation

should all originate from this language.

# 3. Vendor Domain

## 3.1 Vendor

A Vendor is a business that offers food for sale through the HotJoes platform.

A Vendor owns one or more Menus and fulfils Customer Orders.

A Vendor exists for the lifetime of the business.

It may move between operational states but retains the same identity.

A Vendor may represent:

- Sole Trader
- Partnership
- Limited Company
- Charity
- Other legally recognised organisation

## 3.2 Vendor Registration

The process through which a prospective Vendor supplies sufficient Registered Information to create a Vendor within the platform.

A Vendor does not exist before successful submission.

Successful submission creates both the Vendor and its initial lifecycle state of **Pending Activation**.

Vendor Registration does not authorise trading.

Compliance and Activation remain separate business processes.

## 3.3 Vendor Activation

An administrative transition that moves a Vendor from Pending Activation to Active.

Activation occurs only after all Activation Requirements have been satisfied and authorises the Vendor to trade on the platform.

Compliance rules are owned by the Vendor Compliance domain.

## 3.4 Trading Characteristics

Trading Characteristics describe the characteristics of a Vendor's trading operation that are used to determine Compliance Requirements.

Trading Characteristics consist of:

- Trading Location
- Opening Hours
- Service Includes Hot Food
- Alcohol Service

Trading Characteristics describe operating characteristics and do not determine legal identity.

### 3.4.1 Trading Location

Trading Location is a controlled business classification of the location from which a Vendor conducts its trading operation.

| Value | Description |
|-------|-------------|
| Restaurant | Restaurant, Café, Takeaway or Food Market Hall. A customer-facing permanent premises. |
| Stall | Mobile Food Unit or Market Stall. |
| Kitchen | Dark Kitchen, Ghost Kitchen or Home Kitchen. A non-customer-facing food preparation venue that trades exclusively online. |

### 3.4.2 Opening Hours

Opening Hours are represented by:

- Start Time
- End Time

Opening Hours may legitimately span midnight. Validation must not require Start Time to be earlier than End Time.

### 3.4.3 Service Includes Hot Food

Indicates whether the Vendor supplies food or drink heated above ambient room temperature.

This information is used together with Opening Hours to determine applicable Compliance Requirements.

### 3.4.4 Alcohol Service

Indicates whether the Vendor supplies alcohol.

This information is used to determine applicable Compliance Requirements.

## 3.5 Registration Session

A transient application concept used to collect registration information before a Vendor exists.

A Registration Session:

- exists only while registration is being completed
- is not a business entity
- is not part of the Vendor lifecycle
- may be implemented using any suitable transient persistence mechanism
- expires automatically after inactivity
- cannot be resumed
- is discarded completely if abandoned
- produces no business events

> Completion of Vendor Registration represents a deliberate expression of intent to become a Vendor on the platform. Registration Sessions are intentionally transient and cannot be resumed.

## 3.6 Vendor Status

Represents the lifecycle state of the Vendor.

Examples include:

- Pending Activation
- Active
- Suspended
- Deactivated

Vendor Status reflects administrative state.

The Vendor lifecycle begins when the Vendor is created.

Pending Activation means the Vendor exists but has not yet satisfied all activation requirements.

It is not the same as whether the Vendor is currently accepting orders.

## 3.7 Trading Preference

Represents whether an Active Vendor wishes to receive new Orders.

Possible values include:

- Online
- Offline

A Vendor may choose to go Offline for holidays, maintenance or other operational reasons.

A Suspended Vendor is automatically Offline.

## 3.8 Suspension

An administrative action preventing a Vendor from trading.

Suspension may take effect immediately or at a scheduled date and time.

Suspension is independent of Trading Preference.

## 3.9 Trading Name

The public name under which a Vendor trades.

Customers interact with the Trading Name rather than the legal organisation.

## 3.10 Legal Operator

The legally recognised organisation responsible for operating the Vendor.

Examples include:

- Sole Trader
- General Partnership
- Limited Company (Ltd)
- Limited Liability Partnership (LLP)
- Charitable Incorporated Organisation (CIO)
- Registered Charity
- Other legally recognised organisations

Legal Operator Type determines legal identity. Trading Characteristics describe the operating characteristics of the Vendor.

## 3.11 Company Name

The registered legal name of the Legal Operator.

This may differ from the Trading Name.

## 3.12 Registration Number

The government-issued registration identifier of the Legal Operator where applicable.

Whether mandatory depends upon the Legal Operator Type.

## 3.13 Contact Details

The information used to communicate with the Vendor.

Examples include:

- Email Address
- Telephone Number

## 3.14 Address

The Address Domain owns address search, retrieval and validation.

The Vendor Domain stores an approved snapshot of the Business Address as Registered Information.

## 3.15 Registered Information

The information supplied during Vendor Registration that establishes the legal identity and operating characteristics of a Vendor.

Registered Information includes:

- Trading Name
- Legal Operator Name
- Legal Operator Type
- Company Registration Number
- Contact Name
- Contact Email
- Contact Telephone
- Business Address
- Food Registration Authority
- Primary Trading Authority (where applicable)
- Trading Characteristics

Registered Information becomes read-only to the Vendor once Vendor Registration has been successfully submitted and the Vendor enters Pending Activation.

Changes to Registered Information are performed only by authorised platform operators using administrative processes.

## 3.16 Vendor Managed Information

Information that may be maintained directly by the Vendor without affecting the registered identity of the business.

Examples include:

- Website
- Business Description

## 3.17 Food Registration Authority

The competent local authority responsible for Food Business Registration for the Vendor's trading premises.

Food Registration Authority:

- is derived from the approved Business Address
- is not manually selected by the Vendor
- is supplied by the Address Domain
- forms part of Registered Information

## 3.18 Primary Trading Authority

The local authority responsible for the Vendor's declared primary trading area.

Primary Trading Authority:

- is required only for Trading Location = Stall
- is derived through the Address Domain
- is used to determine applicable Compliance Requirements

# 4. Business Concepts

## 4.1 Registration

Creation of a Vendor in Pending Activation following successful completion of Vendor Registration.

## 4.2 Activation

Administrative authorisation permitting a Vendor to begin trading after all Activation Requirements have been satisfied.

## 4.3 Trading

Accepting and fulfilling Customer Orders.

## 4.4 Operational Availability

Whether the Vendor may currently receive new Orders.

Operational Availability is determined by Vendor Status and Trading Preference.

A Vendor must be Active before Trading Preference can enable trading.

## 4.5 Compliance

The verification of legal and regulatory obligations required before activation.

Compliance is owned entirely by the Vendor Compliance domain.

Activation consumes the outcome of Compliance rather than implementing compliance rules.

## 4.6 Activation Requirement

A business requirement that must be satisfied before a Vendor may be activated.

Examples include:

- Food Business Registration
- Council Registration
- Trading Licence
- Identity Verification
- Insurance
- Food Hygiene Rating

The Vendor Compliance domain owns Activation Requirements.

## 4.7 Activation Policy

The policy determining when activation is permitted based upon Activation Requirements.

## 4.8 Pending Activation Closure Policy

The policy governing what happens when a Vendor remains in Pending Activation beyond an acceptable period.

Examples include:

- Reminder notifications
- Administrative review
- Automatic deactivation

# 5. Events

These represent significant business occurrences.

Examples include:

- Vendor Registered
- Vendor Activated
- Vendor Suspended
- Vendor Unsuspended
- Vendor Taken Offline
- Vendor Brought Online
- Trading Name Changed
- Contact Details Updated
- Activation Requirements Satisfied
- Vendor Deactivated

Registration Session is not a business concept and therefore does not generate domain events.

Event names should always be expressed in the past tense.

# 6. Commands

Commands represent requests to perform business actions.

Examples include:

- Register Vendor
- Activate Vendor
- Suspend Vendor
- Resume Vendor
- Change Trading Name
- Update Contact Details
- Set Trading Preference

Registration Session is an application concern rather than a business command.

Command names should always be expressed as imperative verbs.

# 7. Glossary

| Term | Definition |
|------|------------|
| Vendor | A business selling food through HotJoes |
| Trading Name | Public business name |
| Company Name | Legal registered organisation name |
| Vendor Status | Administrative lifecycle state |
| Trading Preference | Whether an Active Vendor wishes to accept Orders |
| Activation | Administrative process allowing trading |
| Registration | Creation of a Vendor record |
| Suspension | Administrative prevention of trading |
| Operational Availability | Whether Orders may currently be accepted, as determined by Vendor Status and Trading Preference |
| Compliance | Verification of legal and regulatory obligations required before activation |
| Vendor Registration | Process through which a prospective Vendor supplies sufficient Registered Information to create a Vendor within the platform |
| Trading Characteristics | Characteristics of a Vendor's trading operation used to determine Compliance Requirements |
| Trading Location | Controlled business classification with the values Restaurant, Stall and Kitchen |
| Address | Address information searched, retrieved and validated by the Address Domain; the approved Business Address snapshot is stored by the Vendor Domain as Registered Information |
| Registered Information | Information supplied during Vendor Registration that establishes the legal identity and operating characteristics of a Vendor, including applicable regulatory authorities and Trading Characteristics |
| Vendor Managed Information | Information that may be maintained directly by the Vendor without affecting the registered identity of the business |
| Registration Session | Transient application concept used to collect registration information before a Vendor exists; it cannot be resumed and produces no business events |
| Food Registration Authority | Competent local authority responsible for Food Business Registration for the Vendor's trading premises |
| Primary Trading Authority | Local authority responsible for the Vendor's declared primary trading area; required only for Trading Location = Stall |
| Activation Requirement | Business requirement that must be satisfied before activation |
| Activation Policy | Policy determining when activation is permitted |
| Pending Activation Closure Policy | Policy governing prolonged Pending Activation |

# 8. Naming Conventions

The following conventions apply throughout the solution.

## 8.1 Commands

Use imperative verbs.

Examples:

- Register Vendor
- Activate Vendor
- Suspend Vendor

## 8.2 Events

Use past tense.

Examples:

- Vendor Registered
- Vendor Activated
- Vendor Suspended

## 8.3 Queries

Describe the information required.

Examples:

- Get Vendor
- Search Vendors
- Get Vendor Summary

## 8.4 Value Objects

Represent business concepts rather than primitive data types.

Examples:

- VendorId
- TradingName
- EmailAddress
- TelephoneNumber
- RegistrationNumber

## 8.5 Aggregates

Aggregate names should represent business nouns.

Example:

- Vendor

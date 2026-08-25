# HJ-003 – Ubiquitous Language Guide

| Property | Value |
|----------|-------|
| **Document ID** | HJ-003 |
| **Document Title** | Ubiquitous Language Guide |
| **Version** | 2.3 |
| **Status** | Approved |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 17 August 2026 |

## Revision History

| Version | Date | Description |
|---------|------|-------------|
| 0.1 | 16 July 2026 | Previous draft. |
| 0.2 | 16 July 2026 | Updated Vendor Domain Models. |
| 1.0 | 17 July 2026 | Applied the standard HotJoes document metadata, revision history, related documents and numbered heading structure. Architectural principles and decision checklist retained unchanged. |
| 1.1 | 20 July 2026 | Introduced Vendor operating classification concepts and aligned the language with the Vendor Registration lifecycle. |
| 1.2 | 22 July 2026 | Introduced Trading Characteristics, Trading Location, Registered Information, Vendor Managed Information and registered-information controls. |
| 2.0 | 22 July 2026 | Approved Vendor Registration terminology. Clarified Registration Session, Trading Characteristics, Regulatory Authorities, Vendor lifecycle, Registered Information and Address ownership. |
| 2.1 | 27 July 2026 | Applied CR-020 and CR-023 to clarify Legal Operator terminology, place Registration Session outside the Vendor service boundary and establish the submitted Register Vendor request as authoritative. |
| 2.2 | 8 August 2026 | Applied CR-026 to define registered Vendor retrieval for Epic 1 and aligned the lifecycle term Vendor State with HJ-004. |
| 2.3 | 17 August 2026 | Applied CR-048. Defined Address Resolution Reference, complete Address result, Business Address Snapshot structure and Address failure terms for CON-006–CON-010. |

## Related Documents

| Document ID | Title | Status |
|-------------|-------|--------|
| HJ-001 | HotJoes Project Vision | Approved |
| HJ-002 | Architectural Principles | Approved |
| HJ-004 | Vendor Domain Models | Approved |
| CR-026 | Define Registered Vendor Retrieval for Epic 1 | Approved |

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

An administrative transition that moves a Vendor from Pending Activation to Activated.

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

A Registration Session is a transient, client- or BFF-owned interaction working set used to collect registration information before a Vendor exists.

It is not known to the Vendor Domain, is not part of any Vendor service boundary, produces no domain events, cannot be resumed, and is discarded by its owner following abandonment or successful submission.

Once a **Register Vendor** command is submitted, the Registration Session has fulfilled its purpose and no longer participates in the registration workflow.

The complete RegisterVendor request is the sole authoritative source of client-authored registration information and the approved Address Resolution reference. Address-owned information is obtained directly from the Address Domain using that reference.

## 3.6 Vendor State

Represents the lifecycle state of the Vendor.

Examples include:

- Pending Activation
- Activated
- Suspended
- Deactivated

Vendor State reflects administrative state.

The Vendor lifecycle begins when the Vendor is created.

Pending Activation means the Vendor exists but has not yet satisfied all activation requirements.

It is not the same as whether the Vendor is currently accepting orders.

## 3.7 Trading Preference

Represents whether an Activated Vendor wishes to receive new Orders.

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

## 3.10 Legal Operator Type

Legal Operator Type defines the classification of the legal organisation or individual responsible for operating the Vendor business.

Examples include:

- Sole Trader
- General Partnership
- Limited Company
- Limited Liability Partnership
- Charitable Community Group
- Charitable Incorporated Organisation

Legal Operator Type determines legal identity. Trading Characteristics describe the operating characteristics of the Vendor.

## 3.11 Legal Operator Name

**Legal Operator Name** is the authoritative business term for the registered legal name of the entity or individual responsible for operating the Vendor business.

All new architectural artefacts, domain models, commands, events, APIs, service contracts and documentation shall use **Legal Operator Name** when referring to this business field.

## 3.12 Company Name

**Company Name** is retained solely as a historical synonym for **Legal Operator Name** to preserve compatibility with earlier project artefacts.

It is not the preferred architectural term and shall not be introduced into new architectural documentation, commands, events, APIs, service contracts or domain models.

## 3.13 Registration Number

The government-issued registration identifier of the Legal Operator where applicable.

Whether mandatory depends upon the Legal Operator Type.

## 3.14 Contact Details

The information used to communicate with the Vendor.

Examples include:

- Email Address
- Telephone Number

## 3.15 Address

The Address Domain owns address search, retrieval and validation.

The Vendor Domain stores an approved snapshot of the Business Address as Registered Information.

### Address Resolution Reference

A permanent opaque reference issued by the Address Domain only after selection of a complete authoritative Address result containing a Canonical Address Identifier. It binds that result and the Trading Location declared at selection. It is non-expiring, non-revocable, reusable and non-consuming. Resolution with the same Trading Location deterministically returns the original immutable result; a mismatched Trading Location is rejected.

### Complete Address Result

An Address Domain success result containing a Canonical Address Identifier, the selected postal-address information and the regulatory-authority information required for the supplied Trading Location. Search refinement and incomplete search results are Address interaction outcomes, not successful Address selections.

### Business Address Snapshot

The immutable Vendor-owned historical representation translated from a complete Address result. It contains up to three address lines, Post Town, Postcode, optional County and optional Recipient or Organisation Name. It is not a live Address record.

### Invalid Reference

The semantic outcome returned when an Address Resolution Reference is unknown or fabricated.

### Invalid Address Result

The semantic outcome returned when a known Address Resolution Reference cannot supply a complete valid result for the Trading Location declared at registration.

## 3.16 Registered Information

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

## 3.17 Vendor Managed Information

Information that may be maintained directly by the Vendor without affecting the registered identity of the business.

Examples include:

- Website
- Business Description

## 3.18 Food Registration Authority

The competent local authority responsible for Food Business Registration for the Vendor's trading premises.

Food Registration Authority:

- is derived from the approved Business Address
- is not manually selected by the Vendor
- is supplied by the Address Domain
- forms part of Registered Information

## 3.19 Primary Trading Authority

The local authority responsible for the Vendor's declared primary trading area.

Primary Trading Authority:

- is required only for Trading Location = Stall
- is derived through the Address Domain
- is used to determine applicable Compliance Requirements

## 3.20 Vendor Administrator

A **Vendor Administrator** is the trusted Epic 1 administrative actor that may retrieve an existing Vendor directly using its VendorId.

Authentication and authorisation of the Vendor Administrator are outside the scope of Epic 1. The term does not imply an Identity Domain dependency within Epic 1.

## 3.21 Retrieve Registered Vendor

**Retrieve Registered Vendor** is a read-only Vendor query that retrieves an existing Vendor using its VendorId and returns the Vendor details established through Vendor Registration.

The query does not modify Vendor state and produces no Domain Event or Integration Event.

## 3.22 Registered Vendor Details

**Registered Vendor Details** is the read representation returned by Retrieve Registered Vendor.

It is derived from persisted Vendor state and contains the Vendor information established through successful Vendor Registration. It is a service/query representation and is not the Vendor aggregate itself.

# 4. Business Concepts

## 4.1 Registration

Creation of a Vendor in Pending Activation following successful completion of Vendor Registration.

## 4.2 Activation

Administrative authorisation permitting a Vendor to begin trading after all Activation Requirements have been satisfied.

## 4.3 Trading

Accepting and fulfilling Customer Orders.

## 4.4 Operational Availability

Whether the Vendor may currently receive new Orders.

Operational Availability is determined by Vendor State and Trading Preference.

A Vendor must be Activated before Trading Preference can enable trading.

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
| Legal Operator Type | Classification of the legal organisation or individual responsible for operating the Vendor business; examples include Sole Trader, Limited Company, Limited Liability Partnership and Charitable Incorporated Organisation |
| Legal Operator Name | Authoritative business term for the registered legal name of the entity or individual responsible for operating the Vendor business; preferred terminology for all new architectural artefacts |
| Company Name | Historical synonym for Legal Operator Name retained solely for continuity with legacy documentation; not an active architectural term and not to be used in new artefacts |
| Vendor State | Administrative lifecycle state |
| Trading Preference | Whether an Activated Vendor wishes to accept Orders |
| Activation | Administrative process allowing trading |
| Registration | Creation of a Vendor record |
| Suspension | Administrative prevention of trading |
| Operational Availability | Whether Orders may currently be accepted, as determined by Vendor State and Trading Preference |
| Compliance | Verification of legal and regulatory obligations required before activation |
| Vendor Registration | Process through which a prospective Vendor supplies sufficient Registered Information to create a Vendor within the platform |
| Trading Characteristics | Characteristics of a Vendor's trading operation used to determine Compliance Requirements |
| Trading Location | Controlled business classification with the values Restaurant, Stall and Kitchen |
| Address | Address information searched, retrieved and validated by the Address Domain; the approved Business Address snapshot is stored by the Vendor Domain as Registered Information |
| Registered Information | Information supplied during Vendor Registration that establishes the legal identity and operating characteristics of a Vendor, including applicable regulatory authorities and Trading Characteristics |
| Vendor Managed Information | Information that may be maintained directly by the Vendor without affecting the registered identity of the business |
| Registration Session | Transient interaction working set owned by the client application or a BFF and existing outside every Vendor service boundary; it is not known to the Vendor Domain, and the submitted Register Vendor request becomes authoritative once registration is submitted |
| Food Registration Authority | Competent local authority responsible for Food Business Registration for the Vendor's trading premises |
| Primary Trading Authority | Local authority responsible for the Vendor's declared primary trading area; required only for Trading Location = Stall |
| Vendor Administrator | Trusted Epic 1 administrative actor that may retrieve an existing Vendor directly using its VendorId; authentication and authorisation are outside the Epic 1 scope |
| Retrieve Registered Vendor | Read-only Vendor query that retrieves one existing Vendor by VendorId and returns Registered Vendor Details without changing state or producing events |
| Registered Vendor Details | Purpose-specific service/query representation derived from persisted Vendor state and containing the information established through successful Vendor Registration; not the Vendor aggregate |
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
- Retrieve Registered Vendor

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

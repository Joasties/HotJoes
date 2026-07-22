# HJ-003 – Ubiquitous Language Guide

| Property | Value |
|----------|-------|
| **Document ID** | HJ-003 |
| **Document Title** | Ubiquitous Language Guide |
| **Version** | 1.2 |
| **Status** | Draft |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 20 July 2026 |

## Revision History

| Version | Date | Description |
|---------|------|-------------|
| 0.1 | 16 July 2026 | Previous draft. |
| 0.2 | 16 July 2026 | Updated Vendor Domain Models. |
| 1.0 | 17 July 2026 | Applied the standard HotJoes document metadata, revision history, related documents and numbered heading structure. Architectural principles and decision checklist retained unchanged. |
| 1.1 | 20 July 2026 | Introduced Vendor operating classification concepts and aligned the language with the Vendor Registration lifecycle. |
| 1.2 | 22 July 2026 | Introduced Trading Characteristics, Trading Location, Registered Information, Vendor Managed Information and registered-information controls. |

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

The process by which a prospective Vendor supplies sufficient information to create a Vendor record within the platform.

Vendor Registration creates the Vendor in Pending Activation and creates the initial Registration Session.

Vendor Registration does not authorise trading and does not imply that compliance has been achieved.

The Vendor Compliance domain determines whether activation requirements have been satisfied.

## 3.3 Vendor Activation

An administrative transition that moves a Vendor from Pending Activation to Active.

Activation occurs only after all Activation Requirements have been satisfied and authorises the Vendor to trade on the platform.

Compliance rules are owned by the Vendor Compliance domain.

## 3.4 Trading Characteristics

Trading Characteristics describe the characteristics of a Vendor's trading operation that are used to determine compliance requirements.

Trading Characteristics consist of:

- Trading Location
- Opening Hours
- Service Includes Hot Food
- Alcohol Service

Trading Characteristics describe operating characteristics and do not determine legal identity.

### 3.4.1 Trading Location

The controlled classification of the location from which a Vendor conducts its trading operation.

Values:

- Customer-Facing Permanent Premises — A fixed premises that customers may visit.
- Mobile Food Unit or Market Stall — A mobile or market-based trading location.
- Home or Dark Kitchen — A home-based or non-customer-facing food preparation location.

## 3.5 Registration Session

The temporary process through which a prospective Vendor supplies registration information.

A Registration Session:

- Exists only while registration is being completed
- Is discarded if abandoned
- Is not part of the Vendor lifecycle
- Is not a persistent business concept after registration completes

## 3.6 Vendor Status

Represents the lifecycle state of the Vendor.

Examples include:

- Draft
- Pending Activation
- Active
- Suspended

Vendor Status reflects administrative state.

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

The physical trading location of the Vendor.

Address data is owned by the Address domain.

The Vendor domain stores only the address reference and any locally required snapshot or reference data.

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
- Trading Characteristics

Registered Information becomes read-only to the Vendor once Vendor Registration has been successfully submitted and the Vendor enters PendingActivation.

Changes to Registered Information are performed only by authorised platform operators using administrative processes.

## 3.16 Vendor Managed Information

Information that may be maintained directly by the Vendor without affecting the registered identity of the business.

Examples include:

- Website
- Business Description

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
- Registration Session Started
- Registration Session Abandoned
- Activation Requirements Satisfied
- Vendor Deactivated

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
- Start Registration Session
- Complete Registration

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
| Trading Characteristics | Characteristics of a Vendor's trading operation used to determine compliance requirements |
| Trading Location | Controlled classification of the location from which a Vendor conducts its trading operation |
| Registered Information | Information supplied during Vendor Registration that establishes the legal identity and operating characteristics of a Vendor |
| Vendor Managed Information | Information that may be maintained directly by the Vendor without affecting the registered identity of the business |
| Registration Session | Temporary process through which a prospective Vendor supplies registration information |
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

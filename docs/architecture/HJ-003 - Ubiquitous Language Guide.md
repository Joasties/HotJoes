# HJ-003 – Ubiquitous Language Guide  
  
| Property | Value |  
|----------|-------|  
| **Document ID** | HJ-003 |  
| **Document Title** | Ubiquitous Language Guide |  
| **Version** | 1.0 |  
| **Status** | Draft |  
| **Classification** | Architecture |  
| **Owner** | Project Architecture |  
| **Last Updated** | 17 July 2026 |  
  
## Revision History  
  
| Version | Date | Description |  
|---------|------|-------------|  
| 0.1 | 16 July 2026 | Previous draft. |  
| 0.2 | 16 July 2026 | Updated Vendor Domain Models. |  
| 1.0 | 17 July 2026 | Applied the standard HotJoes document metadata, revision history, related documents and numbered heading structure. Architectural principles and decision checklist retained unchanged. |  
  
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
  
Vendor Registration does not imply that the Vendor is authorised to trade.  
  
## 3.3 Vendor Activation  
  
The process that changes a Vendor from Pending Activation to Active.  
  
Activation indicates the Vendor is permitted to trade on the platform.  
  
Whether compliance checks, council registration or trading licence verification are prerequisites is outside the scope of this document and will be defined by the Vendor Compliance domain.  
  
## 3.4 Vendor Status  
  
Represents the lifecycle state of the Vendor.  
  
Examples include:  
  
- Draft  
- Pending Activation  
- Active  
- Suspended  
  
Vendor Status reflects administrative state.  
  
It is not the same as whether the Vendor is currently accepting orders.  
  
## 3.5 Trading Preference  
  
Represents whether an Active Vendor wishes to receive new Orders.  
  
Possible values include:  
  
- Online  
- Offline  
  
A Vendor may choose to go Offline for holidays, maintenance or other operational reasons.  
  
A Suspended Vendor is automatically Offline.  
  
## 3.6 Suspension  
  
An administrative action preventing a Vendor from trading.  
  
Suspension may take effect immediately or at a scheduled date and time.  
  
Suspension is independent of Trading Preference.  
  
## 3.7 Trading Name  
  
The public name under which a Vendor trades.  
  
Customers interact with the Trading Name rather than the legal organisation.  
  
## 3.8 Legal Operator  
  
The legally recognised organisation responsible for operating the Vendor.  
  
Examples include:  
  
- Sole Trader  
- Limited Company  
- Partnership  
  
## 3.9 Company Name  
  
The registered legal name of the Legal Operator.  
  
This may differ from the Trading Name.  
  
## 3.10 Registration Number  
  
The government-issued registration identifier of the Legal Operator where applicable.  
  
Whether mandatory depends upon the Legal Operator Type.  
  
## 3.11 Contact Details  
  
The information used to communicate with the Vendor.  
  
Examples include:  
  
- Email Address  
- Telephone Number  
  
## 3.12 Address  
  
The physical trading location of the Vendor.  
  
Address data is owned by the Address domain.  
  
The Vendor domain stores only the address reference and any locally required snapshot or reference data.  
  
# 4. Business Concepts  
  
## 4.1 Registration  
  
The creation of a Vendor within the platform.  
  
## 4.2 Activation  
  
Authorisation to begin trading.  
  
## 4.3 Trading  
  
Accepting and fulfilling Customer Orders.  
  
## 4.4 Availability  
  
Whether the Vendor is currently willing to receive new Orders.  
  
Availability is determined by Trading Preference and Vendor Status.  
  
## 4.5 Compliance  
  
The process of verifying that a Vendor satisfies legal and regulatory obligations.  
  
Compliance belongs to the Vendor Compliance domain.  
  
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
| Availability | Whether Orders may currently be accepted |  
| Compliance | Verification of legal obligations |  
  
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

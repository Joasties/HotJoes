# HJ-104 – Vendor Registration Fields Matrix

| Metadata | Value |
|---|---|
| **Document ID** | HJ-104 |
| **Document Title** | Vendor Registration Fields Matrix |
| **Version** | 1.0 |
| **Status** | Approved |
| **Classification** | Requirements |
| **Owner** | Project Architecture |
| **Last Updated** | 22 July 2026 |

## Revision History

| Version | Date | Description |
|---|---|---|
| 1.0 | 22 July 2026 | Approved the Vendor Registration baseline, including field definitions, controlled Trading Location values, regulatory authority fields, assumptions and resolved outstanding decisions. |

## 1. Purpose

This document defines the fields required to register a Vendor during **Epic 1 – Vendor Registration**.

It is intended to provide a single source of truth for:

- User Interface
- API Contract
- Domain Model
- Validation Rules
- Acceptance Criteria

This document intentionally excludes menu management, operational settings and compliance evidence.

## 2. Registration Field Matrix

| # | Field | Type | Required | Validation / Rules | Classification | Notes |
|---:|---|---|---|---|---|---|
| 1 | Trading Name | Text | Yes | 1–160 characters | Registered Information | Public name displayed to customers |
| 2 | Legal Operator Name | Text | Yes | 1–160 characters | Registered Information | Registered legal entity or individual |
| 3 | Legal Operator Type | Lookup | Yes | Controlled list | Registered Information | Sole Trader, Ltd, LLP etc. |
| 4 | Company Registration Number | Text | Conditional | Mandatory where required by Legal Operator Type.<br>Must match UK Companies House registration number format: `^(?:[A-Za-z]{2})?\d{6,8}$`<br>Stored in canonical uppercase format.<br>Validation confirms format only. | Registered Information | Validation depends on Legal Operator Type |
| 5 | Trading Location | Lookup | Yes | Controlled list | Registered Information | Used to determine Compliance Requirements |
| 6 | Opening Hours | Start Time / End Time | Yes | Opening Hours are represented by Start Time and End Time.<br>Validation shall not require Start Time to be earlier than End Time, allowing legitimate overnight trading periods (for example 23:00–05:00). | Registered Information | Used to determine Compliance Requirements |
| 7 | Service Includes Hot Food | Boolean | Yes | — | Registered Information | Indicates whether the Vendor supplies food or drink heated above ambient room temperature.<br>Used together with Opening Hours to determine applicable Compliance Requirements. |
| 8 | Alcohol Service | Boolean | Yes | — | Registered Information | Indicates whether the Vendor supplies alcohol.<br>Used to determine applicable Compliance Requirements. |
| 9 | Contact Name | Text | Yes | 1–100 characters | Registered Information | Primary business contact |
| 10 | Contact Email | Email | Yes | Valid email format | Registered Information | Business correspondence |
| 11 | Contact Telephone | Telephone | Yes | UK telephone validation | Registered Information | Primary contact number |
| 12 | Business Address | Address | Yes | Selected from Address Service | Registered Information | Stored within Vendor domain |
| 13 | Food Registration Authority | Derived | Yes | Derived from the approved Business Address or mobile unit base address. | Registered Information | Provided by the Address Service. Not manually editable by the Vendor. |
| 14 | Primary Trading Authority | Derived / Conditional | Conditional | Required for Trading Location = Stall. | Registered Information | Represents the primary authority responsible for the Vendor's declared trading area. |
| 15 | Website | URL | No | Valid HTTPS URL | Vendor Managed Information | Optional |
| 16 | Business Description | Text | No | Maximum 2,000 characters | Vendor Managed Information | Optional profile information |

### Trading Location Controlled Values

| Value | Description |
|---|---|
| Restaurant | Restaurant, Café, Takeaway or Food Market Hall. A customer-facing permanent premises. |
| Stall | Mobile Food Unit or Market Stall. |
| Kitchen | Dark Kitchen, Ghost Kitchen or Home Kitchen. A non-customer-facing food preparation venue that trades exclusively online. |

## 3. Identity Fields

These fields belong to the Identity capability rather than the Vendor domain.

| Field | Owned By | Notes |
|---|---|---|
| User Account | Identity | Existing authenticated user |
| Password | Identity | Not part of Vendor Registration |
| Email Verification | Identity | Outside Vendor domain |
| Multi-Factor Authentication | Identity | Outside Vendor domain |

## 4. Fields Deferred from Epic 1

The following information is intentionally excluded from Vendor Registration.

| Field | Future Epic |
|---|---|
| Logo | Vendor Profile |
| Banner Image | Vendor Profile |
| Operational Availability | Trading Operations |
| Trading Preference | Trading Operations |
| Cuisine Types | Vendor Profile |
| Menu | Menu Management |
| Product Images | Menu Management |
| Holiday Periods | Trading Operations |
| Delivery Radius | Delivery |
| Collection Options | Ordering |
| VAT Registration Status | Finance |
| Bank Details | Payments |
| Payment Provider Information | Payments |

## 5. Assumptions

The following assumptions have been made for Version 1.0.

1. Registration Sessions are transient and cannot be resumed.
2. Registration Sessions expire automatically and all collected information is discarded.
3. A Vendor is created only after successful submission of a Registration Session.
4. Identity management is outside the Vendor bounded context.
5. Compliance evidence is outside Epic 1.
6. Business Address search, retrieval and validation are provided by a stubbed Address Service during Epic 1.
7. The Vendor Domain stores an approved snapshot of the Business Address.
8. Food Registration Authority is derived from the approved Business Address.
9. Trading Characteristics determine Compliance Requirements.
10. Compliance Requirements are obtained through a stubbed Compliance Provider during Epic 1.
11. Registered Information becomes read-only to the Vendor after successful registration.
12. Registered Information may only be amended by authorised platform operators.
13. A Vendor represents a single trading location during Epic 1.
14. Support for multiple premises is deferred to a future epic.

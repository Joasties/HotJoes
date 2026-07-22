# HJ-104 – Vendor Registration Fields Matrix

| Metadata | Value |
|---|---|
| **Document ID** | HJ-104 |
| **Document Title** | Vendor Registration Fields Matrix |
| **Version** | 0.2 |
| **Status** | Draft |
| **Classification** | Requirements |
| **Owner** | Project Architecture |
| **Last Updated** | 22 July 2026 |

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
| 4 | Company Registration Number | Text | Conditional | Mandatory for applicable operator types | Registered Information | Validation depends on Legal Operator Type |
| 5 | Trading Location | Lookup | Yes | Controlled list | Registered Information | Used to determine Compliance Requirements |
| 6 | Opening Hours | To be confirmed | Yes | Representation to be confirmed | Registered Information | Used to determine Compliance Requirements |
| 7 | Service Includes Hot Food | Boolean | Yes | Hot Food definition to be confirmed | Registered Information | Used to determine Compliance Requirements |
| 8 | Alcohol Service | Boolean | Yes | Alcohol Service definition to be confirmed | Registered Information | Used to determine Compliance Requirements |
| 9 | Contact Name | Text | Yes | 1–100 characters | Registered Information | Primary business contact |
| 10 | Contact Email | Email | Yes | Valid email format | Registered Information | Business correspondence |
| 11 | Contact Telephone | Telephone | Yes | UK telephone validation | Registered Information | Primary contact number |
| 12 | Business Address | Address | Yes | Selected from Address Service | Registered Information | Stored within Vendor domain |
| 13 | Website | URL | No | Valid HTTPS URL | Vendor Managed Information | Optional |
| 14 | Business Description | Text | No | Maximum 2,000 characters | Vendor Managed Information | Optional profile information |

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

The following assumptions have been made for Version 0.2.

1. A Vendor is created only after successful completion of registration.
2. Identity management is outside the Vendor bounded context.
3. Compliance evidence is outside Epic 1.
4. Trading Characteristics determine Compliance Requirements.
5. Compliance Requirements are obtained through a stubbed Compliance Provider during Epic 1.
6. Business Address is obtained through a stubbed Address Service during Epic 1.
7. Registered Information becomes read-only to the Vendor after successful registration.
8. Registered Information may only be amended by authorised platform operators.
9. Every Vendor has exactly one primary trading address during Epic 1.
10. Multiple trading locations are outside the scope of Epic 1.

## 6. Outstanding Decisions

The following items require review before implementation.

1. Confirm Trading Location controlled values.
2. Confirm Opening Hours representation.
3. Confirm Hot Food definition.
4. Confirm Alcohol Service definition.
5. Final Legal Operator Type values.
6. Whether Website should remain in Epic 1.
7. Whether Business Description should remain in Epic 1.
8. UK company number validation rules.
9. Address storage strategy (snapshot vs reference).
10. Future support for multiple premises.

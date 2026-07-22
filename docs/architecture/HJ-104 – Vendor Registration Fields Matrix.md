# HJ-104 – Vendor Registration Fields Matrix

| Metadata | Value |
|---|---|
| **Document ID** | HJ-104 |
| **Document Title** | Vendor Registration Fields Matrix |
| **Version** | 0.1 |
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

| # | Field | Type | Required | Validation / Rules | Editable After Registration | Notes |
|---:|---|---|---|---|---|---|
| 1 | Trading Name | Text | Yes | 1–160 characters | Yes | Public name displayed to customers |
| 2 | Legal Operator Name | Text | Yes | 1–160 characters | Yes | Registered legal entity or individual |
| 3 | Legal Operator Type | Lookup | Yes | Controlled list | Yes | Sole Trader, Ltd, LLP etc. |
| 4 | Company / Registration Number | Text | Conditional | Mandatory for applicable operator types | Yes | Validation depends on Legal Operator Type |
| 5 | Trading Model | Lookup | Yes | Controlled list | Yes | Used to determine activation requirements |
| 6 | Contact Name | Text | Yes | 1–100 characters | Yes | Primary business contact |
| 7 | Contact Email | Email | Yes | Valid email format | Yes | Business correspondence |
| 8 | Contact Telephone | Telephone | Yes | UK telephone validation | Yes | Primary contact number |
| 9 | Business Address | Address | Yes | Selected from Address Service | Yes | Stored within Vendor domain |
| 10 | Website | URL | No | Valid HTTPS URL | Yes | Optional |
| 11 | Business Description | Text | No | Maximum 2,000 characters | Yes | Optional profile information |

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
| Opening Hours | Trading Operations |
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

The following assumptions have been made for Version 0.1.

1. A Vendor is created only after successful completion of registration.
2. Identity management is outside the Vendor bounded context.
3. Compliance evidence is outside Epic 1.
4. Activation requirements are obtained through an interface, initially implemented by a stub provider.
5. Business Address is supplied by the Address domain.
6. Trading Model determines which activation requirements are required.
7. Every Vendor has exactly one primary trading address during Epic 1.
8. Multiple trading locations are outside the scope of Epic 1.

## 6. Outstanding Decisions

The following items require review before implementation.

1. Final Trading Model values.
2. Final Legal Operator Type values.
3. Whether Website should remain in Epic 1.
4. Whether Business Description should remain in Epic 1.
5. UK company number validation rules.
6. Address storage strategy (snapshot vs reference).
7. Future support for multiple premises.

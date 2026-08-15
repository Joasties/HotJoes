# HJ-104 – Vendor Registration Fields Matrix

| Metadata | Value |
|---|---|
| **Document ID** | HJ-104 |
| **Document Title** | Vendor Registration Fields Matrix |
| **Version** | 3.2 |
| **Status** | Approved |
| **Classification** | Requirements |
| **Owner** | Project Architecture |
| **Last Updated** | 13 August 2026 |

## Revision History

| Version | Date | Description |
|---|---|---|
| 1.0 | 22 July 2026 | Approved the Vendor Registration baseline, including field definitions, controlled Trading Location values, regulatory authority fields, assumptions and resolved outstanding decisions. |
| 1.1 | 24 July 2026 | Applied CR-010 to add mandatory Registration Declarations, introduce the Registration Declaration classification and clarify that declarations form part of the authoritative Vendor Registration contract. |
| 2.0 | 24 July 2026 | Consolidated prior revisions into the approved Vendor Registration Fields Matrix. |
| 3.0 | 25 July 2026 | Applied CR-014 to redefine HJ-104 as the authoritative Vendor Registration Fields Matrix. Introduced Business Rules sections, enhanced Notes column with cross-references, updated Purpose, and refined Assumptions into Assumptions and Outstanding Decisions. |
| 3.1 | 27 July 2026 | Applied CR-015, CR-016, CR-019, CR-020 and CR-023 to add Canonical Address identity, clarify Registration Declaration classification and lifecycle, link creation invariants, standardise Legal Operator terminology and remove server-side Registration Session assumptions. |
| 3.2 | 13 August 2026 | Applied CR-034 to remove delivery-slice scope and recast HJ-104 as the enduring Vendor Registration information contract. |

## 1. Purpose

This document is the **authoritative Vendor Registration Fields Matrix**.

It is the single source of truth for the business semantics governing all Vendor Registration information and Registration Declarations. It defines:

- Information classification
- Required, optional and conditional information
- Validation rules
- Conditional business rules
- Derived information
- Information ownership
- Editability
- Information lifecycle
- Traceability to the Vendor Domain Model

The Registration Field Matrix provides a concise executive summary of all registration information. Business rules and classifications are defined once in their designated authoritative sections and are cross-referenced from the matrix.

This document intentionally excludes:

- Workflow and sequence behaviour (documented in HJ-105)
- Aggregate behaviour and domain model structure (documented in HJ-004)
- API contracts and implementation-specific validation (documented in the relevant service contracts and HJ-005)
- Menu management, operational settings and compliance evidence

## 2. Registration Field Matrix

| # | Field | Type | Required | Validation / Rules | Classification | Notes |
|---:|---|---|---|---|---|---|
| 1 | Trading Name | Text | Yes | 1–160 characters | Registered Information | Public name displayed to customers. See §5.2. |
| 2 | Legal Operator Name | Text | Yes | 1–160 characters | Registered Information | Registered legal entity or individual. See §5.2 and the Legal Operator Terminology note below. |
| 3 | Legal Operator Type | Lookup | Yes | Controlled list | Registered Information | Sole Trader, Ltd, LLP etc. See §5.1, §5.2. |
| 4 | Company Registration Number | Text | Conditional | Mandatory where required by Legal Operator Type.<br>Must match UK Companies House registration number format: `^(?:[A-Za-z]{2})?\d{6,8}$`<br>Stored in canonical uppercase format.<br>Validation confirms format only. | Registered Information | Validation depends on Legal Operator Type. See §5.1, §5.2, §5.3. |
| 5 | Trading Location | Lookup | Yes | Controlled list | Registered Information | Used to determine Compliance Requirements. See §5.1, §5.2. |
| 6 | Opening Hours | Start Time / End Time | Yes | Opening Hours are represented by Start Time and End Time.<br>Validation shall not require Start Time to be earlier than End Time, allowing legitimate overnight trading periods (for example 23:00–05:00). | Registered Information | Used to determine Compliance Requirements. See §5.1, §5.2. |
| 7 | Service Includes Hot Food | Boolean | Yes | — | Registered Information | Indicates whether the Vendor supplies food or drink heated above ambient room temperature.<br>Used together with Opening Hours to determine applicable Compliance Requirements. See §5.1. |
| 8 | Alcohol Service | Boolean | Yes | — | Registered Information | Indicates whether the Vendor supplies alcohol.<br>Used to determine applicable Compliance Requirements. See §5.1. |
| 9 | Contact Name | Text | Yes | 1–100 characters | Registered Information | Primary business contact. See §5.2. |
| 10 | Contact Email | Email | Yes | Valid email format | Registered Information | Business correspondence. See §5.2. |
| 11 | Contact Telephone | Telephone | Yes | UK telephone validation | Registered Information | Primary contact number. See §5.2. |
| 12 | Business Address | Address selection | Yes | An approved Address Resolution reference shall identify the selected address. Client-supplied snapshot or authority values are not accepted. | Registered Information | The Vendor supplies search or selection inputs only. The Address Domain supplies the Canonical Address Identifier, Business Address Snapshot and regulatory-authority information. See §5.4, §5.5, ADR-006, HJ-004 and HJ-105. |
| 13 | Canonical Address Identifier | Derived | Yes | Supplied by the Address Domain from the approved Address Resolution. Not client-editable. | Registered Information | Supplied exclusively by the Address Domain; not entered, edited or overridden by the Vendor. Persisted with the Business Address Snapshot. See §5.4, §5.5, ADR-006 and HJ-004. |
| 14 | Food Registration Authority | Derived | Yes | Derived from the approved Business Address or mobile unit base address. | Registered Information | Provided by the Address Service. Not manually editable by the Vendor. See §5.4, §5.5. |
| 15 | Primary Trading Authority | Derived / Conditional | Conditional | Required for Trading Location = Stall. | Registered Information | Represents the primary authority responsible for the Vendor's declared trading area. See §5.1, §5.4. |
| 16 | Website | URL | No | Valid HTTPS URL | Vendor Managed Information | Optional. See §5.2, §5.5. |
| 17 | Business Description | Text | No | Maximum 2,000 characters | Vendor Managed Information | Optional. See §5.2, §5.5. |
| 18 | Authorised to Register Business | Boolean | Yes | Must be explicitly accepted before registration may be submitted. | Registration Declaration | Confirms that the applicant is authorised to register the business. See Registration Declaration Classification and §5.5. |
| 19 | Information Accurate | Boolean | Yes | Must be explicitly accepted before registration may be submitted. | Registration Declaration | Confirms that the submitted information is accurate. See Registration Declaration Classification and §5.5. |
| 20 | Accept HotJoes Platform Terms | Boolean | Yes | Must be explicitly accepted before registration may be submitted. | Registration Declaration | Confirms acceptance of the applicable HotJoes platform terms and conditions. See Registration Declaration Classification and §5.5. |

### Legal Operator Terminology

**Legal Operator Name** is the authoritative business term defined in **HJ-003 – Ubiquitous Language Guide**.

It represents the registered legal name of the individual or legal entity responsible for operating the Vendor, regardless of Legal Operator Type.

The historical term **Company Name** is retained only for compatibility with earlier documentation and is not the preferred architectural terminology.

### Registration Declaration Classification

This subsection is the authoritative definition of Registration Declarations within HJ-104.

Registration Declarations:

- exist solely as transient business inputs to the **Register Vendor** command;
- are neither Registered Information nor Vendor Managed Information;
- are not persisted on the Vendor aggregate;
- do not become part of Vendor business state;
- never appear in Vendor domain events;
- never appear in Vendor integration events;
- have no lifecycle after completion of the registration transaction; and
- may be retained for audit purposes only outside the Vendor Domain.

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

## 4. Information Outside Vendor Registration

The following information is intentionally excluded from Vendor Registration.

| Information | Related / Owning Capability |
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

## 5. Business Rules

Business rules governing Vendor Registration information are defined in this section. Rules are recorded exactly once. The Registration Field Matrix provides an executive summary and cross-references these sections via the Notes column.

### 5.1 Conditional Business Rules

1. **Company Registration Number applicability**
    | Legal Operator Type | Legal Name Required | Trading Name Required | Company Registration Number Required |
    |------------------------------|:------------------:|:---------------------:|:------------------------------------:|
    | Sole Trader | Yes | Yes | No |
    | General Partnership | Yes | Yes | No |
    | Limited Company | Yes | Yes | Yes |
    | Limited Liability Partnership | Yes | Yes | Yes |
    | Charitable Community Group | Yes | Yes | No |
    | Charitable Incorporated Organisation | Yes | Yes | Yes |

2. **Primary Trading Authority**
   Primary Trading Authority is required when Trading Location = Stall. It is not required for Restaurant or Kitchen.

3. **Compliance Requirements determination**
   Trading Characteristics (Trading Location, Opening Hours, Service Includes Hot Food, Alcohol Service) are used together to determine applicable Compliance Requirements.
   - Service Includes Hot Food indicates whether the Vendor supplies food or drink heated above ambient room temperature.
   - Alcohol Service indicates whether the Vendor supplies alcohol.
   - Opening Hours (including legitimate overnight periods) contribute to the determination of Compliance Requirements.

4. **Legal Operator Type dependencies**
   Legal Operator Type is selected from a controlled list and drives the conditional requirement for Company Registration Number.

The field matrix defines the authoritative registration information requirements. The Vendor aggregate enforces the corresponding Primary Trading Authority and Company Registration Number creation invariants defined in **HJ-004 Section 8**.

### 5.2 Validation Rules

Validation rules represent business constraints. Implementation-specific validation remains within HJ-005.

| Field | Validation Rule |
|---|---|
| Trading Name | 1–160 characters |
| Legal Operator Name | 1–160 characters |
| Legal Operator Type | Must be a value from the controlled list |
| Company Registration Number | Must match UK Companies House registration number format: `^(?:[A-Za-z]{2})?\d{6,8}$`. Validation confirms format only. |
| Trading Location | Must be a value from the controlled list (Restaurant, Stall, Kitchen) |
| Opening Hours | Represented by Start Time and End Time. Validation shall not require Start Time to be earlier than End Time, allowing legitimate overnight trading periods (for example 23:00–05:00). |
| Contact Name | 1–100 characters |
| Contact Email | Valid email format |
| Contact Telephone | UK telephone validation |
| Business Address | Must be selected from the Address Service |
| Website | Valid HTTPS URL (when supplied) |
| Business Description | Maximum 2,000 characters (when supplied) |
| Registration Declarations (Authorised to Register Business, Information Accurate, Accept HotJoes Platform Terms) | Must be explicitly accepted (true) before registration may be submitted |

### 5.3 Canonicalisation Rules

1. **Company Registration Number**
   When supplied, Company Registration Number is stored in canonical uppercase format before persistence.

### 5.4 Derived Information

1. **Canonical Address Identifier**
   The Canonical Address Identifier is supplied exclusively by the Address Domain from the approved Address Resolution. It is not entered, edited or overridden by the Vendor. It forms part of Registered Information and is persisted with the Business Address Snapshot.

2. **Business Address Snapshot**
   The Vendor Domain stores the immutable Business Address Snapshot supplied by the Address Domain for the approved Address Resolution. Snapshot content is not supplied, edited or derived by the Vendor or caller.

3. **Food Registration Authority**
   Food Registration Authority is derived from the approved Business Address (or mobile unit base address). It is provided by the Address Service and is not manually editable by the Vendor.

4. **Primary Trading Authority**
   Primary Trading Authority represents the primary authority responsible for the Vendor's declared trading area. It is derived / conditional and is required when Trading Location = Stall.

The Canonical Address Identifier, Business Address Snapshot and applicable regulatory-authority information originate exclusively from the Address Domain. Address ownership is defined by **ADR-006** and reinforced by **HJ-004**; the Address Resolution workflow is defined in **HJ-105**.

### 5.5 Information Lifecycle

1. **Registered Information**
   Registered Information becomes read-only to the Vendor after successful registration.
   Registered Information may only be amended by authorised platform operators.

2. **Vendor Managed Information**
   Vendor Managed Information (Website, Business Description) may be updated through future Vendor Management services.

3. **Registration Declarations**
   Registration Declarations are transient inputs to **Register Vendor** and have no lifecycle after the registration transaction completes. They are not persisted on the Vendor aggregate or exposed through domain or integration events. Any audit retention occurs outside the Vendor Domain. The authoritative classification is defined in **Section 2 – Registration Declaration Classification**.

4. **Canonical Address Identifier and Business Address Snapshot**
   The Canonical Address Identifier and immutable Business Address Snapshot form part of Registered Information after successful registration. Both are supplied by the Address Domain and persisted together; neither is entered or edited by the Vendor.

5. **Derived fields**
   Food Registration Authority and Primary Trading Authority (where applicable) are derived at registration and form part of Registered Information thereafter.

### 5.6 Traceability to Domain Model

| Business Rule Area | Architectural Source |
|---|---|
| Vendor entity, Registered Information and Vendor Managed Information classifications | HJ-004 Vendor Domain Models |
| Registration Session ownership and Register Vendor request processing | HJ-003 §3.5 / HJ-105 |
| Address ownership, Canonical Address Identifier and Business Address Snapshot | ADR-006 / HJ-004 / HJ-105 |
| Conditional registration rules and corresponding aggregate creation invariants | HJ-004 §8 |
| Compliance Requirements determination from Trading Characteristics | Compliance capability / related Architectural Decision Records |
| Identity separation | Identity capability boundary (outside Vendor bounded context) |

## 6. Assumptions and Outstanding Decisions

### Registration Preconditions

Vendor Registration accepts a complete, self-contained registration request containing all client-authored information and the approved Address Resolution reference required to validate and create a Vendor.

The registration request shall satisfy:

- all mandatory client-supplied registration fields;
- all field validation rules;
- all Registration Declaration requirements; and
- all business-rule preconditions defined by HJ-104.

Address-owned values—including the Canonical Address Identifier, Business Address Snapshot and applicable regulatory-authority information—are obtained directly from the Address Domain using the approved Address Resolution reference and are not supplied by the client.

No server-side Registration Session is required, retrieved or examined during Vendor registration.

### Remaining Assumptions and Outstanding Decisions

The following enduring assumptions and boundaries apply. Rules elevated to approved business rules are defined in Section 5.

1. Vendor Registration operates upon a complete, self-contained **Register Vendor** request.
2. Registration Session ownership belongs exclusively to the client application or Backend-for-Frontend.
3. The Vendor Registration service neither owns nor retrieves Registration Sessions; Registration Session persistence, expiry and lifecycle management are outside the Vendor service boundary.
4. Identity management is outside the Vendor bounded context.
5. Compliance evidence is outside the Vendor Registration information contract.
6. Business Address search, retrieval and validation are owned by the Address capability and consumed through the approved Address contract.
7. Compliance Requirements are determined by the Compliance capability using the information made available through the approved downstream integration contract.

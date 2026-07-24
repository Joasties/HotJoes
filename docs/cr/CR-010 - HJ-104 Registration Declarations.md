# CR-010 -- HJ-104 Registration Declarations

  -----------------------------------------------------------------------
  Property                                  Value
  ----------------------------------------- -----------------------------
  **Change Request ID**                     CR-010

  **Title**                                 HJ-104 Registration
                                            Declarations

  **Status**                                Proposed

  **Priority**                              Medium

  **Author**                                Project Architecture

  **Date**                                  24 July 2026

  **Target Document**                       HJ-104 -- Vendor Registration
                                            Fields Matrix v1.0
  -----------------------------------------------------------------------

## 1. Summary

Amend **HJ-104 -- Vendor Registration Fields Matrix** to include the
mandatory Vendor Registration declarations already defined within
**HJ-004 -- Vendor Domain Models**.

This change resolves an inconsistency identified during an independent
AI review.

HJ-104 states that it is the single source of truth for Vendor
Registration fields used by the User Interface, API Contract, Domain
Model, Validation Rules and Acceptance Criteria. However, it currently
omits the mandatory registration declarations required to complete
Vendor Registration.

This change aligns HJ-104 with the approved Vendor Domain Model while
preserving the existing architectural decisions.

## 2. Background

HJ-004 defines three mandatory registration declarations that must be
supplied before a Vendor may be created:

-   confirmation that the applicant is authorised to register the
    business;
-   confirmation that the submitted information is accurate;
-   acceptance of the applicable HotJoes platform terms.

These declarations form part of the Vendor Registration business
contract but are absent from the Vendor Registration Fields Matrix.

As a result, the current API contract cannot be fully derived from
HJ-104 despite that document defining itself as the authoritative source
for Vendor Registration fields.

## 3. Change Required

### 3.1 Registration Field Matrix

Append the following fields immediately after **Business Description**.

  ---------------------------------------------------------------------------------------------
  \#           Field         Type      Required   Validation /   Classification   Notes
                                                  Rules                           
  ------------ ------------- --------- ---------- -------------- ---------------- -------------
  17           Authorised to Boolean   Yes        Must be        Registration     Confirms that
               Register                           explicitly     Declaration      the applicant
               Business                           accepted                        is authorised
                                                  before                          to register
                                                  registration                    the business.
                                                  may be                          
                                                  submitted.                      

  18           Information   Boolean   Yes        Must be        Registration     Confirms that
               Accurate                           explicitly     Declaration      the submitted
                                                  accepted                        information
                                                  before                          is accurate.
                                                  registration                    
                                                  may be                          
                                                  submitted.                      

  19           Accept        Boolean   Yes        Must be        Registration     Confirms
               HotJoes                            explicitly     Declaration      acceptance of
               Platform                           accepted                        the
               Terms                              before                          applicable
                                                  registration                    HotJoes
                                                  may be                          platform
                                                  submitted.                      terms and
                                                                                  conditions.
  ---------------------------------------------------------------------------------------------

### 3.2 Classification

Introduce a new field classification:

**Registration Declaration**

Registration Declarations are mandatory inputs to the **Register
Vendor** business operation.

They are not part of:

-   Registered Information
-   Vendor Managed Information

They do not describe the Vendor itself and therefore do not become part
of the Vendor's registered business identity.

### 3.3 Purpose Section

Amend the Purpose section where HJ-104 states that it is the single
source of truth for Vendor Registration fields.

Clarify that this includes **Registration Declarations** required by the
Vendor Registration business contract.

### 3.4 Assumptions

No changes required.

## 4. Architectural Impact

This change introduces no new business capability.

It:

-   resolves a documentation inconsistency;
-   aligns HJ-104 with HJ-004;
-   preserves the existing Vendor Domain Model;
-   preserves the approved Vendor Registration workflow;
-   improves traceability from business requirements to API contract and
    acceptance criteria.

No changes are required to:

-   Vendor lifecycle;
-   Vendor aggregate;
-   bounded context ownership;
-   events;
-   commands;
-   architectural principles.

## 5. Rationale

The registration declarations are mandatory business inputs to the
Vendor Registration use case.

They are legal declarations made by the applicant rather than attributes
of the Vendor.

Separating them into their own **Registration Declaration**
classification maintains a clear distinction between:

-   information that defines the Vendor; and
-   declarations required to complete the legal act of registration.

This preserves the integrity of the Vendor Domain Model while ensuring
that HJ-104 remains the authoritative source for all information
required to complete Vendor Registration.

## 6. Acceptance Criteria

The change shall be considered complete when:

1.  HJ-104 contains the three mandatory Registration Declaration fields.
2.  A new **Registration Declaration** classification has been
    introduced.
3.  The Purpose section reflects that Registration Declarations form
    part of the Vendor Registration contract.
4.  HJ-104 can be used as the complete authoritative source for deriving
    the Vendor Registration API Contract.
5.  HJ-104 and HJ-004 are fully consistent regarding mandatory Vendor
    Registration information.

# CR-004 – HJ-004 Vendor Domain Models

## Document

**HJ-004 – Vendor Domain Models**

## Version

Update to **Version 1.2**

## Objective

Replace Trading Model with Trading Characteristics and refine the Vendor Registration model to reflect the latest business decisions.

## Changes

### 1. Replace Trading Model

Remove the Trading Model concept.

Introduce **Trading Characteristics**.

Trading Characteristics are composed of:

- Trading Location
- Opening Hours
- Service Includes Hot Food
- Alcohol Service

### 2. Update Vendor Aggregate

Replace the Trading Model attribute with Trading Characteristics.

Ensure the aggregate reflects the composite structure.

### 3. Compliance Requirements

Update the model to state that Compliance Requirements are derived from:

- Trading Characteristics
- Legal Operator Type
- Business Address

The Vendor Domain requests Compliance Requirements through an abstraction.

The Compliance Domain implementation is stubbed during Epic 1.

### 4. Address Domain

Clarify that Business Address is obtained through an Address Service abstraction.

The Address Domain implementation is stubbed during Epic 1.

### 5. Registered Information

Introduce Registered Information as a distinct business concept.

Registered Information comprises:

- Trading Name
- Legal Operator Name
- Legal Operator Type
- Company Registration Number
- Contact Name
- Contact Email
- Contact Telephone
- Business Address
- Trading Characteristics

### 6. Vendor Managed Information

Introduce Vendor Managed Information.

Initially this consists of:

- Website
- Business Description

### 7. Pending Activation Behaviour

Clarify that:

Once Vendor Registration has been successfully submitted and the Vendor enters PendingActivation:

- Registered Information becomes read-only to the Vendor.
- Registered Information may only be amended by authorised platform operators.
- Vendor Managed Information remains editable where applicable.

### 8. Scheduled Suspension

Clarify that Scheduled Suspension is **not** part of the Vendor aggregate.

A scheduling capability is responsible for invoking the existing Vendor suspension use case at the scheduled time.

The Vendor aggregate records only the resulting lifecycle transition.

### 9. Preserve Aggregate Boundaries

Do not introduce:

- Registration Amendment
- Pending Registration Changes
- Proposed Values

These capabilities are outside the scope of Epic 1.

### 10. Update Revision History

Record Version 1.2 changes.

## Preservation Requirements

- Preserve all existing diagrams where possible.
- Update only those diagrams affected by these changes.
- Preserve aggregate boundaries.
- Preserve lifecycle semantics.

# CR-005 – HJ-104 Vendor Registration Fields Matrix

## Document

**HJ-104 – Vendor Registration Fields Matrix**

## Version

Update to **Version 0.2**

## Objective

Refine the Registration Field Matrix to align with the latest Vendor Registration decisions.

## Changes

### 1. Replace Trading Model

Remove the Trading Model field.

Introduce the following Registered Information fields.

- Trading Location
- Opening Hours
- Service Includes Hot Food
- Alcohol Service

### 2. Replace Editable After Registration

Remove the **Editable After Registration** column.

Replace it with:

**Classification**

Permitted values:

- Registered Information
- Vendor Managed Information

### 3. Classify Fields

Registered Information:

- Trading Name
- Legal Operator Name
- Legal Operator Type
- Company Registration Number
- Contact Name
- Contact Email
- Contact Telephone
- Business Address
- Trading Location
- Opening Hours
- Service Includes Hot Food
- Alcohol Service

Vendor Managed Information:

- Website
- Business Description

### 4. Update Assumptions

Replace the existing assumptions relating to Trading Model.

Add:

- Trading Characteristics determine Compliance Requirements.
- Compliance Requirements are obtained through a stubbed Compliance Provider during Epic 1.
- Business Address is obtained through a stubbed Address Service during Epic 1.
- Registered Information becomes read-only to the Vendor after successful registration.
- Registered Information may only be amended by authorised platform operators.

### 5. Outstanding Decisions

Remove:

- Final Trading Model values.

Replace with:

- Confirm Trading Location controlled values.
- Confirm Opening Hours representation.
- Confirm Hot Food definition.
- Confirm Alcohol Service definition.

### 6. Preserve Existing Structure

- Preserve document numbering.
- Preserve formatting.
- Preserve all existing content unless explicitly changed above.

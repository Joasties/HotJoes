# CR-056 - Define Positional Address Line Translation in ADR-006

## Status

Proposed — 17 August 2026

## Target

ADR-006 v1.2 → v1.3 Accepted.

## Purpose and Approved Baseline

Correct an incomplete propagation of the previously agreed Address-to-Vendor snapshot mapping.

ADR-006 v1.2 correctly establishes that the Address Domain owns authoritative Address information and that a Vendor Application adapter translates the Address-owned result into Vendor-owned values. However, its statement that Address source lines 1–4 are normalised into no more than three snapshot address lines omits the approved positional treatment of source Line 1 and leaves the four-line case ambiguous for executable tests and adapter implementation.

The approved mapping retains source Line 1 separately as `RecipientOrOrganisationName`; source Lines 2–4 then map one-to-one to the three physical snapshot address lines.

## Authorised Changes

Amend only ADR-006 to replace the ambiguous source-line translation wording in **Address Resolution and Consumption Boundary** with an explicit positional rule:

- Address source Line 1 maps to optional `RecipientOrOrganisationName`;
- Address source Line 2 maps to required `BusinessAddressSnapshot.AddressLine1`;
- Address source Line 3 maps to optional `BusinessAddressSnapshot.AddressLine2`;
- Address source Line 4 maps to optional `BusinessAddressSnapshot.AddressLine3`;
- Post Town maps directly to `BusinessAddressSnapshot.PostTown`;
- Postcode maps directly to `BusinessAddressSnapshot.Postcode`; and
- optional County maps directly to `BusinessAddressSnapshot.County`.

State explicitly that:

- no source-line concatenation, compression, shifting or reordering occurs;
- blank or absent optional source fields remain absent in their corresponding optional snapshot fields;
- the Vendor Application adapter performs positional type translation only;
- the Vendor Domain does not interpret, normalise, derive or invent Address-owned content;
- `RecipientOrOrganisationName` remains optional;
- `RecipientOrOrganisationName` is not compared with Legal Operator Name or Trading Name; and
- `RecipientOrOrganisationName` does not affect registration validity, Vendor identity or Address authority.

Add an ADR-006 v1.3 revision-history entry identifying CR-056 and describing the positional mapping clarification.

Update the ADR metadata `Last Updated` value to 17 August 2026. Preserve status as Accepted.

## Proposed Replacement Wording

Replace:

> The translated `BusinessAddressSnapshot` contains `AddressLine1`, optional `AddressLine2`, optional `AddressLine3`, `PostTown`, `Postcode`, optional `County`, and optional `RecipientOrOrganisationName`. Address source lines 1–4 are normalized into no more than three snapshot address lines. `RecipientOrOrganisationName` is retained when supplied and is not compared with Legal Operator Name for registration validity.

with:

> The translated `BusinessAddressSnapshot` contains required `AddressLine1`, optional `AddressLine2`, optional `AddressLine3`, required `PostTown`, required `Postcode`, optional `County`, and optional `RecipientOrOrganisationName`.
>
> Translation is positional. Address source Line 1 maps to `RecipientOrOrganisationName`; source Lines 2, 3 and 4 map respectively to `AddressLine1`, `AddressLine2` and `AddressLine3`. Post Town, Postcode and optional County map directly to their corresponding snapshot fields. Blank or absent optional source fields remain absent in the corresponding optional snapshot fields. No source-line concatenation, compression, shifting or reordering occurs.
>
> The Vendor Application adapter performs this positional type translation without interpreting, normalising, deriving or inventing Address-owned content. `RecipientOrOrganisationName` is retained when supplied, is not compared with Legal Operator Name or Trading Name, and does not affect registration validity, Vendor identity or Address authority.

## Non-goals

- No change to Address Domain ownership or authority.
- No change to the `BusinessAddressSnapshot` field set.
- No new Address search, selection, validation or normalisation behaviour.
- No line concatenation or compression algorithm.
- No name-matching diagnostic or registration rule.
- No change to Canonical Address Identifier or regulatory-authority semantics.
- No Address transport, serialization or API representation decision.
- No change to Address Resolution reference lifetime, binding or failure behaviour.
- No production adapter or test implementation.
- No amendment to HJ-010, HJ-012, HJ-004, HJ-104, HJ-106, HJ-107 or HJ-013 through this Change Request.

> This Change Request amends only `ADR-006`. It does not amend any other controlled artefact.

## Downstream Assessment

Following approval and application of this Change Request:

- assess HJ-010 and HJ-012; no material concern or Approved Approach change is expected;
- amend HJ-004 through its own Change Request to remove ambiguous foreign-source mapping wording and reference ADR-006;
- propagate the clarified mapping into HJ-104 and HJ-106 through their controlled artefact processes; and
- regenerate HJ-107 and HJ-013 from their updated authoritative inputs rather than amending either generated catalogue directly.

## Verification and Completion

The amendment is complete when ADR-006:

- contains the exact positional mapping for every Address source line and target snapshot field;
- preserves Address Domain ownership and the Vendor trust boundary;
- contains no Vendor-side normalisation or name-matching rule;
- removes the ambiguous four-source-lines-to-three-address-lines wording;
- records CR-056 in revision history; and
- remains Accepted as ADR-006 v1.3.

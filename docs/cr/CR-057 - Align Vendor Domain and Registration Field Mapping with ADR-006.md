# CR-057 - Align Vendor Domain and Registration Field Mapping with ADR-006

## Status

Approved — 17 August 2026

## Targets

- HJ-004 v2.4 → v2.5 Approved.
- HJ-104 v3.3 → v3.4 Approved.

## Purpose and Approved Baseline

Propagate the positional Address-line translation clarified by ADR-006 v1.3 under CR-056 into the Vendor Domain model and authoritative Vendor Registration information contract.

The approved mapping is:

- Address source Line 1 maps to optional `RecipientOrOrganisationName`;
- Address source Line 2 maps to required `BusinessAddressSnapshot.AddressLine1`;
- Address source Line 3 maps to optional `BusinessAddressSnapshot.AddressLine2`;
- Address source Line 4 maps to optional `BusinessAddressSnapshot.AddressLine3`;
- Post Town maps directly to `BusinessAddressSnapshot.PostTown`;
- Postcode maps directly to `BusinessAddressSnapshot.Postcode`; and
- optional County maps directly to `BusinessAddressSnapshot.County`.

No source-line concatenation, compression, shifting or reordering occurs.

## Authorised Changes to HJ-004

Amend only the `BusinessAddressSnapshot` subsection and document-control metadata required for publication.

Replace:

> The application adapter translates Address source lines 1–4 into no more than three snapshot address lines. `RecipientOrOrganisationName` is retained when supplied but is not compared with Legal Operator Name as a registration rule.

with:

> The application adapter supplies these Vendor-owned snapshot values according to the positional translation defined by ADR-006. The Vendor Domain does not interpret, shift, concatenate or normalise Address source lines. `RecipientOrOrganisationName` remains optional, is not compared with Legal Operator Name or Trading Name, and does not affect registration validity.

Additionally:

- advance HJ-004 from v2.4 to v2.5;
- add an HJ-004 v2.5 revision-history entry identifying CR-057;
- retain `Approved` status; and
- set `Last Updated` to 17 August 2026.

## Authorised Changes to HJ-104

Amend only the introductory Address translation paragraph in §5.4 Derived Information and document-control metadata required for publication.

Replace:

> The Business Address Snapshot is translated from Address source data into `AddressLine1`, optional `AddressLine2`, optional `AddressLine3`, `PostTown`, `Postcode`, optional `County` and optional `RecipientOrOrganisationName`. Address source lines 1–4 map into no more than three snapshot address lines. `RecipientOrOrganisationName` remains optional and does not participate in Legal Operator Name validation.

with:

> The Business Address Snapshot is translated positionally from Address source data. Address source Line 1 maps to optional `RecipientOrOrganisationName`; source Lines 2, 3 and 4 map respectively to required `AddressLine1`, optional `AddressLine2` and optional `AddressLine3`. Post Town, Postcode and optional County map directly to their corresponding snapshot fields. No source-line concatenation, compression, shifting or reordering occurs. `RecipientOrOrganisationName` is not compared with Legal Operator Name or Trading Name and does not affect registration validity.

Additionally:

- advance HJ-104 from v3.3 to v3.4;
- add an HJ-104 v3.4 revision-history entry identifying CR-057;
- retain `Approved` status; and
- set `Last Updated` to 17 August 2026.

## Non-goals

- No change to the `BusinessAddressSnapshot` field set.
- No change to Address Domain ownership, authority or validation responsibility.
- No new Vendor Domain knowledge of the foreign Address representation.
- No source-line concatenation, compression, shifting or reordering algorithm.
- No comparison of `RecipientOrOrganisationName` with Legal Operator Name or Trading Name.
- No new registration-validity or Vendor-identity rule.
- No change to Canonical Address Identifier or regulatory-authority semantics.
- No Address search, selection, reference, failure, retry, transport or serialization change.
- No production adapter or executable-test implementation.
- No amendment to ADR-006, HJ-010, HJ-012, HJ-106, HJ-107 or HJ-013 through this Change Request.

> This Change Request amends only `HJ-004` and `HJ-104`. It does not amend any other controlled artefact.

## Downstream Assessment

Following approval and application of CR-057:

- assess HJ-010 and HJ-012; no material concern or Approved Approach change is expected;
- propagate the clarified consumed Address mapping into HJ-106 through its controlled artefact process; and
- regenerate HJ-107 and HJ-013 from their updated authoritative inputs rather than amending either generated catalogue directly.

## Verification and Completion

CR-057 is complete when HJ-004 and HJ-104:

- contain their authorised replacement wording;
- agree with the positional mapping in ADR-006 v1.3;
- preserve Address ownership and the Vendor trust boundary;
- contain no line-combination or name-matching rule;
- contain their respective CR-057 revision-history entries; and
- retain `Approved` status at v2.5 and v3.4 respectively.

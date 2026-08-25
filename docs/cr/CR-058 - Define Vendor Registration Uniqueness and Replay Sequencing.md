# CR-058 - Define Vendor Registration Uniqueness and Replay Sequencing

## Status

Approved — 19 August 2026

## Targets

- HJ-104 v3.4 → v3.5 Approved.
- HJ-105 v3.5 → v3.6 Approved.

## Purpose and Approved Baseline

Propagate the approved CON-013 Vendor uniqueness and replay decision from the synchronized HJ-010 v1.5 and HJ-012 v1.5 architecture baseline into the authoritative Vendor Registration information contract and behavioural sequence.

Epic 1 Vendor uniqueness is identified by:

- Trading Name compared after trimming and without regard to case;
- Legal Operator Name compared after trimming and without regard to case; and
- the authoritative `CanonicalAddressId` returned by the Address capability.

The registered Trading Name and Legal Operator Name remain the authoritative display values. Their normalized comparison forms are derived only for uniqueness evaluation and do not replace those registered values.

For an existing composite identity:

- semantically equivalent registration information returns the original committed successful result without repeating any business effect; and
- materially different registration information returns `IdempotencyConflict`, performs no update and leaves the existing Vendor unchanged.

Vendor updates require a separate future administration operation.

## Authorised Changes to HJ-104

Amend HJ-104 to:

- identify Trading Name, Legal Operator Name and `CanonicalAddressId` as the three components of the Epic 1 Vendor uniqueness identity;
- define trimmed, case-insensitive comparison for the two name components without changing their registered display values;
- define semantic registration equivalence as comparison of all materially relevant registration information after its approved canonicalisation;
- exclude transient Registration Declarations, the opaque Address Resolution reference, server-generated values and technical metadata from semantic equivalence;
- state the equivalent-replay, conflict and no-update outcomes; and
- retain the separate ownership and lifecycle of Address-derived Registered Information.

Additionally:

- advance HJ-104 from v3.4 to v3.5;
- add an HJ-104 v3.5 revision-history entry identifying CR-058;
- retain `Approved` status; and
- set `Last Updated` to 19 August 2026.

## Authorised Changes to HJ-105

Amend HJ-105 to:

- remove the generic caller-supplied idempotency-identity alternative;
- validate the complete client-authored request and resolve the permanent Address reference before evaluating Vendor identity;
- derive the approved composite identity only after the Address capability returns `CanonicalAddressId`;
- derive the semantic registration fingerprint from the complete materially relevant information after approved canonicalisation;
- distinguish first processing, equivalent replay and `IdempotencyConflict` before aggregate creation;
- return the original committed successful result for an equivalent replay without repeating any business effect; and
- return `IdempotencyConflict` for materially different information without updating the Vendor or recording any new business effect.

Additionally:

- advance HJ-105 from v3.5 to v3.6;
- add an HJ-105 v3.6 revision-history entry identifying CR-058;
- retain `Approved` status; and
- set `Last Updated` to 19 August 2026.

## Non-goals

- No database schema, index or constraint design.
- No concurrency-control mechanism.
- No persisted replay-record or retention design.
- No transaction-boundary or publication implementation.
- No exact fingerprint serialization or storage representation.
- No Vendor update operation.
- No change to Address ownership or the permanent Address Resolution reference contract.
- No direct amendment to HJ-106, HJ-107 or HJ-013.

CON-014, CON-015, CON-016 and CON-028 remain authoritative for concurrency, persisted replay, transaction and database-enforcement mechanics respectively.

> This Change Request amends only HJ-104 and HJ-105. It does not amend any other controlled artefact.

## Downstream Assessment

Following approval and application of CR-058:

- regenerate HJ-106 from the revised approved domain artefacts using PR-002;
- regenerate HJ-107 from the revised approved service contract using PR-004; and
- regenerate HJ-013 from the resulting approved architecture and behavioural baselines using PR-005.

HJ-004 and ADR-008 remain impact-assessment candidates because CON-013 lists them in scope; neither is amended by this focused Change Request.

## Verification and Completion

CR-058 is complete when HJ-104 and HJ-105:

- define the same three-component Vendor uniqueness identity;
- preserve registered name values separately from normalized identity comparison;
- evaluate final identity only after authoritative Address resolution supplies `CanonicalAddressId`;
- define equivalent replay and materially different conflict consistently;
- prohibit update and repeated business effects through the conflict and replay paths;
- avoid deciding the deferred concurrency, persistence, transaction and database mechanisms; and
- retain `Approved` status at v3.5 and v3.6 respectively.

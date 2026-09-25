# CR-080 - Amend ADR-008 for Join Community Idempotency and Reliable Publication

| Property | Value |
|---|---|
| Change Request | CR-080 |
| Status | Approved |
| Date | 21 September 2026 |
| Owner | Project Architecture |
| Affected artefact | ADR-008 Idempotent Operations and Reliable Event Publication |

## 1. Reason

ADR-008 defines idempotent operations, authoritative replay and atomic outbox publication principally for Register Vendor. Approved CON-046 introduces a second authoritative create-only operation with a distinct natural replay boundary, controlled conflict semantics, concurrency guarantee and Community-owned transaction.

Because this changes the substantive idempotency and reliable-publication decision, PR-007 requires an artefact-specific Change Request.

## 2. Decision Requested

Approve ADR-008 v1.6 so that `JoinCommunity` uses Vendor ID as its unique replay and conflict boundary. A first valid request creates exactly one immutable Community Participation record and one immutable outbox message in a single Community-owned transaction. An equivalent request returns the original committed success; a different Contact Preference returns a controlled conflict; concurrent requests establish exactly one record and at most one logical Integration Event.

The original authoritative result contains Community Participation ID, Vendor ID, recorded Contact Preference and original Joined At. Replay returns those original values. Validation, Vendor-verification, conflict and unavailable outcomes create no Community record or outbox work.

## 3. Controlled Change

This Change Request amends only `ADR-008 - Idempotent Operations and Reliable Event Publication.md`. It does not amend any other controlled artefact.

The accompanying ADR-008 v1.6 candidate shall:

- add the Join Community idempotency boundary based on unique Vendor ID;
- define equivalent replay and different-preference conflict behaviour;
- require atomic resolution of concurrent first requests;
- require atomic Community Participation and outbox persistence;
- preserve the original result and immutable serialized event for replay and publication retry;
- distinguish Community-owned authoritative state, outbox work and consumer receipts; and
- retain Community Participation and consumer duplicate-detection evidence for the approved Epic 1 lifetime while allowing established bounded cleanup of eligible published outbox rows.

## 4. Explicit Non-goals

CR-080 does not:

- alter Register Vendor idempotency or semantic fingerprinting;
- include Community state in Vendor persistence;
- define amendment, withdrawal, deletion or expiry operations;
- treat identifiers as credentials, consent evidence or authorisation;
- define HTTP adaptation or runtime topology; or
- implement source code or tests.

## 5. Verification

Acceptance requires evidence that ADR-008 v1.6:

- permits exactly one Community Participation record and at most one logical event per Vendor;
- returns the original committed result for equivalent replay;
- rejects a different preference without new state or publication work;
- leaves neither Community state nor outbox work after pre-commit failure;
- preserves immutable event identity and bytes across publication retry; and
- introduces no behaviour beyond approved CON-046.

## 6. Completion Standard

CR-080 is complete when the architectural decision-maker approves this Change Request and the accompanying complete ADR-008 v1.6 candidate, after which the human may apply both through the PR-007 cohort order.

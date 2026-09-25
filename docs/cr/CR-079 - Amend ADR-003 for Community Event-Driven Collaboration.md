# CR-079 - Amend ADR-003 for Community Event-Driven Collaboration

| Property | Value |
|---|---|
| Change Request | CR-079 |
| Status | Approved |
| Date | 21 September 2026 |
| Owner | Project Architecture |
| Affected artefact | ADR-003 Event-Driven Collaboration |

## 1. Reason

ADR-003 establishes event-driven collaboration and the Epic 1 reliable-publication profile but currently describes only the Vendor registration publication and Compliance consumption flow in detail. Approved CON-046 introduces a distinct Community-owned `CommunityParticipationRecorded` Integration Event, an independently executable Community consumer and deterministic Community stub processing.

Because this expands the enduring cross-bounded-context collaboration decision, PR-007 requires an artefact-specific Change Request.

## 2. Decision Requested

Approve ADR-003 v1.4 so that a committed first `JoinCommunity` operation publishes one Community-owned `CommunityParticipationRecorded` Integration Event v1 through the established transactional-outbox and RabbitMQ delivery pattern.

The event communicates the completed Community fact and contains only Community Participation ID, Vendor ID, Joined At and Contact Preference within the established immutable versioned envelope. It contains no Primary Contact details, Vendor Registration information, Communication Consent, recipient resolution or delivery instruction.

An independently executable Community consumer validates and idempotently processes the Community event through a deterministic Community-owned stub. Its durable receipt is processing evidence only and performs no communication delivery.

## 3. Controlled Change

This Change Request amends only `ADR-003 - Event-Driven Collaboration.md`. It does not amend any other controlled artefact.

The accompanying ADR-003 v1.4 candidate shall:

- add `CommunityParticipationRecorded` v1 as a Community-owned fact contract separate from `VendorRegistered` and Compliance contracts;
- preserve immutable serialize-once outbox content and unchanged retry delivery;
- establish the independently executable Community consumer and Community-owned deterministic stub;
- apply Event ID idempotency, durable processing evidence and integrity-failure treatment;
- retain bounded retry and dead-letter behaviour for unsupported, malformed or exhausted processing; and
- state explicitly that Community processing neither records Communication Consent nor sends a message.

## 4. Explicit Non-goals

CR-079 does not:

- reuse or amend `VendorRegistered`;
- make Community a subscriber to Vendor registration events;
- include Primary Contact details or communication-provider data in the event;
- define the Join Community HTTP operation or Community persistence mapping;
- introduce communication delivery; or
- implement source code or tests.

## 5. Verification

Acceptance requires evidence that ADR-003 v1.4:

- treats Community Participation as the already-completed publishing fact;
- keeps Community, Vendor and Compliance event contracts distinct;
- preserves at-least-once delivery with idempotent consumption;
- distinguishes authoritative Community state from consumer processing evidence; and
- introduces no behaviour beyond approved CON-046.

## 6. Completion Standard

CR-079 is complete when the architectural decision-maker approves this Change Request and the accompanying complete ADR-003 v1.4 candidate, after which the human may apply both through the PR-007 cohort order.

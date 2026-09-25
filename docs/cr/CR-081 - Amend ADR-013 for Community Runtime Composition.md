# CR-081 - Amend ADR-013 for Community Runtime Composition

| Property | Value |
|---|---|
| Change Request | CR-081 |
| Status | Approved |
| Date | 21 September 2026 |
| Owner | Project Architecture |
| Affected artefact | ADR-013 Epic 1 Runtime and Deployment Composition |

## 1. Reason

ADR-013 defines the Epic 1 executable composition but does not include the Community Application capability, Community persistence and outbox ownership, trusted Edge route, relay responsibility, independently executable Community consumer or durable Community processing receipts approved through CON-046.

Because this changes the controlled Epic 1 runtime topology and readiness responsibilities, PR-007 requires an artefact-specific Change Request.

## 2. Decision Requested

Approve ADR-013 v1.1 so that the Epic 1 runtime includes the Community-owned Join Community capability behind the trusted allowlisted Edge, Community-owned persistence and migration objects within the physically shared PostgreSQL runtime, transactional outbox publication through the established relay pattern, RabbitMQ routing, and an independently executable Community consumer with durable receipt storage and a deterministic Community stub.

Logical Community ownership remains independent even where API hosting, PostgreSQL, relay infrastructure or deployment composition is physically shared. Community API and consumer responsibilities have no direct public ingress. Missing or invalid mandatory Community configuration prevents the affected responsibility from becoming ready.

## 3. Controlled Change

This Change Request amends only `ADR-013 - Epic 1 Runtime and Deployment Composition.md`. It does not amend any other controlled artefact.

The accompanying ADR-013 v1.1 candidate shall:

- add the trusted Edge route for `POST /community-participations`;
- add the Community Application capability and minimal Vendor-verification collaboration;
- add Community-owned schema, repository, transaction, outbox and migration responsibilities in PostgreSQL;
- include Community event relay/routing and the independently executable Community consumer;
- include durable Community receipt storage and deterministic stub processing;
- extend health, readiness, secret-injection and fail-closed configuration treatment to Community responsibilities; and
- preserve controlled local or disposable non-production exposure without introducing public production enrolment.

## 4. Explicit Non-goals

CR-081 does not:

- introduce a public Community API or direct consumer ingress;
- introduce production identity, caller authentication or Vendor-level authorisation;
- add Communication Consent, recipient resolution or provider delivery;
- require a separate physical database, API host or relay deployment where logical ownership remains enforceable;
- change the Azure reference choice; or
- implement source code or tests.

## 5. Verification

Acceptance requires evidence that ADR-013 v1.1:

- makes every new Community runtime responsibility explicit and health-gated;
- preserves logical ownership under physical co-location;
- exposes Community only through the trusted allowlisted Edge route;
- keeps event consumption independently executable and durable;
- fails closed when mandatory Community configuration is unavailable; and
- introduces no behaviour beyond approved CON-046.

## 6. Completion Standard

CR-081 is complete when the architectural decision-maker approves this Change Request and the accompanying complete ADR-013 v1.1 candidate, after which the human may apply both through the PR-007 cohort order.

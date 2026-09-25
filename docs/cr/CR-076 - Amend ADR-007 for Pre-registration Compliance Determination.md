# CR-076 - Amend ADR-007 for Pre-registration Compliance Determination

| Property | Value |
|---|---|
| Change Request | CR-076 |
| Status | Approved |
| Date | 19 September 2026 |
| Owner | Project Architecture |
| Affected artefact | ADR-007 Vendor Compliance as a Separate Bounded Context |

## 1. Reason

ADR-007 currently describes Vendor Compliance only as the separate bounded context responsible for post-registration Compliance Requirements and records the Epic 1 asynchronous Compliance consumer stub. Approved CON-047 additionally requires a synchronous, side-effect-free pre-registration determination boundary while preserving that separation and the existing asynchronous post-registration flow.

Because this changes the substantive collaboration decision recorded by ADR-007, PR-007 requires an artefact-specific Change Request rather than routine propagation alone.

## 2. Decision Requested

Approve ADR-007 v1.2 so that Vendor Application may request a complete immutable Compliance Determination through a consumed Compliance port before Vendor Registration submission.

Compliance owns the applicability policy, determination meaning, canonical ordering and Rule Set Version. Vendor Application validates the controlling draft and resolves authoritative Address information but owns no applicability rule.

Epic 1 implements the port with one in-process, side-effect-free, non-persistent and versioned deterministic Compliance stub adapter. The result remains transient Web-client Registration Session state and creates no Compliance Requirement, Licence Details, evidence, persistence or Compliance Domain state.

The synchronous pre-registration boundary does not replace the asynchronous post-registration collaboration. After successful registration, Compliance independently creates and manages actual lifecycle-bearing Compliance Requirements from authoritative registered facts using its then-active policy.

## 3. Controlled Change

This Change Request amends only `ADR-007 - Vendor Compliance as a Separate Bounded Context.md`. It does not amend any other controlled artefact.

The accompanying ADR-007 v1.2 candidate:

- adds the pre-registration Compliance Determination boundary;
- constrains the Epic 1 adapter to an in-process, replaceable and non-persistent stub;
- preserves Compliance ownership of regulatory policy;
- preserves Vendor Domain independence from regulatory decision logic; and
- preserves asynchronous post-registration Compliance Requirement processing.

## 4. Explicit Non-goals

CR-076 does not:

- define an HTTP route or wire representation;
- introduce a Compliance deployable, database, aggregate or requirement lifecycle in Epic 1;
- add Licence Details, evidence or document capture;
- change RegisterVendor, Vendor persistence, fingerprinting, retrieval or VendorRegistered;
- change ADR-013 deployment topology; or
- implement source code or tests.

## 5. Verification

Acceptance requires evidence that ADR-007 v1.2:

- expresses both the synchronous pre-registration and asynchronous post-registration collaborations without conflating their results;
- keeps applicability rules outside Vendor and the Web client;
- keeps the pre-registration result transient and side-effect-free;
- leaves actual Compliance Requirements to post-registration Compliance processing; and
- introduces no persistence, publication or topology semantics absent from approved CON-047.

## 6. Completion Standard

CR-076 is complete when the architectural decision-maker approves this Change Request and the accompanying complete ADR-007 v1.2 candidate, after which the human may apply both through the stated PR-007 cohort order.

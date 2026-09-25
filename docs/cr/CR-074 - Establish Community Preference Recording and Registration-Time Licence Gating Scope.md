# CR-074 - Establish Community Preference Recording and Registration-Time Licence Gating Scope

| Property | Value |
|---|---|
| Change Request | CR-074 |
| Status | Approved |
| Date | 18 September 2026 |
| Owner | Project Architecture |
| Affected artefact | HJ-011 Epic 1 Vendor Registration Implementation Scope |

## 1. Reason

The implemented Epic 1 journey has exposed two requirements that are visible in the approved interaction but are not yet authorised as complete system behaviour.

First, the post-registration community page permits the user to select Email, SMS or WhatsApp as a contact preference and to choose whether to remain involved. Treating those selections as browser-only state would allow the interface to imply that a choice had been recorded when no authoritative system capability had accepted it.

Second, Step 4 collects Licence Details but does not yet have an approved registration-time rule for determining when those details are mandatory and preventing progression when required information is absent. The Vendor model already identifies the registration inputs upon which Food Business Registration, Street Trading Licence, Late Night Refreshment Licence and alcohol-licensing applicability depend, while real Compliance determination and licence validation remain outside Epic 1.

HJ-011 must therefore establish both requirements as Epic 1 scope before new architectural concerns can be introduced and resolved through Decision Mode.

## 2. Decision Requested

Approve HJ-011 v2.15 with the following two outcome-level requirements.

### 2.1 Post-registration community participation

After definitive Vendor Registration success, the optional community interaction shall genuinely record the user's submitted contact preference and explicit community-participation choice through an authoritative system operation.

The interaction shall remain separate from the Vendor Registration request, Registration Session and registration outcome. Refusal, omission, cancellation, abandonment or failure of the optional operation shall not alter, roll back or cast doubt upon the completed Vendor Registration. The interface shall not claim that a choice has been saved until an authoritative successful outcome has been received.

The available contact-preference choices in the Epic 1 interaction are Email, SMS and WhatsApp. The community-participation choice is explicit and begins unselected. This scope does not authorise sending any communication or introduce a production Email, SMS or WhatsApp provider.

### 2.2 Registration-time Licence Details gating

Before the user may progress beyond Step 4, the journey shall determine whether registration-time Licence Details are required from the approved registration information and shall require the applicable information to be present and structurally complete.

The determination shall remain consistent with the approved applicability dependencies: Food Business Registration is always required; Street Trading Licence applicability depends upon Trading Location and Business Address; Late Night Refreshment Licence applicability depends upon Weekly Opening Hours and Service Includes Hot Food; and alcohol-licensing applicability depends upon Alcohol Service.

Client interaction shall support timely disclosure and correction, but authoritative server-side validation shall prevent an incomplete request from being accepted. Capturing Licence Details does not establish that a licence exists, is authentic, is current or is valid, and does not perform Compliance assessment. Those meanings remain outside this scope amendment.

## 3. Controlled Change

Approve the accompanying complete HJ-011 v2.15 candidate as the amended Epic 1 implementation scope.

No other controlled artefact is changed by this CR. Approval establishes the source requirements needed to introduce and resolve the corresponding architectural concerns; it does not itself select their architecture.

## 4. Required Follow-up

After approval and application:

1. add separate working-register concerns for community participation and contact-preference ownership, and for registration-time Licence Details capture and gating;
2. introduce those concerns into HJ-010 through PR-009;
3. resolve the Licence Details concern first through Decision Mode because it affects the active Step 4 registration path;
4. propagate the approved Licence Details decision through the Ubiquitous Language, Vendor model, registration fields and service contract, Application command and validation, HTTP contract, fingerprinting, persistence, unreleased integration-event contract, Compliance input, Web journey and test catalogues as applicable;
5. resolve the community-participation concern through Decision Mode; and
6. propagate its approved ownership, consent, lifecycle, operation, persistence, contract and verification decisions before presenting a recorded-choice confirmation page.

## 5. Explicit Non-Decisions

This CR does not decide:

- which bounded context or application capability owns community participation or contact preference;
- whether community participation is modelled as an aggregate, entity, value object, profile or another concept;
- consent evidence, withdrawal, amendment, retention, audit or communication-delivery behaviour;
- API routes, commands, results, JSON members, persistence structures or integration events for community participation;
- the detailed registration-time Licence Details data model;
- whether licence information belongs to Vendor Registered Information, Vendor Managed Information, Compliance input or another approved boundary;
- the source and representation of licence-requirement rules;
- semantic-fingerprint, replay, persistence, migration or `VendorRegistered` contract treatment;
- external licence verification or Compliance assessment; or
- the detailed UI control and modal design.

Those matters require explicit concern resolution and downstream propagation. No implementation inference may be made from the current browser-only fields or provisional UI text.

## 6. Acceptance

CR-074 is accepted when the human approves this Change Request and the accompanying HJ-011 v2.15 candidate as the authoritative Epic 1 scope source for:

- genuinely recording optional post-registration contact preference and community participation without changing the completed Vendor Registration; and
- registration-time Licence Details requirement determination and progression/submission gating without claiming licence validation or Compliance assessment.

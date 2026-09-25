# CR-082 - Amend PR-004 for Join Community

| Property | Value |
|---|---|
| Change Request | CR-082 |
| Status | Approved |
| Date | 22 September 2026 |
| Owner | Project Architecture |
| Affected artefact | PR-004 Generate Test Catalogue from Approved Service Contract |

## 1. Reason

Approved HJ-106 v2.2 adds `JoinCommunity` as the fourth Epic 1 business operation alongside `DetermineRequiredLicenceTypes`, `RegisterVendor` and `RetrieveRegisteredVendor`.

PR-004 requires every normative rule in the current Approved HJ-106 Part A contract to receive catalogue coverage, be identified as non-testable or be recorded as blocked. However, its operation scope explicitly permits derivation only for the existing three operations and repeats that restriction in its coverage, failure, traceability, completeness and review controls.

Running PR-004 unchanged would therefore create a direct contradiction: including Join Community obligations would breach its explicit operation scope, while omitting them would breach its completeness rule and weaken the approved HJ-106 v2.2 contract.

PR-004 must be amended before HJ-107 is regenerated.

## 2. Decision Requested

Approve the amendment of PR-004 so that controlled HJ-107 generation and regeneration includes all four currently approved Epic 1 business operations:

- `DetermineRequiredLicenceTypes`;
- `RegisterVendor`;
- `RetrieveRegisteredVendor`; and
- `JoinCommunity`.

`JoinCommunity` shall be treated as a separate post-registration Community Application operation derived from HJ-106 Part A. Its inclusion does not make Community Participation or Contact Preference part of Vendor Registration, the Vendor aggregate, Primary Contact, Communication Consent or message delivery.

## 3. Controlled Amendments to PR-004

### 3.1 Operation scope

Amend the Scope, Test Basis, Review Standard and every equivalent operation-limiting statement so that PR-004 derives behavioural obligations for all four approved operations.

Retain `RegisterVendor` as the primary Vendor creation operation. Do not infer any Community amendment, withdrawal, deletion, expiry, communication-consent, recipient-resolution or message-delivery operation.

### 3.2 Required Join Community coverage

Add explicit required coverage for:

- the exact request containing one successfully registered `VendorId` and one Contact Preference;
- Email, SMS and WhatsApp as the complete Epic 1 Contact Preference set;
- invocation implying affirmative Community Participation without a client-controlled participation Boolean;
- exclusion of Primary Contact information, Communication Consent, recipient resolution and delivery instructions;
- Community Application ownership of the operation, result, immutable record, persistence and event;
- minimal Vendor-owned verification of successful Vendor Registration without exposing the Vendor aggregate, Registered Vendor Details or Primary Contact;
- `VendorNotFound` when successful registration cannot be confirmed;
- first successful creation of one immutable Community Participation containing CommunityParticipationId, VendorId, Contact Preference and original JoinedAt;
- one Community-owned transaction atomically recording the participation and immutable outbox message;
- unique VendorId replay and conflict semantics;
- equivalent repeated requests returning `CommunityParticipationAlreadyRecorded` with the original committed result and no additional effect;
- different-preference replay returning `CommunityParticipationConflict` with no amendment or additional effect;
- concurrent first requests creating exactly one record and at most one logical Integration Event;
- `VendorVerificationUnavailable` and `CommunityPersistenceUnavailable` as controlled retryable outcomes;
- the complete closed transport-independent outcome set from HJ-106 v2.2 §4.14.4;
- `CommunityParticipationRecorded` Integration Event version 1 with the exact approved envelope and payload;
- serialize-once immutable publication and preservation across retry;
- durable Community consumer receipt using EventId as the idempotency key;
- equivalent redelivery suppression and EventId/content conflict treatment;
- the Community-owned deterministic stub creating no communication-delivery effect;
- retention of the authoritative record and original result for at least the associated Vendor lifetime; and
- every approved Epic 1 exclusion, including amendment, withdrawal, deletion, expiry, consent, recipient resolution and Email, SMS or WhatsApp delivery.

### 3.3 Join Community failures and prohibited effects

Require behavioural coverage for:

- `RequestValidationFailure` for invalid VendorId or Contact Preference;
- `VendorNotFound`;
- `CommunityParticipationConflict`;
- `VendorVerificationUnavailable`; and
- `CommunityPersistenceUnavailable`.

Every failure obligation shall verify its controlled outcome, retry guidance where defined and the absence of Community Participation, outbox work and Integration Events. Uncertain retry shall be covered through the approved equivalent-replay guarantee.

### 3.4 Catalogue structure and stable identifiers

Add a dedicated subsection to HJ-107 Section 5 named:

```text
## 5.2 Join Community
```

Place it after Required Licence Types Determination and renumber subsequent Section 5 subsections without changing their retained obligations or stable `VR-*` identifiers.

Add the HJ-107-owned `VR-COMMUNITY-*` area for genuinely new Join Community obligations. Add `COMMUNITY` to the suggested area-code list. Existing `VR-*` identifiers remain governed by the previous controlled HJ-107 as the sole stable-ID baseline and shall not be renumbered merely because subsection order changes.

### 3.5 API Contract coverage

Derive API Contract obligations from approved HJ-106 v2.2 Part B for:

- `POST /community-participations`;
- the exact lower-camel-case `vendorId` and `contactPreference` request;
- Email, SMS and WhatsApp wire values `email`, `sms` and `whatsApp`;
- compatible unknown-member handling and controlled missing, null or invalid required-member validation;
- exclusion of a participation Boolean and contact destination;
- original `201 Created` semantics for both first success and equivalent replay;
- the exact successful representation and stable Community Participation `Location`;
- the shared safe error envelope;
- `400`, `404`, `409` and `503` controlled mappings approved in HJ-106 v2.2; and
- no automatic API retry or HTTP-owned Community behaviour.

Treat these as normative API Contract obligations because HJ-106 v2.2, CON-046 and the applicable HJ-010/HJ-012 technical baseline are Approved. Do not reinterpret the HTTP representation as Community Domain behaviour.

### 3.6 Web behaviour boundary

Derive behavioural obligations only where HJ-106 v2.2 carries them into the service contract. The detailed Angular presentation contract remains governed by the applicable UI architecture and HJ-013 rather than being invented by PR-004.

Preserve the authoritative distinction that `Not Now` performs no Community request and absence is not stored as a negative participation record. Do not infer an amendment or withdrawal capability from later navigation.

### 3.7 Traceability, completeness and review

Amend the Coverage Summary, Requirement-to-Test Traceability Matrix, Completeness Analysis, Assumptions and Open Questions, Review Checklist and Regeneration Reconciliation so they explicitly include the fourth operation.

Every normative HJ-106 v2.2 Part A Join Community requirement shall map to one or more `VR-*` obligations, an explicit non-testable finding or a controlled blocked dependency. The HJ-107/HJ-013 responsibility boundary remains unchanged.

## 4. Preserved PR-004 Controls

The amendment does not change:

- HJ-106 Part A as the primary normative behavioural source;
- HJ-106 Part B treatment for API Contract obligations;
- HJ-006 Test Classification and Test Level mappings;
- the previous HJ-107 as the sole stable-ID baseline for `VR-*` identifiers;
- prohibition of `AI-*` identifiers and version-specific HJ-013 dependencies;
- the requirement to preserve, amend, add and retire Test IDs explicitly;
- Priority and Dependency Status classifications;
- the HJ-107/HJ-013 behavioural-versus-implementation responsibility boundary;
- prohibited-outcome verification;
- proposal-only generation and human review controls under PR-008; or
- the prohibition on executable test-code generation.

## 5. Explicit Non-decisions

CR-082 does not:

- change HJ-106 v2.2 or any approved business behaviour;
- add another business operation;
- make Community Participation part of Vendor Registration or Vendor state;
- define amendment, withdrawal, deletion or expiry;
- define Communication Consent, lawful basis, recipient resolution or message delivery;
- expose Primary Contact information to Community;
- select test classes, frameworks, fixtures, mocks or implementation techniques;
- generate HJ-107, HJ-013 or executable tests; or
- authorise repository modification.

## 6. Required Follow-up

After CR-082 and the amended PR-004 are approved and applied:

1. run PR-004 against Approved HJ-106 v2.2 and the previous controlled HJ-107 v2.1 stable-ID baseline;
2. generate the complete HJ-107 candidate and `VR-*` reconciliation;
3. approve and apply HJ-107 and its HJ-000 synchronization;
4. run PR-005 as the separate downstream iteration for HJ-013; and
5. do not begin CON-046 implementation until the controlled catalogue sequence is complete.

## 7. Acceptance

CR-082 is accepted when the human approves this Change Request as authority to amend PR-004 exactly as described above, without changing approved HJ-106 v2.2 or the established HJ-107/HJ-013 responsibility boundary.

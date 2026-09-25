# CR-078 - Amend ADR-002 for the Community Bounded Context

| Property | Value |
|---|---|
| Change Request | CR-078 |
| Status | Approved |
| Date | 21 September 2026 |
| Owner | Project Architecture |
| Affected artefact | ADR-002 Business Capabilities and Bounded Contexts |

## 1. Reason

ADR-002 establishes bounded-context and authoritative-data ownership but does not yet identify the Community bounded context approved through CON-046. The approved decision makes Community Participation and Contact Preference Community-owned concepts, relates them to a successfully registered Vendor by Vendor ID, and prohibits Community from redefining or copying the Vendor-owned Primary Contact.

Because this adds a bounded context and its authoritative ownership and collaboration boundaries to ADR-002, PR-007 requires an artefact-specific Change Request.

## 2. Decision Requested

Approve ADR-002 v1.2 so that Community is recognised as the bounded context responsible for affirmative Community Participation and Contact Preference.

Community owns its Application capability, immutable participation record, persistence, Integration Event contract and asynchronous processing evidence. Vendor remains authoritative for successful Vendor registration and Primary Contact information. Community relates its record to Vendor through Vendor ID only and verifies successful registration through a minimal Vendor-owned capability without acquiring Vendor data ownership.

Contact Preference expresses only the preferred Email, SMS or WhatsApp channel for possible future community-related contact. It is not Communication Consent, a subscription, a lawful basis, a delivery instruction or delivery evidence.

## 3. Controlled Change

This Change Request amends only `ADR-002 - Business Capabilities and Bounded Contexts.md`. It does not amend any other controlled artefact.

The accompanying ADR-002 v1.2 candidate shall:

- add Community as a dedicated bounded context and Application capability;
- assign authoritative ownership of Community Participation and Contact Preference to Community;
- preserve Vendor ownership of successful-registration status and Primary Contact information;
- define Vendor ID as the cross-context association and prohibit Community from copying Primary Contact details into Community-owned identity;
- distinguish Community Participation and Contact Preference from Communication Consent and communication delivery; and
- preserve physical co-location as an implementation choice that does not transfer logical ownership.

## 4. Explicit Non-goals

CR-078 does not:

- add Community Participation or Contact Preference to the Vendor aggregate or Vendor Registration;
- define HTTP, persistence, Integration Event, broker or runtime mechanics;
- define amendment, withdrawal, recipient resolution, Communication Consent or message delivery;
- change the Compliance bounded context; or
- implement source code or tests.

## 5. Verification

Acceptance requires evidence that ADR-002 v1.2:

- gives each affected concept exactly one authoritative owner;
- preserves Vendor authority without exposing the Vendor aggregate or Primary Contact to Community;
- expresses the permitted minimal verification relationship;
- does not reinterpret Contact Preference as consent or authority to communicate; and
- introduces no behaviour beyond approved CON-046.

## 6. Completion Standard

CR-078 is complete when the architectural decision-maker approves this Change Request and the accompanying complete ADR-002 v1.2 candidate, after which the human may apply both through the PR-007 cohort order.

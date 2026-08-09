# CR-024 – Register Vendor Idempotency Boundary and Reliable Publication Clarification

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-024 |
| **Title** | Register Vendor Idempotency Boundary and Reliable Publication Clarification |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Critical |
| **Affected Documents** | ADR-008 – Idempotent Operations and Reliable Event Publication |

---

# 1. Background

Subsequent architectural refinement has clarified the responsibilities of the Vendor Registration capability, the ownership of Registration Sessions and the distinction between internal Domain processing and published Integration Events.

While ADR-008 already establishes the principles of idempotent operations and reliable event publication, the document does not yet explicitly define the business idempotency boundary for **Register Vendor**, nor does it identify Register Vendor as an operation that cannot be made naturally idempotent through simple state-setting.

Independent reviews of HJ-105 and HJ-106 also identified ambiguity regarding duplicate registration handling, the relationship between Registration Session and Register Vendor processing, and the distinction between internal Domain Events and published Integration Events.

These principles belong in ADR-008 as the authoritative architectural decision governing idempotency and reliable publication.

This Change Request consolidates the ADR-008 requirements that originated in **CR-017** and **CR-023**.

It does **not** replace the HJ-105 or cross-artefact changes contained within those Change Requests.

Following completion of this Change Request, ADR-008 shall become the authoritative source for idempotency and reliable-publication principles referenced by HJ-105 and any future registration workflow documentation.

---

# 2. Objectives

The revised ADR shall:

- identify Register Vendor as an operation that is not naturally idempotent;
- require an explicit idempotency identity or an equivalent uniqueness constraint;
- define the idempotency boundary as the complete **RegisterVendor** request;
- explicitly exclude Registration Session state from that boundary;
- define the mandatory business outcome of duplicate successful submissions;
- extend testing and enforcement guidance for retries and concurrent requests;
- clarify the distinction between internal Domain Events and published Integration Events;
- clarify the relationship between completed business facts and reliable publication;
- reinforce reliable publication principles without prescribing implementation technology; and
- update ADR metadata, revision history and related-document statuses.

---

# 3. Required Changes

## 3.1 Document Metadata

Update the document metadata as appropriate to reflect the revised version.

Update:

- Version;
- Last Updated;
- Revision History; and
- Related Documents.

Ensure Related Document statuses are consistent with the current architectural baseline.

---

## 3.2 Register Vendor Idempotency

Introduce a dedicated subsection describing Register Vendor idempotency.

State that:

- Register Vendor is **not naturally idempotent** because successful execution creates new business state rather than merely setting existing state.
- Consequently, Register Vendor requires an explicit idempotency safeguard.
- The safeguard may consist of:
  - an explicit idempotency identity; or
  - an equivalent uniqueness constraint.

The ADR shall define the architectural requirement without prescribing how the safeguard is implemented.

---

## 3.3 Idempotency Boundary

Introduce a dedicated subsection defining the architectural idempotency boundary.

State that:

- the idempotency boundary is the complete **RegisterVendor** request;
- duplicate processing applies only to the complete registration request submitted to the Vendor Registration capability;
- Registration Session state is explicitly outside the idempotency boundary;
- Registration Session ownership is an interaction concern and has no influence on server-side idempotency behaviour.

Clarify that the Vendor Registration capability is intentionally independent of how the registration request was assembled.

---

## 3.4 Duplicate Submission Behaviour

Define the mandatory business behaviour for duplicate successful submissions.

State that:

> Where a request carries the same idempotency identity and is semantically identical to a previously successful **RegisterVendor** request—or satisfies the equivalent approved uniqueness constraint—processing shall:
>
> - return the original successful outcome;
> - create no additional Vendor;
> - record no additional **VendorRegistered** Domain Event or completed business fact;
> - create no additional publication or outbox record;
> - publish no additional **VendorRegistered** Integration Event; and
> - initiate no additional Pending Activation Process.

State that this behaviour is mandatory regardless of implementation.

---

## 3.5 Testing Expectations

Extend the ADR guidance covering verification of idempotent behaviour.

Require architectural verification that Register Vendor behaves correctly when processing:

- client retries;
- network retries;
- concurrent duplicate requests;
- repeated successful submissions; and
- duplicate message delivery scenarios where applicable.

Testing shall additionally verify that duplicate processing:

- records no additional **VendorRegistered** Domain Event or completed business fact;
- creates no additional publication or outbox record;
- publishes no additional **VendorRegistered** Integration Event;
- initiates no additional Pending Activation Process; and
- produces the original successful business outcome.

The focus shall remain on observable architectural behaviour rather than implementation techniques.

---

## 3.6 Enforcement Expectations

Extend the architectural enforcement guidance.

Require implementation governance to verify that duplicate successful processing:

- cannot create duplicate Vendors;
- cannot record duplicate **VendorRegistered** Domain Events or completed business facts;
- cannot create duplicate publication or outbox records;
- cannot publish duplicate **VendorRegistered** Integration Events;
- cannot initiate duplicate Pending Activation Processes; and
- continues to satisfy the approved idempotency guarantees following future changes.

No implementation technology shall be mandated.

---

## 3.7 Reliable Publication Clarification

Expand the ADR to distinguish clearly between:

- internal Domain Events; and
- published Integration Events.

Clarify that:

- Domain Events represent internal business facts used within the Vendor Domain;
- Integration Events are explicitly published contracts intended for consumption by other business capabilities;
- reliable publication requirements apply to Integration Events;
- reliable publication does not alter Domain behaviour;
- publication concerns remain separate from business decision-making.

Additionally require ADR-008 to state that:

- once a business state change has completed, aggregate persistence and durable recording of the corresponding publication work shall occur atomically;
- reliable publication may produce an Integration Event derived from the completed business fact;
- the internal Domain Event and published Integration Event address different architectural concerns;
- the internal Domain Event and published Integration Event are not required to have identical representations or payloads;
- publication retries shall not repeat the original business operation.

The ADR shall make it clear that reliable publication derives from the completed business fact without implying that the Domain Event itself is necessarily the persisted publication record or outbox message.

Retain the existing architectural principles governing reliable publication.

---

## 3.8 Scope Clarification

Add an editorial note clarifying the relationship between this ADR and the registration workflow documentation.

State that:

- ADR-008 defines the architectural principles governing idempotency and reliable publication;
- HJ-105 applies those principles to the Vendor Registration workflow;
- workflow documents shall remain consistent with ADR-008;
- future regeneration of HJ-105 shall use ADR-008 as the authoritative architectural source for these principles.

---

# 4. Explicit Non-Requirements

This Change Request intentionally does **not** prescribe:

- idempotency-key format;
- uniqueness-token format;
- storage technology;
- persistence mechanism;
- retention duration;
- payload-comparison implementation;
- hashing strategy;
- outbox technology;
- event transport technology;
- serialization format;
- messaging infrastructure; or
- implementation framework.

These remain implementation decisions provided the mandatory architectural behaviour defined by ADR-008 is preserved.

---

# 5. Relationship to Existing Change Requests

This Change Request consolidates the ADR-008 requirements that originated in:

- **CR-017 – Register Vendor Idempotency Identity**; and
- **CR-023 – Registration Session Outside the Vendor Service Boundary**.

It does **not** replace:

- the HJ-105 changes defined by those Change Requests;
- any cross-document alignment work contained within those Change Requests; or
- any requirements relating to HJ-003, HJ-104 or HJ-106.

Those Change Requests continue to govern the affected artefacts.

Following completion of this Change Request, ADR-008 becomes the authoritative architectural source for idempotency and reliable-publication principles used by HJ-105 and any future registration workflow documentation.

---

# 6. Editorial Principles

- Preserve all existing architectural decisions unless explicitly changed.
- No business capability shall change.
- No Vendor lifecycle shall change.
- No Domain boundaries shall change.
- No event sequencing shall change.
- Strengthen architectural clarity without prescribing implementation.
- Clearly distinguish architectural principles from implementation conventions.
- Clearly distinguish completed business facts, internal Domain Events and published Integration Events.
- Ensure all guidance remains technology-neutral.
- Maintain consistency with the Architectural Principles and Business Capability Ownership ADRs.

---

# 7. Expected Outcome

Following this change:

- ADR-008 explicitly identifies Register Vendor as an operation that is not naturally idempotent.
- The architectural idempotency boundary is unambiguously defined as the complete **RegisterVendor** request.
- Registration Session is explicitly excluded from that boundary.
- Mandatory duplicate-submission behaviour explicitly prohibits duplicate business facts, duplicate publication work and duplicate Integration Events.
- Architectural expectations for retries and concurrent duplicate requests include both internal recording and external publication.
- Reliable publication is defined as an atomic transition from completed business state to durable publication work.
- Internal Domain Events and published Integration Events are clearly distinguished while their architectural relationship is explicitly defined.
- Publication retries are explicitly separated from business-operation execution.
- ADR metadata, revision history and related-document statuses are brought up to date.
- ADR-008 becomes the definitive architectural source governing idempotency and reliable publication, supporting consistent regeneration and maintenance of HJ-105 and related architectural artefacts.

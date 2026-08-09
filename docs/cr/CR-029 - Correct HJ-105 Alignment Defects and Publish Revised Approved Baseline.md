# CR-029 – Correct HJ-105 Alignment Defects and Publish Revised Approved Baseline

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-029 |
| **Title** | Correct HJ-105 Alignment Defects and Publish Revised Approved Baseline |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Medium |
| **Affected Document** | HJ-105 – Vendor Registration Sequence Diagram |

---

# 1. Background

HJ-105 is currently an Approved architectural artefact and provides the authoritative behavioural model for Epic 1 Vendor Registration. :contentReference[oaicite:0]{index=0}

Subsequent refinement of the architectural baseline—particularly ADR-008 and the regenerated HJ-106 Service Contract—identified two remaining alignment defects within HJ-105.

These are not architectural design defects; they are documentation alignment issues caused by the evolution of the approved upstream architecture after HJ-105 Version 3.1 was produced.

Leaving known inconsistencies within an Approved upstream artefact weakens traceability and creates ambiguity for downstream artefacts, particularly HJ-107.

This Change Request updates HJ-105 to align it with the approved architecture without changing the underlying business behaviour.

---

# 2. Objectives

This Change Request shall:

- add the controlled idempotency-conflict behaviour defined by ADR-008;
- demonstrate that the conflict path produces no business side effects;
- update obsolete ADR-008 traceability references;
- preserve all existing successful, failure and replay behaviour;
- publish a new Approved revision of HJ-105.

---

# 3. Scope

This Change Request applies only to HJ-105.

It updates:

- behavioural sequence diagrams;
- behavioural notes;
- traceability references;
- document metadata;
- revision history.

It shall not change:

- Vendor Registration behaviour;
- Vendor lifecycle;
- Address ownership;
- Registration Session ownership;
- aggregate invariants;
- Domain Event semantics;
- Integration Event semantics;
- Pending Activation behaviour; or
- Registered Vendor retrieval.

---

# 4. Required Changes

## 4.1 Add Controlled Idempotency Conflict Branch

Update the idempotency sequence (Section 9) to include an additional behavioural branch immediately after the existing successful replay branch.

The new branch shall model:

> Same idempotency identity (or equivalent approved uniqueness constraint) associated with registration information that is **not semantically identical** to the previously successful request.

The sequence shall clearly show that the application returns a controlled idempotency-conflict outcome.

---

## 4.2 Define Mandatory Business Outcome

The new branch shall explicitly show that idempotency conflict processing:

- returns a controlled conflict outcome;
- creates no Vendor;
- modifies no existing Vendor;
- records no completed business fact;
- records no Domain Event;
- records no publication work;
- records no Transactional Outbox entry;
- publishes no Integration Event;
- initiates no Pending Activation Process; and
- leaves all previously committed Vendor state unchanged.

The sequence diagram and accompanying explanatory text shall both state these guarantees.

---

## 4.3 Preserve Existing Replay Behaviour

The existing successful replay branch shall remain unchanged.

HJ-105 shall continue to distinguish clearly between:

- first successful processing;
- successful replay of the same completed request; and
- controlled idempotency conflict.

These are three distinct business outcomes.

---

## 4.4 Update Behavioural Notes

Update the explanatory text associated with Section 9 to state that:

- replay behaviour applies only to the same successful request;
- controlled idempotency conflict is a separate business outcome;
- Registration Session state is never consulted;
- the idempotency boundary remains the complete RegisterVendor request.

---

## 4.5 Correct ADR-008 Traceability

Review every ADR-008 reference throughout HJ-105.

Replace references that still use superseded ADR-008 section numbers with references to the current approved ADR-008 structure.

No obsolete ADR-008 section references shall remain.

---

## 4.6 Traceability Matrix

Update the traceability matrix where necessary so that:

- idempotency behaviour references the current ADR-008 sections;
- reliable publication references the current ADR-008 sections;
- conflict behaviour is traceable to ADR-008 rather than inferred from HJ-106.

HJ-106 shall remain a downstream consumer of HJ-105 rather than becoming a source for it.

---

## 4.7 Document Metadata

Update:

- Version;
- Last Updated; and
- Revision History.

The new revision history entry shall record that this version:

- incorporates the controlled idempotency-conflict behaviour from ADR-008;
- updates ADR-008 traceability references;
- contains no business-behaviour changes beyond architectural alignment.

Status shall remain:

**Approved**

---

# 5. Architectural Constraints

This Change Request shall not:

- redesign RegisterVendor;
- alter the idempotency boundary;
- introduce Registration Session behaviour;
- alter Vendor lifecycle;
- alter Pending Activation behaviour;
- introduce additional events;
- change Integration Event content;
- modify Registered Vendor retrieval;
- introduce technical implementation mechanisms.

Specifically, this Change Request shall not prescribe:

- idempotency-key format;
- uniqueness algorithm;
- payload comparison implementation;
- storage technology;
- outbox implementation;
- serialization;
- transport protocol.

These remain governed by ADR-008.

---

# 6. Expected Outcome

Following application of this Change Request:

- HJ-105 remains the authoritative behavioural model for Epic 1 Vendor Registration.
- Controlled idempotency conflict is explicitly illustrated alongside successful replay behaviour.
- Every idempotency outcome is behaviourally complete and architecturally consistent.
- All ADR-008 traceability references align with the current approved ADR.
- HJ-105 publishes a new Approved revision without altering the approved Vendor Registration architecture.

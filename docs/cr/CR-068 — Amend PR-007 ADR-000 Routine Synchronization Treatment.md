# CR-068 — Amend PR-007 ADR-000 Routine Synchronization Treatment

## 1. Status

Proposed — awaiting human approval.

## 2. Target Artefact

`PR-007 - Generate Architecture Propagation Review Batch.md`

## 3. Purpose

Correct PR-007 so that routine synchronization of ADR-000 does not require a separate Change Request merely to index an architectural decision that has already received its own approval and Change Request treatment.

## 4. Problem

PR-007 currently requires a formal Change Request whenever propagation creates, amends or supersedes an ADR or another independent architectural decision authority. Read literally, this also requires a separate ADR-000 Change Request when the register performs only its normal indexing function.

Adding an already approved ADR to ADR-000 does not make, extend or amend the architectural decision. Requiring another Change Request adds administrative records without creating an additional approval boundary.

## 5. Authorised Change

Amend the Change Request escalation rules to:

1. retain Change Request escalation for creation, amendment or supersession of the substantive decision content of an individual ADR;
2. classify ADR-000 index synchronization as routine when it only reflects separately approved ADR creation, amendment, supersession, deprecation or status change;
3. require an ADR-000 execution/reconciliation record for that routine synchronization;
4. retain separate Change Request escalation when ADR-000's structure, lifecycle, numbering, ownership, governance or substantive authority changes; and
5. retain escalation when correcting an independent ADR-000 defect not authorised by the approved decision batch.

## 6. Explicit Non-Goals

- Do not weaken the approval requirement for a new or materially changed architectural decision.
- Do not permit ADR-000 to confer approval on an ADR.
- Do not change ADR numbering, lifecycle, ownership or governance.
- Do not amend PR-000, PR-008, HJ-000, ADR-000 or any architectural decision.
- Do not retrospectively create or remove Change Requests.

## 7. Verification Criteria

- A new substantive ADR still requires its own Change Request under PR-007.
- Routine addition of that accepted ADR to ADR-000 requires an execution/reconciliation record, not another Change Request.
- A structural or governance change to ADR-000 still requires a Change Request.
- The existing one-target-per-Change-Request rule remains unchanged.
- Repository modification remains human-controlled.

## 8. Completion Standard

The change is complete when the approved PR-007 candidate states the distinction unambiguously and no other PR-007 behaviour changes.

## 9. Controlled Application

This Change Request amends only `PR-007 - Generate Architecture Propagation Review Batch.md`. It does not amend any other controlled artefact.

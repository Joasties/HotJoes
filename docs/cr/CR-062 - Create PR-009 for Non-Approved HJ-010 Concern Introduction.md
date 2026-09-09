# CR-062 - Create PR-009 for Non-Approved HJ-010 Concern Introduction

| Field | Value |
|---|---|
| **Change Request ID** | CR-062 |
| **Status** | Proposed for human review and application approval |
| **Target Artefact** | PR-009 - Introduce or Reconcile Non-Approved Architectural Concerns in HJ-010 |
| **Controlled Baseline** | New controlled prompt |
| **Proposed Version / Status** | Initial Approved prompt |
| **Date Proposed** | 3 September 2026 |

## 1. Purpose

Create PR-009 as the controlled method for introducing or reconciling architectural concerns whose Resolution State is Exploring or Blocked into HJ-010.

This Change Request creates only **PR-009 - Introduce or Reconcile Non-Approved Architectural Concerns in HJ-010**. It does not amend any other controlled artefact.

## 2. Reason for Change

PR-007 is designed to propagate Approved architectural concerns through the affected artefact set. Its validation gate expects selected Approved concerns and its normal output is a multi-artefact Architecture Propagation Review Batch.

Newly identified Exploring and Blocked concerns require a different operation:

- reconcile non-authoritative working-register input against approved scope;
- validate concern identity, granularity, Required Guarantee, dependencies and initial state;
- introduce the concerns into HJ-010 without selecting an Approach;
- preserve the Approved concern subset; and
- avoid HJ-012 and downstream propagation.

Using PR-007 for this purpose would blur concern registration with architecture approval and propagation.

## 3. Required Prompt

Create the complete prompt under the exact controlled filename:

```text
PR-009 - Introduce or Reconcile Non-Approved Architectural Concerns in HJ-010.md
```

PR-009 shall:

- operate under PR-000, PR-008 and HJ-009;
- require approved source scope and explicit human approval to register selected concern rows;
- treat the working spreadsheet as non-authoritative input;
- accept only Exploring and Blocked concern introduction or reconciliation;
- validate all nine concern fields and dependencies;
- generate one HJ-010-only CR and complete HJ-010 candidate;
- generate the HJ-010 candidate with its intended controlled Status Approved;
- distinguish the candidate's Approved target status from its external proposed review-package state;
- preserve all unrelated concern rows;
- prohibit HJ-012 and downstream candidate generation;
- produce No-Change Determinations for unaffected artefacts;
- use supplied CR IDs or CR-TBD rather than infer official numbering;
- generate candidates outside the controlled repository; and
- stop at a human application gate.

## 4. Explicit Non-Changes

CR-062 does not:

- amend HJ-010 or introduce CON-041–CON-044;
- amend or extend PR-007;
- approve an architectural Approach;
- create an ADR;
- amend HJ-011, HJ-012, HJ-013 or HJ-107;
- generate architecture or implementation tests;
- modify source or test code; or
- authorise AI repository writes.

## 5. Relationship to Existing Prompts

| Prompt | Responsibility After CR-062 |
|---|---|
| **PR-000** | Governs Exploration, Decision and Execution Mode and the repository-application boundary. |
| **PR-007** | Propagates Approved architectural concerns through affected controlled artefacts. |
| **PR-008** | Provides global baseline, output, filename, source-write and verification rules. |
| **PR-009** | Introduces or reconciles never-Approved Exploring and Blocked concerns in HJ-010 only. |

PR-009 ends at the HJ-010 human application gate. It shall not invoke PR-007 automatically.

## 6. Verification

Verify that PR-009:

- has one unambiguous HJ-010-only responsibility;
- cannot approve an Approach;
- cannot populate HJ-012;
- cannot generate active downstream obligations;
- requires approved upstream scope;
- requires explicit human approval of concern registration;
- validates stable IDs, Required Guarantees, candidate Approaches, states, priorities, blockers and verification treatment;
- preserves the complete existing HJ-010 population;
- generates a complete HJ-010 candidate carrying Status Approved;
- follows PR-008 filenames and output packaging;
- respects the read-only repository boundary; and
- returns control to the human.

## 7. Acceptance Criteria

CR-062 is complete when:

- PR-009 alone is created;
- the prompt uses its exact controlled filename;
- the prompt clearly differs from PR-007's Approved-concern propagation purpose;
- Exploring and Blocked are the only permitted introduction states;
- HJ-010 is the only controlled candidate target;
- HJ-012 and downstream generation are prohibited;
- candidate HJ-010 status remains Approved;
- official CR numbering is never inferred; and
- the complete prompt is presented for human review rather than applied by AI.

## 8. Next Controlled Action

If approved, the human adds PR-009 to the controlled prompt repository.

HJ-000 shall be synchronized separately under CR-063.

After both changes are applied, PR-009 may be invoked for CON-041–CON-044 using HJ-011 v2.4 and the latest working concern register.

## 9. Human Gate

Approval authorises the human to add PR-009 to the controlled repository. It does not authorise AI to create or modify that repository file.

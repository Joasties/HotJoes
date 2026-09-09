# CR-061 - Establish the Repository Application Boundary in PR-000

| Field | Value |
|---|---|
| **Change Request ID** | CR-061 |
| **Status** | Proposed for human review and application approval |
| **Target Artefact** | PR-000 - Architectural Development Mode |
| **Controlled Baseline** | Current Approved PR-000 repository baseline |
| **Proposed Version / Status** | Amended Approved PR-000 baseline |
| **Date Proposed** | 3 September 2026 |

## 1. Purpose

Amend PR-000 so that Architecture Execution Mode explicitly generates proposed candidates without applying them to controlled documentation, source code or tests.

This Change Request amends only **PR-000 - Architectural Development Mode**. It does not amend any other controlled artefact.

## 2. Reason for Change

PR-000 correctly separates Exploration, Decision and Execution and requires human approval before propagation. It describes Execution Mode as systematic propagation through affected artefacts, but does not explicitly distinguish:

```text
generating a complete propagated candidate
```

from:

```text
applying that candidate to the controlled repository
```

The ambiguity can allow an approved decision or CR to be interpreted incorrectly as repository-write authority.

PR-000 should align with the approved repository-level operational boundary and proposed HJ-009 v1.1 while retaining its existing architectural modes and responsibilities.

## 3. Required Changes

### 3.1 Add PR-000 §4.1

Add **§4.1 Proposal and Repository-Application Boundary** within Execution Mode.

The section shall establish the default operating authority as:

```text
Repository modification authority: Read Only
Candidate generation authority: Complete Candidate
Candidate target status: Approved unless the governing process requires another status
Human application gate: Required
```

It shall state that:

- Execution Mode generates CRs, complete candidate artefacts, proposed code, tests and verification evidence;
- Execution Mode does not apply candidates to a controlled repository;
- AI may inspect the repository and use an external disposable copy or isolated sandbox;
- writable build, test, formatting, migration or generation activity shall not modify the controlled repository;
- human approval confirms that a candidate is accepted for application but does not grant AI application authority;
- complete candidate documents carry the status, version and metadata they will have when applied, normally Approved;
- `Proposed — not controlled` describes the external review-package state and does not replace the Approved target status inside the candidate;
- direct AI modification requires separate explicit repository, file/boundary and write-authority instructions; and
- the AI returns control after presenting the proposed candidate and evidence.

### 3.2 Amend §7 Reciprocal Responsibilities

Add AI responsibilities to:

- generate candidates outside the controlled repository;
- treat repository access as read-only unless explicitly and narrowly overridden; and
- use a disposable copy or isolated sandbox where generation or verification requires writes.

### 3.3 Clarify §8 Core Rule

Clarify that systematic propagation produces reviewed candidates and does not transfer application responsibility from the human to AI.

### 3.4 Document Control

PR-000 currently declares no embedded document version or revision-history structure. CR-061 shall not invent one as part of this focused amendment.

The repository's normal external prompt/version-control process records application of CR-061. A separate future governance change may introduce uniform prompt metadata if required.

## 4. Explicit Non-Changes

CR-061 does not:

- change the three PR-000 modes;
- change mode-transition authority;
- change the architectural decision-maker's responsibilities;
- prevent AI from generating complete propagation candidates;
- prevent read-only inspection or sandbox testing;
- grant AI direct repository-write authority;
- amend HJ-009, HJ-008, PR-007 or another controlled artefact; or
- add unrelated prompt metadata or versioning conventions.

## 5. Verification

Verify that the complete proposed PR-000 candidate:

- preserves the Exploration, Decision and Execution Mode boundaries;
- adds the repository-application boundary specifically to Execution Mode;
- distinguishes approval from application;
- applies the rule to architecture documentation, source code and tests;
- preserves the human-controlled mode-transition process;
- permits read-only inspection and external sandbox verification;
- adds no unrelated architectural-development rule; and
- contains no instruction that permits automatic application after approval.

## 6. Acceptance Criteria

CR-061 is complete when:

- PR-000 alone is amended;
- Execution Mode explicitly produces proposed candidates rather than applying them;
- repository modification is read-only by default;
- complete candidates carry their intended controlled target status, normally Approved;
- the external review package remains Proposed — not controlled until human application;
- the human application gate is explicit;
- approval does not grant repository-write authority;
- the existing architectural modes remain unchanged; and
- the complete candidate is presented for human review rather than applied by AI.

## 7. Next Controlled Action

If approved, the human applies the complete proposed PR-000 candidate.

Review of CR-061 may occur concurrently with CR-060. Each CR remains independently target-specific and neither candidate may be applied by AI.

## 8. Human Gate

Approval authorises the human to apply the proposed PR-000 candidate. It does not authorise AI to modify the controlled repository.

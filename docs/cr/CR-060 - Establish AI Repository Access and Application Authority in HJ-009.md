# CR-060 - Establish AI Repository Access and Application Authority in HJ-009

| Field | Value |
|---|---|
| **Change Request ID** | CR-060 |
| **Status** | Proposed for human review and application approval |
| **Target Artefact** | HJ-009 - AI Operating Guide |
| **Controlled Baseline** | HJ-009 v1.0 Approved |
| **Proposed Version / Status** | HJ-009 v1.1 Approved |
| **Date Proposed** | 3 September 2026 |

## 1. Purpose

Amend HJ-009 to make the human-controlled repository boundary explicit and to distinguish approval of AI-generated work from authority to apply it.

This Change Request amends only **HJ-009 - AI Operating Guide**. It does not amend any other controlled artefact.

## 2. Reason for Change

HJ-009 already establishes human approval gates, human-controlled workflow progression and the rule that AI output does not become an accepted project input without approval.

It does not state sufficiently explicitly that:

- controlled repository access is read-only by default;
- an approved candidate must still be applied by the human;
- approval is not repository-write authority;
- source code and test code have the same application boundary as architecture documentation; and
- writable build and test activity must occur in a disposable copy or isolated sandbox.

This ambiguity allowed an approved Change Request to be incorrectly interpreted as authority for AI to modify the controlled architecture repository directly.

## 3. Required Changes

### 3.1 Add HJ-009 §7.1

Add **§7.1 Repository Access and Application Authority** after §7.

The section shall establish that:

- repositories containing controlled architecture documentation, source code or tests are human-controlled;
- AI access is read-only by default;
- AI may inspect controlled content and generate candidates outside the repository;
- AI may use disposable copies or isolated sandboxes for writable build and verification work;
- AI shall not alter controlled files or Git state;
- the workflow separates candidate generation, human approval and human application;
- human approval confirms that a candidate is accepted for application but does not grant AI application authority;
- complete candidate artefacts carry the status, version and metadata they will have when applied, normally Approved;
- `Proposed — not controlled` describes the external review-package state rather than the status written inside the candidate;
- only the human normally applies approved architecture documentation, source code and tests;
- direct AI repository modification requires a separate explicit instruction identifying the repository, exact files or bounded change and direct-write authorisation; and
- words such as `update`, `implement`, `apply` or `continue` do not independently grant repository-write authority.

### 3.2 Clarify §14 Human Approval Gates

Clarify that human approval makes an output eligible to become an accepted project input but does not authorise AI to apply it to controlled documentation, source code or tests.

Application remains separately human-controlled unless explicitly delegated for one bounded task.

### 3.3 Strengthen §19 Definition of Done

Add confirmation that no controlled repository was modified without separate, explicit and bounded repository-write authority.

### 3.4 Add a Golden Rule

Add:

> Approval is not application authority.

### 3.5 Document Control

Publish HJ-009 as v1.1 Approved when this CR is applied.

Update Last Updated and add a revision-history entry recording the repository-access, application-authority and sandbox-verification rules.

Add CR-060 to Related Documents.

## 4. Explicit Non-Changes

CR-060 does not:

- change the architectural decision-maker's authority;
- prevent AI from reading controlled documentation or source code;
- prevent AI from generating complete proposed documents, code or tests;
- prevent builds or tests in a disposable copy or isolated sandbox;
- grant AI direct repository-write authority;
- apply any candidate to the controlled repository;
- amend HJ-008, PR-000 or another prompt; or
- replace repository-local operational instructions such as `AGENTS.md`.

## 5. Verification

Verify that the complete proposed HJ-009 candidate:

- preserves all existing sections and governance rules;
- adds §7.1 without renumbering existing sections;
- distinguishes generation, approval and application;
- applies the boundary equally to architecture documentation, source code and tests;
- permits read-only inspection and sandbox verification;
- requires explicit file-bounded authority for any exception;
- updates metadata, revision history and Related Documents consistently; and
- introduces no unrelated operating-model change.

## 6. Acceptance Criteria

CR-060 is complete when:

- HJ-009 alone is amended;
- repository access is explicitly read-only by default;
- complete candidates carry their intended controlled target status, normally Approved;
- the external review package remains Proposed — not controlled until human application;
- the human application gate is explicit;
- approval is explicitly distinguished from application authority;
- writable verification is directed to a disposable copy or isolated sandbox;
- HJ-009 v1.1 remains internally consistent; and
- the complete candidate is presented for human review rather than applied by AI.

## 7. Next Controlled Action

If approved, the human applies the complete HJ-009 v1.1 candidate.

PR-000 alignment remains governed by the separate CR-061 candidate and may be reviewed concurrently, but neither CR depends on AI applying the other.

## 8. Human Gate

Approval authorises the human to apply the proposed HJ-009 v1.1 candidate. It does not authorise AI to modify the controlled repository.

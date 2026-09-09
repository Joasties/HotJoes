# Change Request
## CR-065 - Amend PR-009 Routine Concern Reconciliation Evidence and HJ-000 Synchronization

| Field | Value |
|---|---|
| Status | Proposed for approval and human application |
| Target artefact | PR-009 - Introduce or Reconcile Non-Approved Architectural Concerns in HJ-010 |
| Controlled baseline | Current controlled PR-009 |
| Proposed treatment | Complete replacement under the existing controlled filename |
| Classification | Process governance amendment |

## 1. Purpose

Amend PR-009 so routine introduction or reconciliation of explicitly approved Exploring and Blocked concerns produces an HJ-010 Concern Reconciliation Record instead of a mandatory Change Request, while retaining formal HJ-010-only CR escalation for changes outside existing concern-registration authority.

This Change Request amends only `PR-009 - Introduce or Reconcile Non-Approved Architectural Concerns in HJ-010`. It does not amend any other controlled artefact.

## 2. Authorised Changes

- Replace the mandatory HJ-010-only CR with an HJ-010 Concern Reconciliation Record for routine execution.
- Define routine execution as registration or reconciliation of explicitly approved Exploring or Blocked concern data derived from approved scope or authority.
- Require formal CR escalation where proposed work changes scope, architectural semantics, governance, lifecycle, ownership, document structure or another authoritative source beyond routine registration.
- Require the reconciliation record to capture baselines, approval evidence, exact concern delta, document control, non-changes, verification and the human application gate.
- Prohibit placeholder CR references in HJ-010 revision history for routine execution.
- Require a complete HJ-000 candidate whenever the proposed HJ-010 version changes.
- Require routine HJ-000 synchronization evidence without a CR, unless HJ-000's own structure, authority or governance changes.
- Require human application order of HJ-010 followed by HJ-000.
- Align output-package, verification, human-gate, prohibited-action and completion wording with these rules.

## 3. Explicit Non-Changes

- PR-009 remains limited to Exploring and Blocked concerns.
- The working register remains non-authoritative.
- PR-009 still selects no Approach, creates no ADR and propagates no unresolved concern downstream.
- HJ-012, HJ-013, HJ-107, source code and test code remain outside PR-009 candidate generation.
- The complete HJ-010 and HJ-000 candidates carry intended controlled Status Approved while their external review package remains proposed and non-authoritative.
- Repository modification remains prohibited and the human application gate remains mandatory.

## 4. Verification and Completion

The change is complete when no unconditional HJ-010 CR requirement remains; routine reconciliation evidence and CR escalation are unambiguous; HJ-000 generation and application order are mandatory when HJ-010 changes version; no official CR number is inferred; and the controlled filename is unchanged.

The official CR identifier remains `CR-TBD` until assigned by the human. Approval does not authorise AI repository modification; the human applies the approved candidate.

# Change Request
## CR-064 - Amend PR-007 Routine Propagation Evidence and HJ-000 Synchronization

| Field | Value |
|---|---|
| Status | Proposed for approval and human application |
| Target artefact | PR-007 - Generate Architecture Propagation Review Batch |
| Controlled baseline | Current controlled PR-007 |
| Proposed treatment | Complete replacement under the existing controlled filename |
| Classification | Process governance amendment |

## 1. Purpose

Amend PR-007 so routine propagation of an explicitly Approved concern batch produces artefact-specific execution/reconciliation evidence instead of mandatory Change Requests, while retaining formal CR escalation for changes exceeding existing propagation authority.

This Change Request amends only `PR-007 - Generate Architecture Propagation Review Batch`. It does not amend any other controlled artefact.

## 2. Authorised Changes

- Replace mandatory per-artefact CR generation with per-artefact Execution/Reconciliation Records for routine propagation.
- Define routine propagation as application of explicitly Approved concern authority through existing controlled prompts.
- Require formal CR escalation for new or changed scope, independent architectural decisions, governance, lifecycle, ownership, structure, or authoritative-source correction outside the Approved batch.
- Preserve single-target evidence and CR granularity, with a narrow exception for an explicitly approved inseparable multi-prompt process amendment.
- Require candidate revision histories to cite the PR-007 execution, concern batch and approval evidence without invented CR identifiers.
- Distinguish the proposed external review package from complete candidates carrying their intended controlled target status, normally Approved.
- Require a complete HJ-000 candidate whenever the batch proposes changed indexed artefact versions or statuses.
- Require HJ-000 to be applied after the artefacts it records and to receive a CR only when its own structure, authority or governance changes.
- Align verification, output-package, checklist, completion and prohibited-action wording with these rules.

## 3. Explicit Non-Changes

- PR-007 remains limited to propagation of explicitly Approved architectural concerns.
- HJ-010 and HJ-012 remain a transactional application pair.
- Working spreadsheets remain non-authoritative.
- Target-specific generation prompts, dependency ordering, invalidation rules and cross-artefact verification remain required.
- Repository modification remains prohibited and the human application gate remains mandatory.
- No architecture, concern state, source code or test code is changed by this CR.

## 4. Verification and Completion

The change is complete when the candidate contains no unconditional requirement to generate a CR for every changed artefact; routine execution records remain artefact-specific; CR escalation conditions are explicit; candidate status language agrees with PR-000; HJ-000 generation and application order are explicit; and the controlled filename is unchanged.

The official CR identifier remains `CR-TBD` until assigned by the human. Approval does not authorise AI repository modification; the human applies the approved candidate.

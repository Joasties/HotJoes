# CR-063 - Add PR-009 and HJ-011 v2.4 to HJ-000

| Field | Value |
|---|---|
| **Change Request ID** | CR-063 |
| **Status** | Proposed for human review and application approval |
| **Target Artefact** | HJ-000 - Current Approved Baseline Manifest |
| **Controlled Baseline** | HJ-000 v0.28 Approved |
| **Proposed Version / Status** | HJ-000 v0.29 Approved |
| **Date Proposed** | 3 September 2026 |

## 1. Purpose

Synchronize HJ-000 with the already approved HJ-011 v2.4 baseline and add PR-009 to the Governing Process Baseline if CR-062 is approved and applied.

This Change Request amends only **HJ-000 - Current Approved Baseline Manifest**. It does not amend any other controlled artefact.

## 2. Reason for Change

HJ-000 v0.28 still records HJ-011 v2.3 although CR-059 has been approved and HJ-011 v2.4 has been applied.

If PR-009 is approved under CR-062, HJ-000 must list it as the controlled method for introducing or reconciling non-Approved Exploring and Blocked concerns in HJ-010.

HJ-000 remains an operational index. These changes do not approve delivery scope, architecture or concern content independently.

## 3. Required Changes

### 3.1 Current Controlled Baseline

Change the HJ-011 entry from v2.3 Approved to v2.4 Approved.

No other controlled architecture, contract or test-catalogue version changes.

### 3.2 Governing Process Baseline

Add:

| Prompt | Purpose | Controlled Filename |
|---|---|---|
| PR-009 | Introduce or reconcile non-Approved Exploring and Blocked concerns in HJ-010 | `PR-009 - Introduce or Reconcile Non-Approved Architectural Concerns in HJ-010.md` |

This row may be applied only after or transactionally with human application of approved CR-062 and PR-009.

### 3.3 Current Workflow Position

Update the architecture-baseline HJ-011 entry to v2.4 Approved.

Replace the superseded implementation-direction block with:

```yaml
implementation_direction:
  capability: Epic 1 Web UI and browser-based verification concern introduction
  next_activity: Use PR-009 to reconcile CON-041–CON-044 from the approved working input into an HJ-010-only candidate without selecting or propagating an Approach.
```

The workflow-position block remains non-authoritative operational guidance.

### 3.4 Document Control

Publish HJ-000 as v0.29 Approved.

Update Last Updated and add one revision-history entry recording:

- HJ-011 v2.4 synchronization; and
- addition of PR-009 to the governing process baseline.

## 4. Explicit Non-Changes

CR-063 does not:

- approve or create PR-009 independently of CR-062;
- amend HJ-011;
- amend HJ-010 or HJ-012;
- add CON-041–CON-044 to the controlled concern register;
- change the Approved concern baseline;
- change HJ-013 or HJ-107;
- approve a Web UI or browser-automation Approach; or
- modify source or test code.

## 5. Application Dependency

The HJ-011 v2.4 synchronization is independently valid because CR-059 has already been applied.

The PR-009 manifest row depends on approved CR-062 and human application of PR-009. If CR-062 is rejected or revised materially, regenerate the HJ-000 candidate before application rather than applying a manifest entry for an unavailable prompt.

## 6. Verification

Verify that:

- the HJ-011 entry is v2.4 Approved in both §2 and §7;
- PR-009 appears exactly once in §4 under its exact controlled filename;
- PR-009's purpose is concern introduction/reconciliation rather than Approved architecture propagation;
- HJ-010, HJ-012, HJ-013, HJ-104, HJ-105, HJ-106 and HJ-107 versions remain unchanged;
- the Approved concern baseline remains unchanged;
- the workflow position is explicitly non-authoritative; and
- metadata and revision history identify v0.29 consistently.

## 7. Acceptance Criteria

CR-063 is complete when:

- HJ-000 alone is amended;
- HJ-011 v2.4 is indexed correctly;
- approved PR-009 is indexed correctly;
- no architecture or concern is approved by the manifest;
- HJ-000 v0.29 carries Status Approved; and
- the complete candidate is presented for human review rather than applied by AI.

## 8. Human Gate

Approval authorises the human to apply HJ-000 v0.29 after or together with PR-009. It does not authorise AI to modify the controlled repository.

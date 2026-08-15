# CR-042 - Remove Circular Test Catalogue Regeneration Dependency

## 1. Change Summary

Amend **PR-004 - Generate Test Catalogue from Approved Service Contract** so that regeneration of **HJ-107 - Vendor Registration Test Catalogue** does not depend on a particular version of **HJ-013 - Architecture and Implementation Test Catalogue** or on HJ-013-owned `AI-*` identifiers.

Establish explicit catalogue ownership and one-way regeneration rules:

- HJ-107 exclusively owns the `VR-*` Test ID namespace;
- the previous HJ-107 is the sole stable-ID reconciliation baseline for `VR-*` identifiers;
- HJ-013 exclusively owns the `AI-*` Test ID namespace;
- HJ-013 is not an input to HJ-107 stable-ID reconciliation or behavioural derivation;
- PR-004 may identify HJ-013, by stable Document ID only, as the owner of complementary architecture and implementation verification;
- HJ-107 shall not depend on a particular HJ-013 version or reference individual `AI-*` identifiers;
- a change to HJ-013 alone shall not trigger HJ-107 regeneration; and
- HJ-107 regeneration shall report preserved, added, retired and materially changed `VR-*` identifiers.

CR-042 amends PR-004 only. It does not regenerate or amend HJ-107, HJ-013 or any authoritative source artefact.

## 2. Reason for Change

PR-004 currently permits HJ-013 to be supplied for cross-catalogue reconciliation and describes it as a regeneration input. This created an avoidable circular document dependency:

```text
HJ-013 v0.1
    -> used while regenerating HJ-107 v0.2
    -> HJ-107 v0.2 records HJ-013 v0.1
    -> HJ-013 is regenerated as v0.2 from HJ-107 v0.2
    -> HJ-107 now contains a superseded HJ-013 version reference
```

The catalogues do not require symmetrical knowledge of one another's versions or stable IDs.

HJ-107 owns behavioural test obligations and can define its boundary with HJ-013 without reading a specific HJ-013 version. HJ-013 is generated downstream and may use the current HJ-107 to delegate behavioural coverage to stable `VR-*` identifiers. Each catalogue needs only its own previous version to preserve identifiers in its namespace.

Without this correction, routine HJ-013 regeneration can make HJ-107 appear stale and encourage unnecessary HJ-107 regeneration, which can then make HJ-013 stale again. The result is regeneration churn with no corresponding behavioural or architectural change.

## 3. Required Changes to PR-004

### 3.1 Attached Artefacts

Remove HJ-013 from the list of attached regeneration inputs.

Replace:

> HJ-013 – Architecture and Implementation Test Catalogue, where supplied for subsequent cross-catalogue reconciliation

with a rule equivalent to:

> HJ-013 is a downstream complementary catalogue and is not required or used as an HJ-107 behavioural-derivation, regeneration or stable-ID input.

Retain the existing HJ-107 as an input whenever PR-004 is used for regeneration.

### 3.2 Authority Table

Replace the HJ-013 authority-table entry with:

| Artefact | Authority in PR-004 |
|---|---|
| **HJ-013** | Downstream owner of complementary architecture and implementation verification. PR-004 may reference the stable Document ID `HJ-013` when assigning responsibility boundaries, but shall not use an HJ-013 version, its contents or `AI-*` identifiers as HJ-107 behavioural or stable-ID inputs. |

Retain the existing HJ-107 authority and strengthen it to state:

| Artefact | Authority in PR-004 |
|---|---|
| **Existing HJ-107** | Sole regeneration baseline for preservation and reconciliation of the HJ-107-owned `VR-*` Test ID namespace, retained catalogue content and HJ-107 change history. |

### 3.3 Catalogue ID Ownership

Add a section defining stable-ID ownership:

| Catalogue | Owned Namespace | Stable-ID Baseline | Permitted External References |
|---|---|---|---|
| **HJ-107** | `VR-*` | Previous HJ-107 | Authoritative source identifiers and `CON-*` dependencies; stable Document ID `HJ-013` for responsibility assignment only |
| **HJ-013** | `AI-*` | Previous HJ-013, governed outside PR-004 | Current HJ-107 `VR-*` identifiers where behavioural coverage is delegated |

Require PR-004 to apply these rules:

1. HJ-107 shall create, preserve, amend, retire and reconcile only `VR-*` Test IDs.
2. The previous HJ-107 is the only stable-ID baseline for `VR-*` identifiers.
3. HJ-107 shall not preserve, create, validate or reconcile `AI-*` identifiers.
4. HJ-107 shall not require knowledge of the current HJ-013 version.
5. HJ-107 shall not contain a version-specific HJ-013 dependency.
6. HJ-107 shall not identify individual HJ-013 obligations as required inputs or outputs.
7. HJ-013 may be named by stable Document ID when a complementary responsibility is assigned to that catalogue.

### 3.4 HJ-107 and HJ-013 Responsibility Boundary

Retain the substantive responsibility boundary introduced by CR-041:

- HJ-107 owns behavioural test obligations derived from HJ-106 Part A and its authoritative sources;
- HJ-013 owns complementary architecture, dependency, concrete persistence, transaction-mechanism, reliable-publication-mechanism and runtime verification;
- the same guarantee may require distinct evidence at different boundaries; and
- identical obligations shall not be duplicated.

Remove these PR-004 instructions:

- supply HJ-013 for overlap review during HJ-107 regeneration;
- reconcile HJ-107 against the contents of a particular HJ-013 version; or
- use HJ-013 as an HJ-107 source-authority or regeneration-baseline entry.

Replace them with:

> Apply the governed HJ-107/HJ-013 responsibility boundary during behavioural derivation. Where an obligation belongs to complementary architecture or implementation verification, identify `HJ-013` as the owning catalogue without selecting or referencing an `AI-*` identifier. Cross-catalogue reconciliation is performed downstream when HJ-013 is generated from the current HJ-107.

### 3.5 Regeneration Direction and Trigger Rule

Add the controlled generation order:

```text
authoritative behavioural sources + previous HJ-107
    -> regenerate HJ-107 and reconcile VR-* IDs

authoritative architecture sources + current HJ-107 + previous HJ-013
    -> regenerate HJ-013 and reconcile AI-* IDs

current HJ-107 + current HJ-013
    -> perform cross-catalogue completion check
```

State explicitly:

- an HJ-013 version change alone does not trigger HJ-107 regeneration;
- HJ-107 regeneration is triggered only by a controlled change to its behavioural sources, architectural dependency inputs, governing generation prompt or HJ-107 itself;
- HJ-107 does not become stale merely because HJ-013 is regenerated; and
- any HJ-013 generation prompt must govern `AI-*` preservation separately and is outside PR-004.

### 3.6 Related Documents Output

When PR-004 generates the HJ-107 Related Documents table:

- identify HJ-013 as **HJ-013 - Architecture and Implementation Test Catalogue**;
- use `Current` or omit a version value rather than recording a specific draft version;
- describe HJ-013 as the downstream complementary catalogue;
- do not describe an HJ-013 version as a regeneration or source-authority input; and
- do not include `AI-*` identifiers.

This rule applies prospectively when HJ-107 is next regenerated for a substantive reason. It does not require immediate HJ-107 regeneration solely to remove an existing version reference.

### 3.7 Stable Test ID Reconciliation

Retain PR-004's existing stable Test ID rules and make the namespace explicit.

The Regeneration Reconciliation section shall report for `VR-*` identifiers:

| Reconciliation Category | Required Treatment |
|---|---|
| **Preserved** | Existing `VR-*` obligation remains semantically unchanged. |
| **Materially changed** | Existing `VR-*` obligation retains its ID but records changed wording, traceability, classification, dependency or boundary. |
| **Added** | A genuinely new, independently testable behavioural obligation receives a new `VR-*` ID. |
| **Retired, merged, split or superseded** | Previous treatment and replacement relationship are recorded; retired IDs are never reused. |

Retain the reconciliation table:

| Test ID | Previous Treatment | Regenerated Treatment | Reason |
|---|---|---|---|

Add completion totals for:

- preserved `VR-*` IDs;
- materially changed `VR-*` IDs;
- newly added `VR-*` IDs;
- retired, merged, split or superseded `VR-*` IDs; and
- unresolved source or cross-catalogue responsibility references.

No `AI-*` identifier shall appear in this reconciliation.

### 3.8 Source Authority and Completeness Analysis

Remove wording that records a supplied HJ-013 version as a source-authority or cross-catalogue review input.

Require the completeness analysis to distinguish:

- behavioural coverage owned by HJ-107;
- complementary verification assigned generically to HJ-013;
- unresolved HJ-010 dependencies; and
- omissions or conflicts in HJ-107's authoritative sources.

The analysis shall not claim that HJ-107 completeness depends on a particular HJ-013 version or on the existence of particular `AI-*` obligations.

### 3.9 Review Checklist

Add checklist confirmations that the regenerated HJ-107:

- preserves and reconciles only HJ-107-owned `VR-*` identifiers;
- uses the previous HJ-107 as the sole `VR-*` stable-ID baseline;
- does not use HJ-013 as a behavioural, regeneration or stable-ID input;
- contains no version-specific HJ-013 dependency;
- contains no `AI-*` identifiers;
- uses stable Document ID `HJ-013` only to assign complementary verification responsibility;
- does not treat an HJ-013-only change as an HJ-107 regeneration trigger; and
- reports preserved, materially changed, added and retired/restructured `VR-*` identifiers.

## 4. Required Preservation of PR-004

Retain all existing PR-004 rules that:

- use HJ-106 Part A as the primary normative behavioural test basis;
- preserve the distinct authority of HJ-010, HJ-012 and the authoritative behavioural sources;
- preserve the HJ-107/HJ-013 behavioural-versus-implementation responsibility boundary;
- prevent HJ-107 from becoming an omnibus architecture and implementation catalogue;
- preserve stable `VR-*` identifiers during regeneration;
- maintain separate Priority and Dependency Status fields;
- preserve unresolved `CON-*` dependencies without selecting their Approaches;
- keep HJ-106 Part B tests Proposed and non-normative;
- require complete requirement-to-test traceability and completeness analysis; and
- generate a Markdown Test Catalogue rather than executable tests or implementation code.

## 5. Explicit Non-Changes

CR-042 does not:

- change Vendor Registration behaviour;
- change any HJ-107 test obligation or `VR-*` identifier;
- change any HJ-013 obligation or `AI-*` identifier;
- remove the responsibility boundary between HJ-107 and HJ-013;
- prohibit HJ-013 from referencing current HJ-107 `VR-*` identifiers;
- define the HJ-013 generation prompt or its `AI-*` reconciliation process;
- resolve any Current Architectural Concern;
- change HJ-006 classifications or Test Levels;
- require immediate HJ-107 regeneration solely to update its HJ-013 reference;
- regenerate HJ-107 or HJ-013; or
- amend any artefact other than PR-004.

## 6. Impacted Artefacts

| Artefact | Impact |
|---|---|
| **PR-004 - Generate Test Catalogue from Approved Service Contract** | Amend attached-input, authority, catalogue ownership, regeneration direction, stable-ID reconciliation and review rules. |
| **HJ-107 - Vendor Registration Test Catalogue** | No immediate change. Future substantive regeneration will use only the previous HJ-107 as its stable-ID baseline and will reference HJ-013 without a specific version. |
| **HJ-013 - Architecture and Implementation Test Catalogue** | No change. Remains downstream and may reconcile its own `AI-*` IDs while referencing current HJ-107 `VR-*` IDs. |
| **HJ-010 / HJ-012 / HJ-106** | No change; retain their existing authority roles. |

## 7. Acceptance Criteria

CR-042 is satisfied when PR-004:

1. removes HJ-013 as an attached HJ-107 regeneration input.
2. defines HJ-107 as the exclusive owner of the `VR-*` namespace.
3. defines the previous HJ-107 as the sole stable-ID reconciliation baseline for `VR-*` identifiers.
4. states that PR-004 does not preserve, create, validate or reconcile `AI-*` identifiers.
5. prevents HJ-107 from depending on or recording a particular HJ-013 version.
6. permits the stable Document ID `HJ-013` only for assignment of complementary verification responsibility.
7. retains the substantive behavioural-versus-architecture responsibility boundary without requiring HJ-013 content during HJ-107 derivation.
8. defines HJ-107 then HJ-013 as the controlled regeneration order.
9. states that an HJ-013-only change does not trigger HJ-107 regeneration.
10. requires future HJ-107 Related Documents output to avoid a version-specific HJ-013 dependency.
11. requires preserved, materially changed, added and retired/restructured `VR-*` reconciliation results.
12. prohibits `AI-*` identifiers from HJ-107 regeneration reconciliation.
13. retains all existing behavioural derivation, traceability, dependency and non-invention rules introduced by CR-041.
14. does not require immediate HJ-107 regeneration solely to correct the existing HJ-013 version reference.

## 8. Completion Check

After applying CR-042, verify that PR-004 contains no instruction to:

- attach or read HJ-013 for HJ-107 regeneration;
- use HJ-013 as a source-authority or stable-ID baseline;
- reconcile `AI-*` identifiers;
- record a specific HJ-013 version in generated HJ-107; or
- regenerate HJ-107 because HJ-013 changed.

Also verify that PR-004 still:

- assigns complementary architecture and implementation verification to HJ-013 by stable Document ID;
- uses the existing HJ-107 to preserve `VR-*` identifiers;
- requires complete HJ-107 behavioural coverage and traceability; and
- reports controlled `VR-*` reconciliation during regeneration.

## 9. Follow-up Work

After CR-042 is applied:

1. retain the current HJ-107 v0.2 without regenerating it solely for the HJ-013 cross-reference;
2. apply the two catalogue-decomposition corrections identified for HJ-013 v0.3;
3. perform the focused cross-catalogue completion check;
4. define or amend the HJ-013 generation method so that the previous HJ-013 is the `AI-*` baseline and the current HJ-107 supplies referenced `VR-*` coverage IDs; and
5. use the revised PR-004 during the next substantively required HJ-107 regeneration.

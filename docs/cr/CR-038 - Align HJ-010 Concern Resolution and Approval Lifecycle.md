# CR-038 - Align HJ-010 Concern Resolution and Approval Lifecycle

## 1. Change Summary

Amend **HJ-010 - Current Application Architectural Concerns** so that it represents only architectural concerns that remain current for the active implementation scope.

HJ-010 shall contain concerns whose Resolution State is:

```text
Exploring + Selected + Blocked + Challenged
```

Remove the current requirement that a selected Approach must be implemented and verified before it can leave HJ-010. Implementation and test execution are downstream validation of approved architecture, not stages in the architectural concern-resolution lifecycle.

Clarify that:

- an Exploring concern may record multiple candidate Approaches;
- a Selected concern records one chosen Approach whose applicable decision authority is not yet complete or approved;
- a Blocked concern identifies its blocking `CON-xxx` concerns and/or missing authoritative sources in **Decision Treatment / Source**;
- a Challenged concern is a previously approved resolution returned for architectural reconsideration; and
- once the selected Approach receives the required decision approval, the concern leaves HJ-010 and its approved resolution becomes eligible for publication in **HJ-012 - Established Application Architecture Patterns**.

Replace references to the planned **Application Architecture Verification Catalogue** with **HJ-013 - Architecture and Implementation Test Catalogue**.

CR-038 amends HJ-010 only. It does not create, regenerate or amend HJ-012, HJ-106, HJ-107, HJ-013, the Deferred Architectural Concerns artefact or any spreadsheet.

## 2. Reason for Change

HJ-010 v0.2 defines a lifecycle in which selected architecture remains current until it is implemented and verified:

```text
Selected -> Implemented -> Verified -> HJ-012
```

This creates an incorrect dependency on downstream implementation and testing before approved architecture can become authoritative. The intended architecture-first flow is:

```text
Current Architectural Concern
    -> explore candidate Approaches
    -> select one Approach
    -> complete the applicable decision authority
    -> publish the approved reusable resolution in HJ-012
    -> assess/regenerate downstream contracts and test catalogues
    -> generate implementation
    -> execute tests
```

HJ-010 must therefore govern architectural exploration, selection, blocking and challenge. It must not track implementation or test-execution maturity as concern Resolution States.

HJ-010 v0.2 also:

- includes `Identified`, `Implemented` and `Verified` states that are not required by the concern-resolution process;
- describes Selected as already having the required authority while still awaiting implementation or verification;
- requires implementation and verification before promotion to HJ-012;
- retains selected concerns explicitly because implementation or verification has not occurred;
- uses the obsolete planned name **Application Architecture Verification Catalogue**; and
- does not consistently identify related `CON-xxx` dependencies for Blocked concerns.

These inconsistencies must be corrected before HJ-012 and the downstream generated artefacts are established.

## 3. Required Changes to HJ-010

### 3.1 Purpose and Authority

Retain HJ-010 as the authoritative register of **Current Architectural Concerns** for the active implementation scope.

Amend the Purpose section so that it states that HJ-010 contains only concerns whose Resolution State is:

- Exploring;
- Selected;
- Blocked; or
- Challenged.

Replace the current statement:

> Reusable architecture remains current until it has been selected with appropriate authority, implemented and verified.

with wording equivalent to:

> A reusable architectural resolution remains current until one Approach has been selected and approved through the appropriate decision authority. It then leaves HJ-010 and is eligible for publication in HJ-012 as approved application architecture. Implementation and test execution validate conformance with approved architecture; they are not prerequisites for the resolution to leave HJ-010.

HJ-010 shall not describe a spreadsheet or other transitory working aid as an authoritative source. HJ-010 itself remains authoritative for the Current Architectural Concerns it publishes.

### 3.2 Approach Semantics

Retain **Approach** as the column containing the patterns, standards, conventions, policies or implementation mechanisms being considered or selected for a concern.

Define its state-dependent use as follows:

| Resolution State | Approach Treatment |
|---|---|
| **Exploring** | One or more candidate Approaches may be recorded while the architectural decision is being evaluated. |
| **Selected** | Exactly one chosen Approach is recorded. Its applicable decision authority remains to be completed or approved. |
| **Blocked** | One or more candidate Approaches may remain recorded where known, but the concern cannot progress until its identified blockers are resolved. The Approach may be blank where no responsible candidate can yet be identified. |
| **Challenged** | The previously approved Approach and one or more alternative Approaches may be recorded while the concern is reconsidered. |

Alternative analysis and decision rationale shall remain in the applicable ADR or supporting architectural discussion rather than being duplicated extensively in the concern table.

### 3.3 Controlled Resolution States

Replace the current Resolution State set:

- Identified;
- Exploring;
- Selected;
- Blocked;
- Implemented; and
- Verified;

with the following controlled values:

| State | Meaning |
|---|---|
| **Exploring** | One or more candidate Approaches are being evaluated and an architectural selection remains to be made. |
| **Selected** | One Approach has been chosen, but its applicable decision authority has not yet been completed or approved. |
| **Blocked** | Architectural progress depends on one or more explicitly identified concerns or missing authoritative sources. |
| **Challenged** | A previously approved resolution has returned for architectural reconsideration because downstream contract generation, test generation or implementation evidence exposed a potential architectural deficiency. |

Remove `Identified`, `Implemented` and `Verified` from HJ-010.

Where a newly added concern has not yet been explored, it shall enter HJ-010 as **Exploring**. Implementation progress and test results shall be managed by their owning implementation and test artefacts rather than as HJ-010 Resolution States.

### 3.4 Selected and Decision Authority

Amend the Selected definition so that selection and approval are not conflated.

A concern is **Selected** when:

- exactly one Approach has been chosen;
- its Required Guarantee is sufficiently clear;
- **Decision Treatment / Source** identifies the authority required to approve the selection; and
- the required authority is not yet complete or approved for this resolution.

Decision Treatment / Source may identify:

- an existing ADR requiring confirmation or application;
- a new or amended ADR required;
- an Architectural Principle;
- an Engineering Standard;
- an Established Framework / Platform Convention;
- an Implementation-local Decision with the applicable approval treatment; or
- another unresolved authoritative dependency.

Not every concern requires a new ADR. Architectural significance continues to determine whether a new or amended ADR is necessary.

Once the applicable decision authority is complete and the reusable resolution is approved, the concern no longer belongs in HJ-010. HJ-012 treatment is outside CR-038.

### 3.5 Blocked Concern Dependencies

For every concern whose Resolution State is **Blocked**, **Decision Treatment / Source** shall identify:

- one or more blocking `CON-xxx` concerns where the dependency is represented by another Current Architectural Concern; and/or
- the missing authoritative contract, decision or source where no separate concern represents the dependency.

The same field may also state which related concerns are blocked by the concern where this materially improves dependency visibility.

Examples:

```text
Blocked by CON-009; Address failure taxonomy required before resilience treatment can be selected.
```

```text
Blocked by missing approved technical service/API contract; blocks CON-023, CON-025, CON-026, CON-030 and CON-031.
```

Do not introduce a separate dependency column.

As part of applying CR-038, reconcile every current Blocked row, including CON-009, CON-010, CON-013, CON-015, CON-020, CON-024 and CON-025, so that its Decision Treatment / Source contains explicit concern dependencies where applicable.

### 3.6 Challenged Concerns

Define **Challenged** as an architectural reconsideration state, not a general test-failure state.

A previously approved concern becomes Challenged only when evidence indicates that:

- the approved architectural guarantee is insufficient, contradictory or ambiguous;
- correct downstream contracts or tests cannot be derived from the authoritative architecture; or
- the approved Approach cannot fulfil its Required Guarantee under the applicable architectural constraints.

A normal implementation defect does not challenge the architecture. The implementation must be corrected to conform to the approved architecture.

When a concern returns as Challenged:

- retain the same stable `CON-xxx` identifier;
- identify the evidence or affected downstream artefact in Scope / Source or Decision Treatment / Source;
- record the previously approved Approach and any alternatives now under consideration; and
- retain the Challenged state while the approved resolution is being reconsidered.

### 3.7 Concern Lifecycle

Replace the current normal lifecycle with:

```text
Deferred Architectural Concern
    -> Exploring in HJ-010
    -> Selected in HJ-010
    -> applicable decision authority completed
    -> approved reusable resolution leaves HJ-010
    -> eligible for publication in HJ-012
```

Permitted alternatives are:

```text
Exploring / Selected / Blocked / Challenged -> Deferred
Exploring / Selected / Blocked / Challenged -> Superseded / Removed
Previously approved resolution -> Challenged in HJ-010
```

Remove every implication that implementation or passing tests are prerequisites for an approved reusable resolution to leave HJ-010.

### 3.8 Scope Reconciliation

Amend the Epic or material-scope reconciliation steps so that Project Architecture shall:

1. update the active implementation baseline;
2. check the new scope against the Current Architectural Concerns;
3. reassess applicable approved architecture against new architectural forces;
4. promote applicable Deferred Architectural Concerns into HJ-010 as Exploring;
5. add genuinely new concerns as Exploring;
6. retain concerns that remain Exploring, Selected, Blocked or Challenged;
7. remove concerns whose resolutions have received the required approval and no longer belong in the Current Architectural Concerns register;
8. defer concerns no longer required by the active scope but still materially significant;
9. supersede or remove obsolete concerns; and
10. record the reconciliation.

HJ-010 shall continue not to accumulate permanent Epic-specific concern tables.

### 3.9 Verification Catalogue Naming and Responsibilities

Replace every reference to:

> Application Architecture Verification Catalogue

with:

> **HJ-013 - Architecture and Implementation Test Catalogue**

Retain the distinction that:

- HJ-107 contains Vendor Registration behavioural tests generated from HJ-106 and its authoritative business/domain sources; and
- HJ-013 contains architecture and implementation tests generated from applicable approved architecture, Current Architectural Concern verification obligations, ADRs and engineering standards.

HJ-010 shall reference verification destinations but shall not contain the test specifications themselves.

The downstream flow shall be expressed equivalently to:

```text
Approved architecture
    -> assess and regenerate HJ-106 where observable service behaviour is affected
    -> regenerate HJ-107
    -> generate HJ-013
    -> generate implementation
    -> execute HJ-107 and HJ-013 tests
```

CR-038 does not create or populate HJ-013.

### 3.10 Priority Wording

Retain P0, P1 and P2 unless a separate priority change is approved.

Replace wording that describes priority as ordering concerns through implementation and verification with wording equivalent to:

> Priority determines the order in which Current Architectural Concerns should be explored, unblocked, selected and taken through the applicable decision authority. It does not select an Approach.

### 3.11 Current Concern Table Reconciliation

Review the Current Architectural Concerns table when applying CR-038 and:

- ensure every retained row uses only Exploring, Selected, Blocked or Challenged;
- preserve multiple candidate Approaches for Exploring and Blocked concerns where appropriate;
- preserve a single chosen Approach for Selected concerns;
- add explicit `CON-xxx` relationships to Blocked rows under Decision Treatment / Source;
- replace the obsolete verification-catalogue name with HJ-013;
- remove statements that make HJ-012 candidacy conditional on implementation or verification;
- correct Verification Treatment wording so that it identifies HJ-107, HJ-013 or another governed verification mechanism without inventing test specifications; and
- remove any concern that has completed the required architectural approval and therefore no longer belongs in HJ-010.

The treatment of approved concerns in HJ-012 is outside CR-038 and shall be handled separately.

CR-038 does not itself approve an Approach, change a current concern to Approved or identify a specific concern as having completed approval. A concern shall be removed from HJ-010 only when its approval has been established through separate controlled decision treatment.

### 3.12 Reconciliation History

Preserve the historical v0.1-to-v0.2 mapping in §10, but correct statements that imply selected Aggregate, Value Object, Entity, Domain Event, Repository or other Approaches remain current only because implementation or verification is incomplete.

The historical mapping shall describe how the former `AC-xxx` rows became `CON-xxx` concerns without imposing the superseded promotion threshold.

Update the current Reconciliation Record so that it accurately records:

- which concerns remain current;
- which concerns have left HJ-010 after completing the required architectural approval;
- which concerns are Blocked and their related `CON-xxx` dependencies;
- any Challenged concerns returned to HJ-010; and
- the reconciliation date.

Do not record spreadsheet operation, iteration planning or batch-management details as architectural authority.

### 3.13 Open Decisions and Follow-up

Remove or replace current follow-up statements that:

- prohibit promotion of unimplemented or unverified architecture;
- require definition of the obsolete Application Architecture Verification Catalogue; or
- imply that HJ-010 owns implementation or test-execution state.

The revised follow-up section may identify:

- unresolved P0 and Blocked concerns;
- required new or amended ADRs;
- the HJ-106 impact assessment;
- regeneration of HJ-107;
- creation of HJ-013; and
- separate controlled work for HJ-012 and Deferred Architectural Concerns.

## 4. Required Treatment of Existing HJ-010 Sections

| HJ-010 Section | Required CR-038 Treatment |
|---|---|
| Purpose | Remove implementation/verification promotion threshold and state the four current Resolution States. |
| Current Implementation Baseline | Retain; update reconciliation date when CR-038 is applied. |
| Approach | Preserve multiple candidates during Exploring; require one Approach for Selected. |
| Resolution State | Replace the six current states with Exploring, Selected, Blocked and Challenged. |
| Decision Treatment | Retain the non-ADR decision model; clarify incomplete authority for Selected and blocker references for Blocked. |
| Priority | Retain P0/P1/P2; remove implementation/verification sequencing language. |
| Verification Rule | Replace obsolete catalogue name with HJ-013 and retain traceability without copying tests. |
| Epic 1 Concern-Extraction Boundary | Retain unless separate scope reconciliation identifies a correction. |
| Current Architectural Concerns | Reconcile states, Approaches, blockers, HJ-013 references and removal of approved concerns. |
| Resolution Priorities and Dependencies | Retain dependency-driven ordering; update wording to the corrected resolution lifecycle. |
| Verification Architecture | Rename the downstream catalogue to HJ-013 and revise the downstream generation flow. |
| Concern Lifecycle and Scope Reconciliation | Replace implementation/verification promotion with decision-approval transition. |
| Deferred Architectural Concerns | Retain; do not create or amend the separate artefact through CR-038. |
| Reconciliation of v0.1 | Retain historical mapping while removing superseded promotion assertions. |
| Reconciliation Record | Update to reflect the current post-approval HJ-010 concern set. |
| Open Decisions and Follow-up | Remove obsolete lifecycle and catalogue-name statements. |

## 5. Explicit Non-Changes

CR-038 does not:

- create, regenerate or amend HJ-012;
- define the detailed structure of HJ-012;
- create or amend a spreadsheet or make a spreadsheet authoritative;
- create or populate HJ-013;
- amend HJ-106 or HJ-107;
- change Vendor Registration business behaviour;
- change the Vendor Domain model;
- change HJ-011's approved Epic 1 scope;
- select a new Approach for any unresolved concern;
- approve any current Approach or change any current concern to Approved;
- require an ADR for every concern;
- change Address or Compliance ownership;
- introduce feature-management behaviour;
- create the Deferred Architectural Concerns artefact; or
- change HJ-SM-001.

Any impact on HJ-012, HJ-106, HJ-107 or HJ-013 shall be handled through subsequent controlled work.

## 6. Impacted Artefacts

| Artefact | Required Change / Impact |
|---|---|
| **HJ-010 - Current Application Architectural Concerns** | Primary and only artefact amended by CR-038. Revise the concern lifecycle, Approach semantics, Resolution States, Blocked dependencies, verification-catalogue naming, current table and reconciliation material. |
| **HJ-012 - Established Application Architecture Patterns** | No change under CR-038. Separate controlled work will establish or amend its contents. |
| **HJ-013 - Architecture and Implementation Test Catalogue** | No change under CR-038. HJ-010 will use this name for the planned downstream catalogue. |
| **HJ-106 - Vendor Registration Service Contract** | No change under CR-038. Later impact assessment determines whether approved architecture affects the contract. |
| **HJ-107 - Vendor Registration Test Catalogue** | No change under CR-038. Its behavioural derivation boundary remains intact. |
| **Applicable ADRs / Standards** | No automatic change. HJ-010 identifies where existing, new or amended decision authority is required. |

## 7. Document Control

When CR-038 is applied:

- increment HJ-010 from **v0.2** to **v0.3**;
- retain **Status: Draft** until the revised HJ-010 completes architectural review;
- update **Last Updated** to the application date;
- add a Revision History entry identifying CR-038 and the corrected concern-resolution and approval lifecycle;
- add CR-038 to Related Documents;
- update **Last Concern Reconciliation**; and
- retain the HJ-010 document title **Current Application Architectural Concerns**.

## 8. Acceptance Criteria

CR-038 is complete when:

1. HJ-010 remains the authoritative register of Current Architectural Concerns for the active implementation scope.
2. HJ-010 states that it contains only Exploring, Selected, Blocked and Challenged concerns.
3. The only permitted HJ-010 Resolution States are Exploring, Selected, Blocked and Challenged.
4. Identified, Implemented and Verified are removed as HJ-010 Resolution States.
5. Exploring permits one or more candidate Approaches.
6. Selected requires exactly one chosen Approach.
7. Selected means that the applicable decision authority remains incomplete or unapproved for the resolution.
8. Blocked permits one or more candidate Approaches, or no Approach where the blocker prevents responsible candidate identification.
9. Challenged permits the previously approved Approach and one or more alternatives during reconsideration.
10. Every Blocked concern identifies blocking `CON-xxx` concerns and/or missing authoritative sources in Decision Treatment / Source.
11. No separate dependency column is introduced.
12. Challenged is used only for architectural reconsideration, not ordinary implementation defects.
13. A Challenged concern retains its stable `CON-xxx` identifier.
14. Implementation and verification are not prerequisites for an approved reusable resolution to leave HJ-010.
15. The normal lifecycle moves from Exploring to Selected, through completion of the applicable decision authority, and then out of HJ-010.
16. HJ-010 does not define Approved as one of its Current Architectural Concern states.
17. Not every concern is required to have a new ADR.
18. DDD classification does not determine ADR necessity.
19. Every retained concern has a Required Guarantee, Scope / Source, Approach treatment, Resolution State, Priority, Decision Treatment / Source and Verification Treatment.
20. Every retained concern uses only a permitted Resolution State.
21. Multiple candidates are preserved where appropriate for Exploring, Blocked and Challenged concerns.
22. Selected concerns contain one chosen Approach.
23. Statements making HJ-012 candidacy dependent on implementation or verification are removed.
24. Every reference to the planned Application Architecture Verification Catalogue is replaced by **HJ-013 - Architecture and Implementation Test Catalogue**.
25. HJ-107 remains the behavioural test catalogue derived from HJ-106 and its approved business/domain sources.
26. HJ-010 points architecture and implementation verification obligations to HJ-013 or another explicitly governed mechanism without copying test specifications.
27. Priority wording governs concern resolution order rather than implementation/verification maturity.
28. Scope reconciliation retains only Exploring, Selected, Blocked and Challenged concerns in HJ-010.
29. Concerns whose resolutions have completed the required architectural approval are removed from the Current Architectural Concerns table.
30. Historical v0.1-to-v0.2 mapping is retained without the superseded implementation/verification promotion rule.
31. The Reconciliation Record identifies current, approved-and-removed, Blocked and Challenged concern treatments accurately.
32. HJ-010 contains no statement making a spreadsheet or other transitory working aid authoritative.
33. HJ-010 contains no iteration-planning or batch-management detail as architectural authority.
34. HJ-010 is versioned to **v0.3** and remains **Draft** pending review.
35. CR-038 does not create or amend HJ-012, HJ-106, HJ-107, HJ-013 or the Deferred Architectural Concerns artefact.
36. No unrelated business, domain, scope or System Model decision is changed.

## 9. Follow-up Work

After CR-038 has been applied and HJ-010 v0.3 reviewed:

1. create or revise HJ-012 through separate controlled work;
2. assess the approved architecture's impact on HJ-106;
3. regenerate HJ-107 where an authoritative behavioural source changes;
4. create HJ-013 from the applicable approved architecture, current verification obligations, ADRs and engineering standards;
5. generate implementation from the approved architecture and contracts; and
6. create the Deferred Architectural Concerns artefact separately where required.

# CR-039 - Establish Complete Concern Tracking and Publish First Approved Architecture Batch

## 1. Change Summary

Amend **HJ-010 - Current Application Architectural Concerns** so that it becomes the complete architectural concern register and retains every concern throughout its resolution lifecycle, including concerns whose Resolution State is **Approved**.

Create **HJ-012 - Established Application Architecture Patterns** as the authoritative catalogue of concerns that have entered the approved HotJoes application architecture baseline. Once an approved concern has been added to HJ-012, its entry remains there so that the approved architecture and any later challenge can be traced.

Publish the first approved architecture batch comprising:

```text
CON-001, CON-002, CON-003, CON-004, CON-005, CON-017 and CON-027
```

For each concern in the batch:

- retain the concern in HJ-010 and set or retain its Resolution State as Approved;
- add its approved resolution to HJ-012;
- preserve its stable `CON-xxx` identifier and architectural data; and
- verify that its current state and approved resolution are consistent across the two authoritative artefacts.

On completion, publish both HJ-010 and HJ-012 as **version 1.0, Approved**.

## 2. Reason for Change

CR-038 established a model in which an approved concern leaves HJ-010 when its resolution is published in HJ-012. Although that separates current concerns from approved architecture, it also divides the complete concern population across two documents and makes end-to-end concern tracking less direct.

HJ-010 should answer:

> What architectural concerns have been identified, and what is the current Resolution State of each concern?

HJ-012 should answer:

> What application architecture has been approved, and does each approved resolution remain active or has it been challenged?

Retaining Approved concerns in HJ-010 ensures that:

- the complete concern population can be reconciled in one authoritative register;
- no concern disappears when it becomes approved;
- the current lifecycle state of every concern remains visible;
- HJ-012 can be reconciled to the Approved subset of HJ-010; and
- a later challenge can be tracked without deleting the previously approved architecture.

The duplication is controlled and purposeful. HJ-010 is authoritative for concern identity and current Resolution State. HJ-012 is authoritative for the approved architectural resolution and whether that resolution remains active.

## 3. Required Changes to HJ-010

### 3.1 Purpose and Authority

Replace the rule that HJ-010 contains only Exploring, Selected, Blocked and Challenged concerns.

Define HJ-010 as the authoritative and complete register of architectural concerns identified for the active implementation scope. It shall retain concerns in all five controlled Resolution States:

```text
Exploring + Selected + Blocked + Approved + Challenged
```

An Approved concern remains in HJ-010. Approval causes its resolution to be added to HJ-012; it does not cause the concern to be removed from HJ-010.

### 3.2 Controlled Resolution States

Use exactly the following Resolution States:

| Resolution State | Meaning |
|---|---|
| **Exploring** | One or more candidate Approaches are being evaluated and an architectural selection remains to be made. |
| **Selected** | Exactly one Approach has been chosen, but its applicable decision authority has not yet been completed or approved. |
| **Blocked** | Progress depends on one or more identified concerns or missing authoritative sources recorded in Decision Treatment / Source. |
| **Approved** | One Approach has received the applicable decision approval and forms part of the application architecture baseline published in HJ-012. |
| **Challenged** | A previously Approved resolution has returned for architectural reconsideration because downstream derivation or evidence exposed a potential architectural deficiency. |

Do not introduce additional concern, implementation or validation states.

### 3.3 Approach Semantics

Retain the existing Approach rules for Exploring, Selected, Blocked and Challenged, and add:

| Resolution State | Approach Treatment |
|---|---|
| **Approved** | Exactly one approved Approach is recorded and must agree with the corresponding HJ-012 entry. |

An Exploring, Blocked or Challenged concern may continue to contain multiple candidate Approaches where appropriate. The requirement for exactly one Approach applies to Selected and Approved concerns.

### 3.4 Approved Concerns and HJ-012

State that every Approved concern in HJ-010 shall have one corresponding entry in HJ-012 with the same:

- ID;
- Architectural Concern;
- Required Guarantee;
- Scope / Source;
- Approach;
- Resolution State;
- Priority;
- Decision Treatment / Source; and
- Verification Treatment.

HJ-010 remains authoritative for the concern's current Resolution State. HJ-012 remains authoritative for the approved architectural resolution.

### 3.5 Challenged Concerns

Retain the existing definition of Challenged and clarify the coordinated treatment:

- the concern remains in HJ-010 under the same stable Concern ID and changes from Approved to Challenged;
- the corresponding HJ-012 entry remains present and changes from Approved to Challenged;
- the HJ-012 entry is no longer active implementation authority while Challenged;
- reconsideration details, candidate Approaches and blockers are managed in HJ-010; and
- once a revised resolution receives approval, HJ-010 returns to Approved and HJ-012 is updated with the newly approved resolution.

A normal implementation defect does not challenge approved architecture.

### 3.6 Lifecycle

Replace the lifecycle with:

```text
Deferred Architectural Concern
    -> Exploring in HJ-010
    -> Selected in HJ-010
    -> applicable decision authority completed
    -> Approved in HJ-010
    -> approved resolution added to HJ-012
```

Challenge follows:

```text
Approved in HJ-010 and HJ-012
    -> architectural deficiency identified
    -> Challenged in HJ-010 and HJ-012
    -> reconsider and complete applicable decision authority
    -> Approved in HJ-010 and updated in HJ-012
```

Implementation and passing tests remain downstream validation. They are not prerequisites for the Approved Resolution State.

### 3.7 Scope Reconciliation

Amend the reconciliation rules so that approved concerns are retained rather than removed. Reconciliation shall:

1. preserve all concerns applicable to the active scope;
2. update their current Resolution States and other concern data where required;
3. verify that every Approved concern has exactly one corresponding HJ-012 entry;
4. verify that every Challenged concern previously published in HJ-012 remains represented there as Challenged;
5. add newly approved resolutions to HJ-012; and
6. update challenged resolutions in HJ-012 when they are re-approved.

Deferral, supersession or removal from the active concern population remains subject to an explicit reconciliation decision; approval alone is not a reason to remove a concern.

### 3.8 First Approved Batch

Retain the following concerns in the HJ-010 concern table and set or retain their Resolution State as Approved:

| ID | Architectural Concern | Approach |
|---|---|---|
| CON-001 | Vendor consistency boundary | Aggregate |
| CON-002 | Identity-free business concepts | Value Object |
| CON-003 | Vendor identity and lifecycle state | Entity |
| CON-004 | Registration business fact | Domain Event |
| CON-005 | Aggregate persistence abstraction | Repository |
| CON-017 | Reliable publication staging | Transactional Outbox |
| CON-027 | Registered Vendor retrieval | Query handler + Repository + response mapper |

No other concern becomes Approved through this CR.

### 3.9 Document Control

Publish HJ-010 as:

| Field | Value |
|---|---|
| **Version** | 1.0 |
| **Status** | Approved |
| **Last Updated** | Date CR-039 is applied |

Add a revision-history entry stating that CR-039 established HJ-010 as the complete architectural concern register, introduced Approved as a retained Resolution State, reconciled the first approved batch and established synchronization with HJ-012.

Add HJ-012 and CR-039 to Related Documents and record the first approved batch in the reconciliation history.

## 4. Required Creation of HJ-012

### 4.1 Document Identity

Create:

```text
HJ-012 - Established Application Architecture Patterns
```

Publish it as:

| Field | Value |
|---|---|
| **Document ID** | HJ-012 |
| **Document Title** | Established Application Architecture Patterns |
| **Version** | 1.0 |
| **Status** | Approved |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | Date CR-039 is applied |

The initial revision-history entry shall state that HJ-012 was created by CR-039 and populated with the first approved application architecture batch.

### 4.2 Purpose and Authority

Define HJ-012 as the authoritative catalogue of application architecture that has received the applicable decision approval and entered the HotJoes application architecture baseline.

Established means selected and approved architecture. It does not mean that implementation has already been generated or that all applicable tests have passed.

Every concern that enters HJ-012 remains represented there. A challenged entry is retained for traceability but is not active implementation authority until its resolution is approved again.

### 4.3 Established Architecture Table

Create one table with these columns:

| Column | Required Content |
|---|---|
| **ID** | Stable `CON-xxx` identifier inherited from HJ-010. |
| **Architectural Concern** | The architectural problem addressed. |
| **Required Guarantee** | The guarantee preserved by the approved architecture. |
| **Scope / Source** | The authoritative sources creating or constraining the concern. |
| **Approach** | The approved pattern, standard, convention, policy or implementation mechanism retained from HJ-010. |
| **Resolution State** | Approved or Challenged. |
| **Priority** | The concern priority retained from HJ-010. |
| **Decision Treatment / Source** | The authority supporting the approved resolution. |
| **Verification Treatment** | Applicable verification destinations or evidence. |

Do not add a Pattern ID, iteration field, implementation state or validation state.

HJ-012 Resolution State uses only:

| Resolution State | Meaning in HJ-012 |
|---|---|
| **Approved** | The resolution is part of the active application architecture baseline. |
| **Challenged** | The previously approved resolution is retained for traceability but is not active implementation authority; reconsideration is managed in HJ-010. |

### 4.4 Initial Entries

Populate HJ-012 with exactly the seven concerns listed in Section 3.8. Each initial entry shall have Resolution State Approved.

Copy the following from the corresponding HJ-010 row without semantic alteration:

- ID;
- Architectural Concern;
- Required Guarantee;
- Scope / Source;
- Approach;
- Resolution State;
- Priority;
- Decision Treatment / Source; and
- Verification Treatment.

The HJ-010 and HJ-012 concern tables shall use the same nine-column structure and column order:

```text
ID | Architectural Concern | Required Guarantee | Scope / Source | Approach | Resolution State | Priority | Decision Treatment / Source | Verification Treatment
```

Their responsibilities differ through their row populations and state treatment, not through different table structures. HJ-010 contains every concern and its current concern-resolution data. HJ-012 contains only concerns that have reached Approved and preserves their approved architectural resolutions.

### 4.5 Retention and Synchronization

Once a concern has entered HJ-012:

- do not remove it merely because it becomes Challenged;
- update its Resolution State to Challenged when the corresponding HJ-010 concern becomes Challenged;
- restore Approved only when the applicable decision authority has approved the reconsidered resolution; and
- update the approved architectural data when a replacement resolution is approved.

HJ-012 shall direct readers to HJ-010 for the current reconsideration of a Challenged concern.

## 5. Consistency Rules

The following rules apply after CR-039:

| HJ-010 State | Required HJ-012 Treatment |
|---|---|
| Exploring | No entry, unless the concern was previously Approved and is now being reconsidered as Challenged. |
| Selected | No entry for a never-approved concern. |
| Blocked | No entry for a never-approved concern. |
| Approved | Exactly one matching entry with Resolution State Approved. |
| Challenged | If previously Approved, exactly one matching entry with Resolution State Challenged. |

A previously approved concern shall use Challenged—not Exploring, Selected or Blocked—while architectural reconsideration is active. This preserves an unambiguous synchronization rule without adding states.

## 6. Completion Checks

CR-039 is complete only when all of the following checks pass:

| Check | Expected Result |
|---|---|
| HJ-010 completeness | HJ-010 retains all 39 concern rows. |
| Controlled states | Every HJ-010 row uses Exploring, Selected, Blocked, Approved or Challenged. |
| First batch state | CON-001–CON-005, CON-017 and CON-027 are Approved in HJ-010. |
| Approach cardinality | Each Approved concern has exactly one Approach. |
| HJ-012 population | HJ-012 contains exactly seven entries. |
| HJ-012 state | All seven initial HJ-012 entries are Approved. |
| Identity consistency | Each batch Concern ID appears exactly once in each document. |
| Structural consistency | The HJ-010 and HJ-012 concern tables use the same nine columns in the same order. |
| Data consistency | Each initial HJ-012 entry matches its HJ-010 ID, concern name, guarantee, sources, Approach, Resolution State, Priority, decision provenance and verification treatment. |
| Non-batch preservation | No other HJ-010 concern is approved or otherwise semantically changed by this CR. |
| Model restraint | No additional Resolution State, Pattern ID, iteration, implementation state or validation state has been introduced. |
| Document baseline | HJ-010 and HJ-012 are both version 1.0, Approved. |

## 7. Explicit Non-Changes

CR-039 does not:

- remove Approved concerns from HJ-010;
- approve any concern outside the seven listed in Section 3.8;
- alter the approved Approaches or other architectural data of the first batch;
- create Pattern IDs;
- add lifecycle states beyond Exploring, Selected, Blocked, Approved and Challenged;
- add implementation or validation states;
- require completed implementation or passing tests before architectural approval;
- create or amend ADRs;
- amend HJ-SM-001 or HJ-011;
- regenerate or amend HJ-106 or HJ-107;
- create or populate HJ-013; or
- generate implementation code.

## 8. Impacted Artefacts

| Artefact | Impact |
|---|---|
| **HJ-010 - Current Application Architectural Concerns** | Amend lifecycle, retain Approved concerns, reconcile the first approved batch and publish v1.0 Approved. |
| **HJ-012 - Established Application Architecture Patterns** | Create, populate with seven Approved resolutions and publish v1.0 Approved. |
| **HJ-106 - Vendor Registration Service Contract** | No immediate change; assess after the architecture baseline is published. |
| **HJ-107 - Vendor Registration Test Catalogue** | No immediate change; regenerate if required by downstream impact assessment. |
| **HJ-013 - Architecture and Implementation Test Catalogue** | Not created by this CR; generate subsequently from applicable architectural guarantees. |

## 9. Acceptance Criteria

CR-039 is satisfied when:

1. HJ-010 is the complete authoritative register of all 39 architectural concerns.
2. HJ-010 uses only Exploring, Selected, Blocked, Approved and Challenged as Resolution States.
3. Approved concerns remain in HJ-010.
4. CON-001–CON-005, CON-017 and CON-027 are Approved in HJ-010 with one Approach each.
5. HJ-012 contains exactly those seven concerns as Approved entries.
6. The HJ-010 and HJ-012 concern tables use the same nine-column structure and column order.
7. The seven entries, including Priority, are consistent across HJ-010 and HJ-012.
8. HJ-012 states that Challenged entries are retained but cease to be active implementation authority.
9. HJ-010 governs reconsideration of Challenged concerns under their stable Concern IDs.
10. No additional states, Pattern IDs, iteration fields or implementation-validation states are introduced.
11. Both HJ-010 and HJ-012 are published as version 1.0, Approved.

## 10. Follow-up Work

After HJ-010 and HJ-012 have been published and reconciled:

1. assess whether the approved architecture requires changes to HJ-106 or other authoritative Vendor Registration artefacts;
2. regenerate HJ-107 from HJ-106 and its applicable authoritative sources where required;
3. create HJ-013 from the applicable approved architectural guarantees and verification treatments;
4. generate or amend implementation from the approved artefact set; and
5. execute the applicable HJ-107 and HJ-013 tests.

Test failures shall first be classified as implementation defects, incorrectly derived tests or potential architectural deficiencies. Only a potential architectural deficiency changes the affected concern to Challenged.

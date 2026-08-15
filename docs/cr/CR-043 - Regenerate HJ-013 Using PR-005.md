# CR-043 - Regenerate HJ-013 Using PR-005

## 1. Change Summary

Regenerate **HJ-013 - Architecture and Implementation Test Catalogue** using **PR-005 - Generate Architecture and Implementation Test Catalogue** and publish the result as **HJ-013 v0.3 Draft**.

The regeneration shall:

- adopt PR-005 as the governing HJ-013 generation method;
- make the architecture and active delivery-scope boundaries explicit;
- add source-authority, concern-traceability, generation-readiness and reconciliation sections;
- correct the verification responsibility of `AI-AGG-003`;
- narrow `AI-VO-002` to invariant-preserving construction architecture and immutability;
- change `AI-VO-002` from **Dependent** to **Derivable**;
- preserve all 24 stable `AI-*` IDs;
- preserve all seven Approved HJ-012 concern states;
- preserve the HJ-107/HJ-013 behavioural-versus-architecture responsibility boundary; and
- record that no HJ-107 `VR-*` identifier is created, changed, retired or reconciled.

CR-043 amends HJ-013 only. It does not amend PR-005, HJ-010, HJ-012, HJ-107 or any authoritative architecture, Domain or service artefact.

## 2. Reason for Change

HJ-013 v0.2 was produced before PR-005 established a durable, scope-parameterised generation method for the Architecture and Implementation Test Catalogue.

A PR-005 Verification Only execution against HJ-013 v0.2 demonstrated that:

- the existing architecture baseline remains sound;
- all seven Approved concerns remain applicable to the current delivery scope;
- all 24 `AI-*` obligations can be preserved;
- no HJ-107 behavioural obligation is missing;
- no unresolved Approach needs to be selected;
- no Approved concern needs to become Challenged; and
- the catalogue benefits materially from explicit authority, scope, traceability, readiness and reconciliation structure.

The verification also confirmed two HJ-013 derivation defects.

First, `AI-AGG-003` assigned Aggregate rehydration responsibility ambiguously by delegating “aggregate equivalence” and repository CRUD to `AI-REP-003`, while `AI-REP-003` assigned detailed Aggregate fidelity back to `AI-AGG-003`. Aggregate fidelity and repository operation semantics require distinct, independently diagnosable ownership.

Second, `AI-VO-002` combined derivable Value Object immutability evidence with unresolved EmailAddress and TelephoneNumber validation profiles. This unnecessarily made the complete obligation Dependent on CON-026 even though accepted and rejected business values are already owned by HJ-107 and can remain separately dependent there.

The resulting candidate is structurally significant but architecturally modest. It introduces no new Approved Approach or Required Guarantee, but one obligation changes generation readiness. Controlled publication is therefore required.

## 3. Authoritative Generation Baseline

Use the following execution context when applying this CR:

| Input Role | Required Baseline |
|---|---|
| **Governing prompt** | PR-005 - Generate Architecture and Implementation Test Catalogue |
| **Approved architecture baseline** | HJ-012 v1.0 Approved |
| **Complete concern register** | HJ-010 v1.0 Approved |
| **Active delivery-scope artefact** | HJ-011 v1.1 Approved |
| **Applicable System Model** | HJ-SM-001 v1.0 Approved |
| **Domain model** | HJ-004 v2.3 Approved |
| **Service boundary** | HJ-106 v1.1 Approved |
| **Behavioural catalogue** | HJ-107 v0.2 Draft |
| **Stable `AI-*` baseline** | HJ-013 v0.2 Draft |
| **Engineering and test standards** | HJ-005 v2.0, HJ-006 v2.0 and HJ-007 v2.0 Approved |
| **Applicable decisions** | ADR-001, ADR-003, ADR-004 and ADR-008 Accepted |

The PR-005 verification candidate and assessment are evidence supporting this Change Request. They are not substitutes for the authoritative baselines above.

## 4. Required Changes to HJ-013

### 4.1 Document Control

Update HJ-013 to:

| Field | Required Value |
|---|---|
| **Document ID** | HJ-013 |
| **Document Title** | Architecture and Implementation Test Catalogue |
| **Version** | 0.3 |
| **Status** | Draft |
| **Classification** | Test Catalogue |
| **Owner** | Project Architecture / Engineering |
| **Last Updated** | Date of controlled publication |

Use the standard filename:

```text
HJ-013 - Architecture and Implementation Test Catalogue.md
```

Do not append a version or status suffix to the filename.

Add a v0.3 Revision History entry stating that the catalogue was regenerated using PR-005; adds explicit authority, scope, traceability, readiness and reconciliation; corrects `AI-AGG-003` and `AI-VO-002`; preserves all 24 `AI-*` IDs; and changes no Approved concern or behavioural-catalogue ID.

### 4.2 Related Documents and Source Authority

Add PR-005 as the governing prompt.

Add or make explicit:

- HJ-011 v1.1 as the active delivery-scope authority;
- HJ-SM-001 v1.0 as the applicable visible system context;
- HJ-012 v1.0 as the primary Approved architecture source;
- HJ-010 v1.0 as the complete concern and dependency source;
- HJ-107 v0.2 as the upstream behavioural catalogue and stable `VR-*` reference source; and
- HJ-013 v0.2 as the reconciliation baseline only.

Add a **Source Authority** section that distinguishes the authority of each input. State explicitly that:

- HJ-SM-001 visibility does not create delivery scope;
- HJ-107 supplies behavioural ownership and references but does not approve architecture; and
- HJ-013 v0.2 preserves stable IDs but is not independent architectural authority.

### 4.3 Scope Boundary

Add a scope section identifying:

| Boundary | Required Treatment |
|---|---|
| **Approved architecture** | HJ-012 v1.0: CON-001 to CON-005, CON-017 and CON-027 |
| **Active delivery scope** | HJ-011 v1.1 Epic 1 Vendor Registration |
| **Applicable operations** | RegisterVendor and RetrieveRegisteredVendor |
| **Applicable visible architecture** | Vendor Domain, persistence, reliable publication and intended Address/Compliance boundaries where exercised by HJ-011 |
| **Visible but excluded architecture** | Later Vendor lifecycle commands, Identity behaviour, future queries/read models and other capabilities excluded by HJ-011 |

State that Approved architecture is instantiated only within the active delivery boundary. The wider Domain Model and System Model shall not independently expand HJ-013 v0.3.

### 4.4 AI-AGG-003 Responsibility Correction

Retain stable Test ID `AI-AGG-003` and its existing essential requirement, Verification Form, Classification / Level, Priority, dependency and status.

Replace its Expected Evidence with wording equivalent to:

> A production-equivalent database fixture rehydrates the complete Aggregate with owned-state fidelity and valid invariants through the Aggregate Root. Repository save, lookup and absence semantics remain independently owned by AI-REP-003.

`AI-AGG-003` shall own:

- complete Aggregate consistency-boundary rehydration;
- Aggregate-owned-state fidelity; and
- invariant-valid rehydration through the Aggregate Root.

`AI-REP-003` shall continue to own:

- repository save semantics;
- lookup by VendorId;
- controlled absence; and
- infrastructure non-leakage.

Do not assign Aggregate fidelity or equivalence circularly between the two obligations.

### 4.5 AI-VO-002 Responsibility and Status Correction

Retain stable Test ID `AI-VO-002`.

Change its Title to wording equivalent to:

> Epic 1 Value Objects preserve invariants and immutability

Change its Requirement to wording equivalent to:

> Applicable Value Objects expose invariant-preserving construction and cannot be mutated after construction; accepted and rejected business values remain owned by HJ-107.

Change its Expected Evidence to wording equivalent to:

> Construction and structural tests demonstrate controlled creation and absence of mutating public state without duplicating HJ-107 validation profiles.

Change its Dependency to wording equivalent to:

> None; CON-026 continues to govern exhaustive validation allocation and profiles in HJ-107, not this structural obligation.

Change its Derivation Status:

```text
Dependent -> Derivable
```

Retain its existing Verification Form, Domain / Unit classification and P1 Priority.

This change shall not approve an EmailAddress or TelephoneNumber validation profile. Those business-validation boundaries remain governed by HJ-107, CON-026 and the required authoritative validation sources.

### 4.6 Concern-to-Test Traceability

Add a **Concern-to-Test Traceability** section covering all seven Approved concerns.

For each concern identify:

- Approved Concern ID;
- Required Guarantee;
- associated `AI-*` IDs;
- applicable HJ-107 coverage;
- coverage status; and
- unresolved dependencies.

Every one of the 24 `AI-*` obligations shall resolve to one or more applicable Approved concerns.

### 4.7 Derivation Findings and Dependencies

Retain the valid dependency findings for:

- CON-009 and CON-010;
- CON-016;
- CON-018 to CON-021;
- CON-024;
- CON-026;
- CON-028; and
- CON-037.

Clarify that CON-026 continues to block exhaustive validation allocation and profiles, but no longer blocks the narrowed structural `AI-VO-002` obligation.

Record that the controlled regeneration identifies:

- no behavioural-catalogue omission;
- no authoritative source conflict;
- no unresolved stable-ID mapping; and
- no evidence requiring an Approved concern to become Challenged.

### 4.8 Generation Readiness

Add a **Generation Readiness** section with these results:

| Readiness | Obligations |
|---|---|
| **Derivable** | AI-VO-002; AI-REP-002; AI-QRY-001 |
| **HJ-107 Coverage** | AI-AGG-001; AI-ENT-001; AI-DE-002; AI-OUT-004; AI-QRY-002; AI-QRY-003 |
| **Dependent** | The remaining 15 obligations |

State that:

- HJ-107 governs readiness of its own behavioural obligations;
- HJ-013 creates, changes and reconciles no `VR-*` ID; and
- readiness for test-candidate generation does not authorise unrestricted implementation generation across unresolved Current Concerns.

### 4.9 Regeneration Reconciliation

Add an `AI-*` reconciliation table recording:

| AI Test ID | Previous Treatment | Regenerated Treatment | Reason |
|---|---|---|---|
| AI-AGG-003 | Aggregate rehydration with ambiguous delegation to AI-REP-003 | Complete Aggregate fidelity remains with AI-AGG-003; only repository operation semantics remain with AI-REP-003 | Remove circular responsibility |
| AI-VO-002 | Combined immutability with unresolved validation profiles; Dependent | Narrowed structural obligation; Derivable | Remove avoidable blocking while retaining behavioural validation authority in HJ-107 |

Record totals:

- 24 preserved `AI-*` IDs;
- two materially amended under their existing IDs;
- 22 otherwise unchanged IDs;
- zero added IDs;
- zero retired, merged, split, superseded or reused IDs; and
- zero unresolved stable-ID mappings.

Add behavioural-reference reconciliation recording:

- HJ-107 v0.2 as the current behavioural baseline;
- zero changed `VR-*` references;
- zero missing referenced `VR-*` IDs; and
- zero `VR-*` IDs created, changed, retired or reconciled by HJ-013 regeneration.

### 4.10 Review Checklist and Next Steps

Add the PR-005 review checklist and record the completion result for this regeneration.

The checklist shall confirm at minimum that HJ-013 v0.3:

- uses HJ-011 rather than System Model visibility to define active delivery scope;
- contains only the seven applicable Approved concerns;
- preserves all 24 `AI-*` IDs;
- creates or changes no `VR-*` ID;
- contains no duplicate behavioural obligation;
- contains no circular verification responsibility;
- separates derivable evidence from unresolved evidence;
- uses only approved HJ-006 classifications and levels;
- selects no unresolved Approach;
- invents no operation, mapping, schema, technology or lifecycle transition; and
- identifies no evidence requiring an Approved concern to become Challenged.

Replace the previous requirement for another full gap-analysis cycle with a focused completion check of:

- `AI-AGG-003`;
- `AI-VO-002`;
- `AI-*` reconciliation totals;
- behavioural-reference reconciliation; and
- the new authority, scope, traceability and readiness sections.

## 5. Required Preservation

Preserve without material change:

- the HJ-013 Document ID, title, classification and owner;
- the standard filename;
- all seven Approved concern mappings;
- all 24 stable `AI-*` IDs;
- every obligation other than the authorised `AI-AGG-003` and `AI-VO-002` amendments;
- the controlled Verification Forms;
- the approved HJ-006 Classification / Level mapping;
- the Derivable, Dependent and HJ-107 status model;
- all valid unresolved Current Concern dependencies;
- the HJ-107/HJ-013 responsibility boundary; and
- the rule that dependency alone does not challenge Approved architecture.

## 6. Explicit Non-Changes

CR-043 does not:

- change HJ-010;
- change HJ-012;
- change any Approved concern, Required Guarantee, Approach, Resolution State or Priority;
- change HJ-107;
- create, amend, retire, renumber or reconcile a `VR-*` identifier;
- change HJ-106 service behaviour;
- approve any unresolved Current Concern;
- select an Address contract, validation profile, transaction boundary, event contract, relay, broker policy, PostgreSQL mapping or enforcement mechanism;
- introduce a later Vendor lifecycle operation;
- change HJ-006 classifications or levels;
- generate executable test code or implementation code; or
- amend PR-005 or any artefact other than HJ-013.

## 7. Impacted Artefacts

| Artefact | Impact |
|---|---|
| **HJ-013 - Architecture and Implementation Test Catalogue** | Regenerate and publish as v0.3 Draft with the authorised structural and obligation corrections. |
| **PR-005 - Generate Architecture and Implementation Test Catalogue** | No change; governs regeneration. |
| **HJ-010 - Current Application Architectural Concerns** | No change; remains the concern and dependency source. |
| **HJ-012 - Established Application Architecture Patterns** | No change; all seven Approved concerns remain Approved. |
| **HJ-107 - Vendor Registration Test Catalogue** | No change; remains the behavioural catalogue and `VR-*` reference source. |
| **HJ-011 / HJ-SM-001 / HJ-004 / HJ-106 / standards / ADRs** | No change; remain generation inputs within their authority. |

## 8. Acceptance Criteria

CR-043 is satisfied when HJ-013:

1. is published using the standard filename as v0.3 Draft.
2. identifies PR-005 as its governing generation prompt.
3. identifies HJ-012 v1.0 as its Approved architecture baseline.
4. identifies HJ-011 v1.1 as its active delivery-scope authority.
5. states that HJ-SM-001 visibility does not create delivery scope.
6. retains CON-001 to CON-005, CON-017 and CON-027 as the seven applicable Approved concerns.
7. preserves all 24 stable `AI-*` IDs.
8. corrects `AI-AGG-003` so Aggregate fidelity and repository operation semantics have distinct ownership.
9. narrows `AI-VO-002` to invariant-preserving construction architecture and immutability.
10. changes `AI-VO-002` from Dependent to Derivable without approving validation profiles.
11. contains three Derivable, fifteen Dependent and six HJ-107 Coverage obligations.
12. adds complete Concern-to-Test Traceability.
13. adds Generation Readiness and stable-ID reconciliation.
14. records two materially amended and 22 otherwise unchanged `AI-*` obligations.
15. records zero added, retired, merged, split, superseded or reused `AI-*` IDs.
16. records zero changed or missing `VR-*` references.
17. selects no unresolved Approach or missing technical contract.
18. identifies no evidence requiring an Approved concern to become Challenged.
19. changes no HJ-010, HJ-012, HJ-107 or service-contract content.
20. contains no executable test or implementation code.

## 9. Completion Check

After applying CR-043, perform a focused check confirming:

- `AI-AGG-003` owns complete Aggregate fidelity without circular delegation;
- `AI-REP-003` owns repository operation semantics without taking Aggregate fidelity;
- `AI-VO-002` is Derivable and does not depend on CON-026;
- CON-026 remains visible for exhaustive validation allocation and profiles;
- all 24 `AI-*` IDs are present exactly once in the active catalogue;
- the active catalogue contains three Derivable, fifteen Dependent and six HJ-107 Coverage obligations;
- every Approved concern has traceable verification coverage;
- all referenced behavioural IDs exist in HJ-107 v0.2;
- no `VR-*` ID was modified by HJ-013 regeneration; and
- document control and Revision History identify v0.3 Draft and PR-005.

If all checks pass, another full architecture gap analysis is not required before moving to resolution of the Current Concerns that block the selected implementation slice.

## 10. Follow-up Work

After CR-043 is applied and the focused completion check passes:

1. select the next Current Concerns to resolve according to implementation dependency and risk;
2. regenerate affected downstream catalogues only where their authoritative inputs materially change;
3. generate test candidates for Derivable HJ-013 obligations and ready HJ-107 obligations;
4. constrain implementation generation to approved architecture and resolved dependencies; and
5. update HJ-013 through PR-005 when subsequent Approved architecture batches or delivery scopes introduce new verification obligations.

# CR-044 - Align HJ-010 and HJ-012 with Approved Test Catalogues

## 1. Change Summary

Amend:

- **HJ-010 - Current Application Architectural Concerns**; and
- **HJ-012 - Established Application Architecture Patterns**

to recognise the publication of:

- **HJ-107 v1.0 - Vendor Registration Test Catalogue**, Approved; and
- **HJ-013 v1.0 - Architecture and Implementation Test Catalogue**, Approved.

Remove obsolete references describing HJ-107 as Draft and HJ-013 as Planned or not yet created. Update downstream-generation language so both catalogues are regenerated independently only where their authoritative inputs materially change.

Publish the revised HJ-010 and HJ-012 as **v1.1 Approved** under their existing standard filenames.

CR-044 amends HJ-010 and HJ-012 only. It does not amend HJ-107, HJ-013, their test obligations, any architectural concern or any Approved Approach.

## 2. Reason for Change

HJ-010 v1.0 and HJ-012 v1.0 were published while:

- HJ-107 remained Draft; and
- HJ-013 was planned or still being generated.

HJ-107 and HJ-013 have now been promoted to v1.0 Approved. Their publication does not change the architecture baseline, but several HJ-010 and HJ-012 references are now factually obsolete:

- HJ-010 lists HJ-107 as Draft and does not list HJ-013 in Related Documents;
- HJ-010 describes HJ-013 as planned;
- HJ-010 instructs that HJ-013 be created rather than regenerated when required;
- HJ-012 lists HJ-107 as Draft;
- HJ-012 lists HJ-013 as Planned; and
- HJ-012 describes HJ-013 generation as though no controlled catalogue exists.

The approved catalogue relationship is now:

```text
authoritative behavioural inputs
    -> independently regenerate HJ-107 where materially affected

Approved architecture + active delivery scope + current HJ-107
    -> independently regenerate HJ-013 where materially affected
```

HJ-107 owns behavioural test obligations and stable `VR-*` identifiers. HJ-013 owns complementary architecture and implementation verification and stable `AI-*` identifiers. Approval of either catalogue does not merge their responsibilities or introduce a circular regeneration dependency.

## 3. Required Changes to HJ-010

### 3.1 Document Control

Update HJ-010 to:

| Field | Required Value |
|---|---|
| **Version** | 1.1 |
| **Status** | Approved |
| **Last Updated** | Date CR-044 is applied |

Add a v1.1 Revision History entry stating that CR-044 aligned HJ-010 with HJ-107 v1.0 Approved and HJ-013 v1.0 Approved, removed obsolete planned/creation language and changed no concern, Approach, Resolution State, Priority or verification responsibility.

Retain the standard filename:

```text
HJ-010 - Current Application Architectural Concerns.md
```

### 3.2 Related Documents

Replace:

```markdown
| HJ-107 | Vendor Registration Test Catalogue | Draft | Catalogues behavioural verification derived from HJ-106 and its approved sources. |
```

with:

```markdown
| HJ-107 v1.0 | Vendor Registration Test Catalogue | Approved | Approved behavioural-verification catalogue derived from HJ-106 and its authoritative sources. |
| HJ-013 v1.0 | Architecture and Implementation Test Catalogue | Approved | Approved architecture and implementation verification catalogue derived from applicable Approved architecture and delivery scope. |
```

Do not add HJ-013 test details or copy test obligations into HJ-010.

### 3.3 Verification Rule

Replace:

> Verification may be provided through HJ-107, the planned **HJ-013 - Architecture and Implementation Test Catalogue**, static analysis, build enforcement, deployment/configuration validation, runtime evidence, code review or architecture review.

with:

> Verification may be provided through HJ-107, **HJ-013 - Architecture and Implementation Test Catalogue**, static analysis, build enforcement, deployment/configuration validation, runtime evidence, code review or architecture review.

Retain the following sentence unchanged:

> HJ-010 references the destination; it does not copy the underlying test case.

### 3.4 Verification Architecture

Replace:

> Architecture-specific guarantees will be directed to the planned **HJ-013 - Architecture and Implementation Test Catalogue** or another explicitly governed mechanism.

with:

> Architecture-specific guarantees are catalogued in **HJ-013 - Architecture and Implementation Test Catalogue** or verified through another explicitly governed mechanism.

Retain the statement that HJ-010 identifies the verification destination without defining the test specification.

### 3.5 Downstream Flow

In the downstream flow, replace:

```text
    -> regenerate HJ-107
    -> generate HJ-013
```

with:

```text
    -> independently regenerate HJ-107 where its authoritative inputs materially change
    -> independently regenerate HJ-013 where its authoritative inputs materially change
```

Retain implementation generation and test-execution steps after catalogue regeneration.

This wording shall preserve the one-way reference direction from current HJ-107 into HJ-013 without implying that an HJ-013-only change triggers HJ-107 regeneration.

### 3.6 Open Decisions and Follow-up

Replace:

```markdown
4. create HJ-013 - Architecture and Implementation Test Catalogue from the applicable approved architectural guarantees;
```

with:

```markdown
4. regenerate HJ-013 from the applicable Approved architectural guarantees and active delivery scope where required;
```

Retain the separate HJ-107 regeneration step and all remaining follow-up items.

## 4. Required Changes to HJ-012

### 4.1 Document Control

Update HJ-012 to:

| Field | Required Value |
|---|---|
| **Version** | 1.1 |
| **Status** | Approved |
| **Last Updated** | Date CR-044 is applied |

Add a v1.1 Revision History entry stating that CR-044 aligned HJ-012 with HJ-107 v1.0 Approved and HJ-013 v1.0 Approved, replaced initial-generation wording with independent regeneration rules and changed no Approved concern, Approach, Required Guarantee or verification responsibility.

Retain the standard filename:

```text
HJ-012 - Established Application Architecture Patterns.md
```

### 4.2 Related Documents

Replace:

```markdown
| HJ-107 | Vendor Registration Test Catalogue | Draft | Generated behavioural test catalogue for applicable externally observable guarantees. |
| HJ-013 | Architecture and Implementation Test Catalogue | Planned | Generated test catalogue for application architecture and implementation guarantees. |
```

with:

```markdown
| HJ-107 v1.0 | Vendor Registration Test Catalogue | Approved | Approved behavioural test catalogue for applicable externally observable guarantees. |
| HJ-013 v1.0 | Architecture and Implementation Test Catalogue | Approved | Approved test catalogue for application architecture and implementation guarantees. |
```

### 4.3 Implementation and Verification Derivation

Replace:

```text
    -> regenerate HJ-107 behavioural tests
    -> generate HJ-013 architecture and implementation tests
```

with:

```text
    -> independently regenerate HJ-107 behavioural tests where its authoritative inputs materially change
    -> independently regenerate HJ-013 architecture and implementation tests where its authoritative inputs materially change
```

Retain the subsequent implementation-generation and test-execution steps.

Retain the existing responsibility boundary:

- HJ-107 verifies externally observable Vendor Registration behaviour derived from HJ-106 and its authoritative sources; and
- HJ-013 verifies application architecture, dependency, persistence, transaction, reliable-publication and other implementation guarantees derived from applicable Approved architecture, ADRs and engineering standards.

### 4.4 Initial Batch Reconciliation

Replace:

```markdown
| **Downstream generation** | Assess HJ-106 impact, then regenerate HJ-107 and generate HJ-013 where required. |
```

with:

```markdown
| **Downstream generation** | Assess HJ-106 impact, then independently regenerate HJ-107 and HJ-013 where their authoritative inputs materially change. |
```

Retain the approved-concern, approved-Approach, HJ-010 and HJ-012 reconciliation results.

## 5. Required Preservation

Preserve without material change:

- all 39 HJ-010 concern IDs and rows;
- every HJ-010 Architectural Concern and Required Guarantee;
- every HJ-010 Approach and candidate-Approach list;
- every HJ-010 Resolution State and Priority;
- every HJ-010 Decision Treatment / Source and Verification Treatment;
- all seven HJ-012 Approved concern entries;
- every HJ-012 Required Guarantee, Approach, Resolution State and Priority;
- the HJ-010/HJ-012 concern lifecycle and Challenged process;
- the HJ-107/HJ-013 responsibility boundary;
- the rule that generated catalogues must not invent unresolved architectural semantics; and
- the standard filenames of HJ-010 and HJ-012.

No concern-table cell requires amendment under CR-044.

## 6. Explicit Non-Changes

CR-044 does not:

- amend HJ-107 or any `VR-*` identifier;
- amend HJ-013 or any `AI-*` identifier;
- amend HJ-106 or service behaviour;
- add, remove, merge, split, approve, block or challenge an architectural concern;
- change an Approach, Required Guarantee, Resolution State or Priority;
- select an unresolved candidate Approach;
- change an ADR or engineering standard;
- require HJ-107 regeneration because HJ-013 changes;
- require HJ-013 regeneration when its authoritative inputs are unchanged;
- generate test or implementation code; or
- amend any artefact other than HJ-010 and HJ-012.

## 7. Impacted Artefacts

| Artefact | Impact |
|---|---|
| **HJ-010 - Current Application Architectural Concerns** | Publish as v1.1 Approved with updated catalogue references and regeneration language. |
| **HJ-012 - Established Application Architecture Patterns** | Publish as v1.1 Approved with updated catalogue references and regeneration language. |
| **HJ-107 v1.0 - Vendor Registration Test Catalogue** | No change; referenced as Approved. |
| **HJ-013 v1.0 - Architecture and Implementation Test Catalogue** | No change; referenced as Approved. |
| **All other artefacts** | No change. |

## 8. Acceptance Criteria

CR-044 is satisfied when:

1. HJ-010 is published as v1.1 Approved under its standard filename.
2. HJ-012 is published as v1.1 Approved under its standard filename.
3. HJ-010 Related Documents identifies HJ-107 v1.0 and HJ-013 v1.0 as Approved.
4. HJ-012 Related Documents identifies HJ-107 v1.0 and HJ-013 v1.0 as Approved.
5. neither artefact describes HJ-013 as Planned.
6. neither artefact instructs that HJ-013 be created as though no controlled catalogue exists.
7. both artefacts describe HJ-107 and HJ-013 regeneration as independent and conditional on material changes to their respective authoritative inputs.
8. HJ-010 continues to identify verification destinations without copying test specifications.
9. HJ-012 retains the behavioural-versus-architecture catalogue responsibility boundary.
10. all 39 HJ-010 concern rows remain unchanged.
11. all seven HJ-012 Approved concern rows remain unchanged.
12. every existing concern state, Approach, Required Guarantee, Priority and verification responsibility is preserved.
13. HJ-107 and HJ-013 remain unchanged at v1.0 Approved.
14. no `VR-*` or `AI-*` identifier is created, amended, retired or reconciled.

## 9. Completion Check

After applying CR-044, verify that:

- HJ-010 document control shows v1.1 Approved;
- HJ-012 document control shows v1.1 Approved;
- both Revision Histories identify CR-044;
- all Related Documents statuses for HJ-107 and HJ-013 are Approved;
- `planned HJ-013`, `HJ-013 | ... | Planned` and `create HJ-013` no longer appear;
- the downstream flows state independent, input-driven regeneration;
- the HJ-010 concern table is byte-for-byte unchanged;
- the HJ-012 Approved-pattern table is byte-for-byte unchanged; and
- no source artefact outside HJ-010 and HJ-012 was amended.

## 10. Follow-up Work

After CR-044 is applied and the completion check passes:

1. commit HJ-010 v1.1, HJ-012 v1.1, HJ-107 v1.0, HJ-013 v1.0 and their governing prompts/change records as the aligned architecture and verification baseline;
2. resolve the next Current Architectural Concerns according to implementation dependency and risk; and
3. regenerate HJ-107 or HJ-013 independently only when their respective authoritative inputs materially change.

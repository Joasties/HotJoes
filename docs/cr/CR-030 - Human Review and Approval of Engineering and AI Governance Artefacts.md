# CR-030 – Human Review and Approval of Engineering and AI Governance Artefacts

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-030 |
| **Title** | Human Review and Approval of Engineering and AI Governance Artefacts |
| **Status** | Approved |
| **Owner** | Project Architecture |
| **Priority** | Medium |
| **Primary Affected Documents** | HJ-005, HJ-006, HJ-007, HJ-008, HJ-009 |
| **Downstream / Referentially Affected Documents** | HJ-001, HJ-002, HJ-005, HJ-006, HJ-007, HJ-008, HJ-009, HJ-106, ADR-000, plus any additional maintained repository artefact found by the final verification search |

---

# 1. Background

The core HotJoes business and Vendor architectural artefacts have now been human reviewed and form an Approved baseline.

The remaining core engineering and AI-governance documents are still recorded as Draft:

- HJ-005 – Coding Standards, Version 1.0;
- HJ-006 – Testing Strategy and Standards, Version 1.1;
- HJ-007 – Enforcement Strategy, Version 1.0;
- HJ-008 – AI Roles and Responsibilities, Version 1.0; and
- HJ-009 – AI Operating Guide, Version 0.2.

These documents have been used throughout the architecture and delivery process and are now to be formally recorded as **Human Reviewed and Approved**.

This Change Request is principally an approval, documentation-governance and repository-alignment change.

It shall not silently change the substantive engineering or AI-governance rules contained in the documents.

HJ-009 additionally retains an older converted Markdown presentation that is visibly inconsistent with the standard structure used by HJ-003 and the later HotJoes architectural artefacts. HJ-009 shall therefore receive a presentation-only Markdown normalisation as part of its approval.

The change shall be applied by Codex directly to the existing repository documents. Codex shall not generate replacement artefacts outside the repository.

---

# 2. Objectives

This Change Request shall:

1. formally record Human Review and Approval of HJ-005 through HJ-009;
2. promote each document to its approved version;
3. update Status, Last Updated and Revision History;
4. preserve all existing substantive engineering and AI-governance meaning;
5. normalise HJ-009 to the current HotJoes Markdown presentation convention;
6. update Related Documents and other current-status references throughout the supplied architectural baseline;
7. correct prose that still describes a newly Approved document as Draft;
8. preserve genuine historical references in revision histories;
9. perform the work starting with HJ-005 and proceeding downstream through HJ-009; and
10. leave no maintained repository artefact incorrectly identifying HJ-005, HJ-006, HJ-007, HJ-008 or HJ-009 as Draft.

---

# 3. Authoritative Execution Order

Codex shall apply CR-030 in this order:

1. HJ-005 – Coding Standards
2. HJ-006 – Testing Strategy and Standards
3. HJ-007 – Enforcement Strategy
4. HJ-008 – AI Roles and Responsibilities
5. HJ-009 – AI Operating Guide
6. downstream and cross-document status reconciliation
7. repository-wide verification

The ordering is intentional.

Each document shall be promoted before documents downstream from it are treated as depending upon an Approved version.

The final reconciliation pass shall then ensure that earlier documents whose Related Documents tables reference later-promoted documents also reflect the final repository state.

---

# 4. Approval Version Convention

Apply the following version promotions:

| Document | Existing Version | Existing Status | Target Version | Target Status |
|---|---:|---|---:|---|
| HJ-005 – Coding Standards | 1.0 | Draft | **2.0** | **Approved** |
| HJ-006 – Testing Strategy and Standards | 1.1 | Draft | **2.0** | **Approved** |
| HJ-007 – Enforcement Strategy | 1.0 | Draft | **2.0** | **Approved** |
| HJ-008 – AI Roles and Responsibilities | 1.0 | Draft | **2.0** | **Approved** |
| HJ-009 – AI Operating Guide | 0.2 | Draft | **1.0** | **Approved** |

The major-version change records the establishment of the first formally Human Reviewed and Approved baseline for each document.

Do not renumber or rewrite earlier revision-history entries.

---

# 5. Common Changes Required for HJ-005 to HJ-009

For each document:

## 5.1 Metadata

Change:

- Version to the target version specified in Section 4;
- Status from `Draft` to `Approved`;
- Last Updated to the date CR-030 is applied.

Retain:

- Document ID;
- Document Title;
- Classification;
- Owner.

---

## 5.2 Revision History

Append a new revision-history entry for the approved version.

The entry shall make clear that:

- human review has completed;
- the document has been promoted from Draft to Approved;
- the existing substantive guidance has not been changed as part of the promotion; and
- Related Documents/status references have been reconciled where applicable.

For HJ-009, additionally record the Markdown presentation alignment.

Recommended descriptions are provided in the document-specific sections below.

---

## 5.3 Existing Content

Except for the explicit HJ-009 presentation normalisation, CR-030 shall not be used as an opportunity to rewrite or improve substantive content.

Do not silently:

- add engineering rules;
- remove engineering rules;
- change testing policy;
- change enforcement policy;
- change AI roles;
- change AI authority;
- change human accountability;
- change workflow semantics;
- change architectural boundaries;
- resolve unrelated defects.

Any substantive issue discovered while applying CR-030 shall be reported separately.

---

# 6. HJ-005 – Coding Standards

## 6.1 Metadata

Change:

```text
Version: 1.0
Status: Draft
Last Updated: 21 July 2026
```

to:

```text
Version: 2.0
Status: Approved
Last Updated: <CR-030 application date>
```

---

## 6.2 Revision History

Append:

```text
2.0 | <CR-030 application date> | Completed human review and promoted Coding Standards from Draft to Approved. Existing coding standards were retained unchanged. Related-document status references were reconciled with the approved repository baseline.
```

---

## 6.3 Related Documents – Final Required State

HJ-005 currently references:

- HJ-001 – Approved;
- HJ-002 – Approved;
- HJ-003 – Approved;
- HJ-004 – Approved;
- HJ-006 – Draft;
- HJ-007 – Draft;
- HJ-008 – Draft.

After all CR-030 promotions have completed, update:

```text
HJ-006 | Testing Strategy and Standards | Draft
HJ-007 | Enforcement Strategy | Draft
HJ-008 | AI Roles and Responsibilities | Draft
```

to:

```text
HJ-006 | Testing Strategy and Standards | Approved
HJ-007 | Enforcement Strategy | Approved
HJ-008 | AI Roles and Responsibilities | Approved
```

No other HJ-005 Related Documents status requires alteration.

---

## 6.4 Content Constraint

Do not change the Coding Standards body.

The promotion does not amend its:

- engineering principles;
- naming guidance;
- API guidance;
- domain-model guidance;
- persistence guidance;
- error-handling guidance;
- review standards;
- prohibited practices;
- exception mechanism;
- enforcement expectations.

---

# 7. HJ-006 – Testing Strategy and Standards

## 7.1 Metadata

Change:

```text
Version: 1.1
Status: Draft
Last Updated: 9 August 2026
```

to:

```text
Version: 2.0
Status: Approved
Last Updated: <CR-030 application date>
```

---

## 7.2 Revision History

Append:

```text
2.0 | <CR-030 application date> | Completed human review and promoted Testing Strategy and Standards from Draft to Approved. Existing testing strategy and standards were retained unchanged. Related-document status references were reconciled with the approved repository baseline.
```

---

## 7.3 Related Documents – Final Required State

Current stale entries:

```text
HJ-005 | Coding Standards | Draft
HJ-007 | Enforcement Strategy | Draft
HJ-008 | AI Roles and Responsibilities | Draft
```

Replace in the final reconciled version with:

```text
HJ-005 | Coding Standards | Approved
HJ-007 | Enforcement Strategy | Approved
HJ-008 | AI Roles and Responsibilities | Approved
```

HJ-001 through HJ-004 are already correctly recorded as Approved.

---

## 7.4 Content Constraint

Do not alter the testing strategy or testing standards.

In particular, CR-030 shall not change:

- testing philosophy;
- Testing Pyramid;
- Unit Testing Standards;
- Integration Testing;
- End-to-End Testing;
- mocking guidance;
- test-data guidance;
- deterministic-test requirements;
- behaviour-versus-implementation guidance;
- CI testing expectations;
- test maintenance principles.

The existing body may remain in its current Markdown presentation. HJ-006 style normalisation is not part of this CR.

---

# 8. HJ-007 – Enforcement Strategy

## 8.1 Metadata

Change:

```text
Version: 1.0
Status: Draft
Last Updated: 21 July 2026
```

to:

```text
Version: 2.0
Status: Approved
Last Updated: <CR-030 application date>
```

---

## 8.2 Revision History

Append:

```text
2.0 | <CR-030 application date> | Completed human review and promoted Enforcement Strategy from Draft to Approved. Existing enforcement strategy was retained unchanged. Related-document status references were reconciled with the approved repository baseline.
```

---

## 8.3 Related Documents – Final Required State

Current stale entries:

```text
HJ-005 | Coding Standards | Draft
HJ-006 | Testing Strategy and Standards | Draft
HJ-008 | AI Roles and Responsibilities | Draft
HJ-009 | AI Operating Guide | Draft
```

Replace in the final reconciled version with:

```text
HJ-005 | Coding Standards | Approved
HJ-006 | Testing Strategy and Standards | Approved
HJ-008 | AI Roles and Responsibilities | Approved
HJ-009 | AI Operating Guide | Approved
```

HJ-001 through HJ-004 are already correctly recorded as Approved.

---

## 8.4 Content Constraint

Do not alter Enforcement Strategy behaviour or policy.

Specifically preserve:

- enforcement philosophy;
- IDE enforcement;
- `.editorconfig` usage;
- `dotnet format`;
- compiler/analyser controls;
- automated testing enforcement;
- CI enforcement;
- architectural enforcement;
- pull-request review;
- human-versus-automated enforcement responsibilities.

The existing body may remain in its current Markdown presentation. General HJ-007 style normalisation is outside CR-030.

---

# 9. HJ-008 – AI Roles and Responsibilities

## 9.1 Metadata

Change:

```text
Version: 1.0
Status: Draft
Last Updated: 21 July 2026
```

to:

```text
Version: 2.0
Status: Approved
Last Updated: <CR-030 application date>
```

---

## 9.2 Revision History

Append:

```text
2.0 | <CR-030 application date> | Completed human review and promoted AI Roles and Responsibilities from Draft to Approved. Existing AI roles, responsibilities, authority boundaries and human-accountability rules were retained unchanged. Related-document status references were reconciled with the approved repository baseline.
```

---

## 9.3 Related Documents – Final Required State

Current stale entries:

```text
HJ-005 | Coding Standards | Draft
HJ-006 | Testing Strategy and Standards | Draft
HJ-007 | Enforcement Strategy | Draft
```

Replace with:

```text
HJ-005 | Coding Standards | Approved
HJ-006 | Testing Strategy and Standards | Approved
HJ-007 | Enforcement Strategy | Approved
```

HJ-001 through HJ-004 are already correctly recorded as Approved.

---

## 9.4 Content Constraint

The AI authority model shall not change.

Preserve the existing principles that:

- AI assists people;
- AI does not own requirements;
- AI does not own architecture;
- AI does not own production systems;
- humans remain accountable;
- AI cannot approve its own work;
- AI cannot bypass review processes;
- AI work must be traceable;
- AI output must be verified before trust.

Do not add, remove, merge or redefine any AI role as part of CR-030.

---

# 10. HJ-009 – AI Operating Guide

HJ-009 requires both formal approval and an editorial Markdown presentation correction.

## 10.1 Metadata

Current:

```text
Version: 0.2
Status: Draft
Last Updated: 21 July 2026
```

Target:

```text
Version: 1.0
Status: Approved
Last Updated: <CR-030 application date>
```

---

## 10.2 Revision History

Preserve existing entries:

- 0.1 – Initial draft;
- 0.2 – Human-orchestrated AI operating model and associated workflow changes.

Append:

```text
1.0 | <CR-030 application date> | Completed human review and promoted the AI Operating Guide from Draft to Approved. Existing AI operating behaviour and human-governance rules were retained unchanged. Markdown presentation was aligned with the current HotJoes documentation standard and related-document status references were reconciled.
```

---

# 11. HJ-009 Markdown Presentation Alignment

HJ-009 currently uses a converted Markdown form that differs visibly from HJ-003.

The required correction is editorial only.

HJ-003 shall be used as the presentation reference.

## 11.1 Document Title

Replace:

```markdown
**HJ-009 -- AI Operating Guide**
```

with:

```markdown
# HJ-009 – AI Operating Guide
```

Use the same title-heading convention as HJ-003.

---

## 11.2 Metadata Table

Replace the current fixed-width converted table with:

```markdown
| Property | Value |
|----------|-------|
| **Document ID** | HJ-009 |
| **Document Title** | AI Operating Guide |
| **Version** | 1.0 |
| **Status** | Approved |
| **Classification** | Guide |
| **Owner** | Project Architecture |
| **Last Updated** | <CR-030 application date> |
```

---

## 11.3 Revision History Heading and Table

Replace the bold pseudo-heading:

```markdown
**Revision History**
```

with:

```markdown
## Revision History
```

Represent the revision history as a normal Markdown table:

```markdown
| Version | Date | Description |
|---------|------|-------------|
...
```

Preserve all existing revision-history information.

---

## 11.4 Related Documents Heading and Table

Replace:

```markdown
**Related Documents**
```

with:

```markdown
## Related Documents
```

Represent the content as a standard Markdown table:

```markdown
| Document ID | Title | Status |
|-------------|-------|--------|
...
```

---

## 11.5 Numbered Main Sections

Convert bold pseudo-headings such as:

```markdown
**1. Purpose**
**2. Objectives**
**3. Guiding Principles**
```

to proper Markdown headings using the same hierarchy as HJ-003:

```markdown
# 1. Purpose
# 2. Objectives
# 3. Guiding Principles
```

Apply this consistently to every numbered top-level section in HJ-009.

---

## 11.6 Subsections

Where HJ-009 uses bold text as a structural subsection heading, convert it to an appropriate Markdown heading level.

Examples include Guiding Principles such as:

```markdown
**GP-001 AI Assists --- Humans Decide**
```

which shall become an appropriate subsection heading, for example:

```markdown
## GP-001 – AI Assists — Humans Decide
```

Preserve the identifier and wording.

Presentation punctuation may be normalised to the same dash convention used elsewhere in the HotJoes documentation, but meaning shall not change.

---

## 11.7 Lists

Where HJ-009 contains genuine logical lists represented as separated plain-text lines, convert them into Markdown bullet lists.

For example, the Objectives section currently contains individual lines such as:

```text
ensure predictable AI behaviour

maximise engineering quality

minimise hallucination
```

Convert the complete objective set to Markdown bullets without changing wording or order.

Apply the same treatment to other genuine lists where the existing converted representation has lost list syntax.

Do not convert ordinary prose into lists merely for stylistic preference.

---

## 11.8 Workflow Diagrams and Structured Content

Preserve all current HJ-009 workflow semantics.

Do not:

- add workflow stages;
- remove workflow stages;
- reorder workflow stages;
- convert human orchestration into AI chaining;
- weaken human review gates;
- alter self-review requirements;
- alter escalation rules;
- alter tool-usage rules;
- alter role hand-off semantics;
- alter prompt lifecycle controls.

Formatting may change; behaviour may not.

---

# 12. HJ-009 Related Documents – Final Required State

Current HJ-009 entries already correctly show:

```text
HJ-001 | Project Vision | Approved
HJ-002 | Architectural Principles | Approved
HJ-003 | Ubiquitous Language Guide | Approved
HJ-004 | Vendor Domain Models | Approved
```

Change:

```text
HJ-005 | Coding Standards | Draft
HJ-006 | Testing Strategy and Standards | Draft
HJ-007 | Enforcement Strategy | Draft
HJ-008 | AI Roles and Responsibilities | Draft
```

to:

```text
HJ-005 | Coding Standards | Approved
HJ-006 | Testing Strategy and Standards | Approved
HJ-007 | Enforcement Strategy | Approved
HJ-008 | AI Roles and Responsibilities | Approved
```

---

# 13. Downstream and Cross-Document Status Reconciliation

After HJ-005 through HJ-009 have all been promoted, Codex shall update maintained repository artefacts that still describe them as Draft.

The attached approved baseline identifies the following required changes.

---

## 13.1 HJ-001 – Project Vision

HJ-001 is Version 2.0 Approved.

Its Related Documents table currently contains:

```text
HJ-005 | Coding Standards | Draft
HJ-006 | Testing Strategy and Standards | Draft
HJ-007 | Enforcement Strategy | Draft
HJ-008 | AI Roles and Responsibilities | Draft
```

Change to:

```text
HJ-005 | Coding Standards | Approved
HJ-006 | Testing Strategy and Standards | Approved
HJ-007 | Enforcement Strategy | Approved
HJ-008 | AI Roles and Responsibilities | Approved
```

Do not alter the ordinary non-status references to HJ-005 through HJ-008 elsewhere in HJ-001.

No HJ-009 status reference exists in the current HJ-001 Related Documents table.

HJ-001 remains Version 2.0 Approved. Do not version-bump it solely for these referential status corrections unless the repository's document-governance convention specifically requires such a bump for status-reference reconciliation.

---

## 13.2 HJ-002 – Architectural Principles

HJ-002 is Version 2.0 Approved.

Its Related Documents table currently contains:

```text
HJ-005 | Coding Standards | Draft
HJ-006 | Testing Strategy and Standards | Draft
HJ-007 | Enforcement Strategy | Draft
HJ-008 | AI Roles and Responsibilities | Draft
```

Change to:

```text
HJ-005 | Coding Standards | Approved
HJ-006 | Testing Strategy and Standards | Approved
HJ-007 | Enforcement Strategy | Approved
HJ-008 | AI Roles and Responsibilities | Approved
```

No substantive principle shall change.

HJ-002 remains Version 2.0 Approved subject to the same repository convention regarding purely referential corrections.

---

## 13.3 HJ-003 – Ubiquitous Language Guide

No status reference to HJ-005, HJ-006, HJ-007, HJ-008 or HJ-009 exists in the supplied Related Documents table.

**No CR-030 change is required.**

---

## 13.4 HJ-004 – Vendor Domain Models

No status reference to HJ-005, HJ-006, HJ-007, HJ-008 or HJ-009 exists in the supplied Related Documents table.

**No CR-030 change is required.**

---

## 13.5 HJ-104 – Vendor Registration Fields Matrix

HJ-104 contains ordinary textual references to HJ-005 as the location of implementation-specific validation guidance.

Those references do not state HJ-005's status.

**No CR-030 change is required.**

Do not alter these references merely because HJ-005 is being promoted.

---

## 13.6 HJ-105 – Vendor Registration Sequence Diagram

The supplied HJ-105 Version 3.2 Approved contains no current status reference to any of HJ-005 through HJ-009.

**No CR-030 change is required.**

---

## 13.7 HJ-106 – Vendor Registration Service Contract

HJ-106 Version 1.0 Approved contains two stale HJ-005 status assumptions.

### Related Documents

Change:

```text
HJ-005 | Coding Standards | Draft | API boundary, validation, error and HTTP conventions
```

to:

```text
HJ-005 | Coding Standards | Approved | API boundary, validation, error and HTTP conventions
```

### Source-Authority Narrative

Existing text contains:

```text
The approved business artefacts govern business behaviour; draft HJ-005 informs only the proposed technical representation.
```

Replace with:

```text
The approved business artefacts govern business behaviour; HJ-005 informs only the proposed technical representation.
```

Do not change the separate Source Artefact row describing how HJ-005 is used, because that row does not incorrectly state its status.

Do not alter HJ-106 business or HTTP semantics as part of CR-030.

---

## 13.8 ADR-000 – Architectural Decision Register

ADR-000 currently contains:

```text
HJ-008 | AI Roles and Responsibilities | Draft
HJ-009 | AI Operating Guide | Draft
```

Change to:

```text
HJ-008 | AI Roles and Responsibilities | Approved
HJ-009 | AI Operating Guide | Approved
```

No ADR lifecycle state shall change.

ADR-000 remains Accepted.

---

## 13.9 ADR-001

No HJ-005 through HJ-009 status reference requiring correction exists.

**No CR-030 change required.**

---

## 13.10 ADR-002

No HJ-005 through HJ-009 status reference requiring correction exists.

**No CR-030 change required.**

---

## 13.11 ADR-003

No HJ-005 through HJ-009 status reference requiring correction exists.

**No CR-030 change required.**

---

## 13.12 ADR-004

No HJ-005 through HJ-009 status reference requiring correction exists.

**No CR-030 change required.**

---

## 13.13 ADR-005

No HJ-005 through HJ-009 status reference requiring correction exists.

**No CR-030 change required.**

---

## 13.14 ADR-006

No HJ-005 through HJ-009 status reference requiring correction exists.

**No CR-030 change required.**

---

## 13.15 ADR-007

No HJ-005 through HJ-009 status reference requiring correction exists.

**No CR-030 change required.**

---

# 14. Additional Repository-Wide Search

The explicit impact list in Section 13 is derived from the supplied current architectural artefacts.

Codex shall nevertheless perform a final repository-wide search for:

```text
HJ-005
HJ-006
HJ-007
HJ-008
HJ-009
```

combined with status-bearing uses of:

```text
Draft
Approved
```

This search is a verification step, not authority to rewrite every occurrence.

For each result:

- if it represents the **current status** of HJ-005 through HJ-009, update it to Approved;
- if it is an ordinary document reference without status, leave it unchanged;
- if it is a historical revision-history statement, review it for historical intent and normally leave it unchanged;
- if it records that an earlier generated artefact used a Draft source at that historical point, do not rewrite history;
- if it explicitly describes one of these documents as currently Draft, correct it.

Codex shall report any additional current-status references discovered outside the explicit list in Section 13.

---

# 15. Treatment of Downstream Document Versions

CR-030 promotes HJ-005 through HJ-009 themselves.

For other documents changed only because their Related Documents table or prose contains a stale status reference:

- preserve their substantive document version unless the HotJoes repository convention requires a version increment for such metadata reconciliation;
- do not promote or otherwise alter their Status;
- do not rewrite their historical revision entries;
- if a version increment is required by repository convention, use the smallest appropriate revision increment and add a revision-history entry explicitly stating that only related-document/status references were reconciled under CR-030.

No downstream document shall receive a major version increase merely because a referenced document became Approved.

---

# 16. Historical Integrity

CR-030 distinguishes between **current status metadata** and **historical record**.

For example, a past revision-history entry stating that a document was generated from a Draft HJ-005 at that time may remain historically correct.

Therefore Codex shall not globally replace:

```text
Draft
```

with:

```text
Approved
```

The correction applies only where the repository text purports to describe the current status of one of the promoted documents.

---

# 17. Architectural and Governance Constraints

CR-030 shall not:

- change HJ-005 engineering policy;
- change HJ-006 testing policy;
- change HJ-007 enforcement policy;
- change HJ-008 AI roles or authority;
- change HJ-009 AI operating semantics;
- create new AI roles;
- remove existing AI roles;
- alter human accountability;
- allow AI self-approval;
- allow autonomous AI role chaining;
- alter Vendor Domain architecture;
- alter any accepted ADR;
- alter HJ-104 business rules;
- alter HJ-105 behaviour;
- alter HJ-106 service semantics;
- promote any unrelated Draft document;
- change an ADR from Accepted to Approved.

The only authorised content transformation beyond metadata/status reconciliation is the presentation-only HJ-009 Markdown normalisation defined in Section 11.

---

# 18. Codex Execution Procedure

Codex shall perform the work as follows.

## Step 1 – HJ-005

1. Read current HJ-005.
2. Confirm Version 1.0 Draft.
3. Promote to Version 2.0 Approved.
4. Update Last Updated.
5. Add the approval revision-history entry.
6. Preserve substantive content.
7. Continue to HJ-006.

## Step 2 – HJ-006

1. Confirm Version 1.1 Draft.
2. Promote to Version 2.0 Approved.
3. Update Last Updated.
4. Add revision-history entry.
5. Ensure HJ-005 is now treated as Approved.
6. Preserve substantive content.
7. Continue to HJ-007.

## Step 3 – HJ-007

1. Confirm Version 1.0 Draft.
2. Promote to Version 2.0 Approved.
3. Update Last Updated.
4. Add revision-history entry.
5. Treat HJ-005 and HJ-006 as Approved.
6. Preserve substantive content.
7. Continue to HJ-008.

## Step 4 – HJ-008

1. Confirm Version 1.0 Draft.
2. Promote to Version 2.0 Approved.
3. Update Last Updated.
4. Add revision-history entry.
5. Treat HJ-005, HJ-006 and HJ-007 as Approved.
6. Preserve AI role and authority content.
7. Continue to HJ-009.

## Step 5 – HJ-009

1. Confirm Version 0.2 Draft.
2. Promote to Version 1.0 Approved.
3. Update Last Updated.
4. Add revision-history entry.
5. Update HJ-005 through HJ-008 status references to Approved.
6. Apply only the Markdown presentation normalisation specified in Section 11.
7. Verify the substantive content is unchanged.

## Step 6 – Final Cross-Document Reconciliation

Apply the explicit updates specified in Section 13:

- HJ-001;
- HJ-002;
- HJ-005;
- HJ-006;
- HJ-007;
- HJ-008;
- HJ-009;
- HJ-106;
- ADR-000.

Then perform the repository-wide search specified in Section 14.

---

# 19. Final Acceptance Checklist

CR-030 is complete only when all checks below pass.

## HJ-005

- [ ] Version = 2.0.
- [ ] Status = Approved.
- [ ] Last Updated changed to CR-030 application date.
- [ ] Human-review revision-history entry exists.
- [ ] HJ-006 is shown as Approved.
- [ ] HJ-007 is shown as Approved.
- [ ] HJ-008 is shown as Approved.
- [ ] Coding Standards body is substantively unchanged.

## HJ-006

- [ ] Version = 2.0.
- [ ] Status = Approved.
- [ ] Last Updated changed.
- [ ] Human-review revision-history entry exists.
- [ ] HJ-005 is shown as Approved.
- [ ] HJ-007 is shown as Approved.
- [ ] HJ-008 is shown as Approved.
- [ ] Testing Strategy body is substantively unchanged.

## HJ-007

- [ ] Version = 2.0.
- [ ] Status = Approved.
- [ ] Last Updated changed.
- [ ] Human-review revision-history entry exists.
- [ ] HJ-005 is shown as Approved.
- [ ] HJ-006 is shown as Approved.
- [ ] HJ-008 is shown as Approved.
- [ ] HJ-009 is shown as Approved.
- [ ] Enforcement Strategy body is substantively unchanged.

## HJ-008

- [ ] Version = 2.0.
- [ ] Status = Approved.
- [ ] Last Updated changed.
- [ ] Human-review revision-history entry exists.
- [ ] HJ-005 is shown as Approved.
- [ ] HJ-006 is shown as Approved.
- [ ] HJ-007 is shown as Approved.
- [ ] AI roles and authority are substantively unchanged.

## HJ-009

- [ ] Version = 1.0.
- [ ] Status = Approved.
- [ ] Last Updated changed.
- [ ] Human-review revision-history entry exists.
- [ ] HJ-005 through HJ-008 are shown as Approved.
- [ ] Document title uses a proper Markdown heading.
- [ ] Metadata uses a standard Markdown table.
- [ ] Revision History uses a proper Markdown heading and table.
- [ ] Related Documents uses a proper Markdown heading and table.
- [ ] Top-level numbered sections use proper Markdown headings.
- [ ] Structural subsection headings use Markdown heading syntax.
- [ ] Genuine logical lists use normal Markdown list syntax.
- [ ] Human-orchestrated workflow semantics are unchanged.
- [ ] AI role chaining remains prohibited.
- [ ] Human review gates remain unchanged.
- [ ] Tool, escalation, hand-off, self-review and prompt-lifecycle semantics remain unchanged.

## Downstream / Cross-Document

- [ ] HJ-001 shows HJ-005 through HJ-008 as Approved.
- [ ] HJ-002 shows HJ-005 through HJ-008 as Approved.
- [ ] HJ-003 requires no CR-030 change.
- [ ] HJ-004 requires no CR-030 change.
- [ ] HJ-104 requires no status correction.
- [ ] HJ-105 requires no CR-030 change.
- [ ] HJ-106 shows HJ-005 as Approved.
- [ ] HJ-106 no longer describes HJ-005 as `draft HJ-005`.
- [ ] ADR-000 shows HJ-008 and HJ-009 as Approved.
- [ ] ADR-001 through ADR-007 contain no stale CR-030 status references requiring correction.
- [ ] No unrelated artefact has been promoted.
- [ ] No historical record has been rewritten merely because current status changed.

## Repository-Wide

- [ ] No maintained current-status reference identifies HJ-005 as Draft.
- [ ] No maintained current-status reference identifies HJ-006 as Draft.
- [ ] No maintained current-status reference identifies HJ-007 as Draft.
- [ ] No maintained current-status reference identifies HJ-008 as Draft.
- [ ] No maintained current-status reference identifies HJ-009 as Draft.
- [ ] Any additional status-bearing references found by Codex have been reviewed individually.
- [ ] Final diff contains no substantive change outside the explicitly authorised HJ-009 presentation normalisation.

---

# 20. Expected Outcome

After CR-030 is complete:

- HJ-005 Version 2.0 is the Human Reviewed and Approved Coding Standards baseline.
- HJ-006 Version 2.0 is the Human Reviewed and Approved Testing Strategy and Standards baseline.
- HJ-007 Version 2.0 is the Human Reviewed and Approved Enforcement Strategy baseline.
- HJ-008 Version 2.0 is the Human Reviewed and Approved AI Roles and Responsibilities baseline.
- HJ-009 Version 1.0 is the Human Reviewed and Approved AI Operating Guide baseline.
- HJ-009 is visually and structurally consistent with the current HotJoes Markdown documentation convention while preserving its approved meaning.
- The supplied architectural baseline consistently reflects the Approved status of all five documents.
- HJ-106 no longer contains a stale Draft assumption about HJ-005.
- ADR-000 correctly reflects the Approved status of HJ-008 and HJ-009.
- Historical records remain historically accurate.
- Codex can apply the entire change deterministically from HJ-005 downstream without regenerating any architectural artefact.

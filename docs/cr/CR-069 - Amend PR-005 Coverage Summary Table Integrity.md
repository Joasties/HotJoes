# CR-069 — Amend PR-005 Coverage Summary Table Integrity

## 1. Status

Proposed — awaiting human approval.

## 2. Target Artefact

`PR-005 - Generate Architecture and Implementation Test Catalogue.md`

## 3. Purpose

Amend PR-005 so every generated HJ-013 Coverage Summary is structurally valid, complete, contiguous and deterministically ordered.

## 4. Problem

The current HJ-013 Coverage Summary contains blank-line-separated continuation tables and disconnected concern cohorts without repeated headings. Although the concern rows remain present, the rendered Markdown no longer represents one coherent table and makes completeness, ordering and duplicate detection unnecessarily difficult to review.

PR-005 already requires every applicable Approved concern to appear in the Coverage Summary, but it does not explicitly require the summary to be one syntactically valid contiguous table or define deterministic concern ordering. The existing review checklist and completion standard therefore did not reliably detect the structural defect.

## 5. Authorised Change

Amend PR-005 to require:

> The Coverage Summary shall be rendered as one syntactically valid, contiguous Markdown table. Every applicable Approved concern shall appear exactly once and shall be ordered deterministically by numeric CON identifier unless a documented grouping is required. Blank lines, unheaded continuation tables and disconnected concern cohorts are prohibited.

Apply this requirement consistently by:

1. adding it to the Coverage Summary requirements within the Required Output Structure;
2. adding a Review Checklist item confirming that the Coverage Summary is one contiguous table containing every applicable Approved concern exactly once in deterministic numeric CON order, unless an explicit documented grouping applies;
3. strengthening the Completion Standard so completion requires one structurally valid Coverage Summary with no missing, duplicate, disconnected or unheaded concern rows; and
4. requiring Controlled Regeneration to correct inherited Coverage Summary structural defects while preserving concern meaning, architecture authority and stable `AI-*` ownership.

## 6. Explicit Non-Goals

- Do not change any architectural concern, Required Guarantee, Approach, Resolution State, Priority, decision treatment or verification treatment.
- Do not change the authority hierarchy, generation modes or delivery-instantiation boundary of PR-005.
- Do not change HJ-013 ownership of the `AI-*` namespace or HJ-107 ownership of behavioural identifiers.
- Do not require numeric ordering inside sections other than the Coverage Summary.
- Do not prohibit an explicitly documented grouping where it materially improves the Coverage Summary.
- Do not amend PR-008 or any other controlled prompt.
- Do not amend HJ-000 because PR-005 remains indexed as `Current` under the same controlled filename.
- Do not treat correction of an unapproved HJ-013 candidate as a separately versioned architectural change.
- Do not modify the HotJoes repository; human application remains required.

## 7. Verification Criteria

- A generated HJ-013 contains exactly one Coverage Summary table.
- The table has one heading row and one Markdown separator row.
- No blank line occurs between its concern rows.
- Every applicable Approved concern appears exactly once.
- Concern rows are ordered by numeric `CON-*` identifier unless the document explicitly records and explains another grouping.
- No unheaded continuation table or disconnected concern cohort remains.
- Controlled regeneration corrects an inherited structural defect without changing concern meaning or stable `AI-*` ownership.
- Existing PR-005 authority, derivation, classification, reconciliation and human-review rules remain unchanged.

## 8. Completion Standard

The change is complete when the approved PR-005 candidate states the Coverage Summary integrity rule in its output requirements, review checklist and completion standard; Controlled Regeneration explicitly corrects inherited structural defects; and no other PR-005 behaviour changes.

## 9. Controlled Application

This Change Request amends only `PR-005 - Generate Architecture and Implementation Test Catalogue.md`. It does not amend HJ-013, HJ-000, PR-008 or any other controlled artefact.

Approval of CR-069 does not authorise AI modification of the HotJoes repository. The human applies the approved PR-005 candidate.

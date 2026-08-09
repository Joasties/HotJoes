# CR-027 – Promote HJ-002 to Human Reviewed and Approved

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-027 |
| **Title** | Promote HJ-002 to Human Reviewed and Approved |
| **Status** | Proposed |
| **Owner** | Project Architecture |
| **Priority** | Low |
| **Affected Documents** | HJ-002 – Architectural Principles |

---

# 1. Background

HJ-002 – *Architectural Principles* has undergone extensive architectural review throughout the development of the Epic 1 architecture.

The principles defined within HJ-002 have now been exercised and validated through the successful creation and independent review of the downstream architectural artefacts, including:

- HJ-003 – Ubiquitous Language Guide;
- HJ-004 – Vendor Domain Models;
- HJ-104 – Vendor Registration Information Contract;
- HJ-105 – Vendor Registration Sequence Diagram;
- HJ-106 – Vendor Registration Service Contract; and
- HJ-107 – Vendor Registration Test Catalogue.

The document has reached architectural stability and no longer represents a draft proposal.

This Change Request formally promotes HJ-002 to an approved architectural standard without changing the meaning of any existing architectural principle.

During promotion, HJ-002 shall also be aligned with the current HotJoes Markdown document presentation standard adopted by newer architectural documents where appropriate. These changes are editorial only and shall not alter the document's structure, intent or technical content.

---

# 2. Objectives

This Change Request shall:

- promote HJ-002 from **Draft** to **Approved**;
- assign the next approved document version in accordance with the HotJoes versioning convention;
- record formal human architectural approval;
- update document metadata;
- align the Markdown presentation with the current HotJoes document standard where required;
- update repository references that still identify HJ-002 as Draft; and
- preserve the meaning of every existing architectural principle.

---

# 3. Scope

This Change Request applies only to:

- document metadata;
- revision history;
- document presentation;
- repository traceability.

It shall not modify:

- AP-000 through AP-013;
- architectural rationale;
- implications;
- governance rules;
- Architectural Decision Checklist; or
- any architectural intent.

---

# 4. Required Changes

## 4.1 Document Metadata

Update the document metadata as follows:

- Version → next approved version (Project versioning convention).
- Status → **Approved**.
- Last Updated → approval date.
- Retain existing Document ID, Title, Classification and Owner.

---

## 4.2 Revision History

Add a new revision-history entry recording that:

- the document has completed human architectural review;
- no architectural principles were changed;
- the document has been promoted from Draft to Approved.

The revision history shall make it clear that this is an approval milestone rather than a technical revision.

---

## 4.3 Markdown Presentation

Review the document against the current HotJoes Markdown presentation standard used by newer architectural documents.

Where appropriate, update only presentation aspects such as:

- heading consistency;
- table formatting;
- spacing;
- Markdown rendering consistency; and
- other editorial presentation improvements.

These changes shall be formatting only.

They shall not:

- change wording;
- renumber principles;
- alter section numbering;
- modify meaning; or
- introduce additional architectural guidance.

---

## 4.4 Repository References

Update repository references that still identify HJ-002 as **Draft** so they correctly identify the document as **Approved**.

Only status references shall be updated.

No related document shall have its status altered unless it has itself been formally approved.

---

# 5. Architectural Constraints

The promotion shall not:

- alter the meaning of any architectural principle;
- introduce new principles;
- remove existing principles;
- modify architectural rationale;
- modify implications;
- change governance requirements;
- change Architectural Decision Checklist behaviour; or
- introduce any new architectural decisions.

This Change Request is administrative and editorial only.

---

# 6. Expected Outcome

Following application of this Change Request:

- HJ-002 becomes the approved architectural principles baseline for the HotJoes platform.
- All existing principles remain unchanged.
- The document clearly records formal human architectural approval.
- Document metadata reflects its approved status.
- Markdown presentation is consistent with the current HotJoes document standard.
- Repository references correctly identify HJ-002 as an approved document.
- Downstream architectural artefacts continue to derive from the unchanged architectural principles without modification.

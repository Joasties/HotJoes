# HotJoes AI Prompt
## PR-007 - Generate Architecture Propagation Review Batch

### Objective

Act as a **Senior Architecture Configuration Manager and Specification Engineer** with extensive experience in:

- Domain-Driven Design;
- architectural governance;
- controlled document configuration;
- requirements and contract traceability;
- change-impact analysis;
- cross-artefact consistency;
- service-contract and test-catalogue generation; and
- human-gated architectural delivery.

Use the latest HotJoes architectural-concern working register and the controlled HotJoes architecture baseline to generate one complete **Architecture Propagation Review Batch**.

The batch shall contain every artefact candidate required to propagate the selected Approved architectural concerns consistently through the affected artefact set.

For every affected artefact, generate:

1. one Change Request affecting that artefact only;
2. a concise summary of its proposed changes;
3. traceability to the applicable Approved concerns and upstream sources;
4. the complete proposed updated artefact;
5. a controlled comparison with its current version; and
6. artefact-specific verification results.

After generating every affected candidate, produce one cross-artefact consistency and completion report and stop at a single human review gate.

PR-007 does not apply, publish or promote generated candidates. Its normal and default authority is **Proposal Only**.

---

# Governing Mode

Operate under **PR-000 - Architectural Development Mode**.

PR-007 operates in **Execution Mode**. It propagates decisions already explicitly approved by the architectural decision-maker.

Do not reopen, extend or reinterpret an Approved decision during batch generation.

If propagation exposes a genuine contradiction, missing decision or materially ambiguous authority:

- record the issue as a Decision Mode finding;
- do not invent a resolution;
- do not generate an invalid affected candidate merely to complete the batch;
- identify every downstream candidate prevented or rendered provisional by the issue; and
- continue generating unaffected candidates where they remain independently valid.

The final batch report shall make every such limitation visible.

---

# Controlled Architecture Baseline

## HJ-010 Authority

**HJ-010 - Current Application Architectural Concerns** is:

- the complete architectural concern register for its declared active scope;
- authoritative for stable concern identity;
- authoritative for each concern's current Resolution State; and
- the source of current concern, Required Guarantee, Approach, Priority, decision-provenance and verification-treatment data.

HJ-010 retains concerns in all controlled states:

- Exploring;
- Selected;
- Blocked;
- Approved; and
- Challenged.

An Approved concern remains in HJ-010.

## HJ-012 Authority

**HJ-012 - Established Application Architecture Patterns** is:

- authoritative for Approved architectural resolutions;
- populated from the Approved subset of HJ-010; and
- active implementation authority for entries whose Resolution State is Approved.

A previously Approved concern that becomes Challenged remains in HJ-012 for traceability but is not active implementation authority while Challenged.

## Synchronized Concern Data

HJ-010 and HJ-012 use the same concern columns:

```text
ID
Architectural Concern
Required Guarantee
Scope / Source
Approach
Resolution State
Priority
Decision Treatment / Source
Verification Treatment
```

Every Approved concern in HJ-010 shall have exactly one corresponding HJ-012 entry with matching architectural data.

HJ-010 and HJ-012 together form the controlled architecture baseline for propagation to every other artefact.

---

# Working Register Boundary

A supplied architectural-concern spreadsheet is a **non-authoritative working decision input**.

It may:

- identify concern changes explicitly approved by the architectural decision-maker;
- initiate reconciliation with controlled HJ-010/HJ-012;
- supply proposed nine-column concern data; and
- support detection of Approved decisions not yet recorded in the controlled baseline.

It shall not:

- replace HJ-010 or HJ-012;
- independently become architectural authority;
- override controlled content silently;
- be cited by downstream artefacts as normative authority; or
- cause an unapproved spreadsheet edit to become architecture.

Require explicit approval evidence for every concern treated as newly Approved or materially amended. A changed spreadsheet cell alone is not approval evidence.

Spreadsheet operation, iteration history and batch-management detail shall not be recorded as architecture.

---

# Human Review Model

PR-007 uses one human review gate at the end of batch generation.

It does not pause after generating each artefact candidate.

During one execution PR-007 shall:

1. reconcile the working input into provisional HJ-010 and HJ-012 candidates;
2. determine the complete affected artefact set;
3. generate every valid affected candidate in authority order;
4. generate a separate artefact-specific Change Request and summary for each candidate;
5. verify every candidate individually;
6. verify the complete provisional candidate set across artefacts;
7. identify required application order and invalidation dependencies; and
8. present the complete batch for human review.

The architectural decision-maker may then:

- approve the complete batch for controlled application;
- approve a consistent independent subset identified by the batch report;
- request revision of one or more candidates; or
- return one or more findings to Decision Mode.

If an upstream candidate is revised or rejected, every dependent downstream candidate shall be treated as stale and regenerated before application.

---

# Proposal-Only Candidate Chain

Later candidates may depend on earlier candidates generated within the same batch.

For example:

```text
provisional HJ-010/HJ-012
    -> provisional authoritative Domain/workflow artefacts
    -> provisional HJ-106
    -> provisional HJ-107
    -> provisional HJ-013
```

This is permitted only within the batch-generation workspace.

Apply these rules:

1. Every generated candidate remains explicitly **Proposed — not controlled**.
2. A provisional candidate may be used to generate a downstream candidate only when its dependency is recorded.
3. Provisional use does not make the upstream candidate authoritative.
4. Each downstream candidate shall identify the exact provisional upstream versions used.
5. Rejection or material revision of an upstream candidate invalidates every dependent downstream candidate.
6. The final report shall contain a candidate dependency graph and regeneration impact map.
7. No candidate may be used outside the review batch as controlled authority until applied by the architectural decision-maker.

---

# Change Request Granularity

Each generated Change Request shall affect exactly one controlled target artefact.

One concern batch may therefore produce:

```text
one CR for HJ-010
one CR for HJ-012
one CR for ADR-006
one CR for the Address consumed contract
one CR for HJ-003
one CR for HJ-004
one CR for HJ-104
one CR for HJ-105
one CR for HJ-011
one CR for HJ-106
one CR for HJ-107
one CR for HJ-013
```

Do not create an omnibus Change Request spanning several artefacts.

HJ-010 and HJ-012 retain separate artefact-specific Change Requests but form one transactional baseline candidate pair. They shall be applied together or not applied.

If impact assessment concludes that an artefact requires no change, generate a **No-Change Determination** rather than an empty or speculative Change Request.

---

# Execution Context

The execution request shall identify or supply:

```text
Active PR-000 mode:
Controlled HJ-010 baseline:
Controlled HJ-012 baseline:
Latest working concern register:
Selected Approved concern IDs or batch:
Explicit approval evidence:
Active delivery-scope artefact:
Authoritative artefact repository or supplied artefact set:
Applicable target-specific generation prompts:
Previous controlled versions of all possible targets:
Generation mode:
Repository modification authority:
```

Default values are:

```text
Generation mode: Full Propagation Review Batch
Repository modification authority: Proposal Only
```

If a required baseline or target artefact is unavailable:

- identify whether responsible partial generation remains possible;
- record the missing authority;
- do not infer its content or version;
- do not fabricate a complete candidate; and
- identify every downstream candidate blocked by its absence.

---

# Generation Modes

Apply exactly one mode.

## Full Propagation Review Batch

Generate the complete affected candidate set and final review batch.

Use when Approved concern changes are ready for systematic propagation.

## Impact Analysis Only

Generate:

- baseline reconciliation findings;
- affected artefact inventory;
- propagation dependency graph;
- proposed Change Request inventory;
- target-specific generation methods; and
- Decision Mode findings.

Do not generate CRs or updated artefacts.

## Controlled Batch Regeneration

Use after human review comments or an upstream candidate revision.

- preserve unaffected candidates;
- regenerate every materially changed candidate;
- regenerate all transitively dependent candidates;
- reconcile versions and source baselines;
- repeat complete cross-artefact verification; and
- produce a batch reconciliation report.

## Verification Only

Verify an existing review batch without publishing or applying it.

- check every candidate against its declared source chain;
- check every CR against its single target;
- check cross-artefact consistency;
- identify stale candidates; and
- produce a verification report.

---

# Batch Generation Process

## 1. Validate the Approved Concern Batch

For every selected concern:

- confirm stable `CON-*` identity;
- confirm explicit human approval;
- confirm one selected Approach;
- confirm an explicit Required Guarantee;
- confirm Resolution State treatment;
- confirm decision provenance;
- confirm verification treatment;
- identify dependencies on other concerns or missing authorities; and
- reject stale Blocked, Exploring, candidate or unresolved wording where Approval is claimed.

Do not silently repair an unapproved concern decision during propagation.

## 2. Generate the Transactional HJ-010/HJ-012 Pair

### HJ-010 candidate

- compare selected working-register rows to controlled HJ-010 by stable ID;
- reconcile every changed nine-column field;
- retain the complete concern population;
- preserve unaffected concern rows;
- update document control and reconciliation sections;
- generate one HJ-010-only CR; and
- generate the complete proposed HJ-010.

### HJ-012 candidate

- derive from the complete provisional HJ-010 candidate;
- add exactly one entry for each newly Approved concern;
- update materially changed Approved entries;
- retain previously Approved Challenged entries correctly;
- exclude never-Approved Exploring, Selected and Blocked concerns;
- verify matching nine-column data;
- generate one HJ-012-only CR; and
- generate the complete proposed HJ-012.

### Pair verification

Verify that:

- every Approved HJ-010 entry has exactly one matching HJ-012 entry;
- all nine architectural columns match;
- current state and Approved-resolution authority remain distinct;
- versions, revision histories and references describe one synchronized baseline; and
- both candidates can be applied together without an inconsistent intermediate baseline.

## 3. Determine the Complete Affected Artefact Set

Assess every relevant artefact against the provisional HJ-010/HJ-012 pair.

For each artefact record:

- authority role;
- current controlled version;
- applicable concerns;
- direct or derived impact;
- required target-specific prompt;
- upstream candidate dependencies;
- whether change is required;
- reason for inclusion or exclusion; and
- downstream dependants.

Do not assume that every visible or related artefact requires change.

## 4. Generate Authoritative Artefact Candidates

Generate affected authoritative artefacts in authority order.

The order is determined by the concern impact analysis and may include:

```text
ubiquitous language
    -> ADRs and consumed/published contracts
    -> Domain and information models
    -> sequences and workflows
    -> active delivery scope
```

For each affected artefact:

1. identify the highest applicable authority;
2. classify every required change;
3. generate one artefact-specific CR;
4. generate a concise change summary;
5. generate the complete proposed updated artefact;
6. compare it with the controlled baseline;
7. verify it individually; and
8. record provisional dependency and invalidation relationships.

If an apparent target defect originates in an unresolved or contradictory upstream source, do not patch the target. Record a Decision Mode finding.

## 5. Generate Derived Artefact Candidates

After all relevant authoritative candidates are available, generate affected derived artefacts using their governing prompts.

For the current HotJoes chain:

```text
provisional authoritative sources
    -> PR-002 / applicable method -> proposed HJ-106
    -> PR-004 -> proposed HJ-107
    -> PR-005 -> proposed HJ-013
```

For each derived artefact:

- use its previous controlled version as its document/stable-ID baseline;
- use the complete declared provisional upstream candidate set;
- preserve its governing prompt's authority hierarchy;
- reconcile stable IDs only within its owned namespace;
- generate one artefact-specific CR;
- generate the complete proposed updated artefact;
- verify target-specific completeness; and
- record every provisional upstream dependency.

PR-007 orchestrates order and review packaging. It does not replace PR-002, PR-004, PR-005 or another target-specific generation method.

## 6. Perform Cross-Artefact Verification

Verify the complete candidate set for:

- HJ-010/HJ-012 synchronization;
- approved concern coverage;
- authority direction;
- terminology;
- ownership and trust boundaries;
- Domain model consistency;
- information classifications;
- invariants and business rules;
- commands, queries and orchestration;
- diagrams and sequence flows;
- consumed and published contracts;
- outcomes, failures and retry semantics;
- Domain and Integration Event separation;
- delivery-scope consistency;
- test derivation and responsibility boundaries;
- related-document references;
- version and revision-history consistency;
- absence of superseded wording; and
- absence of unintended new architectural semantics.

## 7. Produce the Final Review Batch

Present every generated candidate package and the final batch-level reports together.

Stop once the complete batch is presented.

---

# Finding Classification

Classify every finding as one of:

1. new architectural decision required;
2. conflicting authoritative sources;
3. ambiguous authoritative wording permitting incorrect derivation;
4. propagation inconsistency;
5. target artefact defect;
6. derived artefact defect;
7. superseded wording;
8. document-control update;
9. no change required;
10. missing controlled baseline;
11. stale provisional candidate; or
12. review suggestion outside the Approved architecture or scope.

Trace every defect to the highest authoritative source requiring correction.

Do not patch a downstream symptom whose cause is upstream.

---

# Impact Analysis Requirements

For each selected concern and possible target, assess:

- normative prose;
- ubiquitous language and terminology;
- ownership and authority boundaries;
- Domain models, entities and Value Objects;
- properties and information classifications;
- invariants and business rules;
- commands, queries and application orchestration;
- client interaction and progression rules;
- diagrams, sequences and flows;
- consumed and published contracts;
- outcomes, failures and retry semantics;
- Domain and Integration Events;
- persistence and transaction implications;
- security, configuration and observability implications;
- test implications;
- implementation-scope statements;
- related-document references;
- version, status and revision history; and
- superseded wording.

Not every category affects every artefact. Record material no-change conclusions where they help demonstrate impact completeness.

---

# Authority and Derivation Rules

1. HJ-010/HJ-012 govern implementation architecture; they do not automatically create new business behaviour.
2. Business and Domain behaviour must be expressed in the appropriate authoritative business, Domain, information or workflow artefact before service/test derivation.
3. An Approved Approach may constrain implementation without changing externally observable service behaviour.
4. A target-specific prompt retains authority over its target's derivation, stable IDs and required structure.
5. PR-007 shall not create, modify, retire or reconcile stable IDs outside the owning target's method.
6. A downstream review finding shall be traced to its architectural root before change is proposed.
7. Do not make a spreadsheet, candidate, review report or unapplied CR normative.
8. Do not infer architectural approval from implementation or passing tests.
9. Do not create an ADR merely to complete a table; assess architectural significance using the established governance rules.
10. Do not introduce architecture absent from the Approved concern baseline.

---

# Artefact-Specific Change Request Requirements

Every generated CR shall include:

- CR ID placeholder or supplied ID;
- title naming the single target artefact;
- status;
- target artefact and controlled baseline version;
- applicable Approved concern IDs;
- purpose;
- approved decision baseline;
- propagation problem;
- exact authorised target changes;
- explicit non-goals;
- source authority;
- target impact;
- document-control treatment;
- verification criteria;
- completion standard; and
- next controlled action.

Every CR shall state:

> This Change Request amends only `<target artefact>`. It does not amend any other controlled artefact.

Do not include another artefact's changes for convenience.

---

# Artefact Candidate Package Format

For each changed artefact produce:

## A. Artefact Summary

Include:

- target artefact and current version;
- proposed version and status;
- applicable concern IDs;
- direct and provisional upstream sources;
- downstream dependants; and
- candidate status: Proposed — not controlled.

## B. Change Summary

Summarize:

- sections changed;
- semantics added, amended or removed;
- terminology changes;
- diagrams and tables affected;
- document-control changes; and
- explicit non-changes.

## C. Artefact-Specific Change Request

Provide the complete CR affecting only this target.

## D. Complete Proposed Updated Artefact

Provide the complete updated target under its standard filename.

Do not provide fragments where full-candidate review is required.

## E. Controlled Comparison

| Target Section | Previous Treatment | Proposed Treatment | Reason / Concern Traceability |
|---|---|---|---|

Include stable-ID reconciliation where required by the target's governing prompt.

## F. Artefact Verification

Report:

- baseline fidelity;
- upstream-source fidelity;
- internal consistency;
- terminology and ownership checks;
- superseded-wording scan;
- document-control check;
- target-specific prompt compliance;
- new-semantics check; and
- unresolved findings.

---

# No-Change Determination Format

For each assessed artefact requiring no update, record:

| Artefact | Current Version | Concerns Assessed | Reason No Change Is Required | Verification Evidence |
|---|---|---|---|---|

Do not generate document churn solely to add references when existing content remains correct and traceable.

---

# Required Final Batch Output

Produce the final batch in this order.

## 1. Batch Summary

Include:

- generation mode;
- working-register baseline;
- controlled HJ-010/HJ-012 baselines;
- selected Approved concern IDs;
- explicit approval evidence;
- active delivery scope;
- changed artefact count;
- no-change artefact count;
- blocked candidate count; and
- repository modification authority.

## 2. Approved Concern Reconciliation

| Concern ID | Previous HJ-010 State | Proposed HJ-010 State | Proposed HJ-012 Treatment | Approval Evidence | Reconciliation Status |
|---|---|---|---|---|---|

## 3. Propagation Impact Matrix

| Artefact | Authority Role | Concern IDs | Impact | Candidate / No Change / Blocked | Generation Method | Dependencies |
|---|---|---|---|---|---|---|

## 4. Candidate Dependency Graph

Show:

- provisional generation order;
- direct candidate dependencies;
- transitive invalidation relationships; and
- required controlled application order.

## 5. HJ-010 Candidate Package

Provide the HJ-010 package.

## 6. HJ-012 Candidate Package

Provide the HJ-012 package.

State that these two candidates form one transactional application unit.

## 7. Authoritative Artefact Candidate Packages

Provide every affected authoritative artefact package in dependency order.

## 8. Derived Artefact Candidate Packages

Provide every affected derived artefact package in generation order.

## 9. No-Change Determinations

Provide the consolidated no-change table.

## 10. Decision Mode and Blocking Findings

Identify:

- missing decisions;
- source conflicts;
- ambiguous authorities;
- candidates not generated;
- provisionally generated candidates unsafe to apply; and
- downstream invalidation impact.

## 11. Cross-Artefact Verification Report

Report each cross-artefact check and its result.

## 12. Application and Regeneration Plan

State:

- which candidates form transactional units;
- required application order;
- which independent subsets may be applied safely;
- which candidates must be regenerated if an upstream candidate changes; and
- final promotion checks.

## 13. Human Review Gate

End with these available treatments:

- **Approve Batch** - approve the complete coherent batch for controlled application in the stated order;
- **Approve Independent Subset** - approve only a subset explicitly identified as independent and internally consistent;
- **Revise Candidates** - identify candidates requiring regeneration; or
- **Decision Mode** - reopen identified architectural questions before application.

Do not apply any candidate automatically.

---

# Document-Control Rules

For every candidate:

- preserve stable Document ID and standard filename;
- use the current controlled target as the document/stable-ID baseline;
- increment version according to the proposed change;
- retain applicable revision history;
- add one revision-history entry identifying the artefact-specific CR;
- set Status according to the proposed publication treatment, not automatically;
- update Last Updated consistently;
- reconcile Related Documents where materially relevant;
- identify provisional source versions used; and
- do not append version/status text to a standard filename unless controlled convention requires it.

Candidate generation does not make a candidate Approved.

---

# Prohibited Actions

Do not:

- create one CR for multiple target artefacts;
- give HJ-010 and HJ-012 one shared CR;
- treat HJ-010 or HJ-012 as independently applicable within the baseline pair;
- apply, publish or promote generated candidates;
- pause routine generation after each valid artefact candidate;
- use provisional candidates outside the review batch as controlled authority;
- hide provisional dependencies;
- preserve downstream candidates after a material upstream revision without regeneration assessment;
- propagate directly from the spreadsheet while bypassing HJ-010/HJ-012 reconciliation;
- treat the spreadsheet as normative architecture;
- introduce new architecture during Execution Mode;
- select an Approach for an unresolved concern;
- resolve Blocked or Challenged concerns silently;
- amend stable IDs outside their owning method;
- patch downstream symptoms whose causes are upstream;
- omit complete updated artefacts from the review batch;
- generate implementation code; or
- claim that batch generation constitutes architectural application.

---

# Review Checklist

Confirm that the generated batch:

- uses controlled HJ-010/HJ-012 as the starting architecture baseline;
- treats the working register as non-authoritative input;
- reconciles the complete concern batch into provisional HJ-010/HJ-012 first;
- includes separate HJ-010-only and HJ-012-only CRs;
- treats HJ-010/HJ-012 as one transactional application unit;
- includes every affected artefact and explains every exclusion;
- creates one CR per changed artefact;
- provides one summary and complete candidate per changed artefact;
- uses applicable target-specific generation prompts;
- records every provisional dependency;
- identifies downstream invalidation from upstream revision;
- preserves stable IDs and target authority boundaries;
- contains no silent architectural decisions;
- verifies terminology, ownership, invariants, contracts, sequences and scope;
- reconciles document control and related-document references;
- contains no superseded wording left unintentionally;
- records no-change determinations without document churn;
- records Decision Mode blockers explicitly;
- performs no repository modification; and
- stops once at the final human review gate.

---

# Completion Standard

The Architecture Propagation Review Batch is complete when:

1. every selected concern has been reconciled against explicit approval evidence;
2. the provisional HJ-010/HJ-012 pair is synchronized;
3. the complete affected artefact set has been determined;
4. every valid affected artefact has one CR, summary, complete candidate, comparison and verification report;
5. every no-change artefact has a documented determination;
6. every blocked candidate and Decision Mode issue is explicit;
7. every provisional dependency and invalidation relationship is recorded;
8. derived artefacts use their governing prompts and correct stable baselines;
9. cross-artefact verification is complete;
10. required application order is explicit;
11. no candidate has been treated as applied or authoritative; and
12. the output ends at one final human review gate.

If complete valid generation is impossible, produce the maximum internally valid batch and a precise account of blocked or unsafe candidates. Do not fill gaps by inventing architecture.

---

# Shared Baseline, Output and Verification Control

This prompt is governed by **PR-008 - Global Output and Verification Rules** for baseline validation, filenames, output packaging, source-write boundaries, document control, common preflight verification, direct links and the human-review handoff.

Use **HJ-000 - Current Approved Baseline Manifest** as the compact index of the controlled starting point when available. Validate every required target against its actual controlled metadata; HJ-000 never replaces HJ-010/HJ-012 authority, the working-register boundary or explicit approval evidence.

For an Architecture Propagation Review Batch:

- provide every complete changed controlled artefact under its standard filename;
- provide a direct link to each candidate artefact in the final response, not only to a preview index;
- never append version, status or candidate text to a controlled filename;
- do not generate redundant copies of unaffected approved artefacts;
- include an updated `HJ-000 - Current Approved Baseline Manifest.md` candidate when the batch changes an indexed artefact;
- treat HJ-000 as a non-authoritative index outside the transactional HJ-010/HJ-012 architecture pair; and
- do not recreate an approved artefact merely because the human has applied or approved it.

Complete the PR-008 common preflight and cross-artefact verification before presenting the batch. Any conflict between PR-008 and this prompt shall be reported rather than silently resolved.

# Output Format

Produce complete GitHub-flavoured Markdown suitable for controlled architecture review.

Requirements:

- use standard HotJoes document titles and filenames;
- use numbered headings and Markdown tables in controlled artefacts;
- do not use HTML;
- provide complete candidates rather than fragments;
- keep summaries concise while retaining full traceability;
- record missing authority and conflicts rather than inventing content;
- clearly label all candidates Proposed — not controlled;
- do not generate implementation code; and
- end with the single batch-level human action required.

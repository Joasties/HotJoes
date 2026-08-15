# HotJoes AI Prompt
## PR-005 - Generate Architecture and Implementation Test Catalogue

### Objective

Act as a **Senior Test Architect and Application Architect** with extensive experience in:

- Domain-Driven Design;
- application architecture;
- architecture fitness functions;
- test analysis and test design;
- persistence and transaction verification;
- event-driven and distributed systems;
- reliable publication patterns;
- dependency and boundary enforcement; and
- controlled artefact generation and reconciliation.

Use the supplied HotJoes artefacts to generate or regenerate:

> **HJ-013 - Architecture and Implementation Test Catalogue**

HJ-013 catalogues the executable tests and controlled verification obligations required to demonstrate conformance with Approved HotJoes application architecture.

The output is a **Test Catalogue**. Do not generate executable test code, implementation code, database mappings, broker configuration, CI configuration or deployment manifests.

---

# Execution Context

The execution request shall identify or supply:

```text
Approved architecture baseline:
Complete architectural concern register:
Active delivery-scope artefact:
Applicable System Model baseline:
Current behavioural test catalogue:
Previous HJ-013 baseline:
Applicable Domain/service artefacts:
Applicable standards and ADRs:
Generation mode:
```

The supplied values define the generation boundary. Do not embed the name or contents of a particular Epic permanently into the generation method.

If an execution-context value is unavailable:

- determine whether it is required for the requested generation mode;
- record the missing authority when useful work can still proceed safely; and
- stop rather than infer scope, architecture, stable IDs or normative behaviour where the missing value prevents responsible generation.

---

# Generation Modes

Apply exactly one mode.

## Initial Generation

Use when no previous HJ-013 exists.

- create the standard HJ-013 document and filename;
- allocate stable `AI-*` IDs;
- derive obligations only from the supplied Approved architecture and delivery boundary;
- assign Draft status unless a controlled approval decision states otherwise; and
- record all source baselines.

## Controlled Regeneration

Use when a previous HJ-013 exists and a controlled change or new approved architecture batch requires publication.

- use the previous HJ-013 as the document and stable-ID baseline;
- preserve its Document ID, title and standard filename;
- increment the internal version according to the controlled change;
- set Status according to the applicable review decision rather than resetting it automatically;
- update Last Updated and Revision History;
- retain unaffected obligations;
- reconcile every `AI-*` change; and
- record every source baseline used.

## Verification Only

Use to test this prompt or assess a possible regeneration without publishing HJ-013.

- generate a temporary candidate outside the authoritative repository location;
- label it **Verification candidate — not for publication**;
- compare it with the controlled HJ-013 baseline;
- produce a verification report;
- do not change the controlled HJ-013 version, status, Revision History or file; and
- classify any discovered difference before recommending controlled publication.

Execution of PR-005 does not by itself authorise publication or a version change.

---

# Input Artefacts

Inputs may include:

- HJ-SM-001 - System Model;
- the active delivery-scope artefact;
- HJ-002 - Architectural Principles;
- applicable Domain Model artefacts, including HJ-004 where relevant;
- HJ-005 - Coding Standards;
- HJ-006 - Testing Strategy and Standards;
- HJ-007 - Enforcement Strategy;
- HJ-010 - Current Application Architectural Concerns;
- HJ-012 - Established Application Architecture Patterns;
- applicable approved service contracts, including HJ-106 where relevant;
- the current applicable behavioural test catalogue, including HJ-107 where relevant;
- the previous HJ-013 when regenerating or verifying it; and
- applicable accepted ADRs and approved standards.

Use only artefacts applicable to the supplied architecture and delivery boundary. Do not turn the mere presence or visibility of an artefact into generation scope.

Change Requests explain authorised changes but are not normative architecture unless explicitly identified as an approved source by the resulting controlled artefact.

Where authoritative artefacts disagree, record the conflict and do not select a resolution.

---

# Authority Hierarchy

Use each artefact only within its authority.

| Artefact role | Authority in PR-005 |
|---|---|
| **HJ-012 Approved architecture baseline** | Primary source of active Approved application architecture, Required Guarantees, Approaches, decision authority and verification treatment. |
| **HJ-010 complete concern register** | Confirms all concern states and identifies unresolved dependencies. Candidate Approaches for unresolved concerns are not selectable by generation. |
| **Active delivery-scope artefact** | Defines which components, operations, collaborations and outcomes are included in the current derivation boundary. |
| **Applicable HJ-SM-001 baseline** | Confirms visible system context and component relationships. Visibility alone does not create delivery scope. |
| **Applicable Domain and service artefacts** | Define the business and Domain boundaries that Approved architecture must support; they do not independently approve an implementation Approach. |
| **Current behavioural test catalogue** | Owns behavioural test obligations and supplies stable behavioural coverage references such as `VR-*`. It is not a source of additional architecture. |
| **HJ-005** | Defines approved implementation and coding standards relevant to conformance evidence. |
| **HJ-006** | Defines authoritative Test Classifications, Test Levels and test-design standards. |
| **HJ-007** | Defines approved enforcement mechanisms and boundaries without approving an unresolved architecture Approach. |
| **Applicable ADRs and approved standards** | Supply the decision authority and constraints cited by Approved HJ-012 entries. |
| **Previous HJ-013** | Sole stable-ID and reconciliation baseline for the HJ-013-owned `AI-*` namespace. It is not authority for retaining an obligation contradicted by current Approved sources. |

HJ-012 is authoritative only for entries whose Resolution State is **Approved**. A Challenged HJ-012 entry remains visible but ceases to be active implementation authority.

---

# Scope Boundary

Apply two boundaries together.

## Architecture Inclusion Boundary

Derive active HJ-013 obligations only from Approved HJ-012 entries.

For each Approved concern use:

- stable `CON-*` ID;
- Architectural Concern;
- Required Guarantee;
- approved Approach;
- Scope / Source;
- Decision Treatment / Source; and
- Verification Treatment.

HJ-010 concerns in Exploring, Selected or Blocked state may be referenced only as unresolved dependencies. Do not select one of their candidate Approaches.

A Challenged concern may remain in reconciliation and findings, but its HJ-012 entry must not generate active implementation authority while Challenged.

## Delivery Instantiation Boundary

Instantiate Approved architecture only where it is exercised, constrained or made observable by the supplied active delivery scope.

Use the active delivery-scope artefact to determine:

- in-scope components;
- approved operations;
- required collaborations;
- persistence and integration boundaries;
- controlled stubs or substitutes;
- explicit exclusions; and
- the executable completion boundary.

Do not infer active scope from HJ-SM-001 visibility alone, from the full contents of a Domain Model, or from the previous HJ-013.

An enduring architectural guarantee may be broader than the current delivery scope, but its generated obligation must not invent operations, transitions, components or collaborations solely to exercise future applicability.

The previous HJ-013 provides catalogue continuity. A later active delivery scope does not remove earlier obligations that remain supported by Approved architecture and applicable system scope.

Retain an existing obligation unless it is:

- materially amended by current authority;
- affected by a Challenged pattern;
- superseded by another controlled obligation;
- no longer applicable because its architecture or system scope was explicitly removed; or
- retired through a controlled decision recorded in reconciliation.

---

# Catalogue Ownership and Dependency Direction

HJ-013 exclusively owns the `AI-*` Test ID namespace.

| Catalogue | Owned namespace | Stable-ID baseline | Permitted cross-catalogue use |
|---|---|---|---|
| **HJ-107 or applicable behavioural catalogue** | Its own behavioural IDs, such as `VR-*` | Its previous controlled version, governed by its generation prompt | HJ-013 may reference current behavioural IDs when delegating behavioural coverage. |
| **HJ-013** | `AI-*` | Previous HJ-013 | May identify the current behavioural catalogue and stable behavioural coverage IDs. |

Apply these rules:

1. PR-005 shall create, preserve, amend, retire and reconcile only `AI-*` IDs.
2. The previous HJ-013 is the only stable-ID baseline for `AI-*` identifiers.
3. PR-005 shall not create, modify, retire, renumber or reconcile `VR-*` or other behavioural-catalogue IDs.
4. HJ-013 may reference current behavioural IDs only when that catalogue already owns the behavioural obligation.
5. HJ-013 shall not require the behavioural catalogue to know the HJ-013 version or any `AI-*` ID.
6. An HJ-013-only change does not trigger behavioural-catalogue regeneration.
7. If generation identifies a possible behavioural omission, record a finding; do not amend the behavioural catalogue.

The controlled dependency direction is:

```text
current behavioural catalogue
    -> PR-005 generation input
    -> HJ-013 behavioural-coverage references
```

There is no reverse stable-ID dependency.

---

# Behavioural and Architecture Verification Boundary

The behavioural catalogue owns what the approved service or Domain behaviour must do under defined stimuli.

HJ-013 owns complementary evidence that the implementation conforms to Approved architecture.

## Behavioural Catalogue Responsibilities

These normally include:

- business requests, inputs and outcomes;
- business validation and invariants;
- lifecycle behaviour;
- event occurrence and non-occurrence;
- idempotent replay and conflict outcomes;
- caller-observable persistence and retrieval outcomes;
- externally observable publication guarantees;
- controlled business failures; and
- prohibited business side effects.

## HJ-013 Responsibilities

These may include, where approved and in scope:

- dependency direction and architectural boundaries;
- Aggregate Root mutation control and encapsulation;
- Value Object implementation semantics not already owned behaviourally;
- Entity encapsulation and concrete rehydration identity;
- Domain and infrastructure isolation;
- Domain Event and Integration Event representation separation;
- repository contract shape and concrete adapter integration;
- concrete persistence conversion, mapping and constraint evidence;
- transaction-mechanism atomicity and rollback failure injection;
- Transactional Outbox implementation evidence;
- relay restart and recovery mechanics;
- broker delivery-mechanism evidence;
- handler, repository and mapper composition;
- static analysis, dependency enforcement and architecture fitness checks; and
- other implementation/runtime guarantees explicitly derived from Approved architecture.

Where the behavioural catalogue already owns an obligation, create an **HJ-107 Coverage** record or equivalent behavioural-coverage reference. Do not create a duplicate HJ-013 executable test merely to show coverage.

The same Required Guarantee may require different evidence at distinct boundaries. Such evidence is complementary only when its stimulus, verification boundary and expected evidence are independently identifiable.

---

# Derivation Rules

Every HJ-013 obligation shall be traceable to:

- an Approved HJ-012 concern;
- its matching HJ-010 concern entry;
- the applicable active delivery scope;
- the relevant decision authority, ADR or approved standard; and
- applicable Domain, service or behavioural-catalogue boundaries.

An obligation shall not:

- select an Approach for an unresolved concern;
- invent a schema, format, route, mapping, contract or technology;
- turn a candidate convention into Approved architecture;
- invent a business operation or lifecycle transition;
- expand scope because a component is visible in the System Model;
- duplicate an identical behavioural obligation;
- combine unrelated evidence merely because it can use one fixture; or
- conceal a missing upstream authority as an implementation-local choice.

## Independently Diagnosable Obligations

Each `AI-*` obligation must have one clear verification responsibility and a failure that can be diagnosed independently.

Separate, where applicable:

- Aggregate consistency-boundary integrity;
- Entity identity;
- individual Value Object conversion;
- repository operation semantics;
- response mapping;
- transaction atomicity;
- outbox recovery; and
- dependency enforcement.

Do not assign the same evidence circularly between obligations. A responsibility delegated by one obligation must be clearly and exclusively owned by the referenced obligation.

## Avoidable Blocking

Do not combine a derivable responsibility with an unresolved responsibility when they can be verified independently.

For example:

- Value Object immutability and absence of mutating public state may be derivable even when exhaustive accepted/rejected validation formats are unresolved; and
- repository contract review may be derivable even when concrete PostgreSQL mappings remain unresolved.

Split or narrow the obligation rather than marking all evidence Dependent unnecessarily. Add a new ID only when the separated responsibility is genuinely new and independently verifiable.

---

# Verification Forms

Use these controlled forms:

| Verification Form | Treatment |
|---|---|
| **Executable Test** | Uses exactly one HJ-006 Test Classification and its mapped Test Level. |
| **Automated Enforcement** | Uses build, compiler, static-analysis, dependency-validation or architecture-fitness evidence governed by applicable standards. It is not assigned an invented HJ-006 Test Classification. |
| **Contract Review** | Uses the approved HJ-006 Contract Review / Non-executable classification. |
| **HJ-107 Coverage** | Identifies behaviour already owned by HJ-107 or the applicable behavioural catalogue. No duplicate executable HJ-013 test is generated. |

Use the approved HJ-006 mapping:

| Test Classification | Test Level |
|---|---|
| **Domain** | Unit |
| **Application** | Unit |
| **Persistence Integration** | Integration |
| **Integration Contract** | Integration |
| **API Contract** | API Integration |
| **Contract Review** | Non-executable review |

Do not invent an Architecture Test classification or any other HJ-006 category.

---

# Derivation Status

Use only these controlled values:

| Status | Meaning |
|---|---|
| **Derivable** | The obligation is sufficiently governed to define and generate at its stated boundary. |
| **Dependent** | The obligation is valid, but its executable specification or implementation depends on one or more identified unresolved concerns or missing authorities. |
| **HJ-107** | The behavioural obligation is already owned by HJ-107 or the applicable behavioural catalogue. |

Dependency is not Priority and is not evidence by itself that Approved architecture is deficient.

Every Dependent obligation shall identify:

- the applicable `CON-*` concern or missing authoritative source;
- what evidence is already derivable;
- what remains blocked;
- the decision or authority required; and
- why generation must not select the missing answer.

---

# Priority

Retain the Priority of the governing concern unless a more specific approved rule requires a different treatment.

Use the project-controlled priority values already present in HJ-010/HJ-012, such as `P0`, `P1` and `P2`. Do not use Dependent, Blocked or HJ-107 as Priority values.

---

# Stable AI Test ID Reconciliation

Use identifiers in the form:

```text
AI-<AREA>-<NNN>
```

Retain established area codes where their meaning remains applicable. Do not encode a test framework, class name, technology or current Epic number into an ID.

When regenerating HJ-013:

1. preserve an existing `AI-*` ID when its essential verification obligation remains semantically unchanged;
2. amend the existing entry under the same ID when wording, traceability, classification, expected evidence, dependency or boundary changes without changing the essential obligation;
3. create a new `AI-*` ID only for a genuinely new and independently verifiable obligation;
4. do not reuse a retired ID;
5. record merged, split, retired and superseded obligations explicitly;
6. do not renumber retained IDs merely to make them contiguous;
7. reconcile changed or unresolved behavioural coverage references without modifying their IDs; and
8. report preserved, materially changed, added, retired, merged, split and superseded totals.

Provide a reconciliation table:

| AI Test ID | Previous Treatment | Regenerated Treatment | Reason |
|---|---|---|---|

Provide a separate behavioural-reference reconciliation table when applicable:

| AI Test ID | Previous Behavioural Reference | Current Behavioural Reference | Treatment |
|---|---|---|---|

Do not include a behavioural ID in the `AI-*` stable-ID totals.

---

# Gap and Challenge Classification

Before recommending that an Approved concern become Challenged, classify the problem as one of:

1. incorrect HJ-013 derivation;
2. duplicate behavioural coverage;
3. implementation defect;
4. unresolved HJ-010 concern dependency;
5. missing upstream authority;
6. conflicting authoritative sources; or
7. genuine deficiency in an Approved Required Guarantee or Approach.

Only the final category is direct evidence for a possible architectural challenge. A source conflict may also require architectural reconsideration, but PR-005 shall report it rather than choose the resolution.

PR-005 shall not amend HJ-010 or HJ-012. It may recommend that the stable concern be reviewed under the governed Challenged process and must provide the evidence for that recommendation.

An implementation failing a correctly derived test is normally an implementation defect. A test incorrectly representing the guarantee is an HJ-013 derivation defect. Neither automatically challenges Approved architecture.

---

# Required Output Structure

Produce a complete Markdown document using the standard filename:

```text
HJ-013 - Architecture and Implementation Test Catalogue.md
```

Do not append a version or status suffix to the filename.

Use this structure.

## Document Control

Include:

- Document ID;
- Document Title;
- Version;
- Status;
- Classification;
- Owner; and
- Last Updated.

## Revision History

Record initial generation or controlled regeneration and the source baselines used.

## Related Documents

Identify each relevant artefact, version or baseline, status and relationship.

The current behavioural catalogue may be versioned because it is an upstream PR-005 input. HJ-013 shall not cause that catalogue to depend on the HJ-013 version.

## 1. Purpose

State the architecture baseline and delivery boundary used for this catalogue version.

## 2. Source Authority

Identify:

- artefact;
- version or baseline;
- authority classification; and
- use in derivation.

## 3. Scope and Derivation Rules

Record:

- Approved architecture baseline;
- active delivery-scope baseline;
- applicable System Model baseline;
- operation/component boundary;
- visible but excluded architecture;
- Verification Forms;
- HJ-006 mapping; and
- Derivation Status definitions.

## 4. Coverage Summary

Provide a table containing:

- Concern ID;
- Approved Approach;
- Required Guarantee;
- delivery applicability;
- HJ-013 treatment;
- behavioural-catalogue boundary; and
- known dependencies.

Every active Approved HJ-012 concern applicable to the supplied delivery boundary must appear.

## 5. Architecture and Implementation Test Catalogue

Organise obligations by stable Approved concern or another clearly traceable structure.

For each obligation provide:

- Test ID;
- Title;
- Requirement;
- Source;
- Verification Form;
- Classification / Level;
- Priority;
- Expected Evidence;
- Dependency; and
- Status.

## 6. Cross-Catalogue Responsibility

For each potentially overlapping guarantee identify:

- behavioural-catalogue responsibility;
- HJ-013 responsibility; and
- whether coverage is complementary, delegated or requires review.

## 7. Concern-to-Test Traceability

Provide a table containing:

- Approved Concern ID;
- Required Guarantee;
- AI Test IDs;
- behavioural coverage references;
- coverage status; and
- dependencies.

## 8. Derivation Findings and Dependencies

List:

- unresolved Current Concerns;
- missing upstream authority;
- source conflicts;
- behavioural-catalogue findings;
- scope exclusions;
- possible derivation defects; and
- any evidence requiring architectural review.

Do not silently resolve findings.

## 9. Generation Readiness

Identify separately:

- Derivable HJ-013 obligations;
- behavioural obligations delegated to HJ-107 or the applicable catalogue;
- Dependent obligations;
- what test-candidate generation may proceed; and
- what implementation generation must not decide.

Readiness for test-candidate generation does not authorise unrestricted implementation generation.

## 10. Regeneration Reconciliation

Include `AI-*` reconciliation and behavioural-reference reconciliation when in Controlled Regeneration mode.

## 11. Review Checklist

Include the checklist defined below.

## 12. Next Steps

State the controlled review, dependency-resolution, test-generation and implementation-generation actions supported by the result.

---

# Review Checklist

Confirm that the generated or regenerated HJ-013:

- uses the supplied execution context rather than hard-coded Epic scope;
- derives active obligations only from Approved HJ-012 entries;
- instantiates those obligations only within the supplied delivery boundary;
- does not treat System Model visibility as delivery scope;
- preserves applicable earlier HJ-013 obligations across later delivery scopes;
- uses the previous HJ-013 as the sole `AI-*` stable-ID baseline;
- creates, changes and reconciles only `AI-*` IDs;
- does not create, modify, retire or reconcile behavioural IDs;
- references only current behavioural IDs that exist in the supplied behavioural catalogue;
- contains no behavioural obligation duplicated as an HJ-013 executable test;
- assigns one clear, independently diagnosable responsibility to each obligation;
- contains no circular responsibility delegation;
- separates derivable evidence from unresolved evidence where practical;
- uses exactly one approved HJ-006 Classification and Test Level for each executable obligation;
- does not invent an Architecture Test classification;
- uses only Derivable, Dependent and HJ-107 as Derivation Status values;
- identifies the applicable concern or authority for every dependency;
- does not select an Approach for an unresolved concern;
- does not invent operations, schemas, formats, mappings, technologies or lifecycle transitions;
- distinguishes derivation defects, behavioural duplication, implementation defects, unresolved concerns, missing authority and architectural challenges;
- does not amend HJ-010, HJ-012 or the behavioural catalogue;
- reconciles every preserved, materially changed, added, retired, merged, split or superseded `AI-*` ID;
- identifies exactly what is ready for constrained test-candidate generation; and
- contains no executable test or implementation code.

---

# Completion Standard

The output is complete when:

1. every applicable Approved HJ-012 concern has explicit verification coverage;
2. every obligation is traceable to Approved architecture and the supplied delivery boundary;
3. every behavioural delegation resolves to a current behavioural-catalogue ID;
4. every unresolved dependency is explicit and no missing Approach has been selected;
5. no identical obligation is duplicated across catalogues;
6. every persistence, transaction, integration and enforcement responsibility is independently diagnosable;
7. every stable-ID change is reconciled;
8. every source conflict or missing authority is visible;
9. possible architectural challenges are evidence-backed and distinguished from derivation or implementation defects; and
10. generation readiness is sufficiently explicit to constrain subsequent test and implementation generation.

If completion cannot be achieved, produce the valid partial catalogue and a precise findings section unless missing authority makes any responsible output impossible.

---

# Output Format

Produce a complete GitHub-flavoured Markdown document suitable for controlled inclusion in the HotJoes repository.

Requirements:

- use the standard filename;
- use numbered headings and Markdown tables;
- do not use HTML;
- do not omit a required section because information is unavailable;
- record missing authority or conflicts instead of inventing content;
- do not generate executable test code;
- do not generate implementation code; and
- do not modify any supplied source artefact.

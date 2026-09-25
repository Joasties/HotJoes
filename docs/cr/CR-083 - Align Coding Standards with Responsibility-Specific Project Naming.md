# CR-083 - Align Coding Standards with Responsibility-Specific Project Naming

| Property | Value |
|---|---|
| Change Request | CR-083 |
| Status | Approved |
| Date | 25 September 2026 |
| Owner | Project Architecture |
| Primary affected artefact | HJ-005 Coding Standards |
| Referentially affected artefacts | HJ-107 Vendor Registration Test Catalogue; HJ-013 Architecture and Implementation Test Catalogue; HJ-000 Current Approved Baseline Manifest |

## 1. Reason

HJ-005 v2.0 contains illustrative solution, dependency and namespace examples that predate the responsibility-specific project structure now established by the approved architecture and implemented Epic 1 baseline.

The examples still name `HotJoes.Api` and `HotJoes.Infrastructure.Persistence`. The implemented Vendor API is responsibility-specific as `HotJoes.Api.Vendor`, and the completed persistence refactor places Vendor-owned persistence in `HotJoes.Infrastructure.Vendor.Persistence`. The namespace example `HotJoes.Infrastructure.Persistence.Vendor` likewise no longer describes the approved project and namespace structure.

Leaving these examples unchanged would make the Approved Coding Standards disagree with the implemented and structurally enforced ownership model. Merely replacing one persistence name would also leave the section misleading because it describes a partial historical project list as the solution's current exhaustive structure.

This is a controlled documentation-alignment change. It does not introduce a new architectural decision, alter bounded-context ownership, or justify an implementation that conflicts with the approved baseline.

## 2. Decision Requested

Approve HJ-005 v2.1 as an editorial and structural-alignment revision that:

- replaces the obsolete exhaustive-looking `Existing Structure` example with an explicitly representative Vendor slice;
- uses the responsibility-specific project names `HotJoes.Api.Vendor` and `HotJoes.Infrastructure.Vendor.Persistence`;
- uses `HotJoes.Infrastructure.Vendor.Persistence` as the Vendor persistence namespace example;
- preserves the inward dependency rule and all existing coding standards; and
- records that other bounded contexts own separately named Application, Infrastructure and executable projects according to their approved responsibilities.

Approve the consequential version-reference synchronisation required in HJ-107, HJ-013 and HJ-000 so that no maintained controlled artefact identifies HJ-005 v2.0 as the current standard after HJ-005 v2.1 is applied.

## 3. Controlled Amendments to HJ-005

### 3.1 Metadata and revision history

Change the HJ-005 version from `2.0` to `2.1`, retain Status `Approved`, and set Last Updated to `25 September 2026`.

Append this revision-history entry:

```text
2.1 | 25 September 2026 | Applied CR-083. Replaced obsolete illustrative API and persistence project and namespace names with the responsibility-specific Vendor structure, clarified that the displayed solution structure is representative rather than exhaustive, and preserved the existing dependency and ownership standards unchanged.
```

### 3.2 Representative project structure

Rename Section 3.1 from `Existing Structure` to `Representative Structure`.

Replace its introductory sentence and code block with:

```text
HotJoes projects are named for their architectural responsibility and owning bounded context. A representative Vendor slice follows:

HotJoes.sln

src/
  HotJoes.Api.Vendor
  HotJoes.Application.Vendor
  HotJoes.Domain.Vendor
  HotJoes.Infrastructure.Vendor.Persistence

tests/
  HotJoes.Api.Vendor.Tests
  HotJoes.Application.Vendor.Tests
  HotJoes.Domain.Vendor.Tests
  HotJoes.IntegrationTests
```

After the existing statements that production projects belong under `src` and test projects belong under `tests`, add:

```text
Other bounded contexts and runtime responsibilities use separately named Application, Infrastructure, API or Worker projects where required by the approved architecture. This representative slice is not an exhaustive project inventory.
```

### 3.3 Dependency example

In Section 3.2, replace `HotJoes.Api` with `HotJoes.Api.Vendor` in the intended dependency-direction example. Preserve the existing inward dependency rule, Infrastructure implementation rule, Domain prohibitions and circular-reference prohibition unchanged.

### 3.4 Namespace example

In Section 4.3, replace:

```text
HotJoes.Infrastructure.Persistence.Vendor
```

with:

```text
HotJoes.Infrastructure.Vendor.Persistence
```

Preserve every other namespace and file-organisation rule unchanged.

## 4. Referential Synchronisation

After HJ-005 v2.1 is approved and applied:

1. update the HJ-005 controlled-reference entry in HJ-107 from version `2.0` to `2.1` without changing any behavioural test obligation;
2. update the HJ-005 controlled-reference entry in HJ-013 from version `2.0` to `2.1` without changing any architecture or implementation verification obligation; and
3. synchronise HJ-000 to record HJ-005 v2.1 and the consequential HJ-107 and HJ-013 reference-only revisions as the current Approved baseline.

HJ-107 and HJ-013 shall receive normal revision increments and revision-history entries identifying the changes as reference-only synchronisation under CR-083. Existing stable test identifiers, coverage, priorities, dependencies and verification treatments shall remain unchanged.

## 5. Preserved Decisions and Controls

CR-083 does not change:

- bounded-context or data ownership;
- the approved project dependency direction;
- the rule that Domain projects do not depend on Infrastructure or delivery frameworks;
- Community, Compliance, Address or Vendor Application responsibilities;
- API, Worker, relay, consumer or persistence runtime behaviour;
- any service or Integration Event contract;
- any test obligation, stable test identifier or verification treatment;
- repository modification authority; or
- the requirement for human review and application of controlled artefacts.

The project and namespace examples remain illustrations of approved naming and dependency rules. HJ-005 does not become the authoritative exhaustive inventory of solution projects; that structure remains governed by the approved architecture baseline and its automated structural enforcement.

## 6. Explicit Non-decisions

CR-083 does not:

- rename or move any source or test project;
- authorise additional implementation refactoring;
- introduce a generic shared persistence project;
- require every bounded context to use an identical physical project decomposition;
- reopen CON-046, CON-047 or another Approved concern;
- regenerate HJ-106 or alter its contracts;
- alter HJ-107 or HJ-013 beyond the required HJ-005 version-reference synchronisation; or
- authorise direct repository modification by an AI agent.

## 7. Required Follow-up

After CR-083 is approved and applied:

1. produce and approve HJ-005 v2.1 with only the controlled amendments in Section 3;
2. produce and approve the reference-only HJ-107 and HJ-013 revisions described in Section 4;
3. synchronise and approve HJ-000;
4. verify that maintained source and documentation contain no obsolete `HotJoes.Infrastructure.Persistence` or `HotJoes.Infrastructure.Persistence.Vendor` current-state references, excluding genuine historical text where retention is intentional;
5. verify that the obsolete OpenAPI amendment handoff note is absent; and
6. repeat the repository staging audit before the controlled check-in.

## 8. Acceptance

CR-083 is accepted when the human approves it as authority for the bounded HJ-005 v2.1 project and namespace example alignment and the consequential HJ-107, HJ-013 and HJ-000 reference synchronisation, without changing any approved architecture, contract, behaviour or verification obligation.

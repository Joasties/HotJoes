# CR-059 - Clarify Browser-Based Verification and Repeatable Vendor Registration Demonstration

| Field | Value |
|---|---|
| **Change Request ID** | CR-059 |
| **Status** | Proposed for human review and application approval |
| **Target Artefact** | HJ-011 - Epic 1 Vendor Registration Implementation Scope |
| **Controlled Baseline** | HJ-011 v2.3 Approved |
| **Proposed Version / Status** | HJ-011 v2.4 Approved |
| **Date Proposed** | 3 September 2026 |

## 1. Change Summary

Amend **HJ-011 - Epic 1 Vendor Registration Implementation Scope** to clarify that Epic 1 includes:

- browser-based automated verification of the representative Vendor Registration journey; and
- the ability to demonstrate that journey repeatedly from a documented, controlled, non-production starting state.

The Vendor Web client and completion of Vendor Registration through that client are already in scope. This Change Request makes the associated browser-verification and demonstration obligations explicit.

This Change Request does not select a Web UI development platform, browser-automation platform or detailed Web UI or browser-test architecture.

This Change Request amends only **HJ-011 - Epic 1 Vendor Registration Implementation Scope**. It does not amend any other controlled artefact.

## 2. Reason for Change

HJ-011 v2.3 already requires:

- the Vendor Web client needed to execute the Vendor Registration journey;
- Web-client ownership of the transient Registration Session;
- collection and client-side validation of registration information;
- submission of `RegisterVendor`;
- retrieval and display of Registered Vendor Details; and
- completion of registration by a prospective Vendor through the Web client.

HJ-011 does not currently state explicitly that the browser-visible journey must be verified through browser automation or be repeatably demonstrable.

Without this clarification:

- browser automation could be treated as optional or incidental implementation work;
- completion of the Web-client journey might be demonstrated only through an undocumented manual setup;
- intermittent automation failures could be tolerated without a clear expectation of determinism and diagnosis;
- production identities, credentials or data might be assumed to be necessary for testing or demonstration; and
- security treatment of browser state and diagnostic artefacts could remain implicit.

The clarification establishes the delivery outcome while leaving the necessary architectural and technology decisions to the governed concern-resolution process.

## 3. Required Changes to HJ-011

### 3.1 HJ-011 §2.2 - Client / Interaction

Add the following after the existing list of Web-client responsibilities:

> Epic 1 includes browser-based automated verification of the representative Vendor Registration journey and a repeatable non-production demonstration of that journey.
>
> Browser automation shall exercise the rendered browser-visible interface rather than depend unnecessarily on the internals of the selected Web UI development platform. Tests shall interact preferentially through accessible roles, labels, visible text, semantic HTML and explicit test identifiers where necessary, rather than generated styling classes, fragile DOM hierarchies, positional selectors or framework component internals.
>
> Selection of the Web UI development platform and selection of the browser-automation platform are separate architectural decisions. Neither selection is made by this scope document or by this Change Request.

This wording establishes the required outcome and a stable user-facing automation boundary without selecting a tool or implementation architecture.

### 3.2 HJ-011 §2.7 - Security

Add the following browser-testing and demonstration security requirements:

> Browser-based automated verification and demonstration shall use controlled non-production identities, credentials, data and environments. Production credentials and production identities shall not be required.
>
> Authentication state, cookies, tokens, browser storage, configuration, console output, network evidence, screenshots, videos, execution traces and test reports shall be treated according to their potential sensitivity. Secrets and reusable authentication state shall not be committed to source control.
>
> Diagnostic artefacts shall be generated, retained and made accessible only to the extent necessary for failure diagnosis and governed verification.

These requirements do not select an authentication design and do not bring the production Authentication / Identity Service into Epic 1 scope.

### 3.3 HJ-011 §5 - Epic 1 Completion Boundary

Add the following criteria after existing completion criterion 15:

> 16. The representative Vendor Registration journey is verified through the rendered browser interface using an approved browser-automation approach, with controlled starting state and data, deterministic setup and cleanup, and actionable failure diagnostics.
>
> 17. The browser-based Vendor Registration journey can be demonstrated repeatedly from a documented, controlled, non-production starting state without production credentials, production identities or undocumented manual data repair.

Criterion 16 does not require every behavioural test in HJ-107 to be duplicated as a browser test. Browser automation shall focus on representative user journeys and externally observable integration through the rendered Web interface.

Criterion 17 establishes repeatable demonstration readiness. It does not bring presentation material, marketing content or production Web hosting into Epic 1 scope.

### 3.4 Document Control

Publish HJ-011 as **v2.4 Approved** when CR-059 is applied.

Update **Last Updated** to the date of application.

Add the following revision-history entry, using the actual application date:

| Version | Date | Description |
|---|---|---|
| 2.4 | Date CR-059 is applied | Applied CR-059. Clarified browser-based automated verification and repeatable non-production demonstration of the already in-scope Vendor Registration Web journey without selecting a Web UI or browser-automation technology. |

Add CR-059 to HJ-011 Related Documents where required by the established document convention.

No other Related Documents version shall be changed merely in anticipation of a later concern-reconciliation or propagation activity.

## 4. Architectural Concern Follow-Up

After CR-059 has been approved and applied, its HJ-011 scope clarifications provide authoritative input for a separate reconciliation of the architectural-concern working register with HJ-010.

That separate activity is expected to assess the proposed concerns currently identified in the working register as:

```text
CON-041 - Web UI development platform
CON-042 - Web UI application architecture
CON-043 - Browser UI automation platform
CON-044 - Browser UI test architecture and security
```

The identifiers and concern content remain non-authoritative working input until reconciled into HJ-010 through an independently reviewed HJ-010-only change process.

CR-059 does not introduce, approve or amend any HJ-010 concern.

## 5. Downstream Impact Assessment

| Artefact | Impact of CR-059 |
|---|---|
| **HJ-010 - Current Application Architectural Concerns** | No change through this CR. Requires a separate subsequent concern-introduction reconciliation using approved HJ-011 v2.4 as an authoritative source. |
| **HJ-012 - Established Application Architecture Patterns** | No change. No architectural Approach is approved by this CR. |
| **HJ-006 - Testing Strategy and Standards** | No immediate change. Assess during later browser-automation concern exploration whether an enduring browser-testing standard is required. |
| **HJ-007 - Enforcement Strategy** | No immediate change. Future enforcement treatment depends on approved concern resolutions. |
| **HJ-013 - Architecture and Implementation Test Catalogue** | No immediate normative change. Detailed browser-architecture obligations depend on later Approved concern resolutions. |
| **HJ-107 - Vendor Registration Test Catalogue** | No business-behaviour change. Existing behavioural obligations remain authoritative and may later supply the expected outcomes for representative browser journeys. |
| **ADRs** | No ADR is created or accepted. ADR assessment occurs during subsequent concern exploration. |

No downstream artefact is amended by CR-059.

## 6. Explicit Non-Changes

CR-059 does not:

- amend HJ-010, HJ-012, HJ-013, HJ-107 or any ADR;
- introduce or approve CON-041, CON-042, CON-043 or CON-044 in the controlled architecture;
- select Angular, React, Vue or another Web UI development platform;
- select Playwright, Cypress, Selenium WebDriver, WebdriverIO or another browser-automation platform;
- approve a component, routing, state-management or client API-integration architecture;
- approve a browser-test fixture, identity, data-reset, concurrency or artefact-retention architecture;
- change Vendor Registration business behaviour, validation rules, service contracts, HTTP contracts or Integration Event contracts;
- duplicate every HJ-107 behaviour as a browser test;
- require production identities, credentials or data;
- bring the production Authentication / Identity Service into Epic 1 scope;
- introduce production Web hosting, presentation material or marketing collateral into Epic 1 scope;
- change the current Resolution State, Priority or execution order of any architectural concern; or
- authorize implementation work or technology-selection activity.

## 7. Source Authority

The source authority for this CR is:

- HJ-011 v2.3 for the existing Epic 1 implementation and completion boundary;
- HJ-007 for the principle that required engineering controls must be enforceable;
- HJ-107 for existing Vendor Registration behavioural outcomes; and
- the architectural decision-maker's approval of browser-based automated verification and repeatable demonstration as explicit Epic 1 delivery obligations.

The architectural-concern working register and prior discussion material are supporting decision inputs only. They are not normative architecture sources.

## 8. Verification

Verify the proposed HJ-011 candidate by confirming that:

- §2.2 explicitly includes browser-based automated verification and repeatable non-production demonstration;
- §2.2 keeps the Web UI and browser-automation platform decisions separate;
- §2.2 does not name or select a preferred platform;
- §2.7 requires controlled non-production identities, credentials, data and environments;
- §2.7 covers the sensitivity of browser state and diagnostic artefacts;
- §2.7 does not bring the production Authentication / Identity Service into scope;
- §5 contains the two new completion criteria exactly once;
- browser automation is not defined as duplication of the entire HJ-107 catalogue;
- Vendor Registration behaviour and existing responsibility boundaries remain unchanged;
- no HJ-010 concern is represented as controlled or approved through this CR; and
- metadata, revision history and Related Documents treatment are internally consistent.

## 9. Acceptance Criteria

CR-059 is complete when:

- HJ-011 alone is amended;
- the existing Vendor Web-client scope is preserved;
- browser-based automated verification is an explicit Epic 1 obligation;
- repeatable demonstration from a documented controlled non-production state is an explicit Epic 1 obligation;
- production identities and credentials are explicitly unnecessary;
- sensitive browser state and diagnostic artefacts receive explicit protection;
- the Web UI and browser-automation platform selections remain independent;
- no technology or architectural Approach is selected;
- no other controlled artefact is modified;
- HJ-011 is published as v2.4 Approved only after human application approval; and
- the subsequent HJ-010 concern reconciliation remains a separate controlled action.

## 10. Next Controlled Action

After CR-059 is approved and applied:

1. use HJ-011 v2.4 as the authoritative Epic-scope source;
2. determine and approve the method for introducing Exploring and Blocked concerns from the working register into HJ-010;
3. generate one HJ-010-only CR and complete proposed HJ-010 candidate through that method;
4. produce No-Change Determinations for HJ-012 and other assessed artefacts where no controlled change is justified; and
5. do not begin ADR creation until the applicable concerns enter architectural exploration.

## 11. Human Gate

Apply only the complete reviewed HJ-011 candidate.

Human approval confirms that browser-based automated verification and repeatable non-production demonstration are intended Epic 1 completion obligations.

Approval of CR-059 is not approval of any Web UI platform, browser-automation platform, application architecture, browser-test architecture or working-register concern.

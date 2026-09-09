# ADR-012 – Feature-Oriented Angular Architecture for the Vendor Web Client

| **Document ID** | ADR-012 |
|---|---|
| **Document Title** | Feature-Oriented Angular Architecture for the Vendor Web Client |
| **Version** | 1.0 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 7 September 2026 |

---

# Revision History

| Version | Date | Description |
|---|---|---|
| 1.0 | 7 September 2026 | Applied CR-070. Recorded the approved CON-042 feature-oriented Angular Web UI application architecture. |

---

# Related Documents

| Document ID | Title | Status |
|---|---|---|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-010 | Angular for the Vendor Web Client | Accepted |
| HJ-010 | Current Application Architectural Concerns | Approved |
| HJ-011 | Epic 1 Vendor Registration Implementation Scope | Approved |
| HJ-012 | Established Application Architecture Patterns | Approved |
| HJ-104 | Vendor Registration Fields Matrix | Approved |
| HJ-106 | Vendor Registration Service Contract | Approved |
| HJ-107 | Vendor Registration Test Catalogue | Approved |
| CON-031 | Web client Registration Session | Approved |
| CON-041 | Web UI development platform | Approved |
| CON-042 | Web UI application architecture | Approved |
| CON-044 | Browser UI test architecture and security | Blocked |
| CON-045 | Web UI/UX verification and design validation | Approved |

---

# 1. Context

Epic 1 requires an Angular Vendor Web client that coordinates a multi-step editable Registration Session, submits one complete Vendor Registration request, presents authoritative outcomes, retrieves the resulting persisted Vendor and remains separate from optional post-registration interaction.

ADR-010 selected Angular but intentionally did not select component structure, routing, state management, forms or API integration. CON-031 subsequently established the transient client-owned Registration Session and CON-045 established the semantic, accessible and responsive validation boundary. CON-042 now resolves the detailed client application architecture needed to implement those decisions without creating a second source of business authority in the browser.

# 2. Decision

Approved detailed Angular Web UI architecture decision. HotJoes implements the Epic 1 Vendor Web client as a feature-oriented Angular application using standalone components and route configuration. The Vendor Registration journey forms one explicit feature boundary containing its navigation shell, route-level step components, semantic presentation components, strictly typed reactive forms, client interaction validation, Registration Session state and API adaptation.

The Registration Session is plain typed client state owned by one feature-scoped Angular signal store. Writable state remains encapsulated; consumers observe read-only state and request changes through explicit session commands. Angular forms, components, HTTP representations, server Domain types and post-registration interaction state do not form part of the session.

Each registration step uses a strictly typed reactive form initialised from the Registration Session and commits user-authored values through explicit operations. Client validation remains traceable to approved business authority and assists interaction and request construction without becoming authoritative business validation.

Angular routes represent the registration stages beneath one journey shell. Navigation preserves journey consistency and session state across Back, Continue and Review/Edit actions without establishing business validity. An absent or terminated session returns the user to the beginning. Trading Location changes and explicit abandonment use the approved disclosure, confirmation and cancellation behaviour.

Components and session state do not invoke Angular HttpClient directly. Typed Web-client ports and Angular HTTP adapters own request construction, DTO mapping, configuration, transport invocation, response mapping and safe failure-envelope interpretation. The adapter preserves the approved HTTP contract, reproduces no server business behaviour and does not automatically retry POST /vendors.

Submission permits one active request and exposes explicit ready, submitting, correctable-failure, retryable-failure and completed states. Server validation failures are preserved against applicable steps and fields with an accessible journey-level summary. Correctable and retryable failures retain material registration intent and require explicit user-controlled action.

Only a definitive successful Register Vendor API response completes registration and terminates the editable session. Registered-Vendor retrieval using the returned Vendor ID is a separate operation and presentation boundary. Retrieval failure does not recast definitive registration success as failure. Registration confirmation and optional post-registration interaction remain separate from editable Registration Session state and submission.

Semantic HTML and accessible interaction are owned by presentation components and remain independent of Angular internals and CSS structure. Responsive presentation may reorganise components but preserves meaning, focus order, validation, navigation and completion.

This decision does not define detailed visual design, step-level business rules, precise responsive layouts, browser-test architecture, diagnostic retention, API Gateway product, runtime configuration delivery, same-origin arrangement, deployment topology, production identity or durable Registration Sessions. Those remain governed separately.

# 3. Rationale

- Feature-oriented organisation keeps the complete registration capability cohesive while retaining explicit internal boundaries.
- Standalone components and route configuration use Angular-native composition without an unnecessary module hierarchy.
- Strictly typed reactive forms provide explicit, testable step interaction without making form controls the Registration Session.
- An encapsulated feature-scoped signal store is proportionate to one transient Epic 1 workflow and avoids introducing a wider state-management platform without demonstrated need.
- Typed client ports and HTTP adapters prevent transport and Angular infrastructure from leaking into presentation or session state.
- Explicit submission states make concurrency, failure retention and definitive completion independently verifiable.
- Separate retrieval and post-registration boundaries preserve the authoritative registration outcome.

# 4. Alternatives Considered

## 4.1 NgRx Store and Effects

NgRx provides explicit actions, reducers, selectors and effects and remains credible if Web-client state becomes materially broader or cross-feature. It was not selected for Epic 1 because its additional machinery is disproportionate to one transient Registration Session and risks making client workflow state resemble an alternative Domain model.

## 4.2 Mutable Shared Angular Service

A shared injectable service would be initially simple but would distribute mutation authority across route components, weaken transition modelling and make session disposal, failure retention and single-submission guarantees harder to prove. It was rejected.

## 4.3 Form State as the Registration Session

Keeping the complete journey only in Angular FormGroup instances would couple durable navigation intent to presentation controls and make route transitions, review/edit navigation and API request construction less explicit. It was rejected.

# 5. Consequences

- The Vendor Web client gains one explicit Registration feature and feature-scoped state authority.
- Components use read-only session state and explicit commands rather than direct shared mutation.
- Step forms and API DTOs remain replaceable boundary representations rather than the session model.
- Client validators require traceability to approved business authority.
- Server failures require explicit mapping to accessible journey and field presentation.
- Registered-Vendor retrieval and optional post-registration interaction cannot mutate or reinterpret completed registration.
- CON-042 is resolved and supplies the executable Web UI boundary required by CON-044.
- CON-044 remains blocked only by CON-038.
- HJ-013 requires separate PR-005 regeneration before implementation candidates are generated.

# 6. Verification

Conformance requires:

- architecture and dependency tests for feature organisation, route boundaries, encapsulated writable signals, read-only state and explicit commands;
- component and state tests for form initialisation and commit, navigation, invalidation, confirmed reset, cancellation and session disposal;
- submission tests for every explicit state, one active request, no automatic POST retry, failure retention and definitive completion;
- adapter tests for typed mappings, configuration isolation, safe failures and approved HTTP-contract preservation;
- architecture checks prohibiting direct HttpClient use from components and session state and prohibiting server or Domain representations in the client state;
- retrieval and post-registration tests proving their independence from the completed registration outcome; and
- CON-045 semantic, accessibility and responsive evidence through the framework-independent rendered surface.

Detailed rendered-browser execution remains governed by CON-044. Runtime configuration delivery, same-origin and deployment verification remain governed by CON-038.

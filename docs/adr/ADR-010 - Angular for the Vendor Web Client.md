# ADR-010 – Angular for the Vendor Web Client

| **Document ID** | ADR-010 |
|---|---|
| **Document Title** | Angular for the Vendor Web Client |
| **Version** | 1.0 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 4 September 2026 |

---

# Revision History

| Version | Date | Description |
|---|---|---|
| 1.0 | 4 September 2026 | Applied CR-066. Recorded the approved CON-041 selection of Angular as the TypeScript development platform for the Vendor Web client and established its architectural boundary. |

---

# Related Documents

| Document ID | Title | Status |
|---|---|---|
| ADR-000 | Architectural Decision Register | Accepted |
| HJ-002 | Architectural Principles | Approved |
| HJ-005 | Coding Standards | Approved |
| HJ-006 | Testing Strategy and Standards | Approved |
| HJ-007 | Enforcement Strategy | Approved |
| HJ-010 | Current Application Architectural Concerns | Approved |
| HJ-011 | Epic 1 Vendor Registration Implementation Scope | Approved |
| HJ-012 | Established Application Architecture Patterns | Approved |
| CON-041 | Web UI development platform | Approved |

---

# 1. Context

Epic 1 requires a Vendor Web client through which a prospective Vendor can complete the representative Vendor Registration journey and retrieve the resulting registered information. The Web client must support TypeScript development, accessible interaction, automated testing, maintainable upgrades, CI execution and a credible product lifetime.

The platform choice is long-lived and materially influences Web-client development and maintenance. It must not alter HotJoes' server-side Domain-Driven Design and Clean Architecture boundaries or make the browser an alternative authority for Vendor business rules.

Angular, React, Vue and other credible TypeScript Web UI platforms were considered through an independent weighted assessment. Angular was assessed as the strongest overall fit for the HotJoes constraints, including explicit structure, TypeScript integration, accessibility support, testing compatibility, release discipline, maintainability and professional stewardship.

# 2. Decision

HotJoes shall use Angular, on a currently supported major version, as the TypeScript Web UI development platform for the Vendor Web client.

Angular shall remain confined to the Web-client implementation boundary. Angular components, dependency injection, routing, state representations and other framework types shall not define or enter the server-side Domain, Application or approved HTTP contract.

Vendor business rules and authoritative registration validation remain owned by their approved server-side boundaries. Client validation may support user interaction and construction of a valid request, but it shall not become an alternative business authority.

The Web client shall preserve explicit boundaries between presentation, navigation, transient Registration Session state, client validation, API adaptation, configuration and controlled failure handling. Its rendered interface shall use semantic and accessible interaction independent of Angular internals and shall provide a stable surface for browser automation.

Selection of Angular does not select:

- the detailed Web UI application architecture;
- the Registration Session mechanism;
- the browser UI testing and automation platform;
- the browser-test architecture; or
- runtime and deployment composition.

Those matters remain governed by their respective architectural concerns.

# 3. Rationale

- Angular provides a professionally governed TypeScript-first platform suitable for a structured Web client.
- Its integrated application framework supports the explicit presentation, navigation and boundary separation required by HotJoes without changing server-side authority.
- The platform supports semantic, accessible rendered interfaces and framework-independent browser automation.
- Its supported release and upgrade model provides a credible maintenance path for the expected product lifetime.
- The decision provides enough certainty to continue resolving the Web UI architecture without prematurely determining that architecture.

# 4. Alternatives Considered

## 4.1 React

React is a credible and mature alternative with a strong ecosystem and flexible composition model. It was not selected because HotJoes benefits from Angular's more integrated and consistently structured application platform for this client boundary.

## 4.2 Vue

Vue is a credible and approachable TypeScript-capable alternative. It was not selected because Angular provided the stronger overall fit against the weighted architectural, governance, maintainability and tooling criteria.

## 4.3 Selecting No Framework

Using browser primitives without an application framework was rejected because it would require HotJoes to assemble and govern more of the navigation, composition, tooling and maintenance model without a corresponding product benefit.

# 5. Consequences

- The Vendor Web client will be implemented using a supported Angular major version.
- Angular dependencies and framework types remain within the Web-client implementation boundary.
- The server-side Domain, Application and HTTP contract remain Angular-independent.
- Angular release support and upgrades become managed Web-client dependencies.
- The rendered UI must remain semantic and accessible rather than exposing framework implementation details as its interaction contract.
- CON-041 is resolved and no longer blocks CON-042 solely on platform selection.
- CON-042 must still resolve the detailed Web UI application architecture and its remaining dependencies.
- No Angular source structure, component hierarchy, state library or Registration Session design is selected by this ADR.

# 6. Verification

Conformance requires:

- dependency evidence that the Vendor Web client uses a currently supported Angular major version;
- architecture checks proving Angular does not enter server-side Domain, Application or approved HTTP contracts;
- UI evidence for explicit presentation, navigation, transient state, client-validation, API-adaptation, configuration and controlled-failure boundaries once CON-042 defines them;
- accessibility and semantic-markup evidence for a stable framework-independent rendered surface; and
- controlled upgrade and CI evidence appropriate to the selected supported Angular release.

Detailed test design remains governed by CON-042 and CON-044.

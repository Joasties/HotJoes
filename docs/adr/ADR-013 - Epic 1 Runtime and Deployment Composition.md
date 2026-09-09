# ADR-013 – Epic 1 Runtime and Deployment Composition

| Field | Value |
|---|---|
| **Document ID** | ADR-013 |
| **Document Title** | Epic 1 Runtime and Deployment Composition |
| **Version** | 1.0 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 7 September 2026 |

# Revision History

| Version | Date | Description |
|---|---|---|
| 1.0 | 7 September 2026 | Applied CR-TBD. Recorded the approved CON-038 Docker Compose, Azure Container Apps, ASP.NET Core YARP, single-origin edge, migration and health-gated runtime-composition decision. |

# Related Documents

| Document ID | Title | Status |
|---|---|---|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-009 | Azure Reference Cloud, Centralized Configuration and Secret Management | Accepted |
| ADR-010 | Angular for the Vendor Web Client | Accepted |
| ADR-011 | Playwright Test for Browser UI Testing and Automation | Accepted |
| ADR-012 | Feature-Oriented Angular Architecture for the Vendor Web Client | Accepted |
| HJ-010 | Current Application Architectural Concerns | Approved |
| HJ-011 | Epic 1 Vendor Registration Implementation Scope | Approved |
| HJ-012 | Established Application Architecture Patterns | Approved |
| CON-038 | Runtime and deployment composition | Approved |

# 1. Context

Epic 1 requires one reproducible executable boundary spanning the Angular Vendor Web client, API Gateway, Vendor API, database migration, PostgreSQL persistence, outbox relay, RabbitMQ, Compliance consumer and durable receipt storage. The composition must support local development, integration verification, repeatable demonstration and an Azure reference deployment without changing logical ownership or leaking hosting representations into business or service contracts.

# 2. Decision

HotJoes shall define the Epic 1 runtime as one logical executable composition containing the Vendor Web client, thin API Gateway, Vendor API, explicit database-migration operation, Vendor outbox relay, PostgreSQL persistence, RabbitMQ broker, Compliance consumer and durable Compliance receipt storage. Logical ownership and independently executable responsibilities shall remain unchanged by physical co-location or hosting.

Local development, integration verification and repeatable demonstration shall use a declarative Docker Compose profile with real PostgreSQL and RabbitMQ, controlled non-production configuration and secrets, isolated synthetic data and named health checks. Container creation order shall not be treated as readiness.

The Azure reference deployment shall use declarative infrastructure-as-code and Azure Container Apps for the independently executable Web edge, Vendor API, relay and Compliance consumer. Azure resource representations shall remain confined to deployment and composition boundaries.

An ASP.NET Core YARP edge host shall implement the CON-030 gateway and provide the single browser-facing origin for the compiled Angular client and the allowlisted `POST /vendors` and `GET /vendors/{vendorId}` routes. It shall preserve the approved HTTP semantics without transformation, business validation or automatic retry. The Vendor API and downstream services shall have no direct public exposure.

Externally networked controlled verification shall enter only through the trusted HTTPS edge. TLS shall terminate at the trusted ingress, forwarding information shall be trusted only from configured proxies and internal exposure shall be limited to approved component communication.

PostgreSQL migrations shall execute through an explicit idempotent one-shot pre-readiness operation. Ordinary API, relay and consumer startup shall not create or migrate production schemas. Migration failure shall prevent affected workloads becoming ready.

Every deployable shall implement the CON-036 liveness and responsibility-specific readiness boundary. Composition startup, deployment and browser automation shall use bounded health-gated waiting and shall not use fixed sleeps as readiness evidence.

Local configuration and secrets shall use controlled non-production injection. Azure execution shall use App Configuration, Key Vault and managed identity under ADR-009. Missing or invalid mandatory authority shall fail closed.

Disposable verification environments shall use unique identity, isolated synthetic data, deterministic provisioning and idempotent cleanup. Cleanup failure shall be reported without erasing diagnostic evidence or altering the test result.

Runtime logs, health output and deployment evidence shall exclude registration payloads, credentials and secrets. Browser diagnostic evidence shall follow ADR-011.

This decision does not introduce production Vendor identity, public production exposure, user-level authorisation, durable Registration Sessions, alternative business validation, replacement messaging technology, production-scale availability targets or disaster-recovery objectives.

# 3. Rationale

- Docker Compose provides a reviewable local and disposable component graph using the real infrastructure dependencies.
- Azure Container Apps provides the Azure reference compute boundary without introducing Kubernetes administration into Epic 1.
- YARP preserves the thin .NET gateway boundary and gives the browser one origin without changing the Vendor API contract.
- Explicit migration and readiness gates prevent container startup order from masquerading as operational readiness.
- Provider isolation preserves the existing Domain, Application, HTTP and Integration Event boundaries.

# 4. Alternatives Considered

## 4.1 Single Azure virtual machine

Rejected as the reference topology because it weakens independent workload, ingress and readiness boundaries, despite offering simple Docker Compose parity.

## 4.2 Azure Kubernetes Service

Rejected for Epic 1 because its operational and cost burden exceeds the requirements of the controlled reference deployment.

## 4.3 Separate edge products for static hosting and API management

Rejected because they fragment the temporary Epic 1 exposure boundary without improving the approved demonstration or architectural evidence.

# 5. Consequences

- Docker Compose and Azure infrastructure-as-code become controlled implementation artefacts.
- The Web/Gateway edge, API, relay and consumer remain independently executable.
- YARP becomes the concrete API Gateway product for Epic 1.
- The migration operation becomes a required deployment predecessor.
- Runtime implementation and browser execution gain one explicit composition-readiness boundary.
- HJ-013 must be regenerated for the newly derivable composition obligations.

# 6. Verification

Conformance requires clean and repeatable Docker Compose provisioning with real PostgreSQL and RabbitMQ; YARP allowlisting, same-origin, forwarding, HTTPS and non-retry evidence; prohibited downstream public exposure; explicit migration and failure-gated readiness; responsibility-specific health; bounded readiness waiting; Azure infrastructure-as-code validation; App Configuration, Key Vault and managed-identity integration; isolated disposable environments and idempotent cleanup; and diagnostic leakage checks.

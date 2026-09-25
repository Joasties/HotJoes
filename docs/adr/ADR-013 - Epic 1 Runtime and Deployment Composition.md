# ADR-013 – Epic 1 Runtime and Deployment Composition

| Field | Value |
|---|---|
| **Document ID** | ADR-013 |
| **Document Title** | Epic 1 Runtime and Deployment Composition |
| **Version** | 1.1 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 21 September 2026 |

# Revision History

| Version | Date | Description |
|---|---|---|
| 1.0 | 7 September 2026 | Applied CR-TBD. Recorded the approved CON-038 Docker Compose, Azure Container Apps, ASP.NET Core YARP, single-origin edge, migration and health-gated runtime-composition decision. |
| 1.1 | 21 September 2026 | Applied CR-081. Added the Community Application, persistence, publication, consumer, receipt and trusted Edge responsibilities approved through CON-046. |

# Related Documents

| Document ID | Title | Status |
|---|---|---|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted |
| ADR-003 | Event-Driven Collaboration | Accepted |
| ADR-008 | Idempotent Operations and Reliable Event Publication | Accepted |
| ADR-009 | Azure Reference Cloud, Centralized Configuration and Secret Management | Accepted |
| ADR-010 | Angular for the Vendor Web Client | Accepted |
| ADR-011 | Playwright Test for Browser UI Testing and Automation | Accepted |
| ADR-012 | Feature-Oriented Angular Architecture for the Vendor Web Client | Accepted |
| CR-081 | Amend ADR-013 for Community Runtime Composition | Approved |
| HJ-010 | Current Application Architectural Concerns | Approved |
| HJ-011 | Epic 1 Vendor Registration Implementation Scope | Approved |
| HJ-012 | Established Application Architecture Patterns | Approved |
| CON-038 | Runtime and deployment composition | Approved |
| CON-046 | Community participation and contact preference recording | Approved |

# 1. Context

Epic 1 requires one reproducible executable boundary spanning the Angular Vendor Web client, API Gateway, Vendor API, Community Application capability, database migration, PostgreSQL persistence, outbox relay, RabbitMQ, Compliance consumer, Community consumer and durable receipt storage. The composition must support local development, integration verification, repeatable demonstration and an Azure reference deployment without changing logical ownership or leaking hosting representations into business or service contracts.

Community may share physical API, database and relay infrastructure in Epic 1, but it owns its persistence, transaction, event contract and consumer behaviour. The runtime must expose the authoritative post-registration Community operation only through the trusted Edge, preserve independently executable asynchronous processing, and avoid implying public production enrolment or communication delivery.

# 2. Decision

HotJoes shall define the Epic 1 runtime as one logical executable composition containing the Vendor Web client, thin API Gateway, Vendor and Community Application capabilities, explicit database-migration operation, outbox relay responsibilities, PostgreSQL persistence, RabbitMQ broker, Compliance consumer, Community consumer and durable Compliance and Community receipt storage. Logical ownership and independently executable responsibilities shall remain unchanged by physical co-location or hosting.

Local development, integration verification and repeatable demonstration shall use a declarative Docker Compose profile with real PostgreSQL and RabbitMQ, controlled non-production configuration and secrets, isolated synthetic data and named health checks. Container creation order shall not be treated as readiness.

The Azure reference deployment shall use declarative infrastructure-as-code and Azure Container Apps for the independently executable Web edge, API, relay, Compliance consumer and Community consumer responsibilities. Azure resource representations shall remain confined to deployment and composition boundaries.

An ASP.NET Core YARP edge host shall implement the CON-030 gateway and provide the single browser-facing origin for the compiled Angular client and the allowlisted `POST /vendors`, `GET /vendors/{vendorId}` and `POST /community-participations` routes. It shall preserve the approved HTTP semantics without transformation, business validation or automatic retry. APIs and downstream services shall have no direct public exposure.

The Community HTTP route invokes the Community-owned Join Community Application capability. That capability uses a minimal Vendor-owned verification boundary to confirm successful registration. Physical hosting within an existing API process does not transfer logical ownership to Vendor Application or the Vendor Domain.

Community-owned persistence shall use a logically separate schema, repository and transaction boundary within the Epic 1 PostgreSQL runtime. A successful first Join Community operation atomically commits one immutable Community Participation record and one immutable Community-owned outbox item. Community persistence does not modify Vendor persistence.

The established relay pattern publishes the stored Community event through Community-owned RabbitMQ routing. An independently executable Community consumer uses a Community-owned messaging adapter, durable Community receipt storage and deterministic Community stub processor. Its receipt is processing evidence only and performs no communication delivery.

Externally networked controlled verification shall enter only through the trusted HTTPS edge. TLS shall terminate at the trusted ingress, forwarding information shall be trusted only from configured proxies and internal exposure shall be limited to approved component communication.

PostgreSQL migrations shall execute through an explicit idempotent one-shot pre-readiness operation. The migration set shall include Community-owned schema objects, constraints, outbox objects and receipt storage while preserving logical ownership. Ordinary API, relay and consumer startup shall not create or migrate production schemas. Migration failure shall prevent affected workloads becoming ready.

Every deployable and independently executable responsibility shall implement the CON-036 liveness and responsibility-specific readiness boundary. Community API responsibility, relay publication and consumer processing readiness shall verify their own mandatory configuration and dependencies. Composition startup, deployment and browser automation shall use bounded health-gated waiting and shall not use fixed sleeps as readiness evidence.

Local configuration and secrets shall use controlled non-production injection. Azure execution shall use App Configuration, Key Vault and managed identity under ADR-009. Missing or invalid mandatory Community database, messaging, routing or Vendor-verification configuration shall fail closed and prevent the affected responsibility from becoming ready.

Disposable verification environments shall use unique identity, isolated synthetic data, deterministic provisioning and idempotent cleanup. Automated evidence may inspect Community persistence, outbox state and consumer receipts using synthetic information. Cleanup failure shall be reported without erasing diagnostic evidence or altering the test result.

Runtime logs, health output and deployment evidence shall exclude registration payloads, Community request bodies, serialized event payloads, Primary Contact details, credentials, connection strings and secrets. Browser diagnostic evidence shall follow ADR-011.

The Epic 1 Community capability is available only through the existing controlled local or disposable non-production runtime and trusted allowlisted Edge route. This decision does not introduce production Vendor identity, caller authentication, Vendor-level authorisation, public Community enrolment, public production exposure, durable Registration Sessions, Communication Consent, recipient resolution, communication-provider integration, message delivery, production-scale availability targets or disaster-recovery objectives. Public or production Community exposure requires separately approved identity, authorisation, privacy, retention, withdrawal and production-security decisions.

# 3. Rationale

- Docker Compose provides a reviewable local and disposable component graph using the real infrastructure dependencies.
- Azure Container Apps provides the Azure reference compute boundary without introducing Kubernetes administration into Epic 1.
- YARP preserves the thin .NET gateway boundary and gives the browser one origin without changing Vendor or Community API contracts.
- Explicit migration and readiness gates prevent container startup order from masquerading as operational readiness.
- Provider isolation preserves the existing Domain, Application, HTTP and Integration Event boundaries.
- Logical Community ownership is preserved while allowing proportionate Epic 1 physical co-location.
- An independently executable Community consumer demonstrates the approved asynchronous boundary without implementing communication delivery.

# 4. Alternatives Considered

## 4.1 Single Azure virtual machine

Rejected as the reference topology because it weakens independent workload, ingress and readiness boundaries, despite offering simple Docker Compose parity.

## 4.2 Azure Kubernetes Service

Rejected for Epic 1 because its operational and cost burden exceeds the requirements of the controlled reference deployment.

## 4.3 Separate edge products for static hosting and API management

Rejected because they fragment the temporary Epic 1 exposure boundary without improving the approved demonstration or architectural evidence.

## 4.4 Direct public Community API exposure

Rejected because Epic 1 has no approved production identity, Vendor-level authorisation or public enrolment security boundary. The trusted allowlisted Edge remains the only ingress.

## 4.5 Community processing inside the Web client or Vendor Domain

Rejected because hosting or implementation convenience must not transfer Community ownership or make browser state authoritative.

# 5. Consequences

- Docker Compose and Azure infrastructure-as-code remain controlled implementation artefacts.
- The Web/Gateway edge, API, relay and consumers remain independently executable responsibilities.
- YARP remains the concrete API Gateway product for Epic 1 and gains one allowlisted Community route.
- Community owns its schema, repository, transaction, outbox contract, routing, consumer and receipt evidence despite physical infrastructure sharing.
- The migration operation becomes a required deployment predecessor for Community as well as existing persistence responsibilities.
- Runtime implementation and browser execution retain one explicit composition-readiness boundary extended to Community responsibilities.
- Community adds an independently executable consumer and durable receipt store but no communications provider.
- HJ-013 must later be regenerated for the newly derivable Community composition obligations.

# 6. Verification

Conformance requires clean and repeatable Docker Compose provisioning with real PostgreSQL and RabbitMQ; YARP allowlisting, same-origin, forwarding, HTTPS and non-retry evidence for the Community route; prohibited downstream public exposure; authoritative Join Community invocation through the Edge; minimal Vendor verification; atomic Community record and outbox persistence; Community-owned schema and migration evidence; immutable event routing; independently executable Community consumer and durable idempotent receipts; deterministic stub processing with no communication delivery; explicit migration and failure-gated readiness; responsibility-specific health; bounded readiness waiting; Azure infrastructure-as-code validation; App Configuration, Key Vault and managed-identity integration; isolated disposable environments and idempotent cleanup; and diagnostic leakage checks.

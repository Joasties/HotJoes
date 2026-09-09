# HJ-011 - Epic 1 Vendor Registration Implementation Scope

### Document Metadata

| Field | Value |
| --- | --- |
| **Document ID** | HJ-011 |
| **Document Title** | Epic 1 Vendor Registration Implementation Scope |
| **Version** | 2.12 |
| **Status** | Approved |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 8 September 2026 |

## Revision History

| Version | Date | Description |
| --- | --- | --- |
| 1.0 | 12 August 2026 | Initial approved version of the Epic 1 Vendor Registration Implementation Scope. |
| 1.1 | 13 August 2026 | Applied CR-036 to include the Centralized Configuration Service in the Epic 1 scope. |
| 1.2 | 17 August 2026 | Applied CR-052. Defined the Epic 1 Address port, typed stub adapter, contextual permanent reference, failure taxonomy and no-in-process-retry/no-circuit-breaker boundary. |
| 1.3 | 18 August 2026 | Added the approved CON-012 transport-independent RegisterVendorCommand to the Epic 1 Vendor Application scope. |
| 1.4 | 19 August 2026 | Added the approved CON-040 transport-independent `RegisterVendorResult` to the Epic 1 Vendor Application scope. |
| 1.5 | 19 August 2026 | Added the approved CON-013 composite Vendor uniqueness identity and semantic registration-equivalence boundary to the Epic 1 RegisterVendor scope. |
| 1.6 | 21 August 2026 | Added the approved CON-014–CON-016 and CON-028 PostgreSQL-backed concurrency, permanent replay-outcome, atomic transaction and explicit EF Core mapping boundaries to Epic 1. |
| 1.7 | 22 August 2026 | Applied CR-065. Added the approved CON-019 pre-outbox VendorRegistered mapper and CON-020 versioned Integration Event v1 contract to Epic 1 delivery scope. |
| 1.8 | 23 August 2026 | Applied CR-TBD-HJ011. Added the approved concrete VendorRegistered v1 JSON member structure and deterministic wire-format requirements to Epic 1 delivery scope. |
| 1.9 | 25 August 2026 | Added the approved CON-023–CON-026 thin Minimal API boundary, technical HTTP/JSON contract, centralized controlled-failure mapping and authoritative Application validation allocation to Epic 1. |
| 2.0 | 26 August 2026 | Reconciled the approved unified Application validation-failure outcome across the Epic 1 RegisterVendor delivery scope. |
| 2.1 | 28 August 2026 | Added the approved reliable-publication worker, RabbitMQ, Compliance receipt, migration, observability, health, architecture enforcement and CI scope for CON-018, CON-021, CON-022, CON-029, CON-035–CON-037 and CON-039. |
| 2.2 | 31 August 2026 | Applied CR-TBD-HJ011. Propagated approved CON-032/CON-033 Azure App Configuration, Key Vault, managed-identity, snapshot, failover, bootstrap and rotation boundaries into Epic 1 scope. |
| 2.3 | 1 September 2026 | Applied CR-TBD-HJ011. Synchronized HJ-010/HJ-012 baseline references after completed architecture and test-catalogue propagation; changed no delivery scope or completion criterion. |
| 2.4 | 3 September 2026 | Applied CR-059. Clarified browser-based automated verification and repeatable non-production demonstration of the already in-scope Vendor Registration Web journey without selecting a Web UI or browser-automation technology. |
| 2.5 | 3 September 2026 | Propagated approved CON-034 through PR-007 by constraining the Vendor Registration demonstration to controlled non-production execution, making external-network HTTPS conditional and excluding public or production exposure without separately approved security concerns. |
| 2.6 | 4 September 2026 | Propagated approved CON-041 and CON-043 through PR-007 by naming Angular as the Vendor Web client platform and Playwright Test with TypeScript as the browser UI testing and automation platform while retaining CON-042 and CON-044 as the authorities for their detailed architectures. |
| 2.7 | 4 September 2026 | Propagated approved CON-030 through PR-007 by defining the thin pass-through API Gateway route, forwarding, trace, trust, failure and no-retry boundaries while deferring product and topology selection to CON-038. |
| 2.8 | 6 September 2026 | Propagated approved CON-031 through PR-007 by defining the client-owned in-memory Registration Session lifecycle, confirmed destructive reset, submission, failure retention, explicit retry and post-registration separation boundaries. |
| 2.9 | 6 September 2026 | Propagated approved CON-045 through PR-007 by defining the traceable WCAG 2.2 Level AA Web UI acceptance, responsive interaction, authoritative-outcome and layered design-validation boundary. |
| 2.10 | 7 September 2026 | Propagated approved CON-042 through PR-007 by defining the feature-oriented standalone Angular application, typed reactive forms, encapsulated signal store, routed journey, typed client ports, submission states and separate retrieval and post-registration boundaries. |
| 2.11 | 7 September 2026 | Propagated approved CON-038 and CON-044 through PR-007 by defining the Docker Compose and Azure Container Apps runtime, YARP single-origin edge, health-gated lifecycle and focused secure Playwright browser-test architecture. |
| 2.12 | 8 September 2026 | Propagated the approved CON-009 and CON-034 clarification by defining the shared version-controlled synthetic Address bootstrap catalogue, authoritative Vendor API resolution, client selection boundary and fail-closed runtime consistency requirements. |

## Related Documents

| Document ID | Title | Status |
| --- | --- | --- |
| HJ-001 | Project Vision | Approved |
| HJ-002 | Architectural Principles | Approved |
| HJ-003 | Ubiquitous Language Guide | Approved |
| HJ-004 | Vendor Domain Models | Approved |
| HJ-010 | Current Application Architectural Concerns | Approved v2.14 |
| HJ-012 | Established Application Architecture Patterns | Approved v2.11 |
| HJ-104 | Vendor Registration Fields Matrix | Approved |
| HJ-105 | Vendor Registration Sequence Diagram | Approved |
| HJ-106 | Vendor Registration Service Contract | Approved |
| CR-036 | Include Centralized Configuration Service in Epic 1 Scope | Approved |
| CR-059 | Clarify Browser-Based Verification and Repeatable Vendor Registration Demonstration | Approved |
| ADR-010 | Angular for the Vendor Web Client | Accepted |
| ADR-011 | Playwright Test for Browser UI Testing and Automation | Accepted v1.1 |
| ADR-012 | Feature-Oriented Angular Architecture for the Vendor Web Client | Accepted |
| ADR-013 | Epic 1 Runtime and Deployment Composition | Accepted |

## 1. Purpose

Epic 1 delivers the first executable vertical slice of the HotJoes platform: a prospective Vendor can register through the Web client, the Vendor Domain validates and creates the Vendor, the Vendor is persisted, the registered information can subsequently be retrieved, and the resulting `VendorRegistered` integration event is reliably published and received by a stubbed downstream consumer.

This document defines the authoritative **implementation boundary for Epic 1 Vendor Registration**.

Every relevant capability is classified as:

- **In Scope** – implemented and exercised as part of Epic 1.
- **Out of Scope** – deliberately not implemented as part of Epic 1.
- **Stubbed / Simulated** – required to exercise the Epic 1 slice but substituted for the full production capability.

The wider HotJoes System Model describes the target architecture. Presence on the System Model does not imply inclusion in Epic 1.

---

# 2. In Scope

## 2.1 Domain and Application

Epic 1 implements:

- Vendor Registration.
- An immutable, transport-independent `RegisterVendorCommand` owned by the Vendor Application as the complete registration-intent boundary.
- A closed, immutable, transport-independent `RegisterVendorResult` owned by the Vendor Application as the RegisterVendor application-outcome boundary.
- Creation of the Vendor aggregate and its initial lifecycle state.
- Validation of mandatory and conditional Vendor Registration business rules.
- Registered Information captured at registration.
- Vendor Managed Information required during registration.
- Creation of the internal `VendorRegistered` domain event.
- Creation of the external `VendorRegistered` integration event.
- Pre-outbox translation through an explicit Vendor Application-owned mapper.
- Retrieval of Registered Vendor Details.
- Register Vendor idempotency and duplicate-submission handling.

No Vendor behaviour beyond that required to complete and retrieve the registration is included.

The `RegisterVendorCommand` contains all client-authored registration fields, the opaque Address Resolution reference and transient Registration Declarations. It is independent of the HTTP request representation and any client/BFF Registration Session. It does not contain a Vendor Aggregate, authoritative Address-owned values, server-generated Vendor state, persistence or publication representations, or the derived uniqueness identity, semantic fingerprint and remaining idempotency mechanics governed by CON-013–CON-016.

The `RegisterVendorResult` distinguishes committed success from the expected controlled HJ-106 failure outcomes. Committed success carries only the minimum committed Vendor identity and lifecycle state. Expected failures use stable Vendor Application-owned outcome kinds. Every request-field, Registration Declaration, conditional and cross-field validation failure is represented by one immutable `RequestValidationFailure` containing all independently detectable validation errors; `RegistrationDeclarationFailure` and `ConditionalRuleFailure` are not separate outcomes. The result contains no HTTP representation or status mapping, Address-provider representation, persistence or publication representation, Registration Session state, or framework type. Validation detail and HTTP mapping mechanics remain governed by their separate concerns.

The Vendor Application authoritatively validates all HJ-104 field, declaration, conditional and independently detectable cross-field rules before Address resolution, identity or fingerprint determination, Aggregate creation or persistence. It returns all independently detectable validation errors together. Successful validation supplies canonical values to every downstream stage, including canonical Contact Email and Primary Contact Telephone values. The Vendor Domain remains the final defensive owner of Aggregate and Value Object invariants.

After authoritative Address resolution, RegisterVendor establishes the Vendor uniqueness identity from trimmed, case-insensitive Trading Name, trimmed, case-insensitive Legal Operator Name and CanonicalAddressId. A repeated submission with that identity and semantically equivalent materially relevant registration information returns the original committed successful result without repeating any business effect. The same identity with materially different registration information returns `IdempotencyConflict` and does not update the Vendor. Vendor updates require a separate future administration operation outside Epic 1.

PostgreSQL is the concurrency authority for that identity. One explicit PostgreSQL transaction atomically commits the Vendor Aggregate and Registered Information, the permanent persisted original `RegisterVendorResult` and versioned SHA-256 semantic fingerprint, and exactly one durable outbox item. A uniqueness-race loser commits no effects and resolves the committed record to the original result or `IdempotencyConflict`. Address resolution and pre-transaction validation precede the transaction; outbox dispatch follows commit. Epic 1 introduces no process-local or distributed registration lock and no expiry or deletion of the persisted replay outcome.

For first processing, an explicit Vendor Application mapper translates the completed internal `VendorRegistered` fact and registration-time information into the Vendor-owned `VendorRegistered` Integration Event v1 before outbox persistence. Vendor Infrastructure serializes that event once as UTF-8 camel-case JSON and persists it unchanged inside the registration transaction. The Vendor Domain contains no Integration Event, outbox, serialization or broker representation. The relay shall publish the stored event and shall not reconstruct it from current Vendor state.

## 2.2 Client / Interaction

Epic 1 implements:

- an Angular Vendor Web client, using a currently supported major version, required to execute the Vendor Registration journey;
- Web-client-owned Registration Session.
- Collection and client-side validation of registration information.
- Submission of `RegisterVendor`.
- Retrieval and display of Registered Vendor Details.

Epic 1 includes browser-based automated verification of the representative Vendor Registration journey and a repeatable non-production demonstration of that journey.

Browser automation shall use Playwright Test with TypeScript on a currently supported pinned release together with its corresponding browser binaries. It shall exercise the rendered browser-visible interface and shall not depend on Angular components, dependency injection, internal application state or other framework implementation details.

Angular shall remain confined to the Web-client implementation boundary and shall not define or enter the server-side Domain, Application or approved HTTP contract. Vendor business rules and authoritative registration validation remain owned by their approved server-side boundaries. Client validation may support interaction and request construction but shall not become an alternative business authority.

The Vendor Web client is a feature-oriented Angular application using standalone components and route configuration. The Vendor Registration journey forms one feature boundary containing its navigation shell, route-level step components, semantic presentation components, strictly typed reactive forms, client interaction validation, Registration Session state and API adaptation.

The Registration Session is plain typed client state owned by one feature-scoped Angular signal store. Writable state is encapsulated; consumers observe read-only state and request changes through explicit session commands. Angular form controls, component instances, HTTP representations, server Domain types and post-registration interaction state are excluded from the session.

Each registration step uses a strictly typed reactive form initialised from the Registration Session and commits user-authored values through explicit session operations. Routes represent the stages beneath one journey shell, preserve consistency across Back, Continue and Review/Edit navigation and return an absent or terminated session to the beginning without claiming business validity.

Components and session state do not invoke Angular HttpClient directly. Typed Web-client ports and Angular HTTP adapters own request construction, approved DTO mapping, configuration, transport invocation, response mapping and safe failure-envelope interpretation. They preserve the approved HTTP contract, reproduce no server business behaviour and do not automatically retry POST /vendors.

Submission exposes ready, submitting, correctable-failure, retryable-failure and completed states and permits only one active request. Server validation failures are preserved against applicable steps and fields with an accessible journey-level summary. Correctable and retryable failures retain material intent for explicit user action.

Only a definitive successful Register Vendor response completes registration and terminates the editable session. Registered-Vendor retrieval using the returned Vendor ID is a separate client operation and presentation boundary. Retrieval failure does not invalidate registration success. Registration confirmation and optional post-registration interaction remain separate from editable session state and submission.

Semantic HTML and accessible interaction belong to presentation components and remain independent of Angular internals and CSS structure. Responsive reorganisation preserves meaning, focus order, validation, navigation and completion capability.

Browser UI verification uses a focused Playwright Test suite. The mandatory representative journey uses the real rendered Web interface and full Vendor API, PostgreSQL, RabbitMQ and Compliance path. It begins from a controlled clean environment, registers a Vendor using deterministic synthetic data, observes definitive success, captures the returned Vendor ID, retrieves the persisted Vendor through the rendered interface and verifies the displayed registered information.

The execution harness owns disposable environment provisioning, migration, composition readiness and idempotent cleanup. Each test uses a fresh browser context and isolated deterministic data. Semantic locators, observable-state waiting and bounded central timeouts are required; fixed sleeps, Angular-internal selectors and automatic `POST /vendors` retry are prohibited. Controlled interception is confined to separately identified client tests and makes no claim about replaced server behaviour.

Required local execution uses no automatic retry. CI permits at most one explicitly reported diagnostic retry, and retry-only success remains flaky rather than silently satisfying the gate. Parallel execution requires isolated browser and mutable external state; otherwise tests execute in an explicitly serialized group.

Failure diagnostics are purposeful, access-controlled and retained only for failed or explicitly requested diagnostic execution. HAR and unrestricted body capture are disabled by default. Visual baselines require review, and automated evidence does not replace the manual UX and accessibility evidence required by CON-045.

The Registration Session is transient client-owned in-memory state within the Angular Vendor Web client. It survives navigation between registration steps but is not persisted by the Vendor API, API Gateway, Domain, Application layer or another server-side component. Browser refresh, application closure or explicit abandonment terminates it and requires the journey to restart.

The session retains only user-authored information, journey state, the opaque Address Resolution reference and display information needed to complete and review registration. Address-owned values do not become client-authoritative or replace Address Resolution. Changing Trading Location terminates the current session and begins a new registration journey. Other changes that invalidate dependent information remove it from the session or require it to be re-established before progression.

Before an in-application action terminates or resets a populated session, including changing Trading Location or explicit abandonment, the Web client clearly discloses that captured registration information will be lost and requires explicit confirmation. Cancelling preserves the session unchanged. Detailed interaction implementation follows the approved CON-042 architecture and shall conform to CON-045.

Client validation supports interaction and construction of a complete request but does not replace authoritative Vendor Application validation. Server validation and controlled failure responses are represented against applicable registration steps without relocating business rules into the Web client.

Final submission constructs one complete self-contained registration request. Only one submission is active at a time and the client does not automatically retry `POST /vendors`. A definitive successful response terminates and disposes of the editable session. A correctable or retryable failure retains the material registration intent and permits an explicit user-controlled correction or retry; server-side idempotency remains authoritative.

Post-registration confirmation and optional community participation are separate from the Registration Session and Vendor Registration request. Failure, refusal or abandonment of the optional interaction does not alter the completed registration outcome. Epic 1 does not introduce resumable registration, durable draft storage, authentication, user-level authorisation or production account behaviour.

The implemented Vendor Registration journey shall conform to WCAG 2.2 Level AA within its declared supported scope. It shall provide semantic structure, programmatic headings, names and relationships, complete keyboard operation, visible and predictable focus, non-colour communication, accessible error discovery and authoritative status presentation. Essential information and interaction shall not depend solely on imagery, colour, pointer hover, animation or a particular screen arrangement.

Supported viewport, orientation, input and zoom contexts shall be declared and verified. Responsive reflow may change the visual arrangement but shall preserve required information, interaction order and functionality. The Epic 1 Vendor Registration mockups under `docs/images/Epic1` are design and comparison evidence; they do not override HJ-104, HJ-106, HJ-107 or approved architectural authority.

Every implemented registration field, declaration, conditional rule and authoritative outcome shall be traceable to its approved source. Address search shall produce an explicitly selectable authoritative result. Conditional and dependent information shall be cleared or re-established according to approved rules. Registration Declarations shall begin unchecked.

Submission shall expose a clear busy state, permit only one active submission and present success only from the definitive API result. Optional post-registration community participation shall remain visibly and transactionally separate from completed Vendor Registration.

Verification shall combine automated semantic and accessibility checks, keyboard, focus and assistive-technology verification, responsive viewport, orientation and zoom testing, Playwright user journeys, controlled visual-regression evidence and structured manual review. Automated checks or visual comparison alone do not establish conformance. CON-045 does not select the detailed Angular architecture, browser-test architecture, deployment topology, future community capability or new business rules.

## 2.3 API and Service Boundary

Epic 1 implements:

- Vendor Service.
- a thin ASP.NET Core Minimal API `POST /vendors` endpoint invoking `RegisterVendor`;
- a thin ASP.NET Core Minimal API `GET /vendors/{vendorId}` endpoint invoking `RetrieveRegisteredVendor`;
- API Gateway routing required to expose the Epic 1 Vendor endpoints.
- API-owned request and response DTOs using the approved nested contract and lower-camel-case JSON conventions;
- structural API validation for JSON usability, required-member presence, token and type compatibility, enum, UUID and time wire formats, nested-object structure and supported media type;
- mapping between API DTOs and the transport-independent Vendor Application boundaries;
- centralized mapping from typed Application outcomes to the approved HTTP statuses and API-owned client-safe error envelope;
- generated OpenAPI description of the approved contract;
- cancellation-token forwarding and boundary-specific response headers, including `Location` after successful registration; and
- central unexpected-exception handling that exposes no internal diagnostics.

The endpoints contain no Domain rule, Address resolution, persistence query, transaction, event, outbox or broker behaviour. First registration and equivalent replay both return `201 Created`; retrieval success returns `200 OK`. Epic 1 uses no `422` response, caller-supplied idempotency header, custom correlation convention, custom media type or API versioning. Collection, search, filtering, paging and update endpoints are outside Epic 1.

The API Gateway is a thin pass-through boundary with an explicit allowlist containing only the approved Vendor Registration and registered-Vendor retrieval routes. It forwards approved methods, routes, request bodies and relevant headers without redefining routes, introducing API versioning, transforming registration payloads or duplicating API, Application or Domain validation.

The gateway preserves valid W3C trace context and does not accept untrusted proxy-forwarding information as trusted authority. Gateway diagnostics exclude registration payloads and other sensitive information. A failure occurring before an API response exists produces a safe gateway-owned transport failure. After the Vendor API responds, the gateway propagates its status, headers and response body without semantic remapping. The gateway does not retry `POST /vendors`.

The gateway owns no Registration Session state, Vendor identity, authentication, user-level authorisation, business orchestration, persistence, idempotency, caching or event-publication behaviour. This absence does not permit public or production exposure. An ASP.NET Core YARP edge host provides the single browser-facing origin for the compiled Angular client and allowlisted Vendor routes. Only that trusted edge has external ingress; the Vendor API and downstream services are not directly public. Externally networked controlled verification uses HTTPS with TLS terminated at the trusted ingress and forwarding information accepted only from explicitly configured proxies.

## 2.4 Data and Persistence

Epic 1 implements:

- Vendor PostgreSQL persistence.
- Persistence of the Vendor aggregate and Registered Information.
- Persistence required to support Register Vendor idempotency.
- Persistence support for the approved CON-013–CON-016 identity, equivalence, concurrency, permanent replay and atomic-transaction boundary.
- Explicit EF Core fluent mapping of Vendor state, Registered Information, the one-to-one registration outcome and outbox data in Vendor Infrastructure.
- A PostgreSQL unique constraint over persisted normalized Trading Name, normalized Legal Operator Name and CanonicalAddressId, with restrictive deletion behaviour and supporting indexes.
- Persistence required for reliable integration-event publication.
- Retrieval of persisted Registered Vendor Details.
- Database schema creation and migration required by Epic 1.

## 2.5 Messaging and Integration

Messaging is **in scope**.

Epic 1 implements:

- a real Event Bus / Message Broker;
- creation of the `VendorRegistered` integration event;
- the Vendor-owned, transport-independent `VendorRegistered` Integration Event v1 envelope and payload;
- an Integration Event-owned BusinessAddress representation containing CanonicalAddressId and the approved registration-time address fields without exposing the Domain snapshot type;
- explicit-null representation of optional event values and the approved v1 compatibility rules;
- atomic persistence of Vendor state and the obligation to publish the integration event;
- reliable publication using the agreed outbox/reliability mechanism;
- integration-event serialization and metadata;
- publication to the Event Bus / Message Broker; and
- end-to-end verification that the event can be received and deserialized by the Compliance Event Consumer Stub.

The asynchronous Epic 1 path is:

`Vendor Domain fact → Vendor Application mapper → immutable serialized outbox event → Event Bus / Message Broker → Compliance Event Consumer Stub`

The v1 envelope contains EventId, EventType `VendorRegistered`, EventVersion `1`, OccurredAt and the immutable payload. The payload contains VendorId, RegisteredAt, resulting VendorState, TradingPreference, LegalOperatorType, TradingCharacteristics, the independent BusinessAddress representation, FoodRegistrationAuthority and conditional PrimaryTradingAuthority. Registration Declarations and information not required to initiate Pending Activation and Compliance processing are excluded.

Epic 1 implements the exact nested JSON member structure and deterministic wire representations defined by HJ-004 §7.2: lowercase canonical UUID `D` identifiers, UTC invariant round-trip `O` timestamps, invariant `HH:mm:ss` time-only values without offsets, lower-camel-case enum strings, contract-owned nested Trading Characteristics and Opening Hours representations, and explicit `null` for every absent optional member. No published representation exposes or reuses a Vendor Domain Aggregate, Value Object or enum type.

Compatible optional fields may be added within v1; breaking changes require a new version. Publication retry preserves EventId, version and serialized event.

## 2.6 Configuration

Configuration is **in scope**.

Azure is the primary Epic 1 reference cloud. Azure App Configuration provides centrally managed, environment-specific non-secret configuration for applicable Epic 1 deployables and supporting infrastructure. Each deployable owns strongly typed options for only the values it consumes and validates its complete required configuration before readiness.

Production configuration is promoted as validated immutable snapshots. Running services retain their current validated snapshot during temporary provider outage or invalid refresh. Runtime refresh is limited to settings explicitly classified as reload-safe; consistency-sensitive settings use health-gated rolling restart unless atomic reload is implemented and verified.

Production App Configuration uses cross-region replication and provider failover. A new or recovering instance does not report readiness until one complete approved snapshot has been obtained and validated. If no approved replica is available, the instance fails readiness rather than using incomplete, invalid or unmanaged locally cached configuration.

Configuration retrieval applies where relevant to the Vendor Web client, API Gateway, Vendor Service, Address Domain Stub, Compliance Event Consumer Stub, Vendor persistence, Event Bus / Message Broker, publication components and other explicitly in-scope supporting deployables.

Epic 1 configuration includes application settings, environment-specific settings, endpoints, non-secret connection and integration settings, and consistent component-connection information. It shall not contain secret values, business rules, service contracts, Integration Events, mutable request state or shared hidden coupling.

Feature-management behaviour remains out of scope.

## 2.7 Security

Security is **in scope**.

Azure Key Vault is the production secret store. Production workloads use managed identity wherever the target Azure service supports it. Remaining secrets are versioned in Key Vault and have an identified owner, purpose, consumers, lifetime, rotation mechanism and recovery procedure. App Configuration may contain non-secret references but never secret values.

Local execution uses developer identity or controlled local secret injection. Secret values are excluded from source control, normal configuration files, logs, diagnostics, API responses and recovery evidence.

Rotation uses overlap-and-cutover where supported: validate the replacement, make it available to consumers, complete verified refresh or health-gated rolling replacement, then revoke the previous credential. Rotation failure stops before revoking a credential required by healthy instances.

Epic 1 introduces no Vendor identity or user-level authorisation and does not expose Vendor Registration or registered-Vendor retrieval as publicly accessible production capabilities.

The repeatable Vendor Registration demonstration executes in a controlled local or disposable non-production environment using synthetic, deterministic sample data. The representative journey explicitly selects a deterministic synthetic Address result from the approved bootstrap catalogue, registers a Vendor using its opaque Address Resolution reference and bound Trading Location, captures the newly returned Vendor ID, retrieves the persisted Vendor using that ID and verifies the displayed registered information. A separately seeded fixed Vendor ID may be used only where a test specifically begins with retrieval.

Test-owned Vendor IDs and deterministic test data provide repeatability only. They are not authentication or authorisation mechanisms.

If an Epic 1 environment is accessed across an external network boundary for controlled verification, its endpoints use HTTPS and access remains restricted to that controlled environment.

Any future public or production exposure requires separately approved identity, authorisation, data-access and production-security concerns.

Browser-based automated verification and demonstration shall use controlled non-production identities, credentials, data and environments. Production credentials and production identities shall not be required.

Authentication state, cookies, tokens, browser storage, configuration, console output, network evidence, screenshots, videos, execution traces and test reports shall be treated according to their potential sensitivity. Secrets and reusable authentication state shall not be committed to source control.

Diagnostic artefacts shall be generated, retained and made accessible only to the extent necessary for failure diagnosis and governed verification.

## 2.8 Observability

Observability is **in scope**.

Epic 1 implements sufficient observability to diagnose and verify:

- Vendor Registration execution;
- persistence failures;
- idempotency behaviour;
- outbox and reliable-publication behaviour;
- event publication failures;
- successful receipt by the Compliance Event Consumer Stub;
- correlation of a registration request across Epic 1 components; and
- health of deployable Epic 1 services.

---

## 2.9 Reliable Publication and Enforcement

## Epic 1 Reliable Publication Profile

Epic 1 uses a dedicated Vendor relay worker that polls PostgreSQL in bounded batches and claims eligible outbox records using leased `FOR UPDATE SKIP LOCKED` semantics. It publishes the stored immutable event bytes through durable RabbitMQ topology with publisher confirms, then marks the record published. Expired claims recover automatically. Failed attempts use validated bounded exponential backoff; exhausted work becomes durable `Stalled` work requiring explicit administrative requeue. Records are not deleted.

Delivery is at least once. EventId is the stable message identity; exactly-once and ordering guarantees are not claimed. Consumers acknowledge only after durable idempotent receipt, use bounded retry, and route exhausted or non-retryable messages to a durable dead-letter queue without changing EventId, EventVersion or payload.

The Compliance stub durably records EventId and a serialized-byte hash before acknowledgement and performs no Compliance business behaviour. Reviewed EF Core migrations are applied before readiness, not by ordinary service startup. W3C trace context is carried as outbox and RabbitMQ metadata, with structured redacted logs and focused metrics. Liveness has no external dependency; readiness is responsibility-specific: the API requires PostgreSQL but not RabbitMQ, while relay and consumer readiness require their PostgreSQL/RabbitMQ dependencies. A dedicated HotJoes.ArchitectureTests project and mandatory GitHub Actions gates enforce the approved project, type, migration, PostgreSQL, RabbitMQ and API rules.

## 2.10 Runtime and Deployment Composition

Epic 1 is one logical executable composition containing the Vendor Web client, YARP edge, Vendor API, explicit database-migration operation, Vendor outbox relay, PostgreSQL persistence, RabbitMQ, Compliance consumer and durable Compliance receipt storage. Logical ownership and independent execution responsibilities do not change through physical co-location.

Local development, integration verification and repeatable demonstration use declarative Docker Compose with real PostgreSQL and RabbitMQ, controlled non-production configuration and secrets, isolated synthetic data and named health checks. Container creation order is not readiness evidence.

The Azure reference deployment uses declarative infrastructure-as-code and Azure Container Apps for the independently executable Web edge, Vendor API, relay and Compliance consumer. Azure types remain confined to deployment and composition boundaries. Missing configuration, secrets or dependencies fail closed.

Schema migration is an explicit idempotent one-shot pre-readiness operation. Ordinary workload startup does not migrate production schemas. Composition startup and browser automation use bounded health-gated waiting. Disposable verification environments have unique identity, isolated data, deterministic provisioning and idempotent cleanup whose failure is reported without changing the test result.

Runtime logs, health output and deployment evidence exclude registration payloads, credentials and secret values.

---

# 3. Out of Scope

## 3.1 Domain and Application

The following are not implemented:

- Compliance Domain business behaviour.
- Determination of Compliance Requirements.
- Pending Activation processing.
- Vendor activation.
- Vendor suspension and deactivation workflows beyond state required by registration.
- Menu management.
- Ordering.
- Customer Domain.
- Driver Domain.
- Payment Domain.
- Notification Domain.
- Analytics Domain.
- Search capability.
- Vendor operational availability beyond information required during registration.

## 3.2 Client / Interaction

The following are out of scope:

- BFF implementation.
- Customer applications.
- Driver applications.
- Vendor functionality unrelated to registration and retrieval of Registered Vendor Details.
- Native mobile applications.

The wider architecture may permit a client- or BFF-owned Registration Session in future. Epic 1 implements the Web-client-owned option only.

## 3.3 Platform and Architectural Plumbing

The following System Model capabilities are out of scope:

- Authentication / Identity Service.
- Service Registry / Service Discovery.
- Full production observability platform.
- Enterprise dashboards and operational alerting estate.
- Payment Gateway.
- SMS provider.
- Email provider.
- Analytics platform.

## 3.4 Messaging and Integration

The following are out of scope:

- real Compliance processing of `VendorRegistered`;
- creation of Compliance aggregates or entities;
- execution of Compliance workflows;
- Compliance-driven updates to Vendor state;
- Compliance-generated events or commands;
- messaging for domains unrelated to Vendor Registration; and
- production-scale event topology beyond that required to implement and verify the Epic 1 flow.

## 3.5 External Integrations

The following production integrations are out of scope:

- mapping or geocoding providers;
- local-authority systems;
- payment providers;
- email providers;
- SMS providers; and
- third-party identity providers.

---

# 4. Stubbed / Simulated

## 4.1 Address Domain

The Address Domain is required by Vendor Registration but is not implemented as a production capability in Epic 1.

Epic 1 provides a controlled **Address Domain Stub** behind the same architectural boundary expected of the eventual Address capability.

The stub must support:

- resolution or retrieval of an authoritative address from the supplied resolution reference;
- canonical address information required by Vendor Registration;
- the Business Address Snapshot required by the Vendor Domain;
- Food Registration Authority;
- Primary Trading Authority where applicable; and
- success and failure behaviour required by the Vendor Registration contract.

The stub is consumed through the Vendor Application's Address port and a typed adapter. Stub and Address contract types shall not enter the Vendor Domain Model.

For the controlled Epic 1 demonstration, one version-controlled synthetic Address bootstrap catalogue is supplied as a non-secret deployment asset. It contains a finite set of complete deterministic Address results, each with a stable opaque reference bound to one declared Trading Location. The Vendor API composition root initialises the in-process stub from this catalogue and remains authoritative for resolution.

The Vendor Web client uses the same approved catalogue revision only to present explicitly selectable synthetic results and retain the selected opaque reference and permitted display information. It submits only that reference and the declared Trading Location; client-held Address, Canonical Address and regulatory-authority values do not replace server-side resolution.

The runtime supplies one consistent catalogue revision to the Web and Vendor API boundaries. Missing, malformed, incomplete, conflicting or inconsistent mandatory catalogue data fails closed and prevents the affected registration capability from becoming ready. Clean local and disposable environments reproduce the same entries and bindings. References are not reassigned; catalogue evolution is additive or preserves every reference used by a retained demonstration environment. Catalogue content is synthetic and non-personal and contains no credentials or secrets.

The client-side Address journey may receive one result, a reasonably small selection list, or a request to refine the search. The client shall not progress to `RegisterVendor` submission until it has selected a complete valid result and received a permanent opaque Address Resolution reference.

Successful selection binds a `CanonicalAddressId`, the original immutable result and the declared Trading Location. Resolution must supply that same Trading Location. References do not expire, cannot be revoked, and may be resolved repeatedly without consumption.

The Epic 1 stub shall expose deterministic scenarios for:

- valid and invalid Food Registration Authority outcomes;
- valid and invalid Primary Trading Authority outcomes where Trading Location is `Stall`;
- `InvalidReference` for an unknown or fabricated reference;
- `InvalidAddressResult` for a known reference that cannot satisfy the supplied Trading Location context; and
- technical timeout, unavailability and transient failure.

Semantic failures fail fast. Technical failures return a controlled retryable application failure. The caller may retry `RegisterVendor` using the same permanent reference. The Vendor Application performs no in-process automatic retry, and no circuit breaker is included in Epic 1.

The following are not implemented:

- a public Address-search API or general-purpose reference-data service;
- production Address Domain infrastructure;
- production address-search capability;
- external geocoding or mapping-provider integration; and
- production local-authority resolution services.

Vendor-specific code must not depend upon stub-specific behaviour.

## 4.2 Compliance Event Consumer Stub

The Compliance Domain is not implemented in Epic 1.

Epic 1 provides a **Compliance Event Consumer Stub** solely to verify the asynchronous integration boundary.

The stub must:

- subscribe to `VendorRegistered`;
- receive the published event;
- deserialize the agreed integration-event contract;
- validate that the required integration payload is present; and
- record receipt sufficiently for automated integration verification and observability.

The stub must not:

- implement Compliance domain behaviour;
- create Compliance aggregates or entities;
- determine Compliance Requirements;
- execute Pending Activation processing;
- update Vendor state;
- publish Compliance events or commands; or
- introduce any other Compliance business workflow.

---

# 5. Epic 1 Completion Boundary

Epic 1 Vendor Registration is complete when:

1. A prospective Vendor can complete registration through the Angular Web client without Angular entering the server-side Domain, Application or approved HTTP contract.
2. The transient Registration Session is owned in memory by the Angular Vendor Web client, survives step navigation, remains outside all server-side boundaries and is disposed only by definitive success, refresh, closure or confirmed reset or abandonment.
3. The Vendor Service validates the authoritative Vendor Registration rules.
4. Required Address information is obtained authoritatively through the Address Domain Stub initialised from the approved deterministic synthetic Address bootstrap catalogue shared consistently with the Web presentation boundary.
5. A valid Vendor is created exactly once for an idempotent registration submission.
6. The Vendor is persisted successfully.
7. Registered Vendor Details can subsequently be retrieved.
8. The `VendorRegistered` integration event is durably and reliably published through the Event Bus / Message Broker.
9. Failure of event publication cannot result in loss of the publication obligation.
10. The published integration event is successfully received and deserialized by the Compliance Event Consumer Stub.
11. The complete Epic 1 execution can be observed and diagnosed sufficiently to identify registration, persistence and publication failures.
12. Epic 1 components requiring centrally managed configuration can retrieve one complete approved snapshot from Azure App Configuration through regional or cross-region provider failover, validate it before readiness and retain the last validated running configuration during temporary provider outage.
13. Production components retrieve required secrets from Azure Key Vault using managed identity where supported and demonstrate safe versioned rotation without premature credential revocation.
14. `POST /vendors` and `GET /vendors/{vendorId}` expose the approved Application operations through thin endpoint adapters and the allowlisted pass-through API Gateway without route redefinition, payload transformation, validation duplication or semantic response remapping.
15. Direct Vendor Application invocation remains authoritative for registration validation without requiring an HTTP caller, and every pre-commit failure leaves no Vendor, event, outcome or outbox work.
16. The representative Vendor Registration journey is verified by Playwright Test with TypeScript through the rendered browser interface from a controlled clean state using synthetic, deterministic sample data: explicitly select an approved synthetic Address result, submit its opaque reference with the bound Trading Location, register a Vendor, capture the returned Vendor ID, retrieve the persisted Vendor using that ID and verify the displayed registered information.
17. The browser-based Vendor Registration journey can be demonstrated repeatedly in a controlled local or disposable non-production environment without treating test-owned identifiers as identity, authentication or authorisation mechanisms.
18. Any controlled verification crossing an external network boundary keeps access restricted and uses HTTPS; Vendor Registration and registered-Vendor retrieval are not exposed as publicly accessible production capabilities in Epic 1.
19. Gateway integration evidence proves transparent forwarding, safe gateway-owned pre-response failures, preserved valid W3C trace context, rejection of untrusted forwarding authority, absence of sensitive diagnostic payloads and no gateway retry of `POST /vendors`.
20. Registration Session evidence proves navigation retention, informed confirmation before destructive in-application reset, preservation after cancellation, Trading Location restart, dependent-state invalidation, one active submission, no automatic registration retry, retained intent after correctable or retryable failure, disposal after definitive success and separation of optional post-registration interaction.
21. The implemented Web journey satisfies traceable WCAG 2.2 Level AA, responsive, input-independent, authoritative-outcome and visual-fidelity acceptance criteria through complementary automated and structured manual evidence, without allowing mockups or client validation to become business authority.
22. The Angular Web client demonstrates the approved feature-oriented, routed and standalone-component structure; encapsulated feature-scoped signal state; strictly typed reactive forms; typed client ports and HTTP adapters; explicit submission states; and separate registered-Vendor retrieval and post-registration boundaries.
23. Docker Compose and the Azure Container Apps reference deployment reproduce the approved logical runtime with YARP as the only browser-facing origin, explicit pre-readiness migration, real PostgreSQL and RabbitMQ, responsibility-specific health and deterministic environment lifecycle.
24. The focused Playwright suite proves the mandatory full-stack registration-and-retrieval journey with isolated contexts and data, semantic interaction, deterministic waiting, governed retries and parallelism, protected failure evidence and the complementary CON-045 accessibility and visual-validation boundary.

Completion criterion 16 does not require every behavioural test in HJ-107 to be duplicated as a browser test. Browser automation focuses on representative user journeys, Web-client interaction and externally observable full-stack integration through the rendered Web interface.

Completion criteria 17 and 18 establish repeatable demonstration readiness and its exposure boundary. They do not bring presentation material, marketing content or production Web hosting into Epic 1 scope. Any future public or production exposure requires separately approved identity, authorisation, data-access and production-security concerns.

Real Compliance processing of the published `VendorRegistered` integration event is outside the Epic 1 completion boundary.

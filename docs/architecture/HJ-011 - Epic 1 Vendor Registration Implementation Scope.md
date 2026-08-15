# HJ-011 - Epic 1 Vendor Registration Implementation Scope

### Document Metadata

| Field | Value |
| --- | --- |
| **Document ID** | HJ-011 |
| **Document Title** | Epic 1 Vendor Registration Implementation Scope |
| **Version** | 1.1 |
| **Status** | Approved |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 13 August 2026 |

## Revision History

| Version | Date | Description |
| --- | --- | --- |
| 1.0 | 12 August 2026 | Initial approved version of the Epic 1 Vendor Registration Implementation Scope. |
| 1.1 | 13 August 2026 | Applied CR-036 to include the Centralized Configuration Service in the Epic 1 scope. |

## Related Documents

| Document ID | Title | Status |
| --- | --- | --- |
| HJ-001 | Project Vision | Approved |
| HJ-002 | Architectural Principles | Approved |
| HJ-003 | Ubiquitous Language Guide | Approved |
| HJ-004 | Vendor Domain Models | Approved |
| HJ-010 | Application Architecture and Implementation Pattern Map | Approved |
| HJ-104 | Vendor Registration Fields Matrix | Approved |
| HJ-105 | Vendor Registration Sequence Diagram | Approved |
| HJ-106 | Vendor Registration Service Contract | Approved |
| CR-036 | Include Centralized Configuration Service in Epic 1 Scope | Approved |

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
- Creation of the Vendor aggregate and its initial lifecycle state.
- Validation of mandatory and conditional Vendor Registration business rules.
- Registered Information captured at registration.
- Vendor Managed Information required during registration.
- Creation of the internal `VendorRegistered` domain event.
- Creation of the external `VendorRegistered` integration event.
- Retrieval of Registered Vendor Details.
- Register Vendor idempotency and duplicate-submission handling.

No Vendor behaviour beyond that required to complete and retrieve the registration is included.

## 2.2 Client / Interaction

Epic 1 implements:

- Vendor Web client required to execute the Vendor Registration journey.
- Web-client-owned Registration Session.
- Collection and client-side validation of registration information.
- Submission of `RegisterVendor`.
- Retrieval and display of Registered Vendor Details.

The Registration Session:

- remains outside the Vendor service boundary;
- is transient;
- is not persisted by the Vendor Domain;
- does not become Vendor state; and
- is discarded following successful registration or abandonment.

## 2.3 API and Service Boundary

Epic 1 implements:

- Vendor Service.
- `RegisterVendor` API.
- `RetrieveRegisteredVendor` query/API.
- API Gateway routing required to expose the Epic 1 Vendor endpoints.
- API contract validation.
- Transport-level error handling and response mapping.
- API versioning required by the approved Vendor Registration contract.

## 2.4 Data and Persistence

Epic 1 implements:

- Vendor PostgreSQL persistence.
- Persistence of the Vendor aggregate and Registered Information.
- Persistence required to support Register Vendor idempotency.
- Persistence required for reliable integration-event publication.
- Retrieval of persisted Registered Vendor Details.
- Database schema creation and migration required by Epic 1.

## 2.5 Messaging and Integration

Messaging is **in scope**.

Epic 1 implements:

- a real Event Bus / Message Broker;
- creation of the `VendorRegistered` integration event;
- atomic persistence of Vendor state and the obligation to publish the integration event;
- reliable publication using the agreed outbox/reliability mechanism;
- integration-event serialization and metadata;
- publication to the Event Bus / Message Broker; and
- end-to-end verification that the event can be received and deserialized by the Compliance Event Consumer Stub.

The asynchronous Epic 1 path is:

`Vendor Domain → Outbox / Reliable Publication → Event Bus / Message Broker → Compliance Event Consumer Stub`

## 2.6 Configuration

Configuration is **in scope**.

Epic 1 implements a Centralized Configuration Service providing centrally managed configuration required by the Epic 1 execution path.

Configuration retrieval shall be supported by the following components where configuration is applicable:

- Vendor Web client;
- API Gateway;
- Vendor Service;
- Address Domain Stub;
- Compliance Event Consumer Stub;
- Vendor persistence components;
- Event Bus / Message Broker and associated publication components; and
- other components already classified as In Scope, Stubbed / Simulated, or Supporting runtime / delivery for Epic 1, where explicitly identified by the implementation design.

Epic 1 configuration includes:

- application settings;
- environment-specific configuration;
- service and infrastructure endpoints;
- non-secret connection and integration settings; and
- configuration required to connect Epic 1 components consistently across environments.

Secrets, credentials and other sensitive configuration shall continue to be handled securely and shall not be exposed merely because configuration is centrally managed.

Feature-management behaviour is out of scope for Epic 1. The selected Centralized Configuration Service may possess such capability, but Epic 1 shall neither implement nor exercise it.

Centralized configuration and secret management remain distinct responsibilities. Inclusion of the Centralized Configuration Service does not change the requirement for secure handling of credentials, secrets and connection information.

## 2.7 Security

Security is **in scope**.

Epic 1 implements:

- HTTPS for externally exposed endpoints;
- secure handling of secrets and credentials;
- API boundary and input validation;
- protection against unauthorised modification or disclosure of registration data; and
- secure infrastructure configuration required by the Epic 1 slice.

A dedicated Authentication / Identity capability is not required for Vendor Registration.

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

The following are not implemented:

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

1. A prospective Vendor can complete registration through the Web client.
2. The Registration Session remains outside the Vendor service boundary.
3. The Vendor Service validates the authoritative Vendor Registration rules.
4. Required Address information is obtained through the Address Domain Stub.
5. A valid Vendor is created exactly once for an idempotent registration submission.
6. The Vendor is persisted successfully.
7. Registered Vendor Details can subsequently be retrieved.
8. The `VendorRegistered` integration event is durably and reliably published through the Event Bus / Message Broker.
9. Failure of event publication cannot result in loss of the publication obligation.
10. The published integration event is successfully received and deserialized by the Compliance Event Consumer Stub.
11. The complete Epic 1 execution can be observed and diagnosed sufficiently to identify registration, persistence and publication failures.
12. Epic 1 components requiring centrally managed configuration can retrieve and validate their required configuration from the Centralized Configuration Service.

Real Compliance processing of the published `VendorRegistered` integration event is outside the Epic 1 completion boundary.

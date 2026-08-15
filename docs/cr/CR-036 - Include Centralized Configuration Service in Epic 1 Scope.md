# CR-036 – Include Centralized Configuration Service in Epic 1 Scope

## 1. Change Summary

Amend **HJ-011 – Epic 1 Vendor Registration Implementation Scope** to include the **Centralized Configuration Service** as an Epic 1 **supporting runtime / delivery capability**.

The Configuration Service supports execution and deployment of the Epic 1 slice but is not itself a core Vendor Registration business capability.

Epic 1 shall use centralized configuration for the deployable components and supporting infrastructure that require runtime or environment-specific configuration.

Feature-management behaviour is out of scope for Epic 1. The selected Centralized Configuration Service may possess such capability, but Epic 1 shall neither implement nor exercise it.

## 2. Reason for Change

The current System Model includes a Centralized Configuration Service within the platform / architectural-furniture layer, while HJ-011 currently excludes a Central Configuration Service from Epic 1.

The System Model and HJ-011 must express the same implementation boundary.

Centralized configuration is required to provide a consistent configuration source for the Epic 1 components that require environment-specific or centrally managed configuration.

No approved Epic 1 behaviour currently requires feature-management behaviour. Feature management is therefore out of scope for Epic 1 and shall neither be implemented nor exercised.

## 3. Required Changes

### 3.1 HJ-011 §2.6 – Configuration

Amend §2.6 so that the **Centralized Configuration Service** is explicitly In Scope.

Replace the current configuration definition with wording equivalent to:

> ## 2.6 Configuration
>
> Configuration is **in scope**.
>
> Epic 1 implements a Centralized Configuration Service providing centrally managed configuration required by the Epic 1 execution path.
>
> Configuration retrieval shall be supported by the following components where configuration is applicable:
>
> - Vendor Web client;
> - API Gateway;
> - Vendor Service;
> - Address Domain Stub;
> - Compliance Event Consumer Stub;
> - Vendor persistence components;
> - Event Bus / Message Broker and associated publication components; and
> - other components already classified as In Scope, Stubbed / Simulated, or Supporting runtime / delivery for Epic 1, where explicitly identified by the implementation design.
>
> Epic 1 configuration includes:
>
> - application settings;
> - environment-specific configuration;
> - service and infrastructure endpoints;
> - non-secret connection and integration settings; and
> - configuration required to connect Epic 1 components consistently across environments.
>
> Secrets, credentials and other sensitive configuration shall continue to be handled securely and shall not be exposed merely because configuration is centrally managed.
>
> Feature-management behaviour is out of scope for Epic 1. The selected Centralized Configuration Service may possess such capability, but Epic 1 shall neither implement nor exercise it.

The section shall distinguish **centralized configuration** from **secret management**. Inclusion of the Centralized Configuration Service does not change the existing requirement for secure handling of credentials, secrets and connection information.

### 3.2 HJ-011 §3.3 – Platform and Architectural Plumbing

Remove:

> Central Configuration Service

from the Out of Scope list.

No replacement exclusion shall be added.

### 3.3 Feature Flags

HJ-011 shall define feature-management behaviour as **out of scope for Epic 1** and shall not require or exercise:

- runtime feature toggles;
- rollout percentages;
- user or cohort targeting;
- kill switches;
- configuration-driven feature activation; or
- any other feature-management behaviour

for Epic 1.

No Epic 1 business behaviour currently depends upon such capability.

The selected Centralized Configuration Service may technically possess feature-management capabilities, but Epic 1 shall neither implement nor exercise them.

### 3.4 HJ-011 §5 – Epic 1 Completion Boundary

Add the following completion criterion to HJ-011 §5:

> Epic 1 components requiring centrally managed configuration can retrieve and validate their required configuration from the Centralized Configuration Service.

This ensures that the Centralized Configuration Service is not merely classified within the Epic 1 boundary, but is exercised and verifiably usable by the components that depend upon it.

### 3.5 System Model Alignment

Where the Epic 1 System Model describes the Configuration Service as:

> Centralized configuration, feature flags

replace that wording with:

> Centralized configuration

Feature-management wording shall not appear in the Epic 1 Configuration Service description.

The Centralized Configuration Service shall remain visually classified as **Supporting runtime / delivery**, consistent with the System Model Scope Key.

It shall not be reclassified as a core green **Required for Epic 1** capability.

This change affects only the feature-management description; it does not remove the Configuration Service itself.

## 4. Configuration Consumption Boundary

The Centralized Configuration Service is a supporting platform capability.

Its inclusion does not make every configuration value globally shared.

Each component remains responsible for:

- defining the configuration it consumes;
- consuming only configuration relevant to its responsibility;
- validating required configuration at the appropriate boundary; and
- avoiding hidden behavioural coupling through shared configuration.

Centralized configuration shall not be used as a substitute for:

- explicit service contracts;
- domain ownership;
- Integration Events;
- persistence ownership; or
- business rules.

## 5. Explicit Non-Changes

This Change Request does not change:

- Vendor Registration behaviour;
- Vendor Domain ownership;
- the Address Domain Stub;
- the Compliance Event Consumer Stub;
- API Gateway scope;
- Vendor persistence;
- Event Bus / Message Broker scope;
- reliable `VendorRegistered` publication;
- security requirements;
- secret-management requirements;
- observability requirements;
- Authentication / Identity Service scope;
- Service Registry / Service Discovery scope; or
- any other In Scope, Out of Scope or Stubbed / Simulated classification in HJ-011.

This Change Request does not introduce feature management into Epic 1. Feature-management behaviour remains out of scope.

## 6. Impacted Artefacts

| Artefact | Required Change |
| --- | --- |
| **HJ-011 – Epic 1 Vendor Registration Implementation Scope** | Add Centralized Configuration Service to §2.6, remove it from §3.3, and add the §5 configuration-retrieval completion criterion |
| **Epic 1 System Model** | Retain Centralized Configuration Service as Supporting runtime / delivery and remove feature-flag wording from the Epic 1 capability description |

## 7. Document Control

When CR-036 is applied to HJ-011:

- increment the document version in accordance with the established versioning convention;
- update **Last Updated** to **13 August 2026**;
- add a Revision History entry identifying CR-036; and
- add CR-036 to Related Documents where required by the established document convention.

## 8. Acceptance Criteria

CR-036 is complete when:

- Centralized Configuration Service is explicitly In Scope in HJ-011 §2.6;
- HJ-011 identifies the Epic 1 components that consume centralized configuration where applicable;
- Central Configuration Service no longer appears in HJ-011 §3.3 or elsewhere as Out of Scope;
- HJ-011 §5 requires Epic 1 components that depend on centralized configuration to retrieve and validate their required configuration from the Centralized Configuration Service;
- configuration and secret management remain explicitly distinguishable responsibilities;
- feature-management behaviour is explicitly out of scope in HJ-011 and is neither implemented nor exercised by Epic 1;
- the System Model Configuration Service description is **Centralized configuration** and contains no feature-management wording;
- HJ-011 includes centralized configuration within the Epic 1 implementation boundary as a supporting runtime / delivery capability;
- the System Model continues to classify the Configuration Service as **Supporting runtime / delivery**, rather than as a core **Required for Epic 1** capability; and
- no unrelated Epic 1 scope classification is changed.

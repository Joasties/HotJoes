# ADR-009 – Azure Reference Cloud, Centralized Configuration and Secret Management

| **Document ID** | ADR-009 |
|---|---|
| **Document Title** | Azure Reference Cloud, Centralized Configuration and Secret Management |
| **Version** | 1.0 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 31 August 2026 |

---

# Revision History

| Version | Date | Description |
|---|---|---|
| 1.0 | 31 August 2026 | Applied CR-TBD-ADR009. Established Azure as the Epic 1 reference cloud and approved Azure App Configuration, Azure Key Vault, managed identity, validated snapshot, geo-replicated bootstrap and safe credential-rotation boundaries for CON-032 and CON-033. |

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
| CR-036 | Include Centralized Configuration Service in Epic 1 Scope | Approved |

---

# 1. Context

Epic 1 requires a Centralized Configuration Service, secure secret handling and a reproducible reference deployment. The selected cloud affects configuration, credentials, identity, runtime composition and hosted verification, but it must not leak provider representations into HotJoes business or service contracts.

HotJoes is intended to demonstrate credible cloud delivery while controlling intermittent development cost. Azure offers direct .NET configuration integration, managed identity, Key Vault and scale-to-zero container execution. AWS remains a credible future alternative, but simultaneous multi-cloud implementation would introduce cost and complexity without an Epic 1 business requirement.

Configuration and secret availability affect safe recovery. A running service must not discard known-valid configuration during a transient outage, while a new instance must not enter service using partial, invalid or arbitrarily stale inputs. Credential rotation must not revoke the credential still required by healthy instances.

# 2. Decision

## 2.1 Reference Cloud and Portability Boundary

Azure shall be the primary cloud platform for the Epic 1 reference deployment and its hosted architectural evidence. HotJoes shall use Azure-managed platform services where they provide the required configuration, secret-management, identity and runtime capabilities economically for intermittent development use.

Cloud-provider SDKs, identities and resource representations shall remain confined to deployable composition roots and Infrastructure adapters. Domain, Application, API contract, persistence model and Integration Event contracts shall remain cloud-independent.

AWS is not an Epic 1 deployment target. It is retained as a future portability option, not a simultaneously implemented abstraction requirement. Supporting AWS requires provider-specific pre-production deployment and integration evidence before it may be selected for production.

## 2.2 Centralized Non-Secret Configuration

Azure App Configuration shall provide production non-secret configuration. Each deployable owns strongly typed options containing only the values required by that deployable and validates its complete required configuration before readiness.

Production configuration shall be promoted as a validated immutable snapshot. A configuration release replaces one complete validated snapshot with another. A running service retains its current validated snapshot while App Configuration is temporarily unavailable or while a replacement snapshot fails validation.

Runtime refresh is permitted only for settings explicitly classified as reload-safe. Settings affecting persistence, message delivery, security boundaries, published contracts or other consistency-sensitive infrastructure require controlled health-gated rolling restart unless their consumers explicitly implement and verify safe atomic reload. Feature-management behaviour remains outside Epic 1.

Production App Configuration shall use cross-region replication and provider failover. A newly starting or recovering service does not report readiness until it has obtained and validated one complete approved snapshot. If no approved replica is available, the instance fails readiness. An unmanaged machine-local cache is not cold-start authority.

## 2.3 Secrets and Credentials

Production workloads shall use managed identity instead of stored credentials wherever the target Azure service supports it. Remaining production secrets shall be held in Azure Key Vault. App Configuration may hold only non-secret references needed to locate them and shall never hold secret values.

Every remaining secret has an identified owner, purpose, consuming deployables, permitted lifetime, rotation mechanism and recovery procedure. Local execution uses developer identity or controlled local secret injection; local values are not production authority.

Secrets shall not be committed to source control, logged, returned through APIs, emitted in configuration diagnostics or included in recovery evidence.

## 2.4 Secret Rotation and Recovery

Secret rotation shall use versioning and overlap-and-cutover wherever the protected resource supports concurrent or dual credentials: create and validate the replacement, make it available to consumers, complete a controlled rolling restart or explicitly verified safe refresh, and only then revoke the previous credential.

Runtime secret refresh is permitted only where the consuming client can atomically replace the credential without corrupting in-flight work or losing durable processing obligations. Otherwise rotation uses health-gated rolling replacement of service instances.

Rotation failure retains or restores the last valid credential while it remains secure and valid, raises an operational alert and stops before revoking the credential required by healthy instances.

# 3. Rationale

- Azure is the lowest-friction primary reference cloud for the .NET implementation and intermittent hosted evidence.
- Provider isolation preserves a practical future AWS migration boundary without creating speculative multi-cloud abstractions.
- Immutable snapshots make configuration promotion, rollback and audit explicit.
- Geo-replication removes a single regional App Configuration dependency from recovery.
- Fail-closed cold start protects customer and system consistency when authoritative inputs are unavailable.
- Managed identity removes avoidable application credentials; overlap-and-cutover protects availability for unavoidable secrets.

# 4. Alternatives Considered

## 4.1 AWS as the Epic 1 Reference Cloud

Rejected for Epic 1 because it adds more container, identity and configuration integration work for the current .NET solution and does not improve the present development-cost objective sufficiently to justify that work.

## 4.2 Simultaneous Azure and AWS Implementations

Rejected because no Epic 1 requirement needs two supported clouds. The portability boundary is retained without implementing both provider stacks.

## 4.3 Environment Variables or Local Files as the Production Configuration Authority

Rejected because they do not satisfy the approved centralized-management, controlled-promotion, geo-recovery and audit boundaries.

## 4.4 Cold Start from an Unmanaged Local Cache

Rejected because a recovered process could enter service with unknown or obsolete authority. Geo-replicated authoritative configuration is preferred; absence of all approved sources fails readiness.

# 5. Consequences

- Azure App Configuration and Key Vault become explicit Epic 1 supporting services.
- Production deployment requires regional and cross-region configuration topology, least-privilege identities and readiness evidence.
- Configuration and secret consumers require explicit reload-safe or restart-required classification.
- Cloud-neutral business, API, persistence and event contracts remain unchanged.
- Existing Domain, Application, API, PostgreSQL and reliable-publication work is not redesigned by this decision.
- Broader platform availability, data recovery, queue replay, Ordering/Delivery recovery priority and disaster exercises remain separate future architectural decisions.

# 6. Verification

Conformance requires HJ-013 evidence for configuration ownership and environment separation; immutable complete-snapshot validation; invalid-refresh rejection; last-valid running continuity; replica failover; cold-start readiness failure; secret absence and leakage prevention; managed identity and least privilege; versioned overlap rotation; rolling replacement; rollback; and provider-type isolation from Domain and Application assemblies.

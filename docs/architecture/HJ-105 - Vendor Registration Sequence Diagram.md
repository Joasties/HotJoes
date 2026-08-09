# HJ-105 - Vendor Registration Sequence Diagram

| Property | Value |
|---|---|
| **Document ID** | HJ-105 |
| **Document Title** | Vendor Registration Sequence Diagram |
| **Version** | 3.2 |
| **Status** | Approved |
| **Classification** | Model |
| **Owner** | Project Architecture |
| **Last Updated** | 8 August 2026 |

## Revision History

| Version | Date | Description |
|---|---|---|
| 0.1 | 22 July 2026 | Initial Vendor Registration behavioural model for Epic 1, including the successful registration flow, validation failure, Registration Session expiry, Address Domain collaboration and Compliance Requirement initiation. |
| 0.2 | 24 July 2026 | Added mandatory Registration Declarations to the registration request, server-side validation, validation checklist and sequence guidance. |
| 2.0 | 24 July 2026 | Approved the consolidated Vendor Registration behavioural baseline. |
| 3.0 | 28 July 2026 | Regenerated from the approved Vendor language, domain model and registration requirements and the accepted architectural decisions. Realigned Registration Session ownership, request and Address trust boundaries, creation invariants, idempotency, Domain and Integration Events, reliable publication and downstream activation initiation. |
| 3.1 | 8 August 2026 | Applied CR-026 to add the separate Epic 1 Retrieve Registered Vendor sequence, including the successful and Vendor Not Found outcomes and query boundary constraints. |
| 3.2 | 8 August 2026 | Applied CR-029 to incorporate the controlled idempotency-conflict behaviour defined by ADR-008 and update ADR-008 traceability references. No business behaviour changed beyond alignment with the approved architecture. |

## Related Documents

| Document ID | Title | Status | Relevance |
|---|---|---|---|
| HJ-002 | Architectural Principles | Approved | Supporting architectural principles |
| HJ-003 | Ubiquitous Language Guide | Approved | Authoritative business terminology |
| HJ-004 | Vendor Domain Models | Approved | Vendor aggregate, invariants, lifecycle and event model |
| HJ-104 | Vendor Registration Fields Matrix | Approved | Authoritative registration information and business rules |
| ADR-002 | Business Capabilities and Bounded Contexts | Accepted | Capability and data ownership |
| ADR-003 | Event-Driven Collaboration | Accepted | Cross-context event collaboration |
| ADR-004 | Vendor Lifecycle Begins After Successful Registration | Accepted | Registration Session, service and lifecycle boundaries |
| ADR-005 | Registered Information vs Vendor Managed Information | Accepted | Information classification |
| ADR-006 | Address Domain Ownership and Business Address Snapshots | Accepted | Address trust boundary and snapshot ownership |
| ADR-007 | Vendor Compliance as a Separate Bounded Context | Accepted | Compliance boundary and activation dependency |
| ADR-008 | Idempotent Operations and Reliable Event Publication | Accepted | Idempotency and publication reliability |
| CR-026 | Define Registered Vendor Retrieval for Epic 1 | Approved | Registered Vendor retrieval behaviour and scope |

# 1. Purpose and Authority

This document defines the runtime interactions required to complete **Epic 1 – Vendor Registration**, including the associated minimal retrieval of a registered Vendor.

It is the behavioural bridge between the approved registration language, registration-information contract, Vendor aggregate and accepted architectural decisions. It describes interaction order and responsibility; it does not redefine the underlying business rules.

The authoritative sources are applied in the following order for their respective concerns:

- **HJ-003** for business terminology;
- **HJ-104** for registration fields, classifications, validation and information lifecycle;
- **HJ-004** for aggregate behaviour, creation invariants, resulting state and events;
- the accepted ADRs for capability ownership, Address ownership, lifecycle boundaries, idempotency, reliable publication and cross-context collaboration.

Approved Change Requests are applied through the revised production artefacts listed above. CR-026 is reflected directly to define the registered Vendor retrieval capability, and CR-029 is reflected directly to align controlled idempotency-conflict behaviour and ADR-008 traceability.

# 2. Scope and Boundary

This document covers:

- optional client/BFF assembly of registration information;
- Address search and selection before registration submission;
- submission of one complete, self-contained `RegisterVendor` request;
- server-side validation of client-authored information and Registration Declarations;
- authoritative Address Domain retrieval using the approved Address Resolution reference;
- Vendor aggregate creation and invariant enforcement;
- Vendor persistence and durable Integration Event recording;
- synchronous confirmation of successful registration;
- asynchronous publication of `VendorRegistered`;
- initiation of the Pending Activation Process;
- validation, address, invariant, persistence and publication failures;
- idempotent and concurrent duplicate processing;
- retrieval of one registered Vendor by VendorId;
- mapping of persisted Vendor state to Registered Vendor Details; and
- the controlled Vendor Not Found outcome.

This document does not place Registration Session management inside the Vendor Registration service. It also excludes:

- persisted or resumable registration drafts;
- Identity account creation or authentication;
- compliance evidence submission, evaluation or continuing monitoring;
- completion of the Pending Activation Process;
- Vendor Activation;
- registration amendment;
- multiple premises or Branches;
- Menu, Ordering, Payment and Delivery capabilities;
- Vendor search, filtering, paging or multiple-Vendor retrieval;
- retrieval authentication, authorisation or Identity collaboration;
- Address or Compliance callbacks during retrieval; and
- dedicated read-model infrastructure.

# 3. Architectural Rules Applied

| Rule | Behavioural application |
|---|---|
| Client interaction boundary | A Registration Session, if used, is owned entirely by the Registration UI or BFF and never participates in Vendor Registration service processing. |
| Registration boundary | Vendor Registration begins only when the service receives a complete `RegisterVendor` request. |
| Request authority | The request is authoritative for client-authored registration information and the approved Address Resolution reference. |
| Address authority | Address-owned values are retrieved directly from the Address Service using the approved reference and are never trusted from client-authored values. |
| Server authority | User-interface validation is advisory; server-side validation and aggregate invariants are authoritative. |
| Vendor existence | No Vendor exists before successful processing. A successful request creates the Vendor in `PendingActivation` and `Offline`. |
| Declaration lifecycle | Registration Declarations influence the registration decision only and are never persisted or included in Domain or Integration Events. |
| Event separation | The internal `VendorRegistered` Domain Event and published `VendorRegistered` Integration Event are distinct architectural concepts. |
| Reliable publication | Vendor persistence and durable recording of publication work occur atomically. Publication may be retried without repeating registration. |
| Idempotency | `RegisterVendor` uses an explicit idempotency identity or equivalent approved uniqueness constraint. Successful replay returns the original outcome without additional business effects. Reuse of the same identity with semantically different registration information returns a controlled idempotency-conflict outcome without creating or changing business state. |
| Capability autonomy | The Pending Activation Process consumes the Integration Event and does not synchronously retrieve registration data from the Vendor Domain. |
| Retrieval lookup | `RetrieveRegisteredVendor` uses VendorId as its sole lookup input and performs no search. |
| Retrieval authority | The persisted Vendor aggregate is the authoritative Epic 1 read source; the application maps it to Registered Vendor Details rather than exposing it directly. |
| Retrieval isolation | Registered Vendor retrieval is read-only, has no events or lifecycle effects and invokes no other bounded context. |

# 4. Participants and Responsibilities

| Participant | Responsibility |
|---|---|
| Prospective Vendor | Supplies registration information, confirms all Registration Declarations and submits Vendor Registration. |
| Registration UI / BFF | Owns any temporary interaction state, performs convenience validation, coordinates Address search and selection, assembles the complete request and presents the result. |
| Address Service | Provides Address search and approved Address Resolution references; authoritatively returns canonical identity, immutable snapshot and applicable regulatory authorities. |
| Vendor Registration Application | Establishes idempotency, validates the complete request, obtains Address-owned values, invokes the aggregate, coordinates persistence and returns the authoritative result. |
| Vendor Aggregate | Enforces Vendor creation invariants, creates the Vendor and records the internal completed business fact. |
| Vendor Repository | Persists the Vendor aggregate owned by the Vendor Domain. |
| Transactional Outbox | Durably records publication work in the same atomic unit as Vendor persistence. |
| Integration Event Publisher | Publishes recorded Integration Events and retries failed publication without repeating registration. |
| Event Bus | Delivers published contracts to subscribing capabilities. |
| Pending Activation Process | Reacts to `VendorRegistered` and coordinates post-registration activation work. |
| Compliance Requirement Provider | Determines applicable Compliance Requirements within the Compliance capability. |

A Registration Session is deliberately absent from the server-side participant list. A client/BFF may use local interaction state, but the Vendor Registration Application neither knows nor depends on it.

# 5. Complete RegisterVendor Request

The Registration UI or BFF shall submit one complete, self-contained request containing:

- an idempotency identity, unless the implementation uses an approved equivalent uniqueness constraint;
- all client-authored Registered Information required by HJ-104;
- optional Vendor Managed Information when supplied;
- all mandatory Registration Declarations; and
- the approved Address Resolution reference.

The request does **not** authoritatively supply:

- Canonical Address Identifier;
- Business Address Snapshot;
- Food Registration Authority; or
- Primary Trading Authority.

Those values originate exclusively from the Address Domain. Client-supplied snapshot or regulatory-authority values shall be ignored or rejected.

The three mandatory Registration Declarations are:

- Authorised to Register Business;
- Information Accurate; and
- Accept HotJoes Platform Terms.

They are transient inputs to the registration decision. They are not Registered Information or Vendor Managed Information, are not stored on the aggregate, appear in neither event type and have no lifecycle after request processing. Any permitted audit retention is outside the Vendor Domain.

# 6. Client Assembly and Address Selection

This sequence occurs outside the Vendor Registration service boundary. The Registration UI or BFF may retain temporary information locally while the prospective Vendor completes the interaction.

```mermaid
sequenceDiagram
    autonumber
    actor Applicant as Prospective Vendor
    participant Client as Registration UI / BFF
    participant LocalState as Client-owned interaction state
    participant Address as Address Service

    Applicant->>Client: Begin Vendor Registration
    opt Client chooses to retain temporary interaction state
        Client->>LocalState: Create or update transient Registration Session
    end

    loop Enter registration information
        Applicant->>Client: Enter or amend information
        Client-->>Applicant: Present convenience validation
        opt Temporary state is retained
            Client->>LocalState: Retain interaction state
        end
    end

    Applicant->>Client: Search for Business Address
    Client->>Address: SearchAddress(search criteria)
    Address-->>Client: Candidate addresses
    Client-->>Applicant: Display candidate addresses

    Applicant->>Client: Select candidate address
    Client->>Address: ResolveAddress(candidate reference)
    alt Address Resolution approved
        Address-->>Client: Approved Address Resolution reference and display result
        opt Temporary state is retained
            Client->>LocalState: Retain approved reference
        end
    else Address Resolution rejected
        Address-->>Client: Address validation errors
        Client-->>Applicant: Correct search or select another address
    end

    Applicant->>Client: Confirm mandatory Registration Declarations
    Client->>Client: Assemble complete RegisterVendor request
    Note over Client: Temporary interaction state remains client/BFF-owned.<br/>Vendor Registration has not yet begun.
```

The displayed Address result supports selection and user confirmation. It is not the authoritative source persisted by the Vendor. Authoritative Address-owned values are retrieved again by the Vendor Registration Application using the approved reference during request validation.

# 7. Successful Vendor Registration

```mermaid
sequenceDiagram
    autonumber
    actor Applicant as Prospective Vendor
    participant Client as Registration UI / BFF
    participant Application as Vendor Registration Application
    participant Address as Address Service
    participant Vendor as Vendor Aggregate
    participant Repository as Vendor Repository
    participant Outbox as Transactional Outbox
    participant Publisher as Integration Event Publisher
    participant Bus as Event Bus
    participant Pending as Pending Activation Process
    participant Compliance as Compliance Requirement Provider

    Applicant->>Client: Submit Vendor Registration
    Note over Client,Application: Client/BFF assembles the complete request before invocation.
    Client->>Application: RegisterVendor(complete request, idempotency identity)

    Application->>Application: Establish idempotency outcome
    Application->>Application: Validate request completeness and field rules
    Application->>Application: Validate all Registration Declarations
    Application->>Application: Validate Legal Operator and Trading Characteristics rules

    Application->>Address: Get approved Address result(reference)
    Address->>Address: Validate reference and canonical Address
    Address->>Address: Derive Food Registration Authority
    opt Trading Location = Stall
        Address->>Address: Derive Primary Trading Authority
    end
    Address-->>Application: Canonical Address Identifier<br/>immutable Business Address Snapshot<br/>applicable regulatory authorities

    Application->>Application: Reject or ignore any client-authored Address-owned values
    Application->>Vendor: Create Vendor(complete validated Domain input)
    Vendor->>Vendor: Enforce creation invariants
    Vendor->>Vendor: Create VendorId and RegisteredAt
    Vendor->>Vendor: Store Registered and Vendor Managed Information
    Vendor->>Vendor: Set Vendor State = PendingActivation
    Vendor->>Vendor: Set Trading Preference = Offline
    Vendor->>Vendor: Record internal VendorRegistered Domain Event
    Vendor-->>Application: Vendor created and completed business fact

    Application->>Application: Derive VendorRegistered Integration Event
    Application->>Repository: Begin atomic persistence operation
    Repository->>Repository: Persist Vendor aggregate
    Application->>Outbox: Record Integration Event publication work
    Outbox-->>Application: Publication work recorded
    Repository-->>Application: Atomic commit successful

    Application->>Application: Complete registration transaction<br/>Discard transient declarations from request scope
    Application-->>Client: Registration successful<br/>VendorId and PendingActivation
    Client-->>Applicant: Display confirmed registration outcome

    Publisher->>Outbox: Read unpublished Integration Event
    Publisher->>Bus: Publish VendorRegistered Integration Event
    Bus-->>Pending: Deliver VendorRegistered Integration Event
    Pending->>Compliance: Get applicable Compliance Requirements<br/>using Integration Event information
    Compliance-->>Pending: Applicable Compliance Requirements
    Pending->>Pending: Begin Pending Activation coordination
```

The synchronous response confirms the committed Vendor state. Pending Activation and Compliance processing proceed asynchronously and do not change the successful registration response.

The Pending Activation Process obtains the information required to begin processing from the published Integration Event. It shall not synchronously query the Vendor Domain for registration values already known when registration completed.

# 8. Validation and Creation Failure

```mermaid
sequenceDiagram
    autonumber
    actor Applicant as Prospective Vendor
    participant Client as Registration UI / BFF
    participant Application as Vendor Registration Application
    participant Address as Address Service
    participant Vendor as Vendor Aggregate
    participant Repository as Vendor Repository
    participant Outbox as Transactional Outbox

    Applicant->>Client: Submit Vendor Registration
    Client->>Application: RegisterVendor(complete request, idempotency identity)
    Application->>Application: Validate request and Registration Declarations

    alt Request or declaration validation fails
        Application-->>Client: Structured validation errors
        Client-->>Applicant: Display fields requiring correction
    else Request validation succeeds
        Application->>Address: Get approved Address result(reference)
        alt Address reference or Address result is invalid
            Address-->>Application: Controlled Address validation failure
            Application-->>Client: Structured Address error
            Client-->>Applicant: Correct or select another address
        else Authoritative Address result returned
            Address-->>Application: Canonical identifier, snapshot and authorities
            Application->>Vendor: Create Vendor(complete validated Domain input)
            alt Aggregate invariant fails
                Vendor-->>Application: Domain validation errors
                Application-->>Client: Structured validation errors
                Client-->>Applicant: Display fields requiring correction
            else Aggregate creation succeeds
                Vendor-->>Application: Vendor and completed business fact
                Application->>Repository: Persist Vendor atomically with publication work
                Application->>Outbox: Record Integration Event publication work
            end
        end
    end

    Note over Repository,Outbox: For every validation, Address or invariant failure:<br/>no Vendor is persisted;<br/>no completed business fact or Domain Event is recorded;<br/>no publication record or Integration Event exists.
```

The Vendor Registration service retains no Registration Session after failure. The UI or BFF alone decides whether to retain client interaction state so the applicant can correct and resubmit information.

Server-side validation shall enforce all HJ-104 rules, including:

- all mandatory fields and formats;
- controlled Legal Operator Type and Trading Location values;
- conditional Company Registration Number presence and canonical uppercase storage;
- Opening Hours that permit legitimate overnight periods;
- all three mandatory Registration Declarations;
- approved Address Resolution reference;
- Primary Trading Authority presence only for `Stall`;
- optional Website HTTPS format; and
- optional Business Description maximum length.

# 9. Idempotent Replay and Concurrency

`RegisterVendor` is not naturally idempotent. It shall use an explicit idempotency identity or an equivalent approved uniqueness constraint over the complete request boundary.

```mermaid
sequenceDiagram
    autonumber
    participant Client as Registration UI / BFF
    participant Application as Vendor Registration Application
    participant Idempotency as Idempotency Safeguard
    participant Repository as Vendor Repository
    participant Outbox as Transactional Outbox
    participant Pending as Pending Activation Process

    Client->>Application: RegisterVendor(request, idempotency identity)
    Application->>Idempotency: Establish or retrieve request outcome

    alt First processing of request
        Idempotency-->>Application: Processing ownership established
        Application->>Application: Execute registration once
        Application->>Repository: Persist one Vendor
        Application->>Outbox: Record one publication item
        Application->>Idempotency: Store successful outcome atomically or equivalently safely
        Application-->>Client: Original successful outcome
    else Same identity and semantically identical successful request
        Idempotency-->>Application: Previously successful outcome
        Application-->>Client: Return original successful outcome
        Note over Repository,Pending: No additional Vendor, Domain Event,<br/>business fact, outbox record, Integration Event<br/>or Pending Activation Process.
    else Same identity and registration information is not semantically identical
        Idempotency-->>Application: Idempotency conflict
        Application-->>Client: Return controlled idempotency-conflict outcome
        Note over Application,Pending: No Vendor is created or modified.<br/>No completed business fact, Domain Event,<br/>publication work, Transactional Outbox entry<br/>or Integration Event is recorded or published.<br/>No Pending Activation Process is initiated.<br/>Previously committed Vendor state remains unchanged.
    end
```

The three outcomes are distinct:

- first successful processing executes registration once;
- replay applies only to the same identity and the same semantically identical successful request and returns the original outcome; and
- reuse of the same identity with registration information that is not semantically identical returns the controlled idempotency-conflict outcome.

Conflict processing creates no Vendor, modifies no existing Vendor, records no completed business fact, Domain Event, publication work or Transactional Outbox entry, publishes no Integration Event, initiates no Pending Activation Process and leaves all previously committed Vendor state unchanged.

Concurrent requests with the same identity and semantically identical information must converge on one processing owner and one successful outcome. Registration Session state is outside the idempotency boundary and shall never be consulted. The complete `RegisterVendor` request remains the idempotency boundary.

The idempotency-key format, retention duration, equivalence algorithm, storage mechanism and concurrency technique are implementation conventions governed by ADR-008.

# 10. Persistence and Publication Reliability

Vendor persistence and durable recording of the corresponding Integration Event publication work form one atomic operation.

```mermaid
sequenceDiagram
    autonumber
    participant Application as Vendor Registration Application
    participant Repository as Vendor Repository
    participant Store as Vendor Data Store
    participant Outbox as Transactional Outbox
    participant Publisher as Integration Event Publisher
    participant Bus as Event Bus

    Application->>Repository: Persist Vendor and publication work
    Repository->>Store: Begin transaction
    Repository->>Store: Persist Vendor aggregate
    Repository->>Outbox: Add VendorRegistered Integration Event record
    Outbox->>Store: Persist publication record
    Repository->>Store: Commit transaction
    Store-->>Application: Commit successful
    Application-->>Application: Registration may be confirmed

    loop Until publication is acknowledged
        Publisher->>Outbox: Read unpublished record
        Publisher->>Bus: Publish VendorRegistered Integration Event
        Bus-->>Publisher: Publication acknowledged
        Publisher->>Outbox: Mark record published
    end
```

The internal Domain Event is not required to be the outbox message. The Integration Event is a published contract derived from the completed business fact and may have a different representation.

## 10.1 Minimum VendorRegistered Integration Event Contract

The published `VendorRegistered` Integration Event shall contain, at a minimum:

- VendorId;
- RegisteredAt;
- resulting Vendor State;
- Trading Preference;
- Legal Operator Type;
- Trading Characteristics;
- the approved registration-time Business Address information required by downstream Compliance processing;
- Food Registration Authority; and
- Primary Trading Authority, where applicable.

### Business Address Payload Deferral

This sequence defines the required business semantics of the Address information carried by the VendorRegistered Integration Event, but it does not define its concrete wire schema.
The Address information must:
- originate from the authoritative Address Domain result used to create the Vendor;
- represent the approved registration-time Business Address;
- be sufficient for the Pending Activation Process and Compliance capability to begin processing without synchronously querying the Vendor Domain; and
- exclude client-authored or independently reconstructed Address information.
The exact fields, structure, naming and serialization of this element are deferred until the Compliance-facing integration contract is defined.

Registration Declarations shall never appear in the Integration Event. Implementation-specific metadata may be added provided the minimum business content and contract semantics are preserved.

# 11. Client-Owned Registration Session Lifecycle

A Registration Session is optional client/BFF interaction state and is not part of Vendor Registration processing.

```mermaid
sequenceDiagram
    autonumber
    actor Applicant as Prospective Vendor
    participant Client as Registration UI / BFF
    participant LocalState as Client-owned interaction state

    Applicant->>Client: Begin registration interaction
    opt Client uses a Registration Session
        Client->>LocalState: Create transient state
        Applicant->>Client: Enter partial information
        Client->>LocalState: Update transient state
    end

    alt Abandoned or expired
        Client->>LocalState: Discard transient state
        Client-->>Applicant: Start a new registration interaction
    else Complete request submitted
        Client->>LocalState: Discard when no longer required
        Client-->>Applicant: Continue using authoritative server response
    end

    Note over Client,LocalState: No Vendor, business fact, Domain Event<br/>or Integration Event is created by this lifecycle.
```

The concrete storage mechanism, timeout and expiry policy are client implementation decisions. An abandoned or expired Registration Session cannot be resumed as Vendor business state.

# 12. Failure Behaviour

## 12.1 Request or Declaration Validation Failure

- Return structured validation errors.
- Persist no Vendor.
- Record no completed business fact, Domain Event, publication work or Integration Event.
- Retain no server-side interaction state.

## 12.2 Address Failure

- Reject invalid, expired or unresolved Address Resolution references.
- Reject or ignore client-authored Address-owned values.
- Persist no Vendor and record no event or publication work.
- Return a controlled error suitable for correction or reselection.

## 12.3 Aggregate Invariant Failure

- Return structured Domain validation errors.
- Persist no Vendor and record no event or publication work.
- Do not expose technical exceptions as business validation messages.

## 12.4 Persistence or Atomic Recording Failure

- Roll back Vendor persistence and publication work.
- Do not report registration as successful.
- Permit safe retry through the idempotency safeguard.
- Prevent a partial Vendor or orphaned publication record.

## 12.5 Integration Event Dispatch Failure

- Leave the successfully registered Vendor unchanged.
- Keep the durable publication record available for retry.
- Retry publication without repeating `RegisterVendor`.
- Do not require the applicant to register again.

## 12.6 Duplicate or Concurrent Submission

- Return the original successful outcome for the same identity and semantically identical request.
- Create no additional Vendor, business fact, Domain Event, publication record or Integration Event.
- Initiate no additional Pending Activation Process.

## 12.7 Idempotency Conflict

- Return a controlled idempotency-conflict outcome when the same identity is associated with registration information that is not semantically identical to the previously successful request.
- Create no Vendor and modify no existing Vendor.
- Record no completed business fact, Domain Event, publication work or Transactional Outbox entry.
- Publish no Integration Event.
- Initiate no Pending Activation Process.
- Leave all previously committed Vendor state unchanged.

# 13. Resulting Vendor State

After successful registration:

| Property | Result |
|---|---|
| Vendor exists | Yes |
| Vendor State | `PendingActivation` |
| Trading Preference | `Offline` |
| Vendor may trade | No |
| VendorId | Created |
| RegisteredAt | Recorded |
| Registered Information | Persisted and read-only to the Vendor |
| Vendor Managed Information | Persisted when supplied |
| Canonical Address Identifier | Persisted from the Address Domain |
| Business Address Snapshot | Immutable snapshot persisted exactly as returned by the Address Domain |
| Food Registration Authority | Persisted from the Address Domain |
| Primary Trading Authority | Persisted only when Trading Location is `Stall` |
| Registration Declarations | Not persisted and absent from both event types |
| Internal `VendorRegistered` Domain Event | Records the completed business fact |
| `VendorRegistered` Integration Event | Durably recorded and published |
| Pending Activation Process | Initiated asynchronously from the Integration Event |
| Compliance Requirements | Requested through the Compliance capability |

# 14. Retrieve Registered Vendor

This section defines the read-only registered Vendor retrieval capability. It is separate from the `RegisterVendor` command flow and does not participate in registration processing.

## 14.1 Participants

| Participant | Responsibility |
|---|---|
| Vendor Administrator | Supplies the VendorId and receives either Registered Vendor Details or the controlled Vendor Not Found outcome. Authentication and authorisation are outside the Epic 1 scope. |
| Vendor Application | Accepts the query, requests the Vendor by VendorId and maps persisted Vendor state to Registered Vendor Details. |
| Vendor Repository | Retrieves the existing persisted Vendor aggregate by VendorId as the authoritative Epic 1 read source. |

No Registration UI, Registration Session, Address Service, Compliance Requirement Provider, Pending Activation Process, Identity capability, search infrastructure or event publisher participates in this sequence.

## 14.2 Retrieval Sequence

```mermaid
sequenceDiagram
    autonumber
    actor Administrator as Vendor Administrator
    participant Application as Vendor Application
    participant Repository as Vendor Repository

    Administrator->>Application: RetrieveRegisteredVendor(VendorId)
    Note over Administrator,Application: VendorId is the sole lookup input
    Application->>Repository: GetById(VendorId)

    alt Vendor found
        Repository-->>Application: Existing persisted Vendor
        Application->>Application: Map persisted Vendor state<br/>to Registered Vendor Details
        Application-->>Administrator: RegisteredVendorDetails
    else Vendor not found
        Repository-->>Application: Vendor not found
        Application-->>Administrator: Controlled Vendor Not Found outcome
    end

    Note over Administrator,Repository: No other bounded context participates
```

Registered Vendor Details contains the registration-established information defined by HJ-004 §1.7. The application returns that purpose-specific representation and never exposes the Vendor aggregate directly.

## 14.3 Query Behaviour

Retrieve Registered Vendor is a read-only Vendor query.

It does not mutate the Vendor, alter lifecycle state, change Trading Preference, publish events, create publication work, initiate Pending Activation, invoke Address or Compliance capabilities, perform search or require Identity collaboration.

Neither the successful outcome nor the controlled Vendor Not Found outcome creates a Domain Event, Integration Event or business state.

# 15. Epic 1 Implementation Boundary

## 15.1 Required

- complete, self-contained `RegisterVendor` request;
- authoritative server-side validation of HJ-104 rules;
- transient evaluation and disposal of Registration Declarations;
- explicit idempotency safeguard over the complete request;
- Address Service abstraction and Epic 1 stub implementation;
- approved Address Resolution reference in the request;
- direct retrieval of authoritative Address-owned values;
- Canonical Address Identifier and immutable Business Address Snapshot;
- applicable regulatory authorities;
- explicit aggregate creation invariants;
- Vendor creation in `PendingActivation` and `Offline`;
- atomic Vendor persistence and durable publication recording;
- distinct Domain and Integration Event representations;
- reliable `VendorRegistered` Integration Event publication;
- Pending Activation initiation without a synchronous Vendor Domain callback;
- Compliance Requirement Provider abstraction and Epic 1 stub implementation;
- retrieval of one Vendor using VendorId as the sole lookup criterion;
- repository loading from persisted Vendor state;
- mapping to Registered Vendor Details rather than direct aggregate exposure; and
- controlled Vendor Found and Vendor Not Found outcomes without side effects.

## 15.2 Outside the Vendor Registration Service

- Registration Session creation, retrieval, reconciliation, storage, update, expiry or disposal;
- incomplete interaction-state persistence;
- client workflow implementation; and
- permitted audit retention of Registration Declarations.

## 15.3 Deferred

- persisted registration drafts;
- Branches and multiple premises;
- registration amendment;
- full external Address provider integration;
- full Compliance Domain implementation;
- evidence workflows;
- completion of the Pending Activation Process;
- Vendor Activation;
- Menu and Ordering behaviour; and
- Operational Availability composition.

# 16. Design Decisions

## Decision 1: Registration Session is outside the service boundary

Any Registration Session is optional, client/BFF-owned interaction state. It is unknown to the Vendor Registration service and has no Vendor business lifecycle.

## Decision 2: One complete request starts Vendor Registration

The service performs no server-side draft or session interaction. Registration begins with one complete `RegisterVendor` request.

## Decision 3: Request and Address authority are distinct

The request is authoritative for client-authored information and the approved Address Resolution reference. The Address Domain is authoritative for canonical identity, snapshot and regulatory-authority values.

## Decision 4: Declarations are transient decision inputs

Declarations are validated but never become aggregate state, Domain Event content or Integration Event content.

## Decision 5: Vendor creation is the business transaction boundary

Only complete, valid input that satisfies every aggregate invariant creates a Vendor and completed business fact.

## Decision 6: Registration and activation remain separate

Registration creates a Vendor in `PendingActivation` and `Offline`; it neither authorises trading nor completes compliance.

## Decision 7: Domain and Integration Events are distinct

The aggregate records an internal completed fact. Reliable publication produces the external collaboration contract; their representations need not be identical.

## Decision 8: Persistence and publication recording are atomic

Successful registration cannot be confirmed unless the Vendor and durable publication work have committed together.

## Decision 9: RegisterVendor has explicit idempotency

Reprocessing the same successful business request returns the original outcome without repeating any business effect. Reuse of the same identity with registration information that is not semantically identical returns a controlled idempotency-conflict outcome without creating or modifying business state.

## Decision 10: Downstream capabilities consume the published contract

The Pending Activation Process begins from the Integration Event and does not query back into the Vendor Domain for the event’s required registration information.

## Decision 11: Registered Vendor retrieval is isolated and read-only

`RetrieveRegisteredVendor` loads one persisted Vendor by VendorId and maps its state to Registered Vendor Details. It exposes no aggregate, introduces no cross-domain dependency or dedicated read model, and creates no state change, lifecycle transition, event or publication work.

# 17. Implementation Checklist

Before Epic 1 Vendor Registration is considered complete, confirm that:

- [ ] Registration Session state is never created, retrieved or managed by the Vendor Registration service;
- [ ] `RegisterVendor` receives one complete, self-contained request;
- [ ] the request contains an idempotency identity or an approved equivalent uniqueness safeguard exists;
- [ ] duplicate and concurrent requests converge on one successful outcome;
- [ ] successful replay applies only to the same identity and semantically identical successful request;
- [ ] reuse of the same identity with registration information that is not semantically identical returns a controlled idempotency-conflict outcome;
- [ ] idempotency conflict creates or modifies no Vendor and records or publishes no business fact, Domain Event, publication work, Transactional Outbox entry or Integration Event;
- [ ] idempotency conflict initiates no Pending Activation Process and leaves previously committed Vendor state unchanged;
- [ ] all HJ-104 field and conditional rules are validated server-side;
- [ ] all mandatory Registration Declarations are explicitly accepted;
- [ ] Registration Declarations are not persisted or included in either event type;
- [ ] the request carries an approved Address Resolution reference;
- [ ] the Application retrieves authoritative Address-owned values directly from the Address Service;
- [ ] client-authored snapshot or authority values are rejected or ignored;
- [ ] the Canonical Address Identifier and immutable snapshot are persisted together;
- [ ] every HJ-004 creation invariant is enforced;
- [ ] validation or creation failure persists no Vendor and records no event or publication work;
- [ ] a new Vendor begins in `PendingActivation` and `Offline`;
- [ ] successful creation records the internal `VendorRegistered` Domain Event;
- [ ] Vendor persistence and Integration Event publication recording are atomic;
- [ ] the published `VendorRegistered` Integration Event satisfies the minimum contract;
- [ ] publication retry does not repeat Vendor Registration;
- [ ] the synchronous response reflects committed Vendor state;
- [ ] the Pending Activation Process uses the Integration Event without a synchronous Vendor Domain callback;
- [ ] `RetrieveRegisteredVendor` accepts VendorId as its only lookup input;
- [ ] the Vendor Repository is the authoritative persisted read source;
- [ ] a found Vendor is mapped to Registered Vendor Details without exposing the aggregate;
- [ ] Registered Vendor Details includes the HJ-004 §1.7 content and excludes declarations, compliance, event, Identity and persistence internals;
- [ ] an unknown VendorId produces the controlled Vendor Not Found outcome;
- [ ] retrieval invokes no Address, Compliance or Identity capability;
- [ ] retrieval changes no state and creates no Domain Event, Integration Event or publication work; and
- [ ] no search, additional query infrastructure or other deferred capability has been introduced.

# 18. Traceability

| Behaviour | Authoritative source |
|---|---|
| Registration Session ownership and service boundary | ADR-004; HJ-003 §3.5; HJ-004 §1.2 |
| Complete request and field rules | HJ-104 §§2, 5 and 6 |
| Information classifications | ADR-005; HJ-104 §§2 and 5.5 |
| Address Resolution reference and trust boundary | ADR-006 §2; HJ-104 §§5.4 and 6; HJ-004 §§1.5 and 8 |
| Vendor creation and invariants | HJ-004 §§1.3, 2 and 8 |
| Initial Vendor state | ADR-004; ADR-007; HJ-004 §§8 and 13 |
| Registration Declaration lifecycle | HJ-104 §§2 and 5.5; HJ-004 §§1.3 and 8 |
| Domain and Integration Event separation | HJ-004 §7; ADR-008 §2.5 |
| Minimum Integration Event contract | HJ-004 §7.2 |
| Atomic publication recording and retry | ADR-008 §2.6 |
| Register Vendor idempotency and complete-request boundary | ADR-008 §§2.2 and 2.4 |
| Controlled idempotency conflict | ADR-008 §2.3 |
| Event-driven Pending Activation initiation | ADR-003; ADR-007; HJ-004 §§7 and 12 |
| Retrieve Registered Vendor terminology and actor | HJ-003 §§3.20–3.22 |
| Query definition, result content and read source | HJ-004 §1.7; CR-026 §4.2 |
| Successful and Vendor Not Found retrieval behaviour | HJ-004 §1.7; CR-026 §4.3 |
| Retrieval side-effect and bounded-context constraints | HJ-004 §1.7; CR-026 §§4.2, 4.3 and 5 |

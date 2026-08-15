# CR-035 – Remove Delivery-Slice Scope from Vendor Registration Behavioural Model

## 1. Change Summary

Recast **HJ-105 – Vendor Registration Sequence Diagram** as the enduring behavioural model for Vendor Registration and Registered Vendor retrieval.

Remove Epic-specific implementation scope, delivery substitutions and implementation-completion criteria from HJ-105.

HJ-105 shall describe:

- enduring Vendor Registration runtime behaviour;
- enduring client/service boundaries;
- authoritative Address collaboration;
- Vendor creation and persistence;
- Domain and Integration Event behaviour;
- reliable publication;
- target downstream Pending Activation and Compliance collaboration; and
- Registered Vendor retrieval behaviour.

**HJ-011 – Epic 1 Vendor Registration Implementation Scope** remains the sole authoritative architectural source for the capabilities implemented, excluded or stubbed for Epic 1.

This Change Request does not alter the Vendor Registration business behaviour or target event-driven architecture.

## 2. Reason for Change

HJ-105 v3.3 was aligned directly with the Epic 1 implementation boundary and therefore currently contains delivery-specific decisions including:

- Web-client-only Registration Session ownership;
- exclusion of a BFF implementation;
- substitution of the target downstream Pending Activation and Compliance sequence with a Compliance Event Consumer Stub;
- an explicit Epic 1 Implementation Boundary;
- Epic-specific deferred capability lists; and
- an Epic 1 implementation-completion checklist.

These statements duplicate implementation scope now owned by HJ-011 and cause an enduring behavioural artefact to change whenever delivery sequencing changes.

Following CR-034, project-wide domain, information-contract and architectural artefacts describe enduring behaviour independently of delivery scope.

HJ-105 must follow the same rule.

## 3. General Transformation Rule

Perform a complete audit of normative HJ-105 wording for delivery-slice terminology and implementation selections, including but not limited to:

- `Epic 1`;
- `for Epic 1`;
- `within Epic 1`;
- `Epic 1 implementation`;
- `Epic 1 implementation boundary`;
- `deferred`;
- `Compliance Event Consumer Stub`;
- `Vendor Web Client` where it represents an Epic-specific implementation selection;
- `No BFF is implemented`;
- `real Event Bus / Message Broker` where used as an Epic-specific completion requirement; and
- implementation-completion criteria derived from HJ-011.

For every such normative occurrence:

1. **Remove it** where it exists only to describe delivery scope;
2. **Generalise it** to the enduring behavioural rule where one exists; or
3. **Restore the target architectural interaction** where an Epic-specific stub or substitution has replaced it.

Historical Revision History entries and Change Request provenance may retain Epic-specific wording where they accurately record document history.

Do not convert temporary implementation restrictions into permanent behavioural rules.

## 4. Required Changes

### 4.1 Document Control

Update HJ-105 as follows:

- increment Version from **3.3** to **3.4**;
- update **Last Updated** to **13 August 2026**;
- add a Revision History entry identifying CR-035 and the removal of delivery-slice scope from the behavioural model; and
- update Related Documents and traceability as required by this Change Request.

Historical Revision History entries shall remain unchanged.

### 4.2 Related Documents

Remove HJ-011 as an authoritative behavioural source for HJ-105.

HJ-011 may remain outside HJ-105 as the implementation-scope overlay applied when a delivery slice is implemented, but it shall not determine the behavioural sequence represented by HJ-105.

Add CR-035 to document traceability in accordance with the existing HJ-105 document-control convention.

Do not rewrite historic CR-026, CR-029 or CR-033 provenance merely because their titles or historical purpose refer to Epic 1.

### 4.3 Section 1 – Purpose and Authority

Replace the Epic-specific purpose statement with an enduring statement equivalent to:

> This document defines the runtime interactions required for Vendor Registration, including the associated retrieval of a registered Vendor.

Remove HJ-011 from the list of authoritative sources applied by HJ-105.

Remove statements equivalent to:

> HJ-011 governs the Epic 1 implementation boundary.

Retain the enduring authority hierarchy:

- HJ-003 for business terminology;
- HJ-104 for registration information, classifications, validation and information lifecycle;
- HJ-004 for aggregate behaviour, creation invariants, resulting state and events; and
- accepted ADRs for architectural decisions and capability boundaries.

HJ-105 shall describe interaction behaviour and shall not determine delivery scope.

### 4.4 Section 2 – Scope and Boundary

Recast the section around the **behaviour represented by HJ-105**, not the capabilities implemented in a particular Epic.

The section shall cover the enduring Vendor Registration behaviour including:

- client or BFF assembly of registration information;
- Address search and selection;
- submission of one complete `RegisterVendor` request;
- authoritative server-side validation;
- authoritative Address retrieval;
- Vendor creation and invariant enforcement;
- Vendor persistence;
- durable Integration Event recording;
- synchronous registration confirmation;
- reliable asynchronous `VendorRegistered` publication;
- downstream initiation of the Pending Activation Process through the published Integration Event;
- collaboration with the Compliance capability through its approved abstraction;
- relevant failure and idempotency behaviour;
- retrieval of a registered Vendor by VendorId;
- mapping to Registered Vendor Details; and
- controlled Vendor Not Found behaviour.

Remove the Epic-specific Compliance Event Consumer Stub from this section.

Remove implementation-scope exclusions such as:

- Pending Activation initiation being out of scope;
- Compliance Requirement Provider implementation being out of scope;
- real Compliance processing being deferred;
- BFF implementation exclusions; and
- other capability deferrals that exist only to describe the current delivery slice.

HJ-105 may continue to state enduring behavioural boundaries, for example that it does not define:

- Identity account creation or authentication;
- Compliance evidence workflows;
- Vendor Activation behaviour;
- unrelated Menu, Ordering, Payment or Delivery behaviour; or
- Vendor search where the defined retrieval operation is by VendorId only,

provided those statements express the concern boundary of HJ-105 rather than delivery sequencing.

### 4.5 Section 3 – Architectural Rules Applied

Remove Epic-specific behavioural applications.

#### Client Interaction Boundary

Replace Web-client-only ownership with the enduring architectural rule:

> A Registration Session, if used, is owned entirely by the client application or BFF and never participates in Vendor Registration service processing.

Do not select Web client or BFF within HJ-105.

#### Capability Autonomy

Replace the current Compliance Event Consumer Stub rule with the enduring target rule:

> The Pending Activation Process consumes the published `VendorRegistered` Integration Event and does not synchronously retrieve registration information from the Vendor Domain where that information is already present in the published contract.

Preserve Compliance as a downstream capability with its own behaviour and ownership.

#### Retrieval Authority

Remove the phrase:

> authoritative Epic 1 read source

Retain:

> the persisted Vendor aggregate is the authoritative read source.

No other enduring architectural rule shall depend upon an Epic-specific implementation selection.

### 4.6 Section 4 – Participants and Responsibilities

Replace:

> Vendor Web Client

with the enduring participant:

> Registration UI / BFF

or equivalent client abstraction consistent with the approved architectural terminology.

Its responsibility shall include:

- ownership of temporary interaction state;
- convenience validation;
- Address search and selection;
- assembly of the complete request; and
- presentation of the authoritative result.

Remove:

> Compliance Event Consumer Stub

from the enduring participant model.

Restore the enduring downstream participants:

| Participant | Responsibility |
| --- | --- |
| **Pending Activation Process** | Reacts to `VendorRegistered` and coordinates post-registration work required to move the Vendor towards an activation outcome. |
| **Compliance Requirement Provider / Compliance Capability** | Determines applicable Compliance Requirements through the approved Compliance boundary. |

Use the terminology established by the post-CR-034 HJ-004 and HJ-104 artefacts.

The participant description for Registration Session ownership shall state that temporary interaction state may be client- or BFF-owned but is unknown to the Vendor Registration Application.

Do not state that no BFF is implemented.

### 4.7 Section 5 – Complete RegisterVendor Request

Replace Web-client-specific wording with the enduring client boundary.

Use wording equivalent to:

> The Registration UI or BFF shall submit one complete, self-contained request containing...

Do not otherwise alter:

- request completeness;
- idempotency identity;
- Registered Information;
- Vendor Managed Information;
- Registration Declarations;
- Address Resolution reference; or
- Address-authority rules.

### 4.8 Section 6 – Client Assembly and Address Selection

Generalise the sequence from the Epic-specific Vendor Web Client to the enduring client/BFF interaction boundary.

Use participants equivalent to:

```text
Prospective Vendor
Registration UI / BFF
Client/BFF-owned interaction state
Address Service
```

Preserve:

- temporary Registration Session behaviour;
- Address search;
- candidate selection;
- Address Resolution reference acquisition;
- Registration Declaration confirmation; and
- complete request assembly.

The sequence shall continue to occur outside the Vendor Registration service boundary.

Remove all statements selecting the Web client specifically or stating that no BFF is implemented.

### 4.9 Section 7 – Successful Vendor Registration

Restore the enduring target downstream sequence after reliable publication.

Remove the Compliance Event Consumer Stub participant and its receipt/validation-only sequence.

Following successful Integration Event publication, represent the target behaviour equivalent to:

```text
Integration Event Publisher -> Event Bus:
    Publish VendorRegistered Integration Event

Event Bus -> Pending Activation Process:
    Deliver VendorRegistered Integration Event

Pending Activation Process -> Compliance Capability:
    Request applicable Compliance Requirements using Integration Event information

Compliance Capability -> Pending Activation Process:
    Applicable Compliance Requirements

Pending Activation Process -> Pending Activation Process:
    Begin Pending Activation coordination
```

The exact participant naming shall align with post-CR-034 HJ-004 and HJ-104 terminology.

The explanatory text shall state that:

- the synchronous registration response confirms committed Vendor state;
- downstream Pending Activation and Compliance collaboration proceeds asynchronously;
- the Pending Activation Process uses registration information supplied through the Integration Event; and
- the downstream process shall not synchronously call back into the Vendor Domain for registration information already supplied by the published contract.

Do not mention the Epic 1 Compliance Event Consumer Stub.

The fact that Epic 1 substitutes the target downstream sequence is defined only by HJ-011.

### 4.10 Section 8 – Validation and Creation Failure

Replace Vendor-Web-Client-specific participant terminology with the enduring client/BFF abstraction.

Preserve all enduring validation, Address failure, aggregate invariant and persistence behaviour.

Replace wording equivalent to:

> The Vendor Web Client alone decides whether to retain client interaction state

with:

> The client application or BFF alone decides whether to retain interaction state so the applicant can correct and resubmit information.

No other behavioural change is required.

### 4.11 Section 9 – Idempotent Replay and Concurrency

Replace the Epic-specific Vendor Web Client participant with the enduring client/BFF participant.

Continue to reflect the post-CR-034 ADR-008 idempotency model.

The sequence and explanatory text must prohibit duplicate:

- Vendor creation;
- completed business facts;
- Domain Events;
- outbox/publication records; and
- Integration Events.

Do not reintroduce Pending Activation Process initiation as part of the `RegisterVendor` idempotency side-effect list if ADR-008, following CR-034, no longer treats it as part of that idempotency boundary.

### 4.12 Section 10 – Persistence and Publication Reliability

Preserve the existing enduring reliability behaviour:

- atomic Vendor persistence and durable publication recording;
- Transactional Outbox or equivalent mechanism;
- asynchronous publication;
- retry following publication failure; and
- separation of Domain and Integration Event representations.

No Epic-specific implementation substitution belongs in this section.

### 4.13 Section 10.1 – Minimum VendorRegistered Integration Event Contract

Retain the enduring minimum `VendorRegistered` Integration Event contract.

Retain the requirement that the contract carries sufficient approved registration-time information for downstream Pending Activation and Compliance processing without requiring a synchronous Vendor Domain callback.

Remove the Epic-specific paragraph equivalent to:

> Within Epic 1, the Compliance Event Consumer Stub verifies that this contract can be received, deserialized and validated...

HJ-105 shall define the target contract semantics only.

Whether a delivery slice uses a stubbed subscriber belongs to HJ-011.

### 4.14 Section 11 – Registration Session Lifecycle

Rename the section from:

> Client-Owned Registration Session Lifecycle

to:

> Client / BFF-Owned Registration Session Lifecycle

or equivalent enduring terminology.

Replace:

> A Registration Session is Web-client-owned interaction state for Epic 1 and is not part of Vendor Registration processing. No BFF is implemented.

with:

> A Registration Session, if used, is client- or BFF-owned interaction state and is not part of Vendor Registration processing.

Generalise the sequence participant from Vendor Web Client to Registration UI / BFF.

Preserve:

- transient state;
- abandonment and expiry;
- disposal;
- non-resumability as Vendor business state; and
- absence of Vendor business effects.

### 4.15 Section 13 – Resulting Vendor State

Remove implementation-scope rows:

- Compliance Event Consumer Stub;
- Pending Activation Process – Not initiated in Epic 1; deferred;
- Compliance Requirements – Not determined in Epic 1; deferred.

Restore the enduring target behavioural outcomes.

Use wording consistent with post-CR-034 HJ-004 and HJ-104, including that:

- successful Vendor Registration creates the Vendor in `PendingActivation`;
- `VendorRegistered` is durably recorded and reliably published;
- the published event initiates the downstream Pending Activation collaboration; and
- applicable Compliance Requirements are obtained through the approved Compliance capability.

Do not make the asynchronous downstream outcome part of the synchronous registration success response.

### 4.16 Section 14 – Retrieve Registered Vendor

Remove all Epic-specific retrieval qualifiers.

#### Participants

Replace:

> Authentication and authorisation are outside the Epic 1 scope.

with an enduring behavioural boundary where appropriate, for example:

> Authentication and authorisation are outside the Retrieve Registered Vendor business behaviour defined by this sequence.

Replace:

> authoritative Epic 1 read source

with:

> authoritative persisted read source.

Remove the Compliance Event Consumer Stub from the list of participants that do not participate in retrieval.

Retain the enduring rule that retrieval invokes no:

- Address capability;
- Compliance capability;
- Pending Activation Process;
- event publisher; or
- other bounded context.

#### Query Behaviour

Preserve:

- VendorId as the lookup input;
- read-only behaviour;
- no Vendor mutation;
- no lifecycle change;
- no events;
- no publication work;
- no search; and
- controlled Vendor Not Found outcome.

### 4.17 Remove Section 15 – Epic 1 Implementation Boundary

Remove the entire section:

> Epic 1 Implementation Boundary

including:

- `Required`;
- `Outside the Vendor Registration Service`; and
- `Deferred`.

Do not rename this section or recreate the same implementation-scope classification under another title.

Any enduring service-boundary statement contained only within this section must first be relocated to the appropriate earlier behavioural section.

Examples include:

- Registration Session state remains outside the Vendor Registration service;
- the request is complete and self-contained;
- authoritative Address values are obtained through the Address boundary;
- retrieval is read-only.

Delivery-scope content including:

- Address stub implementation;
- Compliance Event Consumer Stub;
- real Event Bus implementation;
- deferred Pending Activation;
- deferred Compliance;
- BFF exclusion; and
- other implementation inclusion/exclusion lists

must not be relocated into HJ-105.

These remain solely within HJ-011.

### 4.18 Design Decisions

Renumber the section following removal of the Epic 1 Implementation Boundary where required by the document's heading convention.

#### Decision 1 – Registration Session is outside the service boundary

Replace the Epic-specific Web-client decision with the enduring architectural decision:

> Any Registration Session is optional client- or BFF-owned interaction state. It is unknown to the Vendor Registration service and has no Vendor business lifecycle.

#### Decisions 2–9

Retain their enduring behavioural meaning subject to terminology and post-CR-034 source alignment.

#### Decision 10

Replace:

> Epic 1 verifies the published downstream contract without implementing downstream business processing

with:

> Downstream capabilities consume the published contract

Use wording equivalent to:

> The Pending Activation Process begins from the published `VendorRegistered` Integration Event and does not synchronously query the Vendor Domain for registration information already present in that contract. Compliance collaboration occurs through the approved Compliance boundary.

Remove:

- Compliance Event Consumer Stub;
- Epic 1 qualification;
- delivery deferral language; and
- wording about narrowing the Epic 1 implementation boundary.

#### Decision 11

Retain Registered Vendor retrieval as isolated and read-only.

### 4.19 Replace the Epic 1 Implementation Checklist

The existing checklist is currently framed as:

> Before Epic 1 Vendor Registration is considered complete...

It shall no longer define delivery completion.

Either:

1. rename and recast it as an enduring **Behavioural Conformance Checklist**; or
2. remove it if all of its enduring requirements are already represented normatively elsewhere in HJ-105.

If retained, the checklist shall verify enduring behavioural conformance only.

It may include confirmation that:

- Registration Session state is never managed by the Vendor Registration service;
- the client/BFF submits one complete request;
- `RegisterVendor` is explicitly idempotent;
- HJ-104 rules are enforced server-side;
- Registration Declarations remain transient;
- authoritative Address values are obtained through the Address boundary;
- Vendor creation invariants are enforced;
- a new Vendor begins `PendingActivation` and `Offline`;
- Vendor persistence and publication recording are atomic;
- `VendorRegistered` satisfies the approved Integration Event contract;
- publication retry does not repeat Vendor Registration;
- the downstream Pending Activation Process consumes the published contract without synchronous Vendor callback;
- Registered Vendor retrieval remains read-only and isolated; and
- Vendor Not Found is a controlled outcome.

Remove checklist items requiring or prohibiting:

- a Web-client implementation specifically;
- BFF implementation;
- the Compliance Event Consumer Stub;
- a real Event Bus as an Epic-specific completion criterion;
- Epic-specific Pending Activation deferral;
- Epic-specific Compliance deferral; or
- any other delivery-slice implementation selection.

### 4.20 Traceability

Remove the following Epic-specific traceability rows:

- Epic 1 implementation boundary → HJ-011;
- Compliance Event Consumer Stub → HJ-011;
- Web-client-owned Registration Session for Epic 1 → HJ-011.

Retain project-wide traceability for:

- Registration Session ownership and service boundary;
- request and information rules;
- Address authority;
- Vendor creation and invariants;
- Vendor lifecycle;
- Domain and Integration Events;
- event publication reliability;
- idempotency;
- Pending Activation and Compliance collaboration;
- Registered Vendor retrieval.

Replace:

> Target event-driven Pending Activation and Compliance relationship beyond Epic 1

with:

> Event-driven Pending Activation and Compliance relationship

using the applicable enduring architectural sources.

HJ-011 shall not appear as an authority for HJ-105 behavioural rules.

## 5. Architectural Documentation Rule

Following CR-035:

- **HJ-105** defines enduring Vendor Registration and Registered Vendor retrieval interaction behaviour.
- **HJ-011** defines Epic 1 implementation scope and substitutions.

HJ-105 may therefore describe target runtime interactions involving capabilities that a particular delivery slice does not yet implement.

An implementation may substitute or omit those capabilities only where authorised by the applicable implementation-scope artefact.

The behavioural model itself shall not be rewritten merely because a delivery slice uses a stub or defers part of the target architecture.

## 6. Explicit Non-Changes

CR-035 does not change:

- the meaning of Vendor Registration;
- the complete `RegisterVendor` request boundary;
- Registration Declaration lifecycle;
- Address authority;
- Vendor aggregate creation invariants;
- creation of the Vendor in `PendingActivation`;
- initial Trading Preference `Offline`;
- Vendor persistence;
- Domain and Integration Event separation;
- the `VendorRegistered` Integration Event business contract;
- atomic persistence and publication recording;
- reliable publication;
- Register Vendor idempotency;
- controlled idempotency conflict;
- the target Pending Activation Process;
- Compliance ownership;
- event-driven downstream collaboration;
- `RetrieveRegisteredVendor`;
- VendorId retrieval semantics;
- Registered Vendor Details;
- controlled Vendor Not Found behaviour; or
- retrieval side-effect constraints.

CR-035 changes **where implementation scope is documented**, not the target system behaviour.

## 7. Impacted Artefact

| Artefact | Required Change |
| --- | --- |
| **HJ-105 – Vendor Registration Sequence Diagram** | Remove all normative delivery-slice scope and restore the enduring Vendor Registration behavioural model |

## 8. Acceptance Criteria

CR-035 is complete when:

### Purpose and Authority

- HJ-105 no longer defines itself as an Epic 1 behavioural artefact;
- HJ-011 is not an authoritative input to HJ-105 behavioural rules;
- HJ-105 describes Vendor Registration and registered Vendor retrieval independently of delivery sequencing.

### Client Boundary

- HJ-105 no longer selects the Vendor Web Client as the only implementation;
- client/BFF ownership of Registration Session state is restored as the enduring architectural boundary;
- HJ-105 contains no normative statement that a BFF is or is not implemented.

### Downstream Collaboration

- the Compliance Event Consumer Stub is absent from the enduring behavioural model;
- the successful target sequence shows `VendorRegistered` being consumed by the Pending Activation Process;
- Compliance Requirement determination occurs through the approved Compliance boundary;
- downstream processing does not synchronously retrieve registration information from the Vendor Domain where it is already present in the published contract.

### Delivery Scope

- the entire Epic 1 Implementation Boundary section has been removed;
- no equivalent In Scope / Out of Scope / Stubbed / Deferred delivery classification has been recreated elsewhere;
- implementation substitutions remain solely in HJ-011.

### Retrieval

- Registered Vendor retrieval contains no Epic-specific qualifiers;
- persisted Vendor state remains the authoritative retrieval source;
- retrieval remains isolated and read-only.

### Decisions and Verification

- Design Decision 1 expresses client/BFF ownership rather than the Epic-specific Web-client selection;
- Design Decision 10 expresses the enduring downstream event-consumption architecture;
- the Epic 1 implementation checklist has either been removed or converted to an enduring behavioural conformance checklist;
- no checklist item asserts an Epic-specific implementation selection.

### Traceability and Document Control

- Epic-specific HJ-011 traceability rows have been removed;
- enduring Pending Activation and Compliance traceability remains;
- HJ-105 is versioned to **3.4**;
- Last Updated is **13 August 2026**;
- Revision History records application of CR-035; and
- historical document provenance remains unchanged.

### Cross-Artefact

- HJ-105 is consistent with the post-CR-034 HJ-004, HJ-104 and ADR-008;
- HJ-105 contains no normative `Epic 1` implementation-scope decision outside historical provenance;
- no temporary implementation restriction has been converted into an unjustified permanent behavioural rule; and
- HJ-011 remains the sole authoritative architectural source for Epic 1 implementation scope.
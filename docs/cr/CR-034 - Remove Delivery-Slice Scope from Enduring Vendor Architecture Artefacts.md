# CR-034 – Remove Delivery-Slice Scope from Enduring Vendor Architecture Artefacts

## 1. Change Summary

Remove delivery-slice and Epic-specific implementation-scope language from:

- **HJ-004 – Vendor Domain Models**
- **HJ-104 – Vendor Registration Fields Matrix**
- **ADR-008 – Idempotent Operations and Reliable Event Publication**

These artefacts shall describe enduring HotJoes domain behaviour, information contracts and architectural decisions independently of delivery sequencing.

**HJ-011 – Epic 1 Vendor Registration Implementation Scope** is the sole authoritative architectural source for what is implemented, excluded or stubbed for Epic 1.

This Change Request does not alter enduring Vendor Registration, Pending Activation, Compliance, Address, idempotency or reliable-publication behaviour.

## 2. Reason for Change

Epic-specific implementation scope has become embedded in architectural artefacts whose intended responsibilities are project-wide and enduring.

This creates unnecessary coupling between:

- domain and contract definitions; and
- individual delivery slices.

As a result, changes to the Epic 1 implementation boundary require propagation through otherwise stable architectural documents.

HJ-011 now provides a dedicated authoritative implementation-scope artefact.

HJ-004, HJ-104 and ADR-008 must therefore be cleansed of normative delivery-slice scope so that they remain valid independently of which capability is implemented in a particular Epic.

## 3. General Transformation Rule

Perform a complete audit of normative wording in HJ-004 and HJ-104 for delivery-slice terminology, including but not limited to:

- `Epic 1`
- `first epic`
- `initial implementation`
- `initial implementation scope`
- `stubbed during Epic 1`
- `Epic 1 stub implementation`
- `deferred from Epic 1`
- `outside Epic 1`
- `future epic`

For every such normative occurrence:

1. **Remove it** where it exists only to describe delivery sequencing;
2. **Generalise it** where an enduring domain, contract or architectural rule exists beneath the delivery-specific wording; or
3. **Leave the delivery decision to HJ-011** where no project-wide rule exists.

Do not replace removed Epic-specific wording with alternative release-specific wording.

Historical Revision History entries, Change Request provenance and other documentary history may retain references to Epic 1 where they accurately describe the history of the artefact.

## 4. Required Changes

### 4.1 HJ-004 – Vendor Domain Models

Recast HJ-004 entirely as an enduring Vendor domain model.

Preserve all enduring definitions concerning:

- Vendor Registration;
- Vendor lifecycle;
- `PendingActivation`;
- Pending Activation Process;
- Vendor activation and deactivation;
- Registered Information;
- Vendor Managed Information;
- Address ownership and Business Address snapshots;
- Compliance ownership;
- Compliance Requirements;
- Domain and Integration Events;
- registered Vendor retrieval;
- aggregate behaviour and invariants; and
- cross-capability relationships.

#### Address References

Remove delivery-specific statements that the Address capability or Address Service is:

> stubbed during Epic 1

or equivalent.

Retain only enduring rules such as:

- Address is authoritative for canonical address identity and resolution;
- Vendor stores the approved Business Address snapshot and required Address-owned values;
- Vendor must interact with Address through the defined capability boundary.

The choice to use an Address stub in Epic 1 belongs only to HJ-011.

#### Registered Vendor Retrieval

Remove Epic-specific qualifiers from retrieval behaviour, persistence and read-source rules.

Where retrieval behaviour represents an approved enduring service/domain rule, express it without reference to Epic 1.

For example, retain enduring statements that:

- VendorId is the lookup identity where already approved;
- persisted Vendor state is authoritative;
- retrieval is read-only;
- Registered Vendor Details are mapped from Vendor state;
- controlled Vendor Not Found behaviour applies; and
- retrieval introduces no business side effects.

Do not describe those rules as temporary Epic 1 implementation decisions.

#### External Verification

Remove delivery statements such as:

> Companies House verification is outside Epic 1

or equivalent.

If authoritative Companies House verification is not part of the Vendor Registration contract, express that as an enduring boundary.

If its eventual architectural treatment is unresolved, do not invent a replacement rule; remove the release-specific statement and leave the matter unresolved or governed by the appropriate future architectural decision.

#### Address and Compliance Model Classification

Remove labels such as:

> Stubbed External Domain in Epic 1

from Address or Compliance elements.

Replace them with their enduring architectural classification, such as their bounded-context or external-capability role.

The implementation substitution belongs to HJ-011, not the domain model.

#### Pending Activation and Compliance

Preserve the target architecture in which:

- a newly registered Vendor is in `PendingActivation`;
- the Pending Activation Process coordinates post-registration activation work;
- Compliance owns Compliance Requirements and Compliance behaviour; and
- downstream collaboration occurs through explicit contracts.

Remove statements prescribing an Epic-specific Compliance stub, Compliance Requirement Provider stub or other delivery implementation.

#### Initial Implementation Scope

Remove the entire section currently titled:

> Initial Implementation Scope

where that section exists solely to define first-Epic implementation inclusion, exclusion, stubbing or deferral.

Do not selectively rewrite that section as another delivery-scope list.

Any enduring domain rules contained only within that section must first be relocated to their appropriate domain section before the implementation-scope section is removed.

After relocation, HJ-004 shall contain no normative section whose responsibility is to define which capabilities are implemented in a particular Epic.

### 4.2 HJ-104 – Vendor Registration Fields Matrix

Recast HJ-104 as the enduring **Vendor Registration information contract**.

#### Purpose

Remove the Epic-specific qualification from the Purpose statement.

Replace wording equivalent to:

> authoritative Vendor Registration Fields Matrix for Epic 1 – Vendor Registration

with:

> authoritative Vendor Registration Fields Matrix

or equivalent enduring wording.

HJ-104 shall remain authoritative for:

- registration information;
- classifications;
- required, optional and conditional inputs;
- validation;
- derived information;
- ownership;
- editability;
- lifecycle; and
- traceability.

#### Fields Deferred from Epic 1

Rename:

> Fields Deferred from Epic 1

to:

> Information Outside Vendor Registration

or equivalent enduring contract-boundary terminology.

Remove the `Future Epic` concept from this section.

The section shall state only that the listed information is not part of the Vendor Registration information contract.

Where useful, the second column may identify the owning or related capability rather than a future delivery slice.

For example:

| Information | Related / Owning Capability |
| --- | --- |
| Menu | Menu Management |
| Bank Details | Payments |
| Operational Availability | Trading Operations |

Do not make roadmap commitments in HJ-104.

#### Assumptions and Outstanding Decisions

Remove wording such as:

> The following assumptions remain for Epic 1.

Reframe the section around enduring assumptions and outstanding architectural or contract decisions only.

Audit every assumption and classify it as follows:

- retain if it is an enduring Vendor Registration rule;
- generalise if there is a valid enduring rule beneath the Epic wording;
- remove if it exists only because of the Epic 1 implementation slice.

Specifically:

- remove the Address Service **stub** selection and retain only the Address ownership/contract requirement;
- remove the Compliance Provider **stub** selection and retain only Compliance ownership of eventual Compliance Requirement determination;
- replace `Compliance evidence is outside Epic 1` with an enduring statement that Compliance evidence is outside the Vendor Registration information contract, if that remains architecturally correct;
- remove `during Epic 1` from the single-trading-location statement only if one trading location is an enduring Vendor Registration/domain rule;
- if one trading location is merely a delivery restriction, remove it from HJ-104 and leave the implementation restriction to HJ-011;
- remove `future epic` wording concerning multiple premises.

Do not invent a permanent one-location business rule merely to preserve existing Epic 1 scope.

#### Compliance Traceability

Retain the enduring relationship that Trading Characteristics and other approved registration information may be used by the Compliance capability to determine Compliance Requirements.

Do not specify an Epic-specific Compliance Provider or stub implementation.

#### Address Traceability

Retain Address ownership and the approved Address contract.

Do not specify an Epic-specific Address stub.

### 4.3 ADR-008 – Idempotent Operations and Reliable Event Publication

Preserve the architectural decisions governing:

- Register Vendor idempotency;
- explicit idempotency identity or equivalent uniqueness constraint;
- semantically identical replay;
- semantically different idempotency conflict;
- concurrency;
- Domain and Integration Event separation;
- atomic persistence and publication recording;
- Transactional Outbox or equivalent reliability mechanism; and
- reliable publication retry.

#### Register Vendor Replay

For replay of a previously successful semantically identical `RegisterVendor` request, retain requirements to:

- return the original successful outcome;
- create no additional Vendor;
- record no additional `VendorRegistered` Domain Event or completed business fact;
- create no additional publication or outbox record; and
- publish no additional `VendorRegistered` Integration Event.

Remove:

> initiate no additional Pending Activation Process

or equivalent.

ADR-008 shall describe effects directly governed by the idempotency boundary and shall not depend upon a particular delivery slice's downstream implementation.

#### Idempotency Conflict

Remove any statement that conflict processing:

> initiates no Pending Activation Process

or equivalent.

Retain requirements that conflict processing:

- creates no Vendor;
- modifies no existing Vendor;
- records no completed business fact or Domain Event;
- creates no publication or outbox record;
- publishes no Integration Event; and
- leaves previously committed Vendor state unchanged.

#### Testing and Enforcement

Remove testing or enforcement obligations specifically requiring:

- no additional Pending Activation Process; or
- no duplicate Pending Activation Process.

Retain or replace them with verification that replay, concurrency and conflict processing do not produce:

- an additional Vendor;
- an additional `VendorRegistered` Domain Event;
- an additional publication/outbox record; or
- an additional `VendorRegistered` Integration Event.

No idempotency, concurrency, atomicity or reliable-publication guarantee shall be weakened.

## 5. Architectural Documentation Rule

Following this change:

- **HJ-004** defines enduring Vendor domain structure and behaviour.
- **HJ-104** defines the enduring Vendor Registration information contract.
- **ADR-008** defines enduring idempotency and reliable-publication architecture.
- **HJ-011** alone defines Epic 1 architectural implementation scope.

Enduring artefacts may describe target capabilities and interactions irrespective of whether those capabilities are implemented in the current delivery slice.

They must not use delivery sequencing to redefine domain truth, contract semantics or architectural ownership.

## 6. Document Control

For each artefact changed by CR-034:

- increment the document version in accordance with the existing versioning convention;
- update **Last Updated** to the date CR-034 is applied;
- add a Revision History entry identifying CR-034 and summarising the removal of delivery-slice scope;
- add CR-034 to Related Documents or equivalent traceability where that artefact's established document convention requires it.

Historical Revision History entries must not be rewritten merely because they contain historic Epic references.

## 7. Explicit Non-Changes

This Change Request does not change:

- `PendingActivation` as the initial Vendor lifecycle state;
- the target Pending Activation Process;
- Compliance as a separate bounded context;
- Compliance ownership of Compliance Requirements;
- eventual Compliance Requirement determination;
- Address ownership or authoritative Address behaviour;
- Business Address snapshot ownership;
- Vendor Registration field semantics;
- the target event-driven downstream architecture;
- ADR-003 – Event-Driven Collaboration;
- ADR-007 – Vendor Compliance as a Separate Bounded Context;
- the internal `VendorRegistered` Domain Event;
- the external `VendorRegistered` Integration Event;
- the minimum Integration Event business content;
- Register Vendor idempotency;
- atomic persistence and publication recording; or
- reliable Integration Event publication.

This Change Request also does not transfer the Epic 1 implementation-scope content removed from HJ-004 or HJ-104 into another enduring artefact. HJ-011 already owns that concern.

## 8. Impacted Artefacts

| Artefact | Required Change |
| --- | --- |
| **HJ-004 – Vendor Domain Models** | Remove all normative Epic/initial-implementation scope, relocate any enduring rules, and remove the Initial Implementation Scope section |
| **HJ-104 – Vendor Registration Fields Matrix** | Recast as an enduring Vendor Registration information contract and remove delivery-deferral/stub language |
| **ADR-008 – Idempotent Operations and Reliable Event Publication** | Remove Pending Activation from Register Vendor idempotency side-effect and testing semantics |

## 9. Acceptance Criteria

CR-034 is complete when:

### HJ-004

- no normative `Epic 1`, `first epic`, `initial implementation`, `stubbed during Epic 1`, `Epic 1 stub`, `deferred from Epic 1` or equivalent delivery-scope statement remains;
- historical revision/provenance references may remain;
- the Initial Implementation Scope section has been removed;
- any enduring domain rules previously located only in that section have been relocated appropriately;
- Address and Compliance are described by enduring ownership and capability boundaries rather than Epic-specific stub classifications;
- retrieval semantics are expressed as enduring behaviour rather than Epic-specific implementation choices;
- the target Pending Activation and Compliance model remains intact.

### HJ-104

- the Purpose no longer defines HJ-104 specifically for Epic 1;
- `Fields Deferred from Epic 1` has been replaced by an enduring Vendor Registration contract-boundary section;
- no `Future Epic` classification remains in normative field-scope definitions;
- the assumptions section is no longer framed as Epic 1 assumptions;
- no Address Service stub selection remains;
- no Compliance Provider stub selection remains;
- no delivery-specific one-location or multiple-premises statement remains unless converted to a separately justified enduring business rule;
- Compliance evidence and other exclusions are expressed as Vendor Registration contract boundaries rather than delivery deferrals.

### ADR-008

- replay and conflict behaviour contain no implication that Register Vendor initiates a Pending Activation Process;
- testing obligations contain no duplicate-Pending-Activation assertion;
- duplicate Vendor, Domain Event, publication work and Integration Event prevention remains intact;
- all idempotency and reliable-publication guarantees remain unchanged.

### Cross-Artefact

- HJ-004, HJ-104 and ADR-008 contain no normative Epic 1 implementation-scope decisions;
- HJ-011 remains the sole authoritative architectural source for Epic 1 implementation scope; and
- no removed delivery-slice statement has been silently converted into an unjustified permanent domain or contract rule.
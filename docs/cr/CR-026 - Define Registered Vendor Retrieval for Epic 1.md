# CR-026 – Define Registered Vendor Retrieval for Epic 1

| Metadata | Value |
|---|---|
| **Change Request ID** | CR-026 |
| **Title** | Define Registered Vendor Retrieval for Epic 1 |
| **Status** | Approved |
| **Owner** | Project Architecture |
| **Priority** | Important |
| **Affected Documents** | HJ-003 – Ubiquitous Language Guide; HJ-004 – Vendor Domain Models; HJ-105 – Vendor Registration Sequence Diagram |

---

# 1. Background

Epic 1 includes retrieval of a registered Vendor within the initial implementation boundary, but the authoritative upstream artefacts do not yet define the retrieval capability sufficiently for HJ-106 – Vendor Registration Service Contract to derive a complete query contract.

The current derived HJ-106 correctly excludes registered-Vendor retrieval rather than inventing:

- a query operation;
- a response projection;
- authorisation behaviour;
- search behaviour;
- cross-domain dependencies; or
- failure semantics.

For Epic 1, the retrieval requirement is intentionally narrow.

A **Vendor Administrator** retrieves an existing Vendor using its **VendorId** and receives the Vendor details established through Vendor Registration.

The capability shall remain entirely within the Vendor boundary.

It shall not introduce:

- search;
- authentication or authorisation behaviour;
- Identity Domain collaboration;
- Address Domain callbacks;
- Compliance Domain callbacks;
- Compliance state;
- additional lifecycle behaviour;
- event publication; or
- a dedicated read-model infrastructure.

The persisted Vendor aggregate remains the authoritative read source, but the aggregate itself shall not be exposed directly as the external query result.

This Change Request updates the authoritative upstream artefacts so that HJ-106 and HJ-107 can subsequently be regenerated rather than manually patched.

---

# 2. Objectives

The revised artefacts shall:

- define a minimal **Retrieve Registered Vendor** query capability for Epic 1;
- establish **Vendor Administrator** as the actor performing the query;
- use **VendorId** as the sole lookup criterion;
- return the Vendor information established through Vendor Registration;
- define a purpose-specific **Registered Vendor Details** result;
- use persisted Vendor state as the authoritative source;
- prevent direct exposure of the Vendor aggregate as a service contract;
- define a controlled Vendor-not-found outcome;
- introduce no query side effects;
- introduce no cross-domain dependencies; and
- provide sufficient authoritative input for regeneration of HJ-106 and HJ-107.

---

# 3. Scope

This Change Request introduces one read-only Vendor capability:

```text
RetrieveRegisteredVendor(VendorId)
```

The capability:

- retrieves one Vendor by its VendorId;
- returns the Vendor's persisted registration details;
- performs no search;
- performs no authentication or authorisation;
- performs no state change;
- creates no Domain Event;
- publishes no Integration Event.

The query is intended solely to complete the Epic 1 vertical slice.

---

# 4. Required Changes

## 4.1 HJ-003 – Ubiquitous Language Guide

Add the following terms to the appropriate terminology section and Glossary.

### Vendor Administrator

Define:

> **Vendor Administrator** is the trusted Epic 1 administrative actor that may retrieve an existing Vendor directly using its VendorId.
>
> Authentication and authorisation of the Vendor Administrator are outside the scope of Epic 1.

The term shall not imply an Identity Domain dependency within Epic 1.

---

### Retrieve Registered Vendor

Define:

> **Retrieve Registered Vendor** is a read-only Vendor query that retrieves an existing Vendor using its VendorId and returns the Vendor details established through Vendor Registration.
>
> The query does not modify Vendor state and produces no Domain Event or Integration Event.

---

### Registered Vendor Details

Define:

> **Registered Vendor Details** is the read representation returned by Retrieve Registered Vendor.
>
> It is derived from the persisted Vendor state and contains the Vendor information established through successful Vendor Registration.
>
> It is a service/query representation and is not the Vendor aggregate itself.

Update the Glossary with corresponding entries for:

- Vendor Administrator;
- Retrieve Registered Vendor; and
- Registered Vendor Details.

---

## 4.2 HJ-004 – Vendor Domain Models

Introduce the Retrieve Registered Vendor capability as a read-only Vendor query.

### Query Definition

Add:

```text
RetrieveRegisteredVendor(VendorId)
```

Define the query as follows:

> Retrieve Registered Vendor loads an existing Vendor from the Vendor Repository using VendorId and returns a Registered Vendor Details representation derived from persisted Vendor state.

The query shall:

- use VendorId as the sole lookup criterion;
- require no Registration Session;
- require no search operation;
- require no Identity collaboration;
- require no Address Service collaboration;
- require no Compliance collaboration;
- make no change to the Vendor aggregate;
- cause no lifecycle transition;
- record no Domain Event;
- publish no Integration Event.

---

### Read Source

State explicitly:

> The persisted Vendor aggregate is the authoritative read source for Retrieve Registered Vendor during Epic 1.

No dedicated read model, projection store or eventually consistent query infrastructure is required for Epic 1.

This does not prevent a later architecture from introducing an independently optimised read model if justified by future requirements.

---

### Service Representation Boundary

Add the following rule:

> The Vendor aggregate shall not be exposed directly as the query or service response.
>
> The application shall map persisted Vendor state into a purpose-specific Registered Vendor Details representation.

This preserves the distinction between the Domain model and externally consumed service contracts.

---

### Registered Vendor Details Content

Define Registered Vendor Details as containing the persisted Vendor information established through registration.

At minimum include:

- VendorId;
- RegisteredAt;
- Vendor State;
- Trading Preference;
- Legal Operator Type;
- Legal Operator Name;
- Company Registration Number, where applicable;
- Trading Name;
- Trading Characteristics:
  - Trading Location;
  - Opening Hours;
  - Service Includes Hot Food;
  - Alcohol Service;
- Contact Name;
- Contact Email;
- Contact Telephone;
- Canonical Address Identifier;
- immutable Business Address Snapshot;
- Food Registration Authority;
- Primary Trading Authority, where applicable;
- Website, where supplied;
- Business Description, where supplied.

Explicitly exclude:

- Registration Declarations;
- Compliance Requirements;
- Compliance evidence;
- Compliance state or decisions;
- Activation decisions;
- Domain Event representations;
- Integration Event representations;
- outbox or publication metadata;
- Identity information;
- internal persistence metadata.

---

### Query Outcome

Define two business outcomes.

#### Vendor Found

When VendorId identifies an existing Vendor:

> Return Registered Vendor Details derived from the persisted Vendor state.

#### Vendor Not Found

When VendorId does not identify an existing Vendor:

> Return a controlled Vendor Not Found outcome.

No additional business failure semantics are required for Epic 1.

---

### Query Side Effects

Add an explicit invariant:

> Retrieve Registered Vendor is read-only.
>
> Execution shall not:
>
> - modify the Vendor aggregate;
> - change Vendor State;
> - change Trading Preference;
> - create or modify Registered Information;
> - create or modify Vendor Managed Information;
> - record a Domain Event;
> - create publication work;
> - publish an Integration Event;
> - initiate a Pending Activation Process.

---

### Initial Implementation Scope

Ensure the initial Epic 1 implementation scope explicitly includes:

- retrieval of a Vendor by VendorId; and
- mapping of persisted Vendor state to Registered Vendor Details.

No additional query infrastructure shall be required by this change.

---

## 4.3 HJ-105 – Vendor Registration Sequence Diagram

Add a new section describing **Retrieve Registered Vendor**.

The section shall remain separate from the registration command flow.

### Participants

The retrieval sequence shall contain only the participants required by the Vendor capability:

- Vendor Administrator;
- Vendor Application;
- Vendor Repository.

The Vendor aggregate may be represented where useful to show the authoritative persisted source.

Do not introduce:

- Registration UI;
- Registration Session;
- Address Service;
- Compliance Requirement Provider;
- Pending Activation Process;
- Identity capability;
- search infrastructure;
- event publisher.

---

### Retrieval Sequence

Model the successful flow substantially as follows:

```text
Vendor Administrator
        |
        | RetrieveRegisteredVendor(VendorId)
        v
Vendor Application
        |
        | GetById(VendorId)
        v
Vendor Repository
        |
        | Existing Vendor
        v
Vendor Application
        |
        | Map persisted Vendor state
        v
Vendor Administrator
        |
        | RegisteredVendorDetails
```

The sequence shall make explicit that:

- VendorId is the only lookup input;
- the Vendor Repository retrieves the existing Vendor;
- the application maps the persisted Vendor state into Registered Vendor Details;
- no other bounded context participates.

---

### Vendor Not Found

Add the alternative flow:

```text
Vendor Repository -> Vendor Application: Vendor not found
Vendor Application -> Vendor Administrator: Controlled Vendor Not Found outcome
```

No Domain Event, Integration Event or business state shall be created.

---

### Query Behaviour Note

Add an explicit note:

> Retrieve Registered Vendor is a read-only Vendor query.
>
> It does not mutate the Vendor, alter lifecycle state, publish events, invoke Address or Compliance capabilities, perform search, or require Identity collaboration.

---

# 5. Explicit Epic 1 Constraints

The retrieval capability introduced by this Change Request intentionally does **not** include:

- Vendor search;
- filtering;
- paging;
- lookup by Trading Name;
- lookup by Legal Operator Name;
- authentication;
- authorisation;
- caller-to-Vendor ownership checks;
- Identity Domain integration;
- Address Domain re-resolution;
- Address search;
- Compliance state retrieval;
- Compliance Requirement retrieval;
- Activation status beyond the Vendor State already stored by the Vendor;
- multiple-Vendor retrieval;
- dedicated read-model infrastructure;
- eventual-consistency behaviour;
- post-registration editing.

These are outside the scope of Epic 1.

---

# 6. Architectural Principles

- Preserve Vendor capability ownership.
- Do not expose the aggregate directly as a service contract.
- Use persisted Vendor state as the authoritative Epic 1 read source.
- Keep the query side-effect free.
- Introduce no new bounded-context dependency.
- Do not introduce infrastructure not required by the business capability.
- Preserve the existing distinction between Registered Information, Vendor Managed Information and transient Registration Declarations.
- Preserve Address ownership by returning the stored registration-time Address information rather than re-resolving it.
- Preserve Registration and Activation separation.

---

# 7. Relationship to Derived Artefacts

HJ-106 – Vendor Registration Service Contract and HJ-107 – Vendor Registration Test Catalogue are derived artefacts and are not targets of this Change Request.

After this Change Request has been applied to the authoritative upstream artefacts:

1. Regenerate HJ-106 using the approved AI Service Contract prompt.
2. Verify that HJ-106 derives Retrieve Registered Vendor without introducing unsupported search, authorisation or cross-domain behaviour.
3. Independently review HJ-106.
4. Regenerate HJ-107 from the reviewed HJ-106 using the approved AI Test Catalogue prompt.
5. Verify that the previously blocked registered-Vendor retrieval test area is replaced by explicit query test obligations.

No manual patching of the current draft HJ-106 or HJ-107 is required.

---

# 8. Expected Outcome

Following this change:

- Epic 1 has a fully defined minimal Registered Vendor retrieval capability.
- Vendor Administrator can retrieve one Vendor using VendorId.
- VendorId is the sole lookup criterion.
- The Vendor Repository and persisted Vendor state remain the authoritative Epic 1 read source.
- The service returns Registered Vendor Details rather than exposing the Vendor aggregate directly.
- All Vendor details established during successful registration are returned where applicable.
- Registration Declarations and unrelated Domain information remain excluded.
- Retrieval requires no search, authentication, authorisation, Identity, Address or Compliance collaboration.
- Retrieval causes no state change, lifecycle transition, Domain Event, Integration Event or Pending Activation behaviour.
- HJ-003, HJ-004 and HJ-105 provide sufficient authoritative information for HJ-106 and HJ-107 to be regenerated consistently.
- The existing Epic 1 retrieval completeness gap is closed without expanding the scope beyond the Vendor capability.

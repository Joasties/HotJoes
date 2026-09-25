# CR-077 - Amend PR-004 for Required Licence Types Determination

| Property | Value |
|---|---|
| Change Request | CR-077 |
| Status | Approved |
| Date | 19 September 2026 |
| Owner | Project Architecture |
| Affected artefact | PR-004 Generate Test Catalogue from Approved Service Contract |

## 1. Reason

Approved HJ-106 v2.1 adds `DetermineRequiredLicenceTypes` as the third Epic 1 business operation alongside `RegisterVendor` and `RetrieveRegisteredVendor`.

PR-004 already requires every normative rule in the current Approved HJ-106 Part A contract to receive catalogue coverage, be identified as non-testable or be recorded as blocked. However, its operation scope explicitly permits derivation only for `RegisterVendor` and `RetrieveRegisteredVendor`, and its required HJ-107 structure and review checklist contain no place for the new determination operation.

Running PR-004 unchanged would therefore create a direct contradiction: including determination obligations would breach its explicit operation scope, while omitting them would breach its completeness rule and weaken the approved HJ-106 contract.

PR-004 must be amended before HJ-107 is regenerated.

## 2. Decision Requested

Approve the amendment of PR-004 so that controlled HJ-107 generation and regeneration includes all three currently approved Epic 1 business operations:

- `DetermineRequiredLicenceTypes`;
- `RegisterVendor`; and
- `RetrieveRegisteredVendor`.

`DetermineRequiredLicenceTypes` shall be treated as a separate synchronous, side-effect-free operation derived from HJ-106 Part A. Its inclusion does not make determination state part of `RegisterVendor`, Vendor state, persistence, semantic fingerprinting, retrieval or `VendorRegistered`.

## 3. Controlled Amendments to PR-004

### 3.1 Operation scope

Amend the Scope, Test Basis, Review Standard and every equivalent operation-limiting statement so that PR-004 derives behavioural obligations for `DetermineRequiredLicenceTypes`, `RegisterVendor` and `RetrieveRegisteredVendor`.

Retain `RegisterVendor` as the primary Vendor creation operation. Do not infer any additional Vendor query, search, Compliance Requirement, Licence Details or regulatory-validation operation.

### 3.2 Required determination coverage

Add explicit required coverage for:

- the exact six caller-supplied controlling inputs: Legal Operator Type, Trading Location, complete Weekly Opening Hours, Service Includes Hot Food, Alcohol Service and Address Resolution Reference;
- authoritative Address resolution and the prohibition on caller-authored Address or authority values becoming Compliance input;
- Vendor Application validation and orchestration without ownership or duplication of applicability policy;
- the Compliance-owned active Rule Set Version;
- deterministic evaluation of the complete applicability matrix;
- Food Business Registration always required;
- Street Trading Licence required for Stall using authoritative Primary Trading Authority context;
- Late Night Refreshment Licence required when hot-food service overlaps any portion of 23:00 inclusive to 05:00 exclusive;
- the 23:00 and 05:00 boundaries, Closed, Open All Day, ordinary same-day intervals and overnight intervals;
- Premises Licence and Personal Licence Holder required exactly when Alcohol Service is true;
- exactly five duplicate-free Compliance Determination Items in canonical order, including explicit `false` results;
- unsupported coverage failing closed;
- transient Registration Session retention of the result, Rule Set Version and controlling-input fingerprint;
- invalidation when any controlling input changes;
- fresh determination immediately before Web submission;
- renewed review when the item set or Rule Set Version changes;
- retained draft and prevented Web submission following a controlled determination failure;
- direct `RegisterVendor` independence from determination state and Compliance availability;
- absence of determination state from `RegisterVendor`, Vendor state, persistence, semantic fingerprinting, replay comparison, registered-Vendor retrieval, Domain Events and `VendorRegistered`; and
- absence of every side effect, including Vendor creation, Compliance Requirement creation, Licence Details, evidence, persistence, events, outbox work and external regulatory calls.

### 3.3 Determination failures

Require behavioural coverage for:

- `DeterminationRequestValidationFailure` with every independently detectable controlling-input error;
- `InvalidReference`;
- `InvalidAddressResult`;
- `UnsupportedDetermination`; and
- `ComplianceDeterminationTemporarilyUnavailable`.

Every failure obligation shall verify the returned controlled outcome, retry guidance where defined, retained client draft where applicable and all prohibited side effects.

### 3.4 Catalogue structure

Add a dedicated subsection to HJ-107 Section 5 named:

```text
### 5.1 Required Licence Types Determination
```

Renumber the existing Section 5 subsections without changing their retained test obligations or stable `VR-*` identifiers.

Add an HJ-107-owned `VR-DETERMINATION-*` area for genuinely new determination obligations. Existing `VR-*` identifiers remain governed by the previous controlled HJ-107 as the sole stable-ID baseline and shall not be renumbered merely because the subsection order changes.

### 3.5 API Contract coverage

Derive API Contract obligations from the approved HJ-106 v2.1 Part B representation for:

- `POST /vendor-registration/required-licence-types`;
- the determination request shape and prohibition of extra authoritative fields;
- the `200 OK` complete determination response;
- canonical item order and explicit boolean results;
- the shared safe error envelope;
- determination validation, unsupported-coverage and temporary-unavailability mappings; and
- side-effect-free behaviour despite use of `POST`.

Treat these as normative API Contract obligations because HJ-106 v2.1 is Approved and CON-047 is Approved. Do not reinterpret the HTTP representation as a Domain rule.

### 3.6 Traceability, completeness and review

Amend the Coverage Summary, Requirement-to-Test Traceability Matrix, Completeness Analysis, Assumptions and Open Questions, Review Checklist and Regeneration Reconciliation so they explicitly include the third operation.

Every normative HJ-106 v2.1 Part A determination requirement shall map to one or more `VR-*` obligations, an explicit non-testable finding or a controlled blocked dependency. The HJ-107/HJ-013 responsibility boundary remains unchanged.

## 4. Preserved PR-004 Controls

The amendment does not change:

- HJ-106 Part A as the primary normative behavioural source;
- HJ-106 Part B treatment for API Contract obligations;
- HJ-006 Test Classification and Test Level mappings;
- the previous HJ-107 as the sole stable-ID baseline for `VR-*` identifiers;
- prohibition of `AI-*` identifiers and version-specific HJ-013 dependencies;
- the requirement to preserve, amend, add and retire Test IDs explicitly;
- Priority and Dependency Status classifications;
- the HJ-107/HJ-013 behavioural-versus-implementation responsibility boundary;
- prohibited-outcome verification;
- proposal-only generation and human review controls under PR-008; or
- the prohibition on executable test-code generation.

## 5. Explicit Non-decisions

CR-077 does not:

- change HJ-106 v2.1 or any approved business behaviour;
- add another business operation;
- change the Compliance applicability rules or Rule Set Version semantics;
- define Licence Details, evidence or Compliance Requirement lifecycle behaviour;
- move policy into Vendor Application, Vendor Domain, Address or the Web client;
- select test classes, frameworks, fixtures, mocks or implementation techniques;
- generate HJ-107, HJ-013 or executable tests; or
- authorise repository modification.

## 6. Required Follow-up

After CR-077 and the amended PR-004 are approved and applied:

1. run PR-004 against Approved HJ-106 v2.1 and the previous controlled HJ-107 stable-ID baseline;
2. generate the complete HJ-107 candidate and `VR-*` reconciliation;
3. approve and apply HJ-107 and its HJ-000 synchronization;
4. run PR-005 as the separate downstream iteration for HJ-013; and
5. do not begin implementation until the controlled catalogue sequence is complete.

## 7. Acceptance

CR-077 is accepted when the human approves this Change Request as authority to amend PR-004 exactly as described above, without changing the approved HJ-106 v2.1 service contract or the established HJ-107/HJ-013 responsibility boundary.

# CR-075 - Determine Required Licence Types and Defer Licence Details Capture

| Property | Value |
|---|---|
| Change Request | CR-075 |
| Status | Approved |
| Date | 18 September 2026 |
| Owner | Project Architecture |
| Affected artefact | HJ-011 Epic 1 Vendor Registration Implementation Scope |

## 1. Reason

CR-074 correctly established that the approved registration information is sufficient to determine which licence types are required. It also required Step 4 to collect structurally complete Licence Details and made those details a condition of progression and authoritative registration.

Decision Mode analysis has separated those two concerns. Steps 1 to 4 already capture the facts needed to determine the complete set of required licence types. The structure, evidence and validation rules for the corresponding Licence Details have not yet been established and belong to the later Compliance Domain work. Requiring structurally complete details before that model exists would invent a premature contract and incorrectly make Vendor Registration depend upon an undefined Compliance representation.

The Epic 1 implementation scope must therefore retain deterministic required-licence-type identification while deferring Licence Details capture and validation.

## 2. Decision Requested

Approve HJ-011 v2.16 with the following corrected outcome-level requirement.

### 2.1 Required Licence Types determination

Before the user may progress beyond Step 4, the journey shall determine and present the complete set of Required Licence Types from the approved registration information.

The determination shall remain consistent with the approved applicability dependencies: Food Business Registration is always required; Street Trading Licence applicability depends upon Trading Location and Business Address; Late Night Refreshment Licence applicability depends upon Weekly Opening Hours and Service Includes Hot Food; and alcohol-licensing applicability depends upon Alcohol Service.

Changing any controlling registration information shall invalidate the earlier result and cause the complete Required Licence Types set to be recalculated before progression. The Web client may support the interaction, but it shall not invent an alternative set or independently own the authoritative rules.

### 2.2 Deferred Licence Details

Epic 1 does not require Step 4 to collect Licence Details, evidence or documents. Missing Licence Details shall not prevent progression beyond Step 4 or cause an otherwise valid Vendor Registration to be rejected.

Later Compliance Domain work shall define the structure, evidence, validation and lifecycle of Licence Details and shall amend the affected registration steps and downstream artefacts before those details are captured. Determining a Required Licence Type does not assert that a corresponding licence exists, is authentic, is current, is valid or satisfies a Compliance Requirement.

### 2.3 Correction of CR-074

CR-074 remains the historical record of the decision approved on 18 September 2026. This CR supersedes only the parts of CR-074 that require:

- Licence Details to be present and structurally complete before progression beyond Step 4;
- authoritative Vendor Registration rejection when those details are absent or structurally incomplete; and
- immediate architectural propagation of a Licence Details capture model.

CR-074's post-registration community-participation requirement remains unchanged.

## 3. Controlled Change

Approve the accompanying complete HJ-011 v2.16 candidate as the amended Epic 1 implementation scope.

No other controlled artefact is changed by this CR. Approval corrects the source scope; it does not by itself select the ownership, operation, contract, persistence or publication architecture for Required Licence Types.

## 4. Required Follow-up

After approval and application:

1. reconcile CON-047 through PR-009 so that its statement, scope, required guarantee and verification treatment concern Required Licence Types determination rather than current Licence Details capture and gating;
2. resume Decision Mode for the reconciled CON-047;
3. resolve the authoritative ownership, rule representation, operation, result, failure and lifecycle boundaries for Required Licence Types;
4. propagate the approved decision through the Ubiquitous Language, Vendor and Compliance boundaries, registration fields and service contract, Application and HTTP contracts, Web journey and test catalogues as applicable; and
5. introduce and resolve later concerns for Licence Details structure, evidence, capture and validation as part of the Compliance Domain work before implementing those capabilities.

## 5. Explicit Non-Decisions

This CR does not decide:

- which bounded context or application capability owns Required Licence Types determination;
- the source, representation or versioning of the applicability rules;
- the command, result, route, JSON, persistence, fingerprint or integration-event treatment of the determined types;
- the Licence Details data model, evidence or document requirements;
- Licence Details capture, amendment, retention or validation behaviour;
- whether any licence exists, is authentic, is current or is valid;
- Compliance Requirement evaluation or satisfaction; or
- the detailed Step 4 visual treatment.

Those matters require explicit concern resolution and downstream propagation.

## 6. Acceptance

CR-075 is accepted when the human approves this Change Request and the accompanying HJ-011 v2.16 candidate as the authoritative Epic 1 scope source for:

- determining and presenting the complete Required Licence Types set from approved registration information before progression beyond Step 4;
- recalculating that set when controlling registration information changes; and
- deferring Licence Details structure, evidence, capture and validation to later Compliance Domain work without blocking Vendor Registration on their absence.

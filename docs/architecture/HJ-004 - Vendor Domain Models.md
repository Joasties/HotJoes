HJ-004 – Vendor Domain Models


# Revision History
# 
# Related Documents
# 

# 
# 1. Vendor Domain Analysis
## 1.1 Purpose
The Vendor domain manages the identity, registration, lifecycle and platform-controlled trading eligibility of businesses that sell food through HotJoes.
The initial implementation supports Vendor Registration while preserving a clean path to later Vendor Compliance, Menu and Ordering capabilities.
The model explicitly separates:
completion of Vendor Registration;
progression through the activation process;
regulatory and compliance requirements;
Vendor lifecycle state;
Vendor trading preference;
calculated operational availability.
## 1.2 Vendor Existence and Registration Boundary
A Vendor does not exist in the HotJoes platform until a complete and valid Vendor Registration has been successfully submitted.
Registration information entered before submission is transient.
It may be retained only for the lifetime of the registration session. If the registration session expires before successful submission:
the incomplete information is discarded;
no Vendor aggregate is created;
no Vendor lifecycle state exists;
the applicant must restart the registration process.
HotJoes does not persist registration drafts in the initial implementation.
This is a deliberate simplification based on the business rule that an applicant must complete the registration process in one registration session.
Successful submission creates the Vendor directly in:
VendorState = PendingActivation
TradingPreference = Offline
There is therefore no RegistrationProgress property or value object within the Vendor aggregate.
## 1.3 Mandatory Vendor Registration Information
A Vendor may be created only when all mandatory Vendor-domain registration information has been supplied and validated.
### Business identity
LegalOperatorType
LegalName
TradingName
CompanyRegistrationNumber, where required by the Legal Operator Type
### Primary contact
contact name;
email address;
telephone number.
### Trading location
validated TradingAddressId supplied by the Address domain.
### Registration declarations
confirmation that the applicant is authorised to register the business;
confirmation that the submitted information is accurate;
acceptance of applicable HotJoes platform terms.
Mandatory information is conditional upon the selected Legal Operator Type.
For example:

The exact supported Legal Operator Types and conditional validation rules must be defined in the Vendor Registration story and its acceptance criteria.
These rules must be enforced server-side. User-interface validation alone is insufficient.

## 1.4 Core Domain Responsibility
The Vendor domain is responsible for:
creating a Vendor after successful registration;
maintaining the Vendor’s legal and trading identity;
maintaining Vendor contact information;
maintaining references to Vendor addresses;
controlling the Vendor lifecycle;
controlling the Vendor’s trading preference;
enforcing lifecycle transition rules;
recording activation and deactivation decisions;
recording immediate and scheduled suspensions;
preserving suspension history;
publishing Vendor lifecycle events.
The Vendor domain is not responsible for:
persisting incomplete registration attempts;
searching for or validating postal addresses;
managing regulatory evidence;
deciding whether council registration evidence is genuine;
deciding whether a trading licence is valid;
managing menus;
managing opening hours;
accepting orders;
dispatching deliveries;
processing payments;
calculating complete operational availability.

## 1.5 Domain Boundaries
### Vendor Domain
Owns:
Vendor identity;
legal and trading names;
Legal Operator Type;
Vendor lifecycle state;
Vendor trading preference;
activation and deactivation transitions;
suspension decisions;
contact details;
references to addresses;
Vendor lifecycle history.
### Address Domain
Owns:
address search;
address validation;
canonical address data;
integration with third-party UK address-data providers.
The Vendor domain stores an AddressId.
It does not own postal address lookup or third-party address-provider integration.
### Vendor Compliance Domain
Owns:
compliance requirements;
regulatory evidence;
evidence submissions;
evidence verification;
council registration status;
trading-licence status;
compliance review outcomes;
expiry and renewal of regulatory evidence.
The Vendor domain consumes explicit compliance outcomes. It does not become the compliance rules engine.
### Menu Domain
Owns:
menus;
menu sections;
menu items;
prices;
item availability.
A Menu references a VendorId, but Menu entities are not part of the Vendor aggregate.
### Platform Administration
Owns administrative workflows and authorisation for:
immediate Vendor suspension;
scheduled Vendor suspension;
cancellation of a scheduled suspension;
Vendor reactivation;
administrative deactivation.
The Vendor aggregate still enforces the resulting lifecycle invariants.
### Operational Availability Read Model
Operational Availability is a composed read model.
It consumes authoritative information published by relevant bounded contexts.
The Vendor domain contributes:
VendorState;
TradingPreference.
Other domains may contribute:
opening-hours status;
exceptional closures;
menu availability;
ordering-service availability;
platform restrictions.
The Vendor aggregate does not directly reference or own entities from those domains.

## 1.6 Ubiquitous Language
### Vendor
A business or legal operator that has successfully completed Vendor Registration and exists within HotJoes.
### Vendor Registration
The submission of all mandatory Vendor information required to create a Vendor.
Registration is complete only when the information is successfully validated and submitted.
### Registration Session
The temporary interaction during which an applicant enters Vendor Registration information.
An expired registration session produces no Vendor.
### Pending Activation
The lifecycle condition of a successfully registered Vendor that has not yet satisfied all requirements for platform activation.
### Activation
The explicit transition of a Vendor from PendingActivation to Active.
Activation occurs only after all mandatory activation prerequisites are satisfied.
### Active
The lifecycle state of a Vendor that is permitted to participate in the HotJoes platform.
Active does not necessarily mean that the Vendor is currently accepting orders.
### Trading Preference
The Vendor-controlled indication of whether it presently intends to accept orders.
Values:
Online
Offline
### Operational Availability
The calculated answer to:
Can this Vendor accept an order at this moment?
Operational Availability is not a Vendor lifecycle state.
### Suspension
A platform-administrator action that temporarily prevents a Vendor from trading.
Suspension may be:
immediate;
scheduled to take effect at a future date and time.
When suspension takes effect, the Vendor’s Trading Preference is forced to Offline.
### Reactivation
An authorised platform decision that returns a suspended Vendor to Active.
Reactivation does not automatically return the Vendor to Online.
### Deactivation
The closure of the Vendor’s normal lifecycle relationship with HotJoes.
Deactivation requires a structured reason.
### Deactivation Reason
A business-classified explanation for why a Vendor was moved to Deactivated.
### Pending Activation Process
The long-running process that coordinates the actions required to move a Vendor from PendingActivation to either:
Active; or
Deactivated.

# 2. Vendor Aggregate
## 2.1 Aggregate Root
Vendor is the aggregate root.
The aggregate protects:
Vendor identity;
lifecycle-state transitions;
Trading Preference rules;
suspension invariants;
activation rules;
deactivation rules;
historical suspension data.
The aggregate does not contain:
incomplete registration drafts;
Menu entities;
Address entities;
compliance evidence;
council-registration documents;
trading-licence documents;
opening-hours schedules;
orders;
payment data.

## 2.2 Vendor Properties
### Identity
VendorId
### Business identity
LegalOperatorType
LegalName
TradingName
CompanyRegistrationNumber, where applicable
### Contact information
PrimaryContact
### Address references
TradingAddressId
RegisteredAddressId, where applicable
### Lifecycle
VendorState
TradingPreference
RegisteredAt
ActivatedAt
DeactivatedAt
DeactivationDecision, where applicable
### Suspension
ScheduledSuspension, where one exists
CurrentSuspension, while suspended
SuspensionHistory
### Concurrency
aggregate version or equivalent optimistic concurrency token.

## 2.3 Supporting Value Objects
### VendorId
Strongly typed identifier for a Vendor.
### LegalOperatorType
Classification of the legal person or organisation operating the Vendor.
Initial supported values must be defined by the Vendor Registration story.
### VendorName
Validated legal or trading name.
### CompanyRegistrationNumber
Validated company or organisational registration identifier where applicable.
### PrimaryContact
Contains:
contact name;
email address;
telephone number.
### EmailAddress
Validated email-address value.
### TelephoneNumber
Validated telephone-number value.
### AddressId
Reference to an Address owned by the Address domain.
### SuspensionReason
Contains:
structured suspension category;
explanatory note.
### ScheduledSuspension
Contains:
effective date and time;
suspension reason;
scheduling date and time;
administrator identifier.
### SuspensionRecord
Contains:
suspension identifier;
reason;
decision-maker;
effective date and time;
end date and time, where applicable.
### DeactivationDecision
Contains:
reason code;
explanation;
decision timestamp;
actor or automated process identifier;
whether reapplication is permitted;
earliest reapplication date, where applicable.

# 3. Vendor Lifecycle
## 3.1 Lifecycle States
PendingActivation
Active
Suspended
Deactivated
These states describe the Vendor’s platform lifecycle.
They must not be confused with:
compliance status;
council registration status;
trading-licence status;
Trading Preference;
opening-hours status;
operational availability.

## 3.2 Registration
A Vendor is created only after successful submission of complete and valid Vendor Registration information.
A newly created Vendor has:
VendorState = PendingActivation
TradingPreference = Offline
RegisteredAt = current timestamp
Registration completion is not stored as a separate ongoing state because every persisted Vendor has completed registration.

## 3.3 Pending Activation
PendingActivation means:
The Vendor has been successfully registered but has not yet satisfied every prerequisite required to become Active.
A Vendor in PendingActivation:
exists in the platform;
cannot trade;
must remain Offline;
may have outstanding compliance or administrative actions;
is managed through the Pending Activation Process.

## 3.4 Activation
A Vendor may transition from PendingActivation to Active only when the VendorActivationPolicy confirms that all mandatory prerequisites have been satisfied.
Activation is explicit.
Registration alone does not activate the Vendor.

## 3.5 Trading Preference
An Active Vendor may set itself:
Online;
Offline.
A Vendor may be Online only while its lifecycle state is Active.
A Vendor may voluntarily set itself Offline for:
holidays;
staff shortages;
equipment failure;
stock issues;
operational choice;
any other temporary reason.
Changing Trading Preference does not change the Vendor lifecycle state.

## 3.6 Immediate Suspension
An authorised platform administrator may suspend an Active Vendor immediately.
The transition:
records the suspension decision;
changes the Vendor state to Suspended;
forces Trading Preference to Offline;
raises VendorSuspended.
A Vendor cannot suspend itself.

## 3.7 Scheduled Suspension
An authorised platform administrator may schedule a suspension to begin at a future date and time.
A Vendor may have no more than one unresolved scheduled suspension at any time.
This is an intentional current business constraint, not an unaddressed scalability defect.
The administrative workflow must:
display any existing scheduled suspension;
prevent an administrator from creating a conflicting schedule;
require a structured reason;
record the responsible administrator.
The Vendor aggregate must independently enforce the single-scheduled-suspension invariant.
Before the effective time:
the Vendor remains in its current lifecycle state;
its Trading Preference is unchanged;
the scheduled decision is retained.
When the effective time is reached:
the Vendor transitions to Suspended;
Trading Preference is forced to Offline;
VendorSuspended is raised.

## 3.8 Reactivation
A suspended Vendor may return to Active only through an authorised platform decision.
Reactivation:
changes the lifecycle state to Active;
closes the current suspension record;
leaves Trading Preference as Offline;
raises VendorReactivated.
The Vendor must explicitly choose to return Online.

## 3.9 Deactivation
A Vendor may transition to Deactivated from:
PendingActivation;
Active;
Suspended.
Deactivation:
requires a structured reason;
forces Trading Preference to Offline;
records the decision;
prevents further trading;
preserves Vendor identity and historical records.
Deactivated is terminal for the normal lifecycle.
A future reinstatement capability must be designed explicitly and must not be assumed.

# 4. Pending Activation Process
## 4.1 Purpose
The Pending Activation Process coordinates the long-running work required to move a Vendor from PendingActivation to a terminal outcome.
The successful outcome is:
PendingActivation → Active
An unsuccessful outcome is:
PendingActivation → Deactivated
with an explicit reason code.

## 4.2 Responsibilities
The Pending Activation Process:
determines the activation requirements applicable to the Vendor;
obtains requirement status from the owning domains;
identifies actions assigned to the Vendor;
identifies actions assigned to HotJoes;
records Vendor-action deadlines;
sends initial notifications;
sends reminders;
sends final warnings;
reacts to compliance outcomes;
requests activation when all requirements are satisfied;
requests deactivation when closure conditions are met.
The process does not bypass the Vendor aggregate.
It issues explicit commands requesting lifecycle transitions.

## 4.3 Pending Activation Requirement
A requirement managed by the process contains:
RequirementId
RequirementType
ResponsibleParty
RequirementStatus
RequestedAt
DueAt
CompletedAt
LastNotificationAt
### Responsible Party
Vendor
HotJoes
ExternalAuthority
### Requirement Status
Outstanding
InProgress
Submitted
UnderReview
Satisfied
Failed
Waived
These statuses belong to the activation workflow, not the Vendor lifecycle.

## 4.4 Vendor Activation Policy
The VendorActivationPolicy answers:
Is this Vendor currently eligible to transition from PendingActivation to Active?
It may consider authoritative outcomes from:
Vendor Registration;
Vendor Compliance;
council registration;
trading-licence verification;
administrative approval;
platform restrictions.
The policy returns an explicit eligibility decision.
The Vendor aggregate performs the state transition only after approval.

## 4.5 Pending Activation Closure Policy
The PendingActivationClosurePolicy answers:
Should this Pending Activation process be closed, and what is the correct deactivation reason?
The policy must distinguish Vendor failure from delay caused by HotJoes or an external authority.
A Vendor must not be deactivated because:
HotJoes has not reviewed submitted evidence;
an external authority has not responded;
the Vendor has completed its assigned action and is awaiting a decision;
an authorised extension is active;
the matter is under formal review or dispute.

## 4.6 Vendor Non-Response
A Vendor non-response timer may begin only when:
a specific action has been assigned to the Vendor;
the required action has been clearly communicated;
a deadline has been provided;
the requirement remains outstanding;
no authorised extension exists.
The timer pauses or ceases when:
the Vendor responds;
the Vendor submits the required information;
responsibility transfers to HotJoes;
responsibility transfers to an external authority;
an extension is granted.
The reminder periods and final closure period are configurable business policy.
They must not be hard-coded into the Vendor aggregate.

## 4.7 Pending Activation Outcomes
### Successful activation
All mandatory requirements satisfied
    → VendorActivationPolicy approves
    → ActivateVendor command
    → VendorState = Active
### Compliance requirements not met
Mandatory compliance requirement fails
    → no permitted remediation remains
    → PendingActivationClosurePolicy approves closure
    → DeactivateVendor
    → reason = ComplianceRequirementsNotMet
### Vendor action not completed
Vendor responds but does not complete a mandatory action
    → permitted remediation period ends
    → PendingActivationClosurePolicy approves closure
    → DeactivateVendor
    → reason = VendorActionNotCompleted
### Vendor unresponsive
Vendor action requested
    → deadline expires
    → reminders issued
    → final warning issued
    → final deadline expires without response
    → PendingActivationClosurePolicy approves closure
    → DeactivateVendor
    → reason = VendorUnresponsive
### Administrative closure
Authorised administrator closes activation
    → DeactivateVendor
    → reason = AdministrativelyClosed
There is no self-service Vendor withdrawal command in the current scope.

# 5. Deactivation Reason Codes
The initial reason codes are:
ComplianceRequirementsNotMet
VendorActionNotCompleted
VendorUnresponsive
RegistrationRejected
AdministrativelyClosed
### ComplianceRequirementsNotMet
A mandatory compliance condition cannot be satisfied and no permitted remediation remains.
### VendorActionNotCompleted
The Vendor engaged with the process but failed to complete a mandatory assigned action within the permitted period.
### VendorUnresponsive
The Vendor failed to respond after the required notification, reminder and final-warning process.
### RegistrationRejected
Information discovered after registration means that the Vendor cannot proceed to activation.
This reason distinguishes rejection from inactivity or compliance failure.
### AdministrativelyClosed
An authorised administrator closes the activation process for a justified reason not represented by another code.
Detailed compliance failures remain in the Compliance domain.
The Vendor deactivation reason should not duplicate the entire compliance model.

# 6. Commands
Initial Vendor commands include:
RegisterVendor
UpdateVendorBusinessDetails
UpdateVendorPrimaryContact
ChangeVendorTradingAddress
ActivateVendor
SetVendorOnline
SetVendorOffline
ScheduleVendorSuspension
CancelScheduledVendorSuspension
SuspendVendorImmediately
ApplyScheduledVendorSuspension
ReactivateVendor
DeactivateVendor
Commands must express business intent.
Avoid generic commands such as:
UpdateVendorStatus
SetState
PatchVendor
Those commands bypass business rules and weaken the domain model.

# 7. Domain Events
Likely Vendor domain events include:
VendorRegistered
VendorBusinessDetailsChanged
VendorPrimaryContactChanged
VendorTradingAddressChanged
VendorActivated
VendorSetOnline
VendorSetOffline
VendorSuspensionScheduled
VendorScheduledSuspensionCancelled
VendorSuspended
VendorReactivated
VendorDeactivated
Scheduling and applying a suspension are separate business facts:
VendorSuspensionScheduled
VendorSuspended
They must not be represented by a single event.

# 8. Aggregate Invariants
The Vendor aggregate must enforce:
A Vendor can be created only from complete and valid submitted registration data.
A newly created Vendor begins in PendingActivation.
A newly created Vendor begins Offline.
Incomplete registration data does not create a Vendor.
A PendingActivation Vendor cannot trade.
A Vendor may be Online only while Active.
A suspended Vendor must be Offline.
A deactivated Vendor must be Offline.
Activation must be explicit.
Registration does not itself prove activation eligibility.
Suspension requires an authorised administrator and a reason.
A scheduled suspension must have a future effective date and time.
A Vendor may have no more than one unresolved scheduled suspension.
When suspension takes effect, Trading Preference is forced to Offline.
Reactivation does not restore Online.
Deactivation requires a structured reason.
A PendingActivation Vendor must not be deactivated because HotJoes or an external authority is responsible for an outstanding action.
External domains cannot directly mutate Vendor lifecycle state.
Historical suspension information must not be destroyed.
State transitions must occur through explicit domain operations.

# 9. Vendor Domain Model Diagram
classDiagram
    direction LR

    class Vendor {
        <<Aggregate Root>>
        +VendorId Id
        +LegalOperatorType LegalOperatorType
        +VendorName LegalName
        +VendorName TradingName
        +CompanyRegistrationNumber? CompanyRegistrationNumber
        +PrimaryContact PrimaryContact
        +AddressId TradingAddressId
        +AddressId? RegisteredAddressId
        +VendorState State
        +TradingPreference TradingPreference
        +DateTimeOffset RegisteredAt
        +DateTimeOffset? ActivatedAt
        +DateTimeOffset? DeactivatedAt
        +ScheduledSuspension? ScheduledSuspension
        +CurrentSuspension? CurrentSuspension
        +register()
        +updateBusinessDetails()
        +updatePrimaryContact()
        +changeTradingAddress()
        +activate()
        +setOnline()
        +setOffline()
        +scheduleSuspension()
        +cancelScheduledSuspension()
        +suspendImmediately()
        +applyScheduledSuspension()
        +reactivate()
        +deactivate()
    }

    class VendorId {
        <<Value Object>>
        +Guid Value
    }

    class LegalOperatorType {
        <<Enumeration or Reference Data>>
    }

    class VendorName {
        <<Value Object>>
        +string Value
    }

    class CompanyRegistrationNumber {
        <<Value Object>>
        +string Value
    }

    class PrimaryContact {
        <<Value Object>>
        +PersonName Name
        +EmailAddress Email
        +TelephoneNumber Telephone
    }

    class EmailAddress {
        <<Value Object>>
        +string Value
    }

    class TelephoneNumber {
        <<Value Object>>
        +string Value
    }

    class AddressId {
        <<External Reference>>
        +Guid Value
    }

    class VendorState {
        <<Enumeration>>
        PendingActivation
        Active
        Suspended
        Deactivated
    }

    class TradingPreference {
        <<Enumeration>>
        Online
        Offline
    }

    class ScheduledSuspension {
        <<Value Object>>
        +DateTimeOffset EffectiveFrom
        +SuspensionReason Reason
        +DateTimeOffset ScheduledAt
        +AdministratorId ScheduledBy
    }

    class SuspensionRecord {
        <<Historical Record>>
        +SuspensionId Id
        +SuspensionReason Reason
        +DateTimeOffset EffectiveFrom
        +DateTimeOffset? EndedAt
        +AdministratorId DecidedBy
    }

    class SuspensionReason {
        <<Value Object>>
        +SuspensionCategory Category
        +string Explanation
    }

    class DeactivationDecision {
        <<Value Object>>
        +DeactivationReason Reason
        +string Explanation
        +DateTimeOffset DecidedAt
        +ActorId DecidedBy
        +bool ReapplicationPermitted
        +DateTimeOffset? ReapplicationFrom
    }

    class PendingActivationProcess {
        <<Process Manager>>
        +evaluateRequirements()
        +requestVendorAction()
        +sendReminder()
        +sendFinalWarning()
        +requestActivation()
        +requestDeactivation()
    }

    class PendingActivationRequirement {
        <<Process State>>
        +RequirementId Id
        +RequirementType Type
        +ResponsibleParty ResponsibleParty
        +RequirementStatus Status
        +DateTimeOffset RequestedAt
        +DateTimeOffset? DueAt
        +DateTimeOffset? CompletedAt
        +DateTimeOffset? LastNotificationAt
    }

    class VendorActivationPolicy {
        <<Domain Policy>>
        +evaluateEligibility()
    }

    class PendingActivationClosurePolicy {
        <<Domain Policy>>
        +evaluateClosure()
    }

    class ActivationEligibility {
        <<Value Object>>
        +bool IsEligible
        +EligibilityFailure[] Failures
    }

    class ClosureDecision {
        <<Value Object>>
        +bool ShouldClose
        +DeactivationReason? Reason
    }

    class AddressDomain {
        <<External Domain>>
        +searchAddress()
        +validateAddress()
        +getAddress()
    }

    class VendorCompliance {
        <<External Domain>>
        +ComplianceStatus
        +CouncilRegistrationStatus
        +TradingLicenceStatus
    }

    class OperationalAvailability {
        <<Composed Read Model>>
        +VendorState VendorState
        +TradingPreference TradingPreference
        +bool WithinOpeningHours
        +bool MenuAvailable
        +bool OrderingAvailable
        +bool PlatformRestricted
        +bool CanAcceptOrders
    }

    class Menu {
        <<Separate Aggregate>>
        +MenuId Id
        +VendorId VendorId
    }

    Vendor *-- VendorId
    Vendor *-- LegalOperatorType
    Vendor *-- VendorName
    Vendor o-- CompanyRegistrationNumber
    Vendor *-- PrimaryContact
    Vendor *-- VendorState
    Vendor *-- TradingPreference
    Vendor o-- ScheduledSuspension
    Vendor o-- SuspensionRecord
    Vendor o-- DeactivationDecision

    ScheduledSuspension *-- SuspensionReason
    SuspensionRecord *-- SuspensionReason
    PrimaryContact *-- EmailAddress
    PrimaryContact *-- TelephoneNumber

    Vendor --> AddressId
    AddressId ..> AddressDomain : identifies address in

    PendingActivationProcess o-- PendingActivationRequirement
    PendingActivationProcess --> VendorActivationPolicy
    PendingActivationProcess --> PendingActivationClosurePolicy
    PendingActivationProcess ..> Vendor : issues lifecycle commands

    VendorActivationPolicy --> ActivationEligibility
    PendingActivationClosurePolicy --> ClosureDecision
    VendorActivationPolicy ..> VendorCompliance : consumes authoritative status
    PendingActivationClosurePolicy ..> VendorCompliance : consumes failure outcomes

    OperationalAvailability ..> Vendor : consumes state and preference
    OperationalAvailability ..> Menu : consumes availability
    Menu --> VendorId : references

# 10. Vendor Lifecycle State Model
stateDiagram-v2
    [*] --> PendingActivation : Successful Vendor Registration

    PendingActivation --> Active : Activate\n[all activation requirements satisfied]

    PendingActivation --> Deactivated : Compliance requirements not met
    PendingActivation --> Deactivated : Vendor action not completed
    PendingActivation --> Deactivated : Vendor unresponsive
    PendingActivation --> Deactivated : Registration rejected
    PendingActivation --> Deactivated : Administrative closure

    Active --> Suspended : Immediate suspension
    Active --> Suspended : Scheduled suspension becomes effective
    Active --> Deactivated : Administrative deactivation

    Suspended --> Active : Authorised reactivation
    Suspended --> Deactivated : Administrative deactivation

    Deactivated --> [*]

    note right of PendingActivation
        Vendor exists but cannot trade.
        Outstanding activation requirements
        are managed by the Pending Activation Process.
    end note

    note right of Active
        Vendor is permitted to participate.
        Active does not necessarily mean Open.
    end note

    note right of Suspended
        Suspension is platform controlled.
        Trading Preference is forced Offline.
    end note

    note right of Deactivated
        Terminal for the normal lifecycle.
        A structured reason is mandatory.
    end note

# 11. Trading Preference Model
stateDiagram-v2
    [*] --> Offline

    Offline --> Online : Vendor chooses Online\n[VendorState = Active]
    Online --> Offline : Vendor chooses Offline

    Online --> Offline : Suspension takes effect
    Online --> Offline : Vendor deactivated
    Online --> Offline : Platform restriction applied

    Offline --> Offline : Vendor activated
    Offline --> Offline : Vendor reactivated
    Offline --> Offline : Vendor suspended
    Offline --> Offline : Vendor deactivated

    note right of Online
        Online represents Vendor intent.
        It does not prove current operational availability.
    end note

    note right of Offline
        Offline may be voluntary or enforced.
    end note

# 12. Pending Activation Process Model
stateDiagram-v2
    [*] --> RequirementsEvaluated : Vendor registered

    RequirementsEvaluated --> WaitingForVendor : Vendor action required
    RequirementsEvaluated --> WaitingForHotJoes : HotJoes action required
    RequirementsEvaluated --> WaitingForExternalAuthority : External action required
    RequirementsEvaluated --> ReadyForActivation : All requirements satisfied
    RequirementsEvaluated --> ClosureRequired : Terminal failure identified

    WaitingForVendor --> RequirementsEvaluated : Vendor completes action
    WaitingForVendor --> WaitingForVendor : Reminder issued
    WaitingForVendor --> FinalWarning : Response deadline exceeded

    FinalWarning --> RequirementsEvaluated : Vendor responds
    FinalWarning --> ClosureRequired : Final deadline exceeded

    WaitingForHotJoes --> RequirementsEvaluated : Review completed
    WaitingForExternalAuthority --> RequirementsEvaluated : Authority outcome received

    ReadyForActivation --> Activated : Activation policy approves
    ClosureRequired --> Deactivated : Closure policy approves

    Activated --> [*]
    Deactivated --> [*]

    note right of WaitingForVendor
        Vendor inactivity timing runs only
        while the Vendor owns an actionable requirement.
    end note

    note right of WaitingForHotJoes
        The Vendor cannot be penalised
        for delay owned by HotJoes.
    end note

    note right of WaitingForExternalAuthority
        External delay does not count
        as Vendor non-response.
    end note
The states in this diagram are process-manager states. They are not Vendor lifecycle states.

# 13. State Transition Table
# 

# 14. Operational Availability
Operational Availability is calculated outside the Vendor aggregate.
A conceptual calculation is:
CanAcceptOrders =
    VendorState == Active
    AND TradingPreference == Online
    AND WithinOpeningHours
    AND MenuAvailable
    AND OrderingCapabilityAvailable
    AND NoPlatformRestriction
Each contributing value must come from an explicit published event, query contract or maintained read-model projection.
The calculation must not obtain data by directly querying another domain’s internal persistence model.


# 15. Scheduled Suspension Sequence
sequenceDiagram
    actor Administrator
    participant AdminUI as Platform Administration
    participant Application as Vendor Application
    participant Vendor as Vendor Aggregate
    participant Store as Vendor Repository
    participant Scheduler as Suspension Processor
    participant Bus as Event Publisher

    Administrator->>AdminUI: Schedule Vendor suspension
    AdminUI->>Application: ScheduleVendorSuspension

    Application->>Store: Load Vendor
    Application->>Vendor: scheduleSuspension(effectiveFrom, reason, administrator)

    alt Existing unresolved scheduled suspension
        Vendor-->>Application: Reject command
        Application-->>AdminUI: Scheduling conflict
    else Schedule accepted
        Vendor-->>Application: VendorSuspensionScheduled
        Application->>Store: Save Vendor
        Application->>Bus: Publish VendorSuspensionScheduled
    end

    Note over Scheduler: At or after EffectiveFrom

    Scheduler->>Store: Load due Vendor
    Scheduler->>Vendor: applyScheduledSuspension(now)
    Vendor-->>Scheduler: VendorSuspended
    Scheduler->>Store: Save Vendor
    Scheduler->>Bus: Publish VendorSuspended
The scheduled processor must be idempotent.
Repeated processing must not:
apply the same suspension more than once;
create duplicate suspension history;
publish misleading duplicate business events.

# 16. Design Decisions
## Decision 1: No persisted registration draft
Incomplete registration information is transient and discarded when the registration session expires.
## Decision 2: A Vendor exists only after complete registration
Successful registration creates the Vendor directly in PendingActivation.
## Decision 3: RegistrationProgress is removed
Every persisted Vendor has completed registration. Registration completion is therefore not an ongoing aggregate property.
## Decision 4: Lifecycle and compliance remain separate
Compliance, council registration and trading-licence statuses do not become Vendor lifecycle states.
## Decision 5: Pending Activation is managed as a process
The Pending Activation Process coordinates requirements, notifications, deadlines and outcomes.
## Decision 6: Activation and closure use explicit policies
VendorActivationPolicy and PendingActivationClosurePolicy provide explicit decisions.
## Decision 7: Vendor failure and platform delay are distinguished
A Vendor cannot be deactivated for inactivity while HotJoes or an external authority owns the outstanding action.
## Decision 8: No self-service withdrawal
A Vendor withdrawal command is not included because no business requirement has established it.
## Decision 9: Suspension is platform controlled
Only an authorised platform administrator may schedule, cancel or immediately apply a suspension.
## Decision 10: One unresolved scheduled suspension
A Vendor may have no more than one unresolved scheduled suspension at a time.
## Decision 11: Suspension forces Offline
This is a Vendor aggregate invariant.
## Decision 12: Reactivation leaves the Vendor Offline
The Vendor must deliberately choose to resume trading.
## Decision 13: Operational Availability is a composed read model
It uses explicit contracts from contributing bounded contexts and is not owned by the Vendor aggregate.
## Decision 14: Deactivation requires a reason
All transitions to Deactivated require a structured business reason and decision metadata.

# 17. Initial Implementation Scope
The Vendor Registration epic should implement:
a transient registration session;
entry and validation of mandatory Vendor information;
conditional validation based on Legal Operator Type;
address selection through the Address domain;
successful creation of a Vendor;
initial lifecycle state of PendingActivation;
initial Trading Preference of Offline;
persistence of the Vendor aggregate;
publication of VendorRegistered;
retrieval of the registered Vendor.
The first epic should not implement:
persisted registration drafts;
the full Compliance domain;
council-registration workflows;
trading-licence workflows;
the complete Pending Activation Process;
suspension scheduling infrastructure;
Menu management;
opening-hours management;
Operational Availability composition.
Those capabilities are represented in the model so that the Vendor Registration implementation does not block their later introduction.

# 18. Remaining Review Questions
The following require future business decisions but do not block Vendor Registration:
Which Legal Operator Types are supported initially?
What exact fields are mandatory for each Legal Operator Type?
What Pending Activation deadlines and reminder periods apply?
Which actions permit extensions?
Who may approve an extension?
What evidence makes a compliance failure terminal?
When may a deactivated Vendor reapply?
Is a reinstatement process ever required after deactivation?
What retention period applies to suspension and deactivation records?
Which bounded context will own opening hours?
These are explicit future decisions, not reasons to expand the initial Vendor Registration epic.

| Document ID | HJ-004 |
|---|---|
| Document Title | Vendor Domain Models |
| Version | 1.0 |
| Status | Approved |
| Classification | Model |
| Owner | Project Architecture |
| Last Updated | 17 July 2026 |

| Version | Date | Description |
|---|---|---|
| 0.1 | 16 July 2026 | Previous draft. |
| 0.2 | 16 July 2026 | Updated Vendor Domain Models |
| 1.0 | 17 July 2026 | Applied the standard HotJoes document metadata, revision history, related documents and numbered heading structure. Architectural principles and decision checklist retained unchanged. |

| Document ID | Title | Status |
|---|---|---|
| HJ-001 | HotJoes Project Vision | Draft |
| HJ-002 | Architectural Principles | Draft |
| HJ-003 | Ubiquitous Language Guide | Draft |

| Legal Operator Type | Legal Name | Trading Name | Company Registration Number |
|---|---|---|---|
| Sole Trader | Required | Required | Not normally required |
| Limited Company | Required | Required | Required |
| Partnership | Required | Required | Dependent upon partnership type |
| Registered Charity | Required | Required | Charity or company number as applicable |
| Other Organisation | Required | Required | Dependent upon legal form |

| Current lifecycle state | Command or trigger | Conditions | Resulting state | Trading Preference |
|---|---|---|---|---|
| None | Register Vendor | Complete, valid registration submitted | PendingActivation | Offline |
| PendingActivation | Activate Vendor | Activation policy approves | Active | Offline |
| PendingActivation | Deactivate Vendor | Valid closure decision and reason | Deactivated | Offline |
| Active | Set Vendor Online | No effective platform restriction | Active | Online |
| Active | Set Vendor Offline | Always permitted | Active | Offline |
| Active | Schedule Suspension | Authorised administrator; no existing unresolved schedule; future date; reason supplied | Active | Unchanged |
| Active | Cancel Scheduled Suspension | Authorised administrator; suspension not effective | Active | Unchanged |
| Active | Suspend Immediately | Authorised administrator; reason supplied | Suspended | Offline |
| Active | Apply Scheduled Suspension | Effective time reached | Suspended | Offline |
| Active | Deactivate Vendor | Authorised decision and reason | Deactivated | Offline |
| Suspended | Reactivate Vendor | Authorised; blocking issue resolved | Active | Offline |
| Suspended | Deactivate Vendor | Authorised decision and reason | Deactivated | Offline |
| Deactivated | Any normal lifecycle command | Not permitted | Deactivated | Offline |

| Vendor state | Trading Preference | Other conditions | Operational result |
|---|---|---|---|
| PendingActivation | Offline | Any | Unavailable |
| Active | Online | All satisfied | Open |
| Active | Online | Opening hours closed | Closed |
| Active | Online | Menu unavailable | Unavailable |
| Active | Offline | Any | Offline |
| Suspended | Offline | Any | Suspended |
| Deactivated | Offline | Any | Deactivated |
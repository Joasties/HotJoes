# CR-073 - Define Weekly Opening Hours in the Vendor Domain Model

| Property | Value |
|---|---|
| Change Request | CR-073 |
| Status | Approved |
| Date | 14 September 2026 |
| Owner | Project Architecture |
| Affected artefact | HJ-004 Vendor Domain Models |

## 1. Reason

The approved Vendor model represents Opening Hours as one Start Time and End Time interval. The Epic 1 Vendor Registration journey requires a complete Monday-to-Sunday schedule, including different hours by day, closed days and businesses that trade continuously for a full day.

The current model cannot preserve that registration intent. Treating equal Start and End times as 24-hour opening would also be ambiguous.

## 2. Decision Requested

Replace the single Opening Hours interval within Trading Characteristics with one immutable `WeeklyOpeningHours` value object.

`WeeklyOpeningHours` contains exactly seven immutable `DailyOpeningHours` entries: one for each controlled `TradingDay` from Monday through Sunday. Days are unique and retained in deterministic Monday-to-Sunday order.

Each `DailyOpeningHours` contains:

- `Day`;
- `IsClosed`;
- `IsOpenAllDay`;
- optional `StartTime`; and
- optional `EndTime`.

`OpenDuringInterval` is not a separate state. It is implied when `IsClosed` and `IsOpenAllDay` are both false and both times are present.

## 3. Daily Invariants

Exactly one of the following states is valid:

| State | IsClosed | IsOpenAllDay | StartTime | EndTime |
|---|---:|---:|---|---|
| Closed | true | false | absent | absent |
| Open All Day | false | true | absent | absent |
| Timed interval | false | false | present | present |

The following are invalid:

- `IsClosed` and `IsOpenAllDay` both true;
- either state flag combined with Start Time or End Time;
- a timed interval with only one time;
- neither state flag and neither time; and
- a timed interval whose Start Time equals its End Time.

When equal times are entered, the validation outcome shall communicate:

> Start Time and End Time cannot be the same. Select Open All Day if the business trades for 24 hours.

An End Time earlier than Start Time remains valid and represents overnight operation.

## 4. Epic 1 Boundaries

- One interval per Trading Day is supported.
- Split shifts are outside scope.
- Exceptional and holiday opening hours are outside scope.
- Cross-day overlap analysis is outside Vendor Registration scope.
- Everyday is a client interaction shortcut, not a Domain state. It expands into seven equivalent daily entries before registration.
- Retrieval returns the authoritative Weekly Opening Hours value. A client may collapse seven identical entries into an Everyday presentation.

## 5. Controlled Change

Approve HJ-004 v2.9 as the amended Vendor Domain Model.

No other controlled artefact is changed by this CR. Approval establishes the Domain requirement only.

## 6. Required Follow-up

After approval, enter Decision Mode to identify every affected downstream artefact and resolve its representation deliberately. The review must include, without presupposing changes:

- ubiquitous language and registration information definitions;
- Application command, validation, semantic identity and retrieval representations;
- HTTP request, response, validation-error and OpenAPI contracts;
- the unreleased VendorRegistered v1 contract;
- persistence structure and migration treatment;
- Compliance consumer validation;
- behavioural and architecture test catalogues;
- Step 4 Registration Session and UI behaviour; and
- baseline-manifest synchronization.

## 7. Explicit Non-Decision

This CR does not select JSON member names, event compatibility treatment, database tables or columns, migration mechanics, UI control design or implementation types. Those decisions follow from the approved Domain model through Decision Mode.

## 8. Acceptance

CR-073 is accepted when the human approves this Change Request and the accompanying HJ-004 v2.9 candidate as the authoritative Weekly Opening Hours Domain model.

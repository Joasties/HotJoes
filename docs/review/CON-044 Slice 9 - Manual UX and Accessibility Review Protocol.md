# CON-044 Slice 9 — Manual UX and Accessibility Review Protocol

| Field | Value |
|---|---|
| Primary concern | CON-044 |
| Approved cohort | CON-044; CON-045 |
| Slice | 9 — Structured manual UX and accessibility review evidence |
| Verification form | Contract Review / Non-executable review |
| Delivery mode | Manual File Handoff |
| Repository authority | Proposal Only |
| Applicable obligations | AI-BROWSER-009; AI-UX-002–004; AI-UX-007–008 |

## 1. Purpose

This protocol records the human evidence that automated accessibility checks, browser emulation and pixel comparison cannot establish. It evaluates the implemented Epic 1 Vendor Registration journey for understandable operation, keyboard and representative assistive-technology use, responsive comprehension and visual coherence.

This review does not replace the green automated suites, redefine approved business behaviour or claim complete WCAG conformance from one assistive-technology/browser combination.

## 2. Reviewer responsibilities

The reviewer shall:

1. record the exact build, runtime and review environment;
2. perform every applicable activity rather than inferring it from automation;
3. use synthetic information only;
4. record observed outcomes as Pass, Finding, Not Applicable or Blocked;
5. create a finding for every unexpected outcome;
6. avoid recording credentials, complete payloads or real personal information;
7. explicitly approve or reject the visual baseline; and
8. sign off only after all findings have a disposition.

Codex may guide the procedure and help reconcile the record. It must not impersonate the reviewer or mark unobserved outcomes as passed.

## 3. Review environment

Record:

- reviewer name;
- review date and timezone;
- Git commit or precise working-tree baseline;
- operating system and version;
- Chromium version;
- Playwright/browser baseline version for comparison;
- display resolution and scaling;
- keyboard layout;
- VoiceOver/macOS version;
- runtime identity or controlled environment description; and
- whether browser extensions other than required assistive tools were disabled.

Use the controlled disposable browser/demo runtime and synthetic deterministic registration data. Do not use a publicly exposed or production environment.

## 4. Review contexts

Perform the review in these contexts:

| Context | Required treatment |
|---|---|
| Desktop | Chromium, 1440 × 900 viewport, pointer and keyboard |
| Compact | Chromium, 390 × 844 emulated viewport; label explicitly as emulation |
| Zoom | Desktop Chromium at 200% browser zoom |
| Keyboard | Complete representative journey without pointer interaction |
| Assistive technology | macOS VoiceOver with Chromium across representative material states |

Do not record compact emulation as physical-device evidence. Do not claim other screen readers, browsers or operating systems were reviewed.

## 5. Common criteria

At every applicable state, assess:

- purpose and instructions are understandable;
- headings and regions convey a meaningful structure;
- labels, roles, names, values and relationships are understandable through VoiceOver;
- keyboard order follows the intended reading and interaction order;
- focus is visible, predictable and never lost;
- errors, warnings, progress and completion are discoverable without colour alone;
- essential meaning does not depend on imagery, hover, animation or screen position;
- compact width and 200% zoom preserve information and completion;
- controls are not clipped, obscured or unreachable;
- visual hierarchy, spacing, alignment and typography are coherent;
- displayed information agrees with the entered information; and
- authoritative registration success remains distinct from optional Community participation.

## 6. Review journey

### 6.1 Step 1 — Trading Location

Review the initial state and each location choice.

- Confirm the page purpose and Step 1 position are clear.
- Traverse choices with keyboard only.
- Confirm focus and selected state are visible and announced.
- Confirm imagery is supplementary and accessible names convey the choices.
- Compare the governed desktop state with the approved Step 1 baseline.
- At compact width and 200% zoom, confirm every choice remains readable and operable.

### 6.2 Step 2 — Legal Operator

- Confirm the heading, descriptions and choices are understandable.
- Traverse and select an option by keyboard.
- Confirm selected state and resulting navigation are predictable.
- Confirm images do not carry essential meaning alone.
- Check desktop, compact and zoomed presentation.

### 6.3 Step 3 — Business Identity and Address

Review empty, validation, search-result, selected-address and populated states.

- Submit or leave required fields to expose validation.
- Confirm errors identify the affected control and are discoverable with VoiceOver.
- Search for `Hot Joes` and confirm results are distinguishable selectable controls.
- Select the Blackheath Avenue result and confirm selection is announced and visible.
- Confirm keyboard-only Address search and selection.
- Confirm dependent Address information behaves predictably if cleared or changed.
- Review telephone, email, Trading Name and Legal Operator Name labels and errors.
- Where applicable, review conditional Company Registration Number treatment.
- Check desktop, compact and zoomed presentation without horizontal loss.

### 6.4 Step 4 — Trading Characteristics

Review the Everyday state, Custom Hours dialog, service choices, determination and optional profile dialogs.

#### Custom Hours

- Open the dialog by keyboard and confirm its accessible name.
- Confirm focus enters the dialog and remains contained with Tab and Shift+Tab.
- Review Monday–Sunday day, start, end, All Day and Closed relationships with VoiceOver.
- Exercise timed, Closed and Open All Day states.
- Enter equal Start and End Time and confirm guidance is announced and not colour-only.
- Change a value and Cancel; confirm nothing is committed and focus returns to the opener.
- Reopen, save a mixed schedule and confirm focus and page context remain predictable.
- Confirm the compact Monday–Sunday summary exactly reflects saved values.
- Activate Edit and confirm saved values are restored.
- Repeat representative dialog operation at compact width and 200% zoom.

#### Remaining Step 4 states

- Review hot-food and alcohol choices, selection state and warning treatment.
- Confirm licence determination progress and results are understandable.
- Confirm failure information, if reviewed, preserves entered information and offers a safe next action.
- Review Add Licence placeholder without inferring completed Compliance functionality.
- Review Website and Business Description dialogs, including validation and return focus.

### 6.5 Step 5 — Registration Review

- Confirm entered business, contact, trading and Weekly Opening Hours information is easy to compare.
- Confirm Edit/Review actions identify their purpose and return to the intended step.
- Confirm Registration Declarations begin unchecked.
- Traverse and select declarations by keyboard.
- Confirm Terms and Conditions link treatment is understandable.
- Confirm submission remains disabled until the required declarations are selected.
- Observe the submitting state and ensure duplicate action is prevented.
- Review a controlled validation failure if available without altering authoritative rules.
- Confirm definitive success is announced and not inferred optimistically.
- Check desktop, compact and zoomed layouts.

### 6.6 Step 6 — Community choice

- Confirm completed Vendor Registration remains unambiguous.
- Confirm Email is initially selected and Keep me involved is initially unticked.
- Confirm Join the community is disabled until Keep me involved is selected.
- Traverse Email, SMS and WhatsApp by keyboard and VoiceOver.
- Confirm Not Now communicates a separate optional decision and performs no Community request.
- Review a controlled Community failure and confirm selections remain present for retry.
- Review successful join and not-joined confirmation outcomes.
- Confirm wording does not imply Communication Consent or delivery evidence beyond approved scope.

### 6.7 Registered-Vendor retrieval

- Retrieve the Vendor using the returned Vendor ID through the rendered interface.
- Confirm identity, Address and mixed Weekly Opening Hours are understandable and agree with registration.
- Confirm heading, region and reading order are meaningful with VoiceOver.
- Check desktop, compact and zoomed presentation.

## 7. Visual-baseline review

Record:

- baseline: `registration-step-1-desktop-chromium-{platform}.png`;
- viewport: 1440 × 900;
- interaction state: initial Step 1 state;
- whether the current baseline is approved without replacement;
- every observed material difference;
- accepted/rejected/deferred reasoning; and
- confirmation that no tolerance was increased to hide an unexplained difference.

Review the remainder of the journey for visual coherence even where no pixel baseline exists. Business and accessibility authority take precedence over mockup similarity.

## 8. Finding severity

| Severity | Meaning |
|---|---|
| Critical | Prevents completion or creates serious misrepresentation, accessibility or safety failure |
| Major | A required journey or important assistive/keyboard operation cannot be completed reliably |
| Moderate | Material difficulty, ambiguity or inconsistency with a viable workaround |
| Minor | Limited visual or usability defect that does not prevent completion |

Critical and Major findings prevent Slice 9 completion. Moderate and Minor findings require explicit disposition.

## 9. Evidence handling

- Record concise observations, not complete registration data.
- Use synthetic identifiers if correlation is necessary.
- Do not attach unrestricted traces, request bodies, serialized events, credentials or connection strings.
- Screenshots must contain synthetic information and be retained only when they materially support a finding or approved visual review.
- Identify any screenshot by state and context; do not treat it as standalone conformance evidence.

## 10. Completion

The review is complete only when every required context and state is recorded, all findings are dispositioned, the baseline decision is explicit, evidence limitations are acknowledged and the reviewer provides final sign-off.

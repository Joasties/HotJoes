# CON-044 Slice 9 — Manual UX and Accessibility Findings

This register accompanies `CON-044 Slice 9 - Manual UX and Accessibility Review Result - 2026-10-02.md`. Findings must not be deleted after resolution; closure evidence is appended to the relevant row or its detailed entry.

## Findings summary

| ID | Context | Category | Severity | Status | Required disposition |
|---|---|---|---|---|---|
| F-001 | Steps 1–6 information controls | Pointer target, focus indication and visual consistency | Moderate | Open | Align visible and interactive geometry; use the same circular design throughout; verify pointer and keyboard focus. |
| F-002 | Steps 1 and 2 choice confirmation | Keyboard interaction and navigation | Major | Open | Separate radio selection from progression with an explicit Continue action; verify arrows, Back and re-confirmation. |
| F-003 | Step 3 | Natural focus order | Moderate | Open | Place Address Search and result selection before Back and Continue in DOM order without positive `tabindex`. |
| F-004 | Step 4 | Natural focus order | Moderate | Open | Place Website and Business Description before Back and Confirm and continue in DOM order without positive `tabindex`. |
| F-005 | Community outcomes | Completed-journey lifecycle | Moderate | Open | Prevent browser Back from restoring an actionable Community choice after either terminal outcome. |
| F-006 | Step 1 at 200% zoom | Responsive visual composition | Minor | Open | Review and improve the stacked choice proportions so the vertically aligned rectangular controls retain a deliberate visual treatment. |
| F-007 | Step 3 at 200% zoom | Responsive reflow and field usability | Moderate | Open | Stack Primary Contact and Business Address Search so fields retain adequate usable width. |
| F-008 | Steps 4–6 at 200% zoom | Initial scroll/focus position | Moderate | Open | Ensure each route opens at its page heading/top rather than part-way down the page. |

## F-001 — Information-control geometry and consistency

**Affected states:** Steps 1–6.

**Observation:** The invisible interactive area used to expose each `(i)` explanation is positioned too far to the right of its visible symbol. Steps 3, 5 and 6 use a square interactive target while Steps 1 and 2 establish a circular visual treatment. At 200% zoom, the control is disproportionately large on every reviewed page, exaggerating both the displacement and inconsistent shape.

**Impact:** Pointer location does not correspond reliably with the visible affordance, and the control design is inconsistent across a single journey. Keyboard focus may describe an area different from the apparent control.

**Expected behaviour:** The visible `(i)`, pointer target, keyboard focus outline and accessible button occupy the same circular area on every step.

**Authority affected:** AI-UX-007; AI-UX-008.

**Verification required:** Pointer activation at the visible icon centre and boundary; visible keyboard focus; accessible name and explanation relationship; proportionate sizing at 100% and 200%; desktop, compact and 200% zoom inspection across Steps 1–6.

**Closure evidence:** _Pending._

## F-006 — Step 1 loses visual composition at 200% zoom

**Affected state:** Step 1 Trading Location at 200% browser zoom.

**Observation:** The three choices correctly change from a horizontal to a vertical arrangement, but their horizontal rectangular proportions diminish the intended card-like visual effect when stacked.

**Impact:** The choices remain present, but the design becomes less coherent and visually expressive at the required zoom context.

**Expected behaviour:** Reflow may stack the choices, but their proportions, spacing and imagery should retain a deliberate and balanced selectable-card presentation.

**Authority affected:** AI-UX-004; AI-UX-007.

**Disposition required:** Confirm whether revised card proportions are required or explicitly accept the current zoom presentation as a conscious design trade-off.

**Closure evidence:** _Pending._

## F-007 — Step 3 does not reflow into usable-width sections at 200% zoom

**Affected state:** Step 3 Contact and Business Details at 200% browser zoom.

**Observation:** Primary Contact and Business Address Search remain side-by-side. Each column and its fields therefore have substantially less horizontal space for entering and reviewing field content.

**Impact:** Data-entry fields become unnecessarily constrained and provide less room to inspect entered details at a required zoom context.

**Expected behaviour:** At 200% zoom, Primary Contact/Business Details and Business Address Search stack into a single-column reading and interaction sequence with usable field widths.

**Authority affected:** AI-UX-004; AI-UX-007; AI-UX-008.

**Verification required:** Visual and keyboard completion at 200%; no horizontal page clipping; readable field values and address results; natural focus order remains as required by F-003.

**Closure evidence:** _Pending._

## F-008 — Steps 4–6 open part-way down the page at 200% zoom

**Affected states:** Steps 4, 5 and 6 at 200% browser zoom.

**Observation:** On arrival, each route begins approximately half-way down its page rather than presenting the page heading and top content.

**Impact:** The user can miss the page title, instructions and earlier controls, and may not understand that content exists above the initial viewport. This materially disrupts orientation and reading order.

**Expected behaviour:** Route arrival starts at the page top or moves focus to the page heading without producing an inappropriate scroll offset. Zoom must not preserve or introduce a mid-page position from the preceding interaction.

**Authority affected:** AI-UX-003; AI-UX-004; AI-UX-008.

**Verification required:** Navigate forward and backward into Steps 4–6 at 200%; verify heading/top visibility, appropriate focus, predictable scroll restoration, and no regression at 100% or compact width.

**Closure evidence:** _Pending._

## F-002 — Radio selection incorrectly performs navigation

**Affected states:** Step 1 Trading Location and Step 2 Legal Operator.

**Reproduction:**

1. Tab to the information control and then into the radio group.
2. Press Right Arrow to explore the next option.
3. Observe immediate navigation to the next step.
4. Use Back to return.
5. Attempt to activate the already-selected value.
6. Observe that no new `change` event occurs and progression requires selecting a different value first.

**Impact:** Keyboard users cannot explore the complete native radio group before confirming a choice. Returning users cannot confirm an unchanged choice reliably. The behaviour conflicts with expected native radio-group navigation.

**Expected behaviour:** Arrow keys move through the radio group while remaining on the current page. Selection updates the choice only. A separate Continue button performs progression and remains usable when the existing choice is retained.

**Authority affected:** AI-UX-002; AI-UX-003; AI-UX-008.

**Verification required:** Tab enters the radio group once; arrows traverse all choices; selected state is visible; Continue confirms; Back preserves the value; unchanged selection can be reconfirmed; pointer behaviour remains correct.

**Closure evidence:** _Pending._

## F-003 — Step 3 navigation precedes Address interaction

**Affected state:** Step 3 Contact and Business Details.

**Observation:** Tab order correctly traverses Contact name, Telephone number, Email address, Trading name and Legal operator name, but then reaches Back and Continue before Business Address Search and address-result selection.

**Impact:** Keyboard focus order does not follow the intended data-entry and reading sequence. Users encounter progression controls before completing the required Address interaction.

**Expected behaviour:** Information control → contact fields → Vendor fields → Address Search → result selection → Back → Continue.

**Authority affected:** AI-UX-002; AI-UX-003; AI-UX-008.

**Required implementation constraint:** Correct natural DOM order; do not use positive `tabindex` values.

**Closure evidence:** _Pending._

## F-004 — Step 4 navigation precedes optional profile controls

**Affected state:** Step 4 Trading Characteristics.

**Observation:** Back and Confirm and continue occur in the keyboard sequence before Add/Edit Website and Add/Edit Business Description.

**Impact:** The focus order presents page completion before the remaining editable content and does not match the intended journey order.

**Expected behaviour:** Complete opening-hours, service and determination interaction → Website → Business Description → Back → Confirm and continue.

**Authority affected:** AI-UX-002; AI-UX-003; AI-UX-008.

**Required implementation constraint:** Correct natural DOM order; do not use positive `tabindex` values.

**Closure evidence:** _Pending._

## F-005 — Terminal Community outcome permits actionable re-entry

**Affected states:** Not-joined and successful-join outcomes.

**Observation:** Both outcome pages otherwise have correct headings, messages, focus behaviour and keyboard reachability. Browser Back returns to the completed Step 6 Community choice and permits another selection attempt.

**Impact:** The UI presents a completed interaction as though it remains amendable. Equivalent submission is protected by replay semantics, but a changed preference may surface a conflict inconsistent with the approved terminal flow.

**Expected behaviour:** After either terminal outcome, browser history must not restore an actionable Community Participation form. Re-entry resolves to the existing outcome or another approved terminal location.

**Authority affected:** AI-UX-003; AI-UX-007; approved CON-046 Community lifecycle.

**Verification required:** Complete Not Now and Join paths separately; invoke browser Back; confirm Step 6 cannot be resubmitted or amended; verify normal forward navigation and refresh behaviour.

**Closure evidence:** _Pending._

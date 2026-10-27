import AxeBuilder from "@axe-core/playwright";
import { expect, test, type Locator, type Page } from "@playwright/test";
import {
  demonstrationIdentity,
  registerRepresentativeVendor,
  type RepresentativeJourneyState,
} from "../support/representative-demonstration";

test("AI-UX-002 representative material journey states have no detectable WCAG 2.2 A or AA violation", async ({ page }, testInfo) => {
  const visited: RepresentativeJourneyState[] = [];
  const vendor = await registerRepresentativeVendor(
    page,
    demonstrationIdentity(testInfo),
    async (state, currentPage) => {
      await assertNoAutomatedViolation(currentPage, state);
      visited.push(state);
    },
  );

  await page.goto(`/registered-vendors/${vendor.vendorId}`);
  await expect(page.getByRole("heading", { name: "Registered Vendor details" }))
    .toBeVisible();
  await assertNoAutomatedViolation(page, "registered-vendor-retrieval");

  expect(visited).toEqual([
    "trading-location",
    "legal-operator",
    "business-identity",
    "trading-characteristics",
    "registration-review",
    "registration-confirmation",
  ]);
});

test("AI-UX-004 declared emulated context preserves the representative journey without horizontal page clipping", async ({ page }, testInfo) => {
  const vendor = await registerRepresentativeVendor(
    page,
    demonstrationIdentity(testInfo),
    async (state, currentPage) => {
      await expect(currentPage.getByRole("main")).toBeVisible();
      expect(
        await currentPage.evaluate(() =>
          document.documentElement.scrollWidth <= document.documentElement.clientWidth),
        `${state} must reflow within the declared emulated viewport.`,
      ).toBe(true);
    },
  );

  await page.goto(`/registered-vendors/${vendor.vendorId}`);
  await expect(page.getByRole("heading", { name: "Registered Vendor details" }))
    .toBeVisible();
  expect(await page.evaluate(() =>
    document.documentElement.scrollWidth <= document.documentElement.clientWidth,
  )).toBe(true);
});

test("AI-UX-005/008 Custom Hours is keyboard-operable, transactional, focus-safe and visibly confirmable", async ({ page }, testInfo) => {
  let verified = false;
  await registerRepresentativeVendor(
    page,
    demonstrationIdentity(testInfo),
    async (state, currentPage) => {
      if (state !== "trading-characteristics") return;

      const opener = currentPage.getByRole("button", { name: "Edit" });
      await expect(opener).toBeVisible();
      await opener.focus();
      await currentPage.keyboard.press("Enter");

      const dialog = currentPage.getByRole("dialog", { name: "Customise Hours" });
      await expect(dialog).toBeVisible();
      await expect(dialog.getByLabel("Monday open all day")).toBeFocused();

      await currentPage.keyboard.press("Shift+Tab");
      await expectFocusWithin(dialog);
      await currentPage.keyboard.press("Tab");
      await expectFocusWithin(dialog);

      await dialog.getByLabel("Tuesday start time").fill("12:00");
      await dialog.getByRole("button", { name: "Cancel" }).click();
      await expect(dialog).toBeHidden();
      await expect(opener).toBeFocused();

      await opener.click();
      await expect(dialog.getByLabel("Tuesday start time")).toHaveValue("09:00");

      await dialog.getByLabel("Monday closed").uncheck();
      await expect(dialog.getByLabel("Monday start time")).toBeEnabled();
      await expect(dialog.getByLabel("Monday end time")).toBeEnabled();
      await dialog.getByLabel("Monday start time").fill("09:00");
      await dialog.getByLabel("Monday end time").fill("09:00");
      await dialog.getByRole("button", { name: "Save hours" }).click();
      await expect(dialog.getByText("Start and end times must differ"))
        .toBeVisible();

      await dialog.getByLabel("Monday open all day").check();
      await expect(dialog.getByLabel("Monday start time")).toBeDisabled();
      await expect(dialog.getByLabel("Monday end time")).toBeDisabled();
      await dialog.getByRole("button", { name: "Save hours" }).click();
      await expect(dialog).toBeHidden();

      const summary = currentPage.getByLabel("Custom weekly opening hours");
      await expect(summary).toContainText("MonOpen all day");
      await expect(summary).toContainText("Tue09:00 to 17:00");
      await expect(opener).toBeVisible();
      await expect(opener).toBeInViewport();
      expect(await currentPage.evaluate(() =>
        document.documentElement.scrollWidth <= document.documentElement.clientWidth,
      )).toBe(true);

      await assertNoAutomatedViolation(currentPage, "trading-characteristics");
      verified = true;
    },
  );
  expect(verified).toBe(true);
});

async function expectFocusWithin(dialog: Locator): Promise<void> {
  await expect.poll(() => dialog.evaluate((element) =>
    element.contains(document.activeElement),
  )).toBe(true);
}

async function assertNoAutomatedViolation(
  page: Page,
  state: RepresentativeJourneyState | "registered-vendor-retrieval",
): Promise<void> {
  const results = await new AxeBuilder({ page })
    .withTags(["wcag2a", "wcag2aa", "wcag21a", "wcag21aa", "wcag22aa"])
    .analyze();

  expect(
    results.violations.map((violation) => violation.id),
    `${state}: automation complements rather than replaces manual review.`,
  ).toEqual([]);
}
